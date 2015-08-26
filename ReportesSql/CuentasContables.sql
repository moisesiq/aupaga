/* *****************************************************************************
** Script que muestra todas las cuentas contables
** Creado: 19/02/2015 Moisés
***************************************************************************** */

SELECT
	'Cuenta' AS Tipo
	, Cuenta AS Cuenta
	, CuentaSat
	, NULL AS CuentaPadre
FROM ContaCuenta
UNION ALL
SELECT
	'Subcuenta' AS Tipo
	, cs.Subcuenta AS Cuenta
	, cs.CuentaSat
	, cc.Cuenta AS CuentaPadre
FROM
	ContaSubcuenta cs
	LEFT JOIN ContaCuenta cc on cc.ContaCuentaID = cs.ContaCuentaID
UNION ALL
SELECT
	'Cuenta de Mayor' AS Tipo
	, cm.CuentaDeMayor AS Cuenta
	, cm.CuentaSat
	, cs.Subcuenta AS CuentaPadre
FROM
	ContaCuentaDeMayor cm
	LEFT JOIN ContaSubcuenta cs on cm.ContaSubcuentaID = cs.ContaSubcuentaID
UNION ALL
SELECT
	'Cuenta Auxiliar' AS Tipo
	, ca.CuentaAuxiliar AS Cuenta
	, ca.CuentaSat
	, cm.CuentaDeMayor AS CuentaPadre
FROM
	ContaCuentaAuxiliar ca
	LEFT JOIN ContaCuentaDeMayor cm on cm.ContaCuentaDeMayorID = ca.ContaCuentaDeMayorID