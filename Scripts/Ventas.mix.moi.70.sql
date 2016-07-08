/* Script con modificaciones para el módulo de ventas. Archivo 70
 * Creado: 2014/11/28
 * Subido: 2014/12/02
 */

----------------------------------------------------------------------------------- Código de A
GO
----------------------------------------------------------------------------------- Código de A


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY



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



/* *****************************************************************************
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

CREATE VIEW VentasDetalleAvanzadoView AS
	SELECT
		vd.VentaDetalleID
		, vd.VentaID
		, v.VentaEstatusID
		, v.Fecha AS VentaFecha
		, vd.ParteID
		, p.NumeroParte
		, p.NombreParte
		, vd.Costo
		, vd.CostoConDescuento
		, vd.Cantidad
		, vd.PrecioUnitario
		, vd.Iva
	FROM
		VentaDetalle vd
		LEFT JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
	WHERE vd.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

ALTER PROCEDURE [dbo].[pauCajaDetalleCorte] (
	@OpcionID INT
	, @SucursalID INT
	, @Dia DATE
	, @DevDiasAnt BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @OpcionID INT = 1
	DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = '20140121'
	DECLARE @DevDiasAnt BIT = 0
	*/

	/* Posible fallo:
		- Al tener una venta pagada y realizar una devolución parcial de productos
		- Al tener una venta a crédito que ya tiene pagos pero no se ha completado, y luego se cancela
	*/

	-- Definición de variables tipo constante
	DECLARE @OpVentas INT = 1
	DECLARE @OpDevoluciones INT = 2
	DECLARE @OpGarantias INT = 3
	DECLARE @Op9500 INT = 4
	DECLARE @OpCobranza INT = 5
	DECLARE @OpEgresos INT = 6
	DECLARE @OpIngresos INT = 7
	DECLARE @OpCambios INT = 8
	-- Formas de pago
	DECLARE @FpEfectivoID INT = 2
	DECLARE @FpChequeID INT = 1
	DECLARE @FpTarjetaID INT = 4
	DECLARE @FpTransferenciaID INT = 3
	DECLARE @FpNotaDeCreditoID INT = 6
	DECLARE @FpNoIdentificadoID INT = 5
	-- Cadenas de tablas para almacenar en CajaVistoBueno
	DECLARE @CatVentas VARCHAR(64) = 'Ventas.VentaPago'
	DECLARE @CatVentasCredito VARCHAR(64) = 'Ventas.Venta'
	DECLARE @CatVentasAF VARCHAR(64) = 'VentasAF.VentaPago'
	DECLARE @CatFacturasAnt VARCHAR(64) = 'FacturasAnt.VentaFactura'
	DECLARE @CatDevoluciones VARCHAR(64) = 'Devoluciones.VentaDevolucion'
	DECLARE @CatGarantias VARCHAR(64) = 'Garantias.VentaGarantia'
	DECLARE @Cat9500 VARCHAR(64) = '9500.Cotizacion9500'
	DECLARE @CatCobranza VARCHAR(64) = 'Cobranza.VentaPago'
	DECLARE @CatEgresos VARCHAR(64) = 'Egresos.CajaEgreso'
	DECLARE @CatIngresos VARCHAR(64) = 'Ingresos.CajaIngreso'
	-- AutorizacionProceso
	DECLARE @AutDevolucionID INT = 3
	DECLARE @AutGarantiaID INT = 16
	DECLARE @AutRefuerzoID INT = 8
	DECLARE @AutResguardoID INT = 9
	DECLARE @AutIngresoID INT = 6
	DECLARE @AutEgresoID INT = 5
	-- Estatus de Ventas
	DECLARE @VeCanceladaSinPago INT = 5
	-- Estatus genérico
	DECLARE @EgCompletado INT = 3
	-- Acciones de garantías
	DECLARE @AgArticuloNuevo INT = 1
	DECLARE @AgNotaDeCredito INT = 2
	DECLARE @AgEfectivo INT = 3
	DECLARE @AgRevision INT = 4
	DECLARE @AgNoProcede INT = 5
	DECLARE @AgCheque INT = 6
	DECLARE @AgTarjeta INT = 7
	--
	DECLARE @CteCuentaAuxiliar INT = 12

	DECLARE @DiaSig DATE = DATEADD(d, 1, @Dia)

	-- Ventas
	IF @OpcionID = @OpVentas BEGIN
		;WITH _Pagos AS (
			SELECT
				vp.VentaPagoID AS RegistroID
				, v.Folio
				, c.Nombre AS Cliente
				, ISNULL(dbo.fnuCadenaFormasDePago(vp.VentaPagoID), '') AS FormaDePago
				, u.NombreUsuario AS Usuario
				, CASE WHEN c9.Cotizacion9500ID IS NULL THEN '' ELSE '9500' END AS Caracteristica
				, ISNULL(vpc.Importe, 0.00) AS Importe
				, cvb.Fecha AS FechaVistoBueno
				, CASE WHEN vfd.VentaFacturaID IS NULL THEN 0 ELSE 1 END AS Facturada
				, CASE WHEN vd.VentaDevolucionID IS NULL THEN 0 ELSE 1 END AS Devolucion
				, CASE WHEN (vd.VentaDevolucionID IS NOT NULL AND v.Fecha < @Dia) THEN 1 ELSE 0 END AS NoContar
				, v.FolioIni
				, cvbfa.Fecha AS FechaVistoBuenoFA
				, @CatVentas AS CatTabla
				, 1 AS Orden
			FROM
				VentaPago vp
				LEFT JOIN (
					SELECT
						VentaPagoID
						, SUM(Importe) AS Importe
					FROM VentaPagoDetalle
					WHERE Estatus = 1
					GROUP BY VentaPagoID
				) vpc ON vpc.VentaPagoID = vp.VentaPagoID
				INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
				LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1

				LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
				LEFT JOIN VentaDevolucion vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
				LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatVentas AND cvb.TablaRegistroID = vp.VentaPagoID 
					AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
				LEFT JOIN CajaVistoBueno cvbfa ON cvbfa.CatTabla = @CatVentasAF AND cvbfa.TablaRegistroID = vp.VentaPagoID 
					AND (cvbfa.Fecha >= @Dia AND cvbfa.Fecha < @DiaSig)
				
				LEFT JOIN Cotizacion9500 c9 ON c9.AnticipoVentaID = vp.VentaID AND c9.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND vp.SucursalID = @SucursalID
				AND (vp.Fecha >= @Dia AND vp.Fecha < @DiaSig)
				AND v.ACredito = 0
			
			UNION
			
			-- Ventas a Crédito
			SELECT
				v.VentaID AS RegistroID
				, v.Folio
				, c.Nombre AS Cliente
				, '' AS FormaDePago
				, u.NombreUsuario AS Usuario
				, '' AS Caracteristica
				, ISNULL(vdc.Importe, 0.00) AS Importe
				, cvb.Fecha AS FechaVistoBueno
				, CASE WHEN vfd.VentaFacturaID IS NULL THEN 0 ELSE 1 END AS Facturada
				, CASE WHEN vd.VentaDevolucionID IS NULL THEN 0 ELSE 1 END AS Devolucion
				, 0 AS NoContar
				, NULL AS FolioIni
				, NULL AS FechaVistoBuenoAF
				, @CatVentasCredito AS CatTabla
				, 2 AS Orden
			FROM
				Venta v
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
				LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
				LEFT JOIN (
					SELECT
						VentaID
						, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
					FROM VentaDetalle
					WHERE Estatus = 1
					GROUP BY VentaID
				) vdc ON vdc.VentaID = v.VentaID

				LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
				LEFT JOIN VentaDevolucion vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
				LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatVentasCredito AND cvb.TablaRegistroID = v.VentaID 
					AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
			WHERE
				v.Estatus = 1
				AND v.ACredito = 1
				AND v.SucursalID = @SucursalID
				AND (v.Fecha >= @Dia AND v.Fecha < @DiaSig)
		)
		SELECT
			RegistroID
			, Folio
			, Cliente
			, FormaDePago
			, Usuario
			, Caracteristica
			, Importe
			, FechaVistoBueno
			, Devolucion
			, CatTabla
			, Facturada, Orden
		FROM _Pagos
		WHERE
			NoContar = 0
			AND (Facturada = 0 OR FolioIni IS NULL)
		
		-- Se agregan las ventas que primero fueron ventas y luego facturas
		UNION
		SELECT
			RegistroID
			, FolioIni AS Folio
			, Cliente
			, FormaDePago
			, Usuario
			, Folio AS Caracteristica
			, Importe
			, FechaVistoBuenoFA AS FechaVistoBueno
			, Devolucion
			, @CatVentasAF AS CatTabla
			, 0 AS Facturada, Orden
		FROM _Pagos
		WHERE
			NoContar = 0
			AND Facturada = 1
			AND FolioIni IS NOT NULL
		-- Se agregan las ventas de días anteriores que fueron facturadas
		UNION
		SELECT
			vf.VentaFacturaID AS RegistroID
			, (vf.Serie + vf.Folio) AS Folio
			, c.Nombre AS Cliente
			, '' AS FormaDePago
			, u.NombreUsuario AS Usuario
			, '' AS Caracteristica
			, ISNULL(vft.Importe, 0.00) AS Importe
			, cvb.Fecha AS FechaVistoBueno
			, CASE WHEN vfdev.VentaFacturaDevolucionID IS NULL THEN 0 ELSE 1 END AS Devolucion
			, @CatFacturasAnt AS CatTabla
			, 1 AS Facturada, 1 AS Orden
		FROM
			VentaFactura vf
			LEFT JOIN Cliente c ON c.ClienteID = vf.ClienteID AND c.Estatus = 1
			LEFT JOIN Usuario u ON u.UsuarioID = vf.RealizoUsuarioID AND u.Estatus = 1
			LEFT JOIN (
				SELECT
					vfd.VentaFacturaID
					, SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
				FROM
					VentaFacturaDetalle vfd
					LEFT JOIN VentaDetalle vd ON vd.VentaID = vfd.VentaID AND vd.Estatus = 1
				WHERE vfd.Estatus = 1
				GROUP BY vfd.VentaFacturaID
			) vft ON vft.VentaFacturaID = vf.VentaFacturaID
			LEFT JOIN VentaFacturaDevolucion vfdev ON vfdev.VentaFacturaID = vf.VentaFacturaID AND vfdev.Estatus = 1
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatFacturasAnt AND cvb.TablaRegistroID = vf.VentaFacturaID 
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			vf.Estatus = 1
			AND vf.SucursalID = @SucursalID
			AND (vf.Fecha >= @Dia AND vf.Fecha < @DiaSig)
			AND vf.Convertida = 1

		ORDER BY Orden, Facturada DESC, Folio
	END

	-- Devoluciones
	IF @OpcionID = @OpDevoluciones BEGIN
		SELECT
			vd.VentaDevolucionID
			, v.Folio
			, CASE WHEN vd.EsCancelacion = 1 THEN 'CANC-' ELSE '    -DEV' END AS Tipo
			, c.Nombre AS Cliente
			, CASE WHEN v.VentaEstatusID = @VeCanceladaSinPago AND v.ACredito = 1 THEN
				'CRÉDITO'
			  ELSE
				CASE vd.TipoFormaPagoID
					WHEN @FpEfectivoID THEN 'EFECTIVO'
					WHEN @FpChequeID THEN 'CHEQUE'
					WHEN @FpTarjetaID THEN 'TARJETA'
					WHEN @FpNotaDeCreditoID THEN ('-NC (' + ISNULL(CONVERT(VARCHAR(4), vd.NotaDeCreditoID), '') + ')')
				END
			  END AS Salida
			, (u.NombreUsuario + ' > ' + ISNULL(ua.NombreUsuario, '')) AS Autorizacion
			, vddc.Importe
			, cvb.Fecha AS FechaVistoBueno
		FROM
			VentaDevolucion vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = @AutDevolucionID -- AND a.Tabla = 'VentaDevolucion'
				AND a.TablaRegistroID = vd.VentaDevolucionID AND a.Estatus = 1
			LEFT JOIN Usuario u ON u.UsuarioID = vd.RealizoUsuarioID AND u.Estatus = 1
			LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
			LEFT JOIN (
				SELECT
					VentaDevolucionID
					, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
				FROM VentaDevolucionDetalle
				WHERE Estatus = 1
				GROUP BY VentaDevolucionID
			) vddc ON vddc.VentaDevolucionID = vd.VentaDevolucionID
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatDevoluciones
				AND cvb.TablaRegistroID = vd.VentaDevolucionID AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			vd.Estatus = 1
			AND vd.SucursalID = @SucursalID
			AND (vd.Fecha >= @Dia AND vd.Fecha < @DiaSig)
			AND (
				(@DevDiasAnt = 0 AND (v.Fecha >= @Dia AND v.Fecha < @DiaSig))
				OR (@DevDiasAnt = 1 AND v.Fecha < @Dia)
			)
	END

	-- Garantías
	IF @OpcionID = @OpGarantias BEGIN
		SELECT
			vg.VentaGarantiaID
			, v.Folio
			, c.Nombre AS Cliente
			, CASE WHEN v.VentaEstatusID = @VeCanceladaSinPago AND v.ACredito = 1 THEN
				'CRÉDITO'
			  ELSE
				CASE vg.AccionID
					WHEN @AgEfectivo THEN       'EFECTIVO'
					WHEN @AgNoProcede THEN      'NO PROCEDE'
					WHEN @AgArticuloNuevo THEN ('AN (' + ISNULL(CONVERT(VARCHAR(4), vg.NotaDeCreditoID), '') + ')')
					WHEN @AgNotaDeCredito THEN ('NC (' + ISNULL(CONVERT(VARCHAR(4), vg.NotaDeCreditoID), '') + ')')
					WHEN @AgCheque THEN         'CHEQUE'
					WHEN @AgTarjeta THEN        'TARJETA'
					WHEN @AgRevision THEN       'PROVEEDOR'
				END
			  END AS Salida
			, (u.NombreUsuario + ' > ' + ISNULL(ua.NombreUsuario, '')) AS Autorizacion
			, vgdc.Importe
			, cvb.Fecha AS FechaVistoBueno
		FROM
			VentaGarantia vg
			INNER JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN (
				SELECT
					ROW_NUMBER() OVER (PARTITION BY TablaRegistroID ORDER BY FechaAutorizo) AS Registro
					, TablaRegistroID
					, AutorizoUsuarioID
				FROM Autorizacion
				WHERE AutorizacionProcesoID = @AutGarantiaID AND Estatus = 1
			) ac ON ac.TablaRegistroID = vg.VentaGarantiaID AND ac.Registro = 1
			LEFT JOIN Usuario u ON u.UsuarioID = vg.RealizoUsuarioID AND u.Estatus = 1
			LEFT JOIN Usuario ua ON ua.UsuarioID = ac.AutorizoUsuarioID AND ua.Estatus = 1
			LEFT JOIN (
				SELECT
					VentaGarantiaID
					, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
				FROM VentaGarantiaDetalle
				WHERE Estatus = 1
				GROUP BY VentaGarantiaID
			) vgdc ON vgdc.VentaGarantiaID = vg.VentaGarantiaID

			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatGarantias
				AND cvb.TablaRegistroID = vg.VentaGarantiaID AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			vg.Estatus = 1
			-- AND vg.EstatusGenericoID = @EgCompletado
			AND vg.SucursalID = @SucursalID
			AND (vg.Fecha >= @Dia AND vg.Fecha < @DiaSig)
			AND (
				(@DevDiasAnt = 0 AND (v.Fecha >= @Dia AND v.Fecha < @DiaSig))
				OR (@DevDiasAnt = 1 AND v.Fecha < @Dia)
			)
	END

	-- 9500
	IF @OpcionID = @Op9500 BEGIN
		SELECT
			c9.Cotizacion9500ID
			, CONVERT(VARCHAR(8), c9.Cotizacion9500ID) AS Folio
			, c.Nombre AS Cliente
			, c9.Anticipo
			, cvb.Fecha AS FechaVistoBueno
		FROM
			Cotizacion9500 c9
			LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @Cat9500 AND cvb.TablaRegistroID = c9.Cotizacion9500ID 
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			c9.Estatus = 1
			AND c9.SucursalID = @SucursalID
			AND (c9.Fecha >= @Dia AND c9.Fecha < @DiaSig)
	END

	-- Cobranza
	IF @OpcionID = @OpCobranza BEGIN
		SELECT
			vp.VentaPagoID
			, v.Folio
			, c.Nombre AS Cliente
			, dbo.fnuCadenaFormasDePago(vp.VentaPagoID) AS FormaDePago
			, vpc.Importe
			, cvb.Fecha AS FechaVistoBueno
		FROM
			VentaPago vp
			LEFT JOIN (
				SELECT
					VentaPagoID
					, SUM(Importe) AS Importe
				FROM VentaPagoDetalle
				WHERE Estatus = 1
				GROUP BY VentaPagoID
			) vpc ON vpc.VentaPagoID = vp.VentaPagoID
			INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatCobranza AND cvb.TablaRegistroID = vp.VentaPagoID 
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			vp.Estatus = 1
			AND vp.SucursalID = @SucursalID
			AND (vp.Fecha >= @Dia AND vp.Fecha < @DiaSig)
			AND v.ACredito = 1
	END

	-- Gastos
	IF @OpcionID = @OpEgresos BEGIN
		SELECT
			ce.CajaEgresoID AS RegistroID
			, (CASE WHEN ce.CajaTipoEgresoID = @CteCuentaAuxiliar THEN cca.CuentaAuxiliar ELSE cte.NombreTipoEgreso END
				+ ' - ' + ce.Concepto) AS Concepto
			, (ur.NombreUsuario + ' > ' + ISNULL(ua.NombreUsuario, '')) AS Autorizacion
			, ce.Importe
			, cvb.Fecha AS FechaVistoBueno
		FROM
			CajaEgreso ce
			LEFT JOIN CajaTipoEgreso cte ON cte.CajaTipoEgresoID = ce.CajaTipoEgresoID AND cte.Estatus = 1
			LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID IN (@AutEgresoID, @AutResguardoID) -- AND a.Tabla = 'CajaEgreso'
				AND a.TablaRegistroID = ce.CajaEgresoID AND a.Estatus = 1
			LEFT JOIN Usuario ur ON ur.UsuarioID = ce.RealizoUsuarioID AND ur.Estatus = 1
			LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
			LEFT JOIN ContaEgreso cne ON cne.ContaEgresoID = ce.ContaEgresoID
			LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cne.ContaCuentaAuxiliarID
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatEgresos AND cvb.TablaRegistroID = ce.CajaEgresoID
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			ce.Estatus = 1
			AND ce.SucursalID = @SucursalID
			AND (ce.Fecha >= @Dia AND ce.Fecha < @DiaSig)
	END

	-- Ingresos
	IF @OpcionID = @OpIngresos BEGIN
		SELECT
			ci.CajaIngresoID AS RegistroID
			, (cti.NombreTipoIngreso + ' - ' + ci.Concepto) AS Concepto
			, (ur.NombreUsuario + ' > ' + ISNULL(ua.NombreUsuario, '')) AS Autorizacion
			, ci.Importe
			, cvb.Fecha AS FechaVistoBueno
		FROM
			CajaIngreso ci
			LEFT JOIN CajaTipoIngreso cti ON cti.CajaTipoIngresoID = ci.CajaTipoIngresoID AND cti.Estatus = 1
			LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID IN (@AutIngresoID, @AutRefuerzoID) -- AND a.Tabla = 'CajaIngreso'
				AND a.TablaRegistroID = ci.CajaIngresoID AND a.Estatus = 1
			LEFT JOIN Usuario ur ON ur.UsuarioID = ci.RealizoUsuarioID AND ur.Estatus = 1
			LEFT JOIN Usuario ua ON ua.UsuarioID = a.AutorizoUsuarioID AND ua.Estatus = 1
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatIngresos AND cvb.TablaRegistroID = ci.CajaIngresoID
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			ci.Estatus = 1
			AND ci.SucursalID = @SucursalID
			AND (ci.Fecha >= @Dia AND ci.Fecha < @DiaSig)
	END

	-- Ventas cambios
	IF @OpcionID = @OpCambios BEGIN
		SELECT
			vc.VentaCambioID
			, v.Folio
			, c.Nombre AS Cliente
			, (ISNULL(vc.FormasDePagoAntes, '--------------') + ' >> ' 
				+ ISNULL(vc.FormasDePagoDespues, '--------------')) AS FormasDePago
			, (ISNULL(ua.NombreUsuario, '--------------') + ' >> ' + ISNULL(ud.NombreUsuario, '--------------')) AS Vendedor
			, (ISNULL(cca.Alias, '--------------') + ' >> ' + ISNULL(ccd.Alias, '--------------')) AS Comisionista
		FROM
			VentaCambio vc
			LEFT JOIN VentaPago vp ON vp.VentaPagoID = vc.VentaPagoID AND vp.Estatus = 1
			LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN Usuario ua ON ua.UsuarioID = vc.RealizoIDAntes AND ua.Estatus = 1
			LEFT JOIN Usuario ud ON ud.UsuarioID = vc.RealizoIDDespues AND ud.Estatus = 1
			LEFT JOIN Cliente cca ON cca.ClienteID = vc.ComisionistaIDAntes AND cca.Estatus = 1
			LEFT JOIN Cliente ccd ON ccd.ClienteID = vc.ComisionistaIDDespues AND ccd.Estatus = 1
		WHERE
			vc.Estatus = 1
			AND vc.SucursalID = @SucursalID
			AND (vc.Fecha >= @Dia AND vc.Fecha < @DiaSig)
	END
