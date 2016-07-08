/* Validacion de Devoluciones */
CREATE VIEW MovimientosInventarioContadorDevolucionesView
AS
SELECT 
	MovimientoInventario.MovimientoInventarioID	
	,MovimientoInventario.FolioFactura		
	,MovimientoInventarioDetalle.ParteID
	,SUM(MovimientoInventarioDetalle.Cantidad) AS Cantidad
FROM 
	MovimientoInventario 
	INNER JOIN MovimientoInventarioDetalle ON MovimientoInventarioDetalle.MovimientoInventarioID = MovimientoInventario.MovimientoInventarioID
WHERE 
	MovimientoInventario.TipoOperacionID = 4
	AND MovimientoInventario.Estatus = 1
GROUP BY
	MovimientoInventario.MovimientoInventarioID	
	,MovimientoInventario.FolioFactura		
	,MovimientoInventarioDetalle.ParteID
GO

/* Registro de log de movimientos de inventario */
CREATE TABLE [dbo].[MovimientoInventarioHistorial](
	[MovimientoInventarioHistorialID] [int] IDENTITY(1,1) NOT NULL,
	[MovmientoInventarioID] [int] NOT NULL,
	[ParteID] [int] NULL,
	[ExistenciaInicial] [decimal](18, 2) NOT NULL,
	[ExistenciaFinal] [decimal](18, 2) NOT NULL,
	[SucursalID] [int] NULL,
	[UsuarioID] [int] NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[FechaModificacion] [datetime] NULL,
	[Estatus] [bit] NOT NULL,
	[Actualizar] [bit] NOT NULL,
 CONSTRAINT [PK_MovimientoInventarioHistorial] PRIMARY KEY CLUSTERED 
(
	[MovimientoInventarioHistorialID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[MovimientoInventarioHistorial] ADD  CONSTRAINT [DF_MovimientoInventarioHistorial_Estatus]  DEFAULT ((1)) FOR [Estatus]
GO

ALTER TABLE [dbo].[MovimientoInventarioHistorial] ADD  CONSTRAINT [DF_MovimientoInventarioHistorial_Actualizar]  DEFAULT ((1)) FOR [Actualizar]
GO