/* Script con modificaciones para el módulo de ventas. Archivo 27
 * Creado: 2014/02/07
 * Subido: 2014/02/07
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	ALTER TABLE VentaDetalle ALTER COLUMN PrecioUnitario DECIMAL(14, 4) NOT NULL
	ALTER TABLE VentaDetalle ALTER COLUMN Iva DECIMAL(14, 4) NOT NULL

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
-- EXEC pauPartesAbc '2013-03-01', '2014-02-28'
ALTER PROCEDURE pauPartesAbc (
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
	DECLARE @CalIva DECIMAL(4, 2) = CONVERT(DECIMAL(4, 2), '1.' + (SELECT Valor FROM Configuracion WHERE Nombre = 'Iva'))

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
		, ISNULL(ROUND((pp.PrecioUno / @CalIva) - pp.Costo, 2), 0.00) AS UtilidadUniP1
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
