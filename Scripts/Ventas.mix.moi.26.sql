/* Script con modificaciones para el módulo de ventas. Archivo 26
 * Creado: 2014/02/23
 * Subido: 2014/03/06
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

/*
-- Script para replicar los criterios generales de una sucursal a otra
begin tran
DECLARE @SucDeID INT = 2
DECLARE @SucAID INT = 3
update ParteMaxMin Set
	Calcular = pDe.Calcular
	, VentasGlobales = pDe.VentasGlobales
from
	ParteMaxMin pA
	LEFT JOIN (select pDe.ParteID, pDe.Calcular, pDe.VentasGlobales from ParteMaxMin pDe where SucursalID = @SucDeID) pDe
		ON pDe.ParteID = pA.ParteID -- AND p1.SucursalID = 1
Where pA.SucursalID = @SucAID
select top 1000 * from partemaxmin
rollback tran

-- Script para replicar los criterios generales predefinidos de una sucursal a otra
DECLARE @SucDeID INT = 2
DECLARE @SucAID INT = 3
INSERT INTO ParteMaxMinCriterioPredefinido (SucursalID, ProveedorID, MarcaID, LineaID, Calcular, VentasGlobales)
	SELECT @SucAID, ProveedorID, MarcaID, LineaID, Calcular, VentasGlobales
	FROM ParteMaxMinCriterioPredefinido WHERE SucursalID = @SucDeID
select * from ParteMaxMinCriterioPredefinido
*/

