/* Script con modificaciones a la base de datos de Theos. Archivo 079
 * Creado: 2016/09/06
 * Subido: 2016/09/06
 */

DECLARE @ScriptID INT = 79
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE ParteMaxMinFijoHistorico (
	ParteMaxMinFijoHistoricoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
	, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
	, Fecha DATETIME NOT NULL
	, Motivo NVARCHAR(128) NOT NULL
)

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* ****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauParteMaxMinDatosExtra] (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @ParteID INT = 14790
	DECLARE @Desde DATE = '2016-01-01'
	DECLARE @Hasta DATE = '2016-12-31'
	DECLARE @SucursalID INT = 1
	EXEC pauParteMaxMinDatosExtra @ParteID, @Desde, @Hasta, @SucursalID
	*/
	
	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCobradaID INT = 2

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	-- Procedimiento

	;WITH _VentasParte AS (
		SELECT
			p.ParteID
			, CONVERT(DATE, v.Fecha) AS Dia
			, DATEPART(WK, v.Fecha) AS Semana
			, MONTH(v.Fecha) AS Mes
			, YEAR(v.Fecha) AS Anio
			, SUM(vd.Cantidad) AS Cantidad
			, 0 AS Negada
		FROM
			Parte p
			INNER JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		WHERE
			p.Estatus = 1
			AND p.ParteID = @ParteID
			AND v.VentaEstatusID IN (@EstPagadaID, @EstCobradaID)
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
		GROUP BY
			p.ParteID
			, CONVERT(DATE, v.Fecha)
			, DATEPART(WK, v.Fecha)
			, MONTH(v.Fecha)
			, YEAR(v.Fecha)

		-- Se agrega lo negado
		UNION ALL
		SELECT
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro) AS Dia
			, DATEPART(WK, rf.FechaRegistro) AS Semana
			, MONTH(rf.FechaRegistro) AS Mes
			, YEAR(rf.FechaRegistro) AS Anio
			, SUM(rf.CantidadRequerida) AS Cantidad
			, 1 AS Negada
		FROM
			ReporteDeFaltante rf
			-- Para validar que no se haya vendido en el día o en el siguiente
			LEFT JOIN (
				SELECT DISTINCT
					vd.ParteID
					, v.Fecha
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
					AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
			) vdc ON vdc.ParteID = rf.ParteID AND 
				(vdc.Fecha >= CONVERT(DATE, rf.FechaRegistro) AND vdc.Fecha < DATEADD(d, 2, CONVERT(DATE, rf.FechaRegistro)))
		WHERE
			rf.Estatus = 1
			AND rf.ParteID = @ParteID
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			AND (@SucursalID IS NULL OR rf.SucursalID = @SucursalID)
			-- Parte de Venta
			AND vdc.ParteID IS NULL
		GROUP BY
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro)
			, DATEPART(WK, rf.FechaRegistro)
			, MONTH(rf.FechaRegistro)
			, YEAR(rf.FechaRegistro)
		-- Fin lo negado
	)

	-- Venta mayor de un día
	SELECT * FROM (
		SELECT TOP 1
			1 AS Grupo
			, 0 AS Anio
			, 0 AS Periodo
			-- , MAX(Cantidad) AS Cantidad
			, Cantidad
			, CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END AS Negadas
		FROM _VentasParte
		ORDER BY Cantidad DESC
	) c
	
	-- Venta menor de un día
	UNION
	SELECT * FROM (
		SELECT TOP 1
			2 AS Grupo
			, 0 AS Anio
			, 0 AS Periodo
			-- , MIN(Cantidad) AS Cantidad
			, Cantidad
			, CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END AS Negadas
		FROM _VentasParte
		ORDER BY Cantidad
	) c

	-- Ventas por semana
	UNION
	SELECT
		3 AS Grupo
		, Anio
		, Semana AS Periodo
		, SUM(Cantidad) AS Cantidad
		, SUM(CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END) AS Negadas
	FROM _VentasParte
	GROUP BY
		Anio
		, Semana

	-- Ventas por mes
	UNION
	SELECT
		4 AS Grupo
		, Anio
		, Mes AS Periodo
		, SUM(Cantidad) AS Cantidad
		, SUM(CASE WHEN Negada = 1 THEN Cantidad ELSE 0.00 END) AS Negadas
	FROM _VentasParte
	GROUP BY
		Anio
		, Mes

	ORDER BY Grupo, Cantidad DESC

END
GO

ALTER PROCEDURE [dbo].[pauParteVentasPorMes] (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* Ejemplo
	DECLARE @ParteID INT = 5566
	DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2015-09-01'
	EXEC pauParteVentasPorMes @ParteID, @Desde, @Hasta
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3
	DECLARE @EstCobradaID INT = 2

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		v.SucursalID
		, s.NombreSucursal AS Sucursal
		, YEAR(v.Fecha) AS Anio
		, MONTH(v.Fecha) AS Mes
		, SUM(vd.Cantidad) AS Cantidad
		, SUM(ISNULL(n.Cantidad, 0.0)) AS Negado
	FROM
		Parte p
		INNER JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
		INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = v.SucursalID AND s.Estatus = 1
		-- Para agregar lo negado
		LEFT JOIN (
			SELECT
				rf.ParteID
				, rf.SucursalID
				, YEAR(rf.FechaRegistro) AS Anio
				, MONTH(rf.FechaRegistro) AS Mes
				, SUM(rf.CantidadRequerida) AS Cantidad
			FROM
				ReporteDeFaltante rf
				-- Para validar que no se haya vendido en el día o en el siguiente
				LEFT JOIN (
					SELECT DISTINCT
						vd.ParteID
						, v.Fecha
					FROM
						VentaDetalle vd
						INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
					WHERE
						vd.Estatus = 1
						AND v.VentaEstatusID = @EstPagadaID
				) vdc ON vdc.ParteID = rf.ParteID AND 
					(vdc.Fecha >= CONVERT(DATE, rf.FechaRegistro) AND vdc.Fecha < DATEADD(d, 2, CONVERT(DATE, rf.FechaRegistro)))
			WHERE
				rf.Estatus = 1
				AND rf.ParteID = @ParteID
				AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
				-- Parte de Venta
				AND vdc.ParteID IS NULL
			GROUP BY
				rf.ParteID
				, rf.SucursalID
				, YEAR(rf.FechaRegistro)
				, MONTH(rf.FechaRegistro)
		) n ON n.ParteID = p.ParteID AND n.SucursalID = v.SucursalID AND n.Anio = YEAR(v.Fecha) AND n.Mes = MONTH(v.Fecha)
	WHERE
		p.Estatus = 1
		AND p.ParteID = @ParteID
		AND v.VentaEstatusID IN (@EstPagadaID, @EstCobradaID)
		AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
	GROUP BY
		v.SucursalID
		, s.NombreSucursal
		, YEAR(v.Fecha)
		, MONTH(v.Fecha)
	ORDER BY
		SucursalID
		, Anio
		, Mes

END
GO
