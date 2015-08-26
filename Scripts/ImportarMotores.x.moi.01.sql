/* *****************************************************************************
Script para importar los datos de Vehículos (Marcas, Modelos, Motores, Anios)

Se toma en cuenta que en la base de datos ya exisntes las tablas necesarias
con la nueva estructura.

También debe existir ya la tabla fuente, desde donde se obtendrá toda la información.
Dicha tabla debe llamarse "_Motores"
***************************************************************************** */

USE ControlRefaccionaria
GO

-- Se verifica si se debe agregar la columna "ModeloID" a la tabla _Motores
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('_Motores')
AND name = 'ModeloID') BEGIN
	ALTER TABLE _Motores ADD ModeloID INT NOT NULL
END
GO

-- Se borran los datos de las tablas a afectar y se restauran los "Identities"
DELETE FROM MotorAnio
DBCC CHECKIDENT (MotorAnio, RESEED, 0)
DELETE FROM Motor
DBCC CHECKIDENT (Motor, RESEED, 0)
ALTER TABLE ParteAplicacion DROP CONSTRAINT FK_ParteAplicacion_Modelo
DELETE FROM Modelo
ALTER TABLE ParteAplicacion ADD CONSTRAINT FK_ParteAplicacion_Modelo FOREIGN KEY (ModeloID) REFERENCES Modelo(ModeloID)
DBCC CHECKIDENT (Modelo, RESEED, 0)
DELETE FROM Marca
DBCC CHECKIDENT (Marca, RESEED, 0)

-- Se comienza con el proceso
BEGIN TRAN
BEGIN TRY

DECLARE @Hoy DATETIME = GETDATE()
DECLARE @UsuarioID INT = 1
DECLARE @Estatus BIT = 1
DECLARE @Actualizar BIT = 1

-- Marcas
INSERT INTO Marca (NombreMarca, UsuarioID, FechaRegistro, Estatus, Actualizar)
	SELECT DISTINCT
		Marca
		, @UsuarioID
		, @Hoy
		, @Estatus
		, @Actualizar
	FROM _Motores

-- Modelos
INSERT INTO Modelo (MarcaID, NombreModelo, UsuarioID, FechaRegistro, Estatus, Actualizar)
	SELECT
		mr.MarcaID
		, m.Modelo
		, @UsuarioID
		, @Hoy
		, @Estatus
		, @Actualizar
	FROM
		_Motores m
		LEFT JOIN Marca mr ON mr.NombreMarca = m.Marca

UPDATE _Motores SET ModeloID = md.ModeloID FROM
	_Motores m
	INNER JOIN Modelo md ON md.NombreModelo = m.Modelo

-- Se empiezan a recorrer los registros para sacar los motores y los años
DECLARE @Motores INT = 17  -- Número fijo de columnas para motores
DECLARE @Modelos INT = (SELECT MAX(ModeloID) FROM Modelo)
DECLARE @Registro INT = 1
DECLARE @Columna INT
DECLARE @Motor NVARCHAR(8)
DECLARE @AnioIni INT
DECLARE @AnioFin INT
DECLARE @ColumnaC NVARCHAR(2)
DECLARE @Sql NVARCHAR(128)
DECLARE @MotorID INT
DECLARE @CuentaAnio INT

WHILE (@Registro <= @Modelos) BEGIN
	SET @Columna = 0

	SELECT
		@AnioIni = "Año 1"
		, @AnioFin = "Año 2"
	FROM _Motores
	WHERE ModeloID = @Registro
	
	WHILE (@Columna <= @Motores) BEGIN
		SET @ColumnaC = CASE WHEN @Columna = 0 THEN '' ELSE CONVERT(VARCHAR(2), @Columna) END
		SET @Sql = 'SELECT @MotorS = MOTORES' + @ColumnaC + 
			' FROM _Motores WHERE ModeloID = ' + CONVERT(NVARCHAR(4), @Registro)
		EXEC sp_executesql @Sql, N'@MotorS NVARCHAR(8) OUTPUT', @MotorS=@Motor OUTPUT

		IF @Motor IS NULL OR @Motor = '' BEGIN
			SET @Columna = @Columna + 1
			CONTINUE
		END

		-- Se inserta el motor
		INSERT INTO Motor (ModeloID, NombreMotor, UsuarioID, FechaRegistro, Estatus, Actualizar) VALUES
			(@Registro, @Motor, @UsuarioID, @Hoy, @Estatus, @Actualizar)

		SET @MotorID = @@IDENTITY
		SET @CuentaAnio = @AnioIni

		-- Se insertan los años
		WHILE (@CuentaAnio <= @AnioFin) BEGIN
			INSERT INTO MotorAnio (MotorID, Anio, UsuarioID, FechaRegistro, Estatus, Actualizar) VALUES
				(@MotorID, @CuentaAnio, @UsuarioID, @Hoy, @Estatus, @Actualizar)
			SET @CuentaAnio = @CuentaAnio + 1
		END
		
		SET @Columna = @Columna + 1
	END
	
	SET @Registro = @Registro + 1
END

COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN	
END CATCH