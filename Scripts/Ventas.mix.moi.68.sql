/* Script con modificaciones para el módulo de ventas. Archivo 68
 * Creado: 2014/11/26
 * Subido: 2014/11/26
 */

----------------------------------------------------------------------------------- Código de A
Alter table Cliente
ADD LatLong nvarchar (64)
alter table Proveedor
Add HoraMaxima Time

Insert into Configuracion (Nombre, Valor,ValorPredeterminado ,Descripcion, SucursalID)
values('Ventas.ImportePato' ,'1000', '1000','Importe minimo para mostrar animación de matar el pato', 0)

GO

-- DROP PROCEDURE [dbo].[pauParteBusquedaGeneral]
CREATE PROCEDURE [dbo].[pauParteBusquedaGeneral] (
	 @Descripcion1 NVARCHAR(32) = NULL
	, @Descripcion2 NVARCHAR(32) = NULL
	, @Descripcion3 NVARCHAR(32) = NULL
	, @Descripcion4 NVARCHAR(32) = NULL
	, @Descripcion5 NVARCHAR(32) = NULL
	, @Descripcion6 NVARCHAR(32) = NULL
	, @Descripcion7 NVARCHAR(32) = NULL
	, @Descripcion8 NVARCHAR(32) = NULL
	, @Descripcion9 NVARCHAR(32) = NULL
	
) AS BEGIN
	SET NOCOUNT ON

	SELECT 
		p.ParteID
		, p.NumeroParte AS NumeroDeParte
		, p.NombreParte AS Descripcion
		, l.NombreLinea AS Linea
		, mp.NombreMarcaParte AS Marca
		, pv.NombreProveedor AS Proveedor
		, p.FechaRegistro
	FROM
		Parte p
		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1
		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1
		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1
	WHERE
		p.ParteID 
		IN (
			SELECT DISTINCT
				Parte.ParteID
			FROM	
				Parte
				INNER JOIN Linea ON Parte.LineaID = Linea.LineaID
				INNER JOIN MarcaParte ON Parte.MarcaParteID = MarcaParte.MarcaParteID
				INNER JOIN Proveedor ON Parte.ProveedorID = Proveedor.ProveedorID	
			WHERE
				(	
					(
						(@Descripcion1 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion1 + '%')
						OR(@Descripcion1 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion1 + '%')
						OR(@Descripcion1 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion1 + '%')
						OR(@Descripcion1 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion1 + '%')
						OR(@Descripcion1 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion1 + '%')
					)
					
					AND
					(	(@Descripcion2 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion2 + '%')
						OR (@Descripcion2 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion2 + '%')
						OR(@Descripcion2 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion2 + '%')
						OR(@Descripcion2 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion2 + '%')
						OR(@Descripcion2 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion2 + '%')
						
					)
					AND
					(
						(@Descripcion3 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion3 + '%')
						OR (@Descripcion3 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion3 + '%')
						OR (@Descripcion3 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion3 + '%')
						OR (@Descripcion3 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion3 + '%')
						OR (@Descripcion3 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion3 + '%')
					)
					
					AND
					(
						(@Descripcion4 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion4 + '%')
						OR (@Descripcion4 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion4 + '%')
						OR (@Descripcion4 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion4 + '%')
						OR (@Descripcion4 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion4 + '%')
						OR (@Descripcion4 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion4 + '%')
					)
					
					AND
					(
						(@Descripcion5 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion5 + '%')
						OR(@Descripcion5 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion5 + '%')
						OR(@Descripcion5 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion5 + '%')
						OR(@Descripcion5 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion5 + '%')
						OR(@Descripcion5 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion5 + '%')
					)
					
					AND
					(
						(@Descripcion6 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion6 + '%')
						OR(@Descripcion6 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion6 + '%')
						OR(@Descripcion6 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion6 + '%')
						OR(@Descripcion6 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion6 + '%')
						OR(@Descripcion6 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion6 + '%')
					)
					
					AND
					(
						(@Descripcion7 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion7 + '%')
						OR (@Descripcion7 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion7 + '%')
						OR (@Descripcion7 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion7 + '%')
						OR (@Descripcion7 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion7 + '%')
						OR (@Descripcion7 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion7 + '%')
					)
					
					AND
					(
						(@Descripcion8 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion8 + '%')
						OR (@Descripcion8 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion8 + '%')
						OR (@Descripcion8 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion8 + '%')
						OR (@Descripcion8 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion8 + '%')							
						OR (@Descripcion8 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion8 + '%')
					)
					
					AND
					(
						(@Descripcion9 IS NULL OR Parte.NombreParte LIKE '%' + @Descripcion9 + '%')
						OR (@Descripcion9 IS NULL OR Parte.NumeroParte LIKE '%' + @Descripcion9 + '%')
						OR (@Descripcion9 IS NULL OR Linea.NombreLinea LIKE '%' + @Descripcion9 + '%')
						OR (@Descripcion9 IS NULL OR MarcaParte.NombreMarcaParte LIKE '%' + @Descripcion9 + '%')
						OR (@Descripcion9 IS NULL OR Proveedor.NombreProveedor LIKE '%' + @Descripcion9 + '%')
					)
				
					
				)
			)	
END
GO
----------------------------------------------------------------------------------- Código de A


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

