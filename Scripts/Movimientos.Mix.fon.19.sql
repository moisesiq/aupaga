/**/
ALTER VIEW [dbo].[MotoresView]
AS
SELECT ISNULL(ROW_NUMBER() OVER(ORDER BY MotorID), -1)  AS GenericoID, *
FROM
(
	SELECT DISTINCT TOP 1000000
		Modelo.ModeloID
		,Marca.MarcaID
		,Marca.NombreMarca
		,Modelo.NombreModelo		
		,Motor.MotorID
		,Motor.NombreMotor
		,STUFF((SELECT ', ' + CAST(m.Anio AS VARCHAR)
		FROM MotorAnio m	
		WHERE m.MotorID = Motor.MotorID
		ORDER BY m.Anio DESC
		FOR XML PATH('')
		), 1, 1, '') AS Anios		
		,Modelo.NombreModelo 
		+ Marca.NombreMarca 
		+ STUFF((SELECT ', ' + CAST(m.Anio AS VARCHAR)
		FROM MotorAnio m	
		WHERE m.MotorID = Motor.MotorID
		FOR XML PATH('')
		), 1, 1, '') AS Busqueda
	FROM 
		Modelo
		INNER JOIN Marca ON Marca.MarcaID = Modelo.MarcaID
		INNER JOIN Motor ON Motor.ModeloID = Modelo.ModeloID	
	WHERE
		Modelo.Estatus = 1
	ORDER BY 
		Marca.NombreMarca
		,Modelo.NombreModelo
)t
GO

/* Views Estado, Municipio, Ciudad */
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[EstadosView]'))
DROP VIEW [dbo].[EstadosView]
GO

CREATE VIEW EstadosView
AS
SELECT
	Estado.EstadoID
	,Estado.NombreEstado	
	,Estado.NombreEstado AS Busqueda
FROM
	Estado
WHERE
	Estado.Estatus = 1
GO	
--
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[MunicipiosView]'))
DROP VIEW [dbo].[MunicipiosView]
GO
CREATE VIEW MunicipiosView
AS
SELECT TOP 1000000
	Municipio.MunicipioID
	,Municipio.EstadoID
	,Estado.NombreEstado
	,Municipio.NombreMunicipio
	,Estado.NombreEstado
	+ Municipio.NombreMunicipio AS Busqueda
FROM
	Municipio
	INNER JOIN Estado ON Estado.EstadoID = Municipio.EstadoID
WHERE
	Municipio.Estatus = 1
ORDER BY 
	Estado.NombreEstado
	,Municipio.NombreMunicipio
GO
--
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[CiudadesView]'))
DROP VIEW [dbo].[CiudadesView]
GO
CREATE VIEW CiudadesView
AS
SELECT TOP 1000000
	Ciudad.CiudadID
	,Ciudad.MunicipioID
	,Estado.EstadoID
	,Estado.NombreEstado
	,Municipio.NombreMunicipio
	,Ciudad.NombreCiudad
	,Estado.NombreEstado
	+ Municipio.NombreMunicipio
	+ Ciudad.NombreCiudad AS Busqueda
FROM
	Ciudad
	INNER JOIN Municipio ON Municipio.MunicipioID = Ciudad.MunicipioID
	INNER JOIN Estado ON Estado.EstadoID = Municipio.EstadoID
WHERE
	Ciudad.Estatus = 1
ORDER BY
	Estado.NombreEstado
	,Municipio.NombreMunicipio
	,Ciudad.NombreCiudad
GO

/**/
UPDATE Banco SET Estatus = 1
GO

/**/
CREATE FUNCTION [dbo].[fnuClienteCreditoDeuda](
	@ClienteID AS INT,
	@TipoDeuda AS INT
)
RETURNS DECIMAL(18,2)
AS
BEGIN	

