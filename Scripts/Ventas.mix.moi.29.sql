/* Script con modificaciones para el módulo de ventas. Archivo 29
 * Creado: 2014/03/10
 * Subido: 2014/03/14
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE TABLE ParteCambio (
		ParteCambioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, ParteID INT NOT NULL FOREIGN KEY REFERENCES Parte(ParteID)
		, Fecha DATETIME NOT NULL
		, Campo NVARCHAR(64) NOT NULL
		, Antes NVARCHAR(512) NULL
		, Despues NVARCHAR(512) NULL
		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	)

	ALTER TABLE ParteMaxMin ADD Fijo BIT NULL

	ALTER TABLE ParteExistencia DROP CONSTRAINT DF_ParteExistencia_UnidadEmpaque
	ALTER TABLE ParteExistencia DROP COLUMN UnidadEmpaque

	INSERT INTO Permiso (NombrePermiso, MensajeDeError, FechaRegistro) VALUES
		('Administracion.Master.Modificar', 'No tienes permisos para hacer modificaciones masivas a las partes.', GETDATE())

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
ALTER VIEW [dbo].[ExistenciasView] AS
	SELECT
		ParteExistencia.ParteExistenciaID
		,ParteExistencia.ParteID
		,Parte.NumeroParte
		,ParteExistencia.SucursalID
		,Sucursal.NombreSucursal AS Tienda
		,ParteExistencia.Existencia AS Exist
		-- ,ParteExistencia.Maximo AS [Max]
		-- ,ParteExistencia.Minimo AS [Min]
		, ParteMaxMin.Maximo AS [Max]
		, ParteMaxMin.Minimo AS [Min]
		, Parte.UnidadEmpaque AS UEmp
	FROM
		ParteExistencia
		INNER JOIN Parte ON Parte.ParteID = ParteExistencia.ParteID
		INNER JOIN Sucursal ON Sucursal.SucursalID = ParteExistencia.SucursalID
		LEFT JOIN ParteMaxMin ON ParteMaxMin.ParteID = ParteExistencia.ParteID
			AND ParteMaxMin.SucursalID = ParteExistencia.SucursalID
GO

-- DROP VIEW PartesDetalleView
CREATE VIEW PartesDetalleView AS
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
		, p.AplicaComision
		, p.EsServicio
		, p.Etiqueta
		, p.SoloUnaEtiqueta
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
		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = 1 AND pe.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND pp.Estatus = 1
	WHERE p.Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

GO
-- EXEC pauPartesMaxMin 1, '2013-02-01', '2014-01-31', NULL, NULL, NULL
ALTER PROCEDURE pauPartesMaxMin (
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

	-- Consulta final
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
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo

		, p.ProveedorID
		, p.MarcaParteID
		, p.LineaID
		, p.UnidadEmpaque
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
		, pmm.Fijo
		, pmm.Minimo
		, pmm.Maximo
		, pmm.FechaCalculo
		
		, p.ProveedorID
		, p.MarcaParteID
		, p.LineaID
		, p.UnidadEmpaque
		, pmm.VentasGlobales

END
GO