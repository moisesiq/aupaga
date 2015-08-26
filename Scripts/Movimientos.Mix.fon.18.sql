/**/
ALTER TABLE dbo.Cliente ADD
	ClienteComisionistaID int NULL
GO

/**/
CREATE VIEW ClientesView
AS
SELECT
	Cliente.ClienteID
	,ClienteFacturacion.Rfc
	,Cliente.Nombre
	,Cliente.Calle
	,Cliente.NumeroInterior
	,Cliente.NumeroExterior
	,Cliente.Colonia
	,Cliente.CodigoPostal
	,Cliente.EstadoID
	,Estado.NombreEstado
	,Cliente.MunicipioID
	,Municipio.NombreMunicipio
	,Cliente.CiudadID
	,Ciudad.NombreCiudad
	,Cliente.Telefono
	,Cliente.Celular
	,Cliente.Particular
	,Cliente.Nextel
	,Cliente.ListaDePrecios
	,ISNULL(Cliente.TieneCredito, 0) AS TieneCredito
	,Cliente.Credito
	,Cliente.ePoints
	,Cliente.DiasDeCredito
	,Cliente.TipoFormaPagoID
	,TipoFormaPago.NombreTipoFormaPago
	,Cliente.BancoID
	,Banco.NombreBanco
	,Cliente.TipoClienteID
	,TipoCliente.NombreTipoCliente
	,Cliente.CuentaBancaria
	,ISNULL(Cliente.Tolerancia, 0) AS Tolerancia
	,Cliente.LimiteCredito
	,ISNULL(Cliente.EsClienteComisionista, 0) AS EsClienteComisionista
	,ISNULL(Cliente.EsTallerElectrico, 0) AS EsTallerElectrico
	,ISNULL(Cliente.EsTallerMecanico, 0) AS EsTallerMecanico
	,ISNULL(Cliente.EsTallerDiesel, 0) AS EsTallerDiesel
	,Cliente.ClienteComisionistaID
	,Com.Nombre AS NombreClienteComisionista
	,Cliente.FechaRegistro
	,Cliente.Alias
FROM
	Cliente
	LEFT JOIN ClienteFacturacion ON ClienteFacturacion.ClienteID = Cliente.ClienteID
	INNER JOIN Estado ON Estado.EstadoID = Cliente.EstadoID
	INNER JOIN Municipio ON Municipio.MunicipioID = Cliente.MunicipioID
	INNER JOIN Ciudad ON Ciudad.CiudadID = Cliente.CiudadID
	LEFT JOIN TipoFormaPago ON TipoFormaPago.TipoFormaPagoID = Cliente.TipoFormaPagoID
	LEFT JOIN Banco ON Banco.BancoID = Cliente.BancoID
	LEFT JOIN TipoCliente ON TipoCliente.TipoClienteID = Cliente.TipoClienteID
	LEFT JOIN Cliente Com ON Com.ClienteComisionistaID = Cliente.ClienteID
WHERE
	Cliente.Estatus = 1
	
/**/
ALTER TABLE dbo.Linea ADD
	SubsistemaID int NULL
GO

/**/
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
ALTER TABLE [dbo].[ClientePersonal] ADD  CONSTRAINT [DF_ClientePersonal_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
ALTER TABLE [dbo].[ClientePersonal] ADD  CONSTRAINT [DF_ClientePersonal_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

/**/
CREATE TABLE [dbo].[ClienteCredito](
	[ClienteCreditoID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NOT NULL,
	[Accion] [nvarchar](255) NOT NULL,
	[Comentario] [nvarchar](255) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_ClienteCredito] PRIMARY KEY CLUSTERED 
(
	[ClienteCreditoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClienteCredito]  WITH CHECK ADD  CONSTRAINT [FK_ClienteCredito_Cliente] FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])
GO
ALTER TABLE [dbo].[ClienteCredito] CHECK CONSTRAINT [FK_ClienteCredito_Cliente]
GO
ALTER TABLE [dbo].[ClienteCredito]  WITH CHECK ADD  CONSTRAINT [FK_ClienteCredito_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[ClienteCredito] CHECK CONSTRAINT [FK_ClienteCredito_Usuario]
GO
ALTER TABLE [dbo].[ClienteCredito] ADD  CONSTRAINT [DF_ClienteCredito_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
ALTER TABLE [dbo].[ClienteCredito] ADD  CONSTRAINT [DF_ClienteCredito_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

/**/
CREATE VIEW ClientePersonalView
AS
SELECT 
	ClientePersonal.ClientePersonalID
	,ClientePersonal.ClienteID	
	,ClientePersonal.CorreoElectronico
	,CASE WHEN ClientePersonal.EnviarCFDI = 1 THEN 'SI' ELSE 'NO' END AS CFDI	
	,ClientePersonal.NombrePersonal AS Usuario
FROM
	ClientePersonal
WHERE
	ClientePersonal.Estatus = 1
GO

/**/
CREATE VIEW ClienteCreditoView
AS
SELECT
	ClienteCredito.ClienteCreditoID
	,ClienteCredito.ClienteID
	,ClienteCredito.FechaRegistro AS Fecha
	,ClienteCredito.UsuarioID
	,Usuario.NombreUsuario AS Usuario
	,ClienteCredito.Accion
	,ClienteCredito.Comentario
FROM
	ClienteCredito
	INNER JOIN Usuario ON Usuario.UsuarioID = ClienteCredito.UsuarioID
WHERE
	ClienteCredito.Estatus = 1
GO

/***/
ALTER TABLE dbo.Proveedor ADD
	MontoPaqueteria decimal(18, 2) NULL
GO