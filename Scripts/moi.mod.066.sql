/* Script con modificaciones a la base de datos de Theos. Archivo 066
 * Creado: 2016/04/15
 * Subido: 2016/05/05
 */

DECLARE @ScriptID INT = 66
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE Cliente ADD
	CobranzaContacto NVARCHAR(128) NULL
	, CobranzaObservacion NVARCHAR(512) NULL

CREATE INDEX Ix_VentaID_Estatus ON VentaPago (VentaID, Estatus)
CREATE INDEX Ix_VentaID_Estatus ON VentaDetalle (VentaID, Estatus)

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.Clientes.Avisos', 'D', 'P', 'Salida donde se muestra el reporte de los avisos de cobro de los clientes.')

ALTER TABLE Usuario ADD
	AlertaTraspasos BIT NULL
	, AlertaDevFacturaCreditoAnt BIT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[ClientesCreditoView] AS
	WITH _Tiempos AS (
		SELECT
			v.VentaID
			, v.Fecha
			, v.ClienteID
			, DATEDIFF(DAY, v.Fecha, MAX(vp.Fecha)) AS DiasPago
		FROM
			Venta v
			LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
		WHERE
			v.Estatus = 1
			AND v.ACredito = 1
			AND v.VentaEstatusID = 3
			AND v.Fecha >= DATEADD(YEAR, -1, GETDATE())
		GROUP BY
			v.VentaID
			, v.Fecha
			, v.ClienteID
	), _VentasPre AS (
		SELECT
			v.VentaID
			, v.ClienteID
			, v.Fecha
			, DATEDIFF(DAY, v.Fecha, GETDATE()) AS Dias
			, SUM((vd.PrecioUnitario + vd.Iva) * vd.Cantidad) AS Total
		FROM
			Venta v
			LEFT JOIN VentaDetalle vd ON vd.VentaID = v.VentaID AND vd.Estatus = 1
		WHERE
			v.Estatus = 1
			AND v.ACredito = 1
			AND v.VentaEstatusID = 2
		GROUP BY
			v.VentaID
			, v.ClienteID
			, v.Fecha
	), _Ventas AS (
		SELECT
			v.*
			, (v.Total - SUM(ISNULL(vpd.Importe, 0.0))) as Adeudo
		FROM
			_VentasPre v
			LEFT JOIN VentaPago vp ON vp.VentaID = v.VentaID AND vp.Estatus = 1
			LEFT JOIN VentaPagoDetalle vpd ON vpd.VentaPagoID = vp.VentaPagoID AND vpd.Estatus = 1
		GROUP BY
			v.VentaID
			, v.ClienteID
			, v.Fecha
			, v.Dias
			, v.Total
	)
	SELECT
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.LimiteCredito AS LimiteDeCredito
		, c.DiasDeCredito
		, c.Tolerancia
		, c.DiaDeCobro
		, c.HoraDeCobro
		, c.SiempreFactura
		, c.SiempreTicket
		, c.SiempreVale
		, c.TicketPrecio1
		, c.CobranzaContacto
		, SUM(ca.Adeudo) AS Adeudo
		, MIN(ca.Fecha) AS FechaPrimerAdeudo
		-- , ISNULL(CONVERT(BIT, CASE WHEN (ca.Adeudo >= c.LimiteCredito OR ca.FechaPrimerAdeudo >= (GETDATE() + c.DiasDeCredito))
		-- 	THEN 1 ELSE 0 END), 0) AS CreditoVencido
		, SUM(CASE WHEN ca.Dias >= c.DiasDeCredito THEN ca.Adeudo ELSE 0.0 END) AS AdeudoVencido
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID) AS PromedioDePagoAnual
		, (SELECT AVG(DiasPago) FROM _Tiempos WHERE ClienteID = c.ClienteID
			-- AND Fecha > DATEADD(MONTH, -3, GETDATE())) AS PromedioDePago3Meses
			-- Por alguna extraña razón, el filtro se debe de hacer con "Fecha <=" y "NOT IN" en vez de "Fecha >", pues si
			-- se hace de esta segunda manera, la consulta se incrementa como 12 segundos en tiempo de ejecución.
			AND VentaID NOT IN (SELECT VentaID FROM _Tiempos WHERE Fecha <= DATEADD(MONTH, -3, GETDATE())))
			AS PromedioDePago3Meses
	FROM
		Cliente c
		LEFT JOIN _Ventas ca ON ca.ClienteID = c.ClienteID
	GROUP BY
		c.ClienteID
		, c.Nombre
		, c.TieneCredito
		, c.LimiteCredito
		, c.DiasDeCredito
		, c.Tolerancia
		, c.DiaDeCobro
		, c.HoraDeCobro
		, c.SiempreFactura
		, c.SiempreTicket
		, c.SiempreVale
		, c.TicketPrecio1
		, c.CobranzaContacto
