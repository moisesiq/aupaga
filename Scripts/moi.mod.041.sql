/* Script con modificaciones a la base de datos de Theos. Archivo 041
 * Creado: 2015/10/18
 * Subido: 2015/10/19
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE ContaEgresoDevengadoEspecial ADD RealizoUsuarioID INT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
GO
UPDATE ContaEgresoDevengadoEspecial SET RealizoUsuarioID = 1
ALTER TABLE ContaEgresoDevengadoEspecial ALTER COLUMN RealizoUsuarioID INT NOT NULL

ALTER TABLE ContaCuentaAuxiliar ADD DevengarAutEsp BIT NULL
GO
UPDATE ContaCuentaAuxiliar SET DevengarAut = 0, DevengarAutEsp = 1 WHERE ContaCuentaAuxiliarID IN 
	(SELECT ContaCuentaAuxiliarID FROM ContaCuentaAuxiliarDevengadoEspecial)

/* ****************************************************************************
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

