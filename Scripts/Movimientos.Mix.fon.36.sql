ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@SucursalID NVARCHAR(10)
) 
AS BEGIN
SET NOCOUNT ON

	SELECT 
		* 
	FROM (
		SELECT
			ParteID	
			,CAST(1 AS BIT) AS Sel
			,NumeroParte	
			,NombreParte	
			,UnidadEmpaque	
			,CriterioABC	
			,MaxMatriz	
			,MaxSuc02	
			,MaxSuc03	
			,ExistenciaMatriz	
			,ExistenciaSuc02	
			,ExistenciaSuc03	
			,MaxMatriz - ExistenciaMatriz AS NecesidadMatriz
			,MaxSuc02 - ExistenciaSuc02 AS NecesidadSuc02
			,MaxSuc03 - ExistenciaSuc03 AS NecesidadSuc03
			,(MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) AS Total
			,CEILING((MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) /
			(CASE WHEN Pre.UnidadEmpaque = 0 THEN 1 ELSE Pre.UnidadEmpaque END)) * Pre.UnidadEmpaque AS Pedido	
			,Costo	
			,ProveedorID	
			,NombreProveedor	
			,Beneficiario
		FROM (
			SELECT 
				Parte.ParteID
				,Parte.NumeroParte
				,Parte.NombreParte
				,Parte.UnidadEmpaque
				,ParteAbc.AbcDeVentas AS CriterioABC	
				,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxMatriz
				,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxSuc02
				,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxSuc03	
				,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaMatriz
				,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc02
				,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc03
				,PartePrecio.Costo
				,Parte.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			FROM 
				Parte
				INNER JOIN ParteAbc ON  ParteAbc.ParteID = Parte.ParteID
				INNER JOIN ParteMaxMin ON ParteMaxMin.ParteID = Parte.ParteID
				INNER JOIN ParteExistencia ON ParteExistencia.ParteID = Parte.ParteID AND ParteExistencia.SucursalID = ParteMaxMin.SucursalID
				INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
				INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
			WHERE
				Parte.Estatus = 1 
				AND ParteMaxMin.Maximo > 0
				AND ParteExistencia.Existencia <= ParteMaxMin.Minimo
				AND Parte.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
				AND ParteExistencia.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))			
			GROUP BY
				Parte.ParteID
				,Parte.NumeroParte
				,Parte.NombreParte
				,Parte.UnidadEmpaque
				,ParteAbc.AbcDeVentas
				,PartePrecio.Costo
				,Parte.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			) AS Pre
	) AS Pedidos
	WHERE
		Pedidos.Pedido > 0

END