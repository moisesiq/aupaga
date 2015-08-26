/* Cambio para modulo de proveedores*/
ALTER VIEW [dbo].[ProveedorProductosCompradosView]
AS
SELECT 
	MovimientoInventarioDetalle.MovimientoInventarioDetalleID
	,MovimientoInventarioDetalle.MovimientoInventarioID
	,MovimientoInventario.ProveedorID	
	,Linea.LineaID
	,Linea.NombreLinea AS Linea
	,MarcaParte.MarcaParteID
	,MarcaParte.NombreMarcaParte AS Marca
	,Parte.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte AS Descripcion
	,MovimientoInventarioDetalle.Cantidad	
	,MovimientoInventarioDetalle.Importe
	,MovimientoInventario.FechaRecepcion
FROM 
	MovimientoInventarioDetalle
	INNER JOIN MovimientoInventario ON MovimientoInventario.MovimientoInventarioID = MovimientoInventarioDetalle.MovimientoInventarioID
	INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID
	INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
	INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
WHERE
	MovimientoInventarioDetalle.Estatus = 1
	AND MovimientoInventario.TipoOperacionID = 1
GO