/* Script con modificaciones a la base de datos de Theos. Archivo 008
 * Creado: 2015/05/11
 * Subido: 2015/05/12
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE BancoCuentaMovimiento ALTER COLUMN Referencia NVARCHAR(64) NULL
ALTER TABLE BancoCuentaMovimiento ADD
	MovimientoAgrupadorID INT NULL FOREIGN KEY REFERENCES BancoCuentaMovimiento(BancoCuentaMovimientoID)


/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
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
		, ISNULL(bcm.EsDeVenta, 0) AS EsDeVenta
		, ISNULL(vpdr.Resguardado, 0) AS Resguardado
		, bcm.RelacionID
		, bcm.MovimientoAgrupadorID
	FROM
		BancoCuentaMovimiento bcm
		LEFT JOIN Sucursal s ON s.SucursalID = bcm.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = bcm.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario cu ON cu.UsuarioID = bcm.ConciliadoUsuarioID AND cu.Estatus = 1
		LEFT JOIN VentaPagoDetalle vpd ON bcm.EsDeVenta = 1 AND vpd.VentaPagoDetalleID = bcm.RelacionID AND vpd.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

