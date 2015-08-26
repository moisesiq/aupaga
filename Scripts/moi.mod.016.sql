/* Script con modificaciones a la base de datos de Theos. Archivo 016
 * Creado: 2015/06/01
 * Subido: 2015/05/03
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE CajaEfectivoPorDia DROP COLUMN FacturaGlobalRestar, FacturaGlobalRestado

EXEC sp_rename 'CajaFacturaGlobal.RestanteDiasAnt', 'Restante', 'COLUMN'
EXEC sp_rename 'CajaFacturaGlobal.Facturar', 'Facturado', 'COLUMN'
ALTER TABLE CajaFacturaGlobal ADD SaldoRestante DECIMAL(12, 2) NULL
GO
UPDATE CajaFacturaGlobal SET SaldoRestante = 0
ALTER TABLE CajaFacturaGlobal ALTER COLUMN SaldoRestante DECIMAL(12, 2) NOT NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[ProveedoresComprasView] AS
	SELECT
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS Fecha
		, mi.ImporteFactura
		, mi.ImporteTotal
		, ISNULL(SUM(ppd.Importe), 0.0) AS Abonado
		, (mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0)) AS Saldo
		, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END) AS Descuento
		, (
			(mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0))
			- SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END)
		) AS Final
		, u.NombreUsuario AS Usuario
		-- , ppd.ProveedorPolizaID
	FROM
		MovimientoInventario mi
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mi.MovimientoInventarioID AND ppd.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE
		mi.Estatus = 1
		AND mi.TipoOperacionID = 1
	GROUP BY
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
		, u.NombreUsuario
		-- , ppd.ProveedorPolizaID
GO

ALTER VIEW [dbo].[CajaFacturasGlobalesView] AS
	SELECT
		fg.CajaFacturaGlobalID
		, fg.Dia
		, fg.SucursalID
		, s.NombreSucursal AS Sucursal
		, fg.Tickets
		, fg.FacturadoDeDiasAnt
		, fg.Negativos
		, fg.Devoluciones
		, fg.Cancelaciones
		, (fg.Tickets - fg.FacturadoDeDiasAnt - fg.Negativos - fg.Devoluciones - fg.Cancelaciones) AS Oficial
		, fg.Restar
		, (fg.Tickets - fg.FacturadoDeDiasAnt - fg.Negativos - fg.Devoluciones - fg.Cancelaciones - fg.Restar) AS Supuesto
		, fg.CostoMinimo
		, fg.Restante
		, fg.SaldoRestante
		, fg.Facturado
	FROM
		CajaFacturaGlobal fg
		LEFT JOIN Sucursal s ON s.SucursalID = fg.SucursalID AND s.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

