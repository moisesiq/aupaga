/* Script con modificaciones a la base de datos de Theos. Archivo 013
 * Creado: 2015/05/21
 * Subido: 2015/05/22
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[ProveedoresComprasView] AS
	SELECT
		ppd.ProveedorPolizaID
		, ppd.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS Fecha
		, mi.ImporteFactura
		, mi.ImporteTotal
		, ISNULL(SUM(ppd.Importe), 0.0) AS Pagado
		, (mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0)) AS Saldo
		, u.NombreUsuario AS Usuario
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID
			AND mi.TipoOperacionID = 1 AND mi.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE
		ppd.Estatus = 1
	GROUP BY
		ppd.ProveedorPolizaID
		, ppd.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
		, u.NombreUsuario
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

