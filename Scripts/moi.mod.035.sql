/* Script con modificaciones a la base de datos de Theos. Archivo 035
 * Creado: 2015/09/15
 * Subido: 2015/09/18
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE ProveedorParteGanancia ADD Observacion NVARCHAR(1024) NULL

ALTER TABLE VentaFacturaDevolucion ADD
	Subtotal DECIMAL(12, 2) NULL
	, Iva DECIMAL(12, 2) NULL
GO
UPDATE VentaFacturaDevolucion SET Subtotal = 0.0, Iva = 0.0
ALTER TABLE VentaFacturaDevolucion ALTER COLUMN	Subtotal DECIMAL(12, 2) NOT NULL
ALTER TABLE VentaFacturaDevolucion ALTER COLUMN Iva DECIMAL(12, 2) NOT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW ProveedoresPartesGananciasView AS
	/* Método anterior, no funciona porque se duplican los registros cuando ya hay datos en la tabla de DescuentosGanancias
	SELECT
		ROW_NUMBER() OVER (ORDER BY pv.ProveedorID, mp.MarcaParteID, l.LineaID, p.ParteID) AS Registro
		, pv.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, mp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, l.LineaID
		, l.NombreLinea AS Linea
		, p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, ppg.DescuentoFactura1
		, ppg.DescuentoFactura2
		, ppg.DescuentoFactura3
		, ppg.DescuentoArticulo1
		, ppg.DescuentoArticulo2
		, ppg.DescuentoArticulo3
		, ppg.PorcentajeDeGanancia1
		, ppg.PorcentajeDeGanancia2
		, ppg.PorcentajeDeGanancia3
		, ppg.PorcentajeDeGanancia4
		, ppg.PorcentajeDeGanancia5
	FROM
		Proveedor pv
		LEFT JOIN ProveedorMarcaParte pmp ON pmp.ProveedorID = pv.ProveedorID AND pmp.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pmp.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN LineaMarcaParte lmp ON lmp.MarcaParteID = mp.MarcaParteID AND lmp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = lmp.LineaID AND l.Estatus = 1
		LEFT JOIN Parte p ON p.LineaID = l.LineaID AND p.Estatus = 1
		LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = pv.ProveedorID
			AND (ppg.MarcaParteID IS NULL OR ppg.MarcaParteID = mp.MarcaParteID)
			AND (ppg.LineaID IS NULL OR ppg.LineaID = l.LineaID)
			AND (ppg.ParteID IS NULL OR ppg.ParteID = p.ParteID)
	WHERE pv.Estatus = 1
	/ * ORDER BY
		pv.NombreProveedor
		, mp.NombreMarcaParte
		, l.LineaID
		, p.NombreParte
	* /
	*/
	
	WITH _Proveedores AS (
		SELECT
			ppg.ProveedorParteGananciaID
			, pv.ProveedorID
			, 0 AS MarcaID
			, 0 AS LineaID
			, 0 AS ParteID
			, pv.NombreProveedor AS Proveedor
			, NULL AS Marca
			, NULL AS Linea
			, NULL AS Parte			
			, ppg.DescuentoFactura1
			, ppg.DescuentoFactura2
			, ppg.DescuentoFactura3
			, ppg.DescuentoArticulo1
			, ppg.DescuentoArticulo2
			, ppg.DescuentoArticulo3
			, ppg.PorcentajeDeGanancia1
			, ppg.PorcentajeDeGanancia2
			, ppg.PorcentajeDeGanancia3
			, ppg.PorcentajeDeGanancia4
			, ppg.PorcentajeDeGanancia5
			, ppg.Observacion
		FROM
			Proveedor pv
			LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = pv.ProveedorID
				AND ppg.MarcaParteID IS NULL AND ppg.LineaID IS NULL AND ppg.ParteID IS NULL
		WHERE pv.Estatus = 1
	), _Marcas AS (
		SELECT
			ppg.ProveedorParteGananciaID
			, pmp.ProveedorID
			, pmp.MarcaParteID AS MarcaID
			, 0 AS LineaID
			, 0 AS ParteID
			, pv.Proveedor
			, mp.NombreMarcaParte AS Marca
			, NULL AS Linea
			, NULL AS Parte
			, ppg.DescuentoFactura1
			, ppg.DescuentoFactura2
			, ppg.DescuentoFactura3
			, ppg.DescuentoArticulo1
			, ppg.DescuentoArticulo2
			, ppg.DescuentoArticulo3
			, ppg.PorcentajeDeGanancia1
			, ppg.PorcentajeDeGanancia2
			, ppg.PorcentajeDeGanancia3
			, ppg.PorcentajeDeGanancia4
			, ppg.PorcentajeDeGanancia5
			, ppg.Observacion
		FROM
			ProveedorMarcaParte pmp
			INNER JOIN _Proveedores pv ON pv.ProveedorID = pmp.ProveedorID
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pmp.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = pmp.ProveedorID
				AND ppg.MarcaParteID = pmp.MarcaParteID AND ppg.LineaID IS NULL AND ppg.ParteID IS NULL
		WHERE pmp.Estatus = 1
	), _Lineas AS (
		SELECT
			ppg.ProveedorParteGananciaID
			, mp.ProveedorID
			, mp.MarcaID
			, lmp.LineaID
			, 0 AS ParteID
			, mp.Proveedor
			, mp.Marca
			, l.NombreLinea AS Linea
			, NULL AS Parte
			, ppg.DescuentoFactura1
			, ppg.DescuentoFactura2
			, ppg.DescuentoFactura3
			, ppg.DescuentoArticulo1
			, ppg.DescuentoArticulo2
			, ppg.DescuentoArticulo3
			, ppg.PorcentajeDeGanancia1
			, ppg.PorcentajeDeGanancia2
			, ppg.PorcentajeDeGanancia3
			, ppg.PorcentajeDeGanancia4
			, ppg.PorcentajeDeGanancia5
			, ppg.Observacion
		FROM
			LineaMarcaParte lmp
			INNER JOIN _Marcas mp ON mp.MarcaID = lmp.MarcaParteID
			LEFT JOIN Linea l ON l.LineaID = lmp.LineaID AND l.Estatus = 1
			LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = mp.ProveedorID
				AND ppg.MarcaParteID = lmp.MarcaParteID AND ppg.LineaID = lmp.LineaID AND ppg.ParteID IS NULL
		WHERE lmp.Estatus = 1
	), _Partes AS (
		SELECT
			ppg.ProveedorParteGananciaID
			, l.ProveedorID
			, l.MarcaID
			, l.LineaID
			, p.ParteID
			, l.Proveedor
			, l.Marca
			, l.Linea
			, p.NombreParte AS Parte
			, ppg.DescuentoFactura1
			, ppg.DescuentoFactura2
			, ppg.DescuentoFactura3
			, ppg.DescuentoArticulo1
			, ppg.DescuentoArticulo2
			, ppg.DescuentoArticulo3
			, ppg.PorcentajeDeGanancia1
			, ppg.PorcentajeDeGanancia2
			, ppg.PorcentajeDeGanancia3
			, ppg.PorcentajeDeGanancia4
			, ppg.PorcentajeDeGanancia5
			, ppg.Observacion
		FROM
			Parte p
			INNER JOIN _Lineas l ON l.LineaID = p.LineaID
			LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = l.ProveedorID
				AND ppg.MarcaParteID = l.MarcaID AND ppg.LineaID = l.LineaID AND ppg.ParteID = p.ParteID
		WHERE p.Estatus = 1
	)
	
	SELECT * FROM _Proveedores
	UNION
	SELECT * FROM _Marcas
	UNION
	SELECT * FROM _Lineas
	UNION
	SELECT * FROM _Partes
	-- ORDER BY Proveedor, Marca, Linea, Parte