BEGIN TRAN
BEGIN TRY

	-- Se borran tablas y vistas de MaxMin no necesarias
	-- Siempre no, es algo complicado. Otro día será
	/*
	DROP TABLE MxMnCriterio
	DROP TABLE ParteMxMn
	DROP TABLE ParteMxMnCriterio
	DROP TABLE SucursalCriterioABC
	DROP TABLE CriterioABC
	DROP VIEW MxMnCriterioView
	DROP VIEW MxMnView
	DROP VIEW PartesClasificacionAbcView
	DROP VIEW SucursalesCriterioAbcView
	DROP PROCEDURE pauParteActualizaCriterioAbc
	ALTER TABLE ParteExistencia DROP COLUMN Maximo, Minimo
	-- ALTER TABLE Parte DROP COLUMN CriterioABC
	-- ExistenciasView
	*/

	-- DROP TABLE ParteAbc
	CREATE TABLE ParteAbc (
		ParteAbcID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, AbcDeVentas NVARCHAR(1) NULL
		, AbcDeUtilidad NVARCHAR(1) NULL
		, AbcDeNegocio NVARCHAR(1) NULL
		, AbcDeProveedor NVARCHAR(1) NULL
		, AbcDeLinea NVARCHAR(1) NULL
		, FechaModificacion DATETIME NULL
	)
	INSERT INTO ParteAbc (ParteID) SELECT ParteID FROM Parte WHERE Estatus = 1

	CREATE TABLE ParteAbcRango (
		ParteAbcRangoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, TipoDeRango NVARCHAR(16) NOT NULL
		, Inicial DECIMAL(12, 2) NULL
		, Final DECIMAL(12, 2) NULL
		, Abc NVARCHAR(1) NOT NULL
	)
	INSERT INTO ParteAbcRango (TipoDeRango, Inicial, Final, Abc) VALUES
		('VENTAS', 20, NULL, 'A')
		, ('VENTAS', 12, 19, 'B')
		, ('VENTAS', 8, 11, 'C')
		, ('VENTAS', 3, 7, 'D')
		, ('VENTAS', 2, 2, 'E')
		, ('VENTAS', 1, 1, 'N')
		, ('VENTAS', 0, 0, 'Z')
		, ('UTILIDAD', 250, NULL, 'A')
		, ('UTILIDAD', 100, 249.99, 'B')
		, ('UTILIDAD', 50, 99.99, 'C')
		, ('UTILIDAD', 25, 49.99, 'D')
		, ('UTILIDAD', 15, 24.99, 'E')
		, ('UTILIDAD', 7.5, 14.99, 'N')
		, ('UTILIDAD', 0.01, 7.49, 'Z')
		, ('NEGOCIO', 1, 20, 'A')
		, ('NEGOCIO', 21, 40, 'B')
		, ('NEGOCIO', 41, 60, 'C')
		, ('NEGOCIO', 61, 80, 'D')
		, ('NEGOCIO', 81, 90, 'E')
		, ('NEGOCIO', 91, 95, 'N')
		, ('NEGOCIO', 96, 100, 'Z')
		, ('PROVEEDOR', 0.1, 80, 'A')
		, ('PROVEEDOR', 80.1, 90, 'B')
		, ('PROVEEDOR', 90.1, 95, 'C')
		, ('PROVEEDOR', 95.1, 98, 'D')
		, ('PROVEEDOR', 98.1, 99.5, 'E')
		, ('PROVEEDOR', 99.6, 99.9, 'N')
		, ('PROVEEDOR', 100, 100, 'Z')
		, ('LINEA', 1, 50, 'A')
		, ('LINEA', 51, 60, 'B')
		, ('LINEA', 61, 80, 'C')
		, ('LINEA', 81, 90, 'D')
		, ('LINEA', 91, 95, 'E')
		, ('LINEA', 96, 99, 'N')
		, ('LINEA', 100, 100, 'Z')

	ALTER TABLE ParteMaxMin ALTER COLUMN Maximo DECIMAL(12, 2)
	ALTER TABLE ParteMaxMin ALTER COLUMN Minimo DECIMAL(12, 2)
	INSERT INTO ParteMaxMin (ParteID, SucursalID, UsuarioID)
		SELECT p.ParteID, s.SucursalID, 1
		FROM
			Parte p
			LEFT JOIN Sucursal s ON 1 = 1
			LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID AND pmm.SucursalID = s.SucursalID
		WHERE
			pmm.ParteMaxMinID IS NULL

	-- DROP TABLE ParteMaxMinCriterioPredefinido
	CREATE TABLE ParteMaxMinCriterioPredefinido (
		ParteMaxMinCriterioPredefinidoID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, ProveedorID INT NOT NULL FOREIGN KEY REFERENCES Proveedor(ProveedorID)
		, MarcaID INT NOT NULL FOREIGN KEY REFERENCES MarcaParte(MarcaParteID)
		, LineaID INT NOT NULL FOREIGN KEY REFERENCES Linea(LineaID)
		, Calcular BIT NULL
		, VentasGlobales BIT NULL
		
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME NULL
	)

	CREATE TABLE ParteMaxMinRegla (
		ParteMaxMinReglaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, Orden INT NOT NULL
		, Regla VARCHAR(32) NOT NULL
		, Condicion VARCHAR(128) NOT NULL
		, Maximo VARCHAR(128) NOT NULL
		, Minimo VARCHAR(128) NOT NULL
		
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)
	-- DELETE FROM ParteMaxMinRegla
	INSERT INTO ParteMaxMinRegla (Orden, Regla, Condicion, Maximo, Minimo, UsuarioID) VALUES
		(1, 'FÓRMULA MADRE', 'VentasTotal > 1', 'UDE', '0', 1)
		, (2, 'BUJÍA', 'IdLinea == 40 && VentasTotal > 1', 'MultiploSup(CantidadMaxSem, UDE)', '(Maximo / 2) > 8 ? (Maximo / 2) : 8', 1)
		, (3, 'CABLE INSTALACIÓN', 'IdLinea == 43', 'MultiploSup(CantidadMaxMes, UDE)', 'Maximo > 0 ? (CantidadMaxDia > 40 ? CantidadMaxDia : 40) : 0', 1)
		, (4, 'FILTRO', 'IdLinea == 82 && IdMarca == 87 && VentasTotal > 0', 'CantidadMaxSem * 1.2M', 'Maximo / 2', 1)
		, (5, 'MARCHA', 'IdLinea == 101 && VentasTotal > 1', 'CantidadMaxSem', 'Maximo - 1', 1)
		, (6, 'AMORTIGUADOR', 'IdProveedor == 43 && IdLinea == 11 && VentasTotal > 1', 'UDE', 'Maximo - 1', 1)
		, (7, 'BOBINA', 'IdProveedor == 30 && IdLinea == 30 && VentasTotal > 1', 'CantidadMaxDia', 'Maximo - 1', 1)
		, (8, 'CEPILLO ALTERNADOR', 'IdLinea == 53 && VentasTotal > 1', 'CantidadMaxMes > UDE ? CantidadMaxMes : UDE', 'Maximo / 2', 1)
		, (9, 'UNIDAD DE EMPAQUE 2', 'Maximo == 2 && UDE == 2', '2', '1', 1)
		, (10, 'MÍNIMO ENTRE 0 y 1', 'Minimo > 0 && Minimo < 1', 'Maximo', '1', 1)
		, (11, 'MÍNIMO MAYOR QUE MÁXIMO', 'Minimo > Maximo', 'Maximo', 'Maximo - 1', 1)
		, (12, 'MÍNIMO IGUAL A MÁXIMO', 'Minimo > 0 && Minimo == Maximo', 'Maximo', 'Minimo - 1', 1)
		, (13, 'MÁXIMO ES 0', 'Maximo == 0', '0', '0', 1)

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

