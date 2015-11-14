/* Script con modificaciones a la base de datos de Theos. Archivo 048
 * Creado: 2015/11/11
 * Subido: 2015/11/14
 */

DECLARE @ScriptID INT = 48
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE ScriptSql (
	ScriptID INT NOT NULL PRIMARY KEY
	, Fecha DATETIME NOT NULL DEFAULT GETDATE()
	, SubidoPor NVARCHAR(8) NOT NULL
	, Observacion NVARCHAR(512) NULL
)
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)

ALTER TABLE ParteVehiculo DROP CONSTRAINT DF_ParteVehiculo_Estatus, DF_ParteVehiculo_Actualizar
DROP INDEX Ix_Estatus ON ParteVehiculo
DROP INDEX Ix_ParteID ON ParteVehiculo
DROP INDEX Ix_SegunPlan_BusquedaVenta ON ParteVehiculo
ALTER TABLE ParteVehiculo DROP COLUMN Estatus, Actualizar
CREATE INDEX Ix_ParteEquivalente_GrupoID ON ParteEquivalente(GrupoID) INCLUDE(ParteID)
CREATE INDEX Ix_ParteID_ModeloID_Anio_MotorID ON ParteVehiculo(ParteID, ModeloID, Anio, MotorID)

ALTER TABLE ParteCodigoAlterno DROP CONSTRAINT DF_ParteCodigoAlterno_Estatus, DF_ParteCodigoAlterno_Actualizar
DROP INDEX Ix_Estatus ON ParteCodigoAlterno
DROP INDEX Ix_CodigoAlterno ON ParteCodigoAlterno
DROP INDEX Ix_ParteID ON ParteCodigoAlterno
ALTER TABLE ParteCodigoAlterno DROP COLUMN Estatus, Actualizar

DROP INDEX Ix_PedidoDetalle_PedidoDetalleID ON PedidoDetalle
CREATE INDEX Ix_Estatus_PedidoEstatusID ON PedidoDetalle(Estatus, PedidoEstatusID) INCLUDE (PedidoDetalleID, ParteID)

CREATE INDEX Ix_ParteID ON ParteAbc(ParteID) INCLUDE (AbcDeVentas)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