GO

-- DROP VIEW ProveedoresPartesGananciasLineasView
CREATE VIEW ProveedoresPartesGananciasLineasView AS
	WITH _Proveedores AS (
		SELECT
			ppg.ProveedorParteGananciaID
			, pv.ProveedorID
			, 0 AS MarcaID
			, 0 AS LineaID
			, 0 AS ParteID
			, pv.NombreProveedor AS Proveedor
			, NULL AS Marca
			, NULL AS Linea
			, NULL AS Parte			
			, ppg.DescuentoFactura1
			, ppg.DescuentoFactura2
			, ppg.DescuentoFactura3
			, ppg.DescuentoArticulo1
			, ppg.DescuentoArticulo2
			, ppg.DescuentoArticulo3
			, ppg.PorcentajeDeGanancia1
			, ppg.PorcentajeDeGanancia2
			, ppg.PorcentajeDeGanancia3
			, ppg.PorcentajeDeGanancia4
			, ppg.PorcentajeDeGanancia5
			, ppg.Observacion
		FROM
			Proveedor pv
			LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = pv.ProveedorID
				AND ppg.MarcaParteID IS NULL AND ppg.LineaID IS NULL AND ppg.ParteID IS NULL
		WHERE pv.Estatus = 1
	), _Marcas AS (
		SELECT
			ppg.ProveedorParteGananciaID
			, pmp.ProveedorID
			, pmp.MarcaParteID AS MarcaID
			, 0 AS LineaID
			, 0 AS ParteID
			, pv.Proveedor
			, mp.NombreMarcaParte AS Marca
			, NULL AS Linea
			, NULL AS Parte
			, ppg.DescuentoFactura1
			, ppg.DescuentoFactura2
			, ppg.DescuentoFactura3
			, ppg.DescuentoArticulo1
			, ppg.DescuentoArticulo2
			, ppg.DescuentoArticulo3
			, ppg.PorcentajeDeGanancia1
			, ppg.PorcentajeDeGanancia2
			, ppg.PorcentajeDeGanancia3
			, ppg.PorcentajeDeGanancia4
			, ppg.PorcentajeDeGanancia5
			, ppg.Observacion
		FROM
			ProveedorMarcaParte pmp
			INNER JOIN _Proveedores pv ON pv.ProveedorID = pmp.ProveedorID
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pmp.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = pmp.ProveedorID
				AND ppg.MarcaParteID = pmp.MarcaParteID AND ppg.LineaID IS NULL AND ppg.ParteID IS NULL
		WHERE pmp.Estatus = 1
	), _Lineas AS (
		SELECT
			ppg.ProveedorParteGananciaID
			, mp.ProveedorID
			, mp.MarcaID
			, lmp.LineaID
			, 0 AS ParteID
			, mp.Proveedor
			, mp.Marca
			, l.NombreLinea AS Linea
			, NULL AS Parte
			, ppg.DescuentoFactura1
			, ppg.DescuentoFactura2
			, ppg.DescuentoFactura3
			, ppg.DescuentoArticulo1
			, ppg.DescuentoArticulo2
			, ppg.DescuentoArticulo3
			, ppg.PorcentajeDeGanancia1
			, ppg.PorcentajeDeGanancia2
			, ppg.PorcentajeDeGanancia3
			, ppg.PorcentajeDeGanancia4
			, ppg.PorcentajeDeGanancia5
			, ppg.Observacion
		FROM
			LineaMarcaParte lmp
			INNER JOIN _Marcas mp ON mp.MarcaID = lmp.MarcaParteID
			LEFT JOIN Linea l ON l.LineaID = lmp.LineaID AND l.Estatus = 1
			LEFT JOIN ProveedorParteGanancia ppg ON ppg.ProveedorID = mp.ProveedorID
				AND ppg.MarcaParteID = lmp.MarcaParteID AND ppg.LineaID = lmp.LineaID AND ppg.ParteID IS NULL
		WHERE lmp.Estatus = 1
	)
	SELECT * FROM _Proveedores
	UNION
	SELECT * FROM _Marcas
	UNION
	SELECT * FROM _Lineas
