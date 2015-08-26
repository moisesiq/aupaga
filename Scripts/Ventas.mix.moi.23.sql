/* Script con modificaciones para el módulo de ventas. Archivo 23
 * Creado: 2014/01/23
 * Subido: 2014/02/05
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Permiso
	INSERT INTO Permiso (NombrePermiso, MensajeDeError, FechaRegistro) VALUES
		('Ventas.Ingresos.Borrar', 'No tienes permisos para borrar Ingresos de Caja.', GETDATE())
		, ('Ventas.Egresos.Borrar', 'No tienes permisos para borrar Egresos de Caja.', GETDATE())

	-- Estatus venta
	IF NOT EXISTS(SELECT 1 FROM VentaEstatus WHERE VentaEstatusID = 5) BEGIN
		INSERT INTO VentaEstatus (Descripcion, UsuarioID) VALUES
			('Cancelada sin pago', 1)
	END

	UPDATE Venta SET VentaEstatusID = 5 WHERE VentaID IN (
		SELECT v.VentaID
		FROM
			Venta v
			LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		WHERE
			v.Estatus = 1
			AND v.VentaEstatusID = 4
		GROUP BY v.VentaID
		HAVING COUNT(vp.VentaPagoID) = 0
	)

	-- Venta
	ALTER TABLE Venta ADD
		FolioIni NVARCHAR(16) NULL
		, Facturada BIT NOT NULL DEFAULT 0

	-- Se borran los registro marcados con Estatus = 0
	DELETE FROM CajaVistoBueno WHERE Estatus = 0
	-- Se borra los contraints de Estatus y Actualizar, para luego borrar los campos
	DECLARE @ConstFechaRegistro NVARCHAR(256) = (SELECT name FROM sys.default_constraints
		WHERE parent_object_id = object_id('CajaVistoBueno') AND name LIKE 'DF__CajaVisto__Fecha__%')
	DECLARE @ConstEstatus NVARCHAR(256) = (SELECT name FROM sys.default_constraints
		WHERE parent_object_id = object_id('CajaVistoBueno') AND name LIKE 'DF__CajaVisto__Estat__%')
	DECLARE @ConstActualizar NVARCHAR(256) = (SELECT name FROM sys.default_constraints
		WHERE parent_object_id = object_id('CajaVistoBueno') AND name LIKE 'DF__CajaVisto__Actua__%')
	DECLARE @Com NVARCHAR(512) = ('ALTER TABLE CajaVistoBueno DROP CONSTRAINT '
		+ @ConstFechaRegistro + ', ' + @ConstEstatus + ', ' + @ConstActualizar)
	EXEC (@Com)
	--
	ALTER TABLE CajaVistoBueno DROP COLUMN FechaRegistro, FechaModificacion, Estatus, Actualizar

	EXEC sp_rename 'CajaVistoBueno.Tabla', 'CatTabla', 'COLUMN'

	-- Devoluciones
	ALTER TABLE VentaDevolucion ADD NotaDeCreditoID INT NULL FOREIGN KEY REFERENCES NotaDeCredito(NotaDeCreditoID)

	-- Configuracion
	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Comisiones.Vendedor.Porcentaje9500', 20, 20, 'Porcentaje de comisión sobre la utilidad que se le da a los vendedores por producto 9500 vendido.')

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

-- CajaIngreso
ALTER TABLE CajaIngreso ADD RealizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
GO
UPDATE CajaIngreso SET RealizoUsuarioID = UsuarioID
ALTER TABLE CajaIngreso ALTER COLUMN RealizoUsuarioID INT NOT NULL

-- CajaEgreso
ALTER TABLE CajaEgreso ADD RealizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
GO
UPDATE CajaEgreso SET RealizoUsuarioID = UsuarioID
ALTER TABLE CajaEgreso ALTER COLUMN RealizoUsuarioID INT NOT NULL

-- Venta
UPDATE Venta SET Facturada = 1
FROM
	Venta v
	INNER JOIN VentaFacturaDetalle vfd ON vfd.VentaID = v.VentaID AND vfd.Estatus = 1
WHERE v.Estatus = 1

GO

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */

