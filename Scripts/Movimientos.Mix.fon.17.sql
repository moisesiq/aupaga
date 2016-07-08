CREATE TABLE [dbo].[Estado](
	[EstadoID] [int] NOT NULL,
	[NombreEstado] [nvarchar](255) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_Estado] PRIMARY KEY CLUSTERED 
(
	[EstadoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Municipio](
	[MunicipioID] [int] NOT NULL,
	[EstadoID] [int] NOT NULL,
	[NombreMunicipio] [nvarchar](255) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_Municipio] PRIMARY KEY CLUSTERED 
(
	[MunicipioID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ciudad](
	[CiudadID] [int] NOT NULL,
	[MunicipioID] [int] NOT NULL,
	[NombreCiudad] [nvarchar](255) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_Ciudad] PRIMARY KEY CLUSTERED 
(
	[CiudadID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Ciudad] ADD  CONSTRAINT [DF_Ciudad_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[Ciudad] ADD  CONSTRAINT [DF_Ciudad_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[Estado] ADD  CONSTRAINT [DF_Estado_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[Estado] ADD  CONSTRAINT [DF_Estado_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[Municipio] ADD  CONSTRAINT [DF_Municipio_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[Municipio] ADD  CONSTRAINT [DF_Municipio_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[Ciudad]  WITH CHECK ADD  CONSTRAINT [FK_Ciudad_Municipio] FOREIGN KEY([MunicipioID])
REFERENCES [dbo].[Municipio] ([MunicipioID])
GO
ALTER TABLE [dbo].[Ciudad] CHECK CONSTRAINT [FK_Ciudad_Municipio]
GO

ALTER TABLE [dbo].[Ciudad]  WITH CHECK ADD  CONSTRAINT [FK_Ciudad_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[Ciudad] CHECK CONSTRAINT [FK_Ciudad_Usuario]
GO

ALTER TABLE [dbo].[Estado]  WITH CHECK ADD  CONSTRAINT [FK_Estado_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[Estado] CHECK CONSTRAINT [FK_Estado_Usuario]
GO

ALTER TABLE [dbo].[Municipio]  WITH CHECK ADD  CONSTRAINT [FK_Municipio_Estado] FOREIGN KEY([EstadoID])
REFERENCES [dbo].[Estado] ([EstadoID])
GO
ALTER TABLE [dbo].[Municipio] CHECK CONSTRAINT [FK_Municipio_Estado]
GO

ALTER TABLE [dbo].[Municipio]  WITH CHECK ADD  CONSTRAINT [FK_Municipio_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[Municipio] CHECK CONSTRAINT [FK_Municipio_Usuario]
GO

/* Clientes */
ALTER TABLE Cliente ALTER COLUMN Nombre nvarchar(200)
ALTER TABLE Cliente ALTER COLUMN Calle nvarchar(200)
ALTER TABLE Cliente ALTER COLUMN Colonia nvarchar(50)
GO
/* */
ALTER TABLE ClienteFacturacion ALTER COLUMN RazonSocial nvarchar(200)
ALTER TABLE ClienteFacturacion ALTER COLUMN Calle nvarchar(200)
ALTER TABLE ClienteFacturacion ALTER COLUMN Colonia nvarchar(50)
GO
/* */
ALTER TABLE dbo.Cliente ADD
	EstadoID int NULL,
	MunicipioID int NULL,
	CiudadID int NULL,
	TipoFormaPagoID int NULL,
	BancoID int NULL,
	TipoClienteID int NULL,
	Particular nvarchar(16) NULL,
	Nextel nvarchar(50) NULL,
	CuentaBancaria nvarchar(4) NULL,
	Tolerancia bit NULL,
	LimiteCredito decimal(18, 2) NULL,
	EsClienteComisionista bit NULL,
	EsTallerElectrico bit NULL,
	EsTallerMecanico bit NULL,
	EsTallerDiesel bit NULL
GO

/* */
ALTER TABLE dbo.ClienteFacturacion ADD
	EstadoID int NULL,
	MunicipioID int NULL,
	CiudadID int NULL
GO

/* TipoCliente */
CREATE TABLE [dbo].[TipoCliente](
	[TipoClienteID] [int] IDENTITY(1,1) NOT NULL,
	[NombreTipoCliente] [nvarchar](50) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_TipoCliente] PRIMARY KEY CLUSTERED 
(
	[TipoClienteID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TipoCliente] ON
INSERT [dbo].[TipoCliente] ([TipoClienteID], [NombreTipoCliente], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'TALLER', 1, CAST(0xDD370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCliente] ([TipoClienteID], [NombreTipoCliente], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'EMPRESA', 1, CAST(0xDD370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCliente] ([TipoClienteID], [NombreTipoCliente], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, N'PARTICULAR', 1, CAST(0xDD370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCliente] ([TipoClienteID], [NombreTipoCliente], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (4, N'AMIGO-FAMILIAR', 1, CAST(0xDD370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[TipoCliente] OFF

ALTER TABLE [dbo].[TipoCliente] ADD  CONSTRAINT [DF_TipoCliente_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[TipoCliente] ADD  CONSTRAINT [DF_TipoCliente_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[TipoCliente]  WITH CHECK ADD  CONSTRAINT [FK_TipoCliente_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO

/* ClientePersonal */

CREATE TABLE [dbo].[ClientePersonal](
	[ClientePersonalID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NOT NULL,
	[NombrePersonal] [nvarchar](50) NOT NULL,
	[CorreoElectronico] [nvarchar](50) NULL,
	[EnviarCfdi] [bit] NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_ClientePersonal] PRIMARY KEY CLUSTERED 
(
	[ClientePersonalID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ClientePersonal] ADD  CONSTRAINT [DF_ClientePersonal_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[ClientePersonal] ADD  CONSTRAINT [DF_ClientePersonal_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[ClientePersonal]  WITH CHECK ADD  CONSTRAINT [FK_ClientePersonal_Cliente] FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])
GO
ALTER TABLE [dbo].[ClientePersonal] CHECK CONSTRAINT [FK_ClientePersonal_Cliente]
GO

ALTER TABLE [dbo].[ClientePersonal]  WITH CHECK ADD  CONSTRAINT [FK_ClientePersonal_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[ClientePersonal] CHECK CONSTRAINT [FK_ClientePersonal_Usuario]
GO
