/* Script con modificaciones para el módulo de ventas. Archivo 95
 * Creado: 2015/03/16
 * Subido: 2015/03/26
 */

--
-- Procedimiento para borrar una llave foranea de algún campo, se implementa aquí para utilizarse en este mismo script
CREATE PROCEDURE pauBorrarLlaveForanea (
	@Tabla NVARCHAR(64)
	, @ParteLlave NVARCHAR(64)
)
AS BEGIN
	SET NOCOUNT ON
	
	DECLARE @Const NVARCHAR(128) = (
		SELECT TOP 1 name
		FROM sys.foreign_keys
		WHERE
			parent_object_id = OBJECT_ID(@Tabla)
			AND name LIKE ('%' + @ParteLlave + '%')
	)
	DECLARE @Com NVARCHAR(128) = ('ALTER TABLE ' + @Tabla + ' DROP CONSTRAINT ' + @Const)
	EXEC (@Com)
END
GO
--

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Proveedores
	
	CREATE TABLE ProveedorNotaDeCreditoOrigen (
		ProveedorNotaDeCreditoOrigenID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Origen NVARCHAR(32) NOT NULL
	)
	INSERT INTO ProveedorNotaDeCreditoOrigen (Origen) VALUES
		('DEVOLUCIÓN')
		, ('GARANTÍA')
		, ('DESCUENTO')

	-- DROP TABLE ProveedorNotaDeCredito
	CREATE TABLE ProveedorNotaDeCredito (
		ProveedorNotaDeCreditoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ProveedorID INT NOT NULL FOREIGN KEY REFERENCES Proveedor(ProveedorID)
		, Folio NVARCHAR(16) NOT NULL
		, Fecha DATETIME NOT NULL
		, Subtotal DECIMAL(12, 2) NOT NULL
		, Iva DECIMAL(12, 2) NOT NULL
		, Disponible BIT NOT NULL
		, OrigenID INT NOT NULL FOREIGN KEY REFERENCES ProveedorNotaDeCreditoOrigen(ProveedorNotaDeCreditoOrigenID)
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, Observacion NVARCHAR(128) NULL
	)

	CREATE TABLE ProveedorPolizaDetalleOrigen (
		ProveedorPolizaDetalleOrigenID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Origen NVARCHAR(32) NOT NULL
	)
	INSERT INTO ProveedorPolizaDetalleOrigen (Origen) VALUES
		('PAGO DIRECTO')
		, ('DESCUENTO FACTURA')
		, ('DESCUENTO DIRECTO')
		, ('NOTA DE CRÉDITO')
		, ('PAGO DE CAJA')

	EXEC pauBorrarLlaveForanea 'MovimientoInventario', '_Devol'
	ALTER TABLE MovimientoInventario DROP COLUMN DevolucionOrigenID
	DROP TABLE MovimientoInventarioDevolucionOrigen
	UPDATE MovimientoInventario SET ImporteFactura = ImporteTotal WHERE ImporteFactura IS NULL
	ALTER TABLE MovimientoInventario ALTER COLUMN ImporteFactura DECIMAL(12, 2) NOT NULL
	
	ALTER TABLE Proveedor ALTER COLUMN Seguro DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoItemUno DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoItemDos DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoItemTres DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoItemCuatro DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoItemCinco DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoFacturaUno DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoFacturaDos DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoFacturaTres DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoFacturaCuatro DECIMAL(8, 4) NULL
	ALTER TABLE Proveedor ALTER COLUMN DescuentoFacturaCinco DECIMAL(8, 4) NULL

	-- Proveedores pagos
	
	ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN ImporteMovimiento DECIMAL(12, 2) NOT NULL
	EXEC pauBorrarLlaveForanea 'ProveedorPolizaDetalle', 'ProveedorPolizaTipoMovimiento'
	ALTER TABLE ProveedorPolizaDetalle DROP COLUMN ProveedorPolizaTipoMovimientoID
	DROP TABLE ProveedorPolizaTipoMovimiento
	EXEC sp_rename 'ProveedorPolizaDetalle.ImporteMovimiento', 'Importe', 'COLUMN'

	ALTER TABLE CajaEgreso ADD AfectadoEnProveedores BIT NULL

	-- Contabilidad - Gastos caja

	ALTER TABLE CajaEgreso ADD
		AfectadoEnPolizas BIT NULL
		, Facturado BIT NULL
		, FolioFactura NVARCHAR(16) NULL
		, FechaFactura DATETIME NULL
		, Subtotal DECIMAL(12, 2) NULL
		, Iva DECIMAL(12, 2) NULL

	UPDATE ContaConfigAfectacion SET Operacion = 'Gasto facturado' WHERE ContaConfigAfectacionID = 19
	UPDATE ContaConfigAfectacion SET Operacion = 'Pago a proveedor con gasto de caja no facturado' WHERE ContaConfigAfectacionID = 14
	UPDATE ContaConfigAfectacion SET Operacion = 'Pago a proveedor directo' WHERE ContaConfigAfectacionID = 22
	UPDATE ContaConfigAfectacion SET Operacion = 'Devolución de Venta Facturada con Pago' WHERE ContaConfigAfectacionID = 4
	UPDATE ContaConfigAfectacion SET Operacion = 'Devolución de Venta Facturada con Vale' WHERE ContaConfigAfectacionID = 5
	UPDATE ContaConfigAfectacion SET Operacion = 'Devolución de Venta a Crédito Facturada con Pago' WHERE ContaConfigAfectacionID = 8
	UPDATE ContaConfigAfectacion SET Operacion = 'Devolución de Venta a Crédito Facturada con Vale' WHERE ContaConfigAfectacionID = 9
	INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES
		('Pago a proveedor con nota de crédito de garantía', 1)
		, ('Pago a proveedor con nota de crédito de devolución', 1)
		, ('Pago a proveedor con descuento directo', 1)
		, ('Pago a proveedor con gasto de caja facturado', 1)
		, ('Devolución de Venta con Vale Ticket', 1)
		, ('Venta de Contado con Pago - Factura Global', 1)

	ALTER TABLE ContaPolizaDetalle ADD RelacionID INT NULL

	COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH
