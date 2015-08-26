/* 1 */
CREATE TABLE [dbo].[MovimientoInventarioEstatusContingencia](
	[MovimientoInventarioEstatusContingenciaID] [int] IDENTITY(1,1) NOT NULL,
	[NombreEstatusContingencia] [nvarchar](50) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_MovimientoInventarioEstatusContingencia] PRIMARY KEY CLUSTERED 
(
	[MovimientoInventarioEstatusContingenciaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[MovimientoInventarioEstatusContingencia] ON
INSERT [dbo].[MovimientoInventarioEstatusContingencia] ([MovimientoInventarioEstatusContingenciaID], [NombreEstatusContingencia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'SOLUCIONADO', 1, CAST(0xB2370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[MovimientoInventarioEstatusContingencia] ([MovimientoInventarioEstatusContingenciaID], [NombreEstatusContingencia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'NO SOLUCIONADO', 1, CAST(0xB2370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[MovimientoInventarioEstatusContingencia] OFF
/****** Object:  Default [DF_MovimientoInventarioEstatusContingencia_Estatus]    Script Date: 10/15/2013 01:58:37 ******/
ALTER TABLE [dbo].[MovimientoInventarioEstatusContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioEstatusContingencia_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
/****** Object:  Default [DF_MovimientoInventarioEstatusContingencia_Actualizar]    Script Date: 10/15/2013 01:58:37 ******/
ALTER TABLE [dbo].[MovimientoInventarioEstatusContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioEstatusContingencia_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO
/****** Object:  ForeignKey [FK_MovimientoInventarioEstatusContingencia_Usuario]    Script Date: 10/15/2013 01:58:37 ******/
ALTER TABLE [dbo].[MovimientoInventarioEstatusContingencia]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioEstatusContingencia_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[MovimientoInventarioEstatusContingencia] CHECK CONSTRAINT [FK_MovimientoInventarioEstatusContingencia_Usuario]
GO


/* 2 */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MovimientoInventarioTraspasoContingencia]') AND type in (N'U'))
DROP TABLE [dbo].[MovimientoInventarioTraspasoContingencia]
GO

/* 3 */
CREATE TABLE [dbo].[MovimientoInventarioTraspasoContingencia](
	[MovimientoInventarioTraspasoContingenciaID] [int] IDENTITY(1,1) NOT NULL,
	[MovimientoInventarioID] [int] NOT NULL,
	[MovimientoInventarioDetalleID] [int] NOT NULL,
	[ParteID] [int] NOT NULL,
	[CantidadEnviada] [decimal](18, 2) NULL,
	[CantidadRecibida] [decimal](18, 2) NULL,
	[CantidadDiferencia] [decimal](18, 2) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
	[MovimientoInventarioEstatusContingenciaID] [int] NULL,
	[UsuarioSolucionoID] [int] NULL,
	[FechaSoluciono] [datetime] NULL,
	[TipoOperacionID] [int] NULL,
	[TipoConceptoOperacionID] [int] NULL,
	[Comentario] [nvarchar](255) NULL,
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

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioTraspasoContingencia_Estatus_1]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioTraspasoContingencia_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioTraspasoContingencia_Estatus]  DEFAULT ((1)) FOR [MovimientoInventarioEstatusContingenciaID]
GO

ALTER TABLE [dbo].[MovimientoInventarioTraspasoContingencia] ADD  CONSTRAINT [DF_MovimientoInventarioTraspasoContingencia_FueResuelto]  DEFAULT ((0)) FOR [UsuarioSolucionoID]
GO

/* 4 */
CREATE VIEW MovimientoInventarioTraspasosHisView
AS
SELECT
	MovimientoInventarioDetalle.MovimientoInventarioDetalleID
	,MovimientoInventarioDetalle.MovimientoInventarioID
	,MovimientoInventarioDetalle.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte	
	,MovimientoInventarioDetalle.Cantidad AS Enviado
	,ISNULL(MovimientoInventarioTraspasoContingencia.CantidadRecibida, 0) AS Recibido
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

/* 5 */
CREATE VIEW MovimientoInventarioContingenciasView
AS
SELECT
	Contingencia.MovimientoInventarioTraspasoContingenciaID	
	,Contingencia.MovimientoInventarioID	
	,Contingencia.MovimientoInventarioDetalleID	
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
	,Contingencia.Comentario
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
WHERE
	Contingencia.Estatus = 1
	AND MovimientoInventario.Estatus = 1
GO