/* Script con modificaciones para el módulo de ventas. Archivo 28
 * Creado: 2014/03/10
 * Subido: 2014/03/10
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE ParteMaxMinRegla ADD SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		CONSTRAINT DF_SucursalID DEFAULT 1
	ALTER TABLE ParteMaxMinRegla DROP CONSTRAINT DF_SucursalID
	
	UPDATE ParteMaxMinRegla SET Maximo = 'MultiploSup(CantidadMaxMes, UDE)' WHERE Regla = 'CEPILLO ALTERNADOR'

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



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */



/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

GO
-- EXEC pauParteMaxMinDatosExtra 5566, '2013-03-01', '2014-02-28'
ALTER PROCEDURE pauParteMaxMinDatosExtra (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
	, @SucursalID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @ParteID INT = 1
	DECLARE @Desde DATE = '2013-03-01'
	DECLARE @Hasta DATE = '2014-02-28'
	*/
	
	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

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
			AND v.VentaEstatusID = @EstPagadaID
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
					, CONVERT(DATE, v.Fecha) AS Dia
				FROM
					VentaDetalle vd
					INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
				WHERE
					vd.Estatus = 1
					AND v.VentaEstatusID = @EstPagadaID
					AND (@SucursalID IS NULL OR v.SucursalID = @SucursalID)
			) vdc ON vdc.ParteID = rf.ParteID AND (rf.FechaRegistro >= vdc.Dia AND rf.FechaRegistro < DATEADD(d, 2, vdc.Dia))
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