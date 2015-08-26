/* *****************************************************************************
** Script para sacar un listado de las partes vendidas.
** Creado:  Mois�s
***************************************************************************** */

DECLARE @Desde DATE = '2014-01-01'
DECLARE @Hasta DATE = '2014-06-11'

SET @Hasta = DATEADD(d, 1, @Hasta)

SELECT
	v.Fecha
	, v.Folio
	, ve.Descripcion AS Estatus
	, s.NombreSucursal AS Tienda
	, c.Nombre AS Cliente
	, u.NombreUsuario AS Vendedor
	, p.NumeroParte AS NumeroDeParte
	, p.NombreParte AS Descripcion
	, ISNULL(pp.CostoConDescuento, pp.Costo) AS Costo
	, vd.PrecioUnitario AS PrecioDeVenta
	, vd.Cantidad
	, mp.NombreMarcaParte AS Marca
	, l.NombreLinea AS Linea
FROM
	VentaDetalle vd
	INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
	LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID
	LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
	LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
	LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
	LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
	LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
	LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
	LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
WHERE
	vd.Estatus = 1
	AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
ORDER BY v.Fecha