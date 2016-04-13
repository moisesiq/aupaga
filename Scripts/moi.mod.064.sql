/* Script con modificaciones a la base de datos de Theos. Archivo 064
 * Creado: 2016/03/11
 * Subido: 2016/03/21
 */

DECLARE @ScriptID INT = 64
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creaci�n y modificaci�n de tablas
***************************************************************************** */

UPDATE Cliente SET TipoClienteID = 3

-- DROP TABLE VentaCotizacion
CREATE TABLE VentaCotizacion (
	VentaCotizacionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, Fecha DATETIME NOT NULL
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, ClienteID INT NOT NULL FOREIGN KEY REFERENCES Cliente(ClienteID)
	, VendedorID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
)

-- DROP TABLE VentaCotizacionDetalle
CREATE TABLE VentaCotizacionDetalle (
	VentaCotizacionDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, VentaCotizacionID INT NOT NULL FOREIGN KEY REFERENCES VentaCotizacion(VentaCotizacionID)
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, Cantidad DECIMAL(8, 2) NOT NULL
	, PrecioUnitario DECIMAL(12, 2) NOT NULL
	, Iva DECIMAL(12, 2) NOT NULL
)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

CREATE VIEW VentasCotizacionesDetalleAvanzadoView AS
	SELECT
		vcd.VentaCotizacionDetalleID
		, vc.VentaCotizacionID
		, vc.Fecha
		, vc.SucursalID
		, s.NombreSucursal AS Sucursal
		, vc.VendedorID
		, u.NombreUsuario AS Vendedor
		, vc.ClienteID
		, c.Nombre AS Cliente
		, vcd.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, vcd.Cantidad
		, vcd.PrecioUnitario
		, vcd.Iva
		, ((vcd.PrecioUnitario + vcd.Iva) * vcd.Cantidad) AS Importe
		, p.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
	FROM
		VentaCotizacionDetalle vcd
		LEFT JOIN VentaCotizacion vc ON vc.VentaCotizacionID = vcd.VentaCotizacionID
		LEFT JOIN Sucursal s ON s.SucursalID = vc.SucursalID AND s.Estatus = 1
		LEFT JOIN Cliente c ON c.ClienteID = vc.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = vc.VendedorID AND u.Estatus = 1
		LEFT JOIN Parte p ON p.ParteID = vcd.ParteID AND p.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
GO

