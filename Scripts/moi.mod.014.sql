/* Script con modificaciones a la base de datos de Theos. Archivo 014
 * Creado: 2015/05/22
 * Subido: 2015/05/27
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

INSERT INTO ContaConfigAfectacion (Operacion, ContaTipoPolizaID) VALUES
	('Intereses Bancarios', 1)
	, ('Pago a Proveedor Directo Cpcp', 1)
	, ('Gasto Contable Facturado Banco Cpcp', 1)

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.Nomina.Desglose', 'D', 'I', 'Salida donde debe mostrarse el desglose en billetes para la nómina de cada usuario (D - Diseño, P - Pantalla, I - Impresora).')

CREATE TABLE PedidoFiltro (
	PedidoFiltroID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Tabla NVARCHAR(64) NOT NULL
	, RelacionID INT NOT NULL
	, Seleccion BIT NULL
)

-- Pagos de proveedores

ALTER TABLE ProveedorPoliza ADD
	BancoCuentaID INT NULL FOREIGN KEY REFERENCES BancoCuenta(BancoCuentaID)
	, TipoFormaPagoID INT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
	, FolioDePago NVARCHAR(32) NULL
GO
UPDATE ProveedorPoliza SET
	BancoCuentaID = ppd.BancoCuentaID
	, TipoFormaPagoID = ppd.TipoFormaPagoID
	, FolioDePago = ppd.FolioDePago
FROM
	ProveedorPoliza pp
	LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.ProveedorPolizaID = pp.ProveedorPolizaID
ALTER TABLE ProveedorPoliza ALTER COLUMN BancoCuentaID INT NOT NULL
ALTER TABLE ProveedorPoliza ALTER COLUMN TipoFormaPagoID INT NOT NULL
ALTER TABLE ProveedorPoliza ALTER COLUMN FolioDePago NVARCHAR(32) NOT NULL
EXEC pauBorrarLlaveForanea 'ProveedorPolizaDetalle', '_Banco_'
EXEC pauBorrarLlaveForanea 'ProveedorPolizaDetalle', '_TipoF_'
ALTER TABLE ProveedorPolizaDetalle DROP COLUMN BancoCuentaID, TipoFormaPagoID, FolioDePago

ALTER TABLE ProveedorPolizaDetalle ADD Observacion NVARCHAR(128) NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO
select * from proveedorescomprasview
ALTER VIEW [dbo].[ProveedoresComprasView] AS
	SELECT
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS Fecha
		, mi.ImporteFactura
		, mi.ImporteTotal
		, ISNULL(SUM(ppd.Importe), 0.0) AS Abonado
		, (mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0)) AS Saldo
		, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END) AS Descuento
		, (
			(mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0))
			- SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END)
		) AS Final
		, u.NombreUsuario AS Usuario
		-- , ppd.ProveedorPolizaID
	FROM
		MovimientoInventario mi
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mi.MovimientoInventarioID AND ppd.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE
		mi.Estatus = 1
		AND mi.TipoOperacionID = 1
	GROUP BY
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
		, u.NombreUsuario
		, ppd.ProveedorPolizaID
GO

ALTER VIEW [dbo].[ProveedoresPolizasView] AS
	SELECT
		pp.ProveedorPolizaID
		, pp.ProveedorID
		, p.NombreProveedor AS Proveedor
		, pp.FechaPago
		, pp.ImportePago
		, pp.UsuarioID
		, u.NombreUsuario AS Usuario
		, pp.BancoCuentaID
		, bc.NombreDeCuenta
		, pp.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, pp.FolioDePago
	FROM
		ProveedorPoliza pp
		LEFT JOIN Proveedor p ON p.ProveedorID = pp.ProveedorID AND p.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = pp.UsuarioID AND u.Estatus = 1
		LEFT JOIN BancoCuenta bc ON bc.BancoCuentaID = pp.BancoCuentaID
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = pp.TipoFormaPagoID AND tfp.Estatus = 1
	WHERE pp.Estatus = 1
GO

ALTER VIEW [dbo].[ProveedoresPolizasDetalleAvanzadoView] AS
	SELECT
		ppd.ProveedorPolizaDetalleID
		, ppd.ProveedorPolizaID
		, ppd.OrigenID
		, ppo.Origen
		, pp.FechaPago AS Fecha
		, ppd.MovimientoInventarioID
		, mi.FolioFactura
		, ppd.Importe
		-- , ppd.TipoFormaPagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		-- , ppd.BancoCuentaID
		, pp.FolioDePago
		, ppd.NotaDeCreditoID
		, ppd.Observacion
		, ndc.Folio AS NotaDeCredito
		, ndc.OrigenID AS NotaDeCreditoOrigenID
		, ndco.Origen AS NotaDeCreditoOrigen
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN ProveedorPolizaDetalleOrigen ppo ON ppo.ProveedorPolizaDetalleOrigenID = ppd.OrigenID
		LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID AND pp.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = pp.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN ProveedorNotaDeCredito ndc ON ndc.ProveedorNotaDeCreditoID = ppd.NotaDeCreditoID
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
	WHERE ppd.Estatus = 1
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