GO

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

ALTER TABLE ProveedorPolizaDetalle ADD
	OrigenID INT NULL FOREIGN KEY REFERENCES ProveedorPolizaDetalleOrigen(ProveedorPolizaDetalleOrigenID)
	, TipoFormaPagoID INT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
	, BancoCuentaID INT NULL FOREIGN KEY REFERENCES BancoCuenta(BancoCuentaID)
	, FolioDePago NVARCHAR(32) NULL
	, NotaDeCreditoID INT NULL FOREIGN KEY REFERENCES ProveedorNotaDeCredito(ProveedorNotaDeCreditoID)
GO
UPDATE ProveedorPolizaDetalle SET
	OrigenID = 1
	, TipoFormaPagoID = pp.TipoFormaPagoID
	, BancoCuentaID = pp.BancoCuentaID
	, FolioDePago = pp.NumeroDocumento
FROM
	ProveedorPolizaDetalle ppd
	LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID
ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN TipoFormaPagoID INT NOT NULL
ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN OrigenID INT NOT NULL

ALTER TABLE ProveedorPoliza ADD ImportePago DECIMAL(12, 2) NULL
GO
UPDATE ProveedorPoliza SET ImportePago = ImporteTotal

ALTER TABLE ProveedorPoliza DROP CONSTRAINT FK_ProveedorPoliza_Banco
ALTER TABLE ProveedorPoliza DROP CONSTRAINT FK_BancoCuentaID
EXEC pauBorrarLlaveForanea 'ProveedorPoliza', '_TipoFormaPago'
EXEC pauBorrarLlaveForanea 'ProveedorPoliza', '_BancoCuentaID'
ALTER TABLE ProveedorPoliza DROP COLUMN
	ImporteTotal
	, BancoID
	, TipoFormaPagoID
	, NumeroDocumento
	, Beneficiario
	, BancoCuentaID