CREATE VIEW ClientesEventosCalendarioView AS
	SELECT
		cec.ClienteEventoCalendarioID
		, cec.ClienteID
		, c.Nombre AS Cliente
		, cec.Fecha
		, cec.Evento
		, cec.Revisado
	FROM
		ClienteEventoCalendario cec
		LEFT JOIN Cliente c ON c.ClienteID = cec.ClienteID AND c.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauCajaDetalleCorte] (
	@OpcionID INT
	, @SucursalID INT
	, @Dia DATE
	, @DevDiasAnt BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @OpcionID INT = 3
	DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = '2016-03-11'
	DECLARE @DevDiasAnt BIT = 1
	*/

	/* Posible fallo:
		- Al tener una venta pagada y realizar una devoluci�n parcial de productos
		- Al tener una venta a cr�dito que ya tiene pagos pero no se ha completado, y luego se cancela
	*/

	-- Definici�n de variables tipo constante
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
	-- Estatus gen�rico
	DECLARE @EgCompletado INT = 3
	-- Acciones de garant�as
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
				, CASE WHEN
					c9.Cotizacion9500ID IS NULL THEN
						CASE WHEN vpc.ImporteVale > 0 THEN
							('V$ ' + CONVERT(NVARCHAR(16), CONVERT(DECIMAL(12, 2), vpc.ImporteVale))) ELSE '' END
					ELSE
						CASE WHEN vpc.Importe > 0 THEN '9500Anti' ELSE '9500Canc' END
					END AS Caracteristica
				, ISNULL(vpc.Importe, 0.00) AS Importe
				, cvb.Fecha AS FechaVistoBueno
				, CASE WHEN vfd.VentaFacturaID IS NULL THEN 0 ELSE 1 END AS Facturada
				, CASE WHEN vd.VentaDevolucionID IS NULL THEN 0 ELSE 1 END AS Devolucion
				-- Se marcan para sacar las devoluciones y garant�as de d�as anteriores
				, CASE WHEN ((vd.VentaDevolucionID IS NOT NULL AND v.Fecha < @Dia)
					OR (vg.VentaGarantiaID IS NOT NULL AND v.Fecha < @Dia)) THEN 1 ELSE 0 END AS NoContar
				, v.FolioIni
				, cvbfa.Fecha AS FechaVistoBuenoFA
				, @CatVentas AS CatTabla
				, 2 AS Orden
			FROM
				VentaPago vp
				LEFT JOIN (
					SELECT
						VentaPagoID
						, SUM(Importe) AS Importe
						, SUM(CASE WHEN TipoFormaPagoID = @FpNotaDeCreditoID THEN Importe ELSE 0.0 END) AS ImporteVale
					FROM VentaPagoDetalle
					WHERE Estatus = 1
					GROUP BY VentaPagoID
				) vpc ON vpc.VentaPagoID = vp.VentaPagoID
				INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
				LEFT JOIN Usuario u ON u.UsuarioID = v.RealizoUsuarioID AND u.Estatus = 1

				LEFT JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
				LEFT JOIN VentaDevolucion vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
				LEFT JOIN VentaGarantia vg ON vg.VentaID = v.VentaID AND vg.Estatus = 1
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
			
			-- Ventas a Cr�dito
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
				, v.FolioIni-- AS FolioIni dmod
				, NULL AS FechaVistoBuenoAF
				, @CatVentasCredito AS CatTabla
				, 3 AS Orden
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
		-- Se agregan las ventas de d�as anteriores que fueron facturadas
		UNION
		SELECT
			vf.VentaFacturaID AS RegistroID
			, (vf.Serie + vf.Folio) AS Folio
			, c.Nombre AS Cliente
			, '' AS FormaDePago
			, u.NombreUsuario AS Usuario
			, ('$' + CONVERT(NVARCHAR(16), CONVERT(DECIMAL(12, 2), vft.ImporteDiasAnt))) AS Caracteristica
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
					, MIN(v.Fecha) AS FechaPrimeraVenta
					-- Se agrega el importe correspondiente a las ventas de d�as anteriores
					, SUM(CASE WHEN v.Fecha < @Dia THEN ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) ELSE 0.0 END)
						AS ImporteDiasAnt
				FROM
					VentaFacturaDetalle vfd
					LEFT JOIN VentaDetalle vd ON vd.VentaID = vfd.VentaID AND vd.Estatus = 1
					LEFT JOIN Venta v ON v.VentaID = vfd.VentaID AND v.Estatus = 1
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
			-- Se validan las fechas de las ventas involucradas en la factura
			AND vft.FechaPrimeraVenta < @Dia

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
				'CR�DITO'
			  ELSE
				CASE vd.TipoFormaPagoID
					WHEN @FpEfectivoID THEN 'EFECTIVO'
					WHEN @FpChequeID THEN 'CHEQUE'
					WHEN @FpTarjetaID THEN 'TARJETA'
					WHEN @FpNotaDeCreditoID THEN ('-VA (' + ISNULL(CONVERT(VARCHAR(4), vpd.NotaDeCreditoID), '') + ')')
				END
			  END AS Salida
			, (u.NombreUsuario + ' > ' + ISNULL(ua.NombreUsuario, '')) AS Autorizacion
			, vddc.Importe
			, cvb.Fecha AS FechaVistoBueno
		FROM
			VentaDevolucion vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoDetalleID = vd.VentaPagoDetalleID AND vpd.Estatus = 1
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

	-- Garant�as
	IF @OpcionID = @OpGarantias BEGIN
		SELECT
			vg.VentaGarantiaID
			, v.Folio
			, c.Nombre AS Cliente
			, CASE WHEN v.VentaEstatusID = @VeCanceladaSinPago AND v.ACredito = 1 THEN
				'CR�DITO'
			  ELSE
				CASE vg.AccionID
					WHEN @AgEfectivo THEN       'EFECTIVO'
					WHEN @AgNoProcede THEN      'NO PROCEDE'
					WHEN @AgArticuloNuevo THEN ('AN (' + ISNULL(CONVERT(VARCHAR(4), vpd.NotaDeCreditoID), '') + ')')
					WHEN @AgNotaDeCredito THEN ('VA (' + ISNULL(CONVERT(VARCHAR(4), vpd.NotaDeCreditoID), '') + ')')
					WHEN @AgCheque THEN         'CHEQUE'
					WHEN @AgTarjeta THEN        'TARJETA'
					WHEN @AgRevision THEN       'PROVEEDOR'
				END
			  END AS Salida
			, (u.NombreUsuario + ' > ' + ISNULL(ua.NombreUsuario, '')) AS Autorizacion
			, (vg.PrecioUnitario + vg.Iva) AS Importe
			, cvb.Fecha AS FechaVistoBueno
		FROM
			VentaGarantia vg
			INNER JOIN Venta v ON v.VentaID = vg.VentaID AND v.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoDetalleID = vg.VentaPagoDetalleID AND vpd.Estatus = 1
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
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatGarantias
				AND cvb.TablaRegistroID = vg.VentaGarantiaID AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			vg.Estatus = 1
			-- AND vg.EstatusGenericoID = @EgCompletado
			AND vg.SucursalID = @SucursalID
			AND (
				(vg.Fecha >= @Dia AND vg.Fecha < @DiaSig)
				OR (vg.FechaCompletado >= @Dia AND vg.FechaCompletado < @DiaSig)
			)
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

