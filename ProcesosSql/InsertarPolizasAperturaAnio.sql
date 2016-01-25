/* *****************************************************************************
** Proceso para insertar p�lizas de apertura para todas las cuentas de Theos,
** con el saldo inicial obtenido del a�o pasado.
** Creado: 07/01/2016 Mois�s
***************************************************************************** */

BEGIN TRAN

DECLARE @CodigoPro NVARCHAR(4) = '-348'

-- Se insertan las p�lizas, una por cada sucursal
INSERT INTO ContaPoliza (Fecha, ContaTipoPolizaID, Concepto, RealizoUsuarioID, SucursalID, Reportar, Origen)
	SELECT '2016-01-01', 3, 'ASIENTO DE APERTURA 2016', 1, SucursalID, 0, @CodigoPro
	FROM Sucursal
	WHERE Estatus = 1
-- Se insertan las cuentas
INSERT INTO ContaPolizaDetalle (ContaPolizaID, ContaCuentaAuxiliarID, Referencia, Cargo, Abono, SucursalID)
	SELECT
		cp.ContaPolizaID
		, cpd.ContaCuentaAuxiliarID
		, SUBSTRING(CONVERT(NVARCHAR(10), GETDATE(), 120), 3, 8)
		, SUM(cpd.Cargo) AS Cargo
		, SUM(cpd.Abono) AS Abono
		, cpd.SucursalID
	FROM
		ContaPolizaDetalle cpd
		LEFT JOIN ContaPoliza cp ON cp.Origen = @CodigoPro AND cp.SucursalID = cpd.SucursalID
	GROUP BY
		cpd.ContaCuentaAuxiliarID
		, cpd.SucursalID
		, cp.ContaPolizaID
-- Se borra el origen en las p�lizas
UPDATE ContaPoliza SET Origen = NULL WHERE Origen = @CodigoPro

ROLLBACK TRAN