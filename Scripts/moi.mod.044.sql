/* Script con modificaciones a la base de datos de Theos. Archivo 044
 * Creado: 2015/10/26
 * Subido: 2015/10/29
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE ContaPolizaAsigSucursal (
	ContaPolizaAsigSucursalID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Sucursal NVARCHAR(16) NOT NULL
)
INSERT INTO ContaPolizaAsigSucursal (Sucursal) VALUES
	('Local')
	, ('Matriz')
	, ('Donde se hizo')

ALTER TABLE ContaConfigAfectacionDetalle ADD ContaPolizaAsigSucursalID INT NULL
GO
UPDATE ContaConfigAfectacionDetalle SET ContaPolizaAsigSucursalID = 1
ALTER TABLE ContaConfigAfectacionDetalle ALTER COLUMN ContaPolizaAsigSucursalID INT NOT NULL

ALTER TABLE ContaPolizaDetalle ADD SucursalID INT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
GO
UPDATE ContaPolizaDetalle SET SucursalID = cp.SucursalID FROM
	ContaPolizaDetalle cpd
	INNER JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
ALTER TABLE ContaPolizaDetalle ALTER COLUMN SucursalID INT NOT NULL
-- ALTER TABLE ContaPoliza DROP COLUMN SucursalID

ALTER TABLE NotaDeCredito ADD SucursalID INT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[ContaPolizasDetalleAvanzadoView] AS
	SELECT
		cpd.ContaPolizaDetalleID
		, cpd.ContaPolizaID
		, cp.Fecha AS FechaPoliza
		, cp.Origen AS OrigenPoliza
		-- , cp.SucursalID AS SucursalID
		-- , s.NombreSucursal AS Sucursal
		, cp.Concepto AS ConceptoPoliza
		, cp.Error
		, cp.FueManual
		, cp.RelacionTabla
		, cp.RelacionID
		, cpd.SucursalID
		, s.NombreSucursal AS Sucursal
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
		-- LEFT JOIN Sucursal s ON s.SucursalID = cp.SucursalID AND s.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = cpd.SucursalID AND s.Estatus = 1
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
	-- DECLARE @CtaActivo INT = 1
	-- DECLARE @CtaPasivo INT = 2
	-- DECLARE @CtaCapitalContable INT = 3
	-- DECLARE @CtaResultadosAcreedoras INT = 4
	-- DECLARE @CtaResultadosDeudoras INT = 5

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
		, ISNULL(CONVERT(BIT, CASE WHEN pec.ContaCuentaAuxiliarID IS NULL THEN 0 ELSE 1 END), 0) AS Error
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
				-- , cp.SucursalID
				-- Ahora se obtiene la sucursal del detalle
				, cpd.SucursalID
				/* , CASE cs.ContaCuentaID
					WHEN @CtaActivo THEN (cpd.Cargo - cpd.Abono)
					WHEN @CtaResultadosDeudoras THEN (cpd.Cargo - cpd.Abono)
					WHEN @CtaPasivo THEN (cpd.Abono - cpd.Cargo)
					WHEN @CtaCapitalContable THEN (cpd.Abono - cpd.Cargo)
					WHEN @CtaResultadosAcreedoras THEN (cpd.Abono - cpd.Cargo)
					ELSE 0.0
				END	AS Importe
				*/
				, CASE WHEN ccm.RestaInversa = 1 THEN (cpd.Abono - cpd.Cargo) ELSE (cpd.Cargo - cpd.Abono) END	AS Importe
			FROM
				ContaPolizaDetalle cpd
				INNER JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
				INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cpd.ContaCuentaAuxiliarID
				INNER JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
				INNER JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
			WHERE (cp.Fecha >= @Desde AND cp.Fecha < @Hasta)
		) pc ON pc.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
		-- Se agrega union con las tablas de pólizas, para marcar las que tengan error
		LEFT JOIN (
			SELECT DISTINCT
				cpd.ContaCuentaAuxiliarID
			FROM
				ContaPoliza cp
				INNER JOIN ContaPolizaDetalle cpd ON cpd.ContaPolizaID = cp.ContaPolizaID
			WHERE cp.Error = 1
		) pec ON pec.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
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
		, pec.ContaCuentaAuxiliarID
	ORDER BY
		(cc.CuentaSat + cc.Cuenta)
		, (cs.CuentaSat + cs.Subcuenta)
		, (ccm.CuentaSat + ccm.CuentaDeMayor)
		, (cca.CuentaAuxiliar)

END
GO

ALTER PROCEDURE [dbo].[pauContaCuentasPolizasImportes] (
	@Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @Desde DATE = '2015-01-01'
	DECLARE @Hasta DATE = '2015-12-31'
	DECLARE @SucursalID INT = NULL
	*/

	-- Definición de variables tipo constante
	

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		cs.ContaCuentaID
		, cc.Cuenta
		, ccm.ContaSubcuentaID
		, cs.Subcuenta
		, cca.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, pc.SucursalID
		, SUM(CASE WHEN ccm.RestaInversa = 1 THEN (pc.Abono - pc.Cargo) ELSE (pc.Cargo - pc.Abono) END) AS Importe
	FROM
		ContaCuentaAuxiliar cca
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
		LEFT JOIN (
			SELECT
				-- cp.SucursalID
				-- Ahora se obtiene la sucursal del detalle
				cpd.SucursalID
				, cpd.ContaCuentaAuxiliarID
				, SUM(cpd.Cargo) AS Cargo
				, SUM(cpd.Abono) AS Abono
			FROM
				ContaPoliza cp
				LEFT JOIN ContaPolizaDetalle cpd ON cpd.ContaPolizaID = cp.ContaPolizaID
			WHERE
				(@SucursalID IS NULL OR cp.SucursalID = @SucursalID)
				AND (cp.Fecha >= @Desde AND cp.Fecha < @Hasta)
			GROUP BY
				-- cp.SucursalID
				cpd.SucursalID
				, cpd.ContaCuentaAuxiliarID
		) pc ON pc.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
		-- LEFT JOIN ContaPolizaDetalle cpd ON cpd.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
		-- LEFT JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
	GROUP BY
		cs.ContaCuentaID
		, cc.Cuenta
		, ccm.ContaSubcuentaID
		, cs.Subcuenta
		, cca.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, pc.SucursalID
	ORDER BY
		cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar

END
GO