DECLARE @Deuda AS DECIMAL(18,2)
	--Deuda Actual
	IF(@TipoDeuda = 1)
	BEGIN
		SET @Deuda = (SELECT 0.0 AS Deuda)
	END
	
	--Deuda Vencida
	IF(@TipoDeuda = 2)
	BEGIN
		SET @Deuda = (SELECT 0.0 AS Deuda)
	END
	
	RETURN @Deuda
END

GO

/**/
CREATE FUNCTION [dbo].[fnuClientePromedioPago](
	@ClienteID AS INT,
	@TipoPromedio AS INT
)
RETURNS INT
AS
BEGIN	

DECLARE @Dias AS INT
	--Promedio Pago Total
	IF(@TipoPromedio = 1)
	BEGIN
		SET @Dias = (SELECT 0 AS Dias)
	END
	
	--Promedio Pago Total en tres meses
	IF(@TipoPromedio = 2)
	BEGIN
		SET @Dias = (SELECT 0 AS Dias)
	END
	
	RETURN @Dias
END

GO

/**/
ALTER VIEW [dbo].[ClientesView]
AS
SELECT
	Cliente.ClienteID
	,ClienteFacturacion.Rfc
	,Cliente.Nombre
	,Cliente.Calle
	,Cliente.NumeroInterior
	,Cliente.NumeroExterior
	,Cliente.Colonia
	,Cliente.CodigoPostal
	,Cliente.EstadoID
	,Estado.NombreEstado
	,Cliente.MunicipioID
	,Municipio.NombreMunicipio
	,Cliente.CiudadID
	,Ciudad.NombreCiudad
	,Cliente.Telefono
	,Cliente.Celular
	,Cliente.Particular
	,Cliente.Nextel
	,Cliente.ListaDePrecios
	,ISNULL(Cliente.TieneCredito, 0) AS TieneCredito
	,Cliente.Credito
	,Cliente.ePoints
	,Cliente.DiasDeCredito
	,Cliente.TipoFormaPagoID
	,TipoFormaPago.NombreTipoFormaPago
	,Cliente.BancoID
	,Banco.NombreBanco
	,Cliente.TipoClienteID
	,TipoCliente.NombreTipoCliente
	,Cliente.CuentaBancaria
	,ISNULL(Cliente.Tolerancia, 0) AS Tolerancia
	,Cliente.LimiteCredito
	,ISNULL(Cliente.EsClienteComisionista, 0) AS EsClienteComisionista
	,ISNULL(Cliente.EsTallerElectrico, 0) AS EsTallerElectrico
	,ISNULL(Cliente.EsTallerMecanico, 0) AS EsTallerMecanico
	,ISNULL(Cliente.EsTallerDiesel, 0) AS EsTallerDiesel
	,Cliente.ClienteComisionistaID
	,Com.Nombre AS NombreClienteComisionista
	,Cliente.FechaRegistro
	,Cliente.Alias
	,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 1) AS DeudaActual
	,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 2) AS DeudaVencido
	,dbo.fnuClientePromedioPago(Cliente.ClienteID, 1) AS PromedioPagoTotal
	,dbo.fnuClientePromedioPago(Cliente.ClienteID, 2) AS PromedioPagoTresMeses
FROM
	Cliente
	LEFT JOIN ClienteFacturacion ON ClienteFacturacion.ClienteID = Cliente.ClienteID
	INNER JOIN Estado ON Estado.EstadoID = Cliente.EstadoID
	INNER JOIN Municipio ON Municipio.MunicipioID = Cliente.MunicipioID
	INNER JOIN Ciudad ON Ciudad.CiudadID = Cliente.CiudadID
	LEFT JOIN TipoFormaPago ON TipoFormaPago.TipoFormaPagoID = Cliente.TipoFormaPagoID
	LEFT JOIN Banco ON Banco.BancoID = Cliente.BancoID
	LEFT JOIN TipoCliente ON TipoCliente.TipoClienteID = Cliente.TipoClienteID
	LEFT JOIN Cliente Com ON Com.ClienteComisionistaID = Cliente.ClienteID
WHERE
	Cliente.Estatus = 1
GO


