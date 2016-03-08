/* Script con modificaciones a la base de datos de Theos. Archivo 063
 * Creado: 2016/02/18
 * Subido: 2016/03/08
 */

DECLARE @ScriptID INT = 63
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Reportes.NominaDomingo.Salida', 'D', 'P', 'Salida donde se muestra el reporte de la Nómina Domingo (D - Diseño, P - Pantalla, I - Impresora, N - Nada).')

ALTER TABLE ContaCuentaAuxiliar ALTER COLUMN CuentaAuxiliar NVARCHAR(128) NOT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
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
			AND v.VentaEstatusID = @EstPagadaID
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

ALTER PROCEDURE [dbo].[pauParteMaxMinDatosVentas] (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON
	
	/*
	DECLARE @ParteID INT = 5568
	DECLARE @Desde DATE = '2013-03-01'
	DECLARE @Hasta DATE = '2016-02-28'
	EXEC pauParteMaxMinDatosVentas @ParteID, @Desde, @Hasta
	*/
	
	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	-- Para el cálculo de los tres años anteriores
	DECLARE @IniActual DATE = CONVERT(DATE, CONVERT(VARCHAR(4), DATEPART(YEAR, GETDATE())) + '-01-01')
	DECLARE @IniAnt1 DATE = DATEADD(YEAR, -1, @IniActual)
	DECLARE @IniAnt2 DATE = DATEADD(YEAR, -2, @IniActual)
	DECLARE @IniAnt3 DATE = DATEADD(YEAR, -3, @IniActual)
	
	-- Procedimiento
	;WITH _Ventas AS (
		SELECT
			CONVERT(DATE, v.Fecha) AS Dia
			/* , ISNULL(COUNT(v.VentaID), 0) AS Ventas
			, ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
			*/
			, COUNT(CASE WHEN (v.Fecha >= @Desde AND v.Fecha < @Hasta) THEN v.VentaID ELSE NULL END) AS Ventas
			, SUM(CASE WHEN (v.Fecha >= @Desde AND v.Fecha < @Hasta) THEN vd.Cantidad ELSE 0.0 END) AS Cantidad
			, COUNT(CASE WHEN (v.Fecha >= @IniAnt1 AND v.Fecha < @IniActual) THEN v.VentaID ELSE NULL END) AS VentasAnt1
			, COUNT(CASE WHEN (v.Fecha >= @IniAnt2 AND v.Fecha < @IniAnt1) THEN v.VentaID ELSE NULL END) AS VentasAnt2
			, COUNT(CASE WHEN (v.Fecha >= @IniAnt3 AND v.Fecha < @IniAnt2) THEN v.VentaID ELSE NULL END) AS VentasAnt3
		FROM
			Parte p
			INNER JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		WHERE
			p.Estatus = 1
			AND p.ParteID = @ParteID
			AND v.VentaEstatusID = @EstPagadaID
			-- AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND (v.Fecha >= @IniAnt3 AND v.Fecha < @Hasta)
		GROUP BY CONVERT(DATE, v.Fecha)
			
		-- Se agrega lo negado
		UNION ALL
		SELECT
			CONVERT(DATE, rf.FechaRegistro) AS Dia
			/* , COUNT(rf.ReporteDeFaltanteID) AS Ventas
			, SUM(rf.CantidadRequerida) AS Cantidad
			*/
			, COUNT(CASE WHEN (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS Ventas
			, SUM(CASE WHEN (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
				THEN rf.CantidadRequerida ELSE 0.0 END) AS Cantidad
			, COUNT(CASE WHEN (rf.FechaRegistro >= @IniAnt1 AND rf.FechaRegistro < @IniActual)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS VentasAnt1
			, COUNT(CASE WHEN (rf.FechaRegistro >= @IniAnt2 AND rf.FechaRegistro < @IniAnt1)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS VentasAnt2
			, COUNT(CASE WHEN (rf.FechaRegistro >= @IniAnt3 AND rf.FechaRegistro < @IniAnt2)
				THEN rf.ReporteDeFaltanteID ELSE NULL END) AS VentasAnt3
		FROM
			ReporteDeFaltante rf
			-- Para validar que no se haya vendido en el día o en el siguiente
			LEFT JOIN (
				SELECT DISTINCT
					vd.ParteID
					, CONVERT(DATE, v.Fecha) AS Dia
					, v.SucursalID
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
			) vdc ON vdc.ParteID = rf.ParteID
				AND (rf.FechaRegistro >= vdc.Dia AND rf.FechaRegistro < DATEADD(d, 2, vdc.Dia))
				AND vdc.SucursalID = rf.SucursalID
		WHERE
			rf.Estatus = 1
			AND rf.ParteID = @ParteID
			-- AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			AND (rf.FechaRegistro >= @IniAnt3 AND rf.FechaRegistro < @Hasta)
			-- Parte de Venta
			AND vdc.ParteID IS NULL
		GROUP BY
			CONVERT(DATE, rf.FechaRegistro)
		-- Fin lo negado
	)

	-- Consulta final
	SELECT
		SUM(Ventas) AS Ventas
		, SUM(Cantidad) AS Cantidad
		, MAX(Cantidad) AS CantidadMaxDia
		, (
			SELECT TOP 1 SUM(Cantidad) AS Cantidad
			FROM _Ventas
			GROUP BY DATEPART(WK, Dia)
			ORDER BY Cantidad DESC
		) AS CantidadMaxSem
		, (
			SELECT TOP 1 SUM(Cantidad) AS Cantidad
			FROM _Ventas
			GROUP BY MONTH(Dia)
			ORDER BY Cantidad DESC
		) AS CantidadMaxMes
		
		, ISNULL(SUM(v.VentasAnt1), 0) AS VentasAnt1
		, ISNULL(SUM(v.VentasAnt2), 0) AS VentasAnt2
		, ISNULL(SUM(v.VentasAnt3), 0) AS VentasAnt3
	FROM _Ventas v
	
END
GO