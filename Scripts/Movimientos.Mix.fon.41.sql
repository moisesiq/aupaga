ALTER PROCEDURE [dbo].[pauParteBusquedaEnTraspasos] (
	@MarcaParteID INT
	, @LineaID INT
	, @SistemaID INT
	, @SubsistemaID INT
	, @SucursalOrigenID INT
	, @SucursalDestinoID INT
	, @Abcs NVARCHAR(20)
) AS BEGIN
	SET NOCOUNT ON
	IF @MarcaParteID = 0 AND LEN(@Abcs) <= 0 BEGIN
	
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
			,CASE WHEN ExistenciaOrigen.Existencia <= MxMnDestino.Minimo THEN ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) ELSE MxMnDestino.Maximo END AS Sugerencia
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
			--AND MxMnDestino.Maximo <> 0
	
	END	ELSE IF @MarcaParteID = 0 AND LEN(@Abcs) > 0 BEGIN
	
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
			,CASE WHEN ExistenciaOrigen.Existencia <= MxMnDestino.Minimo THEN ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) ELSE MxMnDestino.Maximo END AS Sugerencia
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
			--AND MxMnDestino.Maximo <> 0
			AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
	
	END	ELSE IF @MarcaParteID > 0 AND LEN(@Abcs) > 0 BEGIN
	
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
			,CASE WHEN ExistenciaOrigen.Existencia <= MxMnDestino.Minimo THEN ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) ELSE MxMnDestino.Maximo END AS Sugerencia
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
			--AND MxMnDestino.Maximo <> 0
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
			AND Sistema.SistemaID = @SistemaID
			AND Parte.SubsistemaID = @SubsistemaID	
			AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
					
	END ELSE IF @MarcaParteID > 0 AND LEN(@Abcs) <= 0 BEGIN
	
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
			,CASE WHEN ExistenciaOrigen.Existencia <= MxMnDestino.Minimo THEN ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) ELSE MxMnDestino.Maximo END AS Sugerencia
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
			--AND MxMnDestino.Maximo <> 0
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
			AND Sistema.SistemaID = @SistemaID
			AND Parte.SubsistemaID = @SubsistemaID	
				
	END
END

GO

/* */
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
			,CASE WHEN ExistenciaOrigen.Existencia <= MxMnDestino.Minimo THEN ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) ELSE MxMnDestino.Maximo END AS Sugerencia
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
			--AND MxMnDestino.Maximo <> 0
			--AND Destino.Maximo <> 0
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
			,CASE WHEN ExistenciaOrigen.Existencia <= MxMnDestino.Minimo THEN ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) ELSE MxMnDestino.Maximo END AS Sugerencia
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
			--AND MxMnDestino.Maximo <> 0
			--AND Destino.Maximo <> 0
			AND Parte.NumeroParte = @Codigo
	END

END

GO