/* 1 */
ALTER VIEW [dbo].[MotoresView]
AS
SELECT ISNULL(ROW_NUMBER() OVER(ORDER BY MotorID), -1)  AS GenericoID, *
FROM
(
	SELECT DISTINCT TOP 1000000
		Modelo.ModeloID
		,Marca.MarcaID
		,Marca.NombreMarca
		,Modelo.NombreModelo		
		,Motor.MotorID
		,Motor.NombreMotor
		,STUFF((SELECT ', ' + CAST(m.Anio AS VARCHAR)
		FROM MotorAnio m	
		WHERE m.MotorID = Motor.MotorID
		FOR XML PATH('')
		), 1, 1, '') AS Anios		
		,Modelo.NombreModelo 
		+ Marca.NombreMarca 
		+ STUFF((SELECT ', ' + CAST(m.Anio AS VARCHAR)
		FROM MotorAnio m	
		WHERE m.MotorID = Motor.MotorID
		FOR XML PATH('')
		), 1, 1, '') AS Busqueda
	FROM 
		Modelo
		INNER JOIN Marca ON Marca.MarcaID = Modelo.MarcaID
		INNER JOIN Motor ON Motor.ModeloID = Modelo.ModeloID	
	WHERE
		Modelo.Estatus = 1
	ORDER BY 
		Marca.NombreMarca
		,Modelo.NombreModelo
)t
GO
/* 2 */
ALTER VIEW [dbo].[PartesVehiculosView]
AS
SELECT ISNULL(ROW_NUMBER() OVER(ORDER BY NombreModelo), -1)  AS GenericoID, *
FROM
(
	SELECT 
		ParteVehiculo.ParteID
		,Modelo.ModeloID
		,Modelo.NombreModelo
		,STUFF((SELECT DISTINCT ', ' + m.NombreMotor
		FROM
			ParteVehiculo pv
			INNER JOIN Motor m ON m.MotorID = pv.MotorID	
			INNER JOIN Modelo mo ON mo.ModeloID = m.ModeloID
		WHERE
			pv.ParteID = ParteVehiculo.ParteID
			AND mo.ModeloID = Modelo.ModeloID
		FOR XML PATH('')), 1, 1, '') AS Motores	
		,STUFF((SELECT DISTINCT ', ' + CAST(pv.Anio AS VARCHAR)
		FROM
			ParteVehiculo pv
			--INNER JOIN Motor m ON m.MotorID = pv.MotorID	
			--INNER JOIN Modelo mo ON mo.ModeloID = m.ModeloID
		WHERE
			pv.ParteID = ParteVehiculo.ParteID
			--AND mo.ModeloID = Modelo.ModeloID
		FOR XML PATH('')), 1, 1, '') AS Anios	
	FROM 
		ParteVehiculo
		INNER JOIN Motor ON Motor.MotorID = ParteVehiculo.MotorID
		INNER JOIN Modelo ON Modelo.ModeloID = Motor.ModeloID
	GROUP BY
		ParteVehiculo.ParteID
		,Modelo.ModeloID
		,Modelo.NombreModelo
)t
GO

/* 3 */
ALTER VIEW [dbo].[MovimientoInventarioTraspasosView]
AS
SELECT 
	MovimientoInventario.MovimientoInventarioID
    ,MovimientoInventario.TipoOperacionID
	,MovimientoInventario.SucursalOrigenID
	,Origen.NombreSucursal AS Origen
	,MovimientoInventario.SucursalDestinoID
	,Destino.NombreSucursal	AS Destino	
	,MovimientoInventario.FechaRegistro	
	,Usuario.NombreUsuario AS Registro
	,Solicito.NombreUsuario AS Solicito
	,MovimientoInventario.FechaRecepcion
	,Recibio.NombreUsuario AS Recibio
	,MovimientoInventario.Observacion
FROM 
	MovimientoInventario
	INNER JOIN Sucursal AS Origen ON Origen.SucursalID = MovimientoInventario.SucursalOrigenID
	INNER JOIN Sucursal AS Destino ON Destino.SucursalID = MovimientoInventario.SucursalDestinoID
	INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
	LEFT JOIN Usuario AS Solicito ON Solicito.UsuarioID = MovimientoInventario.UsuarioSolicitoTraspasoID
	LEFT JOIN Usuario AS Recibio ON Recibio.UsuarioID = MovimientoInventario.UsuarioRecibioTraspasoID
WHERE
	MovimientoInventario.Estatus = 1
GO

/* 4 */
ALTER VIEW [dbo].[MxMnView]
AS
SELECT
	Row_Number() OVER (ORDER BY (SELECT 1)) AS MxMnID
	,CAST(0 AS BIT) AS Sel
	,MxMn.* 
	,dbo.fnuPromedioMayorSeisMeses(MesAnterior1,MesAnterior2,MesAnterior3,MesAnterior4,MesAnterior5,MesAnterior6,MesAnterior7,MesAnterior8,MesAnterior9,MesAnterior10,MesAnterior11,MesAnterior12) AS PromedioMayor
	,dbo.fnuPromedioMenorSeisMeses(MesAnterior1,MesAnterior2,MesAnterior3,MesAnterior4,MesAnterior5,MesAnterior6,MesAnterior7,MesAnterior8,MesAnterior9,MesAnterior10,MesAnterior11,MesAnterior12) AS PromedioMenor	
	,ISNULL(ParteMxMn.EsFijo, 0) AS Fijo
	,CASE WHEN ISNULL(ParteMxMn.EsFijo, 0) = 1 THEN ISNULL(ParteMxMn.Minimo, NULL) ELSE NULL END AS [MIN]
	,CASE WHEN ISNULL(ParteMxMn.EsFijo, 0) = 1 THEN ISNULL(ParteMxMn.Maximo, NULL) ELSE NULL END AS [MAX]	
	,CAST(ISNULL(ParteMxMn.Criterio, 0) AS INT) AS Criterio
	,ISNULL(ParteMxMn.AjusteMx, '') AS AjusteMx
	,ISNULL(ParteMxMn.AjusteMn, '') AS AjusteMn	
	,ISNULL(STUFF((SELECT ',' + CAST(m.MxMnCriterioID AS VARCHAR)
	FROM ParteMxMnCriterio m	
	WHERE m.ParteMxMnID = ParteMxMn.ParteMxMnID
	FOR XML PATH('')
	), 1, 1, '') , '') AS CriteriosGenerales	
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
	LEFT JOIN ParteMxMn ON ParteMxMn.ParteID = MxMn.ParteID AND ParteMxMn.SucursalID = MxMn.SucursalID

GO


