/* Script con modificaciones para el módulo de ventas. Archivo 99
 * Creado: 2015/04/15
 * Subido: 2015/04/15
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE ContaPolizaDetalle ALTER COLUMN Referencia NVARCHAR(128) NULL

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

ALTER TABLE VentaDevolucion ADD VentaPagoDetalleID INT NULL REFERENCES VentaPagoDetalle(VentaPagoDetalleID)
GO
UPDATE VentaDevolucion SET VentaPagoDetalleID = vpd.VentaPagoDetalleID
FROM
	VentaDevolucion vd
	LEFT JOIN VentaPagoDetalle vpd ON vpd.NotaDeCreditoID = vd.NotaDeCreditoID AND Importe < 0 AND vpd.Estatus = 1
WHERE vd.NotaDeCreditoID IS NOT NULL

EXEC pauBorrarLlaveForanea 'VentaDevolucion', '_NotaD_'
ALTER TABLE VentaDevolucion DROP COLUMN NotaDeCreditoID

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

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
		, vd.VentaPagoDetalleID
		, vpd.NotaDeCreditoID
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
		LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoDetalleID = vd.VentaPagoDetalleID AND vpd.Estatus = 1
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

