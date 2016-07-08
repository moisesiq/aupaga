/* Script con modificaciones para el módulo de ventas. Archivo 39
 * Creado: 2014/04/30
 * Subido: 2014/04/30
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	
	
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
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

-- EXEC pauPartesMaxMinEquivalentes '2013-04-01', '2014-03-31', 1
ALTER PROCEDURE pauPartesMaxMinEquivalentes (
	@Desde DATE
	, @Hasta DATE
	, @SucursalID INT
	, @ProveedorID INT = NULL
	, @MarcaID INT = NULL
	, @LineaID INT = NULL
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2013-01-01'
	DECLARE @Hasta DATE = '2014-01-31'
	*/

	-- Definición de variables tipo constante
	DECLARE @EstPagadaID INT = 3

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		pe.ParteEquivalenteID
		, pe.GrupoID
		, pe.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, pv.NombreProveedor AS Proveedor
		, mr.NombreMarcaParte AS Marca
		-- , ISNULL(SUM(vd.Cantidad), 0.00) AS Cantidad
		, ISNULL(COUNT(v.VentaID), 0) AS Ventas
		, ISNULL(pp.CostoConDescuento, pp.Costo) AS CostoConDescuento
		, px.Existencia
		, pmm.Maximo
		, pmm.Minimo
		, pmm.Calcular
		, pmm.FechaModificacion
	FROM
		ParteEquivalente pe
		INNER JOIN Parte p ON p.ParteID = pe.ParteID AND p.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = pe.ParteID AND pp.Estatus = 1
		LEFT JOIN ParteExistencia px ON px.ParteID = pe.ParteID AND px.SucursalID = @SucursalID AND px.Estatus = 1
		LEFT JOIN ParteMaxMin pmm ON pmm.ParteID = pe.ParteID AND pmm.SucursalID = @SucursalID
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mr ON mr.MarcaParteID = p.MarcaParteID AND mr.Estatus = 1
		LEFT JOIN VentaDetalle vd ON vd.ParteID = pe.ParteID AND vd.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = vd.VentaID
			AND v.SucursalID = @SucursalID
			AND v.VentaEstatusID = @EstPagadaID
			AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
			AND v.Estatus = 1
	WHERE
		(@ProveedorID IS NULL OR p.ProveedorID = @ProveedorID)
		AND (@MarcaID IS NULL OR p.MarcaParteID = @MarcaID)
		AND (@LineaID IS NULL OR p.LineaID = @LineaID)
	GROUP BY
		pe.ParteEquivalenteID
		, pe.GrupoID
		, pe.ParteID
		, p.NumeroParte
		, p.NombreParte
		, pv.NombreProveedor
		, mr.NombreMarcaParte
		, pp.CostoConDescuento
		, pp.Costo
		, px.Existencia
		, pmm.Maximo
		, pmm.Minimo
		, pmm.Calcular
		, pmm.FechaModificacion
	ORDER BY
		pe.GrupoID
		, p.NumeroParte
END
GO