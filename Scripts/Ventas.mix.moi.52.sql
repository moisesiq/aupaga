/* Script con modificaciones para el módulo de ventas. Archivo 52
 * Creado: 2014/08/15
 * Subido: 2014/08/18
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE ContaCuentaAuxiliar DROP COLUMN DivisorDia
	ALTER TABLE ContaCuentaAuxiliar ADD
		PeriodicidadMes INT NULL
		, FinSemanalizar DATE NULL

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

GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE pauContaCuentasPorSemana (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON
	
	/* EXEC pauContaCuentasPorSemana '2014-01-01', '2014-12-31'
	DECLARE @Desde DATE = '2014-01-01'
	DECLARE @Hasta DATE = '2014-12-31'
	*/

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	-- Inicio del Procedimiento
	SELECT
		ced.ContaEgresoDevengadoID
		, ced.ContaEgresoID
		, ced.SucursalID
		, s.NombreSucursal AS Sucursal
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, cca.PeriodicidadMes
		, cca.FinSemanalizar
		-- , ced.Fecha
		-- , DATEPART(WEEK, ced.Fecha) AS Semana
		-- , CONVERT(DATE, CASE WHEN DATEPART(DW, ced.Fecha) = 7 THEN ced.Fecha
		-- 	ELSE DATEADD(DAY, (DATEPART(DW, ced.Fecha) * -1), ced.Fecha) END) AS DiaIni
		, ced.Fecha
		, SUM(ced.Importe) AS ImporteDev
		-- , ce.Observaciones AS Egreso
		-- , cca.CalculoSemanal
		-- , cca.DiasMovimiento
		-- , (ISNULL(cca.DiasMovimiento, 7) / 7) AS Semanas
		-- , SUM(CASE WHEN cca.CalculoSemanal = 1 THEN ((ced.Importe / cca.DivisorDia) * 7) ELSE ced.Importe END) AS Importe
	FROM
		ContaEgresoDevengado ced
		LEFT JOIN ContaEgreso ce ON ce.ContaEgresoID = ced.ContaEgresoID AND ce.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID

		LEFT JOIN Sucursal s ON s.SucursalID = ced.SucursalID AND s.Estatus = 1
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
	WHERE
		(ced.Fecha >= @Desde AND ced.Fecha < @Hasta)
	GROUP BY
		ced.ContaEgresoDevengadoID
		, ced.ContaEgresoID
		, ced.SucursalID
		, s.NombreSucursal
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, cca.PeriodicidadMes
		, cca.FinSemanalizar
		, ced.Fecha
		-- , DATEPART(WEEK, ced.Fecha)
		-- , CONVERT(DATE, CASE WHEN DATEPART(DW, ced.Fecha) = 7 THEN ced.Fecha
		--	ELSE DATEADD(DAY, (DATEPART(DW, ced.Fecha) * -1), ced.Fecha) END)
		-- , cca.DiasMovimiento
	ORDER BY
		Sucursal
		, Cuenta
		, Subcuenta
		, CuentaDeMayor
		, CuentaAuxiliar
		, ced.Fecha DESC

END
GO