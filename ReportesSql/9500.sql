/* *****************************************************************************
** Script para sacar un listado de todos los 9500 creados.
** Creado:  Moisés
***************************************************************************** */

SELECT
	c9.Cotizacion9500ID AS Folio
	, eg.Descripcion AS Estatus
	, c9.Fecha
	, c9.SucursalID
	, c.Nombre AS Cliente
	, u.NombreUsuario AS Usuario
	, p.NumeroParte
	, p.NombreParte
	, c9d.Costo
	, c9d.Cantidad
	, c9d.PrecioAlCliente
	, STUFF((
		SELECT (', ' + vi.Folio + ' (' + vei.Descripcion + ')')
		FROM
			VentaDetalle vdi
			LEFT JOIN Venta vi ON vi.VentaID = vdi.VentaID AND vi.Estatus = 1
			LEFT JOIN VentaEstatus vei ON vei.VentaEstatusID = vi.VentaEstatusID
		WHERE
			vdi.ParteID = c9d.ParteID
			AND vdi.FechaRegistro > = '20131212'
			-- AND vdi.Estatus = 1
		FOR XML PATH('')
	), 1, 1, '') AS Ventas
	-- , p.ParteID
	--, vd.VentaID
	--, (SELECT COUNT(DISTINCT VentaID) FROM VentaDetalle WHERE ParteID = c9d.ParteID AND Estatus = 1) AS C
	--, ve.Descripcion AS EstatusVenta
FROM
	Cotizacion9500 c9
	LEFT JOIN Cotizacion9500Detalle c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID AND c9d.Estatus = 1
	LEFT JOIN EstatusGenerico eg ON eg.EstatusGenericoID = c9.EstatusGenericoID
	LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
	LEFT JOIN Usuario u ON u.UsuarioID = c9.RealizoUsuarioID AND u.Estatus = 1
	LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
	-- LEFT JOIN VentaDetalle vd ON vd.ParteID = c9d.ParteID AND vd.FechaRegistro >= '20131212' AND vd.Estatus = 1
	-- LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
	-- LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
WHERE c.Estatus = 1
ORDER BY p.NumeroParte

-- SELECT * FROM VentaDetalle WHERE ParteID = 8181 -- AND Estatus = 1
