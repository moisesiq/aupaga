/* Script con modificaciones para el módulo de ventas. Archivo 5
*/

USE ControlRefaccionaria
GO

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Imágenes de partes
	ALTER TABLE ParteImagen ADD CONSTRAINT Cn_ParteImagen_ParteID_Orden UNIQUE (ParteID, Orden)

	-- Facturación

	-- Autorizaciones
	
	CREATE TABLE AutorizacionProceso (
		AutorizacionProcesoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Descricpion NVARCHAR(64) NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	INSERT INTO AutorizacionProceso (Descricpion, UsuarioID) VALUES
		('Venta a crédito no aplicable', 1)         -- 1
		, ('9500 sin anticipo', 1)                  -- 2
		, ('Devolución / Cancelación', 1)           -- 3
		, ('Fondo de caja si hay diferencia', 1)    -- 4
		, ('Gastos', 1)                             -- 5
		, ('Otros ingresos', 1)                     -- 6
		, ('Característica de Venta', 1)            -- 7
		, ('Refuerzo', 1)                           -- 8
		, ('Resguardo', 1)                          -- 9

	CREATE TABLE Autorizacion (
		AutorizacionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, AutorizacionProcesoID INT NOT NULL FOREIGN KEY REFERENCES AutorizacionProceso(AutorizacionProcesoID)
		, Tabla NVARCHAR(64) NOT NULL
		, TablaRegistroID INT NOT NULL
		, Autorizado BIT NOT NULL
		, AutorizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaAutorizo DATETIME NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	
	-- Caja
	
	CREATE TABLE CajaVistoBueno (
		CajaVistoBuenoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Tabla NVARCHAR(64) NOT NULL
		, TablaRegistroID INT NOT NULL
		, UsuarioVistoBuenoID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, Fecha DATETIME NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	CREATE TABLE CajaTipoIngreso (
		CajaTipoIngresoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, NombreTipoIngreso NVARCHAR(64) NOT NULL
		, Seleccionable BIT NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	INSERT INTO CajaTipoIngreso (NombreTipoIngreso, Seleccionable, UsuarioID) VALUES
		('Refuerzo caja', 0, 1)
		, ('Ingreso por Cobranza', 1, 1)
		, ('Cambio por ventas', 1, 1)

	CREATE TABLE CajaTipoEgreso (
		CajaTipoEgresoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, NombreTipoEgreso NVARCHAR(64) NOT NULL
		, Seleccionable BIT NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	INSERT INTO CajaTipoEgreso (NombreTipoEgreso, Seleccionable, UsuarioID) VALUES
		('Resguardo', 0, 1)
		, ('Sueldo Don', 1, 1)
		, ('Sueldo Isi', 1, 1)
		, ('Gasto Matriz', 1, 1)
		, ('Gasto Suc02', 1, 1)
		, ('Gasto Suc03', 1, 1)
		, ('Proveedores', 1, 1)
		, ('Paquetería', 1, 1)
		, ('Donativos', 1, 1)
		, ('Sueldos', 1, 1)

	CREATE TABLE CajaIngreso (
		CajaIngresoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, CajaTipoIngresoID INT NOT NULL FOREIGN KEY REFERENCES CajaTipoIngreso(CajaTipoIngresoID)
		, Concepto NVARCHAR(128) NULL
		, Importe DECIMAL(12, 2) NOT NULL
		, Fecha DATETIME NOT NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	CREATE TABLE CajaEgreso (
		CajaEgresoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, CajaTipoEgresoID INT NOT NULL FOREIGN KEY REFERENCES CajaTipoEgreso(CajaTipoEgresoID)
		, Concepto NVARCHAR(128) NULL
		, Importe DECIMAL(12, 2) NOT NULL
		, Fecha DATETIME NOT NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	CREATE TABLE CajaEfectivoPorDia (
		CajaEfectivoPorDiaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Dia DATE NOT NULL UNIQUE
		, Inicio DECIMAL(12, 2) NOT NULL
		, InicioUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, Cierre DECIMAL(12, 2) NULL
		, CierreUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	
	CREATE TABLE VentaCambio (
		VentaCambioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaPagoID INT NOT NULL FOREIGN KEY REFERENCES VentaPago(VentaPagoID)
		, Fecha DATETIME NOT NULL
		, FormasDePagoAntes NVARCHAR(16) NULL
		, FormasDePagoDespues NVARCHAR(16) NULL
		, RealizoIDAntes INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, RealizoIDDespues INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, ComisionistaIDAntes INT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
		, ComisionistaIDDespues INT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	
	-- VentaPago
	ALTER TABLE VentaPago DROP COLUMN Importe

	-- Venta
	ALTER TABLE Venta ADD ComisionistaClienteID INT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)

	-- Cliente
	ALTER TABLE Cliente ADD Alias NVARCHAR(16) NULL

	-- ParteVehiculo
	ALTER TABLE ParteVehiculo ALTER COLUMN MotorID INT NULL
	ALTER TABLE ParteVehiculo ALTER COLUMN Anio INT NULL

	EXEC sp_rename 'ReporteDeFaltante.UsuarioRegistro', 'RealizoUsuarioID', 'COLUMN'

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Caja.Resguardo.Ideal', '4000', '4000', 'Importe ideal de efectivo que debe haber en Caja.')
		, ('Caja.Resguardo.Detonante', '8000', '8000', 'Importe de efectivo que al ser alcanzado, se mostrará aviso de que se requiere hacer un Resguardo.')
		, ('Caja.CambioTurno.RutaArchivos', 'Archivos\CajaCambioTurno\', 'Archivos\CajaCambioTurno\', 'Ruta donde se almacenan los archivos (Pdfs) correspondientes a los cambios de turno de caja.')
		, ('Caja.Corte.RutaArchivos', 'Archivos\CajaCorte\', 'Archivos\CajaCorte\', 'Ruta donde se almacenan los archivos (Pdfs) correspondientes a los cortes de caja.')

	-- *** Insert que tal vez se deba quitar **
	-- DBCC CHECKIDENT(TipoFuente, RESEED, 3)
	INSERT INTO TipoFuente (NombreTipoFuente, UsuarioID, FechaRegistro) VALUES
		('MOSTRADOR', 1, GETDATE())

	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
END CATCH

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

-- Caja
ALTER TABLE VentaDevolucion ADD TipoFormaPagoID INT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
ALTER TABLE VentaDevolucion ADD EsCancelacion BIT NULL
GO
UPDATE VentaDevolucion SET TipoFormaPagoID = 2
ALTER TABLE VentaDevolucion ALTER COLUMN TipoFormaPagoID INT NOT NULL

-- ParteVehiculo
ALTER TABLE ParteVehiculo ADD ModeloID INT NULL FOREIGN KEY REFERENCES Modelo(ModeloID)
ALTER TABLE ParteVehiculo ADD RegistroUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
GO
UPDATE ParteVehiculo SET
	ModeloID = mt.ModeloID
	, RegistroUsuarioID = pv.UsuarioID
FROM
	ParteVehiculo pv
	LEFT JOIN Motor mt ON mt.MotorID = pv.MotorID
ALTER TABLE ParteVehiculo ALTER COLUMN ModeloID INT NOT NULL
ALTER TABLE ParteVehiculo ALTER COLUMN RegistroUsuarioID INT NOT NULL

GO

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */

CREATE FUNCTION fnuLlenarIzquierda(@Texto NVARCHAR(128), @Llenado NCHAR, @Caracteres INT)
RETURNS NVARCHAR(256)
AS BEGIN
	RETURN REPLICATE(@Llenado, @Caracteres - LEN(@Texto)) + @Texto
END
GO

/* *****************************************************************************
** Crear Vistas
***************************************************************************** */

-- Caja

-- DROP VIEW VentasDevolucionesView
CREATE VIEW VentasDevolucionesView AS
	SELECT
		vd.VentaDevolucionID
		, CONVERT(BIT, 0) AS EsFactura
		, dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), vd.VentaID), '0', 7) AS Folio
		, v.Fecha AS FechaDeVenta
		, vd.Fecha
		, vd.SucursalID
		, CASE WHEN vd.EsCancelacion = 1 THEN 'CANC-' ELSE '    -DEV' END AS Tipo
		, CASE WHEN vd.TipoFormaPagoID = 2 THEN 'EF-' ELSE '  -NC' END AS Salida
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombrePersona, 'PENDIENTE') AS Autorizo
		, vdd.Total
	FROM
		VentaDevolucion vd
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'VentaDevolucion' AND a.TablaRegistroID = vd.VentaDevolucionID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN (
			SELECT
				VentaDevolucionID
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDevolucionDetalle
			WHERE Estatus = 1
			GROUP BY VentaDevolucionID
		) vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID
	WHERE vd.Estatus = 1
GO

-- DROP VIEW VentasPagosView
CREATE VIEW VentasPagosView AS
	SELECT
		vp.VentaPagoID
		, vp.VentaID
		, CONVERT(BIT, 0) AS EsFactura
		, dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), vp.VentaID), '0', 7) AS Folio
		, vp.Fecha
		, (SELECT SUM(Importe) FROM VentaPagoDetalle WHERE VentaPagoID = vp.VentaPagoID AND Estatus = 1) AS Importe
		, vp.TipoPagoID
		, vp.SucursalID
		, c.ClienteID
		, v.VentaEstatusID
		-- , v.
		, c.Nombre AS Cliente
		, u.NombrePersona AS Vendedor
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
	WHERE vp.Estatus = 1
GO

-- DROP VIEW CajaIngresosView
CREATE VIEW CajaIngresosView AS
	SELECT
		ci.CajaIngresoID
		, ci.CajaTipoIngresoID
		, cti.NombreTipoIngreso AS Tipo
		, ci.Concepto
		, ci.Importe
		, ci.Fecha
		, ci.SucursalID
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombrePersona, 'Pendiente') AS Autorizo
	FROM
		CajaIngreso ci
		LEFT JOIN CajaTipoIngreso cti ON cti.CajaTipoIngresoID = ci.CajaTipoIngresoID AND cti.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'CajaIngreso' AND a.TablaRegistroID = ci.CajaIngresoID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
	WHERE ci.Estatus = 1
GO

-- DROP VIEW CajaEgresosView
CREATE VIEW CajaEgresosView AS
	SELECT
		ce.CajaEgresoID
		, ce.CajaTipoEgresoID
		, cte.NombreTipoEgreso AS Tipo
		, ce.Concepto
		, ce.Importe
		, ce.Fecha
		, ce.SucursalID
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombrePersona, 'Pendiente') AS Autorizo
	FROM
		CajaEgreso ce
		LEFT JOIN CajaTipoEgreso cte ON cte.CajaTipoEgresoID = ce.CajaTipoEgresoID AND cte.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'CajaEgreso' AND a.TablaRegistroID = ce.CajaEgresoID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
	WHERE ce.Estatus = 1
GO

CREATE VIEW Cotizaciones9500View AS
	SELECT
		c9.Cotizacion9500ID
		, c9.EstatusGenericoID
		, c9.Fecha
		, c9.SucursalID
		, c9.Anticipo
		, c9.ClienteID
		, c.Nombre AS Cliente
	FROM
		Cotizacion9500 c9
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
	WHERE c9.Estatus = 1
GO

-- DROP VIEW VentasCambiosView
CREATE VIEW VentasCambiosView AS
	SELECT
		vc.VentaCambioID
		, vc.VentaPagoID
		, v.VentaID
		, CONVERT(BIT, 0) AS EsFactura
		, dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), v.VentaID), '0', 7) AS Folio
		, vc.Fecha
		, c.Nombre AS Cliente
		, vc.FormasDePagoAntes
		, vc.FormasDePagoDespues
		, vc.RealizoIDAntes
		, ua.NombreUsuario AS VendedorAntes
		, vc.RealizoIDDespues
		, ud.NombreUsuario AS VendedorDespues
		, vc.ComisionistaIDAntes
		, cca.Alias AS ComisionistaAntes
		, vc.ComisionistaIDDespues
		, ccd.Alias AS ComisionistaDespues
	FROM
		VentaCambio vc
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vc.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = vc.RealizoIDAntes AND ua.Estatus = 1
		LEFT JOIN Usuario ud ON ud.UsuarioID = vc.RealizoIDDespues AND ud.Estatus = 1
		LEFT JOIN Cliente cca ON cca.ClienteID = vc.ComisionistaIDAntes AND cca.Estatus = 1
		LEFT JOIN Cliente ccd ON ccd.ClienteID = vc.ComisionistaIDDespues AND ccd.Estatus = 1
	WHERE vc.Estatus = 1
