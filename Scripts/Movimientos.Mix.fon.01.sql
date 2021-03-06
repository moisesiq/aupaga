--1 Creacion de la tabla TipoDescuento
--2 Creacion de la tabla TipoCilindro y TipoGarantia
--3 Agregar columnas relacionales TipoCilindroID y TipoGarantiaID a la tabla Parte
--4 Agregar columnas a la tabla de lineas para validacion de caracteristicas nuevas
--5 Alter LineasView
--6 Alter MotoresView
--7 Create Proc. upaPartesBusquedaEnMovimientos
--8 Alter View PartesBusquedaEnMovimientos
--9 Alter View PartesView
--10 Alter ProveedorPaqueteria

/* 1 */
USE [ControlRefaccionaria]
GO
/****** Object:  Table [dbo].[TipoDescuento]    Script Date: 08/15/2013 00:47:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoDescuento](
	[TipoDescuentoID] [int] IDENTITY(1,1) NOT NULL,
	[NombreTipoDescuento] [nvarchar](100) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_TipoDescuento] PRIMARY KEY CLUSTERED 
(
	[TipoDescuentoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TipoDescuento] ON
INSERT [dbo].[TipoDescuento] ([TipoDescuentoID], [NombreTipoDescuento], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'A ITEMS', 1, CAST(0x76370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoDescuento] ([TipoDescuentoID], [NombreTipoDescuento], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'A FACTURA', 1, CAST(0x76370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoDescuento] ([TipoDescuentoID], [NombreTipoDescuento], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, N'INDIVIDUAL', 1, CAST(0x76370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoDescuento] ([TipoDescuentoID], [NombreTipoDescuento], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (4, N'A MARCA ARTICULO', 1, CAST(0x76370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoDescuento] ([TipoDescuentoID], [NombreTipoDescuento], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (4, N'A MARCA FACTURA', 1, CAST(0x76370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[TipoDescuento] OFF
/****** Object:  Default [DF_TipoDescuento_Estatus]    Script Date: 08/15/2013 00:47:48 ******/
ALTER TABLE [dbo].[TipoDescuento] ADD  CONSTRAINT [DF_TipoDescuento_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
/****** Object:  Default [DF_TipoDescuento_Actualizar]    Script Date: 08/15/2013 00:47:48 ******/
ALTER TABLE [dbo].[TipoDescuento] ADD  CONSTRAINT [DF_TipoDescuento_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO
/****** Object:  ForeignKey [FK_TipoDescuento_Usuario]    Script Date: 08/15/2013 00:47:48 ******/
ALTER TABLE [dbo].[TipoDescuento]  WITH CHECK ADD  CONSTRAINT [FK_TipoDescuento_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[TipoDescuento] CHECK CONSTRAINT [FK_TipoDescuento_Usuario]
GO

/* 2 */
USE [ControlRefaccionaria]
GO
/****** Object:  Table [dbo].[TipoGarantia]    Script Date: 08/21/2013 12:45:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoGarantia](
	[TipoGarantiaID] [int] IDENTITY(1,1) NOT NULL,
	[NombreTipoGarantia] [nvarchar](50) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_TipoGarantia] PRIMARY KEY CLUSTERED 
(
	[TipoGarantiaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TipoGarantia] ON
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'N/A', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'1 MES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'2 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, N'3 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (4, N'4 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (5, N'5 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (6, N'6 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (7, N'7 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (8, N'8 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (9, N'9 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (10, N'10 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (11, N'11 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (12, N'12 MESES', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (13, N'1 AÑO', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (14, N'2 AÑO', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (15, N'3 AÑOS', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (16, N'4 AÑOS', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (17, N'5 AÑOS', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoGarantia] ([TipoGarantiaID], [NombreTipoGarantia], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (18, N'6 AÑOS', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[TipoGarantia] OFF
/****** Object:  Table [dbo].[TipoCilindro]    Script Date: 08/21/2013 12:45:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoCilindro](
	[TipoCilindroID] [int] IDENTITY(1,1) NOT NULL,
	[NombreTipoCilindro] [nvarchar](50) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_TipoCilindro] PRIMARY KEY CLUSTERED 
(
	[TipoCilindroID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TipoCilindro] ON
INSERT [dbo].[TipoCilindro] ([TipoCilindroID], [NombreTipoCilindro], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'N/A', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCilindro] ([TipoCilindroID], [NombreTipoCilindro], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'4', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCilindro] ([TipoCilindroID], [NombreTipoCilindro], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'5', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCilindro] ([TipoCilindroID], [NombreTipoCilindro], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, N'6', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCilindro] ([TipoCilindroID], [NombreTipoCilindro], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (4, N'8', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCilindro] ([TipoCilindroID], [NombreTipoCilindro], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (5, N'10', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[TipoCilindro] ([TipoCilindroID], [NombreTipoCilindro], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (6, N'12', 1, CAST(0x7C370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[TipoCilindro] OFF
/****** Object:  Default [DF_TipoCilindro_FechaRegistro]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoCilindro] ADD  CONSTRAINT [DF_TipoCilindro_FechaRegistro]  DEFAULT (getdate()) FOR [FechaRegistro]
GO
/****** Object:  Default [DF_TipoCilindro_Estatus]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoCilindro] ADD  CONSTRAINT [DF_TipoCilindro_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
/****** Object:  Default [DF_TipoCilindro_Actualizar]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoCilindro] ADD  CONSTRAINT [DF_TipoCilindro_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO
/****** Object:  Default [DF_TipoGarantia_FechaRegistro]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoGarantia] ADD  CONSTRAINT [DF_TipoGarantia_FechaRegistro]  DEFAULT (getdate()) FOR [FechaRegistro]
GO
/****** Object:  Default [DF_TipoGarantia_Estatus]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoGarantia] ADD  CONSTRAINT [DF_TipoGarantia_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
/****** Object:  Default [DF_TipoGarantia_Actualizar]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoGarantia] ADD  CONSTRAINT [DF_TipoGarantia_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO
/****** Object:  ForeignKey [FK_TipoCilindro_Usuario]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoCilindro]  WITH CHECK ADD  CONSTRAINT [FK_TipoCilindro_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[TipoCilindro] CHECK CONSTRAINT [FK_TipoCilindro_Usuario]
GO
/****** Object:  ForeignKey [FK_TipoGarantia_Usuario]    Script Date: 08/21/2013 12:45:30 ******/
ALTER TABLE [dbo].[TipoGarantia]  WITH CHECK ADD  CONSTRAINT [FK_TipoGarantia_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[TipoGarantia] CHECK CONSTRAINT [FK_TipoGarantia_Usuario]
GO

/* 3 */
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Parte ADD
	TipoCilindroID int NULL,
	TipoGarantiaID int NULL,
	Etiqueta bit NULL,
	SoloUnaEtiqueta bit NULL
GO
ALTER TABLE dbo.Parte SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/* 4 */
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Linea ADD
	Cilindros bit NULL,
	Garantia bit NULL
GO
ALTER TABLE dbo.Linea SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/* 5 */
ALTER VIEW [dbo].[LineasView]
AS
SELECT 
	Linea.LineaID
	,Linea.NombreLinea
	,Linea.Abreviacion
	,Sistema.NombreSistema AS Sistema	
	,Linea.Machote
	,CASE WHEN Linea.Alto = 1 THEN 'SI' ELSE 'NO' END AS Alto	
	,CASE WHEN Linea.Diametro = 1 THEN 'SI' ELSE 'NO' END AS Diametro
	,CASE WHEN Linea.Largo = 1 THEN 'SI' ELSE 'NO' END AS Largo
	,CASE WHEN Linea.Dientes = 1 THEN 'SI' ELSE 'NO' END AS Dientes
	,CASE WHEN Linea.Astrias = 1 THEN 'SI' ELSE 'NO' END AS Astrias
	,CASE WHEN Linea.Sistema = 1 THEN 'SI' ELSE 'NO' END AS Sistem@
	,CASE WHEN Linea.Capacidad = 1 THEN 'SI' ELSE 'NO' END AS Capacidad
	,CASE WHEN Linea.Amperaje = 1 THEN 'SI' ELSE 'NO' END AS Amperaje	
	,CASE WHEN Linea.Voltaje = 1 THEN 'SI' ELSE 'NO' END AS Voltaje
	,CASE WHEN Linea.Watts = 1 THEN 'SI' ELSE 'NO' END AS Watts
	,CASE WHEN Linea.Ubicacion = 1 THEN 'SI' ELSE 'NO' END AS Ubicacion
	,CASE WHEN Linea.Terminales = 1 THEN 'SI' ELSE 'NO' END AS Terminales
	,CASE WHEN Linea.Cilindros = 1 THEN 'SI' ELSE 'NO' END AS Cilindros
	,CASE WHEN Linea.Garantia = 1 THEN 'SI'	ELSE 'NO' END AS Garantia
	,CAST(Linea.LineaID AS VARCHAR) 
	+ Linea.NombreLinea
	+ CASE WHEN Linea.Abreviacion IS NULL THEN '' ELSE Linea.Abreviacion END 
	+ Sistema.NombreSistema AS Busqueda
FROM 
	Linea
	INNER JOIN Sistema ON Sistema.SistemaID = Linea.SistemaID
WHERE
	Linea.Estatus = 1

GO

/* 6 */
ALTER VIEW [dbo].[MotoresView]
AS
SELECT ISNULL(ROW_NUMBER() OVER(ORDER BY MotorID), -1)  AS GenericoID, *
FROM
(
	SELECT TOP 1000000 
		MIN(Motor.MotorID) AS MotorID
		,Marca.NombreMarca
		,Modelo.NombreModelo			
		,MIN(MotorAnio.Anio) AS AnioInicial
		,MAX(MotorAnio.Anio) AS AnioFinal
		,STUFF((SELECT ', ' + m.NombreMotor
		FROM Motor m	
		WHERE m.ModeloID = Modelo.ModeloID
		FOR XML PATH('')
		), 1, 1, '') AS Motor
		,Marca.NombreMarca
		+ Modelo.NombreModelo			
		+ CAST(MIN(MotorAnio.Anio) AS VARCHAR)
		+ CAST(MAX(MotorAnio.Anio) AS VARCHAR)
		+ STUFF((SELECT ', ' + m.NombreMotor
		FROM Motor m	
		WHERE m.ModeloID = Modelo.ModeloID
		FOR XML PATH('')
		), 1, 1, '') AS Busqueda
	FROM 
		Motor
		INNER JOIN Modelo ON Modelo.ModeloID = Motor.ModeloID	
		INNER JOIN Marca ON Marca.MarcaID = Modelo.MarcaID		
		INNER JOIN MotorAnio ON MotorAnio.MotorID = Motor.MotorID	
	WHERE
		Motor.Estatus = 1
	GROUP BY
		Marca.NombreMarca
		,Modelo.NombreModelo
		,Modelo.ModeloID		
	ORDER BY 
		Marca.NombreMarca
		,Modelo.NombreModelo
)t

/* 7 */
CREATE PROCEDURE [dbo].[upaPartesBusquedaEnMovimientos] @filtro NVARCHAR(50)                           
AS
SET NOCOUNT ON
SELECT
	PartesBusquedaEnMovimientos.ParteID
	,PartesBusquedaEnMovimientos.NumeroParte
	,PartesBusquedaEnMovimientos.NombreParte
	,PartesBusquedaEnMovimientos.Linea
	,PartesBusquedaEnMovimientos.Marca
FROM
	PartesBusquedaEnMovimientos
WHERE
	PartesBusquedaEnMovimientos.Busqueda LIKE @filtro
SET NOCOUNT OFF
GO

/* 8 */
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[PartesBusquedaEnMovimientos]'))
DROP VIEW [dbo].[PartesBusquedaEnMovimientos]
GO
CREATE VIEW [dbo].[PartesBusquedaEnMovimientosView]
AS
SELECT
	Parte.ParteID
	,MarcaParte.MarcaParteID
	,Linea.LineaID
	,Parte.NumeroParte
	,Parte.NombreParte	
	,Linea.NombreLinea AS Linea		
	,MarcaParte.NombreMarcaParte AS Marca
	,PartePrecio.PartePrecioID
	,PartePrecio.Costo
	,PartePrecio.PorcentajeUtilidadUno
	,PartePrecio.PorcentajeUtilidadDos
	,PartePrecio.PorcentajeUtilidadTres
	,PartePrecio.PorcentajeUtilidadCuatro
	,PartePrecio.PorcentajeUtilidadCinco
	,PartePrecio.PrecioUno
	,PartePrecio.PrecioDos
	,PartePrecio.PrecioTres
	,PartePrecio.PrecioCuatro
	,PartePrecio.PrecioCinco
	,Parte.NumeroParte
	+ Parte.NombreParte	
	+ Linea.NombreLinea 	
	+ MarcaParte.NombreMarcaParte AS Busqueda
FROM
	Parte
	INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
	INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
	INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
WHERE
	Parte.Estatus = 1

/* 9 */
ALTER VIEW [dbo].[PartesView]
AS
SELECT TOP 1000000000000
	Parte.ParteID
	,Parte.NumeroParte 
	,Parte.NombreParte AS Descripcion
	,MarcaParte.NombreMarcaParte AS Marca
	,Linea.NombreLinea AS Linea
	,MAX(CASE 
	WHEN (Linea.Alto = 1 AND Parte.Alto IS NULL) OR 
		(Linea.Diametro = 1 AND Parte.Diametro IS NULL) OR
		(Linea.Largo = 1 AND Parte.Largo IS NULL) OR
		(Linea.Dientes = 1 AND Parte.Dientes IS NULL) OR
		(Linea.Astrias = 1 AND Parte.Astrias IS NULL) OR
		(Linea.Sistema = 1 AND Parte.ParteSistemaID IS NULL) OR	
		(Linea.Amperaje = 1 AND Parte.Amperes IS NULL) OR
		(Linea.Voltaje = 1 AND Parte.Voltios IS NULL) OR
		(Linea.Watts = 1 AND Parte.Watts IS NULL) OR
		(Linea.Ubicacion = 1 AND Parte.ParteUbicacionID IS NULL) OR
		(Linea.Terminales = 1 AND Parte.Terminales IS NULL) THEN 'SI' ELSE 'NO' END) AS FaltanCaracteristicas
	,ParteEstatus.ParteEstatusID
	,Parte.LineaID
	,MAX(CASE WHEN Parte.ParteID = ParteVehiculo.ParteID THEN 'SI' ELSE 'NO' END) AS Aplicacion
	,MAX(CASE WHEN Parte.ParteID = ParteEquivalente.ParteID THEN 'SI' ELSE 'NO' END) AS Equivalente		
	,Parte.NumeroParte 
	+ Parte.NombreParte
	+ MarcaParte.NombreMarcaParte
	+ Linea.NombreLinea AS Busqueda
FROM 
	Parte
	INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
	INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
	INNER JOIN ParteEstatus ON ParteEstatus.ParteEstatusID = Parte.ParteEstatusID
	LEFT JOIN ParteVehiculo ON ParteVehiculo.ParteID = Parte.ParteID
	LEFT JOIN ParteEquivalente ON ParteEquivalente.ParteID = Parte.ParteID	
WHERE
	Parte.Estatus = 1
GROUP BY
	Parte.ParteID
	,Parte.NumeroParte 
	,Parte.NombreParte
	,MarcaParte.NombreMarcaParte
	,ParteEstatus.ParteEstatusID
	,Linea.NombreLinea
	,Parte.LineaID
ORDER BY 
	Parte.NombreParte

GO

/* 10 */
ALTER TABLE dbo.ProveedorPaqueteria ADD
 CostoPaqueteria decimal(18, 2) NULL