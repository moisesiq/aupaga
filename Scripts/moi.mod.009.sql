/* Script con modificaciones a la base de datos de Theos. Archivo 009
 * Creado: 2015/05/12
 * Subido: 2015/05/13
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- DROP TABLE CajaFacturaGlobal
CREATE TABLE CajaFacturaGlobal (
	CajaFacturaGlobalID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Dia DATE NOT NULL
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, Tickets DECIMAL(12, 2) NOT NULL
	, FacturadoDeDiasAnt DECIMAL(12, 2) NOT NULL
	, Negativos DECIMAL(12, 2) NOT NULL
	, Devoluciones DECIMAL(12, 2) NOT NULL
	, Cancelaciones DECIMAL(12, 2) NOT NULL
	, Restar DECIMAL(12, 2) NOT NULL
	, CostoMinimo DECIMAL(12, 2) NOT NULL
	, RestanteDiasAnt DECIMAL(12, 2) NOT NULL
	, Facturar DECIMAL(12, 2) NOT NULL
)

ALTER TABLE ContaCuentaDeMayor ADD RestaInversa BIT NULL
GO
UPDATE ContaCuentaDeMayor SET RestaInversa = 1 WHERE ContaCuentaDeMayorID IN 
	(SELECT ContaCuentaDeMayorID FROM ContaCuentasAuxiliaresView WHERE ContaCuentaID IN (2, 3, 4))

INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
	('Contabilidad.Reserva.Ver', 'No tienes permisos para ver la información de Reservas.')

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

CREATE VIEW CajaFacturasGlobalesView AS
	SELECT
		fg.CajaFacturaGlobalID
		, fg.Dia
		, fg.SucursalID
		, s.NombreSucursal AS Sucursal
		, fg.Tickets
		, fg.FacturadoDeDiasAnt
		, fg.Negativos
		, fg.Devoluciones
		, fg.Cancelaciones
		, fg.Restar
		, fg.CostoMinimo
		, fg.RestanteDiasAnt
		, fg.Facturar AS Real
		, (fg.Tickets - fg.FacturadoDeDiasAnt - fg.Negativos - fg.Devoluciones - fg.Cancelaciones) AS Oficial
	FROM
		CajaFacturaGlobal fg
		LEFT JOIN Sucursal s ON s.SucursalID = fg.SucursalID AND s.Estatus = 1
GO

ALTER VIEW [dbo].[CascosRegistrosView] AS
	SELECT
		cr.CascoRegistroID
		, cr.Fecha
		, cr.VentaID
		, v.Folio AS FolioDeVenta
		, v.ClienteID
		, v.SucursalID
		, v.VentaEstatusID
		, c.Nombre AS Cliente
		, cr.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, (vd.PrecioUnitario + vd.Iva) AS Precio
		, pc.NumeroParte AS NumeroDeParteDeCasco
		, pc.NombreParte AS DescripcionDeCasco
		, pr.NumeroParte AS NumeroDeParteRecibido
		, pr.NombreParte AS DescripcionRecibido
		, cr.CobroVentaID
		, vc.Folio AS FolioDeCobro
		, STUFF((
			SELECT
				(', ' + CONVERT(NVARCHAR, ci.OrigenID))
			FROM
				CascoRegistroImporte cri
				LEFT JOIN CascoImporte ci ON ci.CascoImporteID = cri.CascoImporteID
			WHERE cri.CascoRegistroID = cr.CascoRegistroID
			FOR XML PATH('')
			), 1, 2, '') AS CascoImporte
	FROM
		CascoRegistro cr
		LEFT JOIN Venta v ON v.VentaID = cr.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = cr.ParteID AND p.Estatus = 1
		LEFT JOIN Parte pc ON pc.ParteID = p.RequiereCascoDe AND pc.Estatus = 1
		LEFT JOIN Parte pr ON pr.ParteID = cr.RecibidoCascoID AND pr.Estatus = 1
		LEFT JOIN Venta vc ON vc.VentaID = cr.CobroVentaID AND vc.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.ParteID = cr.ParteID AND vd.Estatus = 1
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
				, cp.SucursalID
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