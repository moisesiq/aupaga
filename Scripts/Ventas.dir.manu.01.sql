-- 1. Agregar Columna de Principal as tabla ProveedorContacto

USE [ControlRefaccionaria]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[ProveedorContacto] ADD [Principal] [bit] NOT NULL DEFAULT ((0))
GO

SELECT ProveedorID as MyId, count(*) As Contiene, ROW_NUMBER() over (Order by ProveedorID) as Row 
INTO #Duplicates 
FROM dbo.ProveedorContacto
GROUP BY ProveedorID 

DECLARE @Registros INT = (select count(*) from #Duplicates)
DECLARE @Contador INT = 1
DECLARE @MyId INT
DECLARE @Registro INT

while(@Contador <= @Registros ) BEGIN
	select @MyId = 	MyId from #Duplicates where Row = @Contador
	select top 1 @Registro = ProveedorContactoID from ProveedorContacto where @MyId = ProveedorID order by ProveedorContactoID
	update ProveedorContacto SET Principal = '1' where @Registro = ProveedorContactoID
	SET @Contador = 1 + @Contador
END
GO

CREATE VIEW ProveedorContactoPrincipalView
AS
SELECT     p.ProveedorID, p.NombreProveedor, pc.ProveedorContactoID, p.TelUno AS TelProUno, p.TelDos AS TelProDos, p.TelTres AS TelProTres, pc.NombreContacto, pc.TelUno AS TelProCUno, 
                      pc.TelDos AS TelProCDos, pc.TelTres AS TelProCTres, pc.TelCuatro AS TelProCCuatro, p.PaginaWeb, p.Direccion, p.Ciudad, p.Estado, p.CP
FROM         dbo.Proveedor AS p LEFT OUTER JOIN
                      dbo.ProveedorContacto AS pc ON p.ProveedorID = pc.ProveedorID
WHERE     (pc.Principal = 1) OR
                      (pc.Principal IS NULL)
GO

CREATE VIEW ProveedorLineaView
AS
SELECT DISTINCT dbo.Linea.LineaID, dbo.Linea.NombreLinea, dbo.Parte.ProveedorID
FROM         dbo.Parte INNER JOIN
                      dbo.Linea ON dbo.Parte.LineaID = dbo.Linea.LineaID
GO


CREATE VIEW ProveedorEquivalContactoPpalView
AS
SELECT     pcpv.ProveedorID, pcpv.NombreProveedor, pcpv.ProveedorContactoID, pcpv.TelProUno, pcpv.TelProDos, pcpv.TelProTres, pcpv.NombreContacto, pcpv.TelProCUno, pcpv.TelProCDos, 
                      pcpv.TelProCTres, pcpv.TelProCCuatro, pcpv.PaginaWeb, pcpv.Direccion, pcpv.Ciudad, pcpv.Estado, pcpv.CP, dbo.PartesEquivalentesView.ParteID, 
                      dbo.PartesEquivalentesView.ParteIDEquivalente
FROM         dbo.Parte INNER JOIN
                      dbo.ProveedorContactoPrincipalView AS pcpv ON dbo.Parte.ProveedorID = pcpv.ProveedorID INNER JOIN
                      dbo.PartesEquivalentesView ON dbo.Parte.ParteID = dbo.PartesEquivalentesView.ParteID
GO

CREATE TABLE [dbo].[MarcaAttachFile](
	[MarcaAttachFileID] [int] IDENTITY(1,1) NOT NULL,
	[MarcaID] [int] NOT NULL,
	[NombreArchivo] [nvarchar](50) NULL,
	[Descripcion] [nvarchar](350) NOT NULL,
 CONSTRAINT [PK_MarcaAttachFile] PRIMARY KEY CLUSTERED 
(
	[MarcaAttachFileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LineaAttachFile](
	[LineaAttachFileID] [int] IDENTITY(1,1) NOT NULL,
	[LineaID] [int] NOT NULL,
	[NombreArchivo] [nvarchar](50) NULL,
	[Descripcion] [nvarchar](350) NOT NULL,
 CONSTRAINT [PK_LineaAttachFile] PRIMARY KEY CLUSTERED 
(
	[LineaAttachFileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




CREATE VIEW ProveedorMarcaParteView
AS
SELECT     TOP (100) PERCENT dbo.MarcaPartesView.MarcaParteID, dbo.MarcaPartesView.NombreMarcaParte, dbo.Parte.ProveedorID
FROM         dbo.Parte INNER JOIN
                      dbo.MarcaPartesView ON dbo.Parte.MarcaParteID = dbo.MarcaPartesView.MarcaParteID
ORDER BY dbo.MarcaPartesView.MarcaParteID
GO