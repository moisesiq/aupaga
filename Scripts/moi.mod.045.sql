/* Script con modificaciones a la base de datos de Theos. Archivo 045
 * Creado: 2015/10/30
 * Subido: 2015/11/03
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE BancoCuentaMovimiento ALTER COLUMN Concepto NVARCHAR(256) NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

-- DROP VIEW ProveedoresPagosView
CREATE VIEW ProveedoresPagosView AS
	SELECT
		ppd.ProveedorPolizaID
		, ppd.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura AS Factura
		, mi.FechaRecepcion AS FechaFactura
		, mi.ImporteFactura
		, mi.ImporteTotal
		, (SELECT SUM(Importe) FROM ProveedorPolizaDetalle WHERE MovimientoInventarioID = ppd.MovimientoInventarioID)
			AS Abonadototal
		, ISNULL(SUM(ppd.Importe), 0.0) AS Abonado
		, SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END) AS Descuento
		/* , (
			(mi.ImporteFactura - ISNULL(SUM(ppd.Importe), 0.0))
			- SUM(CASE WHEN ppd.OrigenID IS NULL OR ppd.OrigenID = 1 THEN 0.0 ELSE ppd.Importe END)
		) AS Final
		*/
	FROM
		ProveedorPolizaDetalle ppd
		LEFT JOIN ProveedorPoliza pp ON pp.ProveedorPolizaID = ppd.ProveedorPolizaID AND pp.Estatus = 1
		LEFT JOIN MovimientoInventario mi ON mi.MovimientoInventarioID = ppd.MovimientoInventarioID AND mi.Estatus = 1
	WHERE ppd.Estatus = 1
	GROUP BY
		ppd.ProveedorPolizaID
		, ppd.MovimientoInventarioID
		, mi.ProveedorID
		, mi.FolioFactura
		, mi.FechaRecepcion
		, mi.ImporteFactura
		, mi.ImporteTotal
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
		, cs.ContaCuentaID
		, ccm.ContaSubcuentaID
		, cca.ContaCuentaDeMayorID
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
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

