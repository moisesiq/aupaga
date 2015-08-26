/* Script con modificaciones para el módulo de ventas. Archivo 34
 * Creado: 2014/02/28
 * Subido: 2014/04/01
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
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

DROP PROCEDURE pauParteBusquedaAvanzada
GO
-- DROP PROCEDURE pauVentasPartesBusqueda
-- EXEC pauVentasPartesBusqueda @SucursalID = 1, @Descripcion1 = 'una'
CREATE PROCEDURE pauVentasPartesBusqueda (
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
			, pe.Existencia
			, pic.NombreImagen
			, pic.CuentaImagenes
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			-- LEFT JOIN Subsistema ss ON ss.SubsistemaID = p.SubsistemaID AND ss.Estatus = 1
			-- LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1 AND pi.Estatus = 1
			LEFT JOIN (
				SELECT
					ParteID
					, MAX(CASE WHEN Orden = 1 THEN NombreImagen ELSE '' END) AS NombreImagen
					, COUNT(*) AS CuentaImagenes
				FROM ParteImagen
				WHERE Estatus = 1
				GROUP BY ParteID
			) pic ON pic.ParteID = p.ParteID
			-- LEFT JOIN ParteCodigoAlterno pca ON pca.ParteID = p.ParteID AND pca.Estatus = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
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
			, pe.Existencia
			, pi.NombreImagen
			, (SELECT COUNT(*) FROM ParteImagen WHERE ParteID = p.ParteID AND Estatus = 1) AS CuentaImagenes
		FROM
			Parte p
			LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
			LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
			LEFT JOIN ParteImagen pi ON pi.ParteID = p.ParteID AND pi.Orden = 1
			LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.SucursalID = @SucursalID AND pe.Estatus = 1
		WHERE
			p.Estatus = 1
			AND (NumeroParte = @Codigo
			OR CodigoBarra = @Codigo)
	END

END
GO