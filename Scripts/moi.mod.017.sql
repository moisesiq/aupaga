/* Script con modificaciones a la base de datos de Theos. Archivo 017
 * Creado: 2015/06/05
 * Subido: 2015/05/05
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE BancoCuenta ADD EsCpcp BIT NULL
GO
UPDATE BancoCuenta SET EsCpcp = CASE WHEN BancoCuentaID IN (1, 2) THEN 0 ELSE 1 END
ALTER TABLE BancoCuenta ALTER COLUMN EsCpcp BIT NOT NULL

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

