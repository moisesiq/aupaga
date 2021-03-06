--1 Tabla de Etiquetas
--2 Tabla de Precio Historico
--3 View movimientoInventarioEtiquetasView
--4 Tabla Movimientos Descuentos

/* 1 */
CREATE TABLE [dbo].[MovimientoInventarioEtiqueta](
	[MovimientoInventarioEtiquetaID] [int] IDENTITY(1,1) NOT NULL,
	[MovimientoInventarioID] [int] NOT NULL,
	[ParteID] [int] NOT NULL,
	[NumeroEtiquetas] [int] NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_MovimientoInventarioEtiqueta] PRIMARY KEY CLUSTERED 
(
	[MovimientoInventarioEtiquetaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Default [DF_MovimientoInventarioEtiqueta_Estatus]    Script Date: 09/18/2013 18:53:27 ******/
ALTER TABLE [dbo].[MovimientoInventarioEtiqueta] ADD  CONSTRAINT [DF_MovimientoInventarioEtiqueta_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
/****** Object:  Default [DF_MovimientoInventarioEtiqueta_Actualizar]    Script Date: 09/18/2013 18:53:27 ******/
ALTER TABLE [dbo].[MovimientoInventarioEtiqueta] ADD  CONSTRAINT [DF_MovimientoInventarioEtiqueta_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO
/****** Object:  ForeignKey [FK_MovimientoInventarioEtiqueta_MovimientoInventario]    Script Date: 09/18/2013 18:53:27 ******/
ALTER TABLE [dbo].[MovimientoInventarioEtiqueta]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioEtiqueta_MovimientoInventario] FOREIGN KEY([MovimientoInventarioID])
REFERENCES [dbo].[MovimientoInventario] ([MovimientoInventarioID])
GO
ALTER TABLE [dbo].[MovimientoInventarioEtiqueta] CHECK CONSTRAINT [FK_MovimientoInventarioEtiqueta_MovimientoInventario]
GO
/****** Object:  ForeignKey [FK_MovimientoInventarioEtiqueta_Usuario]    Script Date: 09/18/2013 18:53:27 ******/
ALTER TABLE [dbo].[MovimientoInventarioEtiqueta]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioEtiqueta_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[MovimientoInventarioEtiqueta] CHECK CONSTRAINT [FK_MovimientoInventarioEtiqueta_Usuario]
GO

/* 2 */
DROP TABLE [dbo].[PartePrecioHistorico]
GO
/****** Object:  Table [dbo].[PartePrecioHistorico]    Script Date: 09/18/2013 21:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartePrecioHistorico](
	[PartePrecioHistoricoID] [int] IDENTITY(1,1) NOT NULL,
	[MovimientoInventarioID] [int] NOT NULL,
	[ParteID] [int] NOT NULL,
	[CostoActual] [decimal](18, 2) NULL,
	[CostoNuevo] [decimal](18, 2) NULL,
	[CostoConDescuento] [decimal](18, 2) NULL,
	[CostoConDescuentoMasFlete] [decimal](18, 2) NULL,
	[PorcentajeUtilidadUno] [decimal](18, 2) NULL,
	[PorcentajeUtilidadDos] [decimal](18, 2) NULL,
	[PorcentajeUtilidadTres] [decimal](18, 2) NULL,
	[PorcentajeUtilidadCuatro] [decimal](18, 2) NULL,
	[PorcentajeUtilidadCinco] [decimal](18, 2) NULL,
	[PrecioUno] [decimal](18, 2) NULL,
	[PrecioDos] [decimal](18, 2) NULL,
	[PrecioTres] [decimal](18, 2) NULL,
	[PrecioCuatro] [decimal](18, 2) NULL,
	[PrecioCinco] [decimal](18, 2) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL CONSTRAINT [DF_PartePrecioHistorico_Estatus]  DEFAULT ((1)),
	[Actualizar] [bit] NOT NULL CONSTRAINT [DF_PartePrecioHistorico_Actualizar]  DEFAULT ((1)),
 CONSTRAINT [PK_PartePrecioHistorico] PRIMARY KEY CLUSTERED 
(
	[PartePrecioHistoricoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_PartePrecioHistorico_MovimientoInventario]    Script Date: 09/18/2013 21:10:54 ******/
ALTER TABLE [dbo].[PartePrecioHistorico]  WITH CHECK ADD  CONSTRAINT [FK_PartePrecioHistorico_MovimientoInventario] FOREIGN KEY([MovimientoInventarioID])
REFERENCES [dbo].[MovimientoInventario] ([MovimientoInventarioID])
GO
ALTER TABLE [dbo].[PartePrecioHistorico] CHECK CONSTRAINT [FK_PartePrecioHistorico_MovimientoInventario]
GO
/****** Object:  ForeignKey [FK_PartePrecioHistorico_Parte]    Script Date: 09/18/2013 21:10:54 ******/
ALTER TABLE [dbo].[PartePrecioHistorico]  WITH CHECK ADD  CONSTRAINT [FK_PartePrecioHistorico_Parte] FOREIGN KEY([ParteID])
REFERENCES [dbo].[Parte] ([ParteID])
GO
ALTER TABLE [dbo].[PartePrecioHistorico] CHECK CONSTRAINT [FK_PartePrecioHistorico_Parte]
GO
/****** Object:  ForeignKey [FK_PartePrecioHistorico_Usuario]    Script Date: 09/18/2013 21:10:54 ******/
ALTER TABLE [dbo].[PartePrecioHistorico]  WITH CHECK ADD  CONSTRAINT [FK_PartePrecioHistorico_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[PartePrecioHistorico] CHECK CONSTRAINT [FK_PartePrecioHistorico_Usuario]
GO

/* 3 */
CREATE VIEW MovimientoInventarioEtiquetasView
AS
SELECT
	MovimientoInventarioEtiqueta.MovimientoInventarioEtiquetaID
	,MovimientoInventarioEtiqueta.MovimientoInventarioID
	,MovimientoInventarioEtiqueta.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,Parte.CodigoBarra
	,MovimientoInventarioEtiqueta.NumeroEtiquetas
FROM
	MovimientoInventarioEtiqueta
	INNER JOIN Parte ON Parte.ParteID = MovimientoInventarioEtiqueta.ParteID
WHERE
	MovimientoInventarioEtiqueta.Estatus = 1	
GO

/* 4 */
--DROP TABLE [dbo].[MovimientoInventarioDescuento]
--GO
/****** Object:  Table [dbo].[MovimientoInventarioDescuento]    Script Date: 09/19/2013 01:39:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MovimientoInventarioDescuento](
	[MovimientoInventarioDescuentoID] [int] IDENTITY(1,1) NOT NULL,
	[MovimientoInventarioID] [int] NOT NULL,
	[TipoDescuentoID] [int] NOT NULL,
	[ParteID] [int] NOT NULL,
	[DescuentoUno] [decimal](18, 2) NOT NULL,
	[DescuentoDos] [decimal](18, 2) NOT NULL,
	[DescuentoTres] [decimal](18, 2) NOT NULL,
	[DescuentoCuatro] [decimal](18, 2) NOT NULL,
	[DescuentoCinco] [decimal](18, 2) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL CONSTRAINT [DF_MovimientoInventarioDescuento_Estatus]  DEFAULT ((1)),
	[Actualizar] [bit] NOT NULL CONSTRAINT [DF_MovimientoInventarioDescuento_Actualizar]  DEFAULT ((1)),
 CONSTRAINT [PK_MovimientoInventarioDescuento] PRIMARY KEY CLUSTERED 
(
	[MovimientoInventarioDescuentoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_MovimientoInventarioDescuento_MovimientoInventario]    Script Date: 09/19/2013 01:39:31 ******/
ALTER TABLE [dbo].[MovimientoInventarioDescuento]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioDescuento_MovimientoInventario] FOREIGN KEY([MovimientoInventarioID])
REFERENCES [dbo].[MovimientoInventario] ([MovimientoInventarioID])
GO
ALTER TABLE [dbo].[MovimientoInventarioDescuento] CHECK CONSTRAINT [FK_MovimientoInventarioDescuento_MovimientoInventario]
GO
/****** Object:  ForeignKey [FK_MovimientoInventarioDescuento_Usuario]    Script Date: 09/19/2013 01:39:31 ******/
ALTER TABLE [dbo].[MovimientoInventarioDescuento]  WITH CHECK ADD  CONSTRAINT [FK_MovimientoInventarioDescuento_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[MovimientoInventarioDescuento] CHECK CONSTRAINT [FK_MovimientoInventarioDescuento_Usuario]
GO
