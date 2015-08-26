--1 Tabla MxMnCriterio
--2 Tabla ParteMxMn
--3 Tabla ParteMxMnCriterio
--4 Funciones
--5 MxMnView

/* 1 */
CREATE TABLE [dbo].[MxMnCriterio](
	[MxMnCriterioID] [int] IDENTITY(1,1) NOT NULL,
	[NombreCriterio] [nvarchar](50) NOT NULL,
	[Expresion] [nvarchar](255) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_MxMnCriterio] PRIMARY KEY CLUSTERED 
(
	[MxMnCriterioID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[MxMnCriterio] ON
INSERT [dbo].[MxMnCriterio] ([MxMnCriterioID], [NombreCriterio], [Expresion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'UNO', N'[Mn](>){0}$[Mn](<){2}:[MIN]={1}', 1, CAST(0xC0370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[MxMnCriterio] ([MxMnCriterioID], [NombreCriterio], [Expresion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'DOS', N'[Es9500](=){1}:[Mn](=){0},[MAX](=){0}', 1, CAST(0xC0370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[MxMnCriterio] ([MxMnCriterioID], [NombreCriterio], [Expresion], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, N'TRES', N'[Mn](>){0}$[Mn]=[Mx]:[MIN](=)[MIN](-){1}', 1, CAST(0xC0370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[MxMnCriterio] OFF
/****** Object:  Default [DF_MxMnCriterio_Estatus]    Script Date: 10/29/2013 02:23:42 ******/
ALTER TABLE [dbo].[MxMnCriterio] ADD  CONSTRAINT [DF_MxMnCriterio_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
/****** Object:  Default [DF_MxMnCriterio_Actualizar]    Script Date: 10/29/2013 02:23:42 ******/
ALTER TABLE [dbo].[MxMnCriterio] ADD  CONSTRAINT [DF_MxMnCriterio_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO
/****** Object:  ForeignKey [FK_MxMnCriterio_Usuario]    Script Date: 10/29/2013 02:23:42 ******/
ALTER TABLE [dbo].[MxMnCriterio]  WITH CHECK ADD  CONSTRAINT [FK_MxMnCriterio_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[MxMnCriterio] CHECK CONSTRAINT [FK_MxMnCriterio_Usuario]
GO

/* 2 */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ParteMxMn]') AND type in (N'U'))
DROP TABLE [dbo].[ParteMxMn]
GO

CREATE TABLE [dbo].[ParteMxMn](
	[ParteMxMnID] [int] IDENTITY(1,1) NOT NULL,
	[ParteID] [int] NOT NULL,
	[SucursalID] [int] NOT NULL,
	[EsFijo] [bit] NOT NULL,
	[Criterio] [nvarchar](10) NULL,
	[AjusteMx] [nvarchar](10) NULL,
	[AjusteMn] [nvarchar](10) NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_ParteMxMn] PRIMARY KEY CLUSTERED 
(
	[ParteMxMnID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ParteMxMn]  WITH CHECK ADD  CONSTRAINT [FK_ParteMxMn_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[ParteMxMn] CHECK CONSTRAINT [FK_ParteMxMn_Usuario]
GO
ALTER TABLE [dbo].[ParteMxMn] ADD  CONSTRAINT [DF_ParteMxMn_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
ALTER TABLE [dbo].[ParteMxMn] ADD  CONSTRAINT [DF_ParteMxMn_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

/* 3 */
CREATE TABLE [dbo].[ParteMxMnCriterio](
	[ParteMxMnCriterioID] [int] IDENTITY(1,1) NOT NULL,
	[ParteMxMnID] [int] NOT NULL,
	[MxMnCriterioID] [int] NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_ParteMxMnCriterio] PRIMARY KEY CLUSTERED 
(
	[ParteMxMnCriterioID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ParteMxMnCriterio]  WITH CHECK ADD  CONSTRAINT [FK_ParteMxMnCriterio_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[ParteMxMnCriterio] CHECK CONSTRAINT [FK_ParteMxMnCriterio_Usuario]
GO
ALTER TABLE [dbo].[ParteMxMnCriterio] ADD  CONSTRAINT [DF_ParteMxMnCriterio_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO
ALTER TABLE [dbo].[ParteMxMnCriterio] ADD  CONSTRAINT [DF_ParteMxMnCriterio_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

/* 4 */

CREATE FUNCTION [dbo].[fnuPromedioMayorSeisMeses](
	@Numero1 AS DECIMAL(18,2)
	,@Numero2 AS DECIMAL(18,2)
	,@Numero3 AS DECIMAL(18,2)
	,@Numero4 AS DECIMAL(18,2)
	,@Numero5 AS DECIMAL(18,2)
	,@Numero6 AS DECIMAL(18,2)
	,@Numero7 AS DECIMAL(18,2)
	,@Numero8 AS DECIMAL(18,2)
	,@Numero9 AS DECIMAL(18,2)
	,@Numero10 AS DECIMAL(18,2)
	,@Numero11 AS DECIMAL(18,2)
	,@Numero12 AS DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN	
	DECLARE @Table TABLE(Valor DECIMAL(18,2))

	INSERT @Table 
	SELECT @Numero1
	INSERT @Table 
	SELECT @Numero2
	INSERT @Table 
	SELECT @Numero3
	INSERT @Table 
	SELECT @Numero4
	INSERT @Table 
	SELECT @Numero5
	INSERT @Table 
	SELECT @Numero6
	INSERT @Table 
	SELECT @Numero7
	INSERT @Table 
	SELECT @Numero8
	INSERT @Table 
	SELECT @Numero9
	INSERT @Table 
	SELECT @Numero10
	INSERT @Table 
	SELECT @Numero11
	INSERT @Table 
	SELECT @Numero12
	
	RETURN (
		SELECT AVG(Valor) FROM (SELECT TOP 6 Valor FROM @Table ORDER BY Valor DESC) Promedio
	)
END

GO


CREATE FUNCTION [dbo].[fnuPromedioMenorSeisMeses](
	@Numero1 AS DECIMAL(18,2)
	,@Numero2 AS DECIMAL(18,2)
	,@Numero3 AS DECIMAL(18,2)
	,@Numero4 AS DECIMAL(18,2)
	,@Numero5 AS DECIMAL(18,2)
	,@Numero6 AS DECIMAL(18,2)
	,@Numero7 AS DECIMAL(18,2)
	,@Numero8 AS DECIMAL(18,2)
	,@Numero9 AS DECIMAL(18,2)
	,@Numero10 AS DECIMAL(18,2)
	,@Numero11 AS DECIMAL(18,2)
	,@Numero12 AS DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN	
	DECLARE @Table TABLE(Valor DECIMAL(18,2))

	INSERT @Table 
	SELECT @Numero1
	INSERT @Table 
	SELECT @Numero2
	INSERT @Table 
	SELECT @Numero3
	INSERT @Table 
	SELECT @Numero4
	INSERT @Table 
	SELECT @Numero5
	INSERT @Table 
	SELECT @Numero6
	INSERT @Table 
	SELECT @Numero7
	INSERT @Table 
	SELECT @Numero8
	INSERT @Table 
	SELECT @Numero9
	INSERT @Table 
	SELECT @Numero10
	INSERT @Table 
	SELECT @Numero11
	INSERT @Table 
	SELECT @Numero12
	
	RETURN (
		SELECT AVG(Valor) FROM (SELECT TOP 6 Valor FROM @Table ORDER BY Valor ASC) Promedio
	)
END

GO

/* 5 */

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[MxMnView]'))
DROP VIEW [dbo].[MxMnView]
GO

CREATE VIEW [dbo].[MxMnView]
AS
SELECT 
	Row_Number() OVER (ORDER BY (SELECT 1)) AS MxMnID
	,MxMn.* 
	,dbo.fnuPromedioMayorSeisMeses(MesAnterior1,MesAnterior2,MesAnterior3,MesAnterior4,MesAnterior5,MesAnterior6,MesAnterior7,MesAnterior8,MesAnterior9,MesAnterior10,MesAnterior11,MesAnterior12) AS PromedioMayor
	,dbo.fnuPromedioMenorSeisMeses(MesAnterior1,MesAnterior2,MesAnterior3,MesAnterior4,MesAnterior5,MesAnterior6,MesAnterior7,MesAnterior8,MesAnterior9,MesAnterior10,MesAnterior11,MesAnterior12) AS PromedioMenor	
	,ISNULL(ParteMxMn.EsFijo, 0) AS Fijo
	,'' AS [MIN]
	,'' AS [MAX]
	,CAST (CASE WHEN ISNULL(ParteMxMn.EsFijo, 0) = 1 THEN ParteMxMn.EsFijo ELSE ISNULL(ParteMxMn.EsFijo, 0) END AS INT) AS Criterio
	,CASE WHEN ISNULL(ParteMxMn.EsFijo, 0) = 1 THEN ParteMxMn.AjusteMx ELSE '' END AS AjusteMx
	,CASE WHEN ISNULL(ParteMxMn.EsFijo, 0) = 1 THEN ParteMxMn.AjusteMn ELSE '' END AS AjusteMn	
	,CASE WHEN ISNULL(ParteMxMn.EsFijo, 0) = 1 THEN 
	STUFF((SELECT ',' + CAST(m.MxMnCriterioID AS VARCHAR)
	FROM ParteMxMnCriterio m	
	WHERE m.ParteMxMnID = ParteMxMn.ParteMxMnID
	FOR XML PATH('')
	), 1, 1, '') ELSE '' END AS CriteriosGenerales	
FROM
	(
	SELECT 
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,ParteExistencia.Existencia
		,ParteExistencia.UnidadEmpaque
		,Parte.TiempoReposicion
		,SUM(CASE WHEN VentaDetalle.FechaRegistro BETWEEN DATEADD(YEAR,-1,GETDATE()) AND GETDATE() THEN VentaDetalle.Cantidad ELSE 0.00 END) / DATEDIFF(DAY,DATEADD(year,-1,GETDATE()), GETDATE()) AS Promedio		
		,MAX(VentaDetalle.Cantidad) AS Mx
		,MIN(VentaDetalle.Cantidad) AS Mn
		,SUM(VentaDetalle.Cantidad) AS Cantidad
		,Venta.SucursalID
		,Parte.LineaID
		,Linea.NombreLinea
		,Parte.MarcaParteID
		,MarcaParte.NombreMarcaParte
		,Sistema.SistemaID
		,Sistema.NombreSistema
		,Parte.SubsistemaID
		,Subsistema.NombreSubsistema
		,Proveedor.ProveedorID
		,Proveedor.NombreProveedor		
		,ISNULL(Parte.Es9500, 0) AS Es9500	
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -1, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior1'	
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -2, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -2, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior2'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -3, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -3, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior3'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -4, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -4, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior4'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -5, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -5, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior5'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -6, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -6, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior6'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -7, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -7, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior7'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -8, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -8, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior8'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -9, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -9, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior9'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -10, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -10, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior10'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -11, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -11, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior11'
		,SUM(CASE WHEN MONTH(Venta.Fecha) = MONTH(DATEADD(MONTH, -12, GETDATE())) AND YEAR(Venta.Fecha) = YEAR(DATEADD(MONTH, -12, GETDATE())) THEN VentaDetalle.Cantidad ELSE 0.00 END) AS 'MesAnterior12'	
	FROM
		Venta
		INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
		INNER JOIN Parte ON Parte.ParteID = VentaDetalle.ParteID
		INNER JOIN Linea ON Linea.LineaID = Parte.LineaID	
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
		INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
		INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
		INNER JOIN ParteExistencia ON ParteExistencia.ParteID = VentaDetalle.ParteID 
				AND ParteExistencia.SucursalID = Venta.SucursalID
	WHERE
		Venta.Fecha BETWEEN DATEADD(YEAR,-1,GETDATE()) AND GETDATE()
		AND Venta.Estatus = 1
	GROUP BY
		Venta.SucursalID	
		,Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,ParteExistencia.Existencia
		,ParteExistencia.UnidadEmpaque
		,Parte.LineaID
		,Linea.NombreLinea
		,Parte.MarcaParteID
		,MarcaParte.NombreMarcaParte
		,Sistema.SistemaID
		,Sistema.NombreSistema
		,Parte.SubsistemaID
		,Subsistema.NombreSubsistema
		,Proveedor.ProveedorID
		,Proveedor.NombreProveedor
		,Parte.TiempoReposicion
		,Parte.Es9500
	) AS MxMn
	LEFT JOIN ParteMxMn ON ParteMxMn.ParteID = MxMn.ParteID AND ParteMxMn.SucursalID = MxMn.SucursalID
GO

/* 6 */
CREATE VIEW MxMnCriterioView
AS
SELECT 
	MxMnCriterio.MxMnCriterioID
	,MxMnCriterio.NombreCriterio
	,MxMnCriterio.Expresion
FROM 
	MxMnCriterio
WHERE
	MxMnCriterio.Estatus = 1

