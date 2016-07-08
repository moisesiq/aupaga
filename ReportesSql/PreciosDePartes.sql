/* *****************************************************************************
** Script que muestra un reporte de las partes y sus precios
** Creado: 19/02/2014 Moisés
***************************************************************************** */

SELECT
	p.NumeroParte AS NumeroDeParte
	, p.NombreParte AS Descripcion
	, l.NombreLinea AS Linea
	, mp.NombreMarcaParte AS Marca
	, pp.Costo
	, pp.PrecioUno
	, pp.PrecioDos
	, pp.PrecioTres
	, pp.PrecioCuatro
	, pp.PrecioCinco
FROM
	Parte p
	LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
	LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
WHERE p.Estatus = 1
ORDER BY p.NumeroParte