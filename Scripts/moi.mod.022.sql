/* Script con modificaciones a la base de datos de Theos. Archivo 022
 * Creado: 2015/06/25
 * Subido: 2015/05/25
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE ContaPolizaResguardoOcultar (
	ContaPolizaDetalleID INT NOT NULL PRIMARY KEY
		FOREIGN KEY REFERENCES ContaPolizaDetalle(ContaPolizaDetalleID)
	, CONSTRAINT Un_ContaPolizaDetalleID UNIQUE (ContaPolizaDetalleID)
)

INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
	('Contabilidad.Polizas.Borrar', 'No tienes permisos para borrar pólizas contables.')
	, ('Bancos.Movimientos.Borrar', 'No tienes permisos para borrar movimientos bancarios.')

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
		, CASE WHEN agr.MovimientoAgrupadorID IS NULL THEN bcm.Conciliado ELSE agr.Conciliado END AS Conciliado
		, bcm.FechaConciliado
		, bcm.ConciliadoUsuarioID
		, cu.NombreUsuario AS UsuarioConciliado
		, bcm.Observacion
		-- , ISNULL(bcm.EsDeVenta, 0) AS EsDeVenta
		, bcm.RelacionTabla
		, bcm.RelacionID
		, ISNULL(vpdr.Resguardado, 0) AS Resguardado
		, bcm.MovimientoAgrupadorID
		, CONVERT(BIT, CASE WHEN agr.MovimientoAgrupadorID IS NULL THEN 0 ELSE 1 END) AS EsAgrupador
		, bcm.FueManual
	FROM
		BancoCuentaMovimiento bcm
		LEFT JOIN Sucursal s ON s.SucursalID = bcm.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = bcm.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario cu ON cu.UsuarioID = bcm.ConciliadoUsuarioID AND cu.Estatus = 1
		LEFT JOIN VentaPagoDetalle vpd ON bcm.RelacionTabla = 'VentaPagoDetalle' AND vpd.VentaPagoDetalleID = bcm.RelacionID AND vpd.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
		-- Para saber si ya todos los movimientos de un agrupado ya fueron o no conciliados
		LEFT JOIN (
			SELECT
				MovimientoAgrupadorID
				, CONVERT(BIT, MIN(CASE WHEN Conciliado = 1 THEN 1 ELSE 0 END)) AS Conciliado
			FROM BancoCuentaMovimiento
			WHERE MovimientoAgrupadorID IS NOT NULL
			GROUP BY MovimientoAgrupadorID
		) agr ON agr.MovimientoAgrupadorID = bcm.BancoCuentaMovimientoID
GO

ALTER VIEW [dbo].[NominaUsuariosOficialView] AS
	SELECT
		nuo.NominaUsuarioOficialID
		, nuo.NominaID
		, n.Semana
		, nuo.IdUsuario AS UsuarioID
		, nuo.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, nuo.Importe
		, nuo.Suma
	FROM
		NominaUsuarioOficial nuo
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = nuo.ContaCuentaDeMayorID
		LEFT JOIN Nomina n ON n.NominaID = nuo.NominaID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

