/* Script con modificaciones para el módulo de ventas. Archivo 45
 * Creado: 2014/06/03
 * Subido: 2014/05/04
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



/* *****************************************************************************
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

DROP VIEW ContaCuentasSubcuentasView
GO

ALTER VIEW ContaCuentasAuxiliaresView AS
	SELECT
		ca.ContaCuentaAuxiliarID
		, ca.CuentaAuxiliar
		, ca.VisibleEnCaja
		, cm.ContaCuentaDeMayorID
		, cm.CuentaDeMayor
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, cc.ContaCuentaID
		, cc.Cuenta
	FROM
		ContaCuentaAuxiliar ca
		LEFT JOIN ContaCuentaDeMayor cm ON cm.ContaCuentaDeMayorID = ca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = cm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE [dbo].[pauContaCuentasMovimientosTotales] (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauContaCuentasMovimientosTotales 1, '2014-04-01', '2014-04-30'
	DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-04-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @CdmRepartoDeUtilidades INT = 18

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Gastos fijos - SucursalGastoFijo
	SELECT
		gf.SucursalGastoFijoID AS TablaRegistroID
		, CONVERT(BIT, 1) AS GastoFijo
		, gf.SucursalID
		, gf.Importe
		, cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, NULL AS Egreso
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
	FROM
		SucursalGastoFijo gf
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = gf.ContaCuentaAuxiliarID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE gf.SucursalID = @SucursalID

	UNION

	-- Gastos variables - ContaEgreso
	SELECT
		ce.ContaEgresoID AS TablaRegistroID
		, CONVERT(BIT, 0) AS GastoFijo
		, cedc.SucursalID
		, cedc.ImporteDev AS Importe
		, cca.ContaCuentaAuxiliarID
		, ccm.ContaCuentaDeMayorID
		, cs.ContaSubcuentaID
		, cc.ContaCuentaID
		, ce.Observaciones AS Egreso
		, cca.CuentaAuxiliar
		, ccm.CuentaDeMayor
		, cs.Subcuenta
		, cc.Cuenta
	FROM
		ContaEgreso ce
		INNER JOIN (
			SELECT
				ContaEgresoID
				, SucursalID
				, SUM(Importe) AS ImporteDev
			FROM ContaEgresoDevengado
			WHERE (Fecha >= @Desde AND Fecha < @Hasta)
			GROUP BY ContaEgresoID, SucursalID
		) cedc ON cedc.ContaEgresoID = ce.ContaEgresoID
		INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		ce.Estatus = 1
		AND cedc.SucursalID = @SucursalID
		-- Se quitan los egresos que ya se tienen como gastos fijos
		AND ce.ContaCuentaAuxiliarID NOT IN (
			SELECT ContaCuentaAuxiliarID
			FROM SucursalGastoFijo
			WHERE SucursalID = @SucursalID
		)
		-- Se quitan los egresos de la Cuenta de Mayor "Reparto de Utilidades"
		AND cca.ContaCuentaDeMayorID != @CdmRepartoDeUtilidades
	
	-- Se asigna el orden
	ORDER BY
		cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, Egreso

END
GO