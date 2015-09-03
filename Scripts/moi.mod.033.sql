/* Script con modificaciones a la base de datos de Theos. Archivo 033
 * Creado: 2015/09/02
 * Subido: 2015/09/03
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */



/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[VentasPagosDetalleAvanzadoView] AS
	SELECT
		vpd.VentaPagoDetalleID
		, vpd.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, vpd.Importe
		, vpd.BancoID
		, b.NombreBanco AS Banco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, ISNULL(vpdr.Resguardado, 0) AS Resguardado
		, vp.VentaPagoID
		, vp.Fecha
		, vp.SucursalID
		, vp.VentaID
		, SUM(vpd.Importe) AS ImportePago
		, ISNULL(CONVERT(BIT, v.Facturada), 0) AS Facturada
		, v.Folio AS FolioVenta
		, v.Fecha AS FechaVenta
		, v.VentaEstatusID
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.ACredito
		, uv.NombreUsuario AS Vendedor
	FROM
		VentaPagoDetalle vpd
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vpd.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario uv ON uv.UsuarioID = v.RealizoUsuarioID AND uv.Estatus = 1
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
	WHERE vpd.Estatus = 1
	GROUP BY
		vpd.VentaPagoDetalleID
		, vpd.TipoFormaPagoID
		, tfp.NombreTipoFormaPago
		, vpd.Importe
		, vpd.BancoID
		, b.NombreBanco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, vpdr.Resguardado
		, vp.VentaPagoID
		, vp.Fecha
		, vp.SucursalID
		, vp.VentaID
		, v.Facturada
		, v.Folio
		, v.Fecha
		, v.VentaEstatusID
		, v.ClienteID
		, c.Nombre
		, v.ACredito
		, uv.NombreUsuario
GO

ALTER VIEW [dbo].[VentasPagosView] AS
	SELECT
		vp.VentaPagoID
		, vp.VentaID
		, ISNULL(CONVERT(BIT, v.Facturada), 0) AS Facturada
		, v.Folio
		, vp.Fecha
		, ISNULL((SELECT SUM(Importe) FROM VentaPagoDetalle WHERE VentaPagoID = vp.VentaPagoID AND Estatus = 1), 0) AS Importe
		, vp.SucursalID
		, c.ClienteID
		, v.VentaEstatusID
		, ISNULL(v.ACredito, 0) AS ACredito
		-- , v.
		, c.Nombre AS Cliente
		, u.NombrePersona AS Vendedor
		, u.NombreUsuario AS VendedorUsuario
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
	WHERE vp.Estatus = 1
GO

ALTER VIEW [dbo].[VentasDevolucionesView] AS
	SELECT
		vd.VentaDevolucionID
		, ISNULL(CONVERT(BIT, v.Facturada), 0) AS Facturada
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

ALTER VIEW [dbo].[VentasGarantiasView] AS
	SELECT
		vg.VentaGarantiaID
		, ISNULL(CONVERT(BIT, v.Facturada), 0) AS Facturada
		, vg.VentaID
		, v.Folio AS FolioDeVenta
		, v.Fecha AS FechaDeVenta
		, v.ClienteID
		, vg.SucursalID
		, s.NombreSucursal AS Sucursal
		, ISNULL(CONVERT(BIT, v.ACredito), 0) AS VentaACredito
		, vg.Fecha
		, vg.MotivoID
		, vgm.Motivo
		, vg.MotivoObservacion
		, vg.AccionID
		, vga.Accion
		, vg.EstatusGenericoID
		, eg.Descripcion AS Estatus
		, vg.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, ISNULL(CONVERT(BIT, CASE WHEN ac.AutorizoUsuarioID IS NULL THEN 0 ELSE 1 END), 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'PENDIENTE') AS AutorizoUsuario
		-- , CONVERT(DECIMAL(12, 2), vgd.Subtotal) AS Subtotal
		-- , CONVERT(DECIMAL(12, 2), vgd.Iva) AS Iva
		-- , CONVERT(DECIMAL(12, 2), vgd.Total) AS Total
		, (vg.PrecioUnitario + vg.Iva) AS Total
		, vg.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS NombreDeParte
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, vg.Costo
		, vg.PrecioUnitario
		, vg.Iva
		, m.NombreMedida AS Medida
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, vg.RespuestaID
		, vg.FechaCompletado
		, vg.ObservacionCompletado
		, vg.VentaPagoDetalleID
		, vpd.NotaDeCreditoID
		, mi.FolioFactura AS FacturaDeCompra
		, ppd.NotaDeCreditoID AS ProveedorNotaDeCreditoID
	FROM
		VentaGarantia vg
		LEFT JOIN VentaGarantiaMotivo vgm ON vgm.VentaGarantiaMotivoID = vg.MotivoID
		LEFT JOIN VentaGarantiaAccion vga ON vga.VentaGarantiaAccionID = vg.AccionID
		LEFT JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
		LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoDetalleID = vg.VentaPagoDetalleID AND vpd.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = vg.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vg.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN (
			SELECT
				ROW_NUMBER() OVER (PARTITION BY TablaRegistroID ORDER BY FechaAutorizo) AS Registro
				, TablaRegistroID
				, AutorizoUsuarioID
			FROM Autorizacion
			WHERE Tabla = 'VentaGarantia' AND Estatus = 1
		) ac ON ac.TablaRegistroID = vg.VentaGarantiaID AND ac.Registro = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = ac.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND m.Estatus = 1
		LEFT JOIN EstatusGenerico eg ON eg.EstatusGenericoID = vg.EstatusGenericoID
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioDetalleID = vg.MovimientoInventarioDetalleID
			AND mid.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mid.MovimientoInventarioID AND ppd.Estatus = 1
	WHERE vg.Estatus = 1
