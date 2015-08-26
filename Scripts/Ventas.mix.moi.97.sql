/* Script con modificaciones para el módulo de ventas. Archivo 97
 * Creado: 2015/03/30
 * Subido: 2015/03/31
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	

	COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH
GO

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */

ALTER TABLE ContaConfigAfectacionDetalle ADD EsCasoFijo BIT NULL
GO
UPDATE ContaConfigAfectacionDetalle SET EsCasoFijo = 0
ALTER TABLE ContaConfigAfectacionDetalle ALTER COLUMN EsCasoFijo BIT NOT NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE pauContaCuentasPolizas (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-04-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @SucMatrizID INT = 1
	DECLARE @Suc02ID INT = 2
	DECLARE @Suc03ID INT = 3
	DECLARE @CtaActivo INT = 1
	DECLARE @CtaPasivo INT = 2
	DECLARE @CtaCapitalContable INT = 3
	DECLARE @CtaResultadosAcreedoras INT = 4
	DECLARE @CtaResultadosDeudoras INT = 5

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, SUM(pc.Importe) AS Total
		, SUM(CASE WHEN pc.SucursalID = @SucMatrizID THEN pc.Importe ELSE 0.0 END) AS Matriz
		, SUM(CASE WHEN pc.SucursalID = @Suc02ID THEN pc.Importe ELSE 0.0 END) AS Suc02
		, SUM(CASE WHEN pc.SucursalID = @Suc03ID THEN pc.Importe ELSE 0.0 END) AS Suc03
	FROM
		ContaCuenta cc
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaSubcuentaID = cs.ContaSubcuentaID
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaDeMayorID = ccm.ContaCuentaDeMayorID
		LEFT JOIN (
			SELECT
				cpd.ContaCuentaAuxiliarID
				, cp.Fecha
				, cp.SucursalID
				, CASE cs.ContaCuentaID
					WHEN @CtaActivo THEN (cpd.Cargo - cpd.Abono)
					WHEN @CtaResultadosDeudoras THEN (cpd.Cargo - cpd.Abono)
					WHEN @CtaPasivo THEN (cpd.Abono - cpd.Cargo)
					WHEN @CtaCapitalContable THEN (cpd.Abono - cpd.Cargo)
					WHEN @CtaResultadosAcreedoras THEN (cpd.Abono - cpd.Cargo)
					ELSE 0.0
				END	AS Importe
			FROM
				ContaPolizaDetalle cpd
				INNER JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
				INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cpd.ContaCuentaAuxiliarID
				INNER JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
				INNER JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
			WHERE (cp.Fecha >= @Desde AND cp.Fecha < @Hasta)
		) pc ON pc.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
	GROUP BY
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, cc.CuentaSat
		, cs.CuentaSat
		, ccm.CuentaSat
		, cca.CuentaSat
	ORDER BY
		(cc.CuentaSat + cc.Cuenta)
		, (cs.CuentaSat + cs.Subcuenta)
		, (ccm.CuentaSat + ccm.CuentaDeMayor)
		, (cca.CuentaSat + cca.CuentaAuxiliar)

END
GO