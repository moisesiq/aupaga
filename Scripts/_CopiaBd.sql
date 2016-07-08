/* *****************************************************************************
** Código para crear una copia de ControlRefaccionaria y ejecutar el script
** sin afectar los datos reales
***************************************************************************** */
DECLARE @RutaResp NVARCHAR(32) = 'C:\tmp\CR.bak'
DECLARE @RutaDatos NVARCHAR(256) = 'C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\'
DECLARE @Original NVARCHAR(32) = 'ControlRefaccionaria'
DECLARE @OriginalLog NVARCHAR(32) = 'ControlRefaccionaria_log'
DECLARE @Copia NVARCHAR(32) = '_CrPrueba'
DECLARE @CopiaA NVARCHAR(256) = @RutaDatos + '_CrPrueba.mdf'
DECLARE @CopiaALog NVARCHAR(256) = @RutaDatos + '_CrPrueba.ldf'

BACKUP DATABASE @Original TO DISK = @RutaResp

RESTORE DATABASE @Copia FROM DISK = @RutaResp
	WITH MOVE @Original TO @CopiaA
	, MOVE @OriginalLog TO @CopiaALog

-- Se hace una segunda copia
SET @Copia = '_CrPrueba2'
SET @CopiaA = @RutaDatos + '_CrPrueba2.mdf'
SET @CopiaALog = @RutaDatos + '_CrPrueba2.ldf'
RESTORE DATABASE @Copia FROM DISK = @RutaResp
	WITH MOVE @Original TO @CopiaA
	, MOVE @OriginalLog TO @CopiaALog

GO

/* *****************************************************************************
** Fin prueba
***************************************************************************** */