GO

/* *****************************************************************************
** Modificar Vistas
***************************************************************************** */

-- 9500

ALTER VIEW [dbo].[ProveedorGananciasView] AS
	SELECT
		ProveedorGanancia.ProveedorGananciaID
		,ProveedorGanancia.ProveedorID	
		,MarcaParte.NombreMarcaParte AS Marca	
		,Linea.NombreLinea AS Linea
		,ProveedorGanancia.PorcentajeUno AS PCT1
		,ProveedorGanancia.PorcentajeDos AS PCT2
		,ProveedorGanancia.PorcentajeTres AS PCT3
		,ProveedorGanancia.PorcentajeCuatro AS PCT4
		,ProveedorGanancia.PorcentajeCinco AS PCT5
		, Linea.LineaID
		, MarcaParte.MarcaParteID
		, Linea.NombreLinea + ' - ' + MarcaParte.NombreMarcaParte AS LineaMarca
	FROM
		ProveedorGanancia
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = ProveedorGanancia.MarcaParteID
		INNER JOIN Linea ON Linea.LineaID = ProveedorGanancia.LineaID
GO

-- Caja

ALTER VIEW VentasView AS
	SELECT
		v.VentaID
		, CONVERT(BIT, 0) AS EsFactura
		, dbo.fnuLlenarIzquierda(CONVERT(NVARCHAR(7), v.VentaID), '0', 7) AS Folio
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, SUM(vd.PrecioUnitario * vd.Cantidad) AS Subtotal
		, SUM(vd.Iva * vd.Cantidad) AS Iva
		, SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Total
		, vpt.Importe AS Pagado
		, v.RealizoUsuarioID AS VendedorID
		, u.NombrePersona AS Vendedor
		, v.ComisionistaClienteID AS ComisionistaID
	FROM
		Venta v
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN (
			SELECT
				vp.VentaID
				, SUM(vpd.Importe) AS Importe
			FROM
				VentaPago vp
				LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE vp.Estatus = 1
			GROUP BY vp.VentaID
		) vpt ON vpt.VentaID = v.VentaID
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		-- LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		-- LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
	WHERE v.Estatus = 1
	GROUP BY
		v.VentaID
		, v.Fecha
		, v.ClienteID
		, c.Nombre
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion
		, vpt.Importe
		, v.RealizoUsuarioID
		, u.NombrePersona
		, v.ComisionistaClienteID