-- Contabilidad

ALTER TABLE VentaFactura ADD
	Costo DECIMAL(12, 2) NULL
	, Subtotal DECIMAL(12, 2) NULL
	, Iva DECIMAL(12, 2) NULL
GO
UPDATE VentaFactura SET Costo = 0, Subtotal = 0, Iva = 0
ALTER TABLE VentaFactura ALTER COLUMN Costo DECIMAL(12, 2) NOT NULL
ALTER TABLE VentaFactura ALTER COLUMN Subtotal DECIMAL(12, 2) NOT NULL
ALTER TABLE VentaFactura ALTER COLUMN Iva DECIMAL(12, 2) NOT NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

DROP VIEW [dbo].[MovimientosInventarioNoPagadosView]
DROP VIEW ProveedorKardexOperacionesView
DROP VIEW ProveedorReportePolizasView
DROP VIEW ProveedorReportePolizasDetalleView
GO

-- DROP VIEW ProveedoresComprasView
CREATE VIEW ProveedoresComprasView AS
	SELECT
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS Fecha
		, mi.ImporteFactura
		, mi.ImporteTotal
		, ISNULL(SUM(ppd.Importe), 0.0) AS Pagado
		, (mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0)) AS Saldo
		, u.NombreUsuario AS Usuario
	FROM
		MovimientoInventario mi
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mi.MovimientoInventarioID AND ppd.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE
		mi.Estatus = 1
		AND mi.TipoOperacionID = 1
	GROUP BY
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
		, u.NombreUsuario
GO

CREATE VIEW ProveedoresNotasDeCreditoView AS
	SELECT
		ndc.ProveedorNotaDeCreditoID
		, ndc.ProveedorID
		, ndc.Folio
		, ndc.Fecha
		, ndc.Subtotal
		, ndc.Iva
		, (ndc.Subtotal + ndc.Iva) AS Total
		, SUM(pd.Importe) AS Usado
		, ((ndc.Subtotal + ndc.Iva) - SUM(pd.Importe)) AS Restante
		, ndc.OrigenID
		, ndco.Origen
		, ndc.Observacion
		, ndc.Disponible
	FROM
		ProveedorNotaDeCredito ndc
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
		LEFT JOIN ProveedorPolizaDetalle pd ON pd.NotaDeCreditoID = ndc.ProveedorNotaDeCreditoID AND pd.Estatus = 1
	GROUP BY
		ndc.ProveedorNotaDeCreditoID
		, ndc.ProveedorID
		, ndc.Folio
		, ndc.Fecha
		, ndc.Subtotal
		, ndc.Iva
		, ndc.OrigenID
		, ndco.Origen
		, ndc.Observacion
		, ndc.Disponible
GO

