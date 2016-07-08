/* Script con modificaciones a la base de datos de Theos. Archivo 036
 * Creado: 2015/09/23
 * Pruebas: 2015/09/28
 * Prod: 
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE CorteCategoria (
	CorteCategoriaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Categoria NVARCHAR(64) NOT NULL
)
INSERT INTO CorteCategoria (Categoria) VALUES
	('Ventas')
	, ('Cancelaciones del día')
	, ('Cancelaciones de días anteriores')
	, ('Garantías del día')
	, ('Garantías de días anteriores')
	, ('Cobranza')
	, ('Vales creados')
	, ('Gastos')
	, ('Resguardos')
	, ('Refuerzos')

CREATE TABLE CorteDetalleHistorico (
	CorteDetalleHistoricoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Dia DATE NOT NULL
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, CorteCategoriaID INT NOT NULL FOREIGN KEY REFERENCES CorteCategoria(CorteCategoriaID)
	, RelacionTabla NVARCHAR(64) NULL
	, RelacionID INT NULL
	, Concepto NVARCHAR(32) NOT NULL
	, Importe DECIMAL(12, 2) NOT NULL
	, Efectivo DECIMAL(12, 2) NULL
	, Cheque DECIMAL(12, 2) NULL
	, Tarjeta DECIMAL(12, 2) NULL
	, Transferencia DECIMAL(12, 2) NULL
	, Vale DECIMAL(12, 2) NULL
)

ALTER TABLE ParteKardex ADD
	RelacionTabla NVARCHAR(64) NULL
	, RelacionID INT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

-- DROP VIEW VentasPagosFormasDePagoView
CREATE VIEW VentasPagosFormasDePagoView AS
	SELECT
		vp.VentaPagoID
		, vp.Fecha
		, vp.SucursalID
		, vp.VentaID
		, v.Folio AS FolioDeVenta
		, v.Fecha AS FechaDeVenta
		, v.VentaEstatusID
		, v.ACredito
		, v.Facturada
		, SUM(CASE WHEN vpd.TipoFormaPagoID = 2 THEN vpd.Importe ELSE 0.0 END) AS Efectivo
		, SUM(CASE WHEN vpd.TipoFormaPagoID = 1 THEN vpd.Importe ELSE 0.0 END) AS Cheque
		, SUM(CASE WHEN vpd.TipoFormaPagoID = 4 THEN vpd.Importe ELSE 0.0 END) AS Tarjeta
		, SUM(CASE WHEN vpd.TipoFormaPagoID = 3 THEN vpd.Importe ELSE 0.0 END) AS Transferencia
		, SUM(CASE WHEN vpd.TipoFormaPagoID = 6 THEN vpd.Importe ELSE 0.0 END) AS Vale
		, SUM(vpd.Importe) AS Importe
	FROM
		VentaPago vp
		LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
	WHERE vp.Estatus = 1
	GROUP BY
		vp.VentaPagoID
		, vp.Fecha
		, vp.SucursalID
		, vp.VentaID
		, v.Folio
		, v.Fecha
		, v.VentaEstatusID
		, v.ACredito
		, v.Facturada
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

-- DROP PROCEDURE [dbo].[pauContaCuentasPolizasImportes]
CREATE PROCEDURE [dbo].[pauContaCuentasPolizasImportes] (
	@Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* 
	DECLARE @Desde DATE = '2015-01-01'
	DECLARE @Hasta DATE = '2015-12-31'
	DECLARE @SucursalID INT = NULL
	EXEC pauContaCuentasPolizasImportes @Desde, @Hasta, @SucursalID
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
				cp.SucursalID
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
				cp.SucursalID
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