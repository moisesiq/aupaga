/**/
CREATE FUNCTION [dbo].[fnuClienteAcumulado](
	@ClienteID AS INT
)
RETURNS DECIMAL(18,2)
AS
BEGIN	

DECLARE @Deuda AS DECIMAL(18,2)
	
	SET @Deuda = (SELECT 0.0 AS Deuda)	
	
	RETURN @Deuda
	
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
	,Cliente.Acumulado AS AcumuladoH
	,dbo.fnuClienteAcumulado(Cliente.ClienteID) AS Acumulado
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


