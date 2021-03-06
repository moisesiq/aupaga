ALTER PROCEDURE [dbo].[pauVentasCancelaciones] (
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
--SET @FechaInicial = CAST('2013-12-15' AS DATE);
--SET @FechaFinal = CAST('2013-12-16' AS DATE);

	WITH Ventas
	AS
	( 		
		SELECT	/*Ventas Cobradas y Pagadas*/	
			VentaDetalle.VentaID
			,VentaDetalle.ParteID
			,VentaDetalle.Cantidad
		FROM
			Venta
			INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID AND VentaDetalle.Estatus = 1
		WHERE
			Venta.VentaEstatusID IN (2, 3)
			AND CAST(Venta.Fecha AS DATE) BETWEEN @FechaInicial AND @FechaFinal 
			AND Venta.Estatus = 1
			AND Venta.SucursalID = @SucursalID
	) 
	SELECT 	
		res.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,SUM(CantidadVendida) AS CantidadVendida
		,SUM(Cancelaciones) AS Cancelaciones	
		,SUM(CantidadVendida) - SUM(Cancelaciones) AS PorSurtir
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
				AND CAST(VentaDevolucion.Fecha AS DATE) BETWEEN @FechaInicial AND @FechaFinal
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


