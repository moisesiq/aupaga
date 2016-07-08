--1 Alter tabla MovimientoInventario
--2 Alter View
--3 Mod Tabla 

/* 1 */
ALTER TABLE dbo.MovimientoInventario ADD
	Articulos decimal(18, 2) NULL,
	Unidades decimal(18, 2) NULL,
	Seguro decimal(18, 2) NULL,
	ImporteTotalSinDescuento decimal(18, 2) NULL
GO
ALTER TABLE dbo.MovimientoInventario SET (LOCK_ESCALATION = TABLE)
GO

/* 2 */
ALTER VIEW [dbo].[MovimientoInventarioView]
AS
SELECT
	MovimientoInventario.MovimientoInventarioID
	,MovimientoInventario.TipoOperacionID
	,TipoOperacion.NombreTipoOperacion
	,MovimientoInventario.TipoPagoID	
	,TipoPago.NombreTipoPago
	,MovimientoInventario.ProveedorID	
	,Proveedor.NombreProveedor
	,MovimientoInventario.SucursalOrigenID	
	,s1.NombreSucursal AS SucursalOrigen
	,MovimientoInventario.SucursalDestinoID	
	,s2.NombreSucursal AS SucursalDestino
	,MovimientoInventario.FechaFactura	
	,MovimientoInventario.FechaRecepcion	
	,MovimientoInventario.FolioFactura	
	,MovimientoInventario.AplicaEnMovimientoInventarioID	
	,MovimientoInventario.FechaAplicacion	
	,MovimientoInventario.Subtotal	
	,MovimientoInventario.IVA	
	,MovimientoInventario.ImporteTotal	
	,MovimientoInventario.FueLiquidado	
	,MovimientoInventario.ConceptoMovimiento	
	,MovimientoInventario.Observacion	
	,MovimientoInventario.UsuarioID	
	,Usuario.NombreUsuario
	,MovimientoInventario.FechaRegistro	
	,MovimientoInventario.FechaModificacion	
	,MovimientoInventario.NombreImagen	
	,MovimientoInventario.Articulos
	,MovimientoInventario.Unidades
	,MovimientoInventario.Seguro
	,MovimientoInventario.ImporteTotalSinDescuento
FROM
	MovimientoInventario
	INNER JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
	INNER JOIN TipoOperacion ON TipoOperacion.TipoOperacionID = MovimientoInventario.TipoOperacionID
	INNER JOIN TipoPago ON TipoPago.TipoPagoID = MovimientoInventario.TipoPagoID
	INNER JOIN Sucursal s1 ON s1.SucursalID = MovimientoInventario.SucursalOrigenID
	INNER JOIN Sucursal s2 ON s2.SucursalID = MovimientoInventario.SucursalDestinoID
	INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
WHERE
	MovimientoInventario.Estatus = 1
GO

/* 3 */
/****** Object:  Table [dbo].[TipoConceptoOperacion]    Script Date: 09/27/2013 23:03:55 ******/
ALTER TABLE [dbo].[TipoConceptoOperacion] DROP CONSTRAINT [FK_TipoConceptoOperacion_Usuario]
GO
ALTER TABLE [dbo].[TipoConceptoOperacion] DROP CONSTRAINT [DF_TipoConceptoOperacion_Estatus]
GO
ALTER TABLE [dbo].[TipoConceptoOperacion] DROP CONSTRAINT [DF_TipoConceptoOperacion_Actualizar]
GO
DROP TABLE [dbo].[TipoConceptoOperacion]
GO
/****** Object:  Table [dbo].[TipoConceptoOperacion]    Script Date: 09/27/2013 23:03:55 ******/

CREATE TABLE [dbo].[TipoConceptoOperacion](
	[TipoConceptoOperacionID] [int] IDENTITY(1,1) NOT NULL,
	[TipoOperacionID] [int] NULL,
	[NombreConceptoOperacion] [nvarchar](100) NOT NULL,
	[Abreviacion] [nvarchar](5) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL CONSTRAINT [DF_TipoConceptoOperacion_Estatus]  DEFAULT ((1)),
	[Actualizar] [bit] NOT NULL CONSTRAINT [DF_TipoConceptoOperacion_Actualizar]  DEFAULT ((1)),
 CONSTRAINT [PK_TipoConceptoOperacion] PRIMARY KEY CLUSTERED 
(
	[TipoConceptoOperacionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TipoConceptoOperacion] ON
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, 2, N'ESTA EN ALMACEN', N'EAL', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, 2, N'OBSEQUIO', N'OBS', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, 3, N'NO ESTA EN ALMACEN', N'NAL', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (4, 3, N'OBSEQUIO', N'OBS', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (5, 3, N'MERMA', N'MER', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (6, 4, N'NO ES ARTICULO', N'NAR', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (7, 4, N'EXCESO DE INVENTARIO', N'EXC', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoConceptoOperacion] ([TipoConceptoOperacionID], [TipoOperacionID], [NombreConceptoOperacion], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (8, 4, N'NO SE PIDIO', N'NPD', 1, CAST(0x6D370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[TipoConceptoOperacion] OFF
/****** Object:  ForeignKey [FK_TipoConceptoOperacion_Usuario]    Script Date: 09/27/2013 23:03:55 ******/
ALTER TABLE [dbo].[TipoConceptoOperacion]  WITH CHECK ADD  CONSTRAINT [FK_TipoConceptoOperacion_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[TipoConceptoOperacion] CHECK CONSTRAINT [FK_TipoConceptoOperacion_Usuario]
GO