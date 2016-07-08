/* Script con modificaciones a la base de datos de Theos. Archivo 069
 * Creado: 2016/06/20
 * Subido: 2016/06/27
 */

DECLARE @ScriptID INT = 69
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE ProveedorPolizaDetalle ADD
	FechaPago DATETIME NULL
	, TipoFormaPagoID INT NULL FOREIGN KEY REFERENCES TipoFormaPago(TipoFormaPagoID)
	, BancoCuentaID INT NULL FOREIGN KEY REFERENCES BancoCuenta(BancoCuentaID)
	, Pagado BIT NULL
GO
UPDATE ProveedorPolizaDetalle SET
	FechaPago = pp.FechaPago
	, Pagado = 1
FROM
	ProveedorPolizaDetalle ppd
	LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID
ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN FechaPago DATETIME NOT NULL
ALTER TABLE ProveedorPolizaDetalle ALTER COLUMN Pagado BIT NOT NULL

EXEC pauBorrarLlaveForanea 'ProveedorPoliza', 'FK__Proveedor__Banco'
EXEC pauBorrarLlaveForanea 'ProveedorPoliza', 'FK__Proveedor__TipoF'
ALTER TABLE ProveedorPoliza DROP COLUMN BancoCuentaID, TipoFormaPagoID, FolioDePago

EXEC pauBorrarLlaveForanea 'NominaImpuesto', 'FK__NominaImp__Banco'

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[ProveedoresPolizasView] AS
	SELECT
		pp.ProveedorPolizaID
		, pp.ProveedorID
		, p.NombreProveedor AS Proveedor
		, p.Beneficiario
		, pp.FechaPago
		-- , pp.ImportePago
		, pp.UsuarioID
		, u.NombreUsuario AS Usuario
		, ppd.BancoCuentaID
		, bc.NombreDeCuenta
		, ppd.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, ppd.Folio
	FROM
		ProveedorPoliza pp
		LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.ProveedorPolizaID = pp.ProveedorPolizaID AND ppd.OrigenID = 1 AND ppd.Estatus = 1
		LEFT JOIN Proveedor p ON p.ProveedorID = pp.ProveedorID AND p.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = pp.UsuarioID AND u.Estatus = 1
		LEFT JOIN BancoCuenta bc ON bc.BancoCuentaID = ppd.BancoCuentaID
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = ppd.TipoFormaPagoID AND tfp.Estatus = 1
	WHERE pp.Estatus = 1

GO

