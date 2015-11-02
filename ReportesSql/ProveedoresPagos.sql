/* *****************************************************************************
 * Reporte de pagos a proveedores
 * Creado: 2015-11-02 Moisés
 **************************************************************************** */

SELECT
	pp.FechaPago
	, pv.NombreProveedor AS Proveedor
	, mi.FolioFactura
	, SUM(ppd.Importe) AS Importe
FROM
	ProveedorPoliza pp
	LEFT JOIN Proveedor pv ON pv.ProveedorID = pp.ProveedorID AND pv.Estatus = 1
	LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.ProveedorPolizaID = pp.ProveedorPolizaID AND ppd.Estatus = 1
	LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID AND mi.Estatus = 1
WHERE pp.Estatus = 1
GROUP BY
	pp.FechaPago
	, pv.NombreProveedor
	, mi.FolioFactura
ORDER BY
	pp.FechaPago
	, mi.FolioFactura