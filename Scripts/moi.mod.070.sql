/* Script con modificaciones a la base de datos de Theos. Archivo 070
 * Creado: 2016/06/30
 * Subido: 2016/07/07
 */

DECLARE @ScriptID INT = 70
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE InventarioLinea ALTER COLUMN LineaID INT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauPartesMaxMin] (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
	, @Proveedores tpuTablaEnteros READONLY
	, @Marcas tpuTablaEnteros READONLY
	, @Lineas tpuTablaEnteros READONLY
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @SucursalID INT = 1
	DECLARE @Desde DATE = '2015-01-01'
	DECLARE @Hasta DATE = '2015-12-31'
	DECLARE @Proveedores tpuTablaEnteros
	DECLARE @Marcas tpuTablaEnteros
	DECLARE @Lineas tpuTablaEnteros
	EXEC pauPartesMaxMin @SucursalID, @Desde, @Hasta
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCobradaID INT = 2
	DECLARE @ParteEstActivo INT = 1

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- Para el cálculo de los tres años anteriores
	DECLARE @IniActual DATE = CONVERT(DATE, CONVERT(VARCHAR(4), DATEPART(YEAR, GETDATE())) + '-01-01')
	DECLARE @IniAnt1 DATE = DATEADD(YEAR, -1, @IniActual)
	DECLARE @IniAnt2 DATE = DATEADD(YEAR, -2, @IniActual)
	DECLARE @IniAnt3 DATE = DATEADD(YEAR, -3, @IniActual)

	-- Procedimiento
	
	-- Ventas
	;WITH _Ventas AS (
		SELECT
			vd.ParteID
			, CONVERT(DATE, v.Fecha) AS Fecha
			/* , COUNT(v.VentaID) AS Ventas
			, SUM(vd.Cantidad) AS Cantidad
			, SUM((vd.PrecioUnitario - vd.Costo) * vd.Cantidad) AS Utilidad
			*/
			, COUNT(CASE WHEN (v.Fecha >= @Desde AND v.Fecha < @Hasta) THEN v.VentaID ELSE NULL END) AS Ventas
			, SUM(CASE WHEN (v.Fecha >= @Desde AND v.Fecha < @Hasta) THEN vd.Cantidad ELSE 0.0 END) AS Cantidad
			, SUM(CASE WHEN (v.Fecha >= @Desde AND v.Fecha < @Hasta) THEN ((vd.PrecioUnitario - vd.Costo) * vd.Cantidad)
				ELSE 0.0 END) AS Utilidad
			, COUNT(CASE WHEN (v.Fecha >= @IniAnt1 AND v.Fecha < @IniActual) THEN v.VentaID ELSE NULL END) AS VentasAnt1
			, COUNT(CASE WHEN (v.Fecha >= @IniAnt2 AND v.Fecha < @IniAnt1) THEN v.VentaID ELSE NULL END) AS VentasAnt2
			, COUNT(CASE WHEN (v.Fecha >= @IniAnt3 AND v.Fecha < @IniAnt2) THEN v.VentaID ELSE NULL END) AS VentasAnt3
		FROM
			VentaDetalle vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			-- LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
			-- ?? No sé por´qué estaba esta línea aquí.. left join Parte p on p.ParteID = vd.ParteID and p.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND v.VentaEstatusID IN (@EstPagadaID, @EstCobradaID)
			-- AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND (v.Fecha >= @IniAnt3 AND v.Fecha < @Hasta)
			AND v.SucursalID = @SucursalID
		GROUP BY
			vd.ParteID
			, CONVERT(DATE, v.Fecha)

		-- Se agrega lo negado
		UNION ALL
		SELECT
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro) AS Fecha
			/* , COUNT(rf.ReporteDeFaltanteID) AS Ventas
			, SUM(rf.CantidadRequerida) AS Cantidad
			, SUM((pp.PrecioUno - pp.Costo) * rf.CantidadRequerida) AS Utilidad
			*/
			, COUNT(CASE WHEN (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS Ventas
			, SUM(CASE WHEN (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
				THEN rf.CantidadRequerida ELSE 0.0 END) AS Cantidad
			, SUM(CASE WHEN (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
				THEN ((pp.PrecioUno - pp.Costo) * rf.CantidadRequerida) ELSE 0.0 END) AS Utilidad
			, COUNT(CASE WHEN (rf.FechaRegistro >= @IniAnt1 AND rf.FechaRegistro < @IniActual)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS VentasAnt1
			, COUNT(CASE WHEN (rf.FechaRegistro >= @IniAnt2 AND rf.FechaRegistro < @IniAnt1)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS VentasAnt2
			, COUNT(CASE WHEN (rf.FechaRegistro >= @IniAnt3 AND rf.FechaRegistro < @IniAnt2)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS VentasAnt3
		FROM
			ReporteDeFaltante rf
			LEFT JOIN PartePrecio pp ON pp.ParteID = rf.ParteID AND pp.Estatus = 1
			-- Para validar que no se haya vendido en el día o en el siguiente
			LEFT JOIN (
				SELECT DISTINCT
					vd.ParteID
					, CONVERT(DATE, v.Fecha) AS Dia
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
					AND v.SucursalID = @SucursalID
			) vdc ON vdc.ParteID = rf.ParteID AND (rf.FechaRegistro >= vdc.Dia AND rf.FechaRegistro < DATEADD(d, 2, vdc.Dia))
		WHERE
			rf.Estatus = 1
			-- AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			AND (rf.FechaRegistro >= @IniAnt3 AND rf.FechaRegistro < @Hasta)
			AND rf.SucursalID = @SucursalID
			-- Parte de Venta
			AND vdc.ParteID IS NULL
		GROUP BY
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro)
		-- Fin lo negado
	)
	-- Ventas por semana
	, _VentasPorSem AS (
		SELECT
			ParteID
			, DATEPART(WK, Fecha) AS Semana
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, DATEPART(WK, Fecha)
	)
	-- Ventas por mes
	, _VentasPorMes AS (
		SELECT
			ParteID
			, MONTH(Fecha) AS Mes
			, SUM(Cantidad) AS Cantidad
		FROM _Ventas
		GROUP BY
			ParteID
			, MONTH(Fecha)
	)

	-- Consulta final
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		, pmm.ParteMaxMinReglaID
		-- , pmmr.Descripcion AS DescripcionCalculo
		
		, ISNULL(SUM(vc.Ventas), 0) AS VentasTotal
		, ISNULL(SUM(vc.Cantidad), 0.00) AS CantidadTotal
		, ISNULL(MAX(vc.Cantidad), 0.00) AS CantidadMaxDia
		, ISNULL(MAX(vcs.Cantidad), 0.00) AS CantidadMaxSem
		, ISNULL(MAX(vcm.Cantidad), 0.00) AS CantidadMaxMes
		, ISNULL(SUM(vc.Utilidad), 0.00) AS UtilidadTotal
		, ISNULL(pmm.VentasGlobales, 0) AS VentasGlobales
		
		, ISNULL(SUM(vc.VentasAnt1), 0) AS VentasAnt1
		, ISNULL(SUM(vc.VentasAnt2), 0) AS VentasAnt2
		, ISNULL(SUM(vc.VentasAnt3), 0) AS VentasAnt3
	FROM
		Parte p
		INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID AND pmm.SucursalID = @SucursalID
		LEFT JOIN _Ventas vc ON vc.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorSem GROUP BY ParteID
		) vcs ON vcs.ParteID = p.ParteID
		LEFT JOIN (
			SELECT ParteID, MAX(Cantidad) AS Cantidad FROM _VentasPorMes GROUP BY ParteID
		) vcm ON vcm.ParteID = p.ParteID
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		LEFT JOIN ParteAbc pa ON pa.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		-- LEFT JOIN ParteMaxMinRegla pmmr ON pmmr.ParteMaxMinReglaID = pmm.ParteMaxMinReglaID
	WHERE
		p.Estatus = 1
		AND p.ParteEstatusID = @ParteEstActivo
		AND pmm.Calcular = 1
		-- AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Proveedores) OR p.ProveedorID IN (SELECT Entero FROM @Proveedores))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))
	GROUP BY
		p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.ProveedorID
		, pv.NombreProveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte
		, p.LineaID
		, l.NombreLinea
		, p.UnidadEmpaque
		, p.EsPar
		, p.TiempoReposicion
		, pe.Existencia
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		, pmm.ParteMaxMinReglaID
		-- , pmmr.Descripcion
		, pmm.VentasGlobales
	ORDER BY
		VentasTotal DESC

END
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
			ExistenciaMatriz <= MinMatriz
			OR (
				(ExistenciaSuc02 <= MinSuc02 OR ExistenciaSuc03 <= MinSuc03)
				-- Se sacan los que su necesidad se pueda cubrir con la existencia de matriz
				AND (ExistenciaMatriz - MinMatriz) <= ((MaxSuc02 - ExistenciaSuc02) + (MaxSuc03 - ExistenciaSuc03))
			)
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
		-- , 'MxMn' AS Caracteristica
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

ALTER PROCEDURE [dbo].[pauProAsignarConteoInventario]
AS BEGIN
	SET NOCOUNT ON

	-- EXEC pauProAsignarConteoInventario

	DECLARE @EstGenCompletado INT = 3
	DECLARE @EstGenEnCurso INT = 6
	-- DECLARE @EstGenContado INT = 7
	DECLARE @ConfigHabilitar NVARCHAR(32) = 'Inventario.Asignacion.Habilitar'

	-- Variables calculadas para el proceso
	DECLARE @Ahora DATETIME = GETDATE()
	DECLARE @Hoy DATE = @Ahora
	DECLARE @Maniana DATETIME = DATEADD(DAY, 1, @Hoy)

	-- Se obtiene la lista de las líneas ordenadas
	DECLARE @Lineas TABLE (LineaID INT, Costo DECIMAL(12, 2), Procesada BIT)
	INSERT INTO @Lineas
		SELECT
			l.LineaID
			-- , SUM(pe.Existencia) AS Existencia
			, SUM(pe.Existencia * pp.Costo) AS Costo
			, 0
		FROM
			Linea l
			LEFT JOIN Parte p ON p.LineaID = l.LineaID AND p.Estatus = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
			-- LEFT JOIN InventarioLinea il ON il.LineaID = l.LineaID AND il.EstatusGenericoID = @EstGenCompletado
			--	AND il.InventarioVueltaID = @Vuelta
		WHERE
			l.Estatus = 1
			-- AND il.Id IS NULL
		GROUP BY l.LineaID
		ORDER BY Costo DESC

	-- Se obtiene la lista de periodicidad que aplica para el día
	DECLARE @Periodicidad TABLE (SucursalID INT, LineaID INT, Procesada BIT)
	INSERT INTO @Periodicidad
		SELECT s.SucursalID, ilp.LineaID, 0
		FROM
			InventarioLineaPeriodicidad ilp
			LEFT JOIN Sucursal s ON s.Estatus = 1
			LEFT JOIN InventarioLinea il ON il.LineaID = ilp.LineaID AND il.EstatusGenericoID = @EstGenCompletado
			-- LEFT JOIN InventarioLinea il2 ON il2.LineaID = ilp.LineaID AND il2.SucursalID = s.sucursalID
			-- 	AND ((il2.EstatusGenericoID = @EstGenEnCurso AND il2.AvPeriodicidad IS NULL) OR il2.EstatusGenericoID = @EstGenContado)
			-- Para sacar las líneas sucursal que están en curso o en esta vuelta ya fueron procesadas
			LEFT JOIN InventarioLinea il2 ON il2.LineaID = ilp.LineaID AND il2.SucursalID = s.SucursalID
				AND (il2.EstatusGenericoID = @EstGenEnCurso OR (il2.EstatusGenericoID = @EstGenCompletado AND
				il2.AvPeriodicidad >= (SELECT MAX(AvPeriodicidad) FROM InventarioLinea)))
		WHERE il2.InventarioLineaID IS NULL
		GROUP BY ilp.LineaID, ilp.Periodicidad, s.SucursalID
		HAVING (DATEDIFF(D, MAX(il.FechaCompletado), @Hoy) - ilp.Periodicidad) >= 0
		ORDER BY (DATEDIFF(D, MAX(il.FechaCompletado), @Hoy) - ilp.Periodicidad) DESC

	-- Se comienza el proceso de asignación, primero por sucursal
	DECLARE @SucursalID INT = 0
	DECLARE @UsuarioID INT = 0
	DECLARE @CantidadUsuario INT
	DECLARE @LineaID INT
	DECLARE @InvLineaId INT
	DECLARE @Insertados INT
	DECLARE @ComConteo NVARCHAR(1024)
	DECLARE @EsPeriod BIT
	DECLARE @Vuelta INT

	WHILE 1 = 1 BEGIN
		-- Se obtiene la sucursal
		SET @SucursalID = (SELECT TOP 1 SucursalID FROM Sucursal WHERE SucursalID > @SucursalID AND Estatus = 1 ORDER BY SucursalID)
		IF @SucursalID IS NULL BEGIN BREAK END
		-- Se verifica si está habilitada
		IF EXISTS(SELECT 1 FROM Configuracion WHERE Nombre = @ConfigHabilitar AND SucursalID = @SucursalID AND Valor != 'V') BEGIN
			CONTINUE
		END
		-- Se obtiene la vuelta correspondiente a la sucursal
		SET @Vuelta = (SELECT ISNULL(MAX(AvVuelta), 1) FROM InventarioLinea WHERE SucursalID = @SucursalID)
		--
		UPDATE @Lineas SET Procesada = 0
		-- Ciclo de Usuario
		WHILE 1 = 1 BEGIN
			SELECT TOP 1
				@UsuarioID = InvUsuarioID
				, @CantidadUsuario = ArticulosDiarios
			FROM InventarioUsuario
			WHERE SucursalID = @SucursalID AND InvUsuarioID > @UsuarioID
			ORDER BY InvUsuarioID
		
			IF @@ROWCOUNT = 0 BEGIN BREAK END
		
			WHILE @CantidadUsuario > 0 BEGIN

				-- Se obtiene la línea actual
				SET @LineaID = (SELECT TOP 1 LineaID FROM @Lineas WHERE Procesada = 0)
				-- Si la línea es NULL, significa que ya se acabó la vuelta
				IF @LineaID IS NULL BEGIN
					-- SET @Vuelta = (SELECT TOP 1 Id FROM InventarioVuelta WHERE Id > @Vuelta ORDER BY Id)
					-- IF @Vuelta IS NULL BEGIN
					-- 	INSERT INTO InventarioVuelta (FechaInicio) VALUES (@Hoy)
					--	SET @Vuelta = @@IDENTITY
					-- END
					SET @Vuelta = @Vuelta + 1
					-- Para asignar la línea
					UPDATE @Lineas SET Procesada = 0
					SET @LineaID = (SELECT TOP 1 LineaID FROM @Lineas WHERE Procesada = 0)
				END

				SET @EsPeriod = 0

				-- Se obtiene el InvetarioLineaId correspondiente a este conteo, si existe, si no, se crea
				SET @InvLineaId = (SELECT InventarioLineaID FROM InventarioLinea
					WHERE SucursalID = @SucursalID AND AvVuelta = @Vuelta AND LineaID = @LineaID)
				IF @InvLineaId IS NULL BEGIN
				
					-- Para lo de la periodicidad
					IF EXISTS(SELECT TOP 1 1 FROM @Periodicidad WHERE SucursalID = @SucursalID AND Procesada = 0) BEGIN
						SET @LineaID = (SELECT TOP 1 LineaID FROM @Periodicidad WHERE SucursalID = @SucursalID AND Procesada = 0)
						SET @InvLineaId = (SELECT InventarioLineaID FROM InventarioLinea
							WHERE SucursalID = @SucursalID AND LineaID = @LineaID AND AvPeriodicidad > 0 AND EstatusGenericoID != @EstGenCompletado)
						IF @InvLineaId IS NULL BEGIN
							DECLARE @AvPerSig INT = (SELECT (ISNULL(MAX(AvPeriodicidad), 0) + 1) FROM InventarioLinea
								WHERE SucursalID = @SucursalID)
							INSERT INTO InventarioLinea (SucursalID, LineaID, EstatusGenericoID, FechaIniciado, AvPeriodicidad)
								VALUES (@SucursalID, @LineaID, @EstGenEnCurso, @Ahora, @AvPerSig)
							SET @InvLineaId = @@IDENTITY
						END
						SET @EsPeriod = 1
					END
					--

					ELSE BEGIN
						INSERT INTO InventarioLinea (SucursalID, LineaID, EstatusGenericoID, AvVuelta, FechaIniciado)
							VALUES (@SucursalID, @LineaID, @EstGenEnCurso, @Vuelta, @Ahora)
						SET @InvLineaId = @@IDENTITY
					END
				END ELSE BEGIN
					-- Se valida si ya se procesó esa línea, para no volverlo a ejecutar
					IF EXISTS(SELECT 1 FROM InventarioLinea WHERE InventarioLineaID = @InvLineaId AND EstatusGenericoID != @EstGenEnCurso) BEGIN
						UPDATE @Lineas SET Procesada = 1 WHERE LineaID = @LineaID
						CONTINUE
					END
				END

				-- Se insertan las partes para el conteo
				SET @ComConteo = N'
					SELECT TOP ' + CONVERT(NVARCHAR(4), @CantidadUsuario) + '
						@InvLineaId
						, @Hoy
						, @UsuarioID
						, p.ParteID
						-- , 0
					FROM
						Parte p
						INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
						LEFT JOIN (
							SELECT DISTINCT ic.ParteID
							FROM
								InventarioConteo ic
								INNER JOIN InventarioUsuario iu ON iu.InvUsuarioID = ic.ConteoUsuarioID AND iu.SucursalID = @SucursalID
							WHERE ic.InventarioLineaId = @InvLineaId
						) icc ON icc.ParteID = p.ParteID
						-- LEFT JOIN InventarioUsuario iu ON iu.SucursalID = @SucursalID
						-- LEFT JOIN InventarioConteo ic ON ic.ParteID = p.ParteID AND ic.ConteoUsuarioID = iu.InvUsuarioID
						-- 	AND ic.InventarioLineaId = @InvLineaId
					WHERE
						p.Estatus = 1
						AND p.LineaID = @LineaID
						AND pe.Existencia > 0
						AND icc.ParteId IS NULL
					'
				INSERT INTO InventarioConteo (InventarioLineaId, Dia, ConteoUsuarioID, ParteID)
					EXEC sp_executesql @ComConteo
					, N'@CantidadUsuario INT, @InvLineaId INT, @Hoy DATETIME, @UsuarioID INT, @SucursalID INT, @LineaID INT'
					, @CantidadUsuario = @CantidadUsuario
					, @InvLineaID = @InvLineaID
					, @Hoy = @Hoy
					, @UsuarioID = @UsuarioID
					, @SucursalID = @SucursalID
					, @LineaID = @LineaID
				SET @Insertados = @@ROWCOUNT

				SET @CantidadUsuario = (@CantidadUsuario - @Insertados)

				-- Si @@RowCount es cero, significa que ya se procesaron todas las partes de la línea
				IF @Insertados = 0 BEGIN
					IF @EsPeriod = 1 BEGIN
						UPDATE @Periodicidad SET Procesada = 1 WHERE SucursalID = @SucursalID AND LineaID = @LineaID
					END ELSE BEGIN
					
						UPDATE @Lineas SET Procesada = 1 WHERE LineaID = @LineaID
					
					END
				END
			END
		END
	END

	-- Se verifica si ya se completaron los conteos de todas las sucursales, para cambiar estatus
	/* Revisar si realmente esto será necesario aquí en el procedimiento
	SET @InvLineaId = 0
	WHILE 1 = 1 BEGIN
		SELECT TOP 1
			@InvLineaId = InventarioLineaID
			, @SucursalID = SucursalID
			, @LineaID = LineaID
		FROM InventarioLinea
		WHERE EstatusGenericoID = @EstGenEnCurso AND InventarioLineaID > @InvLineaId
		ORDER BY InventarioLineaID
		IF @@ROWCOUNT = 0 BEGIN BREAK END

		IF EXISTS(
			SELECT 1 FROM (
				SELECT
					-- , COUNT(DISTINCT p.ParteID) AS Partes
					-- , COUNT(ic.ParteID) AS Contadas
					(COUNT(DISTINCT p.ParteID) - COUNT(ic.ParteID)) AS Diferencia
				FROM
					Parte p
					INNER JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
					LEFT JOIN InventarioUsuario iu ON iu.SucursalID = @SucursalID
					LEFT JOIN InventarioConteo ic ON ic.ParteID = p.ParteID AND ic.ConteoUsuarioID = iu.InvUsuarioID
						AND ic.InventarioLineaId = @InvLineaId AND ic.Diferencia IS NOT NULL AND ic.AplicaRevision = 0
				WHERE
					p.Estatus = 1
					AND p.LineaID = @LineaID
					AND pe.Existencia > 0
				-- GROUP BY s.SucursalID
			) c
			HAVING SUM(Diferencia) <= 0
		) BEGIN
			UPDATE InventarioLinea SET EstatusGenericoID = @EstGenContado WHERE InventarioLineaID = @InvLineaId
		END
	END
	*/

	-- Se insertan los conteos por devoluciones y garantías
	DECLARE @Sucursales tpuTablaEnteros
	INSERT INTO @Sucursales
		SELECT DISTINCT s.SucursalID
		FROM
			Sucursal s
			INNER JOIN Configuracion c ON c.Nombre = @ConfigHabilitar AND c.Valor = 'V'
		WHERE s.Estatus = 1
	-- Se obtiene las partes
	DECLARE @Partes TABLE (ParteID INT, SucursalID INT)
	INSERT INTO @Partes
		SELECT DISTINCT
			vdd.ParteID
			, vd.SucursalID
		FROM
			VentaDevolucionDetalle vdd
			INNER JOIN VentaDevolucion vd ON vd.VentaDevolucionID = vdd.VentaDevolucionID AND vd.Estatus = 1
		WHERE
			vdd.Estatus = 1
			AND CONVERT(DATE, vd.Fecha) = @Hoy
		UNION
		SELECT DISTINCT
			vg.ParteID
			, vg.SucursalID
		FROM VentaGarantia vg
		WHERE
			vg.Estatus = 1
			AND CONVERT(DATE, vg.Fecha) = @Hoy
	DECLARE @PartesProc TABLE (ParteID INT, SucursalID INT)
	-- Se obtienen los usuarios
	DECLARE @Usuarios TABLE (UsuarioID INT, SucursalID INT, Conteos INT)
	INSERT INTO @Usuarios
		SELECT
			iu.InvUsuarioID AS UsuarioID
			, iu.SucursalID
			, ISNULL(CEILING(ps.Partes / (us.Usuarios * 1.0)), 0.0) AS Conteos
		FROM
			InventarioUsuario iu
			LEFT JOIN (
				SELECT SucursalID, COUNT(*) AS Usuarios
				FROM InventarioUsuario
				GROUP BY SucursalID
			) us ON us.SucursalID = iu.SucursalID
			LEFT JOIN (
				SELECT SucursalID, COUNT(*) AS Partes
				FROM @Partes
				GROUP BY SucursalID
			) ps ON ps.SucursalID = iu.SucursalID

	-- Ciclo de Sucursal
	SET @SucursalID = 0
	SET @UsuarioID = 0
	WHILE 1 = 1 BEGIN
		-- Se obtiene la sucursal
		SET @SucursalID = (SELECT TOP 1 Entero FROM @Sucursales WHERE Entero > @SucursalID ORDER BY Entero)
		IF @SucursalID IS NULL BEGIN BREAK END
		-- Se inserta el encabezado del conteo, 
		IF EXISTS(SELECT TOP 1 1 FROM @Partes WHERE SucursalID = @SucursalID) BEGIN
			INSERT INTO InventarioLinea (SucursalID, EstatusGenericoID, FechaIniciado) VALUES
				(@SucursalID, @EstGenEnCurso, @Ahora)
			SET @InvLineaId = @@IDENTITY
		END
		-- Ciclo de Usuario
		WHILE 1 = 1 BEGIN
			SELECT TOP 1
				@UsuarioID = UsuarioID
				, @CantidadUsuario = Conteos
			FROM @Usuarios
			WHERE SucursalID = @SucursalID AND UsuarioID > @UsuarioID
			ORDER BY UsuarioID
			-- IF @UsuarioID IS NULL BEGIN BREAK END
			IF @@ROWCOUNT = 0 BEGIN BREAK END
		
			INSERT INTO @PartesProc
				SELECT TOP (@CantidadUsuario) ParteID, SucursalID
				FROM @Partes
				WHERE SucursalID = @SucursalID
			INSERT INTO InventarioConteo
			SELECT
				@InvLineaId
				, @Maniana
				, @UsuarioID
				, ParteID
				, NULL, NULL, NULL, NULL
			FROM @PartesProc

			DELETE p FROM @Partes p INNER JOIN @PartesProc pp ON pp.ParteID = p.ParteID AND pp.SucursalID = p.SucursalID
			DELETE FROM @PartesProc

		END
	END

END
GO