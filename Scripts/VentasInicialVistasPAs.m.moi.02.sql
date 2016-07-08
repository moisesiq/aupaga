/* Script inicial para modificar Vistas relacionados con el 
   módulo de punto de venta.
*/

USE ControlRefaccionaria
GO

-- 8. Equivalentes

ALTER VIEW PartesEquivalentesView AS
	SELECT
		ParteEquivalente.ParteEquivalenteID
		,ParteEquivalente.ParteID
		,ParteEquivalente.ParteIDequivalente
		,Parte.NumeroParte
		,Parte.NombreParte AS Descripcion
		,ParteImagen.NombreImagen
		,(SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteExistencia.ParteID = ParteEquivalente.ParteID AND ParteExistencia.SucursalID = 1) AS Matriz
		,(SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteExistencia.ParteID = ParteEquivalente.ParteID AND ParteExistencia.SucursalID = 2) AS Suc02
		,(SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteExistencia.ParteID = ParteEquivalente.ParteID AND ParteExistencia.SucursalID = 3) AS Suc03
	FROM
		ParteEquivalente
		INNER JOIN Parte ON Parte.ParteID = ParteEquivalente.ParteIDequivalente
		LEFT JOIN ParteImagen ON ParteImagen.ParteID = ParteEquivalente.ParteIDequivalente
	WHERE
		ParteEquivalente.Estatus = 1
GO

-- 11. 9500

ALTER VIEW [dbo].[LineaMarcaPartesView] AS
	SELECT
		LineaMarcaParte.LineaMarcaParteID
		, LineaMarcaParte.LineaID
		, Linea.NombreLinea
		, LineaMarcaParte.MarcaParteID
		, MarcaParte.NombreMarcaParte
		, Linea.NombreLinea + ' - ' + MarcaParte.NombreMarcaParte AS LineaMarca
	FROM
		LineaMarcaParte
		INNER JOIN Linea ON Linea.LineaID = LineaMarcaParte.LineaID
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = LineaMarcaParte.MarcaParteID
	WHERE
		LineaMarcaParte.Estatus = 1
GO