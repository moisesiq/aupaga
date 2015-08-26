/* Script con modificaciones a la base de datos de Theos. Archivo 023
 * Creado: 2015/07/02
 * Subido: 2015/07/09
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

UPDATE MovimientoInventario SET ConceptoMovimiento = tco.TipoConceptoOperacionID
FROM
	MovimientoInventario mi
	LEFT JOIN TipoConceptoOperacion tco ON tco.NombreConceptoOperacion = mi.ConceptoMovimiento
WHERE tco.TipoConceptoOperacionID IS NOT NULL
ALTER TABLE MovimientoInventario ADD TipoConceptoOperacionID INT NULL 
	FOREIGN KEY REFERENCES TipoConceptoOPeracion(TipoConceptoOPeracionID)
GO
UPDATE MovimientoInventario SET TipoConceptoOperacionID = CASE WHEN ConceptoMovimiento = '' THEN NULL ELSE ConceptoMovimiento END
ALTER TABLE MovimientoInventario DROP COLUMN ConceptoMovimiento

ALTER TABLE VentaGarantia ADD MovimientoInventarioDetalleID INT NULL
	FOREIGN KEY REFERENCES MovimientoInventarioDetalle(MovimientoInventarioDetalleID)

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[MovimientoInventarioView] AS
	SELECT
		MovimientoInventario.MovimientoInventarioID
		,MovimientoInventario.TipoOperacionID
		,TipoOperacion.NombreTipoOperacion
		,MovimientoInventario.TipoPagoID	
		,TipoPago.NombreTipoPago
		,MovimientoInventario.ProveedorID	
		,Proveedor.NombreProveedor
		,MovimientoInventario.SucursalOrigenID	
		,s1.NombreSucursal AS SucursalOrigen
		,MovimientoInventario.SucursalDestinoID	
		,s2.NombreSucursal AS SucursalDestino
		,MovimientoInventario.FechaFactura	
		,MovimientoInventario.FechaRecepcion	
		,MovimientoInventario.FolioFactura	
		,MovimientoInventario.AplicaEnMovimientoInventarioID	
		,MovimientoInventario.FechaAplicacion	
		,MovimientoInventario.Subtotal	
		,MovimientoInventario.IVA	
		,MovimientoInventario.ImporteTotal	
		,MovimientoInventario.FueLiquidado	
		-- ,MovimientoInventario.ConceptoMovimiento	
		, MovimientoInventario.TipoConceptoOperacionID
		, tco.NombreConceptoOperacion AS ConceptoOperacion
		,MovimientoInventario.Observacion	
		,MovimientoInventario.UsuarioID	
		,Usuario.NombreUsuario
		,MovimientoInventario.FechaRegistro	
		,MovimientoInventario.FechaModificacion	
		,MovimientoInventario.NombreImagen	
		,MovimientoInventario.Articulos
		,MovimientoInventario.Unidades
		,MovimientoInventario.Seguro
		,MovimientoInventario.ImporteTotalSinDescuento
	FROM
		MovimientoInventario
		LEFT JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
		INNER JOIN TipoOperacion ON TipoOperacion.TipoOperacionID = MovimientoInventario.TipoOperacionID
		LEFT JOIN TipoPago ON TipoPago.TipoPagoID = MovimientoInventario.TipoPagoID
		INNER JOIN Sucursal s1 ON s1.SucursalID = MovimientoInventario.SucursalOrigenID
		INNER JOIN Sucursal s2 ON s2.SucursalID = MovimientoInventario.SucursalDestinoID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
		LEFT JOIN TipoConceptoOperacion tco ON tco.TipoConceptoOperacionID = MovimientoInventario.TipoConceptoOperacionID
	WHERE
		MovimientoInventario.Estatus = 1
GO

ALTER VIEW [dbo].[MovimientosInventarioDetalleAvanzadoView] AS
	SELECT
		mid.MovimientoInventarioDetalleID
		, mid.MovimientoInventarioID
		, mi.TipoOperacionID
		, mi.FolioFactura
		, mi.FechaFactura
		, mi.FechaRecepcion
		, mi.ImporteTotal
		, u.NombreUsuario AS Usuario
		, mid.ParteID
		, mid.Cantidad
		, mid.CantidadDevuelta
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, mp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, l.LineaID
		, l.NombreLinea AS Linea
	FROM
		MovimientoInventarioDetalle mid
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = mid.ParteID AND p.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE mid.Estatus = 1
GO

ALTER VIEW [dbo].[VentasGarantiasView] AS
	SELECT
		vg.VentaGarantiaID
		, ISNULL(CONVERT(BIT, vf.VentaFacturaID), 0) AS Facturada
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
		-- LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
		LEFT JOIN VentaFactura vf ON vf.VentaFacturaID = vfd.VentaFacturaID AND vf.EstatusGenericoID = 3 AND vf.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = vg.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND m.Estatus = 1
		LEFT JOIN EstatusGenerico eg ON eg.EstatusGenericoID = vg.EstatusGenericoID
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioDetalleID = vg.MovimientoInventarioDetalleID
			AND mid.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = mid.MovimientoInventarioID AND mi.Estatus = 1
	WHERE vg.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

