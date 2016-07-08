/* Script con modificaciones para el módulo de ventas. Archivo 80
 * Creado: 2015/01/12
 * Subido: 2015/01/13
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



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

-- DROP PROCEDURE pauVentasPartesPor
CREATE PROCEDURE pauVentasPartesPor (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	DECLARE @EstPagada INT = 3
	DECLARE @EstCobrada INT = 2
	
	SET @Hasta = DATEADD(DAY, 1, @Hasta)
	
	SELECT
		ISNULL(SUM(CASE WHEN vdc.Partes = 1 THEN 1 ELSE 0 END), 0) AS Uno
		, ISNULL(SUM(CASE WHEN vdc.Partes = 2 THEN 1 ELSE 0 END), 0) AS Dos
		, ISNULL(SUM(CASE WHEN vdc.Partes = 3 THEN 1 ELSE 0 END), 0) AS Tres
		, ISNULL(SUM(CASE WHEN vdc.Partes > 3 THEN 1 ELSE 0 END), 0) AS Mas
	FROM
		Venta v
		LEFT JOIN (
			SELECT VentaID, COUNT(*) AS Partes
			FROM VentaDetalle
			WHERE Estatus = 1
			GROUP BY VentaID
		) vdc ON vdc.VentaID = v.VentaID
	WHERE
		v.Estatus = 1
		AND v.VentaEstatusID IN (@EstCobrada, @EstPagada)
		AND (v.Fecha >= @Desde AND v.Fecha < @Hasta)
END