END
GO

ALTER PROCEDURE [dbo].[pauParteBusquedaEnTraspasos] (
	@MarcaParteID INT
	, @LineaID INT
	, @ProveedorID INT
	, @SucursalOrigenID INT
	, @SucursalDestinoID INT
	, @Abcs NVARCHAR(20)
) AS BEGIN
		
	--DECLARE @MarcaParteID INT = 0 --12
	--DECLARE @LineaID INT = 0 --180
	--DECLARE @ProveedorID INT = 0 --10
	--DECLARE @SucursalOrigenID INT = 1 
	--DECLARE @SucursalDestinoID INT = 3
	--DECLARE @Abcs NVARCHAR(20) = 'A'

	SELECT
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,Parte.MarcaParteID	
		,Parte.LineaID	
		,Sistema.SistemaID
		,Parte.SubsistemaID
		, pv.NombreProveedor AS Proveedor
		,ExistenciaOrigen.SucursalID
		,ExistenciaOrigen.Existencia
		,MxMnOrigen.Maximo
		,MxMnOrigen.Minimo
		,ExistenciaDestino.SucursalID AS SucursalDestinoID
		,ExistenciaDestino.Existencia AS DestinoExistencia
		,MxMnDestino.Maximo AS DestinoMaximo
		,MxMnDestino.Minimo AS DestinoMinimo
		,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		,Parte.NumeroParte + Parte.NombreParte AS Busqueda
	FROM
		Parte	
		INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
		INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteAbc ON ParteAbc.ParteID = Parte.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = Parte.ProveedorID AND pv.Estatus = 1
	WHERE
		Parte.Estatus = 1
		AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
		AND (@MarcaParteID <= 0 OR Parte.MarcaParteID = @MarcaParteID)
		AND (@LineaID <= 0 OR Parte.LineaID = @LineaID)
		AND (@ProveedorID <= 0 OR Parte.ProveedorID = @ProveedorID)
		AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
		AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
		AND ExistenciaOrigen.Existencia > 0
		AND ExistenciaDestino.Existencia <= MxMnDestino.Minimo
		AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)

