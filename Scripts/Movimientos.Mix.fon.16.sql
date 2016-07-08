/* 1 */
/* 1 */
CREATE TABLE [dbo].[PedidoEstatus](
	[PedidoEstatusID] [int] IDENTITY(1,1) NOT NULL,
	[NombrePedidoEstatus] [nvarchar](50) NOT NULL,
	[Abreviacion] [nvarchar](50) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_PedidoEstatus] PRIMARY KEY CLUSTERED 
(
	[PedidoEstatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[PedidoEstatus] ON
INSERT [dbo].[PedidoEstatus] ([PedidoEstatusID], [NombrePedidoEstatus], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'SURTIDO', N'S', 1, CAST(0xD8370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[PedidoEstatus] ([PedidoEstatusID], [NombrePedidoEstatus], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'NO SURTIDO', N'N', 1, CAST(0xD8370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[PedidoEstatus] ([PedidoEstatusID], [NombrePedidoEstatus], [Abreviacion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, N'CANCELADO', N'C', 1, CAST(0xD8370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[PedidoEstatus] OFF
/****** Object:  Default [DF_PedidoEstatus_Estatus]    Script Date: 11/21/2013 11:41:27 ******/
ALTER TABLE [dbo].[PedidoEstatus] ADD  CONSTRAINT [DF_PedidoEstatus_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[PedidoEstatus] ADD  CONSTRAINT [DF_PedidoEstatus_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[PedidoEstatus]  WITH CHECK ADD  CONSTRAINT [FK_PedidoEstatus_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[PedidoEstatus] CHECK CONSTRAINT [FK_PedidoEstatus_Usuario]
GO

/**/

CREATE TABLE [dbo].[Pedido](
	[PedidoID] [int] IDENTITY(1,1) NOT NULL,
	[ProveedorID] [int] NOT NULL,
	[ImporteTotal] [decimal](18, 2) NOT NULL,
	[PedidoEstatusID] [int] NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[FechaModificacion] [datetime] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_Pedido] PRIMARY KEY CLUSTERED 
(
	[PedidoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Pedido] ADD  CONSTRAINT [DF_Pedido_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[Pedido] ADD  CONSTRAINT [DF_Pedido_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[Pedido]  WITH CHECK ADD  CONSTRAINT [FK_Pedido_PedidoEstatus] FOREIGN KEY([PedidoEstatusID])
REFERENCES [dbo].[PedidoEstatus] ([PedidoEstatusID])
GO
ALTER TABLE [dbo].[Pedido] CHECK CONSTRAINT [FK_Pedido_PedidoEstatus]
GO

ALTER TABLE [dbo].[Pedido]  WITH CHECK ADD  CONSTRAINT [FK_Pedido_Proveedor] FOREIGN KEY([ProveedorID])
REFERENCES [dbo].[Proveedor] ([ProveedorID])
GO
ALTER TABLE [dbo].[Pedido] CHECK CONSTRAINT [FK_Pedido_Proveedor]
GO

ALTER TABLE [dbo].[Pedido]  WITH CHECK ADD  CONSTRAINT [FK_Pedido_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[Pedido] CHECK CONSTRAINT [FK_Pedido_Usuario]
GO

/* */
CREATE TABLE [dbo].[PedidoDetalle](
	[PedidoDetalleID] [int] IDENTITY(1,1) NOT NULL,
	[PedidoID] [int] NOT NULL,
	[ParteID] [int] NOT NULL,
	[PedidoEstatusID] [int] NOT NULL,
	[CantidadPedido] [decimal](18, 2) NOT NULL,
	[CantidadSurtida] [decimal](18, 2) NULL,
	[CostosUnitario] [decimal](18, 2) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[FechaModificacion] [datetime] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_PedidoDetalle] PRIMARY KEY CLUSTERED 
(
	[PedidoDetalleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PedidoDetalle] ADD  CONSTRAINT [DF_PedidoDetalle_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[PedidoDetalle] ADD  CONSTRAINT [DF_PedidoDetalle_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[PedidoDetalle]  WITH CHECK ADD  CONSTRAINT [FK_PedidoDetalle_Parte] FOREIGN KEY([ParteID])
REFERENCES [dbo].[Parte] ([ParteID])
GO
ALTER TABLE [dbo].[PedidoDetalle] CHECK CONSTRAINT [FK_PedidoDetalle_Parte]
GO

ALTER TABLE [dbo].[PedidoDetalle]  WITH CHECK ADD  CONSTRAINT [FK_PedidoDetalle_Pedido] FOREIGN KEY([PedidoID])
REFERENCES [dbo].[Pedido] ([PedidoID])
GO
ALTER TABLE [dbo].[PedidoDetalle] CHECK CONSTRAINT [FK_PedidoDetalle_Pedido]
GO

ALTER TABLE [dbo].[PedidoDetalle]  WITH CHECK ADD  CONSTRAINT [FK_PedidoDetalle_PedidoEstatus] FOREIGN KEY([PedidoEstatusID])
REFERENCES [dbo].[PedidoEstatus] ([PedidoEstatusID])
GO
ALTER TABLE [dbo].[PedidoDetalle] CHECK CONSTRAINT [FK_PedidoDetalle_PedidoEstatus]
GO

ALTER TABLE [dbo].[PedidoDetalle]  WITH CHECK ADD  CONSTRAINT [FK_PedidoDetalle_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[PedidoDetalle] CHECK CONSTRAINT [FK_PedidoDetalle_Usuario]
GO

/* 2 */
ALTER VIEW [dbo].[PedidosSugeridosView]
AS
SELECT
	Parte.ParteID
	,CAST(1 AS BIT) AS Sel
	,Parte.NumeroParte
	,Parte.NombreParte
	,ParteExistencia.UnidadEmpaque	
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
	ParteExistencia.UnidadEmpaque) * ParteExistencia.UnidadEmpaque AS Pedido	
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
GROUP BY
	Parte.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,ParteExistencia.UnidadEmpaque
	,Parte.ProveedorID
	,Proveedor.NombreProveedor
	,Proveedor.Beneficiario
	,PartePrecio.Costo
	,Parte.CriterioABC

GO

/* 3 */
CREATE VIEW PedidosView
AS
SELECT
	Pedido.PedidoID
	,Pedido.ProveedorID
	,Proveedor.NombreProveedor
	,Proveedor.Beneficiario
	,Pedido.ImporteTotal
	,CAST(Pedido.FechaRegistro AS DATE) AS Fecha
	,Pedido.FechaRegistro AS FechaRegistro
	,Pedido.PedidoEstatusID
	,PedidoEstatus.NombrePedidoEstatus
	,PedidoEstatus.Abreviacion
FROM
	Pedido
	INNER JOIN Proveedor ON Proveedor.ProveedorID = Pedido.ProveedorID
	INNER JOIN PedidoEstatus ON PedidoEstatus.PedidoEstatusID = Pedido.PedidoEstatusID
WHERE
	Pedido.Estatus = 1
GO

/* 4 */
CREATE VIEW PedidosDetalleView
AS
SELECT
	PedidoDetalle.PedidoDetalleID	
	,PedidoDetalle.PedidoID	
	,PedidoDetalle.ParteID	
	,Parte.NumeroParte
	,Parte.NombreParte
	,PedidoDetalle.PedidoEstatusID	
	,PedidoEstatus.NombrePedidoEstatus
	,PedidoEstatus.Abreviacion
	,PedidoDetalle.CantidadPedido	
	,PedidoDetalle.CantidadSurtida
	,PedidoDetalle.CostosUnitario
	,PedidoDetalle.FechaRegistro
	,CAST(PedidoDetalle.FechaRegistro AS DATE) AS Fecha
FROM
	PedidoDetalle
	INNER JOIN Parte ON Parte.ParteID = PedidoDetalle.ParteID
	INNER JOIN PedidoEstatus ON PedidoEstatus.PedidoEstatusID = PedidoDetalle.PedidoEstatusID	
WHERE
	PedidoDetalle.Estatus = 1
GO