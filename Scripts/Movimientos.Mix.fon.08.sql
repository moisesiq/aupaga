/* 1 */
ALTER TABLE dbo.MovimientoInventarioTraspasoContingencia ADD
	ObservacionSolucion nvarchar(255) NULL
GO

/* 2 */
ALTER VIEW [dbo].[MovimientoInventarioContingenciasView]
AS
SELECT
	Contingencia.MovimientoInventarioTraspasoContingenciaID	
	,Contingencia.MovimientoInventarioID	
	,Contingencia.MovimientoInventarioDetalleID	
	,CONVERT(VARCHAR, Contingencia.FechaRegistro, 101) AS FechaRegistro
	,MovimientoInventario.SucursalOrigenID
	,Origen.NombreSucursal AS Origen	
	,MovimientoInventario.SucursalDestinoID
	,Destino.NombreSucursal AS Destino
	,MovimientoInventario.UsuarioID
	,Registro.NombreUsuario AS Registro
	,MovimientoInventario.UsuarioRecibioTraspasoID
	,Recibio.NombreUsuario AS Recibio
	,Contingencia.ParteID	
	,Parte.NumeroParte
	,Parte.NombreParte
	,Contingencia.CantidadEnviada AS Enviado
	,Contingencia.CantidadRecibida AS Recibido	
	,Contingencia.CantidadDiferencia AS Diferencia
	,Contingencia.MovimientoInventarioEstatusContingenciaID
	,EstatusContingencia.NombreEstatusContingencia	
	,Contingencia.UsuarioSolucionoID
	,Soluciono.NombreUsuario AS Soluciono
	,Contingencia.FechaSoluciono
	,Contingencia.TipoOperacionID
	,TipoOperacion.NombreTipoOperacion
	,Contingencia.TipoConceptoOperacionID
	,TipoConceptoOperacion.NombreConceptoOperacion
	,Contingencia.Comentario
	,Contingencia.ObservacionSolucion	
FROM
	MovimientoInventarioTraspasoContingencia AS Contingencia
	INNER JOIN MovimientoInventarioEstatusContingencia AS EstatusContingencia ON 
		EstatusContingencia.MovimientoInventarioEstatusContingenciaID = Contingencia.MovimientoInventarioEstatusContingenciaID
	INNER JOIN MovimientoInventario ON MovimientoInventario.MovimientoInventarioID = Contingencia.MovimientoInventarioID
	INNER JOIN Sucursal Origen ON Origen.SucursalID = MovimientoInventario.SucursalOrigenID
	INNER JOIN Sucursal Destino ON Destino.SucursalID = MovimientoInventario.SucursalDestinoID
	INNER JOIN Usuario AS Registro ON Registro.UsuarioID = MovimientoInventario.UsuarioID
	INNER JOIN Usuario AS Recibio ON Recibio.UsuarioID = MovimientoInventario.UsuarioRecibioTraspasoID
	INNER JOIN Parte ON Parte.ParteID = Contingencia.ParteID
	LEFT JOIN Usuario AS Soluciono ON Soluciono.UsuarioID = Contingencia.UsuarioSolucionoID
	LEFT JOIN TipoOperacion ON TipoOperacion.TipoOperacionID = Contingencia.TipoOperacionID
	LEFT JOIN TipoConceptoOperacion ON TipoConceptoOperacion.TipoConceptoOperacionID = Contingencia.TipoConceptoOperacionID
WHERE
	Contingencia.Estatus = 1
	AND MovimientoInventario.Estatus = 1

GO

/* 3 */
INSERT [dbo].[TipoConceptoOperacion] ([TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'NO LOS QUISO', N'NLO', 1, CAST(0xB5370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (5, N'VOLVER A TRASPASAR', N'VOL', 1, CAST(0xB5370B00 AS Date), NULL, 1, 1)


/* 4 */
ALTER VIEW [dbo].[MovimientoInventarioTraspasosHisView]
AS
SELECT
	MovimientoInventarioDetalle.MovimientoInventarioDetalleID
	,MovimientoInventarioDetalle.MovimientoInventarioID
	,MovimientoInventarioDetalle.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte	
	,MovimientoInventarioDetalle.Cantidad AS Enviado
	,ISNULL(MovimientoInventarioTraspasoContingencia.CantidadRecibida, MovimientoInventarioDetalle.Cantidad) AS Recibido
	,ISNULL(MovimientoInventarioTraspasoContingencia.CantidadDiferencia, 0) AS Diferencia
FROM
	MovimientoInventarioDetalle
	INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID
	LEFT JOIN MovimientoInventarioTraspasoContingencia ON 
		MovimientoInventarioTraspasoContingencia.MovimientoInventarioID = MovimientoInventarioDetalle.MovimientoInventarioID 
		AND MovimientoInventarioTraspasoContingencia.ParteID = MovimientoInventarioDetalle.ParteID 		
WHERE
	MovimientoInventarioDetalle.Estatus = 1
	
GO
