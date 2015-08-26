/* Script con modificaciones para el módulo de ventas. Archivo 67
 * Creado: 2014/11/14
 * Subido: 2014/11/21
 */


----------------------------------------------------------------------------------- Código de André

----------------------------------------------------------------------------------- Código de André


/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE TABLE NotaDeCreditoFiscal (
		NotaDeCreditoFiscalID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Fecha DATETIME NOT NULL
		, ClienteID INT NOT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, FolioFiscal NVARCHAR(36) NULL
		, Serie NVARCHAR(8) NULL
		, Folio NVARCHAR(8) NULL
		, Subtotal DECIMAL(12, 2) NOT NULL
		, Iva DECIMAL(12, 2) NOT NULL
		, RealizoUsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
	)

	UPDATE TipoFormaPago SET NombreTipoFormaPago = 'VALE' WHERE TipoFormaPagoID = 6
	INSERT INTO TipoFormaPago (NombreTipoFormaPago, UsuarioID, FechaRegistro) VALUES
		('NOTA DE CRÉDITO FISCAL', 1, GETDATE())

	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Ventas.NotasDeCreditoFiscales.Agregar', 'No tienes permisos para agregar Notas de Crédito Fiscales.')
		, ('Autorizaciones.Ventas.NotasDeCreditoFiscales.Agregar', 'No tienes permisos para autorizar el agregar Notas de Crédito Fiscales.')
	INSERT INTO AutorizacionProceso (Descripcion, UsuarioID, Permiso) VALUES
		('NOTA DE CRÉDITO FISCAL', 1, 'Autorizaciones.Ventas.NotasDeCreditoFiscales.Agregar')

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