ALTER VIEW [dbo].[ProveedoresComprasView] AS
	WITH _Compras AS (
		SELECT
			mi.MovimientoInventarioID
			, mi.ProveedorID
			, mi.FolioFactura AS Factura
			, mi.FechaRecepcion AS Fecha
			, mi.ImporteFactura
			, mi.ImporteTotal
			, ppc.Abonado
			, (mi.ImporteFactura - ISNULL(ppc.Abonado, 0.0)) AS Saldo
			, ppc.Descuento
			, (mi.ImporteFactura - ISNULL(ppc.Abonado, 0.0) - ISNULL(ppc.Descuento, 0.0)) AS Final
			, (ppc.Abonado + ppc.AbonadoPendiente) AS AbonadoMasPendiente
			, (mi.ImporteFactura - ISNULL(ppc.Abonado, 0.0) - ISNULL(ppc.AbonadoPendiente, 0.0)) AS SaldoMasPendiente
			, (ppc.Descuento + ppc.DescuentoPendiente) AS DescuentoMasPendiente
			, (mi.ImporteFactura - ISNULL(ppc.Abonado, 0.0) - ISNULL(ppc.Descuento, 0.0) - ISNULL(ppc.AbonadoPendiente, 0.0)
				- ISNULL(ppc.DescuentoPendiente, 0.0)) AS FinalMasPendiente
			/*
			, ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0) AS Abonado
			, (mi.ImporteFactura - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0)) AS Saldo
			, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE (ppd.Subtotal + ppd.Iva) END) AS Descuento
			, (
				(mi.ImporteFactura - ISNULL(SUM(ppd.Subtotal + ppd.Iva), 0.0))
				- SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE (ppd.Subtotal + ppd.Iva) END)
			) AS Final
			*/
			, u.NombreUsuario AS Usuario
			-- , ppd.ProveedorPolizaID
			, mi.EsAgrupador
			, mi.MovimientoAgrupadorID
			, p.NombreProveedor AS Proveedor
			, p.DiasPlazo
		FROM
			MovimientoInventario mi
			LEFT JOIN (
				SELECT
					ppd.MovimientoInventarioID
					, SUM(CASE WHEN ppd.Pagado = 1 THEN isnull(ppd.Subtotal + ppd.Iva,0) ELSE 0.0 END) AS Abonado
					, SUM(CASE WHEN ppd.Pagado = 1 AND ppd.OrigenID > 1 THEN (ppd.Subtotal + ppd.Iva) ELSE 0.0 END) AS Descuento
					, SUM(CASE WHEN ppd.Pagado = 0 THEN (ppd.Subtotal + ppd.Iva) ELSE 0.0 END) AS AbonadoPendiente
					, SUM(CASE WHEN ppd.Pagado = 0 AND ppd.OrigenID > 1 THEN (ppd.Subtotal + ppd.Iva) ELSE 0.0 END)
						AS DescuentoPendiente
				FROM ProveedorPolizaDetalle ppd
				WHERE ppd.Estatus = 1
				GROUP BY ppd.MovimientoInventarioID
			) ppc ON ppc.MovimientoInventarioID = mi.MovimientoInventarioID
			-- LEFT JOIN ProveedorPolizaDetalle ppd ON ppd.MovimientoInventarioID = mi.MovimientoInventarioID AND ppd.Estatus = 1
			LEFT JOIN Proveedor p ON p.ProveedorID = mi.ProveedorID AND p.Estatus = 1
			LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
		WHERE
			mi.Estatus = 1
			AND mi.TipoOperacionID = 1
			AND mi.EsAgrupador = 0
		GROUP BY
			mi.MovimientoInventarioID
			, mi.ProveedorID
			, mi.FolioFactura
			, mi.FechaRecepcion
			, mi.ImporteFactura
			, mi.ImporteTotal
			, u.NombreUsuario
			-- , ppd.ProveedorPolizaID
			, mi.EsAgrupador
			, mi.MovimientoAgrupadorID
			, p.NombreProveedor
			, p.DiasPlazo
			, ppc.Abonado
			, ppc.Descuento
			, ppc.AbonadoPendiente
			, ppc.DescuentoPendiente
	)
	SELECT * FROM _Compras
	UNION
	SELECT
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS Fecha
		, mi.ImporteFactura
		, mi.ImporteTotal
		, ISNULL(SUM(c.Abonado), 0.0) AS Abonado
		, ISNULL(SUM(c.Saldo), 0.0) AS Saldo
		, ISNULL(SUM(c.Descuento), 0.0) AS Descuento
		, ISNULL(SUM(c.Final), 0.0) AS Final
		, ISNULL(SUM(c.AbonadoMasPendiente), 0.0) AS AbonadoMasPendiente
		, ISNULL(SUM(c.SaldoMasPendiente), 0.0) AS SaldoMasPendiente
		, ISNULL(SUM(c.DescuentoMasPendiente), 0.0) AS DescuentoMasPendiente
		, ISNULL(SUM(c.FinalMasPendiente), 0.0) AS FinalMasPendiente
		, u.NombreUsuario AS Usuario
		-- , ppd.ProveedorPolizaID
		, mi.EsAgrupador
		, mi.MovimientoAgrupadorID
		, MAX(c.Proveedor) AS Proveedor
		, MAX(c.DiasPlazo) AS DiasDePlazo
	FROM
		MovimientoInventario mi
		LEFT JOIN _Compras c ON c.MovimientoAgrupadorID = mi.MovimientoInventarioID
		LEFT JOIN Usuario u ON u.UsuarioID = mi.UsuarioID AND u.Estatus = 1
	WHERE
		mi.Estatus = 1
		AND mi.EsAgrupador = 1
	GROUP BY
		mi.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
		, u.NombreUsuario
		, mi.EsAgrupador
		, mi.MovimientoAgrupadorID
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
		, ppd.Subtotal
		, ppd.Iva
		, (ppd.Subtotal + ppd.Iva) AS Importe
		-- , ppd.TipoFormaPagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		-- , ppd.BancoCuentaID
		, ppd.NotaDeCreditoID
		, ppd.CajaEgresoID
		, ppd.Observacion
		, ppd.Folio
		, ppd.Pagado
		, ndc.Folio AS NotaDeCredito
		, ndc.OrigenID AS NotaDeCreditoOrigenID
		, ndco.Origen AS NotaDeCreditoOrigen
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN ProveedorPolizaDetalleOrigen ppo ON ppo.ProveedorPolizaDetalleOrigenID = ppd.OrigenID
		LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID AND pp.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID AND mi.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = ppd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN ProveedorNotaDeCredito ndc ON ndc.ProveedorNotaDeCreditoID = ppd.NotaDeCreditoID
		LEFT JOIN ProveedorNotaDeCreditoOrigen ndco ON ndco.ProveedorNotaDeCreditoOrigenID = ndc.OrigenID
	WHERE ppd.Estatus = 1

GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

