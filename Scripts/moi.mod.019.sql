/* Script con modificaciones a la base de datos de Theos. Archivo 019
 * Creado: 2015/06/10
 * Subido: 2015/05/10
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE BancoCuentaMovimiento ADD RelacionTabla NVARCHAR(64) NULL
GO
UPDATE BancoCuentaMovimiento SET RelacionTabla = 'VentaPagoDetalle' WHERE EsDeVenta = 1
ALTER TABLE BancoCuentaMovimiento DROP COLUMN EsDeVenta

ALTER TABLE ContaPoliza ADD RelacionTabla NVARCHAR(64) NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[BancosCuentasMovimientosView] AS
	SELECT
		bcm.BancoCuentaMovimientoID
		, bcm.BancoCuentaID
		, bcm.EsIngreso
		, bcm.Fecha
		, bcm.FechaAsignado
		, bcm.SucursalID
		, s.NombreSucursal AS Sucursal
		, bcm.Importe
		, bcm.Concepto
		, bcm.Referencia
		, bcm.TipoFormaPagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, bcm.DatosDePago
		, bcm.SaldoAcumulado
		, bcm.Conciliado
		, bcm.FechaConciliado
		, bcm.ConciliadoUsuarioID
		, cu.NombreUsuario AS UsuarioConciliado
		, bcm.Observacion
		-- , ISNULL(bcm.EsDeVenta, 0) AS EsDeVenta
		, bcm.RelacionTabla
		, bcm.RelacionID
		, ISNULL(vpdr.Resguardado, 0) AS Resguardado
		, bcm.MovimientoAgrupadorID
	FROM
		BancoCuentaMovimiento bcm
		LEFT JOIN Sucursal s ON s.SucursalID = bcm.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = bcm.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario cu ON cu.UsuarioID = bcm.ConciliadoUsuarioID AND cu.Estatus = 1
		LEFT JOIN VentaPagoDetalle vpd ON bcm.RelacionTabla = 'Venta' AND vpd.VentaPagoDetalleID = bcm.RelacionID AND vpd.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

