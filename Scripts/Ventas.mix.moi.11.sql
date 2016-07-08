/* Script con modificaciones para el módulo de ventas. Archivo 11
 * Creado: 2013/12/09
 * Subido: 2013/12/10
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	CREATE TABLE VentaFacturaDevolucion (
		VentaFacturaDevolucionID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaFacturaID INT NOT NULL FOREIGN KEY REFERENCES VentaFactura(VentaFacturaID)
		, Fecha DATETIME NOT NULL
		, SucursalID INT NOT NULL FOREIGN KEY REFERENCES Sucursal(SucursalID)
		, EsCancelacion BIT NOT NULL
		, FolioFiscal NVARCHAR(36) NULL
		, Serie NVARCHAR(8) NULL
		, Folio NVARCHAR(16) NULL

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	CREATE TABLE VentaFacturaDevolucionDetalle (
		VentaFacturaDevolucionDetalleID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		, VentaFacturaDevolucionID INT NOT NULL FOREIGN KEY REFERENCES VentaFacturaDevolucion(VentaFacturaDevolucionID)
		, VentaDevolucionID INT NOT NULL FOREIGN KEY REFERENCES VentaDevolucion(VentaDevolucionID)

		, UsuarioID INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
		, FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
		, FechaModificacion DATETIME
		, Estatus BIT NOT NULL DEFAULT 1
		, Actualizar BIT NOT NULL DEFAULT 1
	)

	INSERT INTO AutorizacionProceso (Descripcion, UsuarioID)
		VALUES ('USAR NOTA DE CRÉDITO DE OTRO CLIENTE', 1)

	DELETE FROM Configuracion WHERE Nombre LIKE 'FacturacionElectronica.Ruta%'
	UPDATE Configuracion SET SucursalID = 0 WHERE Nombre IN (
		'Facturacion.RutaCertificado'
		, 'Facturacion.RutaArchivoKey'
		, 'Facturacion.ContraseniaArchivoKey'
		, 'Facturacion.RutaArchivoPfx'
		, 'Facturacion.ContraseniaArchivoPfx'
		, 'Facturacion.UsuarioPac'
		, 'Facturacion.ContraseniaPac'
		, 'Facturacion.Rfc'
		, 'Facturacion.RazonSocial'
		, 'Facturacion.RegimenesFiscales'
	)
	UPDATE Configuracion SET Valor = 'Archivos\CFDI\' WHERE Nombre IN ('Facturacion.RutaPdf', 'Facturacion.RutaXml')
	INSERT INTO Configuracion (Nombre, Valor, Descripcion, SucursalID)
		SELECT Nombre, Valor, Descripcion, 2 FROM Configuracion WHERE Nombre IN (
			'Facturacion.Calle'
			, 'Facturacion.NumeroExterior'
			, 'Facturacion.NumeroInterior'
			, 'Facturacion.Referencia'
			, 'Facturacion.Colonia'
			, 'Facturacion.CodigoPostal'
			, 'Facturacion.Localidad'
			, 'Facturacion.Municipio'
			, 'Facturacion.Estado'
			, 'Facturacion.Pais'
		)
		UNION
		SELECT Nombre, Valor, Descripcion, 3 FROM Configuracion WHERE Nombre IN (
			'Facturacion.Calle'
			, 'Facturacion.NumeroExterior'
			, 'Facturacion.NumeroInterior'
			, 'Facturacion.Referencia'
			, 'Facturacion.Colonia'
			, 'Facturacion.CodigoPostal'
			, 'Facturacion.Localidad'
			, 'Facturacion.Municipio'
			, 'Facturacion.Estado'
			, 'Facturacion.Pais'
		)
	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion, SucursalID) VALUES
		('Facturacion.Serie', 'PER', 'PER', 'Serie a utilizar al facturar una venta.', 2)
		, ('Facturacion.Folio', '1', '1', 'Siguiente folio a utilizar al facturar una venta.', 2)
		, ('Facturacion.Serie', 'IMSS', 'IMSS', 'Serie a utilizar al facturar una venta.', 3)
		, ('Facturacion.Folio', '1', '1', 'Siguiente folio a utilizar al facturar una venta.', 3)
		, ('Facturacion.Devolucion.Serie', 'NC00', 'NC00', 'Serie a utilizar en factura al generar una nota de crédito por devolución.', 1)
		, ('Facturacion.Devolucion.Folio', '1', '1', 'Siguiente folio a utilizar en factura al generar una nota de crédito por devolución.', 1)
		, ('Facturacion.Devolucion.Serie', 'NC02', 'NC02', 'Serie a utilizar en factura al generar una nota de crédito por devolución.', 2)
		, ('Facturacion.Devolucion.Folio', '1', '1', 'Siguiente folio a utilizar en factura al generar una nota de crédito por devolución.', 2)
		, ('Facturacion.Devolucion.Serie', 'NC03', 'NC03', 'Serie a utilizar en factura al generar una nota de crédito por devolución.', 3)
		, ('Facturacion.Devolucion.Folio', '1', '1', 'Siguiente folio a utilizar en factura al generar una nota de crédito por devolución.', 3)
		
		, ('Facturacion.Devolucion.RutaPdf', 'Archivos\NC\', NULL, 'Ruta donde se almacenan los archivos Pdf correspondientes a las facturas electrónicas de devoluciones (notas de crédito).', 0)
		, ('Facturacion.Devolucion.RutaXml', 'Archivos\NC\', NULL, 'Ruta donde se almacenan los archivos Xml correspondientes a las facturas electrónicas de devoluciones (notas de crédito).', 0)
		
		, ('Reportes.VentaFactura.Salida', 'D', 'I', 'Salida donde debe mostrarse una factura (D - Diseño, P - Pantalla, I - Impresora, N - Nada).', 0)
		, ('Reportes.VentaFacturaDevolucion.Salida', 'D', 'I', 'Salida donde debe mostrarse una factura de devolución (nota de crédito) (D - Diseño, P - Pantalla, I - Impresora, N - Nada).', 0)
	INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
		('Correo.Servidor', 'smtp.live.com', 'smtp.live.com', 'Servidor de correos a utilizar para el envío de correos electrónicos.')
		, ('Correo.Puerto', '587', '587', 'Puerto del servidor para el envío de correos electrónicos.')
		, ('Correo.Ssl', 'V', 'V', 'Valor que indica si el servidor de correos utiliza seguridad SSL.')
		, ('Correo.Usuario', 'autopartes.garibaldi03@hotmail.com', 'autopartes.garibaldi03@hotmail.com', 'Nombre de usuario de la cuenta a utilizar para el envío de correos elecrónicos.')
		, ('Correo.Contrasenia', 'admin321', 'admin321', 'Contraseña de la cuenta a utilizar para el envío de correos elecrónicos.')
		, ('Correo.Nombre', 'Autopartes Garibaldi', 'Autopartes Garibaldi', 'Nombre que se visualiza como remitente al enviar un correo.')
		, ('Correo.RutaFormatos', 'Formatos\', 'Formatos\', 'Ruta donde se guardan los formatos a utilizar para el envío de correos.')

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

DROP VIEW BancosView

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

