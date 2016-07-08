/* Script con modificaciones a la base de datos de Theos. Archivo 053
 * Creado: 2015/12/04
 * Subido: 2015/12/09
 */

DECLARE @ScriptID INT = 53
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE VentaGarantia ADD MovimientoInventarioID INT NULL FOREIGN KEY REFERENCES MovimientoInventario(MovimientoInventarioID)
GO
UPDATE VentaGarantia SET MovimientoInventarioID = mi2.MovimientoInventarioID
FROM
	VentaGarantia vg
	LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioDetalleID = vg.MovimientoInventarioDetalleID AND mid.Estatus = 1
	LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
	LEFT JOIN MovimientoInventario mi2 ON mi2.FolioFactura = mi.FolioFactura AND mi2.TipoConceptoOperacionID = 12 AND mi2.Estatus = 1
EXEC sp_rename 'VentaGarantia.MovimientoInventarioDetalleID', '_MovimientoInventarioDetalleID'
EXEC pauBorrarLlaveForanea 'VentaGarantia', 'FK__VentaGara__Prove__'
ALTER TABLE VentaGarantia DROP COLUMN ProveedorNotaDeCreditoID

ALTER TABLE MovimientoInventario DROP COLUMN AplicaEnMovimientoInventarioID, FechaAplicacion
ALTER TABLE MovimientoInventario ADD DeMovimientoInventarioID INT NULL
	FOREIGN KEY REFERENCES MovimientoInventario(MovimientoInventarioID)

CREATE TABLE ProveedorNotaDeCreditoDetalle (
	ProveedorNotaDeCreditoDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ProveedorNotaDeCreditoID INT NOT NULL FOREIGN KEY REFERENCES ProveedorNotaDeCredito(ProveedorNotaDeCreditoID)
	, MovimientoInventarioID INT NOT NULL FOREIGN KEY REFERENCES MovimientoInventario(MovimientoInventarioID)
)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[ProveedoresPolizasDetalleAvanzadoView] AS
	SELECT
		ppd.ProveedorPolizaDetalleID
		, ppd.ProveedorPolizaID
		, ppd.OrigenID
		, ppo.Origen
		, pp.FechaPago AS Fecha
		, ppd.MovimientoInventarioID
		, mi.FolioFactura
		, ppd.Subtotal
		, ppd.Iva
		, (ppd.Subtotal + ppd.Iva) AS Importe
		-- , ppd.TipoFormaPagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		-- , ppd.BancoCuentaID
		, pp.FolioDePago
		, ppd.NotaDeCreditoID
		, ppd.Observacion
		, ppd.Folio
		, ndc.Folio AS NotaDeCredito
		, ndc.OrigenID AS NotaDeCreditoOrigenID
		, ndco.Origen AS NotaDeCreditoOrigen
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN ProveedorPolizaDetalleOrigen ppo ON ppo.ProveedorPolizaDetalleOrigenID = ppd.OrigenID
		LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID AND pp.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = pp.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN ProveedorNotaDeCredito ndc ON ndc.ProveedorNotaDeCreditoID = ppd.NotaDeCreditoID
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
	WHERE ppd.Estatus = 1
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
		-- , ppd.NotaDeCreditoID AS ProveedorNotaDeCreditoID
		-- , vg.ProveedorNotaDeCreditoID
		, pncd.ProveedorNotaDeCreditoID
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
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = vg.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN ProveedorNotaDeCreditoDetalle pncd ON pncd.MovimientoInventarioID = vg.MovimientoInventarioID
		-- LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mid.MovimientoInventarioID AND ppd.Estatus = 1
	WHERE vg.Estatus = 1
GO

ALTER VIEW [dbo].[ProveedoresNotasDeCreditoView] AS
	SELECT
		ndc.ProveedorNotaDeCreditoID
		, ndc.ProveedorID
		, ndc.Folio
		, ndc.Fecha
		, ndc.Subtotal
		, ndc.Iva
		, (ndc.Subtotal + ndc.Iva) AS Total
		, ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0) AS Usado
		, ((ndc.Subtotal + ndc.Iva) - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0)) AS Restante
		, ndc.OrigenID
		, ndco.Origen
		, ndc.Observacion
		, ndc.Disponible
		, STUFF((SELECT ', ' + FolioFactura
			FROM
				ProveedorNotaDeCreditoDetalle ncdi
				INNER JOIN MovimientoInventario mii ON mii.MovimientoInventarioID = ncdi.MovimientoInventarioID
					AND mii.Estatus = 1
			WHERE ncdi.ProveedorNotaDeCreditoID = ndc.ProveedorNotaDeCreditoID
			FOR XML PATH('')
			), 1, 2, '') AS Facturas
	FROM
		ProveedorNotaDeCredito ndc
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.NotaDeCreditoID = ndc.ProveedorNotaDeCreditoID AND ppd.Estatus = 1
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

ALTER VIEW [dbo].[ProveedoresComprasView] AS
	WITH _Compras AS (
		SELECT
			mi.MovimientoInventarioID
			, mi.ProveedorID
			, mi.FolioFactura AS Factura
			, mi.FechaRecepcion AS Fecha
			, mi.ImporteFactura
			, mi.ImporteTotal
			, ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0) AS Abonado
			, (mi.ImporteFactura - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0)) AS Saldo
			, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE (ppd.Subtotal + ppd.Iva) END) AS Descuento
			, (
				(mi.ImporteFactura - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0))
				- SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE (ppd.Subtotal + ppd.Iva) END)
			) AS Final
			, u.NombreUsuario AS Usuario
			-- , ppd.ProveedorPolizaID
			, mi.EsAgrupador
			, mi.MovimientoAgrupadorID
			, p.NombreProveedor AS Proveedor
			, p.DiasPlazo
		FROM
			MovimientoInventario mi
			LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mi.MovimientoInventarioID AND ppd.Estatus = 1
			LEFT JOIN Proveedor p ON p.ProveedorID = mi.ProveedorID AND p.Estatus = 1
			LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
		WHERE
			mi.Estatus = 1
			AND mi.TipoOperacionID = 1
			AND mi.EsAgrupador = 0
		GROUP BY
			mi.MovimientoInventarioID
			, mi.ProveedorID
			, mi.FolioFactura
			, mi.FechaRecepcion
			, mi.ImporteFactura
			, mi.ImporteTotal
			, u.NombreUsuario
			-- , ppd.ProveedorPolizaID
			, mi.EsAgrupador
			, mi.MovimientoAgrupadorID
			, p.NombreProveedor
			, p.DiasPlazo
	)
	SELECT * FROM _Compras
	UNION
	SELECT
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS Fecha
		, mi.ImporteFactura
		, mi.ImporteTotal
		, ISNULL(SUM(c.Abonado), 0.0) AS Abonado
		, ISNULL(SUM(c.Saldo), 0.0) AS Saldo
		, ISNULL(SUM(c.Descuento), 0.0) AS Descuento
		, ISNULL(SUM(c.Final), 0.0) AS Final
		, u.NombreUsuario AS Usuario
		-- , ppd.ProveedorPolizaID
		, mi.EsAgrupador
		, mi.MovimientoAgrupadorID
		, MAX(c.Proveedor) AS Proveedor
		, MAX(c.DiasPlazo) AS DiasDePlazo
	FROM
		MovimientoInventario mi
		LEFT JOIN _Compras c ON c.MovimientoAgrupadorID = mi.MovimientoInventarioID
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE
		mi.Estatus = 1
		AND mi.EsAgrupador = 1
	GROUP BY
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
		, u.NombreUsuario
		, mi.EsAgrupador
		, mi.MovimientoAgrupadorID	
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

