/* Script con modificaciones a la base de datos de Theos. Archivo 030
 * Creado: 2015/08/24
 * Subido: 2015/08/24
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
	('Administracion.CatalogosPartes.Parte.Ver', 'No tienes permisos para acceder a la opción de Partes.')
	, ('Administracion.CatalogosPartes.Kardex.Ver', 'No tienes permisos para acceder a la opción de Kardex.')
	, ('Administracion.CatalogosPartes.Avance.Ver', 'No tienes permisos para acceder a la opción de Avance de Partes.')
	, ('Administracion.CatalogosPartes.Errores.Ver', 'No tienes permisos para acceder a la opción de Errores en Partes.')

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