/* Script con modificaciones a la base de datos de Theos. Archivo 062
 * Creado: 2016/03/03
 * Subido: 2016/03/03
 */

DECLARE @ScriptID INT = 62
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- Se borran los duplicados de las aplicaciones
DELETE FROM ParteVehiculo WHERE ParteVehiculoID IN (
	SELECT ParteVehiculoID FROM (
	SELECT
		ROW_NUMBER() OVER (PARTITION BY ParteID, ModeloID, Anio, MotorID ORDER BY ParteID, ModeloID, Anio, MotorID) AS Reg
		, ParteID
		, ModeloID
		, Anio
		, MotorID
		, ParteVehiculoID
	FROM ParteVehiculo
	-- order by ParteID, ModeloID, Anio, MotorID
	) c WHERE Reg != 1
)
-- Se crea constraint para que no vuelva a suceder
ALTER TABLE ParteVehiculo ADD CONSTRAINT Un_ParteVehiculo_ParteID_ModeloID_Anio_MotorID UNIQUE (ParteID, ModeloID, Anio, MotorID)
ALTER TABLE ParteVehiculo ADD CONSTRAINT Df_ParteVehiculo_FechaRegistro DEFAULT GETDATE() FOR FechaRegistro

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

