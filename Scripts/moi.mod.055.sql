/* Script con modificaciones a la base de datos de Theos. Archivo 055
 * Creado: 2015/12/11
 * Subido: 2015/12/11
 */

DECLARE @ScriptID INT = 55
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */



/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

DROP VIEW ProveedorReporteDevolucionDetalleView

GO

ALTER VIEW [dbo].[MovimientoInventarioView] AS
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
		-- ,MovimientoInventario.AplicaEnMovimientoInventarioID	
		-- ,MovimientoInventario.FechaAplicacion	
		,MovimientoInventario.Subtotal	
		,MovimientoInventario.IVA	
		,MovimientoInventario.ImporteTotal
		, MovimientoInventario.ImporteFactura
		,MovimientoInventario.FueLiquidado	
		-- ,MovimientoInventario.ConceptoMovimiento	
		, MovimientoInventario.TipoConceptoOperacionID
		, tco.NombreConceptoOperacion AS ConceptoOperacion
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
		LEFT JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
		INNER JOIN TipoOperacion ON TipoOperacion.TipoOperacionID = MovimientoInventario.TipoOperacionID
		LEFT JOIN TipoPago ON TipoPago.TipoPagoID = MovimientoInventario.TipoPagoID
		INNER JOIN Sucursal s1 ON s1.SucursalID = MovimientoInventario.SucursalOrigenID
		INNER JOIN Sucursal s2 ON s2.SucursalID = MovimientoInventario.SucursalDestinoID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
		LEFT JOIN TipoConceptoOperacion tco ON tco.TipoConceptoOperacionID = MovimientoInventario.TipoConceptoOperacionID
	WHERE
		MovimientoInventario.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO
