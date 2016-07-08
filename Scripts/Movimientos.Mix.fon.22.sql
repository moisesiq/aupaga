/**/
ALTER VIEW [dbo].[PartesClasificacionAbcView]
AS
SELECT TOP 1000000000000
	Parte.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,ISNULL(Ventas.Cantidad, CAST(0.0 AS DECIMAL(9,2))) AS Cantidad
	,Parte.CriterioABC AS VigenteAbc
	,c.Clasificacion AS NuevaAbc
FROM
	Parte
	LEFT JOIN (
		SELECT 	
			VentaDetalle.ParteID
			,SUM(VentaDetalle.Cantidad) AS Cantidad		
		FROM
			Venta
			INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
		WHERE
			Venta.Fecha BETWEEN DATEADD(YEAR,-1,GETDATE()) AND GETDATE()
			AND Venta.Estatus = 1
			AND Venta.VentaEstatusID IN (1, 4)
		GROUP BY
			VentaDetalle.ParteID) AS Ventas ON Ventas.ParteID = Parte.ParteID	
	LEFT JOIN CriterioABC c ON ISNULL(Ventas.Cantidad, 0) BETWEEN c.RangoInicial AND c.RangoFinal
WHERE
	Parte.Estatus = 1
ORDER BY
	Parte.ParteID

GO

/* - - */
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
	+ Subsistema.NombreSubsistema AS Busqueda
FROM 
	Linea
	INNER JOIN Sistema ON Sistema.SistemaID = Linea.SistemaID
	INNER JOIN Subsistema ON Subsistema.SubsistemaID = Linea.SubsistemaID
WHERE
	Linea.Estatus = 1
GO

/* - - */
ALTER VIEW [dbo].[PerfilPermisosView]
AS
SELECT	
	Row_Number() OVER (ORDER BY (SELECT 1)) AS PerfilPermisosID
	,Perfil.PerfilID
	,Permiso.PermisoID
	,Permiso.NombrePermiso
	,Permiso.MensajeDeError
FROM
	Perfil
	INNER JOIN PerfilPermiso ON PerfilPermiso.PerfilID = Perfil.PerfilID
	INNER JOIN Permiso ON Permiso.PermisoID = PerfilPermiso.PermisoID 
WHERE
	Perfil.Estatus = 1

GO

/**/
CREATE PROCEDURE [dbo].[pauVentasCancelaciones] (
	@SucursalID INT
	, @FechaInicial DATE
	, @FechaFinal DATE
) 
AS BEGIN
SET NOCOUNT ON;

--DECLARE @SucursalID INT
--DECLARE @FechaInicial DATE
--DECLARE @FechaFinal DATE

--SET @SucursalID = 1;
--SET @FechaInicial = CAST('2013-12-12' AS DATE);
--SET @FechaFinal = CAST('2013-12-14' AS DATE);

WITH Ventas
AS
( /*Ventas Cobradas y Pagadas*/	
	
	SELECT 
		VentaDetalle.VentaID
		,VentaDetalle.ParteID
		,VentaDetalle.Cantidad
	FROM
		Venta
		INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID AND VentaDetalle.Estatus = 1
	WHERE
		Venta.VentaEstatusID IN (2, 3)
		AND CAST(Venta.Fecha AS DATE) BETWEEN @FechaInicial AND @FechaFinal -->= '2013-12-13' AND Venta.Fecha <= '2013-12-15'
		AND Venta.Estatus = 1
		AND Venta.SucursalID = @SucursalID
) 

SELECT 	
	res.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,SUM(CantidadVendida) AS CantidadVendida
	,SUM(Cancelaciones) AS Cancelaciones	
FROM (

	SELECT 
		Ventas.VentaID
		,Ventas.ParteID
		,Ventas.Cantidad AS CantidadVendida
		,ISNULL(Devs.Cantidad, 0.0) AS Cancelaciones
	FROM Ventas
	LEFT JOIN (
		SELECT /* Cancelaciones */
			VentaDevolucion.VentaID
			,VentaDevolucionDetalle.ParteID
			,VentaDevolucionDetalle.Cantidad
		FROM 
			VentaDevolucion
			INNER JOIN VentaDevolucionDetalle ON VentaDevolucionDetalle.VentaDevolucionID = VentaDevolucion.VentaDevolucionID
			AND VentaDevolucionDetalle.Estatus = 1
		WHERE
			VentaDevolucion.Estatus = 1
			AND CAST(VentaDevolucion.Fecha AS DATE) BETWEEN @FechaInicial AND @FechaFinal -->= '2013-12-13' AND VentaDevolucion.Fecha <= '2013-12-15'
			AND VentaDevolucion.VentaID NOT IN (SELECT VentaID FROM Ventas)			
			AND VentaDevolucion.SucursalID = @SucursalID
	) AS Devs ON Devs.ParteID = Ventas.ParteID	

) res
INNER JOIN Parte ON Parte.ParteID = res.ParteID
GROUP BY 
	res.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	
END