-- DROP FUNCTION fnuCadenaFormasDePago
CREATE FUNCTION fnuCadenaFormasDePago(@VentaPagoID INT)
RETURNS VARCHAR(18)
AS BEGIN
	DECLARE @FormasDePago VARCHAR(18)

	;WITH _Formas AS (
		SELECT
			MAX(CASE TipoFormaPagoID WHEN 2 THEN 'EF' ELSE '  ' END) AS Efectivo
			, MAX(CASE TipoFormaPagoID WHEN 1 THEN 'CH' ELSE '  ' END) AS Cheque
			, MAX(CASE TipoFormaPagoID WHEN 4 THEN 'TC' ELSE '  ' END) AS Tarjeta
			, MAX(CASE TipoFormaPagoID WHEN 3 THEN 'TR' ELSE '  ' END) AS Transferencia
			, MAX(CASE TipoFormaPagoID WHEN 6 THEN 'NC' ELSE '' END) AS NotaDeCredito
			, MIN(NotaDeCreditoID) AS NotaDeCreditoID
			, COUNT(NotaDeCreditoID) AS CuentaNotas
		FROM VentaPagoDetalle
		WHERE
			Estatus = 1
			AND VentaPagoID = @VentaPagoID
			-- AND Importe > 0
	)
	SELECT @FormasDePago =
		(Efectivo
		+ '-' + Cheque
		+ '-' + Tarjeta
		+ '-' + Transferencia
		+ '-' + NotaDeCredito
			+ CASE WHEN NotaDeCredito = '' THEN '' ELSE (CONVERT(VARCHAR(4), NotaDeCreditoID)
			+ CASE WHEN CuentaNotas > 1 THEN '+' ELSE '' END) END)
	FROM _Formas

	RETURN @FormasDePago
END
GO

/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */



/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