ALTER PROCEDURE [dbo].[pauCajaCorte] (
	@SucursalID INT
	, @Dia DATE
) AS BEGIN
	SET NOCOUNT ON

	/* Posible fallo:
		- Cancelaciones/Devoluciones de cr�dito, con o sin pagos
	*/

	/*
	DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = GETDATE()
	-- EXEC pauCajaCorte 1, '2014-11-15'
	*/

	-- Definici�n de variables tipo constante
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
	-- Estatus gen�rico
	DECLARE @EgCompletado INT = 3
	-- Acciones de garant�as
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
			-- Se separan los pagos de ventas a cr�dito (Cobranza), de las normales
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
				-- Si el importe es negativo, se verifica si la devoluci�n es de d�as anteriores, en cuyo caso, no se cuenta
				-- pues esas ya restan abajo en el corte
				-- Se quitan tambi�n las garant�as de d�as anteriores
				AND (vpd.Importe >= 0 OR (vdc.VentaID IS NULL OR v.Fecha >= @Dia))
				AND (vpd.Importe >= 0 OR (vg.VentaID IS NULL OR v.Fecha >= @Dia))
				-- Si es una nota de cr�dito negativa, no se cuenta aqu�
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

	-- Para las Garant�as (Egresos)
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
			AND (
				(vg.Fecha >= @Dia AND vg.Fecha < @DiaSig)
				OR (vg.FechaCompletado >= @Dia AND vg.FechaCompletado < @DiaSig)
			)
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
			WHEN @FpNotaDeCreditoID THEN 9  -- Notas de cr�dito
			-- WHEN @FpNotaDeCreditoFiscalID THEN 10 -- Notas de cr�dito fiscales
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

	-- Ventas a Cr�dito
	SELECT
		10 AS Orden
		, 'Ingresos' AS Categoria
		, 'Cr�dito' AS Concepto
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
		, 'Dev./Canc. del d�a' AS Concepto
		, TicketsDia AS Tickets
		, FacturasDia AS Facturas
		, (TicketsDia + FacturasDia) AS Total
		, PendientesDia
	FROM _Devoluciones
	UNION
	SELECT
		12 AS Orden
		, 'Egresos' AS Categoria
		, 'Dev./Canc. d�as ant.' AS Concepto
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
		, 'Garant�as del d�a' AS Concepto
		, TicketsDia AS Tickets
		, FacturasDia AS Facturas
		, (TicketsDia + FacturasDia) AS Total
		, PendientesDia
	FROM _Garantias
	UNION
	SELECT
		14 AS Orden
		, 'Egresos' AS Categoria
		, 'Garant�as d�as ant.' AS Concepto
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

	-- Notas de cr�dito negativas
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
	
	-- Notas de cr�dito fiscales
	SELECT
		18 AS Orden
		, 'Egresos' AS Categoria
		, 'Notas de cr�dito fiscales' AS Concepto
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
		, 'Notas de cr�dito fiscales' AS Concepto
		, 0.0 AS Tickets
		, SUM(ncf.Subtotal + ncf.Iva) AS Facturas
		, SUM(ncf.Subtotal + ncf.Iva) AS Total
		, 0 AS Pendientes
	FROM NotaDeCreditoFiscal ncf
	WHERE (ncf.Fecha >= @Dia AND ncf.Fecha < @DiaSig)
	*/
END
GO