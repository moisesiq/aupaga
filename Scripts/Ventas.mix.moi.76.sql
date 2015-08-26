/* Script con modificaciones para el módulo de ventas. Archivo 76
 * Creado: 2014/12/24
 * Subido: 2014/12/26
 */


----------------------------------------------------------------------------------- Código de André

----------------------------------------------------------------------------------- Código de André


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

ALTER VIEW PartesDiferenciasEnExistenciaView AS
	SELECT * FROM (
		SELECT
			pk.ParteID
			, pk.SucursalID
			, s.NombreSucursal AS Sucursal
			, p.NumeroParte as NumeroDeParte
			, p.NombreParte AS Descripcion
			, pk.ExistenciaNueva AS ExistenciaKardex
			, pe.Existencia
			, (pk.ExistenciaNueva - pe.Existencia) AS Diferencia
			-- , ROW_NUMBER() OVER (PARTITION BY pk.ParteID, pk.SucursalID ORDER BY pk.Fecha DESC) AS Orden
		 FROM
			(SELECT * FROM (
				SELECT
					ParteID
					, SucursalID
					, ExistenciaNueva
					, ROW_NUMBER() OVER (PARTITION BY ParteID, SucursalID ORDER BY Fecha DESC, ParteKardexID DESC) AS Orden
				FROM ParteKardex
			 ) c WHERE Orden = 1) pk
			 LEFT JOIN ParteExistencia pe on pe.ParteID = pk.ParteID AND pe.SucursalID = pk.SucursalID AND pe.Estatus = 1
			 LEFT JOIN Parte p on p.ParteID = pk.ParteID AND p.Estatus = 1
			 LEFT JOIN Sucursal s ON s.SucursalID = pk.SucursalID AND s.Estatus = 1
	) c
	WHERE c.Diferencia != 0
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

