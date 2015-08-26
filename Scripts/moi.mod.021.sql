/* Script con modificaciones a la base de datos de Theos. Archivo 021
 * Creado: 2015/06/17
 * Subido: 2015/05/19
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE NominaImpuesto (
	NominaImpuestoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ContaCuentaDeMayorID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaDeMayor(ContaCuentaDeMayorID)
	, Fecha DATETIME NOT NULL
	, Periodo INT NOT NULL
	, BancoCuentaID INT NOT NULL FOREIGN KEY REFERENCES BancoCuentaMovimiento(BancoCuentaMovimientoID)
	, TipoFormaPagoID INT NOT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
	, FolioDePago NVARCHAR(32) NOT NULL
	
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
)

CREATE TABLE NominaImpuestoUsuario (
	NominaImpuestoUsuarioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, NominaImpuestoID INT NOT NULL FOREIGN KEY REFERENCES NominaImpuesto(NominaImpuestoID)
	, IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, Retenido DECIMAL(12, 2) NULL
	, Total DECIMAL(12, 2) NOT NULL
)

INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES
	('Pago 2%', 1)
	, ('Pago Imss', 1)
	, ('Pago Infonavit', 1)

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

