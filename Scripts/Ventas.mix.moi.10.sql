/* Script con modificaciones para el módulo de ventas. Archivo 10
 * Creado: 2013/12/05
 * Subido: 2013/12/08
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE TABLE VentaPagoDetalleResguardo (
		VentaPagoDetalleResguardoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaPagoDetalleID INT NOT NULL FOREIGN KEY REFERENCES VentaPagoDetalle(VentaPagoDetalleID)
		, Resguardado BIT NOT NULL
	)

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion, SucursalID) VALUES
		('Ventas.Folio', '1', '1', 'Siguiente folio a utilizar al registrar una venta no facturada.', 2)
		, ('Ventas.Folio', '1', '1', 'Siguiente folio a utilizar al registrar una venta no facturada.', 3)

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

ALTER TABLE CajaEfectivoPorDia ADD SucursalID INT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
GO
UPDATE CajaEfectivoPorDia SET SucursalID = 1
ALTER TABLE CajaEfectivoPorDia ALTER COLUMN SucursalID INT NOT NULL

ALTER TABLE VentaCambio ADD SucursalID INT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
GO
UPDATE VentaCambio SET SucursalID = 1
ALTER TABLE VentaCambio ALTER COLUMN SucursalID INT NOT NULL

GO

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

ALTER VIEW VentasView AS
	SELECT
		v.VentaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, v.Folio
		, v.Fecha
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.SucursalID
		, v.VentaEstatusID
		, ve.Descripcion AS Estatus
		, ISNULL(vd.Subtotal, 0) AS Subtotal
		, ISNULL(vd.Iva, 0) AS Iva
		, ISNULL(vd.Total, 0) AS Total
		, ISNULL(vpt.Importe, 0) AS Pagado
		, v.ACredito
		, v.RealizoUsuarioID AS VendedorID
		, u.NombrePersona AS Vendedor
		, u.NombreUsuario AS VendedorUsuario
		, v.ComisionistaClienteID AS ComisionistaID
	FROM
		Venta v
		-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN VentaEstatus ve ON ve.VentaEstatusID = v.VentaEstatusID AND ve.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
		LEFT JOIN (
			SELECT
				VentaID
				, SUM(PrecioUnitario * Cantidad) AS Subtotal
				, SUM(Iva * Cantidad) AS Iva
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDetalle
			WHERE Estatus = 1
			GROUP BY VentaID
		) vd ON vd.VentaID = v.VentaID
		LEFT JOIN (
			SELECT
				vp.VentaID
				, SUM(vpd.Importe) AS Importe
			FROM
				VentaPago vp
				LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE vp.Estatus = 1
			GROUP BY vp.VentaID
		) vpt ON vpt.VentaID = v.VentaID
		-- LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		-- LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
	WHERE v.Estatus = 1
GO

ALTER VIEW VentasPagosView AS
	SELECT
		vp.VentaPagoID
		, vp.VentaID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, v.Folio
		, vp.Fecha
		, ISNULL((SELECT SUM(Importe) FROM VentaPagoDetalle WHERE VentaPagoID = vp.VentaPagoID AND Estatus = 1), 0) AS Importe
		, vp.SucursalID
		, c.ClienteID
		, v.VentaEstatusID
		, ISNULL(v.ACredito, 0) AS ACredito
		-- , v.
		, c.Nombre AS Cliente
		, u.NombrePersona AS Vendedor
		, u.NombreUsuario AS VendedorUsuario
	FROM
		VentaPago vp
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
	WHERE vp.Estatus = 1
GO

ALTER VIEW VentasDevolucionesView AS
	SELECT
		vd.VentaDevolucionID
		, ISNULL(CONVERT(BIT, CASE WHEN vfdv.FolioFiscal IS NULL THEN 0 ELSE 1 END), 0) AS Facturada
		, vd.VentaID
		, v.Folio
		, v.Fecha AS FechaDeVenta
		, vd.Fecha
		, vd.MotivoID
		, vdm.Descripcion AS Motivo
		, vd.Observacion
		, vd.SucursalID
		, vd.RealizoUsuarioID
		, u.NombrePersona AS Realizo
		, vd.EsCancelacion
		, vd.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		-- , CASE WHEN vd.EsCancelacion = 1 THEN 'CANC-' ELSE '    -DEV' END AS Tipo
		-- , CASE WHEN vd.TipoFormaPagoID = 2 THEN 'EF-' ELSE '  -NC' END AS Salida
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'PENDIENTE') AS AutorizoUsuario
		, vdd.Subtotal
		, vdd.Iva
		, vdd.Total
	FROM
		VentaDevolucion vd
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDevolucionMotivo vdm ON vdm.VentaDevolucionMotivoID = vd.MotivoID AND vdm.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vd.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'VentaDevolucion' AND a.TablaRegistroID = vd.VentaDevolucionID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
		LEFT JOIN VentasFacturasDetalleView vfdv ON vfdv.VentaID = v.VentaID AND vfdv.EstatusGenericoID != 4
		LEFT JOIN (
			SELECT
				VentaDevolucionID
				, SUM(PrecioUnitario * Cantidad) AS Subtotal
				, SUM(Iva * Cantidad) AS Iva
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Total
			FROM VentaDevolucionDetalle
			WHERE Estatus = 1
			GROUP BY VentaDevolucionID
		) vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID
	WHERE vd.Estatus = 1
GO

ALTER VIEW CajaIngresosView AS
	SELECT
		ci.CajaIngresoID
		, ci.CajaTipoIngresoID
		, cti.NombreTipoIngreso AS Tipo
		, ci.Concepto
		, ci.Importe
		, ci.Fecha
		, ci.SucursalID
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'Pendiente') AS AutorizoUsuario
	FROM
		CajaIngreso ci
		LEFT JOIN CajaTipoIngreso cti ON cti.CajaTipoIngresoID = ci.CajaTipoIngresoID AND cti.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'CajaIngreso' AND a.TablaRegistroID = ci.CajaIngresoID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
	WHERE ci.Estatus = 1
GO

ALTER VIEW CajaEgresosView AS
	SELECT
		ce.CajaEgresoID
		, ce.CajaTipoEgresoID
		, cte.NombreTipoEgreso AS Tipo
		, ce.Concepto
		, ce.Importe
		, ce.Fecha
		, ce.SucursalID
		, ISNULL(a.Autorizado, 0) AS Autorizado
		, ISNULL(ua.NombreUsuario, 'Pendiente') AS AutorizoUsuario
	FROM
		CajaEgreso ce
		LEFT JOIN CajaTipoEgreso cte ON cte.CajaTipoEgresoID = ce.CajaTipoEgresoID AND cte.Estatus = 1
		LEFT JOIN Autorizacion a ON a.Tabla = 'CajaEgreso' AND a.TablaRegistroID = ce.CajaEgresoID AND a.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
	WHERE ce.Estatus = 1
GO

ALTER VIEW VentasPagosDetalleView AS
	SELECT
		vpd.VentaPagoDetalleID
		, vpv.VentaID
		, ISNULL(vpv.Facturada, 0) AS Facturada
		, vpv.Folio AS VentaFolio
		, vpv.VentaPagoID
		, vpv.Fecha
		, vpv.Importe AS ImporteTotal
		, vpv.VentaEstatusID
		, vpv.ClienteID
		, vpv.Cliente
		, ISNULL(vpv.ACredito, 0) AS ACredito
		, vpv.SucursalID
		, vpd.TipoFormaPagoID AS FormaDePagoID
		, tfp.NombreTipoFormaPago AS FormaDePago
		, vpd.Importe
		, vpd.BancoID
		, b.NombreBanco AS Banco
		, vpd.Folio
		, vpd.Cuenta
		, vpd.NotaDeCreditoID
		, vpv.Vendedor
		, ISNULL(vpdr.Resguardado, 0) AS Resguardado
	FROM
		VentaPagoDetalle vpd
		LEFT JOIN VentasPagosView vpv ON vpv.VentaPagoID = vpd.VentaPagoID
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
		LEFT JOIN VentaPagoDetalleResguardo vpdr ON vpdr.VentaPagoDetalleID = vpd.VentaPagoDetalleID
		/*
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vpd.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Banco b ON b.BancoID = vpd.BancoID AND b.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		*/
	WHERE vpd.Estatus = 1