GO

ALTER VIEW VentasPagosDetalleView AS
	SELECT
		vpd.VentaPagoDetalleID
		, vpv.VentaID
		, ISNULL(vpv.EsFactura, 0) AS EsFactura
		, vpv.Folio AS VentaFolio
		, vpv.VentaPagoID
		, vpv.Fecha
		, vpv.Importe AS ImporteTotal
		, vpv.ClienteID
		, vpv.Cliente
		, vpv.TipoPagoID AS TipoDePagoID
		, vpd.TipoFormaPagoID AS FormaDePagoID
		, vpd.Importe
		, b.NombreBanco AS Banco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, vpv.Vendedor
	FROM
		VentaPagoDetalle vpd
		LEFT JOIN VentasPagosView vpv ON vpv.VentaPagoID = vpd.VentaPagoID
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		-- LEFT JOIN TipoPago tp ON tp.TipoPagoID = vp.TipoPagoID AND tp.Estatus = 1
		-- LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1

		/*
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vpd.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		*/
	WHERE vpd.Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

-- Búsqueda y búsqueda avanzada
ALTER PROCEDURE pauParteBusquedaAvanzada (
	@Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
	, @SistemaID INT = NULL
	, @LineaID INT = NULL
	, @MarcaID INT = NULL
	, @TipoCilindroID INT = NULL
	, @Largo INT = NULL
	, @Alto INT = NULL
	, @Dientes INT = NULL
	, @Amperes INT = NULL
	, @Watts INT = NULL
	, @Diametro INT = NULL
	, @Astrias INT = NULL
	, @Terminales INT = NULL
	, @Voltios INT = NULL
	, @CodigoAlterno NVARCHAR(32) = NULL

	, @VehiculoModeloID INT = NULL -- Se debe incluir el ModeloID para que el filtro por vehículo tenga efecto
	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, l.NombreLinea
			, mp.NombreMarcaParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			-- LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
			-- LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID AND pca.Estatus = 1
		WHERE
			(@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
			
			AND (@SistemaID IS NULL OR p.SubsistemaID IN (
				SELECT SubsistemaID
				FROM Subsistema
				WHERE SistemaID = @SistemaID AND Estatus = 1
				))
			AND (@LineaID IS NULL OR p.LineaID = @LineaID)
			AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
			AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
			AND (@Largo IS NULL OR p.Largo = @Largo)
			AND (@Alto IS NULL OR p.Alto = @Alto)
			AND (@Dientes IS NULL OR p.Dientes = @Dientes)
			AND (@Amperes IS NULL OR p.Amperes = @Amperes)
			AND (@Watts IS NULL OR p.Watts = @Watts)
			AND (@Diametro IS NULL OR p.Diametro = @Diametro)
			AND (@Astrias IS NULL OR p.Astrias = @Astrias)
			AND (@Terminales IS NULL OR p.Terminales = @Terminales)
			AND (@Voltios IS NULL OR p.Voltios = @Voltios)
			
			AND (@CodigoAlterno IS NULL OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteCodigoAlterno
				WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
			))
			
			AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
				SELECT ParteID
				FROM ParteVehiculo
				WHERE
					TipoFuenteID <> @IdTipoFuenteMostrador
					AND ModeloID = @VehiculoModeloID
					AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
					AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
			))
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, l.NombreLinea
			, mp.NombreMarcaParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
		WHERE NumeroParte = @Codigo
	END

END
GO