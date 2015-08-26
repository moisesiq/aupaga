/* Script con modificaciones para el módulo de ventas. Archivo 17
 * Creado: 2013/12/20
 * Subido: 2013/12/22
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Reportes.CajaCorte.Salida', 'D', 'I', 'Salida donde debe mostrarse el reporte del corte (D - Diseño, P - Pantalla, I - Impresora, N - Nada).')

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

ALTER VIEW VentasDevolucionesView AS
	SELECT
		vd.VentaDevolucionID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vd.VentaID
		, v.Folio
		, v.Fecha AS FechaDeVenta
		, ISNULL(CONVERT(BIT, v.ACredito), 0) AS VentaACredito
		, vd.Fecha
		, vd.MotivoID
		, vdm.Descripcion AS Motivo
		, vd.Observacion
		, vd.SucursalID
		, vd.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, vd.EsCancelacion
		, vd.TipoFormaPagoID AS FormaDePagoID
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
		LEFT JOIN VentaDevolucionMotivo vdm ON vdm.VentaDevolucionMotivoID = vd.MotivoID AND vdm.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vd.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'VentaDevolucion' AND a.TablaRegistroID = vd.VentaDevolucionID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
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
** Modificar procedimientos almacenados
***************************************************************************** */

