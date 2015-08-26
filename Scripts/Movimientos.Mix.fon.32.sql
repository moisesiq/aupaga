BEGIN TRAN
BEGIN TRY
/* Cambio de Tipo de Dato */
ALTER TABLE MovimientoInventario
ALTER COLUMN FechaFactura DATETIME NULL
--GO
ALTER TABLE MovimientoInventario
ALTER COLUMN FechaRecepcion DATETIME NULL
--GO
ALTER TABLE MovimientoInventario
ALTER COLUMN FechaAplicacion DATETIME NULL
--GO
ALTER TABLE MovimientoInventario
ALTER COLUMN FechaRegistro DATETIME 
--GO
ALTER TABLE MovimientoInventario
ALTER COLUMN FechaModificacion DATETIME 
--GO
--
ALTER TABLE MovimientoInventarioDescuento
ALTER COLUMN FechaModificacion DATETIME 
--GO
ALTER TABLE MovimientoInventarioDescuento
ALTER COLUMN FechaRegistro DATETIME 
--GO

ALTER TABLE MovimientoInventarioDetalle
ALTER COLUMN FechaRegistro DATETIME 
--GO

ALTER TABLE MovimientoInventarioEstatusContingencia
ALTER COLUMN FechaRegistro DATETIME 
--GO
ALTER TABLE MovimientoInventarioEstatusContingencia
ALTER COLUMN FechaModificacion DATETIME 
--GO
--
ALTER TABLE MovimientoInventarioEtiqueta
ALTER COLUMN FechaRegistro DATETIME 
--GO
ALTER TABLE MovimientoInventarioEtiqueta
ALTER COLUMN FechaModificacion DATETIME 
--GO
--
ALTER TABLE MovimientoInventarioHistorial
ALTER COLUMN FechaModificacion DATETIME 
--GO
ALTER TABLE MovimientoInventarioHistorial
ALTER COLUMN FechaRegistro DATETIME 
--GO
--
ALTER TABLE MovimientoInventarioTraspasoContingencia
ALTER COLUMN FechaRegistro DATETIME 
--GO
ALTER TABLE MovimientoInventarioTraspasoContingencia
ALTER COLUMN FechaModificacion DATETIME 
--GO
ALTER TABLE MovimientoInventarioTraspasoContingencia
ALTER COLUMN FechaSoluciono DATETIME NULL
--GO
COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH
GO


/* Actualización de las vistas relacionadas */
EXECUTE sp_refreshview N'dbo.MovimientoInventarioTraspasosView';
GO
EXECUTE sp_refreshview N'dbo.MovimientoInventarioView';
GO
EXECUTE sp_refreshview N'dbo.MovimientosInventarioDevolucionesView';
GO
EXECUTE sp_refreshview N'dbo.MovimientosInventarioNoPagadosView';
GO
EXECUTE sp_refreshview N'dbo.ProveedorKardexOperacionesView';
GO
EXECUTE sp_refreshview N'dbo.MovimientoInventarioDescuentoView';
GO
EXECUTE sp_refreshview N'dbo.MovimientoInventarioDetalleView';
GO
EXECUTE sp_refreshview N'dbo.MovimientoInventarioTraspasosHisView';
GO
EXECUTE sp_refreshview N'dbo.MovimientosInventarioContadorDevolucionesView';
GO
EXECUTE sp_refreshview N'dbo.ProveedorDetalleDocumentosView';
GO
EXECUTE sp_refreshview N'dbo.ProveedorKardexDetalleOperacionesView';
GO
EXECUTE sp_refreshview N'dbo.ProveedorProductosCompradosView';
GO
EXECUTE sp_refreshview N'dbo.ProveedorReporteDevolucionDetalleView';
GO
EXECUTE sp_refreshview N'dbo.MovimientoInventarioEtiquetasView';
GO
EXECUTE sp_refreshview N'dbo.MovimientoInventarioContingenciasView';
GO




