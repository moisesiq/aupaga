/* Script con modificaciones a la base de datos de Theos. Archivo 034
 * Creado: 2015/09/03
 * Subido: 2015/09/14
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE ProveedorParteGanancia (
	ProveedorParteGananciaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ProveedorID INT NOT NULL FOREIGN KEY REFERENCES Proveedor(ProveedorID)
	, MarcaParteID INT NULL FOREIGN KEY REFERENCES MarcaParte(MarcaParteID)
	, LineaID INT NULL FOREIGN KEY REFERENCES Linea(LineaID)
	, ParteID INT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, DescuentoFactura1 DECIMAL(5, 2) NOT NULL
	, DescuentoFactura2 DECIMAL(5, 2) NOT NULL
	, DescuentoFactura3 DECIMAL(5, 2) NOT NULL
	, DescuentoArticulo1 DECIMAL(5, 2) NOT NULL
	, DescuentoArticulo2 DECIMAL(5, 2) NOT NULL
	, DescuentoArticulo3 DECIMAL(5, 2) NOT NULL
	, PorcentajeDeGanancia1 DECIMAL(5, 2) NOT NULL
	, PorcentajeDeGanancia2 DECIMAL(5, 2) NOT NULL
	, PorcentajeDeGanancia3 DECIMAL(5, 2) NOT NULL
	, PorcentajeDeGanancia4 DECIMAL(5, 2) NOT NULL
	, PorcentajeDeGanancia5 DECIMAL(5, 2) NOT NULL
	
	, CONSTRAINT Pk_ProveedorID_MarcaParteID_LineaID_ParteID UNIQUE (ProveedorID, MarcaParteID, LineaID, ParteID)
)

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.Partes.Etiqueta', 'I', 'I', 'Salida donde debe mostrarse la etiqueta de una parte (D - Diseño, P - Pantalla, I - Impresora).')

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

-- DROP VIEW ProveedoresPartesGananciasView
CREATE VIEW ProveedoresPartesGananciasView AS
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

ALTER VIEW [dbo].[PartesExistenciasView] AS
	SELECT
		ISNULL(ROW_NUMBER() OVER (ORDER BY p.ParteID, pe.SucursalID), 0) AS Registro
		, pe.ParteExistenciaID
		, p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID AS MarcaID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, pp.Costo
		, pp.CostoConDescuento
		, pe.SucursalID
		, pe.Existencia
		-- , SUM(pe.Existencia) AS Existencia
		, MAX(vd.FechaRegistro) AS UltimaVenta
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
	WHERE
		p.Estatus = 1
	GROUP BY
		pe.ParteExistenciaID
		, p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, p.ProveedorID
		, pv.NombreProveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte
		, p.LineaID
		, l.NombreLinea
		, pp.Costo
		, pp.CostoConDescuento
		, pe.SucursalID
		, pe.Existencia
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
				LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
				LEFT JOIN Venta v ON v.VentaID = vd.VentaID AND v.VentaEstatusID = @EstVentaPagada 
					AND DATEDIFF(DAY, v.Fecha, GETDATE()) <= 1 AND v.Estatus = 1
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
				-- Para el filtro de fecha de venta
				AND v.VentaID IS NULL
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

ALTER PROCEDURE [dbo].[pauPartesMaster] (
	@Desde DATE
	, @Hasta DATE
	-- , @ProveedorID INT = NULL
	-- , @MarcaID INT = NULL
	-- , @LineaID INT = NULL
	, @Proveedores tpuTablaEnteros READONLY
	, @Marcas tpuTablaEnteros READONLY
	, @Lineas tpuTablaEnteros READONLY
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-02-01'
	DECLARE @Hasta DATE = '2014-03-31'
	DECLARE @ProveedorID INT = NULL
	DECLARE @MarcaID INT = NULL
	DECLARE @LineaID INT = NULL
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	-- Procedimiento
	
	SELECT
		p.ParteID
		, p.CodigoBarra AS CodigoDeBara
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, m.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
		, p.MedidaID
		, md.NombreMedida AS UnidadDeMedida
		, p.UnidadEmpaque AS UnidadDeEmpaque
		, p.TiempoReposicion AS TiempoDeReposicion
		, p.AplicaComision
		, p.EsServicio
		, p.Etiqueta
		, p.SoloUnaEtiqueta
		, p.EsPar
		, pec.Existencia
		, ISNULL(vc.Ventas, 0) AS Ventas
		, pp.Costo
		, pp.PorcentajeUtilidadUno
		, pp.PrecioUno
		, pp.PorcentajeUtilidadDos
		, pp.PrecioDos
		, pp.PorcentajeUtilidadTres
		, pp.PrecioTres
		, pp.PorcentajeUtilidadCuatro
		, pp.PrecioCuatro
		, pp.PorcentajeUtilidadCinco
		, pp.PrecioCinco
	FROM
		Parte p
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte m ON m.MarcaParteID = p.MarcaParteID AND m.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN Medida md ON md.MedidaID = p.MedidaID AND md.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN (
			SELECT
				ParteID
				, SUM(Existencia) AS Existencia
			FROM ParteExistencia
			WHERE Estatus = 1
			GROUP BY ParteID
		) pec ON pec.ParteID = p.ParteID
		LEFT JOIN (
			SELECT
				vd.ParteID
				, COUNT(v.VentaID) AS Ventas
			FROM
				VentaDetalle vd
				INNER JOIN Venta v ON v.VentaID = vd.VentaID
			WHERE
				vd.Estatus = 1
				AND v.Estatus = 1
				AND v.VentaEstatusID = @EstPagadaID
				AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			GROUP BY vd.ParteID
		) vc ON vc.ParteID = p.ParteID
	WHERE
		p.Estatus = 1
		-- AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		-- AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		-- AND (@LineaID IS NULL OR p.LineaID = @LineaID)
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Proveedores) OR p.ProveedorID IN (SELECT Entero FROM @Proveedores))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Marcas) OR p.MarcaParteID IN (SELECT Entero FROM @Marcas))
		AND (NOT EXISTS(SELECT TOP 1 1 FROM @Lineas) OR p.LineaID IN (SELECT Entero FROM @Lineas))

END
GO
