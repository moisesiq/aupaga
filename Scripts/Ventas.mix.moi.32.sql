/* Script con modificaciones para el módulo de ventas. Archivo 32
 * Creado: 2014/02/21
 * Subido: 2014/03/26
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE Parte ADD EsPar BIT NULL
	
	ALTER TABLE CobranzaTicket ADD VentaPagoID INT NULL	FOREIGN KEY REFERENCES VentaPago(VentaPagoID)
	
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
ALTER VIEW [dbo].[PartesEquivalentesView] AS
	SELECT
		ParteEquivalente.ParteEquivalenteID
		,ParteEquivalente.ParteID
		,ParteEquivalente.ParteIDequivalente
		,Parte.NumeroParte
		,Parte.NombreParte AS Descripcion
		,ParteImagen.NombreImagen
		,(SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteExistencia.ParteID = ParteEquivalente.ParteID AND ParteExistencia.SucursalID = 1) AS Matriz
		,(SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteExistencia.ParteID = ParteEquivalente.ParteID AND ParteExistencia.SucursalID = 2) AS Suc02
		,(SELECT SUM(Existencia) FROM ParteExistencia WHERE ParteExistencia.ParteID = ParteEquivalente.ParteID AND ParteExistencia.SucursalID = 3) AS Suc03
	FROM
		ParteEquivalente
		INNER JOIN Parte ON Parte.ParteID = ParteEquivalente.ParteIDequivalente
		LEFT JOIN ParteImagen ON ParteImagen.ParteID = ParteEquivalente.ParteIDequivalente AND ParteImagen.Orden = 1
	WHERE
		ParteEquivalente.Estatus = 1
GO

ALTER VIEW [dbo].[PartesView] AS
SELECT TOP 1000000000000
	Parte.ParteID
	,Parte.NumeroParte 
	,Parte.NombreParte AS Descripcion
	,MarcaParte.NombreMarcaParte AS Marca
	,Linea.NombreLinea AS Linea
	,MAX(CASE 
	WHEN (Linea.Alto = 1 AND Parte.Alto IS NULL) OR 
		(Linea.Diametro = 1 AND Parte.Diametro IS NULL) OR
		(Linea.Largo = 1 AND Parte.Largo IS NULL) OR
		(Linea.Dientes = 1 AND Parte.Dientes IS NULL) OR
		(Linea.Astrias = 1 AND Parte.Astrias IS NULL) OR
		(Linea.Sistema = 1 AND Parte.ParteSistemaID IS NULL) OR	
		(Linea.Amperaje = 1 AND Parte.Amperes IS NULL) OR
		(Linea.Voltaje = 1 AND Parte.Voltios IS NULL) OR
		(Linea.Watts = 1 AND Parte.Watts IS NULL) OR
		(Linea.Ubicacion = 1 AND Parte.ParteUbicacionID IS NULL) OR
		(Linea.Terminales = 1 AND Parte.Terminales IS NULL) THEN 'SI' ELSE 'NO' END) AS FaltanCaracteristicas
	,ParteEstatus.ParteEstatusID
	,Parte.LineaID
	,MAX(CASE WHEN Parte.ParteID = ParteVehiculo.ParteID THEN 'SI' ELSE 'NO' END) AS Aplicacion
	,MAX(CASE WHEN Parte.ParteID = ParteEquivalente.ParteID THEN 'SI' ELSE 'NO' END) AS Equivalente
	,Parte.FechaRegistro
	,Parte.NumeroParte 
	+ Parte.NombreParte
	+ MarcaParte.NombreMarcaParte
	+ Linea.NombreLinea AS Busqueda
FROM 
	Parte
	INNER JOIN Linea ON Linea.LineaID = Parte.LineaID
	INNER JOIN MarcaParte ON MarcaParte.MarcaParteID = Parte.MarcaParteID
	INNER JOIN ParteEstatus ON ParteEstatus.ParteEstatusID = Parte.ParteEstatusID
	LEFT JOIN ParteVehiculo ON ParteVehiculo.ParteID = Parte.ParteID
	LEFT JOIN ParteEquivalente ON ParteEquivalente.ParteID = Parte.ParteID	
WHERE
	Parte.Estatus = 1
GROUP BY
	Parte.ParteID
	,Parte.NumeroParte 
	,Parte.NombreParte
	,MarcaParte.NombreMarcaParte
	,ParteEstatus.ParteEstatusID
	,Linea.NombreLinea
	,Parte.LineaID
	,Parte.FechaRegistro
ORDER BY 
	Parte.NombreParte
GO

ALTER VIEW PartesDetalleView AS
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
		, p.EsPar
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
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO
-- EXEC pauParteBusquedaAvanzada 'uno'
ALTER PROCEDURE pauParteBusquedaAvanzada (
	@Codigo NVARCHAR(32) = NULL
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
	, @TipoCilindroID INT = NULL
	, @Largo INT = NULL
	, @Alto INT = NULL
	, @Dientes INT = NULL
	, @Amperes INT = NULL
	, @Watts INT = NULL
	, @Diametro INT = NULL
	, @Astrias INT = NULL
	, @Terminales INT = NULL
	, @Voltios INT = NULL
	, @CodigoAlterno NVARCHAR(32) = NULL

	, @VehiculoModeloID INT = NULL -- Se debe incluir el ModeloID para que el filtro por vehículo tenga efecto
	, @VehiculoAnio INT = NULL
	, @VehiculoMotorID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @IdTipoFuenteMostrador INT = 4

	IF @Codigo IS NULL BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, l.NombreLinea
			, mp.NombreMarcaParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			-- LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
			-- LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID AND pca.Estatus = 1
		WHERE
			p.Estatus = 1
			AND (@Descripcion1 IS NULL OR p.NombreParte LIKE '%' + @Descripcion1 + '%')
			AND (@Descripcion2 IS NULL OR p.NombreParte LIKE '%' + @Descripcion2 + '%')
			AND (@Descripcion3 IS NULL OR p.NombreParte LIKE '%' + @Descripcion3 + '%')
			AND (@Descripcion4 IS NULL OR p.NombreParte LIKE '%' + @Descripcion4 + '%')
			AND (@Descripcion5 IS NULL OR p.NombreParte LIKE '%' + @Descripcion5 + '%')
			AND (@Descripcion6 IS NULL OR p.NombreParte LIKE '%' + @Descripcion6 + '%')
			AND (@Descripcion7 IS NULL OR p.NombreParte LIKE '%' + @Descripcion7 + '%')
			AND (@Descripcion8 IS NULL OR p.NombreParte LIKE '%' + @Descripcion8 + '%')
			AND (@Descripcion9 IS NULL OR p.NombreParte LIKE '%' + @Descripcion9 + '%')
			
			AND (@SistemaID IS NULL OR p.SubsistemaID IN (
				SELECT SubsistemaID
				FROM Subsistema
				WHERE SistemaID = @SistemaID AND Estatus = 1
				))
			AND (@LineaID IS NULL OR p.LineaID = @LineaID)
			AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
			AND (@TipoCilindroID IS NULL OR p.TipoCilindroID = @TipoCilindroID)
			AND (@Largo IS NULL OR p.Largo = @Largo)
			AND (@Alto IS NULL OR p.Alto = @Alto)
			AND (@Dientes IS NULL OR p.Dientes = @Dientes)
			AND (@Amperes IS NULL OR p.Amperes = @Amperes)
			AND (@Watts IS NULL OR p.Watts = @Watts)
			AND (@Diametro IS NULL OR p.Diametro = @Diametro)
			AND (@Astrias IS NULL OR p.Astrias = @Astrias)
			AND (@Terminales IS NULL OR p.Terminales = @Terminales)
			AND (@Voltios IS NULL OR p.Voltios = @Voltios)
			
			AND (@CodigoAlterno IS NULL OR p.ParteID IN (
				SELECT DISTINCT ParteID
				FROM ParteCodigoAlterno
				WHERE CodigoAlterno LIKE '%' + @CodigoAlterno + '%' AND Estatus = 1
			))
			
			AND (@VehiculoModeloID IS NULL OR p.ParteID IN (
				SELECT ParteID
				FROM ParteVehiculo
				WHERE
					TipoFuenteID <> @IdTipoFuenteMostrador
					AND ModeloID = @VehiculoModeloID
					AND (@VehiculoMotorID IS NULL OR MotorID = @VehiculoMotorID)
					AND (@VehiculoAnio IS NULL OR Anio = @VehiculoAnio)
			))
	END ELSE BEGIN
		SELECT
			p.ParteID
			, p.NumeroParte
			, p.NombreParte
			, l.NombreLinea
			, mp.NombreMarcaParte
			, pi.NombreImagen
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
		WHERE
			p.Estatus = 1
			AND (NumeroParte = @Codigo
			OR CodigoBarra = @Codigo)
	END

END
GO

-- EXEC pauPartesMaxMin 1, '2013-02-01', '2014-02-28'
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
		, p.EsPar
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
		, p.EsPar
		, pmm.VentasGlobales

END
GO