GO
ALTER VIEW PartesMaxMinView AS
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pmm.SucursalID
		, pmm.Calcular
		, pmm.VentasGlobales
		, p.ProveedorID
		, pv.NombreProveedor AS Proveedor
		, p.MarcaParteID
		, mp.NombreMarcaParte AS Marca
		, p.LineaID
		, l.NombreLinea AS Linea
	FROM
		Parte p
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
	/* ORDER BY
		pv.NombreProveedor
		, mp.NombreMarcaParte
		, l.NombreLinea
		, p.NombreParte
		
	*/
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

GO
-- EXEC pauPartesAbc '2013-03-01', '2014-02-28'
-- DROP PROCEDURE pauPartesAbc
CREATE PROCEDURE pauPartesAbc (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/*
	DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2014-02-28'
	*/
	
	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pa.AbcDeNegocio
		, pa.AbcDeProveedor
		, pa.AbcDeLinea
		
		, p.ProveedorID
		, p.LineaID
		, pp.Costo
		, pp.PrecioUno
		, ISNULL(vc.Cantidad, 0.00) AS Cantidad
		, ISNULL(vc.Utilidad, 0.00) AS Utilidad
	FROM
		Parte p
		LEFT JOIN ParteAbc pa ON pa.ParteID = p.ParteID
		
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
		LEFT JOIN (
			SELECT
				vd.ParteID
				, SUM(vd.Cantidad) AS Cantidad
				, SUM((vd.PrecioUnitario - ppi.Costo) * vd.Cantidad) AS Utilidad
			FROM
				VentaDetalle vd
				LEFT JOIN PartePrecio ppi ON ppi.ParteID = vd.ParteID AND ppi.Estatus = 1
				INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			WHERE
				vd.Estatus = 1
				AND v.VentaEstatusID = @EstPagadaID
				AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			GROUP BY vd.ParteID
		) vc ON vc.ParteID = p.ParteID
	WHERE p.Estatus = 1
	ORDER BY p.NombreParte
END
GO

-- EXEC pauParteMaxMinDatosVentas 9267, '2013-03-01', '2014-02-28'
-- DROP PROCEDURE pauParteMaxMinDatosVentas
CREATE PROCEDURE pauParteMaxMinDatosVentas (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON
	
	/* DECLARE @ParteID INT = 9267
	DECLARE @Desde DATE = '2013-03-01'
	DECLARE @Hasta DATE = '2014-02-28'
	*/
	
	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)
	
	-- Procedimiento
	;WITH _Ventas AS (
		SELECT
			CONVERT(DATE, v.Fecha) AS Dia
			, ISNULL(COUNT(v.VentaID), 0) AS Ventas
			, ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
		FROM
			Parte p
			INNER JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
		WHERE
			p.Estatus = 1
			AND p.ParteID = @ParteID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
		GROUP BY CONVERT(DATE, v.Fecha)
			
		-- Se agrega lo negado
		UNION ALL
		SELECT
			CONVERT(DATE, rf.FechaRegistro) AS Dia
			, COUNT(rf.ReporteDeFaltanteID) AS Ventas
			, SUM(rf.CantidadRequerida) AS Cantidad
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
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
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
	FROM _Ventas v
	
END
GO

-- EXEC pauParteMaxMinDatosExtra 5566, '2013-03-01', '2014-02-28'
-- DROP PROCEDURE pauParteMaxMinDatosExtra
CREATE PROCEDURE pauParteMaxMinDatosExtra (
	@ParteID INT
	, @Desde DATE
	, @Hasta DATE
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
					-- AND v.SucursalID = @SucursalID
			) vdc ON vdc.ParteID = rf.ParteID AND (rf.FechaRegistro >= vdc.Dia AND rf.FechaRegistro < DATEADD(d, 2, vdc.Dia))
		WHERE
			rf.Estatus = 1
			AND rf.ParteID = @ParteID
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
			-- AND rf.SucursalID = @SucursalID
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

-- EXEC pauPartesMaxMin 1, '2013-02-01', '2014-01-31', NULL, NULL, NULL
-- DROP PROCEDURE pauPartesMaxMin
CREATE PROCEDURE pauPartesMaxMin (
	@SucursalID INT
	, @Desde DATE
	, @Hasta DATE
	, @ProveedorID INT = NULL
	, @MarcaID INT = NULL
	, @LineaID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @SucursalID INT = 1
	DECLARE @Desde DATE = '2013-02-01'
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
	
	-- Ventas
	;WITH _Ventas AS (
		SELECT
			vd.ParteID
			, CONVERT(DATE, v.Fecha) AS Fecha
			, COUNT(v.VentaID) AS Ventas
			, SUM(vd.Cantidad) AS Cantidad
			, SUM((vd.PrecioUnitario - pp.Costo) * vd.Cantidad) AS Utilidad
		FROM
			VentaDetalle vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.SucursalID = @SucursalID
		GROUP BY
			vd.ParteID
			, CONVERT(DATE, v.Fecha)

		-- Se agrega lo negado
		UNION ALL
		SELECT
			rf.ParteID
			, CONVERT(DATE, rf.FechaRegistro) AS Fecha
			, COUNT(rf.ReporteDeFaltanteID) AS Ventas
			, SUM(rf.CantidadRequerida) AS Cantidad
			, SUM((pp.PrecioUno - pp.Costo) * rf.CantidadRequerida) AS Utilidad
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
			AND (rf.FechaRegistro >= @Desde AND rf.FechaRegistro < @Hasta)
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

	-- 
	-- , _PartesMaxMin AS (
	SELECT
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pv.NombreProveedor AS Proveedor
		, mp.NombreMarcaParte AS Marca
		, l.NombreLinea AS Linea
		, pe.Existencia
		, p.TiempoReposicion
		, pa.AbcDeNegocio
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo

		, p.ProveedorID
		, p.MarcaParteID
		, p.LineaID
		, pe.UnidadEmpaque
		, ISNULL(SUM(vc.Ventas), 0) AS VentasTotal
		, ISNULL(SUM(vc.Cantidad), 0.00) AS CantidadTotal
		, ISNULL(MAX(vc.Cantidad), 0.00) AS CantidadMaxDia
		, ISNULL(MAX(vcs.Cantidad), 0.00) AS CantidadMaxSem
		, ISNULL(MAX(vcm.Cantidad), 0.00) AS CantidadMaxMes
		, ISNULL(SUM(vc.Utilidad), 0.00) AS UtilidadTotal
		, ISNULL(pmm.VentasGlobales, 0) AS VentasGlobales
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
	WHERE
		p.Estatus = 1
		AND pmm.Calcular = 1
		AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		AND (@LineaID IS NULL OR p.LineaID = @LineaID)
	GROUP BY
		p.ParteID
		, p.NumeroParte
		, p.NombreParte
		, pv.NombreProveedor
		, mp.NombreMarcaParte
		, l.NombreLinea
		, pe.Existencia
		, p.TiempoReposicion
		, pa.AbcDeNegocio
		, pa.AbcDeVentas
		, pa.AbcDeUtilidad
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		
		, p.ProveedorID
		, p.MarcaParteID
		, p.LineaID
		, pe.UnidadEmpaque
		, pmm.VentasGlobales
	-- )
	
	-- Para sacar la parte de ventas globales
	/* , _VentasGlobales AS (
		SELECT
			vd.ParteID
			, CONVERT(DATE, v.Fecha) AS Fecha
			, SUM(vd.Cantidad) AS Cantidad
			-- , SUM((vd.PrecioUnitario - pp.Costo) * vd.Cantidad) AS Utilidad
		FROM
			VentaDetalle vd
			INNER JOIN Venta v ON v.VentaID = vd.VentaID AND v.Estatus = 1
			-- LEFT JOIN PartePrecio pp ON pp.ParteID = vd.ParteID AND pp.Estatus = 1
		WHERE
			vd.Estatus = 1
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND vd.ParteID IN (SELECT ParteID FROM _PartesMaxMin WHERE VentasGlobales = 1)
		GROUP BY
			vd.ParteID
			, CONVERT(DATE, v.Fecha)
	), _Globales AS (
		SELECT
			p.ParteID
			, ISNULL(SUM(vc.Cantidad), 0.00) AS VentasTotal
			, ISNULL(MAX(vc.Cantidad), 0.00) AS VentasMaxDia
			, ISNULL(MAX(vcs.Cantidad), 0.00) AS VentasMaxSem
			, ISNULL(MAX(vcm.Cantidad), 0.00) AS VentasMaxMes
			-- , ISNULL(SUM(vc.Utilidad), 0.00) AS UtilidadTotal
		FROM
			Parte p
			INNER JOIN ParteMaxMin pmm ON pmm.ParteID = p.ParteID AND pmm.SucursalID = @SucursalID
			LEFT JOIN _VentasGlobales vc ON vc.ParteID = p.ParteID
			LEFT JOIN (
				SELECT
					ParteID
					, DATEPART(WK, Fecha) AS Semana
					, SUM(Cantidad) AS Cantidad
				FROM _VentasGlobales
				GROUP BY
					ParteID
					, DATEPART(WK, Fecha)
			) vcs ON vcs.ParteID = p.ParteID
			LEFT JOIN (
				SELECT
					ParteID
					, MONTH(Fecha) AS Mes
					, SUM(Cantidad) AS Cantidad
				FROM _VentasGlobales
				GROUP BY
					ParteID
					, MONTH(Fecha)
			) vcm ON vcm.ParteID = p.ParteID
		WHERE
			p.Estatus = 1
			AND pmm.Calcular = 1
			AND (@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
			AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
			AND (@LineaID IS NULL OR p.LineaID = @LineaID)
		GROUP BY p.ParteID
	)
	
	-- Se obtiene la consulta final
	SELECT
		mm.*
		, g.VentasTotal AS VentasGlobal
		, g.VentasMaxDia AS VentasGlobalMaxDia
		, g.VentasMaxSem AS VentasGlobalMaxSem
		, g.VentasMaxMes AS VentasGlobalMaxMes
	FROM
		_PartesMaxMin mm
		LEFT JOIN _Globales g ON g.ParteID = mm.ParteID
	*/

END
GO