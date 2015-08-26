/* Script con modificaciones a la base de datos de Theos. Archivo 002
 * Creado: 2015/04/16
 * Subido: 2015/04/17
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE ContaPoliza ADD Origen NVARCHAR(64) NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[ContaPolizasDetalleAvanzadoView] AS
	SELECT
		cpd.ContaPolizaDetalleID
		, cpd.ContaPolizaID
		, cp.Fecha AS FechaPoliza
		, cp.Origen AS OrigenPoliza
		, cp.SucursalID AS SucursalID
		, s.NombreSucursal AS Sucursal
		, cp.Concepto AS ConceptoPoliza
		, cpd.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, cca.CuentaContpaq
		, cca.CuentaSat
		, cpd.Cargo
		, cpd.Abono
		, cpd.Referencia
	FROM
		ContaPolizaDetalle cpd
		LEFT JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
		LEFT JOIN Sucursal s ON s.SucursalID = cp.SucursalID AND s.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cpd.ContaCuentaAuxiliarID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauContaCuentasPolizas] (
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
		, (cca.CuentaAuxiliar)

END
GO

ALTER PROCEDURE [dbo].[pauContaCuentasTotales] (
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
	DECLARE @SubcuentaGastosGenerales INT = 11

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
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID AND cs.ContaSubcuentaID = @SubcuentaGastosGenerales
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
	WHERE cs.ContaSubcuentaID IS NOT NULL
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
		, (cca.CuentaAuxiliar)

END
GO