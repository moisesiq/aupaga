/* Script con modificaciones a la base de datos de Theos. Archivo 039
 * Creado: 2015/10/12
 * Subido: 2015/10/14
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE ContaCuentaAuxiliarDevengadoEspecial (
	ContaCuentaAuxiliarDevengadoEspecialID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ContaCuentaAuxiliarID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaAuxiliar(ContaCuentaAuxiliarID)
	, DuenioID INT NOT NULL FOREIGN KEY REFERENCES Duenio(DuenioID)
)

CREATE TABLE ContaEgresoDetalleDevengadoEspecial (
	ContaEgresoDetalleDevengadoEspecialID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ContaEgresoDetalleID INT NOT NULL FOREIGN KEY REFERENCES ContaEgresoDetalle(ContaEgresoDetalleID)
	, ContaEgresoDevengadoEspecialID INT NOT NULL FOREIGN KEY REFERENCES ContaEgresoDevengadoEspecial(ContaEgresoDevengadoEspecialID)
	, Cantidad DECIMAL(7, 2) NOT NULL
)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[MovimientoInventarioView] AS
	SELECT
		MovimientoInventario.MovimientoInventarioID
		,MovimientoInventario.TipoOperacionID
		,TipoOperacion.NombreTipoOperacion
		,MovimientoInventario.TipoPagoID	
		,TipoPago.NombreTipoPago
		,MovimientoInventario.ProveedorID	
		,Proveedor.NombreProveedor
		,MovimientoInventario.SucursalOrigenID	
		,s1.NombreSucursal AS SucursalOrigen
		,MovimientoInventario.SucursalDestinoID	
		,s2.NombreSucursal AS SucursalDestino
		,MovimientoInventario.FechaFactura	
		,MovimientoInventario.FechaRecepcion	
		,MovimientoInventario.FolioFactura	
		,MovimientoInventario.AplicaEnMovimientoInventarioID	
		,MovimientoInventario.FechaAplicacion	
		,MovimientoInventario.Subtotal	
		,MovimientoInventario.IVA	
		,MovimientoInventario.ImporteTotal
		, MovimientoInventario.ImporteFactura
		,MovimientoInventario.FueLiquidado	
		-- ,MovimientoInventario.ConceptoMovimiento	
		, MovimientoInventario.TipoConceptoOperacionID
		, tco.NombreConceptoOperacion AS ConceptoOperacion
		,MovimientoInventario.Observacion	
		,MovimientoInventario.UsuarioID	
		,Usuario.NombreUsuario
		,MovimientoInventario.FechaRegistro	
		,MovimientoInventario.FechaModificacion	
		,MovimientoInventario.NombreImagen	
		,MovimientoInventario.Articulos
		,MovimientoInventario.Unidades
		,MovimientoInventario.Seguro
		,MovimientoInventario.ImporteTotalSinDescuento
	FROM
		MovimientoInventario
		LEFT JOIN Proveedor ON Proveedor.ProveedorID = MovimientoInventario.ProveedorID
		INNER JOIN TipoOperacion ON TipoOperacion.TipoOperacionID = MovimientoInventario.TipoOperacionID
		LEFT JOIN TipoPago ON TipoPago.TipoPagoID = MovimientoInventario.TipoPagoID
		INNER JOIN Sucursal s1 ON s1.SucursalID = MovimientoInventario.SucursalOrigenID
		INNER JOIN Sucursal s2 ON s2.SucursalID = MovimientoInventario.SucursalDestinoID
		INNER JOIN Usuario ON Usuario.UsuarioID = MovimientoInventario.UsuarioID
		LEFT JOIN TipoConceptoOperacion tco ON tco.TipoConceptoOperacionID = MovimientoInventario.TipoConceptoOperacionID
	WHERE
		MovimientoInventario.Estatus = 1
GO

ALTER VIEW [dbo].[ContaEgresosDetalleView] AS
	SELECT
		ced.ContaEgresoDetalleID
		, ced.ContaEgresoID
		, ced.ContaConsumibleID
		, cc.Consumible
		, ced.Cantidad
		, ced.Importe
		-- , SUM(cedd.Cantidad) OVER (PARTITION BY cedd.ContaEgresoDetalleID) AS CantidadDev
		, SUM(edd.Cantidad) AS CantidadDev
		, SUM(edde.Cantidad) AS CantidadDevEspecial
	FROM
		ContaEgresoDetalle ced
		LEFT JOIN ContaConsumible cc ON cc.ContaConsumibleID = ced.ContaConsumibleID
		LEFT JOIN (
			SELECT ContaEgresoDetalleID, SUM(Cantidad) AS Cantidad			
			FROM ContaEgresoDetalleDevengado
			GROUP BY ContaEgresoDetalleID
		) edd ON edd.ContaEgresoDetalleID = ced.ContaEgresoDetalleID
		LEFT JOIN (
			SELECT ContaEgresoDetalleID, SUM(Cantidad) AS Cantidad			
			FROM ContaEgresoDetalleDevengadoEspecial
			GROUP BY ContaEgresoDetalleID
		) edde ON edde.ContaEgresoDetalleID = ced.ContaEgresoDetalleID
	GROUP BY
		ced.ContaEgresoDetalleID
		, ced.ContaEgresoID
		, ced.ContaConsumibleID
		, cc.Consumible
		, ced.Cantidad
		, ced.Importe
GO

ALTER VIEW [dbo].[ContaEgresosDevengadoEspecialCuentasView] AS
	SELECT
		ede.ContaEgresoDevengadoEspecialID
		, ede.ContaEgresoID
		, ede.DuenioID
		, d.Duenio
		, cc.ContaCuentaID
		, cc.Cuenta
		, cs.ContaSubcuentaID
		, cs.Subcuenta
		, ccm.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, cca.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, cca.PeriodicidadMes
		, cca.FinSemanalizar
		, ede.Fecha
		, ede.Importe AS ImporteDev
	FROM
		ContaEgresoDevengadoEspecial ede
		INNER JOIN ContaEgreso ce ON ce.ContaEgresoID = ede.ContaEgresoID
		INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
		LEFT JOIN Duenio d ON d.DuenioID = ede.DuenioID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
		LEFT JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
		LEFT JOIN ContaCuenta cc ON cc.ContaCuentaID = cs.ContaCuentaID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauCajaCorte] (
	@SucursalID INT
	, @Dia DATE
) AS BEGIN
	SET NOCOUNT ON

	/* Posible fallo:
		- Cancelaciones/Devoluciones de crédito, con o sin pagos
	*/

	/* EXEC pauCajaCorte 1, '2014-11-15'
	DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = GETDATE()
	*/

	-- Definición de variables tipo constante
	-- Formas de pago
	DECLARE @FpEfectivoID INT = 2
	DECLARE @FpChequeID INT = 1
	DECLARE @FpTarjetaID INT = 4
	DECLARE @FpTransferenciaID INT = 3
	DECLARE @FpNotaDeCreditoID INT = 6
	DECLARE @FpNoIdentificadoID INT = 5
	DECLARE @FpNotaDeCreditoFiscalID INT = 7
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
	DECLARE @AgNoProcede INT = 5
	DECLARE @AgCheque INT = 6
	DECLARE @AgTarjeta INT = 7

	-- Variables calculadas para el proceso
	DECLARE @DiaSig DATE = DATEADD(d, 1, @Dia)

	-- Para los pagos (Ingresos)
	;WITH _Ingresos AS (
		SELECT
			vpd.TipoFormaPagoID
			-- Se separan los pagos de ventas a crédito (Cobranza), de las normales
			, SUM(CASE WHEN v.ACredito = 0 AND vfd.VentaFacturaID IS NULL THEN vpd.Importe ELSE 0.00 END) AS Tickets
			, SUM(CASE WHEN v.ACredito = 0 AND vfd.VentaFacturaID IS NOT NULL THEN vpd.Importe ELSE 0.00 END) AS Facturas
			-- , SUM(CASE WHEN v.ACredito = 0 THEN vpd.Importe ELSE 0.00 END) AS Total
			-- Pagos que van para Cobranza
			, SUM(CASE WHEN v.ACredito = 1 AND vfd.VentaFacturaID IS NULL THEN vpd.Importe ELSE 0.00 END) AS TicketsCre
			, SUM(CASE WHEN v.ACredito = 1 AND vfd.VentaFacturaID IS NOT NULL THEN vpd.Importe ELSE 0.00 END) AS FacturasCre
			-- , SUM(CASE WHEN v.ACredito = 1 THEN vpd.Importe ELSE 0.00 END) AS TotalCre
			-- Visto Bueno
			, SUM(CASE WHEN v.ACredito = 0 AND cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) AS Pendientes
			, SUM(CASE WHEN v.ACredito = 1 AND cvb2.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) AS PendientesCre
		FROM
			VentaPago vp
			INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
			LEFT JOIN (SELECT DISTINCT VentaID FROM VentaDevolucion WHERE Estatus = 1) vdc ON vdc.VentaID = v.VentaID
			LEFT JOIN (SELECT VentaID FROM VentaGarantia WHERE Estatus = 1) vg ON vg.VentaID = v.VentaID
			LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID
				-- Si el importe es negativo, se verifica si la devolución es de días anteriores, en cuyo caso, no se cuenta
				-- pues esas ya restan abajo en el corte
				-- Se quitan también las garantías de días anteriores
				AND (vpd.Importe >= 0 OR (vdc.VentaID IS NULL OR v.Fecha >= @Dia))
				AND (vpd.Importe >= 0 OR (vg.VentaID IS NULL OR v.Fecha >= @Dia))
				-- Si es una nota de crédito negativa, no se cuenta aquí
				AND (vpd.TipoFormaPagoID != @FpNotaDeCreditoID OR (vpd.TipoFormaPagoID = @FpNotaDeCreditoID AND vpd.Importe > 0))
				AND vpd.Estatus = 1
			LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND Primera = 1 AND vfd.Estatus = 1
			-- LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatVentas AND cvb.TablaRegistroID = vp.VentaPagoID
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
			LEFT JOIN CajaVistoBueno cvb2 ON cvb2.CatTabla = @CatCobranza AND cvb2.TablaRegistroID = vp.VentaPagoID
				AND (cvb2.Fecha >= @Dia AND cvb2.Fecha < @DiaSig)
		WHERE
			vp.Estatus = 1
			AND vp.SucursalID = @SucursalID
			AND (vp.Fecha >= @Dia AND vp.Fecha < @DiaSig)
		GROUP BY
			vpd.TipoFormaPagoID
	)

	-- Para las Devoluciones (Egresos)
	, _Devoluciones AS (
		SELECT
			SUM(
				CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND vfd.VentaFacturaID IS NULL) THEN 
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vddc.Importe THEN vddc.Importe ELSE vpc.Pagado END)
					ELSE
						vddc.Importe
					END)
				ELSE
					0.00
				END
			) AS TicketsDia
			, SUM(
				CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND vfd.VentaFacturaID IS NOT NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vddc.Importe THEN vddc.Importe ELSE vpc.Pagado END)
					ELSE
						vddc.Importe
					END)
				ELSE
					0.00
				END
			) AS FacturasDia
			, SUM(
				CASE WHEN (v.Fecha < @Dia AND vfd.VentaFacturaID IS NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vddc.Importe THEN vddc.Importe ELSE vpc.Pagado END)
					ELSE
						vddc.Importe
					END)
				ELSE
					0.00
				END
			) AS TicketsAnt
			, SUM(
				CASE WHEN (v.Fecha < @Dia AND vfd.VentaFacturaID IS NOT NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vddc.Importe THEN vddc.Importe ELSE vpc.Pagado END)
					ELSE
						vddc.Importe
					END)
				ELSE
					0.00
				END
			) AS FacturasAnt
			
			-- Visto Bueno
			, SUM(CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND cvb.CajaVistoBuenoID IS NULL) THEN 1 ELSE 0 END) 
				+ SUM(CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND ISNULL(a.Autorizado, 0) = 0) THEN 1 ELSE 0 END)
				AS PendientesDia
			, SUM(CASE WHEN (v.Fecha < @Dia AND cvb.CajaVistoBuenoID IS NULL) THEN 1 ELSE 0 END) 
				+ SUM(CASE WHEN (v.Fecha < @Dia AND ISNULL(a.Autorizado, 0) = 0) THEN 1 ELSE 0 END) AS PendientesAnt
		FROM
			VentaDevolucion vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND Primera = 1 AND vfd.Estatus = 1
			LEFT JOIN (
				SELECT
					VentaDevolucionID
					, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
				FROM VentaDevolucionDetalle
				WHERE Estatus = 1
				GROUP BY VentaDevolucionID
			) vddc ON vddc.VentaDevolucionID = vd.VentaDevolucionID
			
			-- Para determinar lo que ha pagado
			LEFT JOIN (
				SELECT
					vpi.VentaID
					, SUM(vpdi.Importe) AS Pagado
				FROM
					VentaPago vpi
					LEFT JOIN VentaPagoDetalle vpdi ON vpdi.VentaPagoID = vpi.VentaPagoID AND vpdi.Importe > 0
						AND vpdi.Estatus = 1
				WHERE vpi.Estatus = 1
				GROUP BY vpi.VentaID
			) vpc ON vpc.VentaID = vd.VentaID
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatDevoluciones
				AND cvb.TablaRegistroID = vd.VentaDevolucionID AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
			LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = @AutDevolucionID AND a.TablaRegistroID = vd.VentaDevolucionID
				AND a.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND vd.SucursalID = @SucursalID
			AND (vd.Fecha >= @Dia AND vd.Fecha < @DiaSig)
			AND vd.TipoFormaPagoID != @FpNotaDeCreditoID
	)

	-- Para las Garantías (Egresos)
	, _Garantias AS (
		SELECT
			SUM(
				CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND vfd.VentaFacturaID IS NULL) THEN 
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > (vg.PrecioUnitario + vg.Iva) THEN (vg.PrecioUnitario + vg.Iva)
							ELSE vpc.Pagado END)
					ELSE
						(vg.PrecioUnitario + vg.Iva)
					END)
				ELSE
					0.00
				END
			) AS TicketsDia
			, SUM(
				CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND vfd.VentaFacturaID IS NOT NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > (vg.PrecioUnitario + vg.Iva) THEN (vg.PrecioUnitario + vg.Iva)
							ELSE vpc.Pagado END)
					ELSE
						(vg.PrecioUnitario + vg.Iva)
					END)
				ELSE
					0.00
				END
			) AS FacturasDia
			, SUM(
				CASE WHEN (v.Fecha < @Dia AND vfd.VentaFacturaID IS NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > (vg.PrecioUnitario + vg.Iva) THEN (vg.PrecioUnitario + vg.Iva)
							ELSE vpc.Pagado END)
					ELSE
						(vg.PrecioUnitario + vg.Iva)
					END)
				ELSE
					0.00
				END
			) AS TicketsAnt
			, SUM(
				CASE WHEN (v.Fecha < @Dia AND vfd.VentaFacturaID IS NOT NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > (vg.PrecioUnitario + vg.Iva) THEN (vg.PrecioUnitario + vg.Iva)
							ELSE vpc.Pagado END)
					ELSE
						(vg.PrecioUnitario + vg.Iva)
					END)
				ELSE
					0.00
				END
			) AS FacturasAnt
			
			-- Visto Bueno
			, SUM(CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND cvb.CajaVistoBuenoID IS NULL) THEN 1 ELSE 0 END) 
				+ SUM(CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND ac.AutorizoUsuarioID IS NULL) THEN 1 ELSE 0 END)
				AS PendientesDia
			, SUM(CASE WHEN (v.Fecha < @Dia AND cvb.CajaVistoBuenoID IS NULL) THEN 1 ELSE 0 END) 
				+ SUM(CASE WHEN (v.Fecha < @Dia AND ac.AutorizoUsuarioID IS NULL) THEN 1 ELSE 0 END) AS PendientesAnt
		FROM
			VentaGarantia vg
			INNER JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND Primera = 1 AND vfd.Estatus = 1
			
			-- Para determinar lo que ha pagado
			LEFT JOIN (
				SELECT
					vpi.VentaID
					, SUM(vpdi.Importe) AS Pagado
				FROM
					VentaPago vpi
					LEFT JOIN VentaPagoDetalle vpdi ON vpdi.VentaPagoID = vpi.VentaPagoID AND vpdi.Importe > 0
						AND vpdi.Estatus = 1
				WHERE vpi.Estatus = 1
				GROUP BY vpi.VentaID
			) vpc ON vpc.VentaID = vg.VentaID
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatGarantias
				AND cvb.TablaRegistroID = vg.VentaGarantiaID AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
			LEFT JOIN (
				SELECT
					ROW_NUMBER() OVER (PARTITION BY TablaRegistroID ORDER BY FechaAutorizo) AS Registro
					, TablaRegistroID
					, AutorizoUsuarioID
				FROM Autorizacion
				WHERE AutorizacionProcesoID = @AutGarantiaID AND Estatus = 1
			) ac ON ac.TablaRegistroID = vg.VentaGarantiaID AND ac.Registro = 1
		WHERE
			vg.Estatus = 1
			-- AND vg.EstatusGenericoID = @EgCompletado
			AND vg.SucursalID = @SucursalID
			AND (vg.Fecha >= @Dia AND vg.Fecha < @DiaSig)
			AND vg.AccionID IN (@AgEfectivo, @AgCheque, @AgTarjeta)
	)

	-- Fondo de Caja
	SELECT
		1 AS Orden
		, 'Ingresos' AS Categoria
		, 'Fondo de Caja' AS Concepto
		, 0.00 AS Tickets
		, 0.00 AS Facturas
		, Inicio AS Total
		, 0 AS Pendientes
	FROM
		(SELECT 1 AS Orden) v
		LEFT JOIN CajaEfectivoPorDia ON SucursalID = @SucursalID AND Dia = @Dia AND Estatus = 1

	-- Ingresos

	UNION

	-- Pagos
	SELECT
		CASE tfp.TipoFormaPagoID
			WHEN @FpEfectivoID THEN 2  -- Efectivo
			WHEN @FpChequeID THEN 3  -- Cheques
			WHEN @FpTarjetaID THEN 4  -- Tarjetas
			WHEN @FpTransferenciaID THEN 5  -- Transferencias
			WHEN @FpNotaDeCreditoID THEN 9  -- Notas de crédito
			-- WHEN @FpNotaDeCreditoFiscalID THEN 10 -- Notas de crédito fiscales
		END AS Orden
		, 'Ingresos' AS Categoria
		, CASE WHEN tfp.TipoFormaPagoID = @FpNotaDeCreditoID
			THEN 'Vales usados' ELSE tfp.NombreTipoFormaPago END AS Concepto
		, i.Tickets
		, i.Facturas
		, (i.Tickets + i.Facturas) AS Total
		, i.Pendientes
	FROM
		TipoFormaPago tfp
		LEFT JOIN _Ingresos i ON i.TipoFormaPagoID = tfp.TipoFormaPagoID
	WHERE tfp.Estatus = 1 AND tfp.TipoFormaPagoID NOT IN (@FpNoIdentificadoID, @FpNotaDeCreditoFiscalID)
	UNION
	SELECT
		6 AS Orden
		, 'Ingresos' AS Categoria
		, 'Cobranza' AS Concepto
		, SUM(TicketsCre) AS Tickets
		, SUM(FacturasCre) AS Facturas
		, SUM(TicketsCre + FacturasCre) AS Total
		, SUM(PendientesCre) AS Pendientes
	FROM _Ingresos
	WHERE TipoFormaPagoID NOT IN (@FpNotaDeCreditoID, @FpNotaDeCreditoFiscalID)

	UNION

	-- Ventas a Crédito
	SELECT
		10 AS Orden
		, 'Ingresos' AS Categoria
		, 'Crédito' AS Concepto
		-- , SUM(CASE WHEN vfd.VentaFacturaID IS NULL THEN ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) ELSE 0.00 END) AS Tickets
		-- , SUM(CASE WHEN vfd.VentaFacturaID IS NOT NULL THEN ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) ELSE 0.00 END) AS Facturas
		-- , SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Total
		, SUM(CASE WHEN vfd.VentaFacturaID IS NULL THEN vdc.Importe ELSE 0.00 END) AS Tickets
		, SUM(CASE WHEN vfd.VentaFacturaID IS NOT NULL THEN vdc.Importe ELSE 0.00 END) AS Facturas
		, SUM(vdc.Importe) AS Total
		-- Visto Bueno
		, SUM(CASE WHEN cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) AS Pendientes
	FROM
		Venta v
		-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN (
			SELECT
				VentaID
				, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
			FROM VentaDetalle
			WHERE Estatus = 1
			GROUP BY VentaID
		) vdc ON vdc.VentaID = v.VentaID
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND Primera = 1 AND vfd.Estatus = 1
		
		LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatVentasCredito AND cvb.TablaRegistroID = v.VentaID
			AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
	WHERE
		v.Estatus = 1
		AND v.SucursalID = @SucursalID
		AND (v.Fecha >= @Dia AND v.Fecha < @DiaSig)
		AND v.ACredito = 1

	UNION

	-- Refuerzos
	SELECT
		8 AS Orden
		, 'Ingresos' AS Categoria
		, 'Refuerzo' AS Concepto
		, 0.00 AS Tickets
		, 0.00 AS Facturas
		, SUM(ci.Importe) AS Total
		-- Visto Bueno
		, SUM(CASE WHEN cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) 
			+ SUM(CASE WHEN a.Autorizado = 1 THEN 0 ELSE 1 END) AS Pendientes
	FROM
		CajaIngreso ci
		
		LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatIngresos AND cvb.TablaRegistroID = ci.CajaIngresoID
			AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = @AutRefuerzoID AND a.TablaRegistroID = ci.CajaIngresoID
			AND a.Estatus = 1
	WHERE
		ci.Estatus = 1
		AND ci.SucursalID = @SucursalID
		AND (ci.Fecha >= @Dia AND ci.Fecha < @DiaSig)
		AND ci.CajaTipoIngresoID = 1
	-- Ingresos Caja
	UNION
	SELECT
		7 AS Orden
		, 'Ingresos' AS Categoria
		, 'Otros Ingresos' AS Concepto
		, 0.00 AS Tickets
		, 0.00 AS Facturas
		, SUM(ci.Importe) AS Total
		-- Visto Bueno
		, SUM(CASE WHEN cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) 
			+ SUM(CASE WHEN a.Autorizado = 1 THEN 0 ELSE 1 END) AS Pendientes
	FROM
		CajaIngreso ci
		
		LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatIngresos AND cvb.TablaRegistroID = ci.CajaIngresoID
			AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = @AutIngresoID AND a.TablaRegistroID = ci.CajaIngresoID
			AND a.Estatus = 1
	WHERE
		ci.Estatus = 1
		AND ci.SucursalID = @SucursalID
		AND (ci.Fecha >= @Dia AND ci.Fecha < @DiaSig)
		AND ci.CajaTipoIngresoID != 1


	-- Egresos

	UNION

	-- Devoluciones
	SELECT
		11 AS Orden
		, 'Egresos' AS Categoria
		, 'Dev./Canc. del día' AS Concepto
		, TicketsDia AS Tickets
		, FacturasDia AS Facturas
		, (TicketsDia + FacturasDia) AS Total
		, PendientesDia
	FROM _Devoluciones
	UNION
	SELECT
		12 AS Orden
		, 'Egresos' AS Categoria
		, 'Dev./Canc. días ant.' AS Concepto
		, TicketsAnt AS Tickets
		, FacturasAnt AS Facturas
		, (TicketsAnt + FacturasAnt) AS Total
		, PendientesAnt
	FROM _Devoluciones

	UNION

	-- Garantias
	SELECT
		13 AS Orden
		, 'Egresos' AS Categoria
		, 'Garantías del día' AS Concepto
		, TicketsDia AS Tickets
		, FacturasDia AS Facturas
		, (TicketsDia + FacturasDia) AS Total
		, PendientesDia
	FROM _Garantias
	UNION
	SELECT
		14 AS Orden
		, 'Egresos' AS Categoria
		, 'Garantías días ant.' AS Concepto
		, TicketsAnt AS Tickets
		, FacturasAnt AS Facturas
		, (TicketsAnt + FacturasAnt) AS Total
		, PendientesAnt
	FROM _Garantias

	UNION

	-- Resguardos
	SELECT
		15 AS Orden
		, 'Egresos' AS Categoria
		, 'Resguardo' AS Concepto
		, 0.00 AS Tickets
		, 0.00 AS Facturas
		, SUM(ce.Importe) AS Total
		-- Visto Bueno
		, SUM(CASE WHEN cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) 
			+ SUM(CASE WHEN a.Autorizado = 1 THEN 0 ELSE 1 END) AS Pendientes
	FROM
		CajaEgreso ce

		LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatEgresos AND cvb.TablaRegistroID = ce.CajaEgresoID
			AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = @AutResguardoID AND a.TablaRegistroID = ce.CajaEgresoID
	WHERE
		ce.Estatus = 1
		AND ce.SucursalID = @SucursalID
		AND (ce.Fecha >= @Dia AND ce.Fecha < @DiaSig)
		AND ce.CajaTipoEgresoID = 1
	-- Egresos Caja
	UNION
	SELECT
		16 AS Orden
		, 'Egresos' AS Categoria
		, 'Gastos' AS Concepto
		, 0.00 AS Tickets
		, 0.00 AS Facturas
		, SUM(ce.Importe) AS Total
		-- Visto Bueno
		, SUM(CASE WHEN cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) 
			+ SUM(CASE WHEN a.Autorizado = 1 THEN 0 ELSE 1 END) AS Pendientes
	FROM
		CajaEgreso ce
		
		LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatEgresos AND cvb.TablaRegistroID = ce.CajaEgresoID
			AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		LEFT JOIN Autorizacion a ON a.AutorizacionProcesoID = @AutEgresoID AND a.TablaRegistroID = ce.CajaEgresoID
	WHERE
		ce.Estatus = 1
		AND ce.SucursalID = @SucursalID
		AND (ce.Fecha >= @Dia AND ce.Fecha < @DiaSig)
		AND ce.CajaTipoEgresoID != 1

	UNION

	-- Notas de crédito negativas
	SELECT
		17 AS Orden
		, 'Egresos' AS Categoria
		, 'Vales creados' AS Concepto
		, (SUM(CASE WHEN vfd.VentaFacturaID IS NULL THEN vpd.Importe ELSE 0.00 END) * -1) AS Tickets
		, (SUM(CASE WHEN vfd.VentaFacturaID IS NOT NULL THEN vpd.Importe ELSE 0.00 END) * -1) AS Facturas
		, (SUM(vpd.Importe) * -1) AS Total
		
		-- Visto Bueno
		, SUM(CASE WHEN cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) AS Pendientes
	FROM
		VentaPago vp
		INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID	AND vpd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND Primera = 1 AND vfd.Estatus = 1
		
		LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatVentas AND cvb.TablaRegistroID = vp.VentaPagoID
			AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
	WHERE
		vp.Estatus = 1
		AND vp.SucursalID = @SucursalID
		AND (vp.Fecha >= @Dia AND vp.Fecha < @DiaSig)
		AND vpd.TipoFormaPagoID = @FpNotaDeCreditoID AND vpd.Importe < 0
	
	
	UNION
	
	-- Notas de crédito fiscales
	SELECT
		18 AS Orden
		, 'Egresos' AS Categoria
		, 'Notas de crédito fiscales' AS Concepto
		, 0.0 AS Tickets
		-- , SUM(vfd.Subtotal + vfd.Iva) AS Facturas
		-- , SUM(vfd.Subtotal + vfd.Iva) AS Total
		, (
			ISNULL((SELECT SUM(Subtotal + Iva) FROM VentaFacturaDevolucion WHERE Estatus = 1
				AND (Fecha >= @Dia AND Fecha < @DiaSig)), 0.0)
			+ ISNULL((SELECT SUM(Subtotal + Iva) FROM NotaDeCreditoFiscal WHERE (Fecha >= @Dia AND Fecha < @DiaSig)), 0.0)
		) AS Facturas
		, (
			ISNULL((SELECT SUM(Subtotal + Iva) FROM VentaFacturaDevolucion WHERE Estatus = 1
				AND (Fecha >= @Dia AND Fecha < @DiaSig)), 0.0)
			+ ISNULL((SELECT SUM(Subtotal + Iva) FROM NotaDeCreditoFiscal WHERE (Fecha >= @Dia AND Fecha < @DiaSig)), 0.0)
		) AS Total
		, 0 AS Pendientes
	/*
	FROM VentaFacturaDevolucion vfd
	WHERE
		vfd.Estatus = 1
		AND (vfd.Fecha >= @Dia AND vfd.Fecha < @DiaSig)
	UNION
	SELECT
		18 AS Orden
		, 'Egresos' AS Categoria
		, 'Notas de crédito fiscales' AS Concepto
		, 0.0 AS Tickets
		, SUM(ncf.Subtotal + ncf.Iva) AS Facturas
		, SUM(ncf.Subtotal + ncf.Iva) AS Total
		, 0 AS Pendientes
	FROM NotaDeCreditoFiscal ncf
	WHERE (ncf.Fecha >= @Dia AND ncf.Fecha < @DiaSig)
	*/
END
GO
