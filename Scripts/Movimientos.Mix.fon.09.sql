ALTER VIEW [dbo].[MxMnView]
AS
SELECT 
	Row_Number() OVER (ORDER BY (SELECT 1)) AS MxMnID
	,* 
FROM
	(
	SELECT 
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,ParteExistencia.Existencia
		,ParteExistencia.UnidadEmpaque
		,Parte.TiempoReposicion
		,SUM(CASE WHEN VentaDetalle.FechaRegistro BETWEEN DATEADD(YEAR,-1,GETDATE()) AND GETDATE() THEN VentaDetalle.Cantidad ELSE 0.00 END) / DATEDIFF(DAY,DATEADD(year,-1,GETDATE()), GETDATE()) AS Promedio		
		,MAX(VentaDetalle.Cantidad) AS Mx
		,MIN(VentaDetalle.Cantidad) AS Mn
		,SUM(VentaDetalle.Cantidad) AS Cantidad
		,Venta.SucursalID
		,Parte.LineaID
		,Linea.NombreLinea
		,Parte.MarcaParteID
		,MarcaParte.NombreMarcaParte
		,Sistema.SistemaID
		,Sistema.NombreSistema
		,Parte.SubsistemaID
		,Subsistema.NombreSubsistema
		,Proveedor.ProveedorID
		,Proveedor.NombreProveedor		
		,ISNULL(Parte.Es9500, 0) AS Es9500	
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -1, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior1'	
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -2, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -2, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior2'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -3, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -3, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior3'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -4, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -4, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior4'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -5, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -5, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior5'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -6, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -6, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior6'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -7, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -7, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior7'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -8, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -8, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior8'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -9, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -9, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior9'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -10, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -10, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior10'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -11, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -11, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior11'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -12, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -12, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior12'	
	FROM
		Venta
		INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
		INNER JOIN Parte ON Parte.ParteID = VentaDetalle.ParteID
		INNER JOIN Linea ON Linea.LineaID = Parte.LineaID	
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
		INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
		INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
		INNER JOIN ParteExistencia ON ParteExistencia.ParteID = VentaDetalle.ParteID 
				AND ParteExistencia.SucursalID = Venta.SucursalID
	WHERE
		Venta.Fecha BETWEEN DATEADD(YEAR,-1,GETDATE()) AND GETDATE()
		AND Venta.Estatus = 1
	GROUP BY
		Venta.SucursalID	
		,Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,ParteExistencia.Existencia
		,ParteExistencia.UnidadEmpaque
		,Parte.LineaID
		,Linea.NombreLinea
		,Parte.MarcaParteID
		,MarcaParte.NombreMarcaParte
		,Sistema.SistemaID
		,Sistema.NombreSistema
		,Parte.SubsistemaID
		,Subsistema.NombreSubsistema
		,Proveedor.ProveedorID
		,Proveedor.NombreProveedor
		,Parte.TiempoReposicion
		,Parte.Es9500
	) AS MxMn

GO