GO

CREATE VIEW ProveedoresMarcasLineasView AS
	SELECT
		pmp.ProveedorMarcaParteID
		, lmp.LineaMarcaParteID
		, pmp.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, pmp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, lmp.LineaID
		, l.NombreLinea AS Linea
		, (l.NombreLinea + ' ' + mp.NombreMarcaParte) AS LineaMarca
	FROM
		ProveedorMarcaParte pmp
		INNER JOIN LineaMarcaParte lmp ON lmp.MarcaParteID = pmp.MarcaParteID AND lmp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = pmp.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pmp.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = lmp.LineaID AND l.Estatus = 1
	WHERE pmp.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauPedidosSugeridos] (
	@SucursalID NVARCHAR(10)
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
				-- Se agregan filtro de ventas recientes, para artículos con ABC de C en adelante
				-- LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
				-- LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.VentaEstatusID = @EstVentaPagada 
				-- 	AND DATEDIFF(DAY, v.Fecha, GETDATE()) <= 1 AND v.Estatus = 1
			WHERE
				p.Estatus = 1 
				AND pmm.Maximo > 0
				-- AND pe.Existencia <= pmm.Minimo
				AND p.ParteID NOT IN (SELECT PedidoDetalle.ParteID FROM PedidoDetalle WHERE PedidoDetalle.Estatus = 1 AND PedidoDetalle.PedidoEstatusID = 2)	
				AND pe.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))
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
				AND (ParteAbc.AbcDeVentas IN ('A', 'B', 'C') OR NOT EXISTS(
					SELECT 1
					FROM
						VentaDetalle vd
						INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.VentaEstatusID = @EstVentaPagada
							AND DATEDIFF(DAY, v.Fecha, GETDATE()) <= 1 AND v.Estatus = 1
					WHERE vd.Estatus = 1
					))
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
		AND c9.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
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
		AND rf.SucursalID IN (SELECT * FROM dbo.fnuDividirCadena(@SucursalID, ','))	
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
		, mp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
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
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta)
			THEN ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad)
			ELSE 0.0 END) AS PrecioActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt)
			THEN ((vd.PrecioUnitario + vd.Iva) * vd.Cantidad)
			ELSE 0.0 END) AS PrecioAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta)
			THEN (vd.Costo * vd.Cantidad) ELSE 0.0 END) AS CostoActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt)
			THEN (vd.Costo * vd.Cantidad) ELSE 0.0 END) AS CostoAnterior
		, SUM(CASE WHEN (vpc.Fecha >= @Desde AND vpc.Fecha < @Hasta)
			THEN (vd.CostoConDescuento * vd.Cantidad) ELSE 0.0 END) AS CostoDescActual
		, SUM(CASE WHEN (vpc.Fecha >= @DesdeAnt AND vpc.Fecha < @HastaAnt)
			THEN (vd.CostoConDescuento * vd.Cantidad) ELSE 0.0 END) AS CostoDescAnterior
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
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
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
		, mp.MarcaParteID
		, mp.NombreMarcaParte
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
		, mp.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
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
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			THEN ((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad)
			ELSE 0.0 END) * -1) AS PrecioActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
			THEN ((vdd.PrecioUnitario + vdd.Iva) * vdd.Cantidad)
			ELSE 0.0 END) * -1) AS PrecioAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta) 
			THEN (vdd.Costo * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
			THEN (vdd.Costo * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoAnterior
		, (SUM(CASE WHEN (vd.Fecha >= @Desde AND vd.Fecha < @Hasta)
			THEN (vdd.CostoConDescuento * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoDescActual
		, (SUM(CASE WHEN (vd.Fecha >= @DesdeAnt AND vd.Fecha < @HastaAnt)
			THEN (vdd.CostoConDescuento * vdd.Cantidad) ELSE 0.0 END) * -1) AS CostoDescAnterior
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
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
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
		, mp.MarcaParteID
		, mp.NombreMarcaParte
		, pv.ProveedorID
		, pv.NombreProveedor

	/* ORDER BY
		DATEPART(DAYOFYEAR, vp.Fecha)
		, DATEPART(HOUR, vp.Fecha)
	*/

END
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
		, SUM(vfd.Subtotal + vfd.Iva) AS Facturas
		, SUM(vfd.Subtotal + vfd.Iva) AS Total
		, 0 AS Pendientes
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
END
GO

ALTER PROCEDURE [dbo].[pauCajaDetalleCorte] (
	@OpcionID INT
	, @SucursalID INT
	, @Dia DATE
	, @DevDiasAnt BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @OpcionID INT = 1
	DECLARE @SucursalID INT = 1
	DECLARE @Dia DATE = '2015-02-09'
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
				-- Se marcan para sacar las devoluciones y garantías de días anteriores
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
		-- Se agregan las ventas de días anteriores que fueron facturadas
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
					-- Se agrega el importe correspondiente a las ventas de días anteriores
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
				'CRÉDITO'
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