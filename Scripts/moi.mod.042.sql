/* Script con modificaciones a la base de datos de Theos. Archivo 042
 * Creado: 2015/10/20
 * Subido: 2015/10/21
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */



/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

-- DROP VIEW ContaEgresosDevengadosView
CREATE VIEW ContaEgresosDevengadosView AS
	SELECT
		ced.ContaEgresoDevengadoID AS Id
		, ced.ContaEgresoID
		, ced.Fecha
		, ced.Importe
		, s.NombreSucursal AS Grupo
		, ced.RealizoUsuarioID
		, u.NombreUsuario AS Usuario
		, 0 AS EsEspecial
	FROM
		ContaEgresoDevengado ced
		LEFT JOIN Sucursal s ON s.SucursalID = ced.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ced.RealizoUsuarioID AND u.Estatus = 1
	UNION ALL
	SELECT
		cede.ContaEgresoDevengadoEspecialID AS Id
		, cede.ContaEgresoID
		, cede.Fecha
		, cede.Importe
		, d.Duenio AS Grupo
		, cede.RealizoUsuarioID
		, u.NombreUsuario AS Usuario
		, 1 AS EsEspecial
	FROM
		ContaEgresoDevengadoEspecial cede
		LEFT JOIN Duenio d ON d.DuenioID = cede.DuenioID
		LEFT JOIN Usuario u ON u.UsuarioID = cede.RealizoUsuarioID AND u.Estatus = 1
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
		, ede.Importe AS ImporteDevEsp
	FROM
		ContaEgreso ce
		LEFT JOIN Sucursal s ON s.SucursalID = ce.SucursalID AND s.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = ce.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ce.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS ImporteDev FROM ContaEgresoDevengado GROUP BY ContaEgresoID
		) cedc ON cedc.ContaEgresoID = ce.ContaEgresoID
		-- Se agrega para calcular el importe de devengado especial
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengadoEspecial GROUP BY ContaEgresoID
		) ede ON ede.ContaEgresoID = ce.ContaEgresoID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

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
	DECLARE @SubcuentaGastosFinancieros INT = 10
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
		, SUM(ISNULL(ede.Importe, 0.00)) AS ImporteDevEsp
	FROM
		ContaCuenta cc
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID
			AND cs.ContaSubcuentaID IN (@SubcuentaGastosGenerales, @SubcuentaGastosFinancieros)
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
		-- Se agrega el devengado especial
		LEFT JOIN (
			SELECT ContaEgresoID, SUM(Importe) AS Importe FROM ContaEgresoDevengadoEspecial
			GROUP BY ContaEgresoID
		) ede ON ede.ContaEgresoID = ce.ContaEgresoID
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
