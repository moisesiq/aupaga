/* Script con modificaciones para el módulo de ventas. Archivo 31
 * Creado: 2014/02/14
 * Subido: 2014/03/19
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	UPDATE EstatusGenerico SET Descripcion = UPPER(Descripcion)
	INSERT INTO EstatusGenerico (Descripcion, UsuarioID) VALUES ('POR COMPLETAR', 1)
	
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
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO
ALTER VIEW Lista9500View AS
	SELECT
		c9.Cotizacion9500ID AS Folio
		, c9.Fecha
		, pv.NombreProveedor
		, (l.NombreLinea + ' - ' + mp.NombreMarcaParte) AS LineaMarca
		, p.NumeroParte
		, p.NombreParte
		, c9d.Costo
		, c9d.PrecioAlCliente
		, c9.Anticipo
		, c.Nombre AS Cliente
		, s.NombreSucursal AS Sucursal
		, v.NombrePersona AS Vendedor
	FROM
		Cotizacion9500 c9
		LEFT JOIN (
			SELECT *
			FROM (
				SELECT
					Cotizacion9500ID
					, ProveedorID
					, MarcaParteID
					, LineaID
					, ParteID
					, Costo
					, PrecioAlCliente
					, ROW_NUMBER() OVER (PARTITION BY Cotizacion9500ID ORDER BY Cotizacion9500DetalleID) AS Numero
				FROM Cotizacion9500Detalle c9d
				WHERE Estatus = 1
			) c9d
			WHERE Numero = 1
		) c9d ON c9d.Cotizacion9500ID = c9.Cotizacion9500ID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = c9d.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = c9d.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = c9d.LineaID AND l.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = c9d.ParteID AND p.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = c9.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario v ON v.UsuarioID = c9.RealizoUsuarioID AND v.Estatus = 1
	WHERE
		c9.Estatus = 1
		AND c9.EstatusGenericoID = 2
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO
-- EXEC pauCajaCorte 1, '20140129'
ALTER PROCEDURE pauCajaCorte (
	@SucursalID INT
	, @Dia DATE
) AS BEGIN
	SET NOCOUNT ON

	/* Posible fallo:
		- Cancelaciones/Devoluciones de crédito, con o sin pagos
	*/

	/* DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = GETDATE()
	*/

	-- Definición de variables tipo constante
	DECLARE @FpEfectivoID INT = 2
	DECLARE @FpChequeID INT = 1
	DECLARE @FpTarjetaID INT = 4
	DECLARE @FpTransferenciaID INT = 3
	DECLARE @FpNotaDeCreditoID INT = 6
	DECLARE @FpNoIdentificadoID INT = 5
	DECLARE @CatVentas VARCHAR(64) = 'Ventas.VentaPago'
	DECLARE @CatVentasCredito VARCHAR(64) = 'Ventas.Venta'
	DECLARE @CatDevoluciones VARCHAR(64) = 'Devoluciones.VentaDevolucion'
	DECLARE @Cat9500 VARCHAR(64) = '9500.Cotizacion9500'
	DECLARE @CatCobranza VARCHAR(64) = 'Cobranza.VentaPago'
	DECLARE @CatEgresos VARCHAR(64) = 'Egresos.CajaEgreso'
	DECLARE @CatIngresos VARCHAR(64) = 'Ingresos.CajaIngreso'
	DECLARE @AutDevolucionID INT = 3
	DECLARE @AutRefuerzoID INT = 8
	DECLARE @AutResguardoID INT = 9
	DECLARE @AutIngresoID INT = 6
	DECLARE @AutEgresoID INT = 5

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
			LEFT JOIN VentaDevolucion vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
			LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID
				-- Si el importe es negativo, se verifica si la devolución es de días anteriores, en cuyo caso, no se cuenta
				-- pues esas ya restan abajo en el corte
				AND (vpd.Importe >= 0 OR (vd.VentaDevolucionID IS NULL OR (v.Fecha >= @Dia)))
				-- Si es una nota de crédito negativa, no se cuenta aquí
				AND (vpd.TipoFormaPagoID != @FpNotaDeCreditoID OR (vpd.TipoFormaPagoID = @FpNotaDeCreditoID AND vpd.Importe > 0))
				AND vpd.Estatus = 1
			LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
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
			LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
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
		END AS Orden
		, 'Ingresos' AS Categoria
		, tfp.NombreTipoFormaPago AS Concepto
		, i.Tickets
		, i.Facturas
		, (i.Tickets + i.Facturas) AS Total
		, i.Pendientes
	FROM
		TipoFormaPago tfp
		LEFT JOIN _Ingresos i ON i.TipoFormaPagoID = tfp.TipoFormaPagoID
	WHERE tfp.Estatus = 1 AND tfp.TipoFormaPagoID != @FpNoIdentificadoID
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
	WHERE TipoFormaPagoID != @FpNotaDeCreditoID

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
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
		
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

	-- Resguardos
	SELECT
		13 AS Orden
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
		14 AS Orden
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
		15 AS Orden
		, 'Egresos' AS Categoria
		, 'Notas de Crédito' AS Concepto
		, (SUM(CASE WHEN vfd.VentaFacturaID IS NULL THEN vpd.Importe ELSE 0.00 END) * -1) AS Tickets
		, (SUM(CASE WHEN vfd.VentaFacturaID IS NOT NULL THEN vpd.Importe ELSE 0.00 END) * -1) AS Facturas
		, (SUM(vpd.Importe) * -1) AS Total
		
		-- Visto Bueno
		, SUM(CASE WHEN cvb.CajaVistoBuenoID IS NULL THEN 1 ELSE 0 END) AS Pendientes
	FROM
		VentaPago vp
		INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID	AND vpd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
		LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
		
		LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatVentas AND cvb.TablaRegistroID = vp.VentaPagoID
			AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
	WHERE
		vp.Estatus = 1
		AND vp.SucursalID = @SucursalID
		AND (vp.Fecha >= @Dia AND vp.Fecha < @DiaSig)
		AND vpd.TipoFormaPagoID = @FpNotaDeCreditoID AND vpd.Importe < 0
