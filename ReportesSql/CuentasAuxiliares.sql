/* *****************************************************************************
** Script de características de Cuentas Auxiliares
** Creado: 09/09/2014 Moisés
***************************************************************************** */

SELECT
	cc.Cuenta
	, cs.Subcuenta
	, ccm.CuentaDeMayor
	, cca.CuentaAuxiliar
	, cca.Devengable
	, SUM(CASE WHEN ccad.SucursalID = 1 THEN Porcentaje ELSE 0.00 END) AS PorMatriz
	, SUM(CASE WHEN ccad.SucursalID = 2 THEN Porcentaje ELSE 0.00 END) AS PorSuc2
	, SUM(CASE WHEN ccad.SucursalID = 3 THEN Porcentaje ELSE 0.00 END) AS PorSuc3
	, cca.Detallable
	, cca.VisibleEnCaja
	, cca.CalculoSemanal
	, cca.DevengarAut
	, cca.PeriodicidadMes
	, cca.FinSemanalizar
	, cca.AfectaMetas
	, cca.SumaGastosFijos
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
	LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
	LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	LEFT JOIN ContaCuentaAuxiliarDevengadoAutomatico ccad ON ccad.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
GROUP BY
	cc.Cuenta
	, cs.Subcuenta
	, ccm.CuentaDeMayor
	, cca.CuentaAuxiliar
	, cca.Devengable
	, cca.Detallable
	, cca.VisibleEnCaja
	, cca.CalculoSemanal
	, cca.DevengarAut
	, cca.PeriodicidadMes
	, cca.FinSemanalizar
	, cca.AfectaMetas
	, cca.SumaGastosFijos
ORDER BY
	cc.Cuenta
	, cs.Subcuenta
	, ccm.CuentaDeMayor
	, cca.CuentaAuxiliar