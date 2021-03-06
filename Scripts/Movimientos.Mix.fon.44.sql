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
				AND Parte.ParteID NOT IN 
					(SELECT d.ParteID 
					FROM MovimientoInventario m 
					INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID 
					WHERE m.TipoOperacionID = 5 
					AND m.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
					AND m.FechaRecepcion IS NULL 
					AND m.Estatus = 1)			
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

GO

/**/
ALTER PROCEDURE [dbo].[pauParteBusquedaAvanzadaEnTraspasos] (
	@Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
	, @SucursalOrigenID INT = NULL
	, @SucursalDestinoID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,ExistenciaOrigen.SucursalID
			,ExistenciaOrigen.Existencia
			,MxMnOrigen.Maximo
			,MxMnOrigen.Minimo
			,ExistenciaDestino.SucursalID AS SucursalDestinoID
			,ExistenciaDestino.Existencia AS DestinoExistencia
			,MxMnDestino.Maximo AS DestinoMaximo
			,MxMnDestino.Minimo AS DestinoMinimo
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		FROM
			Parte	
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
			INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
			INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		WHERE
			Parte.Estatus = 1
			AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
			--AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			--AND ExistenciaOrigen.Existencia > 0	
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)		
			AND (@Descripcion1 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion9 + '%')
	END ELSE BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,ExistenciaOrigen.SucursalID
			,ExistenciaOrigen.Existencia
			,MxMnOrigen.Maximo
			,MxMnOrigen.Minimo
			,ExistenciaDestino.SucursalID AS SucursalDestinoID
			,ExistenciaDestino.Existencia AS DestinoExistencia
			,MxMnDestino.Maximo AS DestinoMaximo
			,MxMnDestino.Minimo AS DestinoMinimo
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		FROM
			Parte	
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
			INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
			INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
			INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		WHERE
			Parte.Estatus = 1
			AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
			AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
			AND Parte.NumeroParte = @Codigo
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
			--AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			--AND ExistenciaOrigen.Existencia > 0
	END

END

GO
/**/
ALTER PROCEDURE [dbo].[pauParteBusquedaEnTraspasos] (
	@MarcaParteID INT
	, @LineaID INT
	, @ProveedorID INT
	, @SucursalOrigenID INT
	, @SucursalDestinoID INT
	, @Abcs NVARCHAR(20)
) AS BEGIN
		
	--DECLARE @MarcaParteID INT = 0 --12
	--DECLARE @LineaID INT = 0 --180
	--DECLARE @ProveedorID INT = 0 --10
	--DECLARE @SucursalOrigenID INT = 1 
	--DECLARE @SucursalDestinoID INT = 3
	--DECLARE @Abcs NVARCHAR(20) = 'A'

	SELECT
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,Parte.MarcaParteID	
		,Parte.LineaID	
		,Sistema.SistemaID
		,Parte.SubsistemaID
		,ExistenciaOrigen.SucursalID
		,ExistenciaOrigen.Existencia
		,MxMnOrigen.Maximo
		,MxMnOrigen.Minimo
		,ExistenciaDestino.SucursalID AS SucursalDestinoID
		,ExistenciaDestino.Existencia AS DestinoExistencia
		,MxMnDestino.Maximo AS DestinoMaximo
		,MxMnDestino.Minimo AS DestinoMinimo
		,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		,Parte.NumeroParte + Parte.NombreParte AS Busqueda
	FROM
		Parte	
		INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
		INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteAbc ON ParteAbc.ParteID = Parte.ParteID
	WHERE
		Parte.Estatus = 1
		AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
		AND (@MarcaParteID <= 0 OR Parte.MarcaParteID = @MarcaParteID)
		AND (@LineaID <= 0 OR Parte.LineaID = @LineaID)
		AND (@ProveedorID <= 0 OR Parte.ProveedorID = @ProveedorID)
		AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
		AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
		AND ExistenciaOrigen.Existencia > 0
		AND ExistenciaDestino.Existencia <= MxMnDestino.Minimo
		AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)

END