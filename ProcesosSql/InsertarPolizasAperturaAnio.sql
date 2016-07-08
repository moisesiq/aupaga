/* *****************************************************************************
** Proceso para insertar pólizas de apertura para todas las cuentas de Theos,
** con el saldo inicial obtenido del año pasado.
** Creado: 25/01/2016 Moisés
***************************************************************************** */

BEGIN TRAN

DECLARE @Anio INT = 2016
DECLARE @CadAnio NVARCHAR(4) = CONVERT(NVARCHAR(4), @Anio)
DECLARE @Origen NVARCHAR(32) = ('ASIENTO DE APERTURA ' + @CadAnio)
DECLARE @DiaPrimero DATETIME = CONVERT(DATETIME, (@CadAnio + '-01-01'))
DECLARE @DiaPrimeroAnt DATETIME = DATEADD(YEAR, -1, @DiaPrimero)

-- Se borran las pólizas de asiente de apertura que pudiera haber por corridas anteriores
DELETE FROM ContaPolizaDetalle WHERE ContaPolizaID IN (SELECT ContaPolizaID FROM ContaPoliza WHERE Origen = @Origen)
DELETE FROM ContaPoliza WHERE Origen = @Origen

-- Se insertan las pólizas, una por cada sucursal
INSERT INTO ContaPoliza (Fecha, ContaTipoPolizaID, Concepto, RealizoUsuarioID, SucursalID, Reportar, Origen)
	SELECT @DiaPrimero, 3, @Origen, 1, SucursalID, 0, @Origen
	FROM Sucursal
	WHERE Estatus = 1
-- Se insertan las cuentas
INSERT INTO ContaPolizaDetalle (ContaPolizaID, ContaCuentaAuxiliarID, Referencia, Cargo, Abono, SucursalID)
	SELECT
		cpr.ContaPolizaID
		, cpd.ContaCuentaAuxiliarID
		, SUBSTRING(CONVERT(NVARCHAR(10), GETDATE(), 120), 3, 8)
		, SUM(cpd.Cargo) AS Cargo
		, SUM(cpd.Abono) AS Abono
		, cpd.SucursalID
	FROM
		ContaPolizaDetalle cpd
		INNER JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
		LEFT JOIN ContaPoliza cpr ON cpr.Origen = @Origen AND cpr.SucursalID = cpd.SucursalID
	WHERE
		(cp.Fecha > @DiaPrimeroAnt AND cp.Fecha < @DiaPrimero)
	GROUP BY
		cpd.ContaCuentaAuxiliarID
		, cpd.SucursalID
		, cpr.ContaPolizaID
	HAVING
		SUM(cpd.Cargo) != SUM(cpd.Abono)

ROLLBACK TRAN