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
			AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			AND ExistenciaOrigen.Existencia > 0
			AND ExistenciaDestino.Existencia <= ExistenciaDestino.Minimo
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
			
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
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
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
			AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
			AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			AND ExistenciaOrigen.Existencia > 0
			AND ExistenciaDestino.Existencia <= ExistenciaDestino.Minimo
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
						
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
			,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
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
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
			AND Sistema.SistemaID = @SistemaID
			AND Parte.SubsistemaID = @SubsistemaID	
			AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
			AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			AND ExistenciaOrigen.Existencia > 0
			AND ExistenciaDestino.Existencia <= ExistenciaDestino.Minimo
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
			
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
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
			AND Sistema.SistemaID = @SistemaID
			AND Parte.SubsistemaID = @SubsistemaID	
			AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
			AND ExistenciaOrigen.Existencia > 0
			AND ExistenciaDestino.Existencia <= ExistenciaDestino.Minimo
			AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)
			
	END
END
