/* Script inicial para crear estructura de tablas relacionadas con el 
   módulo de punto de venta.
*/

USE ControlRefaccionaria
GO

-- 4. Búsqueda y búsqueda avanzada
ALTER TABLE ParteImagen ADD Orden INT
GO
UPDATE ParteImagen SET Orden = o.Orden FROM
	ParteImagen pi
	INNER JOIN (
		SELECT
			ParteImagenID
			, ROW_NUMBER() OVER (PARTITION BY ParteID ORDER BY ParteImagenID) AS Orden
		FROM ParteImagen
		) o ON o.ParteImagenID = pi.ParteImagenID
ALTER TABLE ParteImagen ALTER COLUMN Orden INT NOT NULL
GO

BEGIN TRAN
BEGIN TRY

-- 1. Indentificar vehículo

-- Va con el punto 6

-- 5. Comisiones

-- Va con las tablas de ventas. A analizar

-- 7. ePoints

-- Pendiente. A ser analizado por Isidro

-- 8. Equivalentes

-- 9. Cotizaciones (Condiciones proveedores)

-- Va con punto 11

-- 10. Alta de Clientes

CREATE TABLE Cliente (
	ClienteID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Nombre NVARCHAR(64) NOT NULL
	, ApellidoPaterno NVARCHAR(32)
	, ApellidoMaterno NVARCHAR(32)
	, Calle NVARCHAR(32)
	, NumeroExterior NVARCHAR(8)
	, NumeroInterior NVARCHAR(8)
	, Colonia NVARCHAR(32)
	, CodigoPostal NVARCHAR(8)
	, Ciudad NVARCHAR(32)  -- rev
	, Estado NVARCHAR(32)  -- rev
	, Telefono NVARCHAR(16)
	, Celular NVARCHAR(16)
	, RFC NVARCHAR(16)
	, CalleFacturacion NVARCHAR(32)
	, NumeroExteriorFacturacion NVARCHAR(8)
	, NumeroInteriorFacturacion NVARCHAR(8)
	, ColoniaFacturacion NVARCHAR(32)
	, CodigoPostalFacturacion NVARCHAR(8)
	, CiudadFacturacion NVARCHAR(32)  -- rev
	, EstadoFacturacion NVARCHAR(32)  -- rev

	, ListaDePrecios INT NOT NULL
	, TieneCredito BIT NOT NULL
	, Credito DECIMAL(12, 2)
	, ePoints INT

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

INSERT INTO Cliente (Nombre, ListaDePrecios, TieneCredito, UsuarioID) VALUES
	('Ventas mostrador', 1, 0, 1)

-- 11. 9500

CREATE INDEX Ix_NumeroParte ON Parte(NumeroParte)

CREATE TABLE CatEstatusGenerico (
	EstatusID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Descripcion NVARCHAR(16) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)
INSERT INTO CatEstatusGenerico (Descripcion, UsuarioID) VALUES
	('Pendiente', 1)
	, ('Completado', 1)
	, ('Cancelado', 1)

CREATE TABLE Cotizacion9500 (
	Cotizacion9500ID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, EstatusID INT NOT NULL FOREIGN KEY REFERENCES CatEstatusGenerico(EstatusID)
	, Fecha DATETIME
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, NombreDelCliente NVARCHAR(128) NOT NULL
	, Celular NVARCHAR(16)
	, Telefono NVARCHAR(16)
	, Anticipo DECIMAL(12, 2) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

CREATE TABLE Cotizacion9500Detalle (
	Cotizacion9500DetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ProveedorID INT NOT NULL FOREIGN KEY REFERENCES Proveedor(ProveedorID)
	, LineaID INT NOT NULL FOREIGN KEY REFERENCES Linea(LineaID)
	, MarcaParteID INT NOT NULL FOREIGN KEY REFERENCES MarcaParte(MarcaParteID)
	, ParteID INT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, Cantidad INT NOT NULL
	, Descripcion NVARCHAR(256) NOT NULL
	, Costo DECIMAL(12, 2) NOT NULL
	, PrecioAlCliente DECIMAL(12, 2) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

-- 12. Flotilla del Cliente

CREATE TABLE ClienteFlotilla (
	ClienteFlotillaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ClienteID INT NOT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
	, NumeroEconomico NVARCHAR(8)
	, MotorID INT NOT NULL FOREIGN KEY REFERENCES Motor(MotorID)
	, Anio INT NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

-- 13. Reporte a compras de faltante

CREATE TABLE ReporteDeFaltante (
	ReporteDeFaltanteID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, UsuarioRegistro INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, CantidadRequerida INT
	, Comentario NVARCHAR(512)

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

-- 14. Reimprimir Venta

CREATE TABLE VentaEstatus (
	VentaEstatusID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Descripcion NVARCHAR(32) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)
INSERT INTO VentaEstatus (Descripcion, UsuarioID) VALUES
	('Completada', 1)
	, ('Cancelada', 1)

CREATE TABLE Venta (
	VentaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Fecha DATETIME NOT NULL
	, ClienteID INT NOT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
	, VentaEstatusID INT NOT NULL FOREIGN KEY REFERENCES VentaEstatus(VentaEstatusID)
	, Subtotal DECIMAL (12, 2) NOT NULL
	, Iva DECIMAL (12, 2) NOT NULL
	, Total DECIMAL (12, 2) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

CREATE TABLE VentaFactura (
	VentaFacturaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
	, Fecha DATETIME NOT NULL
	, Factura NVARCHAR(8) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

CREATE TABLE VentaDetalle (
	VentaDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, Cantidad INT NOT NULL
	, PrecioUnitario DECIMAL(12, 2) NOT NULL
	, Iva DECIMAL(12, 2) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
END CATCH