GO

ALTER VIEW VentasCambiosView AS
	SELECT
		vc.VentaCambioID
		, vc.VentaPagoID
		, v.VentaID
		, v.Folio AS VentaFolio
		, vc.Fecha
		, vc.SucursalID
		, c.Nombre AS Cliente
		, vc.FormasDePagoAntes
		, vc.FormasDePagoDespues
		, vc.RealizoIDAntes
		, ua.NombreUsuario AS VendedorAntes
		, vc.RealizoIDDespues
		, ud.NombreUsuario AS VendedorDespues
		, vc.ComisionistaIDAntes
		, cca.Alias AS ComisionistaAntes
		, vc.ComisionistaIDDespues
		, ccd.Alias AS ComisionistaDespues
	FROM
		VentaCambio vc
		LEFT JOIN VentaPago vp ON vp.VentaPagoID = vc.VentaPagoID AND vp.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario ua ON ua.UsuarioID = vc.RealizoIDAntes AND ua.Estatus = 1
		LEFT JOIN Usuario ud ON ud.UsuarioID = vc.RealizoIDDespues AND ud.Estatus = 1
		LEFT JOIN Cliente cca ON cca.ClienteID = vc.ComisionistaIDAntes AND cca.Estatus = 1
		LEFT JOIN Cliente ccd ON ccd.ClienteID = vc.ComisionistaIDDespues AND ccd.Estatus = 1
	WHERE vc.Estatus = 1
GO

ALTER VIEW VentasDevolucionesDetalleView AS
	SELECT
		vdd.VentaDevolucionDetalleID
		, vdd.VentaDevolucionID
		, vdd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vdd.Cantidad
		, vdd.PrecioUnitario
		, vdd.Iva
		, m.NombreMedida
	FROM
		VentaDevolucionDetalle vdd
		LEFT JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN Medida m ON m.MedidaID = p.MedidaID AND m.Estatus = 1
	WHERE vdd.Estatus = 1
GO

ALTER VIEW VentasACreditoView AS
	SELECT
		vv.VentaID
		, vv.ClienteID
		, vv.Folio
		, vv.Fecha
		, vv.VentaEstatusID
		, (vv.Fecha + c.DiasDeCredito) AS Vencimiento
		, vv.Total
		, vv.Pagado
		, ISNULL(vv.Total - vv.Pagado, 0) AS Restante
	FROM
		VentasView vv
		LEFT JOIN Cliente c ON c.ClienteID = vv.ClienteID AND c.Estatus = 1
	WHERE vv.ACredito = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