GO

ALTER VIEW [dbo].[VentasView] AS
	SELECT
		v.VentaID
		, ISNULL(CONVERT(BIT, v.Facturada), 0) AS Facturada
		, v.Folio
		, v.FolioIni
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Subtotal), 0) AS Subtotal
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Iva), 0) AS Iva
		, ISNULL(CONVERT(DECIMAL(12, 2), vd.Total), 0) AS Total
		, ISNULL(CONVERT(DECIMAL(12, 2), vpt.Importe), 0) AS Pagado
		, v.ACredito
		, v.RealizoUsuarioID AS VendedorID
		, u.NombrePersona AS Vendedor
		, u.NombreUsuario AS VendedorUsuario
		, v.ComisionistaClienteID AS ComisionistaID
		, v.ClienteVehiculoID
		, v.Kilometraje
	FROM
		Venta v
		-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN (
			SELECT
				VentaID
				, SUM(PrecioUnitario * Cantidad) AS Subtotal
				, SUM(Iva * Cantidad) AS Iva
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDetalle
			WHERE Estatus = 1
			GROUP BY VentaID
		) vd ON vd.VentaID = v.VentaID
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
		-- LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		-- LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
	WHERE v.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

-- DROP PROCEDURE pauParteVentasPorMes
CREATE PROCEDURE pauParteVentasPorMes (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* Ejemplo
	DECLARE @ParteID INT = 5566
	DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2015-09-01'
	EXEC pauParteVentasPorMes @ParteID, @Desde, @Hasta
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		v.SucursalID
		, s.NombreSucursal AS Sucursal
		, YEAR(v.Fecha) AS Anio
		, MONTH(v.Fecha) AS Mes
		, SUM(vd.Cantidad) AS Cantidad
		, SUM(ISNULL(n.Cantidad, 0.0)) AS Negado
	FROM
		Parte p
		INNER JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		-- Para agregar lo negado
		LEFT JOIN (
			SELECT
				rf.ParteID
				, rf.SucursalID
				, YEAR(rf.FechaRegistro) AS Anio
				, MONTH(rf.FechaRegistro) AS Mes
				, SUM(rf.CantidadRequerida) AS Cantidad
			FROM
				ReporteDeFaltante rf
				-- Para validar que no se haya vendido en el día o en el siguiente
				LEFT JOIN (
					SELECT DISTINCT
						vd.ParteID
						, v.Fecha
					FROM
						VentaDetalle vd
						INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
					WHERE
						vd.Estatus = 1
						AND v.VentaEstatusID = @EstPagadaID
				) vdc ON vdc.ParteID = rf.ParteID AND 
					(vdc.Fecha >= CONVERT(DATE, rf.FechaRegistro) AND vdc.Fecha < DATEADD(d, 2, CONVERT(DATE, rf.FechaRegistro)))
			WHERE
				rf.Estatus = 1
				AND rf.ParteID = @ParteID
				AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
				-- Parte de Venta
				AND vdc.ParteID IS NULL
			GROUP BY
				rf.ParteID
				, rf.SucursalID
				, YEAR(rf.FechaRegistro)
				, MONTH(rf.FechaRegistro)
		) n ON n.ParteID = p.ParteID AND n.SucursalID = v.SucursalID AND n.Anio = YEAR(v.Fecha) AND n.Mes = MONTH(v.Fecha)
	WHERE
		p.Estatus = 1
		AND p.ParteID = @ParteID
		AND v.VentaEstatusID = @EstPagadaID
		AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
	GROUP BY
		v.SucursalID
		, s.NombreSucursal
		, YEAR(v.Fecha)
		, MONTH(v.Fecha)
	ORDER BY
		SucursalID
		, Anio
		, Mes

END
GO