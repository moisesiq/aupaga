/* Script con modificaciones para el módulo de ventas. Archivo 56
 * Creado: 2014/09/03
 * Subido: 2014/09/05
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Ventas.BarraDeMetas.SegundosActualizacion', '300', '300', 'Período de tiempo, en segundos, en el que se actualiza la Barra de Metas, en Ventas.')

	ALTER TABLE MetaSucursal ADD DiasPorSemana INT NOT NULL CONSTRAINT Df_DiasPorSemana DEFAULT 6
	ALTER TABLE MetaSucursal DROP CONSTRAINT Df_DiasPorSemana

	DECLARE @Constraint NVARCHAR(256) = (SELECT name FROM sys.default_constraints
		WHERE parent_object_id = object_id('ContaEgreso') AND name LIKE 'DF__ContaEgre__Estat__%')
	DECLARE @Com NVARCHAR(512) = 'ALTER TABLE ContaEgreso DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	DROP INDEX ContaEgreso.Ix_ContaEgreso_Estatus
	ALTER TABLE ContaEgreso DROP COLUMN Estatus

	SET @Constraint = (SELECT name FROM sys.default_constraints
		WHERE parent_object_id = object_id('ContaEgresoDetalle') AND name LIKE 'DF__ContaEgre__Estat__%')
	SET @Com = 'ALTER TABLE ContaEgresoDetalle DROP CONSTRAINT ' + @Constraint
	EXEC (@Com)
	DROP INDEX ContaEgresoDetalle.Ix_ContaEgresoDetalle_Estatus
	ALTER TABLE ContaEgresoDetalle DROP COLUMN Estatus
	
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

UPDATE MetaSucursal SET DiasPorSemana = 7 WHERE SucursalID = 1

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

ALTER VIEW [dbo].[ContaEgresosView] AS
	SELECT
		ce.ContaEgresoID
		, ce.ContaCuentaAuxiliarID
		, ce.Fecha
		, ce.FolioDePago
		, ce.Importe
		, s.NombreSucursal AS Sucursal
		, tfp.NombreTipoFormaPago AS FormaDePago
		, u.NombreUsuario AS Usuario
		, ce.Observaciones
		, cedc.ImporteDev
	FROM
		ContaEgreso ce
		LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = ce.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ce.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS ImporteDev FROM ContaEgresoDevengado GROUP BY ContaEgresoID
		) cedc ON cedc.ContaEgresoID = ce.ContaEgresoID
GO

ALTER VIEW ContaEgresosDetalleView AS
	SELECT
		ced.ContaEgresoDetalleID
		, ced.ContaEgresoID
		, ced.ContaConsumibleID
		, cc.Consumible
		, ced.Cantidad
		, ced.Importe
		-- , SUM(cedd.Cantidad) OVER (PARTITION BY cedd.ContaEgresoDetalleID) AS CantidadDev
		, SUM(cedd.Cantidad) AS CantidadDev
	FROM
		ContaEgresoDetalle ced
		LEFT JOIN ContaConsumible cc ON cc.ContaConsumibleID = ced.ContaConsumibleID
		LEFT JOIN ContaEgresoDetalleDevengado cedd ON cedd.ContaEgresoDetalleID = ced.ContaEgresoDetalleID
	GROUP BY
		ced.ContaEgresoDetalleID
		, ced.ContaEgresoID
		, ced.ContaConsumibleID
		, cc.Consumible
		, ced.Cantidad
		, ced.Importe
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE pauContaCuentasTotales (
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
		, SUM(CASE WHEN ce.EsFiscal = 1 THEN ce.Importe ELSE 0.00 END) AS Fiscal
		, SUM(ce.Importe) AS Total
		, SUM(CASE WHEN ce.SucursalID = @SucMatrizID THEN (ce.Importe 
			- ISNULL(ced2.Importe, 0.00) - ISNULL(ced3.Importe, 0.00)) ELSE ced1.Importe END) AS Matriz
		, SUM(CASE WHEN ce.SucursalID = @Suc02ID THEN (ce.Importe
			- ISNULL(ced1.Importe, 0.00) - ISNULL(ced3.Importe, 0.00)) ELSE ced2.Importe END) AS Suc02
		, SUM(CASE WHEN ce.SucursalID = @Suc03ID THEN (ce.Importe 
			- ISNULL(ced1.Importe, 0.00) - ISNULL(ced2.Importe, 0.00)) ELSE ced3.Importe END) AS Suc03
		, SUM(ISNULL(ced1.Importe, 0.00) + ISNULL(ced2.Importe, 0.00) + ISNULL(ced3.Importe, 0.00)) AS ImporteDev
	FROM
		ContaCuenta cc
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaSubcuentaID = cs.ContaSubcuentaID
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaDeMayorID = ccm.ContaCuentaDeMayorID
		LEFT JOIN ContaEgreso ce ON ce.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
			AND (ce.Fecha >= @Desde AND ce.Fecha < @Hasta)
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @SucMatrizID GROUP BY ContaEgresoID, SucursalID
		) ced1 ON ced1.ContaEgresoID = ce.ContaEgresoID
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @Suc02ID GROUP BY ContaEgresoID, SucursalID
		) ced2 ON ced2.ContaEgresoID = ce.ContaEgresoID
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengado
			WHERE SucursalID = @Suc03ID GROUP BY ContaEgresoID, SucursalID
		) ced3 ON ced3.ContaEgresoID = ce.ContaEgresoID
	GROUP BY
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
	ORDER BY
		cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar

END
GO

ALTER PROCEDURE pauContaCuentasMovimientosTotales (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauContaCuentasMovimientosTotales 1, '2014-08-23', '2014-08-28'
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
		, cca.AfectaMetas
		, cca.SumaGastosFijos
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
		, cca.AfectaMetas
		, cca.SumaGastosFijos
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
		cedc.SucursalID = @SucursalID
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
		LEFT JOIN ContaEgreso ce ON ce.ContaEgresoID = ced.ContaEgresoID
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