GO

ALTER VIEW [dbo].[ClientesView]
AS
	SELECT
		Cliente.ClienteID
		,ClienteFacturacion.Rfc
		,Cliente.Nombre
		,Cliente.Calle
		,Cliente.NumeroInterior
		,Cliente.NumeroExterior
		,Cliente.Colonia
		,Cliente.CodigoPostal
		,Cliente.EstadoID
		,Estado.NombreEstado
		,Cliente.MunicipioID
		,Municipio.NombreMunicipio
		,Cliente.CiudadID
		,Ciudad.NombreCiudad
		,Cliente.Telefono
		,Cliente.Celular
		,Cliente.Particular
		,Cliente.Nextel
		,Cliente.ListaDePrecios
		,ISNULL(Cliente.TieneCredito, 0) AS TieneCredito
		-- ,Cliente.Credito
		,Cliente.DiasDeCredito
		,Cliente.TipoFormaPagoID
		,TipoFormaPago.NombreTipoFormaPago
		,Cliente.BancoID
		,Banco.NombreBanco
		,Cliente.TipoClienteID
		,TipoCliente.NombreTipoCliente
		,Cliente.CuentaBancaria
		,ISNULL(Cliente.Tolerancia, 0) AS Tolerancia
		,Cliente.LimiteCredito
		,ISNULL(Cliente.EsClienteComisionista, 0) AS EsClienteComisionista
		,ISNULL(Cliente.EsTallerElectrico, 0) AS EsTallerElectrico
		,ISNULL(Cliente.EsTallerMecanico, 0) AS EsTallerMecanico
		,ISNULL(Cliente.EsTallerDiesel, 0) AS EsTallerDiesel
		,ISNULL(Cliente.Vip, 0) AS Vip
		,Cliente.ClienteComisionistaID
		,Com.Nombre AS NombreClienteComisionista
		,Cliente.FechaRegistro
		,Cliente.Alias
		-- ,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 1) AS DeudaActual
		-- ,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 2) AS DeudaVencido
		-- ,dbo.fnuClientePromedioPago(Cliente.ClienteID, 1) AS PromedioPagoTotal
		-- ,dbo.fnuClientePromedioPago(Cliente.ClienteID, 2) AS PromedioPagoTresMeses
		,Cliente.Acumulado AS AcumuladoH
		-- ,dbo.fnuClienteAcumulado(Cliente.ClienteID) AS Acumulado
		, Cliente.CobranzaContacto
		, Cliente.CobranzaObservacion
	FROM
		Cliente
		LEFT JOIN ClienteFacturacion ON ClienteFacturacion.ClienteID = Cliente.ClienteID
		INNER JOIN Estado ON Estado.EstadoID = Cliente.EstadoID
		INNER JOIN Municipio ON Municipio.MunicipioID = Cliente.MunicipioID
		INNER JOIN Ciudad ON Ciudad.CiudadID = Cliente.CiudadID
		LEFT JOIN TipoFormaPago ON TipoFormaPago.TipoFormaPagoID = Cliente.TipoFormaPagoID
		LEFT JOIN Banco ON Banco.BancoID = Cliente.BancoID
		LEFT JOIN TipoCliente ON TipoCliente.TipoClienteID = Cliente.TipoClienteID
		LEFT JOIN Cliente Com ON Com.ClienteComisionistaID = Cliente.ClienteID
	WHERE
		Cliente.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@Sucursales tpuTablaEnteros READONLY
	, @Marcas tpuTablaEnteros READONLY
	, @Lineas tpuTablaEnteros READONLY
)
AS BEGIN
SET NOCOUNT ON

	/*
	DECLARE @Sucursales tpuTablaEnteros
	INSERT INTO @Sucursales (Entero) VALUES (1), (2), (3)
	DECLARE @Marcas tpuTablaEnteros
	DECLARE @Lineas tpuTablaEnteros
	EXEC pauPedidosSugeridos @Sucursales
	*/

	DECLARE @EstGenPendiente INT = 2
	DECLARE @EstPedidoNoSurtido INT = 2
	DECLARE @OpeTraspaso INT = 5
	DECLARE @EstVentaPagada INT = 3

	DECLARE @Ayer DATE = DATEADD(D, -1, GETDATE())

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
			, CASE WHEN MinMatriz >= ExistenciaMatriz THEN MaxMatriz - ExistenciaMatriz - TraspasoMatriz ELSE 0.0 END AS NecesidadMatriz
			, CASE WHEN MinSuc02 >= ExistenciaSuc02 THEN MaxSuc02 - ExistenciaSuc02 - TraspasoSuc02 ELSE 0.0 END AS NecesidadSuc02
			, CASE WHEN MinSuc03 >= ExistenciaSuc03 THEN MaxSuc03 - ExistenciaSuc03 - TraspasoSuc03 ELSE 0.0 END AS NecesidadSuc03
			-- , MaxMatriz - ExistenciaMatriz AS NecesidadMatriz
			-- , MaxSuc02 - ExistenciaSuc02 AS NecesidadSuc02
			-- , MaxSuc03 - ExistenciaSuc03 AS NecesidadSuc03
			-- ,(MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) AS Total
			-- ,CEILING((MaxMatriz - ExistenciaMatriz) + (MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03) /
			-- (CASE WHEN Pre.UnidadEmpaque = 0 THEN 1 ELSE Pre.UnidadEmpaque END)) * Pre.UnidadEmpaque AS Pedido
			,Costo
			, CostoConDescuento
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
				, PartePrecio.CostoConDescuento
				, p.ProveedorID
				,Proveedor.NombreProveedor
				,Proveedor.Beneficiario	
			FROM 
				Parte p
				INNER JOIN ParteAbc ON  ParteAbc.ParteID = p.ParteID
				INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
				INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = pmm.SucursalID -- AND pe.Estatus = 1
				INNER JOIN PartePrecio ON PartePrecio.ParteID = p.ParteID -- AND PartePrecio.Estatus = 1
				INNER JOIN Proveedor ON Proveedor.ProveedorID = p.ProveedorID -- AND Proveedor.Estatus = 1
				LEFT JOIN (
					SELECT
						mi.SucursalDestinoID AS SucursalID
						, mid.ParteID
						, mid.Cantidad
					FROM
						MovimientoInventario mi
						INNER JOIN MovimientoInventarioDetalle mid ON 
							mid.MovimientoInventarioID = mi.MovimientoInventarioID AND mid.Estatus = 1
					WHERE
						mi.Estatus = 1
						AND mi.TipoOperacionID = @OpeTraspaso
						AND mi.FechaRecepcion IS NULL
				) tra ON tra.ParteID = p.ParteID AND tra.SucursalID = pmm.SucursalID
				-- Se agregan filtro de ventas recientes, para artículos con ABC de C en adelante
				/*
				LEFT JOIN (
					SELECT DISTINCT vd.ParteID
					FROM
						VentaDetalle vd
						INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.VentaEstatusID = @EstVentaPagada
							AND DATEDIFF(DAY, v.Fecha, GETDATE()) <= 1 AND v.Estatus = 1
						LEFT JOIN ParteAbc pa ON pa.ParteID = vd.ParteID AND pa.AbcDeVentas IN ('A', 'B', 'C')
					WHERE
						vd.Estatus = 1
						AND pa.ParteAbcID IS NULL
				) vr ON vr.ParteID = p.ParteID
				-- Se valida que la parte no haya sido vendida ayer u hoy, pues si sí, no se debe pedir
				*/
				LEFT JOIN (
					SELECT DISTINCT vd.ParteID, SUM(vd.Cantidad) AS Cantidad
					FROM
						VentaDetalle vd
						INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.VentaEstatusID = @EstVentaPagada
							AND v.Fecha >= @Ayer AND v.Estatus = 1
						LEFT JOIN ParteAbc pa ON pa.ParteID = vd.ParteID AND pa.AbcDeVentas IN ('C', 'D', 'E', 'N', 'Z')
					WHERE
						vd.Estatus = 1
						AND pa.ParteAbcID IS NOT NULL
					GROUP BY vd.ParteID
					HAVING
						SUM(vd.Cantidad) > 0
				) vr2 ON vr2.ParteID = p.ParteID AND (vr2.Cantidad > (pmm.Minimo - pe.Existencia))
			WHERE
				p.Estatus = 1 
				AND pmm.Maximo > 0
				-- AND pe.Existencia <= pmm.Minimo
				AND p.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
				AND (pe.SucursalID IN (SELECT Entero FROM @Sucursales))
				-- Se agregan los filtros de Marcas y Líneas
				AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
				AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))
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
				
				-- Se agregan filtro de ventas recientes, para artículos con ABC de C en adelante
				-- AND vr.ParteID IS NULL
				--
				AND vr2.ParteID IS NULL
				/* AND (ParteAbc.AbcDeVentas IN ('A', 'B', 'C') OR NOT EXISTS(
					SELECT 1
					FROM
						VentaDetalle vd
						INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.VentaEstatusID = @EstVentaPagada
							AND DATEDIFF(DAY, v.Fecha, GETDATE()) <= 1 AND v.Estatus = 1
					WHERE vd.Estatus = 1
					))
				*/
			GROUP BY
				p.ParteID
				, p.NumeroParte
				, p.NombreParte
				, p.UnidadEmpaque
				,ParteAbc.AbcDeVentas
				,PartePrecio.Costo
				, PartePrecio.CostoConDescuento
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
		, CostoConDescuento
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
				-- _Pedidos pi
				(SELECT DISTINCT ParteID FROM ParteEquivalente) pi
				LEFT JOIN ParteEquivalente pq ON pq.ParteID = pi.ParteID
				LEFT JOIN ParteEquivalente pee ON pee.GrupoID = pq.GrupoID AND pee.ParteID != pq.ParteID
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
		, (SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 1 THEN pec.ExistenciaMatriz ELSE 0.0 END, 0.0))) AS NecesidadMatriz
		, (SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END) 
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 2 THEN pec.ExistenciaSuc02 ELSE 0.0 END, 0.0))) AS NecesidadSuc02
		, (SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 3 THEN pec.ExistenciaSuc03 ELSE 0.0 END, 0.0))) AS NecesidadSuc03
		, SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
			+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 1 THEN pec.ExistenciaMatriz ELSE 0.0 END, 0.0))
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 2 THEN pec.ExistenciaSuc02 ELSE 0.0 END, 0.0))
			- SUM(ISNULL(CASE WHEN c9.SucursalID = 3 THEN pec.ExistenciaSuc03 ELSE 0.0 END, 0.0))
		AS Total
		, -- Se calcula el Pedido
			(CEILING((
				SUM(CASE WHEN c9.SucursalID = 1 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 2 THEN c9d.Cantidad ELSE 0.0 END)
				+ SUM(CASE WHEN c9.SucursalID = 3 THEN c9d.Cantidad ELSE 0.0 END)
			) / (CASE WHEN p.UnidadEmpaque = 0 THEN 1 ELSE p.UnidadEmpaque END)) * p.UnidadEmpaque
			) AS Pedido
		, pp.Costo
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, (CONVERT(VARCHAR, c9.Fecha, 103) + ' ' + CONVERT(VARCHAR(5), c9.Fecha, 114)
			+ ' - ' + u.NombreUsuario) AS Observacion
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
		LEFT JOIN Usuario u ON u.UsuarioID = c9.RealizoUsuarioID AND u.Estatus = 1
	WHERE
		c9d.Estatus = 1
		AND c9.EstatusGenericoID = @EstGenPendiente
		AND (c9.SucursalID IN (SELECT Entero FROM @Sucursales))
		-- Se agregan los filtros de Marcas y Líneas
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))
		--
		AND pd.PedidoDetalleID IS NULL
	GROUP BY
		c9d.ParteID
		, c9.Fecha
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, u.NombreUsuario
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
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, (CONVERT(VARCHAR, rf.FechaRegistro, 103) + ' ' + CONVERT(VARCHAR(5), rf.FechaRegistro, 114)
			+ ' - ' + u.NombreUsuario + ' - ' + rf.Comentario) AS Observacion
		, 'RF' AS Caracteristica
	FROM
		ReporteDeFaltante rf
		INNER JOIN Parte p ON p.ParteID = rf.ParteID AND p.Estatus = 1
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
		AND (rf.SucursalID IN (SELECT Entero FROM @Sucursales))
		-- Se agregan los filtros de Marcas y Líneas
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))
	GROUP BY
		rf.ParteID
		, rf.FechaRegistro
		, p.NumeroParte
		, p.NombreParte
		, p.UnidadEmpaque
		, pa.AbcDeVentas
		, pec.ExistenciaMatriz
		, pec.ExistenciaSuc02
		, pec.ExistenciaSuc03
		, pp.Costo
		, pp.CostoConDescuento
		, p.ProveedorID
		, pv.NombreProveedor
		, pv.Beneficiario
		, rf.Comentario
		, u.NombreUsuario

END
GO