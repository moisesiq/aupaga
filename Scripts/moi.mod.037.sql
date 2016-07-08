/* Script con modificaciones a la base de datos de Theos. Archivo 037
 * Creado: 2015/09/28
 * Subido: 2015/10/08
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- DROP TABLE ParteExistenciaHistorico
CREATE TABLE ParteExistenciaHistorico (
	ParteExistenciaHistoricoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, Cantidad DECIMAL(5, 2) NOT NULL
	, ExistenciaNueva DECIMAL(5, 2) NOT NULL
	, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
	, RelacionTabla NVARCHAR(64) NOT NULL
	, RelacionID INT NOT NULL
)

ALTER TABLE CajaFacturaGlobal ADD
	DevolucionesDia DECIMAL(12, 2) NULL
	, DevolucionesDiasAnt DECIMAL(12, 2) NULL
	, GarantiasDia DECIMAL(12, 2) NULL
	, GarantiasDiasAnt DECIMAL(12, 2) NULL
	, Cobranza DECIMAL(12, 2) NULL
GO
UPDATE CajaFacturaGlobal SET
	DevolucionesDia = 0
	, DevolucionesDiasAnt = (Devoluciones + Cancelaciones)
	, GarantiasDia = 0
	, GarantiasDiasAnt = 0
	, Cobranza = 0
ALTER TABLE CajaFacturaGlobal ALTER COLUMN DevolucionesDia DECIMAL(12, 2) NOT NULL
ALTER TABLE CajaFacturaGlobal ALTER COLUMN DevolucionesDiasAnt DECIMAL(12, 2) NOT NULL
ALTER TABLE CajaFacturaGlobal ALTER COLUMN GarantiasDia DECIMAL(12, 2) NOT NULL
ALTER TABLE CajaFacturaGlobal ALTER COLUMN GarantiasDiasAnt DECIMAL(12, 2) NOT NULL
ALTER TABLE CajaFacturaGlobal ALTER COLUMN Cobranza DECIMAL(12, 2) NOT NULL
ALTER TABLE CajaFacturaGlobal DROP COLUMN Devoluciones, Cancelaciones

ALTER TABLE CorteDetalleHistorico ADD Factura BIT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

-- DROP VIEW CorteValidacionView
CREATE VIEW CorteValidacionView AS
	SELECT
		cdh.CorteDetalleHistoricoID
		, cdh.Dia
		, cdh.SucursalID
		, cdh.CorteCategoriaID
		, cdh.RelacionID
		, cdh.Concepto
		, cdh.Factura
		, cdh.Importe
		, cdh.Efectivo
		, cdh.Cheque
		, cdh.Tarjeta
		, cdh.Transferencia
		, cdh.Vale
		, cp.ContaPolizaID
		-- , SUM(CASE WHEN cpd.ContaCuentaAuxiliarID = 2 THEN (cpd.Cargo + cpd.Abono) ELSE 0.0 END) AS CuentaCaja
		-- , SUM(CASE WHEN cpd.ContaCuentaAuxiliarID = 571 THEN (cpd.Cargo + cpd.Abono) ELSE 0.0 END) AS CuentaAnticipoClientes
		-- , SUM(CASE WHEN cpd.ContaCuentaAuxiliarID = 353 THEN (cpd.Cargo + cpd.Abono) ELSE 0.0 END) AS CuentaInventario
		, cpd.CuentaCaja
		, cpd.CuentaAnticipoClientes
		, cpd.CuentaInventario
		, SUM(pk.Cantidad) AS KardexCantidad
		, SUM(pk.Importe) AS KardexImporte
		, SUM(peh.Cantidad) AS ExistenciaCantidad
	FROM
		CorteDetalleHistorico cdh
		LEFT JOIN ContaPoliza cp ON cp.RelacionTabla = cdh.RelacionTabla AND cp.RelacionID = cdh.RelacionID
		-- LEFT JOIN ContaPolizaDetalle cpd ON cpd.ContaPolizaID = cp.ContaPolizaID AND cpd.ContaCuentaAuxiliarID IN (2, 571, 353)
		LEFT JOIN (
			SELECT
				ContaPolizaID
				, SUM(CASE WHEN ContaCuentaAuxiliarID = 2 THEN (Cargo + Abono) ELSE 0.0 END) AS CuentaCaja
				, SUM(CASE WHEN ContaCuentaAuxiliarID = 571 THEN (Cargo + Abono) ELSE 0.0 END) AS CuentaAnticipoClientes
				, SUM(CASE WHEN ContaCuentaAuxiliarID = 353 THEN (Cargo + Abono) ELSE 0.0 END) AS CuentaInventario
			FROM ContaPolizaDetalle
			WHERE ContaCuentaAuxiliarID IN (2, 571, 353)
			GROUP BY ContaPolizaID
		) cpd ON cpd.ContaPolizaID = cp.ContaPolizaID
		LEFT JOIN ParteKardex pk ON pk.RelacionTabla = cdh.RelacionTabla AND pk.RelacionID = cdh.RelacionID
		LEFT JOIN ParteExistenciaHistorico peh ON peh.RelacionTabla = cdh.RelacionTabla AND peh.RelacionID = cdh.RelacionID
	GROUP BY
		cdh.CorteDetalleHistoricoID
		, cdh.Dia
		, cdh.SucursalID
		, cdh.CorteCategoriaID
		, cdh.RelacionID
		, cdh.Concepto
		, cdh.Factura
		, cdh.Importe
		, cdh.Efectivo
		, cdh.Cheque
		, cdh.Tarjeta
		, cdh.Transferencia
		, cdh.Vale
		, cp.ContaPolizaID
		, cpd.CuentaCaja
		, cpd.CuentaAnticipoClientes
		, cpd.CuentaInventario
GO

ALTER VIEW [dbo].[CajaFacturasGlobalesView] AS
	SELECT
		fg.CajaFacturaGlobalID
		, fg.Dia
		, fg.SucursalID
		, s.NombreSucursal AS Sucursal
		, fg.Tickets
		, fg.FacturadoDeDiasAnt
		, fg.Negativos
		-- , fg.Devoluciones
		-- , fg.Cancelaciones
		, fg.DevolucionesDia
		, fg.DevolucionesDiasAnt
		, fg.GarantiasDia
		, fg.GarantiasDiasAnt
		, fg.Cobranza
		, (fg.Tickets - fg.FacturadoDeDiasAnt - fg.Negativos
			- fg.DevolucionesDia - fg.DevolucionesDiasAnt - fg.GarantiasDia - fg.GarantiasDiasAnt) AS Oficial
		, fg.Restar
		, (fg.Tickets - fg.FacturadoDeDiasAnt - fg.Negativos
			- fg.DevolucionesDia - fg.DevolucionesDiasAnt - fg.GarantiasDia - fg.GarantiasDiasAnt
			- fg.Restar) AS Supuesto
		, fg.CostoMinimo
		, fg.Restante
		, fg.SaldoRestante
		, fg.Facturado
	FROM
		CajaFacturaGlobal fg
		LEFT JOIN Sucursal s ON s.SucursalID = fg.SucursalID AND s.Estatus = 1
GO

ALTER VIEW [dbo].[ContaPolizasDetalleAvanzadoView] AS
	SELECT
		cpd.ContaPolizaDetalleID
		, cpd.ContaPolizaID
		, cp.Fecha AS FechaPoliza
		, cp.Origen AS OrigenPoliza
		, cp.SucursalID AS SucursalID
		, s.NombreSucursal AS Sucursal
		, cp.Concepto AS ConceptoPoliza
		, cp.Error
		, cp.FueManual
		, cp.RelacionTabla
		, cp.RelacionID
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
		LEFT JOIN Sucursal s ON s.SucursalID = cp.SucursalID AND s.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cpd.ContaCuentaAuxiliarID
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

	DECLARE @EstGenPendiente INT = 2
	DECLARE @EstPedidoNoSurtido INT = 2
	DECLARE @OpeTraspaso INT = 5
	DECLARE @EstVentaPagada INT = 3

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
				INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = pmm.SucursalID AND pe.Estatus = 1
				INNER JOIN PartePrecio ON PartePrecio.ParteID = p.ParteID AND PartePrecio.Estatus = 1
				INNER JOIN Proveedor ON Proveedor.ProveedorID = p.ProveedorID AND Proveedor.Estatus = 1
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
				AND vr.ParteID IS NULL
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