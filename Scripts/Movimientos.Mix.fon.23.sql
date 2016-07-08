ALTER VIEW [dbo].[PedidosSugeridosView]
AS
SELECT * FROM (
	SELECT
		Parte.ParteID
		,CAST(1 AS BIT) AS Sel
		,Parte.NumeroParte
		,Parte.NombreParte
		,ParteExistencia.UnidadEmpaque	
		,Parte.CriterioABC	
		,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) AS MaxMatriz
		,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) AS MaxSuc02
		,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) AS MaxSuc03	
		,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaMatriz
		,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc02
		,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc03		
		,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadMatriz
		,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadSuc02
		,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadSuc03		
		,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) +
		SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) +
		SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS Total	
		,CEILING((SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) +
		SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) +
		SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END)) / 
		ParteExistencia.UnidadEmpaque) * ParteExistencia.UnidadEmpaque AS Pedido	
		,PartePrecio.Costo
		,Parte.ProveedorID
		,Proveedor.NombreProveedor
		,Proveedor.Beneficiario
		,ParteExistencia.SucursalID
	FROM
		Parte
		INNER JOIN ParteExistencia ON ParteExistencia.ParteID = Parte.ParteID
		INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
		INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
	WHERE
		Parte.Estatus = 1
		AND ParteExistencia.Maximo > 0
		AND ParteExistencia.Existencia <= ParteExistencia.Minimo
		AND Parte.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
	GROUP BY
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,ParteExistencia.UnidadEmpaque
		,Parte.ProveedorID
		,Proveedor.NombreProveedor
		,Proveedor.Beneficiario
		,PartePrecio.Costo
		,Parte.CriterioABC
		,ParteExistencia.SucursalID
)pedidos
WHERE Pedidos.Pedido > 0
GO


