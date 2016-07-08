ALTER TABLE dbo.Parte ADD
	UnidadEmpaque decimal(18, 2) NULL
GO
/**/
UPDATE Parte SET Parte.UnidadEmpaque = ParteExistencia.UnidadEmpaque
FROM
	ParteExistencia 
WHERE
	ParteExistencia.ParteID = Parte.ParteID 
	AND ParteExistencia.SucursalID = 1
GO
/**/
UPDATE Parte SET UnidadEmpaque = 0 WHERE UnidadEmpaque IS NULL
GO
/**/
ALTER TABLE Parte ALTER COLUMN UnidadEmpaque decimal(18, 2) NOT NULL
GO
/**/
ALTER VIEW [dbo].[ExistenciasView]
AS
SELECT
	ParteExistencia.ParteExistenciaID
	,ParteExistencia.ParteID
	,Parte.NumeroParte
	,ParteExistencia.SucursalID
	,Sucursal.NombreSucursal AS Tienda
	,ParteExistencia.Existencia AS Exist
	,ParteExistencia.Maximo AS [Max]
	,ParteExistencia.Minimo AS [Min]
	,Parte.UnidadEmpaque AS UEmp
FROM
	ParteExistencia
	INNER JOIN Parte ON Parte.ParteID = ParteExistencia.ParteID
	INNER JOIN Sucursal ON Sucursal.SucursalID = ParteExistencia.SucursalID
GO
/**/
ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@SucursalID NVARCHAR(10)
) 
AS BEGIN
SET NOCOUNT ON

	SELECT * FROM (
		SELECT
			Parte.ParteID
			,CAST(1 AS BIT) AS Sel
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.UnidadEmpaque	
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
			Parte.UnidadEmpaque) * Parte.UnidadEmpaque AS Pedido	
			,PartePrecio.Costo
			,Parte.ProveedorID
			,Proveedor.NombreProveedor
			,Proveedor.Beneficiario		
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
			AND ParteExistencia.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		GROUP BY
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.UnidadEmpaque
			,Parte.ProveedorID
			,Proveedor.NombreProveedor
			,Proveedor.Beneficiario
			,PartePrecio.Costo
			,Parte.CriterioABC
	)Pedidos
	WHERE Pedidos.Pedido > 0

END
GO
/**/
DROP VIEW MxMnView
GO
/**/
DROP VIEW PedidosSugeridosView
GO