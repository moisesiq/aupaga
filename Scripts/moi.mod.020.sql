/* Script con modificaciones a la base de datos de Theos. Archivo 020
 * Creado: 2015/06/10
 * Subido: 2015/05/15
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE BancoCuentaMovimiento ADD FueManual BIT NULL

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
		, bcm.FueManual
	FROM
		BancoCuentaMovimiento bcm
		LEFT JOIN Sucursal s ON s.SucursalID = bcm.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = bcm.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario cu ON cu.UsuarioID = bcm.ConciliadoUsuarioID AND cu.Estatus = 1
		LEFT JOIN VentaPagoDetalle vpd ON bcm.RelacionTabla = 'VentaPagoDetalle' AND vpd.VentaPagoDetalleID = bcm.RelacionID AND vpd.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
GO

ALTER VIEW [dbo].[Cotizaciones9500View] AS
	SELECT
		c9.Cotizacion9500ID
		, c9.EstatusGenericoID
		, c9.Fecha
		, c9.SucursalID
		, c9dc.Importe AS Total
		, c9.Anticipo
		, c9.ClienteID
		, c.Nombre AS Cliente
		, c.Celular
		, u.NombrePersona AS Vendedor
		, c9.AnticipoVentaID
		, v.Folio AS AnticipoVentaFolio
	FROM
		Cotizacion9500 c9
		LEFT JOIN (
			SELECT Cotizacion9500ID, SUM(PrecioAlCliente * Cantidad) AS Importe
			FROM Cotizacion9500Detalle
			GROUP BY Cotizacion9500ID
		) c9dc ON c9dc.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = c9.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = c9.AnticipoVentaID AND v.Estatus = 1
	WHERE c9.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