END
GO

-- EXEC pauPedidosSugeridos 1
ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@SucursalID NVARCHAR(10)
) 
AS BEGIN
SET NOCOUNT ON

	SELECT * FROM (
		SELECT
			Parte.ParteID
			,CAST(1 AS BIT) AS Sel
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.UnidadEmpaque	
			/* ,Parte.CriterioABC	
			,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) AS MaxMatriz
			,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) AS MaxSuc02
			,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) AS MaxSuc03	
			,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaMatriz
			,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc02
			,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc03		
			,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadMatriz
			,SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadSuc02
			,SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadSuc03		
			,SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) +
			SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) +
			SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS Total	
			,CEILING((SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) +
			SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) +
			SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END)) / 
			CASE WHEN Parte.UnidadEmpaque = 0 THEN 1 ELSE Parte.UnidadEmpaque END) * Parte.UnidadEmpaque AS Pedido	
			*/
			, ParteAbc.AbcDeVentas AS CriterioABC
			, SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxMatriz
			, SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxSuc02
			, SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteMaxMin.Maximo ELSE 0.0 END) AS MaxSuc03	
			, SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaMatriz
			, SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc02
			, SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS ExistenciaSuc03		
			, SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadMatriz
			, SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadSuc02
			, SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS NecesidadSuc03		
			, SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) +
				SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) +
				SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END) AS Total
			, CEILING((SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 1 THEN ParteExistencia.Existencia ELSE 0.0 END) +
				SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 2 THEN ParteExistencia.Existencia ELSE 0.0 END) +
				SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteMaxMin.Maximo ELSE 0.0 END) - SUM(CASE WHEN ParteExistencia.SucursalID = 3 THEN ParteExistencia.Existencia ELSE 0.0 END)) / 
				CASE WHEN Parte.UnidadEmpaque = 0 THEN 1 ELSE Parte.UnidadEmpaque END) * Parte.UnidadEmpaque AS Pedido
			
			,PartePrecio.Costo
			,Parte.ProveedorID
			,Proveedor.NombreProveedor
			,Proveedor.Beneficiario		
		FROM
			Parte
			INNER JOIN ParteExistencia ON ParteExistencia.ParteID = Parte.ParteID
			INNER JOIN PartePrecio ON PartePrecio.ParteID = Parte.ParteID
			INNER JOIN Proveedor ON Proveedor.ProveedorID = Parte.ProveedorID
			LEFT JOIN ParteAbc ON ParteAbc.ParteID = Parte.ParteID
			LEFT JOIN ParteMaxMin ON ParteMaxMin.ParteID = Parte.ParteID
		WHERE
			Parte.Estatus = 1
			AND ParteExistencia.Maximo > 0
			AND ParteExistencia.Existencia <= ParteExistencia.Minimo
			AND Parte.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
			AND ParteExistencia.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
		GROUP BY
			Parte.ParteID
			,Parte.NumeroParte
			,Parte.NombreParte
			,Parte.UnidadEmpaque
			,Parte.ProveedorID
			,Proveedor.NombreProveedor
			,Proveedor.Beneficiario
			,PartePrecio.Costo
			-- ,Parte.CriterioABC
			, ParteAbc.AbcDeVentas
	)Pedidos
	WHERE Pedidos.Pedido > 0

END