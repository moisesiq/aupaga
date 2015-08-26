/* Script con modificaciones para el módulo de ventas. Archivo 89
 * Creado: 2015/02/11
 * Subido: 2015/02/12
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Compras
	ALTER TABLE MovimientoInventario ADD ImporteFactura DECIMAL(12, 2) NULL
	ALTER TABLE ProveedorPoliza ALTER COLUMN FechaPago DATETIME NOT NULL
	ALTER TABLE ProveedorPoliza ALTER COLUMN FechaRegistro DATETIME NOT NULL
	ALTER TABLE ProveedorPoliza ALTER COLUMN FechaModificacion DATETIME NULL
	ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN FechaRegistro DATETIME NOT NULL

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



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

DROP VIEW [MovimientosInventarioDevolucionesView]

GO

ALTER VIEW [dbo].[MovimientosInventarioNoPagadosView] AS
SELECT 
	MovimientoInventario.MovimientoInventarioID	
	,MovimientoInventario.ProveedorID
	,MovimientoInventario.FolioFactura AS Factura
	,MovimientoInventario.FechaRecepcion AS Fecha
	,MovimientoInventario.ImporteTotal AS Importe
	-- ,CAST(0 AS DECIMAL(18,2)) AS Pagos
	, (ISNULL(ppdc.Importe, 0.0) + ISNULL(dc.Importe, 0.0)) AS Pagos
	,CAST(0 AS DECIMAL(18,2)) AS Aplicados
	,CAST(0 AS DECIMAL(18,2)) AS PorAplicar
	-- ,CAST(0 AS DECIMAL(18,2)) AS Saldo
	, (MovimientoInventario.ImporteTotal - ISNULL(ppdc.Importe, 0.0) + ISNULL(dc.Importe, 0.0)) AS Saldo
	,CAST(0 AS DECIMAL(18,2)) AS Pago
FROM 
	MovimientoInventario
	LEFT JOIN (
		SELECT
			MovimientoInventarioID
			, SUM(ImporteMovimiento) AS Importe
		FROM ProveedorPolizaDetalle
		WHERE Estatus = 1
		GROUP BY MovimientoInventarioID
	) ppdc ON ppdc.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
	LEFT JOIN (
		SELECT
			AplicaEnMovimientoInventarioID
			, SUM(ImporteTotal) AS Importe
		FROM MovimientoInventario
		WHERE
			Estatus = 1
			AND TipoOperacionID = 4
			AND AplicaEnMovimientoInventarioID > 0
		GROUP BY AplicaEnMovimientoInventarioID
	) dc ON dc.AplicaEnMovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
WHERE
	MovimientoInventario.FueLiquidado = 0
	AND MovimientoInventario.Estatus = 1
	AND MovimientoInventario.TipoOperacionID = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

