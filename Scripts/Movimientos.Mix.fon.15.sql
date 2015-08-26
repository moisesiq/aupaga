/* */
ALTER TABLE dbo.Parte ADD
	CriterioABC nvarchar(5) NULL
GO

/* */
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
FROM
	Parte
	INNER JOIN ParteExistencia ON ParteExistencia.ParteID = Parte.ParteID
	INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
	INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
WHERE
	Parte.Estatus = 1
	AND ParteExistencia.Maximo > 0
	AND ParteExistencia.Existencia <= ParteExistencia.Minimo
GROUP BY
	Parte.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,ParteExistencia.UnidadEmpaque
	,Parte.ProveedorID
	,Proveedor.NombreProveedor
	,PartePrecio.Costo
	,Parte.CriterioABC
GO

/* */
CREATE TABLE [dbo].[CriterioABC](
	[CriterioAbcID] [int] IDENTITY(1,1) NOT NULL,
	[Clasificacion] [nvarchar](50) NOT NULL,
	[RangoInicial] [decimal](18, 2) NOT NULL,
	[RangoFinal] [decimal](18, 2) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_CriterioABC] PRIMARY KEY CLUSTERED 
(
	[CriterioAbcID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[CriterioABC] ON
INSERT [dbo].[CriterioABC] ([CriterioAbcID], [Clasificacion], [RangoInicial], [RangoFinal], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (1, N'A', CAST(20.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), 1, CAST(0xD1370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[CriterioABC] ([CriterioAbcID], [Clasificacion], [RangoInicial], [RangoFinal], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (2, N'B', CAST(12.00 AS Decimal(18, 2)), CAST(19.00 AS Decimal(18, 2)), 1, CAST(0xD1370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[CriterioABC] ([CriterioAbcID], [Clasificacion], [RangoInicial], [RangoFinal], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (3, N'C', CAST(8.00 AS Decimal(18, 2)), CAST(11.00 AS Decimal(18, 2)), 1, CAST(0xD1370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[CriterioABC] ([CriterioAbcID], [Clasificacion], [RangoInicial], [RangoFinal], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (4, N'D', CAST(3.00 AS Decimal(18, 2)), CAST(7.00 AS Decimal(18, 2)), 1, CAST(0xD1370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[CriterioABC] ([CriterioAbcID], [Clasificacion], [RangoInicial], [RangoFinal], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (5, N'E', CAST(2.00 AS Decimal(18, 2)), CAST(2.00 AS Decimal(18, 2)), 1, CAST(0xD1370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[CriterioABC] ([CriterioAbcID], [Clasificacion], [RangoInicial], [RangoFinal], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (6, N'N', CAST(1.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), 1, CAST(0xD1370B00 AS Date), NULL, 1, 1)
INSERT [dbo].[CriterioABC] ([CriterioAbcID], [Clasificacion], [RangoInicial], [RangoFinal], [UsuarioID], [FechaRegistro], [FechaModificacion], [Estatus], [Actualizar]) VALUES (7, N'Z', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 1, CAST(0xD1370B00 AS Date), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[CriterioABC] OFF

ALTER TABLE [dbo].[CriterioABC] ADD  CONSTRAINT [DF_CriterioABC_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[CriterioABC] ADD  CONSTRAINT [DF_CriterioABC_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[CriterioABC]  WITH CHECK ADD  CONSTRAINT [FK_CriterioABC_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[CriterioABC] CHECK CONSTRAINT [FK_CriterioABC_Usuario]
GO

/* */
CREATE VIEW [dbo].[PartesClasificacionAbcView]
AS
SELECT TOP 1000000000000
	Parte.ParteID
	,Parte.NumeroParte
	,Parte.NombreParte
	,ISNULL(Ventas.Cantidad, 0) AS Cantidad
	,Parte.CriterioABC AS VigenteAbc
	,c.Clasificacion AS NuevaAbc
FROM
	Parte
	LEFT JOIN (
		SELECT 	
			VentaDetalle.ParteID
			,SUM(VentaDetalle.Cantidad) AS Cantidad		
		FROM
			Venta
			INNER JOIN VentaDetalle ON VentaDetalle.VentaID = Venta.VentaID
		WHERE
			Venta.Fecha BETWEEN DATEADD(YEAR,-1,GETDATE()) AND GETDATE()
			AND Venta.Estatus = 1
			AND Venta.VentaEstatusID IN (1, 4)
		GROUP BY
			VentaDetalle.ParteID) AS Ventas ON Ventas.ParteID = Parte.ParteID	
	LEFT JOIN CriterioABC c ON ISNULL(Ventas.Cantidad, 0) BETWEEN c.RangoInicial AND c.RangoFinal
WHERE
	Parte.Estatus = 1
ORDER BY
	Parte.ParteID
GO

/* */
CREATE PROCEDURE pauParteActualizaCriterioAbc
AS
BEGIN
    SET NOCOUNT ON
	UPDATE Parte SET Parte.CriterioABC = c.NuevaAbc
	FROM 
		PartesClasificacionAbcView c
	WHERE 
		Parte.ParteID = c.ParteID
END
GO

/* */

CREATE TABLE [dbo].[SucursalCriterioABC](
	[SucursalCriterioAbcID] [int] IDENTITY(1,1) NOT NULL,
	[SucursalID] [int] NULL,
	[CriterioAbcID] [int] NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [date] NOT NULL,
	[FechaModificacion] [date] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_SucursalCriterioABC] PRIMARY KEY CLUSTERED 
(
	[SucursalCriterioAbcID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SucursalCriterioABC] ADD  CONSTRAINT [DF_SucursalCriterioABC_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[SucursalCriterioABC] ADD  CONSTRAINT [DF_SucursalCriterioABC_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO

ALTER TABLE [dbo].[SucursalCriterioABC]  WITH CHECK ADD  CONSTRAINT [FK_SucursalCriterioABC_CriterioABC] FOREIGN KEY([CriterioAbcID])
REFERENCES [dbo].[CriterioABC] ([CriterioAbcID])
GO
ALTER TABLE [dbo].[SucursalCriterioABC] CHECK CONSTRAINT [FK_SucursalCriterioABC_CriterioABC]
GO

ALTER TABLE [dbo].[SucursalCriterioABC]  WITH CHECK ADD  CONSTRAINT [FK_SucursalCriterioABC_Sucursal] FOREIGN KEY([SucursalID])
REFERENCES [dbo].[Sucursal] ([SucursalID])
GO
ALTER TABLE [dbo].[SucursalCriterioABC] CHECK CONSTRAINT [FK_SucursalCriterioABC_Sucursal]
GO

ALTER TABLE [dbo].[SucursalCriterioABC]  WITH CHECK ADD  CONSTRAINT [FK_SucursalCriterioABC_Usuario] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[SucursalCriterioABC] CHECK CONSTRAINT [FK_SucursalCriterioABC_Usuario]
GO


/* */
CREATE VIEW SucursalesCriterioAbcView
AS
SELECT
	Sucursal.SucursalID
	,Sucursal.NombreSucursal
	,STUFF((SELECT ', ' + CAST(SucursalCriterioAbc.CriterioAbcID AS VARCHAR)
		FROM SucursalCriterioAbc
		WHERE SucursalCriterioAbc.SucursalID = Sucursal.SucursalID
		FOR XML PATH('')
		), 1, 1, '') AS Ids
	,STUFF((SELECT ', ' + CriterioABC.Clasificacion
		FROM SucursalCriterioAbc		
		INNER JOIN CriterioABC ON CriterioABC.CriterioAbcID = SucursalCriterioAbc.CriterioAbcID
		WHERE SucursalCriterioAbc.SucursalID = Sucursal.SucursalID
		FOR XML PATH('')
		), 1, 1, '') AS Nivel
FROM
	Sucursal
WHERE
	Sucursal.Estatus = 1
GO