/* Script inicial para crear estructura de tablas relacionadas con el 
   módulo de punto de venta. Archivo 2
*/

USE ControlRefaccionaria
GO

BEGIN TRAN
BEGIN TRY

/* *****************************************************************************
** Creación de tablas
***************************************************************************** */

-- Punto 00. Venta en sí

INSERT INTO VentaEstatus (Descripcion, UsuarioID) VALUES
	('Realizada', 1)
	, ('9500', 1)

CREATE TABLE NotaDeCredito (
	NotaDeCreditoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, FechaDeEmision DATETIME NOT NULL
	, Importe DECIMAL(12, 2) NOT NULL
	, ClienteID INT NOT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
	, Valida BIT NOT NULL
	, FechaDeUso DATETIME
	, Observacion NVARCHAR(512)
	
	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

CREATE TABLE VentaPago (
	VentaPagoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, VentaID INT NOT NULL FOREIGN KEY REFERENCES Venta(VentaID)
	, TipoPagoID INT NOT NULL FOREIGN KEY REFERENCES TipoPago(TipoPagoID)
	, Fecha DATETIME NOT NULL
	, Importe DECIMAL(12, 2) NOT NULL

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

ALTER TABLE TipoFormaPago ADD CONSTRAINT Df_FechaRegistro DEFAULT GETDATE() FOR FechaRegistro
INSERT INTO TipoFormaPago (NombreTipoFormaPago, UsuarioID) VALUES
	('NO IDENTIFICADO', 1)
	, ('NOTA DE CRÉDITO', 1)

CREATE TABLE VentaPagoDetalle (
	VentaPagoDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, VentaPagoID INT NOT NULL FOREIGN KEY REFERENCES VentaPago(VentaPagoID)
	, TipoFormaPagoID INT NOT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
	, Importe DECIMAL(12, 2) NOT NULL
	, BancoID INT FOREIGN KEY REFERENCES Banco(BancoID)
	, Folio NVARCHAR(32)
	, Cuenta NVARCHAR(8)

	, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, FechaModificacion DATETIME
	, Estatus BIT NOT NULL DEFAULT 1
	, Actualizar BIT NOT NULL DEFAULT 1
)

-- Punto 11. 9500

DELETE FROM CatEstatusGenerico
DBCC CHECKIDENT (CatEstatusGenerico, RESEED, 0)
INSERT INTO CatEstatusGenerico (Descripcion, UsuarioID) VALUES
	('Realizado', 1)
	, ('Pendiente', 1)
	, ('Completado', 1)
	, ('Cancelado', 1)

	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
END CATCH

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

-- 10. Clientes
ALTER TABLE Cliente ADD DiasDeCredito INT
GO

-- 11. 9500
ALTER TABLE Cotizacion9500Detalle ALTER COLUMN ParteID INT NOT NULL
GO
ALTER TABLE Cotizacion9500Detalle ADD Cotizacion9500ID INT NOT NULL FOREIGN KEY REFERENCES Cotizacion9500(Cotizacion9500ID)
GO
ALTER TABLE Cotizacion9500Detalle DROP COLUMN Descripcion
GO

-- 12. Flotilla del Cliente
ALTER TABLE ClienteFlotilla ADD
	Placa NVARCHAR(8)
	, Color NVARCHAR(16)
GO

/* *****************************************************************************
** Crear Vistas
***************************************************************************** */

-- Para mostrar detalles de clientes

-- DROP VIEW VentasView
CREATE VIEW VentasView AS
	SELECT
		v.VentaID
		, v.Fecha
		, v.ClienteID
		, ve.Descripcion AS Estatus
		, v.Subtotal
		, v.Iva
		, v.Total
		, ISNULL(SUM(vp.Importe), 0.00) AS Pagado
	FROM
		Venta v
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
		LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
	WHERE v.Estatus = 1
	GROUP BY
		v.VentaID
		, v.Fecha
		, v.ClienteID
		, ve.Descripcion
		, v.Subtotal
		, v.Iva
		, v.Total
GO

CREATE VIEW VentasPagosDetalleView AS
	SELECT
		vp.VentaID
		, vp.Fecha
		, vp.Importe AS ImporteTotal
		, c.ClienteID
		, tp.NombreTipoPago AS TipoDePago
		, tfp.NombreTipoFormaPago AS FormaDePago
		, vpd.Importe
		, b.NombreBanco
		, vpd.Folio
		, vpd.Cuenta
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c.ClienteID AND c.Estatus = 1
		LEFT JOIN TipoPago tp ON tp.TipoPagoID = vp.TipoPagoID AND tp.Estatus = 1
		LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
	WHERE vp.Estatus = 1
GO

-- 10. Clientes

-- DROP VIEW ClientesAdeudosView
CREATE VIEW ClientesAdeudosView AS
	SELECT
		c.ClienteID
		, v.VentaID
		, v.Fecha
		, v.Total
		, v.Pagado
		, v.Total - v.Pagado AS Restante
	FROM
		VentasView v
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
	WHERE (v.Total > v.Pagado)
GO