ALTER VIEW [dbo].[ProveedorPagosYdevolucionesView] AS
	SELECT 
		NEWID() AS GenericoID
		,ProveedorPolizaDetalle.MovimientoInventarioID
		,ProveedorPoliza.ProveedorID
		,ProveedorPoliza.FechaRegistro AS Fecha
		-- ,ProveedorPolizaTipoMovimiento.ProveedorPolizaTipoMovimientoID
		-- ,ProveedorPolizaTipoMovimiento.NombreTipoMovimiento AS TipoMovimiento
		,CAST(ProveedorPoliza.ProveedorPolizaID AS VARCHAR) AS [Poliza / Devolucion]	
		,ProveedorPolizaDetalle.Importe
	FROM
		ProveedorPolizaDetalle	
		INNER JOIN ProveedorPoliza ON ProveedorPoliza.ProveedorPolizaID = ProveedorPolizaDetalle.ProveedorPolizaID
		-- INNER JOIN ProveedorPolizaTipoMovimiento ON ProveedorPolizaTipoMovimiento.ProveedorPolizaTipoMovimientoID = ProveedorPolizaDetalle.ProveedorPolizaTipoMovimientoID
	WHERE	
		ProveedorPolizaDetalle.Estatus = 1
	UNION
	SELECT 
		NEWID() AS GenericoID 
		,MovimientoInventario.AplicaEnMovimientoInventarioID AS MovimientoInventarioID
		,MovimientoInventario.ProveedorID
		,MovimientoInventario.FechaAplicacion AS Fecha
		-- ,3 AS ProveedorPolizaTipoMovimientoID
		-- ,'DEVOLUCION' AS TipoMovimiento
		,'         / ' + CAST(MovimientoInventario.MovimientoInventarioID AS VARCHAR) AS [Poliza / Devolucion]	
		,MovimientoInventario.ImporteTotal AS Importe
	FROM
		MovimientoInventario
	WHERE
		MovimientoInventario.TipoOperacionID = 4 --Devolución
		AND MovimientoInventario.Estatus = 1
		AND MovimientoInventario.AplicaEnMovimientoInventarioID IS NOT NULL
GO

-- DROP VIEW ProveedoresPolizasDetalleAvanzadoView
CREATE VIEW ProveedoresPolizasDetalleAvanzadoView AS
	SELECT
		ppd.ProveedorPolizaDetalleID
		, ppd.ProveedorPolizaID
		, ppd.OrigenID
		, ppo.Origen
		, pp.FechaPago AS Fecha
		, ppd.MovimientoInventarioID
		, ppd.Importe
		, ppd.TipoFormaPagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, ppd.BancoCuentaID
		, ppd.FolioDePago
		, ppd.NotaDeCreditoID
		, ndc.Folio AS NotaDeCredito
		, ndc.OrigenID AS NotaDeCreditoOrigenID
		, ndco.Origen AS NotaDeCreditoOrigen
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN ProveedorPolizaDetalleOrigen ppo ON ppo.ProveedorPolizaDetalleOrigenID = ppd.OrigenID
		LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID AND pp.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = ppd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN ProveedorNotaDeCredito ndc ON ndc.ProveedorNotaDeCreditoID = ppd.NotaDeCreditoID
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
	WHERE ppd.Estatus = 1
GO

ALTER VIEW [dbo].[CajaEgresosView] AS
	SELECT
		ce.CajaEgresoID
		, ce.CajaTipoEgresoID
		, cte.NombreTipoEgreso AS Tipo
		, ce.Concepto
		, ce.Importe
		, ce.Fecha
		, ce.SucursalID
		, s.NombreSucursal AS Sucursal
		, ce.AfectadoEnProveedores
		, ce.AfectadoEnPolizas
		, u.NombreUsuario AS Usuario
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'Pendiente') AS AutorizoUsuario
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
	FROM
		CajaEgreso ce
		LEFT JOIN CajaTipoEgreso cte ON cte.CajaTipoEgresoID = ce.CajaTipoEgresoID AND cte.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'CajaEgreso' AND a.TablaRegistroID = ce.CajaEgresoID AND a.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ce.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN ContaEgreso cne ON cne.ContaEgresoID = ce.ContaEgresoID -- AND cne.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cne.ContaCuentaAuxiliarID
	WHERE ce.Estatus = 1
GO

-- DROP VIEW ProveedoresPolizasView
CREATE VIEW ProveedoresPolizasView AS
	SELECT
		pp.ProveedorPolizaID
		, pp.ProveedorID
		, p.NombreProveedor AS Proveedor
		, pp.FechaPago
		, pp.ImportePago
	FROM
		ProveedorPoliza pp
		LEFT JOIN Proveedor p ON p.ProveedorID = pp.ProveedorID AND p.Estatus = 1
	WHERE pp.Estatus = 1
GO

