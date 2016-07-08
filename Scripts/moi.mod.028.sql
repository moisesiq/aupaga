/* Script con modificaciones a la base de datos de Theos. Archivo 028
 * Creado: 2015/08/17
 * Subido: 2015/08/18
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE Venta ADD FacturaDividida BIT NULL

INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
	('Administracion.Movimientos.Ver', 'No tienes permisos para acceder a la opción de Movimientos.')

INSERT INTO Configuracion (Nombre, Valor, ValorPredeterminado, Descripcion) VALUES
	('Facturacion.ImporteDividir', '2000', '2000', 'Importe a partir del cual se puede dividir una factura.')

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
***************************************************************************** */

GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO