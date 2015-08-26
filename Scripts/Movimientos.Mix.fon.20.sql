/**/
ALTER VIEW [dbo].[LineasView]
AS
SELECT 
	Linea.LineaID
	,Linea.NombreLinea
	,Linea.Abreviacion
	,Sistema.NombreSistema AS Sistema	
	,Subsistema.NombreSubsistema AS Subsistema
	,Linea.Machote
	,CASE WHEN Linea.Alto = 1 THEN 'SI' ELSE 'NO' END AS Alto	
	,CASE WHEN Linea.Diametro = 1 THEN 'SI' ELSE 'NO' END AS Diametro
	,CASE WHEN Linea.Largo = 1 THEN 'SI' ELSE 'NO' END AS Largo
	,CASE WHEN Linea.Dientes = 1 THEN 'SI' ELSE 'NO' END AS Dientes
	,CASE WHEN Linea.Astrias = 1 THEN 'SI' ELSE 'NO' END AS Astrias
	,CASE WHEN Linea.Sistema = 1 THEN 'SI' ELSE 'NO' END AS Sistem@
	,CASE WHEN Linea.Capacidad = 1 THEN 'SI' ELSE 'NO' END AS Capacidad
	,CASE WHEN Linea.Amperaje = 1 THEN 'SI' ELSE 'NO' END AS Amperaje	
	,CASE WHEN Linea.Voltaje = 1 THEN 'SI' ELSE 'NO' END AS Voltaje
	,CASE WHEN Linea.Watts = 1 THEN 'SI' ELSE 'NO' END AS Watts
	,CASE WHEN Linea.Ubicacion = 1 THEN 'SI' ELSE 'NO' END AS Ubicacion
	,CASE WHEN Linea.Terminales = 1 THEN 'SI' ELSE 'NO' END AS Terminales
	,CASE WHEN Linea.Cilindros = 1 THEN 'SI' ELSE 'NO' END AS Cilindros
	,CASE WHEN Linea.Garantia = 1 THEN 'SI'	ELSE 'NO' END AS Garantia
	,CAST(Linea.LineaID AS VARCHAR) 
	+ Linea.NombreLinea
	+ CASE WHEN Linea.Abreviacion IS NULL THEN '' ELSE Linea.Abreviacion END 
	--+ Sistema.NombreSistema 
	+ Subsistema.NombreSubsistema AS Busqueda
FROM 
	Linea
	INNER JOIN Sistema ON Sistema.SistemaID = Linea.SistemaID
	INNER JOIN Subsistema ON Subsistema.SubsistemaID = Linea.SubsistemaID
WHERE
	Linea.Estatus = 1


GO


