--1 Alter View
--2 Create Procedure para busquedas en traspasos
--3 Busqueda avanzada en traspasos
--4 drop procedure
--5 Create Procedure
--6 Create Procedure
--7 Alter MovimientoInventario
--8 View Traspasos
--9 Alter DetalleMovimientos
--10 Alter ViewDetalle
--11 Create Table Contingencia
/* 1 */
ALTER VIEW [dbo].[MovimientosInventarioDevolucionesView]
AS
SELECT 
	MovimientoInventario.MovimientoInventarioID AS DevolucionID
	,MovimientoInventario.ProveedorID
	,MovimientoInventario.FolioFactura AS Factura
	,MovimientoInventario.FechaRegistro AS Fecha
	,MovimientoInventario.ImporteTotal AS Importe
FROM 
	MovimientoInventario
WHERE
	MovimientoInventario.Estatus = 1
	AND MovimientoInventario.TipoOperacionID = 4	
	AND MovimientoInventario.AplicaEnMovimientoInventarioID IS NULL

GO

/* 2 */
CREATE PROCEDURE [dbo].[pauParteBusquedaEnTraspasos] (
	@MarcaParteID INT
	, @LineaID INT
	, @SistemaID INT
	, @SubsistemaID INT
	, @SucursalOrigenID INT
	, @SucursalDestinoID INT
) AS BEGIN
	SET NOCOUNT ON
	IF @MarcaParteID = 0 BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,Origen.SucursalID
			,Origen.Existencia
			,Origen.Maximo
			,Origen.Minimo
			,Destino.SucursalID AS SucursalDestinoID
			,Destino.Existencia AS DestinoExistencia
			,Destino.Maximo AS DestinoMaximo
			,Destino.Minimo AS DestinoMinimo
			,CASE WHEN Destino.Existencia <= Destino.Minimo THEN ISNULL(Destino.Maximo - Destino.Existencia, 0) ELSE Destino.Maximo END AS Sugerencia
		FROM
			Parte
			INNER JOIN ParteExistencia AS Origen ON Origen.ParteID = Parte.ParteID
			INNER JOIN ParteExistencia AS Destino ON Destino.ParteID = Parte.ParteID
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		WHERE
			Parte.Estatus = 1
			AND Origen.SucursalID = @SucursalOrigenID
			AND Destino.SucursalID = @SucursalDestinoID	
			AND Destino.Maximo <> 0	
	END ELSE BEGIN
		SELECT
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.MarcaParteID	
			,Parte.LineaID	
			,Sistema.SistemaID
			,Parte.SubsistemaID
			,Origen.SucursalID
			,Origen.Existencia
			,Origen.Maximo
			,Origen.Minimo
			,Destino.SucursalID AS SucursalDestinoID
			,Destino.Existencia AS DestinoExistencia
			,Destino.Maximo AS DestinoMaximo
			,Destino.Minimo AS DestinoMinimo
			,CASE WHEN Destino.Existencia <= Destino.Minimo THEN ISNULL(Destino.Maximo - Destino.Existencia, 0) ELSE Destino.Maximo END AS Sugerencia
		FROM
			Parte
			INNER JOIN ParteExistencia AS Origen ON Origen.ParteID = Parte.ParteID
			INNER JOIN ParteExistencia AS Destino ON Destino.ParteID = Parte.ParteID
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		WHERE
			Parte.Estatus = 1
			AND Origen.SucursalID = @SucursalOrigenID
			AND Destino.SucursalID = @SucursalDestinoID
			AND Destino.Maximo <> 0
			AND Parte.MarcaParteID = @MarcaParteID
			AND Parte.LineaID = @LineaID
			AND Sistema.SistemaID = @SistemaID
			AND Parte.SubsistemaID = @SubsistemaID			
	END
END
GO

