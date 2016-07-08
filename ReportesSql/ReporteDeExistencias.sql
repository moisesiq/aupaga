/* *****************************************************************************
** Script que muestra un reporte de existencias por sucursal de las partes.
** Creado: 12/02/2014 Moisés
***************************************************************************** */

SELECT
	p.ParteID
	, p.NumeroParte 
	, p.NombreParte AS Descripcion
	, p.EsServicio
	, l.NombreLinea AS Linea
	, mp.NombreMarcaParte AS Marca
	, pp.Costo
	, s.NombreSucursal AS Sucursal
	, pe.Existencia
	, CASE WHEN p.EsServicio = 1 THEN 0.00 ELSE (pp.Costo * pe.Existencia) END AS CostoTotal
FROM
	Parte p
	LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
	LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
	LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND p.Estatus = 1
	LEFT JOIN Sucursal s ON s.SucursalID = pe.SucursalID AND s.Estatus = 1
WHERE p.Estatus = 1
ORDER BY p.NumeroParte