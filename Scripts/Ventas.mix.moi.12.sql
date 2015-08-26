/* Script con modificaciones para el módulo de ventas. Archivo 12
 * Creado: 2013/12/11
 * Subido: 2013/12/12
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE INDEX Ix_Folio ON Venta(Folio)

	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Reportes.Cobranza.Salida', 'D', 'I', 'Salida donde debe mostrarse el reporte de cobranza de un cliente (D - Diseño, P - Pantalla, I - Impresora, N - Nada).')
		, ('Reportes.CobranzaTicket.Salida', 'D', 'I', 'Salida donde debe mostrarse el ticket de cobranza (D - Diseño, P - Pantalla, I - Impresora, N - Nada).')

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

ALTER VIEW Cotizaciones9500View AS
	SELECT
		c9.Cotizacion9500ID
		, c9.EstatusGenericoID
		, c9.Fecha
		, c9.SucursalID
		, (SELECT SUM(PrecioAlCliente) FROM Cotizacion9500Detalle 
			WHERE Cotizacion9500ID = c9.Cotizacion9500ID AND Estatus = 1) AS Total
		, c9.Anticipo
		, c9.ClienteID
		, c.Nombre AS Cliente
		, c.Celular
		, u.NombrePersona AS Vendedor
		, v.Folio AS AnticipoVentaFolio
	FROM
		Cotizacion9500 c9
		LEFT JOIN Cliente c ON c.ClienteID = c9.ClienteID AND c.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = c9.RealizoUsuarioID AND u.Estatus = 1
		LEFT JOIN Venta v ON v.VentaID = c9.AnticipoVentaID AND v.Estatus = 1
	WHERE c9.Estatus = 1
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

