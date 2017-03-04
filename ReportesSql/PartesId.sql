SELECT
	p.ParteID
	, p.NumeroParte 
	, p.NombreParte AS Descripcion	
	, p.ProveedorID
	, pv.NombreProveedor AS Proveedor
	, p.MarcaParteID 
	, mp.NombreMarcaParte AS Marca
	, p.LineaID
	, l.NombreLinea AS Linea
	, pp.Costo
	, pp.CostoConDescuento
	, pp.PrecioUno
	, pp.PrecioDos
	, pp.PrecioTres
	, pp.PrecioCuatro
	, pp.PrecioCinco
FROM
	Parte p
	LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
	LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
WHERE p.Estatus = 1
and l.LineaID = 132

--ORDER BY p.NumeroParte