/* 3 */
CREATE PROCEDURE [dbo].[pauParteBusquedaAvanzadaEnTraspasos] (
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
			,Origen.SucursalID
			,Origen.Existencia
			,Origen.Maximo
			,Origen.Minimo
			,Destino.SucursalID AS SucursalDestinoID
			,Destino.Existencia AS DestinoExistencia
			,Destino.Maximo AS DestinoMaximo
			,Destino.Minimo AS DestinoMinimo
			,CASE WHEN Destino.Existencia <= Destino.Minimo THEN ISNULL(Destino.Maximo - Destino.Existencia, 0) ELSE Destino.Maximo END AS Sugerencia
		FROM
			Parte
			INNER JOIN ParteExistencia AS Origen ON Origen.ParteID = Parte.ParteID
			INNER JOIN ParteExistencia AS Destino ON Destino.ParteID = Parte.ParteID
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		WHERE
			Parte.Estatus = 1
			AND Origen.SucursalID = @SucursalOrigenID
			AND Destino.SucursalID = @SucursalDestinoID
			AND Destino.Maximo <> 0
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
			,Origen.SucursalID
			,Origen.Existencia
			,Origen.Maximo
			,Origen.Minimo
			,Destino.SucursalID AS SucursalDestinoID
			,Destino.Existencia AS DestinoExistencia
			,Destino.Maximo AS DestinoMaximo
			,Destino.Minimo AS DestinoMinimo
			,CASE WHEN Destino.Existencia <= Destino.Minimo THEN ISNULL(Destino.Maximo - Destino.Existencia, 0) ELSE Destino.Maximo END AS Sugerencia
		FROM
			Parte
			INNER JOIN ParteExistencia AS Origen ON Origen.ParteID = Parte.ParteID
			INNER JOIN ParteExistencia AS Destino ON Destino.ParteID = Parte.ParteID
			INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
			INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		WHERE
			Parte.Estatus = 1
			AND Origen.SucursalID = @SucursalOrigenID
			AND Destino.SucursalID = @SucursalDestinoID
			AND Destino.Maximo <> 0
			AND Parte.NumeroParte = @Codigo
	END

END
GO

/* 4 */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[upaPartesBusquedaEnMovimientos]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[upaPartesBusquedaEnMovimientos]
GO

/* 5 */
CREATE PROCEDURE pauPartesBusquedaEnMovimientos
(
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
) AS BEGIN
	SET NOCOUNT ON
	IF @Codigo IS NULL BEGIN
		SELECT
			PartesBusquedaEnMovimientosView.ParteID
			,PartesBusquedaEnMovimientosView.NumeroParte
			,PartesBusquedaEnMovimientosView.NombreParte
			,PartesBusquedaEnMovimientosView.Linea
			,PartesBusquedaEnMovimientosView.Marca
		FROM
			PartesBusquedaEnMovimientosView
		WHERE
			(@Descripcion1 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR PartesBusquedaEnMovimientosView.NombreParte LIKE '%' + @Descripcion9 + '%')
	END ELSE BEGIN
		SELECT
			PartesBusquedaEnMovimientosView.ParteID
			,PartesBusquedaEnMovimientosView.NumeroParte
			,PartesBusquedaEnMovimientosView.NombreParte
			,PartesBusquedaEnMovimientosView.Linea
			,PartesBusquedaEnMovimientosView.Marca
		FROM
			PartesBusquedaEnMovimientosView
		WHERE
			PartesBusquedaEnMovimientosView.NumeroParte = @Codigo
	END

END
SET NOCOUNT OFF
GO

/* 6 */
CREATE PROCEDURE [dbo].[pauParteBusquedaAvanzadaDeMovimientos] (
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
	, @ProveedorID INT = NULL
	
) AS BEGIN
	SET NOCOUNT ON

	IF @Codigo IS NULL BEGIN

		SELECT 
			MovimientoInventario.MovimientoInventarioID
			,MovimientoInventario.FolioFactura
			,MovimientoInventario.FechaFactura
			,MovimientoInventario.FechaRecepcion
			,MovimientoInventario.ImporteTotal
			,Usuario.NombreUsuario
		FROM
			MovimientoInventario
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
		WHERE
			MovimientoInventario.TipoOperacionID = 1 
			AND MovimientoInventario.FueLiquidado = 0	
			AND MovimientoInventario.Estatus = 1
			
			AND MovimientoInventario.ProveedorID = @ProveedorID
			AND MovimientoInventario.MovimientoInventarioID 
			IN (
				SELECT DISTINCT
					MovimientoInventarioDetalle.MovimientoInventarioID
				FROM	
					MovimientoInventarioDetalle
					INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID	
				WHERE	
					(@Descripcion1 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion1 + '%')
					AND (@Descripcion2 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion2 + '%')
					AND (@Descripcion3 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion3 + '%')
					AND (@Descripcion4 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion4 + '%')
					AND (@Descripcion5 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion5 + '%')
					AND (@Descripcion6 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion6 + '%')
					AND (@Descripcion7 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion7 + '%')
					AND (@Descripcion8 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion8 + '%')
					AND (@Descripcion9 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion9 + '%')
				)
				
		END ELSE BEGIN
		
		SELECT 
			MovimientoInventario.MovimientoInventarioID
			,MovimientoInventario.FolioFactura
			,MovimientoInventario.FechaFactura
			,MovimientoInventario.FechaRecepcion
			,MovimientoInventario.ImporteTotal
			,Usuario.NombreUsuario
		FROM
			MovimientoInventario
			INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
		WHERE
			MovimientoInventario.TipoOperacionID = 1 
			AND MovimientoInventario.FueLiquidado = 0	
			AND MovimientoInventario.Estatus = 1
			
			AND MovimientoInventario.ProveedorID = @ProveedorID
			AND MovimientoInventario.MovimientoInventarioID 
			IN (
				SELECT DISTINCT
					MovimientoInventarioDetalle.MovimientoInventarioID
				FROM	
					MovimientoInventarioDetalle
					INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID	
				WHERE	
					Parte.NumeroParte = @Codigo
				)
		
	END
	
	SET NOCOUNT OFF
END

/* 7 */
ALTER TABLE dbo.MovimientoInventario ADD
	UsuarioSolicitoTraspasoID int NULL,
	UsuarioRecibioTraspasoID int NULL,
	ExisteContingencia bit NULL
GO

/* 8 */
CREATE VIEW MovimientoInventarioTraspasosView
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
/* 9 */
ALTER TABLE dbo.MovimientoInventarioDetalle ADD
	CantidadRecibida decimal(18, 2) NULL
GO

/* 10 */
ALTER VIEW [dbo].[MovimientoInventarioDetalleView]
AS
SELECT 
	MovimientoInventarioDetalle.MovimientoInventarioDetalleID
	,MovimientoInventarioDetalle.MovimientoInventarioID
	,MovimientoInventarioDetalle.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,MovimientoInventarioDetalle.Cantidad
	,MovimientoInventarioDetalle.PrecioUnitario
	,MovimientoInventarioDetalle.Importe
	,MovimientoInventarioDetalle.FueDevolucion
	,MovimientoInventarioDetalle.FechaRegistro
	,MovimientoInventarioDetalle.CantidadRecibida
FROM 
	MovimientoInventarioDetalle
	INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioDetalle.ParteID
WHERE
	MovimientoInventarioDetalle.Estatus = 1
GO

/* 11 */
CREATE TABLE [dbo].[MovimientoInventarioTraspasoContingencia](
	[MovimientoInventarioTraspasoContingenciaID] [int] IDENTITY(1,1) NOT NULL,
	[MovimientoInventarioID] [int] NOT NULL,
	[MovimientoInventarioDetalleID] [int] NOT NULL,
	[ParteID] [int] NOT NULL,
	[CantidadEnviada] [decimal](18, 2) NULL,
	[CantidadRecibida] [decimal](18, 2) NULL,
	[Comentario] [nvarchar](255) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[Estatus] [bit] NOT NULL,
	[FueResuelto] [bit] NOT NULL,
 CONSTRAINT [PK_MovimientoInventarioTraspasoContingencia] PRIMARY KEY CLUSTERED 
(
	[MovimientoInventarioTraspasoContingenciaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioTraspasoContingencia_MovimientoInventario] FOREIGN KEY([MovimientoInventarioID])
REFERENCES [dbo].[MovimientoInventario] ([MovimientoInventarioID])
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] CHECK CONSTRAINT [FK_MovimientoInventarioTraspasoContingencia_MovimientoInventario]
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioTraspasoContingencia_Parte] FOREIGN KEY([ParteID])
REFERENCES [dbo].[Parte] ([ParteID])
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] CHECK CONSTRAINT [FK_MovimientoInventarioTraspasoContingencia_Parte]
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioTraspasoContingencia_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] CHECK CONSTRAINT [FK_MovimientoInventarioTraspasoContingencia_Usuario]
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioTraspasoContingencia_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioTraspasoContingencia_FueResuelto]  DEFAULT ((0)) FOR [FueResuelto]
GO