END
GO

ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@SucursalID NVARCHAR(10)
) 
AS BEGIN
SET NOCOUNT ON

	DECLARE @EstGenPendiente INT = 2
	DECLARE @EstPedidoNoSurtido INT = 2
	DECLARE @OpeTraspaso INT = 5

	;WITH _Pedidos AS (
		SELECT
			ParteID	
			,CAST(1 AS BIT) AS Sel
			,NumeroParte	
			,NombreParte	
			,UnidadEmpaque	
			,CriterioABC	
			,MaxMatriz	
			,MaxSuc02	
			,MaxSuc03	
			,ExistenciaMatriz	
			,ExistenciaSuc02	
			,ExistenciaSuc03	
			, CASE WHEN MinMatriz > ExistenciaMatriz THEN MaxMatriz - ExistenciaMatriz - TraspasoMatriz ELSE 0.0 END AS NecesidadMatriz
			, CASE WHEN MinSuc02 > ExistenciaSuc02 THEN MaxSuc02 - ExistenciaSuc02 - TraspasoSuc02 ELSE 0.0 END AS NecesidadSuc02
			, CASE WHEN MinSuc03 > ExistenciaSuc03 THEN MaxSuc03 - ExistenciaSuc03 - TraspasoSuc03 ELSE 0.0 END AS NecesidadSuc03
			-- , MaxMatriz - ExistenciaMatriz AS NecesidadMatriz
			-- , MaxSuc02 - ExistenciaSuc02 AS NecesidadSuc02
			-- , MaxSuc03 - ExistenciaSuc03 AS NecesidadSuc03
			-- ,(MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) AS Total
			-- ,CEILING((MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) /
			-- (CASE WHEN Pre.UnidadEmpaque = 0 THEN 1 ELSE Pre.UnidadEmpaque END)) * Pre.UnidadEmpaque AS Pedido	
			,Costo	
			,ProveedorID	
			,NombreProveedor	
			,Beneficiario
			, '' AS Observacion
		FROM (
			SELECT 
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, p.UnidadEmpaque
				,ParteAbc.AbcDeVentas AS CriterioABC	
				, SUM(CASE WHEN pe.SucursalID = 1 THEN pmm.Maximo ELSE 0.0 END) AS MaxMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 THEN pmm.Maximo ELSE 0.0 END) AS MaxSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 THEN pmm.Maximo ELSE 0.0 END) AS MaxSuc03	
				, SUM(CASE WHEN pmm.SucursalID = 1 THEN pmm.Minimo ELSE 0.0 END) AS MinMatriz
				, SUM(CASE WHEN pmm.SucursalID = 2 THEN pmm.Minimo ELSE 0.0 END) AS MinSuc02
				, SUM(CASE WHEN pmm.SucursalID = 3 THEN pmm.Minimo ELSE 0.0 END) AS MinSuc03
				, SUM(CASE WHEN pe.SucursalID = 1 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc03
				, SUM(CASE WHEN tra.SucursalID = 1 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoMatriz
				, SUM(CASE WHEN tra.SucursalID = 2 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoSuc02
				, SUM(CASE WHEN tra.SucursalID = 3 THEN tra.Cantidad ELSE 0.0 END) AS TraspasoSuc03
				,PartePrecio.Costo
				, p.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			FROM 
				Parte p
				INNER JOIN ParteAbc ON  ParteAbc.ParteID = p.ParteID
				INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
				INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = pmm.SucursalID
				INNER JOIN PartePrecio ON PartePrecio.ParteID = p.ParteID
				INNER JOIN Proveedor ON Proveedor.ProveedorID = p.ProveedorID
				LEFT JOIN (
					SELECT
						mi.SucursalDestinoID AS SucursalID
						, mid.ParteID
						, mid.Cantidad
					FROM
						MovimientoInventario mi
						INNER JOIN MovimientoInventarioDetalle mid ON mid.MovimientoInventarioID = mi.MovimientoInventarioID AND mid.Estatus = 1
					WHERE
						mi.Estatus = 1
						AND mi.TipoOperacionID = @OpeTraspaso
						AND mi.FechaRecepcion IS NULL
				) tra ON tra.ParteID = p.ParteID AND tra.SucursalID = pmm.SucursalID
			WHERE
				p.Estatus = 1 
				AND pmm.Maximo > 0
				-- AND pe.Existencia <= pmm.Minimo
				AND p.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
				AND pe.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
				/* Ya se incluyen aún las partes que estén en traspaso
				AND Parte.ParteID NOT IN 
					(SELECT d.ParteID 
					FROM MovimientoInventario m 
					INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID 
					WHERE m.TipoOperacionID = 5 
					AND m.SucursalDestinoID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
					AND m.FechaRecepcion IS NULL 
					AND m.Estatus = 1)
				*/
			GROUP BY
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, p.UnidadEmpaque
				,ParteAbc.AbcDeVentas
				,PartePrecio.Costo
				, p.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			) AS Pre
		
		WHERE
			(ExistenciaMatriz <= MinMatriz
			OR ExistenciaSuc02 <= MinSuc02
			OR ExistenciaSuc03 <= MinSuc03)
			-- Se sacan los que su necesidad se pueda cubrir con la existencia de matriz
			AND (ExistenciaMatriz - MinMatriz) <= ((MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03))
	)
	SELECT
		p.ParteID	
		,CAST(1 AS BIT) AS Sel
		, p.NumeroParte	
		, p.NombreParte	
		, p.UnidadEmpaque	
		, p.CriterioABC	
		,MaxMatriz	
		,MaxSuc02	
		,MaxSuc03	
		,ExistenciaMatriz	
		,ExistenciaSuc02	
		,ExistenciaSuc03	
		, NecesidadMatriz
		, NecesidadSuc02
		, NecesidadSuc03
		, p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03 AS Total
		, CEILING((p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03) /
			(CASE WHEN pt.UnidadEmpaque = 0 THEN 1 ELSE pt.UnidadEmpaque END)) * pt.UnidadEmpaque AS Pedido	
		,Costo	
		, p.ProveedorID
		,NombreProveedor	
		,Beneficiario
		, '' AS Observacion
		-- Para los que no se deben pedir por la existencia en sus equivalentes
		, CASE WHEN (
			(p.ExistenciaMatriz <= 0 AND pc.ExEquivMatriz > 0)
			OR (p.ExistenciaSuc02 <= 0 AND pc.ExEquivSuc02 > 0)
			OR (p.ExistenciaSuc03 <= 0 AND pc.ExEquivSuc03 > 0)
		) THEN 'NP' ELSE 'MxMn' END AS Caracteristica
	FROM
		_Pedidos p
		LEFT JOIN (
			SELECT
				pi.ParteID
				, SUM(CASE WHEN pe.SucursalID = 1 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivMatriz
				, SUM(CASE WHEN pe.SucursalID = 2 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivSuc02
				, SUM(CASE WHEN pe.SucursalID = 3 AND ISNULL(pmm.Calcular, 0) != 1 THEN pe.Existencia ELSE 0.0 END) AS ExEquivSuc03
			FROM
				_Pedidos pi
				LEFT JOIN ParteEquivalente pq ON pq.ParteID = pi.ParteID AND pq.Estatus = 1
				LEFT JOIN ParteEquivalente pee ON pee.GrupoID = pq.GrupoID AND pee.ParteID != pq.ParteID AND pee.Estatus = 1
				LEFT JOIN ParteExistencia pe ON pe.ParteID = pee.ParteID AND pe.Estatus = 1
				LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID and pmm.SucursalID = pe.SucursalID
			GROUP BY pi.ParteID
		) pc ON pc.ParteID = p.ParteID
		LEFT JOIN Parte pt ON pt.ParteID = p.ParteID AND pt.Estatus = 1
	WHERE
		CEILING((p.NecesidadMatriz + p.NecesidadSuc02 + p.NecesidadSuc03) /
			(CASE WHEN pt.UnidadEmpaque = 0 THEN 1 ELSE pt.UnidadEmpaque END)) * pt.UnidadEmpaque
		> 0

	-- Se agregan los 9500
	UNION
	SELECT
		c9d.ParteID
		, CAST(1 AS BIT) AS Sel
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas AS CriterioAbc
		, NULL AS MaxMatriz
		, NULL AS MaxSuc02
		, NULL AS MaxSuc03
		-- , SUM(CASE WHEN pe.SucursalID = 1 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaMatriz
		-- , SUM(CASE WHEN pe.SucursalID = 2 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc02
		-- , SUM(CASE WHEN pe.SucursalID = 3 THEN pe.Existencia ELSE 0.0 END) AS ExistenciaSuc03
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, (SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaMatriz, 0.0)) AS NecesidadMatriz
		, (SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc02, 0.0)) AS NecesidadSuc02
		, (SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc03, 0.0)) AS NecesidadSuc03
		, SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			- ISNULL(pec.ExistenciaMatriz, 0.0) - ISNULL(pec.ExistenciaSuc02, 0.0) - ISNULL(pec.ExistenciaSuc03, 0.0)
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, '' AS Observacion
		, '9500' AS Caracteristica
	FROM
		Cotizacion9500Detalle c9d
		INNER JOIN Cotizacion9500 c9 ON c9.Cotizacion9500ID = c9d.Cotizacion9500ID AND c9.Estatus = 1
		LEFT JOIN PedidoDetalle pd ON pd.ParteID = c9d.ParteID AND pd.PedidoEstatusID = @EstPedidoNoSurtido AND pd.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = c9d.ParteID
		-- LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = c9d.ParteID
		-- LEFT JOIN ParteExistencia pe ON pe.ParteID = c9d.ParteID AND pe.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(CASE WHEN SucursalID = 1 THEN Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN SucursalID = 2 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN SucursalID = 3 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc03
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = c9d.ParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = c9d.ParteID AND pp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		c9d.Estatus = 1
		AND c9.EstatusGenericoID = @EstGenPendiente
		AND c9.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
		AND pd.PedidoDetalleID IS NULL
	GROUP BY
		c9d.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
	HAVING
		(SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaMatriz, 0.0)) > 0
		OR (SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc02, 0.0)) > 0
		OR (SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END) - ISNULL(pec.ExistenciaSuc03, 0.0)) > 0

	-- Se agregan los de Reporte de Faltante
	UNION
	SELECT
		rf.ParteID
		, CAST(1 AS BIT) AS Sel
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas AS CriterioABC
		, NULL AS MaxMatriz
		, NULL AS MaxSuc02
		, NULL AS MaxSuc03
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadMatriz
		, SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadSuc02
		, SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END) AS NecesidadSuc03
		, SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END)
			+ SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END)
			+ SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END)
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN rf.SucursalID = 1 THEN rf.CantidadRequerida ELSE 0.0 END)
				+ SUM(CASE WHEN rf.SucursalID = 2 THEN rf.CantidadRequerida ELSE 0.0 END)
				+ SUM(CASE WHEN rf.SucursalID = 3 THEN rf.CantidadRequerida ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, ('(' + u.NombreUsuario + ') ' + rf.Comentario) AS Observacion
		, 'RF' AS Caracteristica
	FROM
		ReporteDeFaltante rf
		LEFT JOIN Parte p ON p.ParteID = rf.ParteID AND p.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = rf.ParteID
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(CASE WHEN SucursalID = 1 THEN Existencia ELSE 0.0 END) AS ExistenciaMatriz
				, SUM(CASE WHEN SucursalID = 2 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc02
				, SUM(CASE WHEN SucursalID = 3 THEN Existencia ELSE 0.0 END) AS ExistenciaSuc03
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = rf.ParteID
		LEFT JOIN PartePrecio pp ON pp.ParteID = rf.ParteID AND pp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = rf.RealizoUsuarioID AND u.Estatus = 1
	WHERE
		rf.Estatus = 1
		AND rf.Pedido = 0
		AND rf.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
	GROUP BY
		rf.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, rf.Comentario
		, u.NombreUsuario

END
GO

ALTER PROCEDURE [dbo].[pauParteBusquedaEnTraspasos] (
	@MarcaParteID INT
	, @LineaID INT
	, @ProveedorID INT
	, @SucursalOrigenID INT
	, @SucursalDestinoID INT
	, @Abcs NVARCHAR(20)
) AS BEGIN
		
	--DECLARE @MarcaParteID INT = 0 --12
	--DECLARE @LineaID INT = 0 --180
	--DECLARE @ProveedorID INT = 0 --10
	--DECLARE @SucursalOrigenID INT = 1 
	--DECLARE @SucursalDestinoID INT = 3
	--DECLARE @Abcs NVARCHAR(20) = 'A'

	SELECT
		Parte.ParteID
		,Parte.NumeroParte
		,Parte.NombreParte
		,Parte.MarcaParteID	
		,Parte.LineaID	
		,Sistema.SistemaID
		,Parte.SubsistemaID
		, pv.NombreProveedor AS Proveedor
		,ExistenciaOrigen.SucursalID
		,ExistenciaOrigen.Existencia
		,MxMnOrigen.Maximo
		,MxMnOrigen.Minimo
		,ExistenciaDestino.SucursalID AS SucursalDestinoID
		,ExistenciaDestino.Existencia AS DestinoExistencia
		,MxMnDestino.Maximo AS DestinoMaximo
		,MxMnDestino.Minimo AS DestinoMinimo
		,ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) AS Sugerencia
		,Parte.NumeroParte + Parte.NombreParte AS Busqueda
	FROM
		Parte	
		INNER JOIN Subsistema ON Subsistema.SubsistemaID = Parte.SubsistemaID
		INNER JOIN Sistema ON Sistema.SistemaID = Subsistema.SistemaID
		INNER JOIN ParteExistencia AS ExistenciaOrigen ON ExistenciaOrigen.ParteID = Parte.ParteID AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteExistencia AS ExistenciaDestino ON ExistenciaDestino.ParteID = Parte.ParteID AND ExistenciaDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteMaxMin AS MxMnOrigen ON MxMnOrigen.ParteID = Parte.ParteID AND MxMnOrigen.SucursalID = @SucursalOrigenID
		INNER JOIN ParteMaxMin AS MxMnDestino ON MxMnDestino.ParteID = Parte.ParteID AND MxMnDestino.SucursalID = @SucursalDestinoID
		INNER JOIN ParteAbc ON ParteAbc.ParteID = Parte.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = Parte.ProveedorID AND pv.Estatus = 1
	WHERE
		Parte.Estatus = 1
		AND ExistenciaOrigen.SucursalID = @SucursalOrigenID
		AND ExistenciaDestino.SucursalID = @SucursalDestinoID	
		AND (@MarcaParteID <= 0 OR Parte.MarcaParteID = @MarcaParteID)
		AND (@LineaID <= 0 OR Parte.LineaID = @LineaID)
		AND (@ProveedorID <= 0 OR Parte.ProveedorID = @ProveedorID)
		AND ParteAbc.AbcDeVentas IN (SELECT * FROM dbo.fnuDividirCadena(@Abcs, ','))
		AND ISNULL(MxMnDestino.Maximo - ExistenciaDestino.Existencia, 0) > 0
		AND ExistenciaOrigen.Existencia > 0
		AND ExistenciaDestino.Existencia <= MxMnDestino.Minimo
		-- AND Parte.ParteID NOT IN (SELECT d.ParteID FROM MovimientoInventario m INNER JOIN MovimientoInventarioDetalle d ON m.MovimientoInventarioID = d.MovimientoInventarioID WHERE m.TipoOperacionID = 5 AND m.SucursalDestinoID = @SucursalDestinoID AND m.FechaRecepcion IS NULL AND m.Estatus = 1)

END
GO
