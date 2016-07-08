--1 Alter view 
--2 Cambiar el Campo Seguro de la tabla Proveedor, a decimal(18,5)
--3 Alter Tabla

/* 1 */
ALTER VIEW [dbo].[PartesBusquedaEnMovimientosView]
AS
SELECT
	Parte.ParteID
	,MarcaParte.MarcaParteID
	,Linea.LineaID
	,Parte.NumeroParte
	,Parte.NombreParte	
	,Linea.NombreLinea AS Linea		
	,MarcaParte.NombreMarcaParte AS Marca
	,PartePrecio.PartePrecioID
	,PartePrecio.Costo
	,PartePrecio.PorcentajeUtilidadUno
	,PartePrecio.PorcentajeUtilidadDos
	,PartePrecio.PorcentajeUtilidadTres
	,PartePrecio.PorcentajeUtilidadCuatro
	,PartePrecio.PorcentajeUtilidadCinco
	,PartePrecio.PrecioUno
	,PartePrecio.PrecioDos
	,PartePrecio.PrecioTres
	,PartePrecio.PrecioCuatro
	,PartePrecio.PrecioCinco
	,Parte.Etiqueta
	,Parte.SoloUnaEtiqueta
	,Parte.NumeroParte
	+ Parte.NombreParte	
	+ Linea.NombreLinea 	
	+ MarcaParte.NombreMarcaParte AS Busqueda
FROM
	Parte
	INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
	INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
	INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
WHERE
	Parte.Estatus = 1
GO

/* 2 */

GO

/* 3 */
ALTER TABLE dbo.MovimientoInventario ADD
	NombreImagen nvarchar(50) NULL
GO