CREATE VIEW NotasDeCreditoFiscalesView AS
	SELECT
		ncf.NotaDeCreditoFiscalID
		, ncf.Fecha
		, ncf.ClienteID
		, c.Nombre AS Cliente
		, ncf.SucursalID
		, s.NombreSucursal AS Sucursal
		, ncf.Serie
		, ncf.Folio
		, ncf.Subtotal
		, ncf.Iva
		, ISNULL((ncf.Subtotal + ncf.Iva), 0.0) AS Total
		, ncf.RealizoUsuarioID
		, u.NombreUsuario AS Usuario		
	FROM
		NotaDeCreditoFiscal ncf
		LEFT JOIN Cliente c ON c.ClienteID = ncf.ClienteID AND c.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = ncf.SucursalID AND s.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = ncf.RealizoUsuarioID AND u.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

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

	-- Para las Garantías (Egresos)
	, _Garantias AS (
		SELECT
			SUM(
				CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND vfd.VentaFacturaID IS NULL) THEN 
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vgdc.Importe THEN vgdc.Importe ELSE vpc.Pagado END)
					ELSE
						vgdc.Importe
					END)
				ELSE
					0.00
				END
			) AS TicketsDia
			, SUM(
				CASE WHEN ((v.Fecha >= @Dia AND v.Fecha < @DiaSig) AND vfd.VentaFacturaID IS NOT NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vgdc.Importe THEN vgdc.Importe ELSE vpc.Pagado END)
					ELSE
						vgdc.Importe
					END)
				ELSE
					0.00
				END
			) AS FacturasDia
			, SUM(
				CASE WHEN (v.Fecha < @Dia AND vfd.VentaFacturaID IS NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vgdc.Importe THEN vgdc.Importe ELSE vpc.Pagado END)
					ELSE
						vgdc.Importe
					END)
				ELSE
					0.00
				END
			) AS TicketsAnt
			, SUM(
				CASE WHEN (v.Fecha < @Dia AND vfd.VentaFacturaID IS NOT NULL) THEN
					(CASE WHEN v.ACredito = 1 THEN
						(CASE WHEN vpc.Pagado > vgdc.Importe THEN vgdc.Importe ELSE vpc.Pagado END)
					ELSE
						vgdc.Importe
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
			LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
			LEFT JOIN (
				SELECT
					VentaGarantiaID
					, SUM((PrecioUnitario + Iva) * Cantidad) AS Importe
				FROM VentaGarantiaDetalle
				WHERE Estatus = 1
				GROUP BY VentaGarantiaID
			) vgdc ON vgdc.VentaGarantiaID = vg.VentaGarantiaID
			
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
			AND vg.EstatusGenericoID = @EgCompletado
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
			WHEN @FpNotaDeCreditoFiscalID THEN 10 -- Notas de crédito fiscales
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

ALTER PROCEDURE [dbo].[pauCuadroDeControlGeneral] (
	@SucursalID INT = NULL
	, @Pagadas BIT
	, @Cobradas BIT
	, @Solo9500 BIT
	, @OmitirDomingo BIT
	-- , @CostoConDescuento BIT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* EXEC pauCuadroDeControlGeneral 1, 1, 1, 0, 0, 1, '2014-01-01', '2014-08-01'
	DECLARE @SucursalID INT = 1
	DECLARE @Pagadas BIT = 1
	DECLARE @Cobradas BIT = 1
	DECLARE @Anio INT = 2014
	DECLARE @Desde DATE = '2014-06-01'
	DECLARE @Hasta DATE = '2014-06-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstCanceladaSinPagoID INT = 5
	
	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	DECLARE @DesdeAnt DATE = DATEADD(YEAR, -1, @Desde)
	DECLARE @HastaAnt DATE = DATEADD(YEAR, -1, @Hasta)
	
	-- Consulta
	SELECT
		vpc.VentaID
		, v.Folio
		, ISNULL(vpc.Fecha, '') AS Fecha
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.RealizoUsuarioID AS VendedorID
		, u.NombreUsuario AS Vendedor
		, l.LineaID
		, l.NombreLinea AS Linea
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN
			((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0.0 END) AS Actual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN
			((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) ELSE 0.0	END) AS Anterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN
			((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) ELSE 0.0 END) AS UtilDescActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN
			((vd.PrecioUnitario - vd.CostoConDescuento) * vd.Cantidad) ELSE 0.0 END) AS UtilDescAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN (vd.PrecioUnitario + vd.Iva)
			ELSE 0.0 END) AS PrecioActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN (vd.PrecioUnitario + vd.Iva)
			ELSE 0.0 END) AS PrecioAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN vd.Costo ELSE 0.0 END) AS CostoActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN vd.Costo ELSE 0.0 END) AS CostoAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN vd.CostoConDescuento ELSE 0.0 END) AS CostoDescActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN vd.CostoConDescuento ELSE 0.0 END) AS CostoDescAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta) THEN 1 ELSE 0 END) AS VentasActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt) THEN 1 ELSE 0 END) AS VentasAnterior
	FROM
		-- VentaPago vp
		-- INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		(
			SELECT
				VentaID
				, MAX(vp.Fecha) AS Fecha
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND (
					(vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
					OR (vp.Fecha >= @DesdeAnt AND vp.Fecha < @HastaAnt)
				)
				AND vpd.Importe > 0
			GROUP BY VentaID
			UNION ALL
			SELECT
				VentaID
				, Fecha
			FROM Venta v
			WHERE
				@Cobradas = 1
				AND v.Estatus = 1
				AND v.VentaEstatusID = @EstCobradaID
				AND (
					(v.Fecha >= @Desde AND v.Fecha < @Hasta)
					OR (v.Fecha >= @DesdeAnt AND v.Fecha < @HastaAnt)
				)
		) vpc
		INNER JOIN Venta v ON v.VentaID = vpc.VentaID AND v.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		-- vp.Estatus = 1
		-- AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		-- AND vpd.Importe > 0
		p.AplicaComision = 1
		
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
			OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		/* AND (
			(vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
			OR (vp.Fecha >= @DesdeAnt AND vp.Fecha < @HastaAnt)
		)
		*/
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		vpc.VentaID
		, v.Folio
		, vpc.Fecha
		, v.SucursalID
		, s.NombreSucursal
		, v.ClienteID
		, c.Nombre
		, v.RealizoUsuarioID
		, u.NombreUsuario
		, l.LineaID
		, l.NombreLinea
		, pv.ProveedorID
		, pv.NombreProveedor

	-- Canceladas
	UNION ALL
	SELECT
		vd.VentaID
		, v.Folio
		, vd.Fecha
		, v.SucursalID
		, s.NombreSucursal AS Sucursal
		, v.ClienteID
		, c.Nombre AS Cliente
		, v.RealizoUsuarioID AS VendedorID
		, u.NombreUsuario AS Vendedor
		, l.LineaID
		, l.NombreLinea AS Linea
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN
			((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) ELSE 0.0 END) * -1) AS Actual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN
			((vdd.PrecioUnitario - vdd.Costo) * vdd.Cantidad) ELSE 0.0 END) * -1) AS Anterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN
			((vdd.PrecioUnitario - vdd.CostoConDescuento) * vdd.Cantidad) ELSE 0.0 END) * -1) AS UtilDescActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN
			((vdd.PrecioUnitario - vdd.CostoConDescuento) * vdd.Cantidad) ELSE 0.0 END) * -1) AS UtilDescAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN (vdd.PrecioUnitario + vdd.Iva)
			ELSE 0.0 END) * -1) AS PrecioActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN (vdd.PrecioUnitario + vdd.Iva)
			ELSE 0.0 END) * -1) AS PrecioAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN vdd.Costo ELSE 0.0 END) * -1) AS CostoActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN vdd.Costo ELSE 0.0 END) * -1) AS CostoAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN vdd.CostoConDescuento ELSE 0.0 END) * -1) AS CostoDescActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN vdd.CostoConDescuento ELSE 0.0 END) * -1) AS CostoDescAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) THEN 1 ELSE 0 END) * -1) AS VentasActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt) THEN 1 ELSE 0 END) * -1) AS VentasAnterior
	FROM
		VentaDevolucion vd
		LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1
		INNER JOIN Parte p ON p.ParteID = vdd.ParteID AND p.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		vd.Estatus = 1
		-- AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
		AND p.AplicaComision = 1
		AND v.Fecha < @Desde
		
		AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		AND (
			(@Pagadas = 1 AND (v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)))
			-- Se quita que se consideren las canceladas sin pago, porque restan cuando no deberían
			OR (@Cobradas = 1 AND (v.VentaEstatusID IN (@EstCobradaID)))
		)
		AND (@Solo9500 = 0 OR c9.Cotizacion9500ID IS NOT NULL)
		AND (
			(vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			OR (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
		)
		-- Se quitan las ventas de los domingos, si aplica
		AND (@OmitirDomingo = 0 OR DATEPART(DW, v.Fecha) != 1)
	GROUP BY
		vd.VentaID
		, v.Folio
		, vd.Fecha
		, v.SucursalID
		, s.NombreSucursal
		, v.ClienteID
		, c.Nombre
		, v.RealizoUsuarioID
		, u.NombreUsuario
		, l.LineaID
		, l.NombreLinea
		, pv.ProveedorID
		, pv.NombreProveedor

	/* ORDER BY
		DATEPART(DAYOFYEAR, vp.Fecha)
		, DATEPART(HOUR, vp.Fecha)
	*/

END
GO