ALTER VIEW [dbo].[VentasDevolucionesView] AS
	SELECT
		vd.VentaDevolucionID
		, ISNULL(CONVERT(BIT, vf.VentaFacturaID), 0) AS Facturada
		, vd.VentaID
		, v.Folio AS FolioDeVenta
		, v.Fecha AS FechaDeVenta
		, ISNULL(CONVERT(BIT, v.ACredito), 0) AS VentaACredito
		, v.ClienteID
		, c.Nombre AS Cliente
		, vd.Fecha
		, vd.MotivoID
		, vdm.Descripcion AS Motivo
		, vd.Observacion
		, vd.SucursalID
		, s.NombreSucursal AS Sucursal
		, vd.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, vd.EsCancelacion
		, vd.TipoFormaPagoID AS FormaDePagoID
		, vd.NotaDeCreditoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		-- , CASE WHEN vd.EsCancelacion = 1 THEN 'CANC-' ELSE '    -DEV' END AS Tipo
		-- , CASE WHEN vd.TipoFormaPagoID = 2 THEN 'EF-' ELSE '  -NC' END AS Salida
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'PENDIENTE') AS AutorizoUsuario
		, CONVERT(DECIMAL(12, 2), vdd.Subtotal) AS Subtotal
		, CONVERT(DECIMAL(12, 2), vdd.Iva) AS Iva
		, CONVERT(DECIMAL(12, 2), vdd.Total) AS Total
	FROM
		VentaDevolucion vd
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDevolucionMotivo vdm ON vdm.VentaDevolucionMotivoID = vd.MotivoID
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = vd.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vd.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'VentaDevolucion' AND a.TablaRegistroID = vd.VentaDevolucionID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
		LEFT JOIN VentaFactura vf ON vf.VentaFacturaID = vfd.VentaFacturaID AND vf.EstatusGenericoID = 3 AND vf.Estatus = 1
		-- LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
		LEFT JOIN (
			SELECT
				VentaDevolucionID
				, SUM(PrecioUnitario * Cantidad) AS Subtotal
				, SUM(Iva * Cantidad) AS Iva
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDevolucionDetalle
			WHERE Estatus = 1
			GROUP BY VentaDevolucionID
		) vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID
	WHERE vd.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauParteBusquedaAvanzadaEnTraspasos] (
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
	, @SucursalOrigenID INT = NULL
	, @SucursalDestinoID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,ExistenciaOrigen.SucursalID
			,ExistenciaOrigen.Existencia
			,MxMnOrigen.Maximo
			,MxMnOrigen.Minimo
			,ExistenciaDestino.SucursalID AS SucursalDestinoID
			,ExistenciaDestino.Existencia AS DestinoExistencia
			,MxMnDestino.Maximo AS DestinoMaximo
			,MxMnDestino.Minimo AS DestinoMinimo
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		FROM
			Parte	
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
			INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
			INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		WHERE
			Parte.Estatus = 1
			AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
			--AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			--AND ExistenciaOrigen.Existencia > 0	
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)		
			AND (@Descripcion1 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion9 + '%')
	END ELSE BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,ExistenciaOrigen.SucursalID
			,ExistenciaOrigen.Existencia
			,MxMnOrigen.Maximo
			,MxMnOrigen.Minimo
			,ExistenciaDestino.SucursalID AS SucursalDestinoID
			,ExistenciaDestino.Existencia AS DestinoExistencia
			,MxMnDestino.Maximo AS DestinoMaximo
			,MxMnDestino.Minimo AS DestinoMinimo
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		FROM
			Parte	
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
			INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
			INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		WHERE
			Parte.Estatus = 1
			AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			AND ExistenciaDestino.SucursalID = @SucursalDestinoID
			AND (Parte.NumeroParte = @Codigo OR Parte.CodigoBarra = @Codigo)
			-- La validación de si ya hay un traspaso se hace desde Visual, y sólo muestra una advertencia
			-- AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
			--AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			--AND ExistenciaOrigen.Existencia > 0
	END

END
GO
