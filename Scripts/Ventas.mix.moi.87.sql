/* Script con modificaciones para el módulo de ventas. Archivo 87
 * Creado: 2015/02/06
 * Subido: 2015/02/07
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Actualizacion.RutaImagenes', '\\192.168.1.71\Img\Partes\', '', 'Ruta donde se guardan las imágenes más actuales de las partes.')

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
					WHERE ParteID = p.ParteID AND Estatus = 1)) AS Equivalentes
				, (SELECT TOP 1 1 FROM ParteVehiculo WHERE ParteID = p.ParteID AND Estatus = 1) AS Aplicaciones
				, (SELECT TOP 1 1 FROM ParteCodigoAlterno WHERE ParteID = p.ParteID AND Estatus = 1) AS Alternos
				, (SELECT TOP 1 1 FROM ParteComplementaria WHERE ParteID = p.ParteID) AS Complementarios
				, (SELECT TOP 1 1 FROM ParteCaracteristica WHERE ParteID = p.ParteID) AS Caracteristicas
			FROM Parte p
			WHERE p.Estatus = 1
		) pc
		LEFT JOIN Proveedor pv ON pv.ProveedorID = pc.ProveedorID AND pv.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = pc.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Linea l ON l.LineaID = pc.LineaID AND l.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