ALTER VIEW [dbo].[PartesAvancesView] AS
	SELECT
		pc.*
		, pv.NombreProveedor AS Proveedor
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
	FROM
		(
			SELECT
				p.ParteID
				, p.NumeroParte AS NumeroDeParte
				, p.NombreParte AS Descripcion
				, p.ProveedorID
				, p.MarcaParteID
				, p.LineaID
				, p.FechaRegistro
				, (SELECT 0) AS Fotos
				-- , (SELECT TOP 1 1 FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS Fotos
				, (SELECT TOP 1 1 FROM ParteEquivalente WHERE GrupoID = (SELECT GrupoID FROM ParteEquivalente
					WHERE ParteID = p.ParteID)) AS Equivalentes
				, (SELECT TOP 1 1 FROM ParteVehiculo WHERE ParteID = p.ParteID AND Estatus = 1) AS Aplicaciones
				, (SELECT TOP 1 1 FROM ParteCodigoAlterno WHERE ParteID = p.ParteID) AS Alternos
				, (SELECT TOP 1 1 FROM ParteComplementaria WHERE ParteID = p.ParteID) AS Complementarios
				, (SELECT TOP 1 1 FROM ParteCaracteristica WHERE ParteID = p.ParteID) AS Caracteristicas
			FROM Parte p
			WHERE p.Estatus = 1
		) pc
		LEFT JOIN Proveedor pv ON pv.ProveedorID = pc.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pc.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = pc.LineaID AND l.Estatus = 1
GO

ALTER VIEW [dbo].[PartesCodigosAlternosView] AS
	SELECT
		ParteCodigoAlterno.ParteCodigoAlternoID,
		ParteCodigoAlterno.ParteID,
		ParteCodigoAlterno.MarcaParteID,		
		MarcaParte.NombreMarcaParte,
		MarcaParte.Abreviacion AS MarcaAbreviacion,
		ParteCodigoAlterno.CodigoAlterno
	FROM
		ParteCodigoAlterno
		INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = ParteCodigoAlterno.MarcaParteID
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

ALTER PROCEDURE pauVentasPartesBusqueda (
	@SucursalID INT
	, @Codigo NVARCHAR(32) = NULL
	, @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
	, @SistemaID INT = NULL
	, @LineaID INT = NULL
	, @MarcaID INT = NULL
	
	, @Caracteristicas BIT = 0
	, @Car01ID INT = NULL
	, @Car01 NVARCHAR(64) = NULL
	, @Car02ID INT = NULL
	, @Car02 NVARCHAR(64) = NULL
	, @Car03ID INT = NULL
	, @Car03 NVARCHAR(64) = NULL
	, @Car04ID INT = NULL
	, @Car04 NVARCHAR(64) = NULL
	, @Car05ID INT = NULL
	, @Car05 NVARCHAR(64) = NULL
	, @Car06ID INT = NULL
	, @Car06 NVARCHAR(64) = NULL
	, @Car07ID INT = NULL
	, @Car07 NVARCHAR(64) = NULL
	, @Car08ID INT = NULL
	, @Car08 NVARCHAR(64) = NULL
	, @Car09ID INT = NULL
	, @Car09 NVARCHAR(64) = NULL
	, @Car10ID INT = NULL
	, @Car10 NVARCHAR(64) = NULL
	, @Car11ID INT = NULL
	, @Car11 NVARCHAR(64) = NULL
	, @Car12ID INT = NULL
	, @Car12 NVARCHAR(64) = NULL
	, @Car13ID INT = NULL
	, @Car13 NVARCHAR(64) = NULL
	, @Car14ID INT = NULL
	, @Car14 NVARCHAR(64) = NULL
	, @Car15ID INT = NULL
	, @Car15 NVARCHAR(64) = NULL
	, @Car16ID INT = NULL
	, @Car16 NVARCHAR(64) = NULL
	, @Car17ID INT = NULL
	, @Car17 NVARCHAR(64) = NULL

	, @CodigoAlterno NVARCHAR(32) = NULL

	, @VehiculoModeloID INT = NULL -- Se debe incluir el ModeloID para que el filtro por vehículo tenga efecto
	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL
	
	, @Equivalentes BIT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4
	DECLARE @ParteEstActivo INT = 1

	-- Se verifica si es por código o 
	DECLARE @Partes TABLE (ParteID INT NOT NULL)
	IF @Codigo IS NULL BEGIN
		-- Se hace el filtro inicial
		INSERT INTO @Partes
		SELECT TOP 201 p.ParteID
		FROM
			Parte p
			LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
			LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID -- AND pca.Estatus = 1
			-- LEFT JOIN ParteVehiculo pv ON pv.ParteID = p.ParteID
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		WHERE
			p.Estatus = 1
			AND ParteEstatusID = @ParteEstActivo
			AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
			/* AND (@SistemaID IS NULL OR p.SubsistemaID IN (
				SELECT SubsistemaID
				FROM Subsistema
				WHERE SistemaID = @SistemaID AND Estatus = 1
			))
			*/
			AND (@SistemaID IS NULL OR ss.SistemaID = @SistemaID)
			AND (@LineaID IS NULL OR p.LineaID = @LineaID)
			AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
			/* AND (@CodigoAlterno IS NULL OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteCodigoAlterno
				WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
			))
			*/
			AND (@CodigoAlterno IS NULL OR pca.CodigoAlterno LIKE '%' + @CodigoAlterno + '%')
			AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteVehiculo
				WHERE
					ModeloID = @VehiculoModeloID
					AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
					AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
					-- AND TipoFuenteID != @IdTipoFuenteMostrador
			))
			/* AND (@VehiculoModeloID IS NULL OR pv.ModeloID = @VehiculoModeloID)
			AND (@VehiculoAnio IS NULL OR pv.Anio = @VehiculoAnio)
			AND (@VehiculoMotorID IS NULL OR pv.MotorID = @VehiculoMotorID)
			*/
			
			AND (@Caracteristicas = 0 OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteCaracteristica
				WHERE
					(@Car01 IS NULL OR CaracteristicaID = @Car01ID AND Valor = @Car01)
					OR (@Car02 IS NULL OR CaracteristicaID = @Car02ID AND Valor = @Car02)
					OR (@Car03 IS NULL OR CaracteristicaID = @Car03ID AND Valor = @Car03)
					OR (@Car04 IS NULL OR CaracteristicaID = @Car04ID AND Valor = @Car04)
					OR (@Car05 IS NULL OR CaracteristicaID = @Car05ID AND Valor = @Car05)
					OR (@Car06 IS NULL OR CaracteristicaID = @Car06ID AND Valor = @Car06)
					OR (@Car07 IS NULL OR CaracteristicaID = @Car07ID AND Valor = @Car07)
					OR (@Car08 IS NULL OR CaracteristicaID = @Car08ID AND Valor = @Car08)
					OR (@Car09 IS NULL OR CaracteristicaID = @Car09ID AND Valor = @Car09)
					OR (@Car10 IS NULL OR CaracteristicaID = @Car10ID AND Valor = @Car10)
					OR (@Car11 IS NULL OR CaracteristicaID = @Car11ID AND Valor = @Car11)
					OR (@Car12 IS NULL OR CaracteristicaID = @Car12ID AND Valor = @Car12)
					OR (@Car13 IS NULL OR CaracteristicaID = @Car13ID AND Valor = @Car13)
					OR (@Car14 IS NULL OR CaracteristicaID = @Car14ID AND Valor = @Car14)
					OR (@Car15 IS NULL OR CaracteristicaID = @Car15ID AND Valor = @Car15)
					OR (@Car16 IS NULL OR CaracteristicaID = @Car16ID AND Valor = @Car16)
					OR (@Car17 IS NULL OR CaracteristicaID = @Car17ID AND Valor = @Car17)
				)
			)
		
		-- Se verifica el número de filas encontradas, si son más de 200, se sale
		IF (SELECT COUNT(*) FROM @Partes) > 200 BEGIN
			-- Se selecciona un registro vacío, sólo para mandar la estructura
			SELECT
				CONVERT(INT, 0) AS ParteID
				, CONVERT(NVARCHAR(64), '') AS NumeroDeParte
				, CONVERT(NVARCHAR(512), '* Se encontraron más de 200 resultados. Debes ser más específico.') AS Descripcion
				, CONVERT(NVARCHAR(128), '') AS Marca
				, CONVERT(NVARCHAR(128), '') AS Linea
				, CONVERT(DECIMAL(12, 2), 0) AS Existencia
				, CONVERT(DECIMAL(12, 2), 0) AS ExistenciaLocal
			RETURN
		END
			
		-- Se filtran las características, si aplica
		/* IF @Caracteristicas = 1 BEGIN
			DELETE FROM @Partes WHERE ParteID NOT IN (
				SELECT p.ParteID
				FROM
					@Partes p
					INNER JOIN ParteCaracteristica pc ON pc.ParteID = p.ParteID
				WHERE
					(@Car01 IS NULL OR CaracteristicaID = @Car01ID AND Valor = @Car01)
					OR (@Car02 IS NULL OR CaracteristicaID = @Car02ID AND Valor = @Car02)
					OR (@Car03 IS NULL OR CaracteristicaID = @Car03ID AND Valor = @Car03)
					OR (@Car04 IS NULL OR CaracteristicaID = @Car04ID AND Valor = @Car04)
					OR (@Car05 IS NULL OR CaracteristicaID = @Car05ID AND Valor = @Car05)
					OR (@Car06 IS NULL OR CaracteristicaID = @Car06ID AND Valor = @Car06)
					OR (@Car07 IS NULL OR CaracteristicaID = @Car07ID AND Valor = @Car07)
					OR (@Car08 IS NULL OR CaracteristicaID = @Car08ID AND Valor = @Car08)
					OR (@Car09 IS NULL OR CaracteristicaID = @Car09ID AND Valor = @Car09)
					OR (@Car10 IS NULL OR CaracteristicaID = @Car10ID AND Valor = @Car10)
					OR (@Car11 IS NULL OR CaracteristicaID = @Car11ID AND Valor = @Car11)
					OR (@Car12 IS NULL OR CaracteristicaID = @Car12ID AND Valor = @Car12)
					OR (@Car13 IS NULL OR CaracteristicaID = @Car13ID AND Valor = @Car13)
					OR (@Car14 IS NULL OR CaracteristicaID = @Car14ID AND Valor = @Car14)
					OR (@Car15 IS NULL OR CaracteristicaID = @Car15ID AND Valor = @Car15)
					OR (@Car16 IS NULL OR CaracteristicaID = @Car16ID AND Valor = @Car16)
					OR (@Car17 IS NULL OR CaracteristicaID = @Car17ID AND Valor = @Car17)
			)
		
		END
		*/

		-- Se obtienen los equivalente, si aplica
		IF @Equivalentes = 1 BEGIN
			DECLARE @ParaEquiv TABLE (ParteID INT NOT NULL)
			INSERT INTO @ParaEquiv SELECT * FROM @Partes
			DELETE FROM @Partes
			
			INSERT INTO @Partes
			SELECT DISTINCT ParteID FROM (
				SELECT
					ISNULL(pe.ParteID, p.ParteID) AS ParteID
					, ROW_NUMBER() OVER(PARTITION BY p.ParteID ORDER BY 
						CASE WHEN pex.Existencia > 0 THEN 1 ELSE 2 END
						, pp.PrecioUno DESC
					) AS Fila
				FROM
					(
						SELECT p.ParteID, pe.GrupoID
						FROM @ParaEquiv p INNER JOIN ParteEquivalente pe ON pe.ParteID = p.ParteID
					) p
					LEFT JOIN ParteEquivalente pe ON pe.GrupoID = p.GrupoID
					LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
					LEFT JOIN ParteExistencia pex ON pex.ParteID = pe.ParteID 
						AND pex.SucursalID = @SucursalID AND pex.Estatus = 1
			) c
			WHERE c.Fila = 1
		END
	END ELSE BEGIN
		INSERT INTO @Partes
		SELECT ParteID
		FROM Parte p
		WHERE
			p.Estatus = 1
			AND ParteEstatusID = @ParteEstActivo
			AND (NumeroParte = @Codigo OR CodigoBarra = @Codigo)
	END

	-- Se realiza la consulta final, con todos los campos necesarios
	SELECT
		pc.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, SUM(pe.Existencia) AS Existencia
		, SUM(CASE WHEN pe.SucursalID = @SucursalID THEN pe.Existencia ELSE 0.00 END) AS ExistenciaLocal
	FROM
		@Partes pc
		LEFT JOIN Parte p ON p.ParteID = pc.ParteID AND p.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = pc.ParteID AND pe.Estatus = 1
	GROUP BY
		pc.ParteID
		, p.NumeroParte
		, p.NombreParte
		, mp.NombreMarcaParte
		, l.NombreLinea

END
GO