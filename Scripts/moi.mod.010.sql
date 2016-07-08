/* Script con modificaciones a la base de datos de Theos. Archivo 010
 * Creado: 2015/05/14
 * Subido: 2015/05/14
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

CREATE TABLE TipoUsuario (
	TipoUsuarioID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, TipoDeUsuario NVARCHAR(32) NOT NULL
)
INSERT INTO TipoUsuario (TipoDeUsuario) VALUES
	('VENDEDOR')
	, ('GERENTE')

ALTER TABLE Usuario ADD TipoUsuarioID INT NULL FOREIGN KEY REFERENCES TipoUsuario(TipoUsuarioID)

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO



/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