-- EXEC pauCajaDetalleCorte 1, 1, '20140121', 1
-- DROP PROCEDURE pauCajaDetalleCorte
CREATE PROCEDURE pauCajaDetalleCorte (
	@OpcionID INT
	, @SucursalID INT
	, @Dia DATE
	, @DevDiasAnt BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* Posible fallo:
		- Al tener una venta pagada y realizar una devolución parcial de productos
		- Al tener una venta a crédito que ya tiene pagos pero no se ha completado, y luego se cancela
	*/

	-- Definición de variables tipo constante
	DECLARE @CatVentasID INT = 1
	DECLARE @CatDevolucionesID INT = 2
	DECLARE @Cat9500ID INT = 3
	DECLARE @CatCobranzaID INT = 4
	DECLARE @CatEgresosID INT = 5
	DECLARE @CatIngresosID INT = 6
	DECLARE @CatCambiosID INT = 7
	-- 
	DECLARE @FpEfectivoID INT = 2
	DECLARE @FpChequeID INT = 1
	DECLARE @FpTarjetaID INT = 4
	DECLARE @FpTransferenciaID INT = 3
	DECLARE @FpNotaDeCreditoID INT = 6
	DECLARE @FpNoIdentificadoID INT = 5
	DECLARE @CatVentas VARCHAR(64) = 'Ventas.VentaPago'
	DECLARE @CatVentasCredito VARCHAR(64) = 'Ventas.Venta'
	DECLARE @CatVentasAF VARCHAR(64) = 'VentasAF.VentaPago'
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
	DECLARE @VeCanceladaSinPago INT = 5

	/* DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = '20140121'
	DECLARE @DevDiasAnt BIT = 0
	*/

	DECLARE @DiaSig DATE = DATEADD(d, 1, @Dia)

	-- Ventas
	IF @OpcionID = @CatVentasID BEGIN
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
		WHERE NoContar = 0
		
		-- Se agregan las ventas que primero fueron ventas y luego facturas
		UNION
		SELECT
			RegistroID
			, FolioIni
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
		
		ORDER BY Orden, Facturada DESC, Folio
	END

	-- Devoluciones
	IF @OpcionID = @CatDevolucionesID BEGIN
		SELECT
			vd.VentaDevolucionID
			, v.Folio
			, CASE WHEN vd.EsCancelacion = 1 THEN 'CANC-' ELSE '    -DEV' END AS Tipo
			, c.Nombre AS Cliente
			, CASE WHEN v.VentaEstatusID = @VeCanceladaSinPago AND v.ACredito = 1 THEN
				'CREDITO'
			  ELSE
				CASE vd.TipoFormaPagoID
					WHEN @FpEfectivoID THEN 'EF-  -  -  '
					WHEN @FpChequeID THEN '  -CH-  -  '
					WHEN @FpTarjetaID THEN '  -  -TC-  '
					WHEN @FpNotaDeCreditoID THEN ('  -  -  -NC' + ISNULL(CONVERT(VARCHAR(4), vd.NotaDeCreditoID), ''))
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

	-- 9500
	IF @OpcionID = @Cat9500ID BEGIN
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
	IF @OpcionID = @CatCobranzaID BEGIN
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
	IF @OpcionID = @CatEgresosID BEGIN
		SELECT
			ce.CajaEgresoID AS RegistroID
			, (cte.NombreTipoEgreso + ' - ' + ce.Concepto) AS Concepto
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
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatEgresos AND cvb.TablaRegistroID = ce.CajaEgresoID
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
		WHERE
			ce.Estatus = 1
			AND ce.SucursalID = @SucursalID
			AND (ce.Fecha >= @Dia AND ce.Fecha < @DiaSig)
	END

	-- Ingresos
	IF @OpcionID = @CatIngresosID BEGIN
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
	IF @OpcionID = @CatCambiosID BEGIN
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

-- EXEC pauCajaCorte 1, '20140129'
-- DROP PROCEDURE pauCajaCorte
CREATE PROCEDURE pauCajaCorte (
	@SucursalID INT
	, @Dia DATE
) AS BEGIN
	SET NOCOUNT ON

	/* Posible fallo:
		- Cancelaciones/Devoluciones de crédito, con o sin pagos
	*/

	/* DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = '20140121'
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
			tfp.NombreTipoFormaPago AS Concepto
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
			LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = vpd.TipoFormaPagoID AND tfp.Estatus = 1
			
			LEFT JOIN CajaVistoBueno cvb ON cvb.CatTabla = @CatVentas AND cvb.TablaRegistroID = vp.VentaPagoID
				AND (cvb.Fecha >= @Dia AND cvb.Fecha < @DiaSig)
			LEFT JOIN CajaVistoBueno cvb2 ON cvb2.CatTabla = @CatCobranza AND cvb2.TablaRegistroID = vp.VentaPagoID
				AND (cvb2.Fecha >= @Dia AND cvb2.Fecha < @DiaSig)
		WHERE
			vp.Estatus = 1
			AND vp.SucursalID = @SucursalID
			AND (vp.Fecha >= @Dia AND vp.Fecha < @DiaSig)
		GROUP BY
			tfp.NombreTipoFormaPago
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
		, ISNULL(i.Concepto, tfp.NombreTipoFormaPago) AS Concepto
		, i.Tickets
		, i.Facturas
		, (i.Tickets + i.Facturas) AS Total
		, i.Pendientes
	FROM
		TipoFormaPago tfp
		LEFT JOIN _Ingresos i ON i.Concepto = tfp.NombreTipoFormaPago
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

-- EXEC pauComisiones 1, 7, '20140104', '20140110'
-- EXEC pauComisiones 2, 7
ALTER PROCEDURE pauComisiones (
	@ModoID INT
	, @VendedorID INT
	, @Desde DATE = NULL
	, @Hasta DATE = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @ModoID INT = 2
	DECLARE @VendedorID INT = 7
	DECLARE @Desde DATE = ''
	DECLARE @Hasta DATE = ''
	*/

	-- Definición de variables tipo constante
	DECLARE @ModoNormalID INT = 1
	DECLARE @ModoNoPagadasID INT = 2
	
	DECLARE @EstCobradaID INT = 2
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCanceladaID INT = 4
	DECLARE @EstGenCompletado INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- De momento no se toma en cuenta la sucursal
	DECLARE @PorComision DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje')
	DECLARE @PorComision9500 DECIMAL(5, 2) = (SELECT (CONVERT(DECIMAL(5, 2), Valor) / 100) FROM Configuracion WHERE Nombre = 'Comisiones.Vendedor.Porcentaje9500')
	DECLARE @IvaMul DECIMAL(5, 2) = (SELECT (1 + (CONVERT(DECIMAL(5, 2), Valor) / 100)) FROM Configuracion WHERE Nombre = 'IVA')

	-- Modo normal
	IF @ModoID = @ModoNormalID BEGIN
		;WITH _ComVentas AS (
			SELECT
				vp.VentaID
				, MAX(vp.Fecha) AS Fecha
				, c.Nombre AS Cliente
				, v.Folio
				, SUM(vpd.Importe) AS Importe
				, (
					SELECT
						CASE WHEN v.ComisionistaClienteID > 0 THEN
							SUM(ROUND(((
								CASE com.ListaDePrecios
									WHEN 1 THEN ROUND(pp.PrecioUno / @IvaMul, 2)
									WHEN 2 THEN ROUND(pp.PrecioDos / @IvaMul, 2)
									WHEN 3 THEN ROUND(pp.PrecioTres / @IvaMul, 2)
									WHEN 4 THEN ROUND(pp.PrecioCuatro / @IvaMul, 2)
									WHEN 5 THEN ROUND(pp.PrecioCinco / @IvaMul, 2)
								END
								- pp.Costo) * vdt.Cantidad) * @PorComision, 2))
						ELSE
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(ROUND(((vdt.PrecioUnitario - pp.Costo) * vdt.Cantidad) * @PorComision, 2))
							ELSE
								SUM(ROUND(((vdt.PrecioUnitario - pp.Costo) * vdt.Cantidad) * @PorComision9500, 2))
							END
							-- SUM(ROUND(((vdt.PrecioUnitario - pp.Costo) * vdt.Cantidad) *
								-- (CASE WHEN p.Es9500 = 1 THEN @PorComision9500 ELSE @PorComision END), 2))
								-- (CASE WHEN c9.VentaID IS NULL THEN @PorComision ELSE @PorComision9500 END), 2))
						END
					FROM
						VentaDetalle vdt
						INNER JOIN Parte p ON p.ParteID = vdt.ParteID AND p.Estatus = 1
						LEFT JOIN PartePrecio pp ON pp.ParteID = vdt.ParteID AND pp.Estatus = 1
						LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
					WHERE
						vdt.Estatus = 1
						AND vdt.VentaID = vp.VentaID
						AND p.AplicaComision = 1
				) AS Comision
				, (
					SELECT
						CASE WHEN v.ComisionistaClienteID > 0 THEN
							SUM(ROUND(((
								CASE com.ListaDePrecios
									WHEN 1 THEN ROUND(pp.PrecioUno / @IvaMul, 2)
									WHEN 2 THEN ROUND(pp.PrecioDos / @IvaMul, 2)
									WHEN 3 THEN ROUND(pp.PrecioTres / @IvaMul, 2)
									WHEN 4 THEN ROUND(pp.PrecioCuatro / @IvaMul, 2)
									WHEN 5 THEN ROUND(pp.PrecioCinco / @IvaMul, 2)
								END
								- pp.Costo) * vddi.Cantidad) * @PorComision, 2))
						ELSE
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(ROUND(((vddi.PrecioUnitario - pp.Costo) * vddi.Cantidad) * @PorComision, 2))
							ELSE
								SUM(ROUND(((vddi.PrecioUnitario - pp.Costo) * vddi.Cantidad) * @PorComision9500, 2))
							END
						END
					FROM
						VentaDevolucion vdi
						INNER JOIN VentaDevolucionDetalle vddi ON vddi.VentaDevolucionID = vdi.VentaDevolucionID
							AND vddi.Estatus = 1
						INNER JOIN Parte p ON p.ParteID = vddi.ParteID AND p.Estatus = 1
						LEFT JOIN PartePrecio pp ON pp.ParteID = vddi.ParteID AND pp.Estatus = 1
						LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
					WHERE
						vdi.Estatus = 1
						AND vdi.VentaID = vp.VentaID
						AND p.AplicaComision = 1
				) AS ComisionDev
				, v.ACredito
				, 'V' Caracteristica
				, 1 AS Orden
			FROM
				VentaPago vp
				INNER JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vp.VentaID AND v.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
					AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			WHERE
				vp.Estatus = 1
				AND (vp.Fecha >= @Desde AND vp.Fecha < @Hasta)
				AND vpd.Importe > 0
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
			GROUP BY
				vp.VentaID
				, v.Folio
				, c.Nombre
				, v.ACredito
				, v.ComisionistaClienteID
				, c9.VentaID
		)
		
		, _ComDevoluciones AS (
			SELECT
				vd.VentaID
				, vd.Fecha
				, c.Nombre AS Cliente
				, v.Folio
				, (SUM((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad)) AS Importe
				, ((
					SELECT
						CASE WHEN v.ComisionistaClienteID > 0 THEN
							SUM(ROUND(((
								CASE com.ListaDePrecios
									WHEN 1 THEN ROUND(pp.PrecioUno / @IvaMul, 2)
									WHEN 2 THEN ROUND(pp.PrecioDos / @IvaMul, 2)
									WHEN 3 THEN ROUND(pp.PrecioTres / @IvaMul, 2)
									WHEN 4 THEN ROUND(pp.PrecioCuatro / @IvaMul, 2)
									WHEN 5 THEN ROUND(pp.PrecioCinco / @IvaMul, 2)
								END
								- pp.Costo) * vddt.Cantidad) * @PorComision, 2))
						ELSE
							CASE WHEN c9.VentaID IS NULL THEN
								SUM(ROUND(((vddt.PrecioUnitario - pp.Costo) * vddt.Cantidad) * @PorComision, 2))
							ELSE
								SUM(ROUND(((vddt.PrecioUnitario - pp.Costo) * vddt.Cantidad) * @PorComision9500, 2))
							END
						END
					FROM
						VentaDevolucionDetalle vddt
						INNER JOIN Parte p ON p.ParteID = vddt.ParteID AND p.Estatus = 1
						LEFT JOIN PartePrecio pp ON pp.ParteID = vddt.ParteID AND pp.Estatus = 1
						LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
					WHERE
						vddt.Estatus = 1
						AND vddt.VentaDevolucionID = vd.VentaDevolucionID
						AND p.AplicaComision = 1
				)) AS Comision
				, v.ACredito
				, 'D' AS Caracteristica
				, 2 AS Orden

				-- Para sacar el total de lo pagado
				, (
					SELECT SUM(vpd.Importe)
					FROM
						VentaPago vp
						LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID
							AND vpd.Importe > 0 AND vpd.Estatus = 1
					WHERE vp.VentaID = vd.VentaID and vp.Estatus = 1
				) AS Pagado
			FROM
				VentaDevolucion vd
				LEFT JOIN VentaDevolucionDetalle vdd ON vdd.VentaDevolucionID = vd.VentaDevolucionID AND vdd.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
					AND c9.Estatus = 1
				LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
			WHERE
				vd.Estatus = 1
				AND (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
				AND v.RealizoUsuarioID = @VendedorID
				AND v.VentaEstatusID IN (@EstPagadaID, @EstCanceladaID)
			GROUP BY
				vd.VentaDevolucionID
				, vd.VentaID
				, vd.Fecha
				, c.Nombre
				, v.Folio
				, v.ACredito
				, v.ComisionistaClienteID
				, c9.VentaID
		)

		SELECT
			VentaID
			, Fecha
			, Cliente
			, Folio
			, CASE WHEN ACredito = 1 THEN 0.00 ELSE Importe END AS Importe
			, CASE WHEN ACredito = 1 THEN Importe ELSE 0.00 END AS Cobranza
			, (ISNULL(Comision, 0.00) + ISNULL(ComisionDev, 0.00)) AS Comision
			, ACredito
			, CASE WHEN ComisionDev IS NULL THEN Caracteristica ELSE 'VD' END AS Caracteristica
			, Orden
		FROM _ComVentas
		UNION
		SELECT
			VentaID
			, Fecha
			, Cliente
			, Folio
			, CASE WHEN ACredito = 1 THEN 0.00 ELSE (Importe * -1) END AS Importe
			, CASE WHEN ACredito = 1 THEN (Importe * -1) ELSE 0.00 END AS Cobranza
			, ISNULL((Comision * -1), 0.00) AS Comision
			, ACredito
			, Caracteristica
			, Orden
		FROM _ComDevoluciones
		
		WHERE Pagado >= Importe

		ORDER BY Orden, Fecha
	END

	-- Modo ventas no pagadas (serían las de crédito)
	IF @ModoID = @ModoNoPagadasID BEGIN
		SELECT
			v.VentaID
			, v.Fecha
			, c.Nombre AS Cliente
			, v.Folio
			-- , SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Importe
			, vdc.Importe
			, (
				SELECT
					CASE WHEN v.ComisionistaClienteID > 0 THEN
						SUM(ROUND(((
							CASE com.ListaDePrecios
								WHEN 1 THEN ROUND(pp.PrecioUno / @IvaMul, 2)
								WHEN 2 THEN ROUND(pp.PrecioDos / @IvaMul, 2)
								WHEN 3 THEN ROUND(pp.PrecioTres / @IvaMul, 2)
								WHEN 4 THEN ROUND(pp.PrecioCuatro / @IvaMul, 2)
								WHEN 5 THEN ROUND(pp.PrecioCinco / @IvaMul, 2)
							END
							- pp.Costo) * vdt.Cantidad) * @PorComision, 2))
					ELSE
						CASE WHEN c9.VentaID IS NULL THEN
							SUM(ROUND(((vdt.PrecioUnitario - pp.Costo) * vdt.Cantidad) * @PorComision, 2))
						ELSE
							SUM(ROUND(((vdt.PrecioUnitario - pp.Costo) * vdt.Cantidad) * @PorComision9500, 2))
						END
					END
				FROM
					VentaDetalle vdt
					INNER JOIN Parte p ON p.ParteID = vdt.ParteID AND p.Estatus = 1
					LEFT JOIN PartePrecio pp ON pp.ParteID = vdt.ParteID AND pp.Estatus = 1
					LEFT JOIN Cliente com ON com.ClienteID = v.ComisionistaClienteID AND com.Estatus = 1
				WHERE
					vdt.Estatus = 1
					AND vdt.VentaID = v.VentaID
					AND p.AplicaComision = 1
			) AS Comision
			, 0.00 AS Cobranza
			, v.ACredito
			, '' AS Caracteristica
			, 1 AS Orden
		FROM
			Venta v
			-- LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
			LEFT JOIN (
				SELECT
					VentaId
					, SUM((precioUnitario + iva) * cantidad) as Importe
				FROM VentaDetalle
				WHERE Estatus = 1
				GROUP BY VentaID
			) vdc ON vdc.VentaID = v.VentaID
			LEFT JOIN Cotizacion9500 c9 ON c9.VentaID = v.VentaID AND c9.EstatusGenericoID = @EstGenCompletado
					AND c9.Estatus = 1
			LEFT JOIN Cliente c ON c.ClienteID = v.ClienteID AND c.Estatus = 1
		WHERE
			v.Estatus = 1
			-- AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.VentaEstatusID = @EstCobradaID
			AND v.RealizoUsuarioID = @VendedorID
		/* GROUP BY
			v.VentaID
			, v.Fecha
			, v.Folio
			, c.Nombre
			, v.ACredito
			, v.ComisionistaClienteID
		*/
		ORDER BY Fecha
	END

END
GO