/* Script con modificaciones a la base de datos de Theos. Archivo 075
 * Creado: 2016/07/20
 * Subido: 2016/07/
 */

DECLARE @ScriptID INT = 75
DECLARE @Por NVARCHAR(8) = 'Moi'
DECLARE @Observacion NVARCHAR(512) = ''
INSERT INTO ScriptSql (ScriptID, SubidoPor, Observacion) VALUES (@ScriptID, @Por, @Observacion)
-- SELECT * FROM ScriptSql ORDER BY ScriptID DESC

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

ALTER TABLE Cliente ADD MenorQue2000Efectivo BIT NULL

/* ****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

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
		-- ,Cliente.Credito
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
		,ISNULL(Cliente.Vip, 0) AS Vip
		,Cliente.ClienteComisionistaID
		,Com.Nombre AS NombreClienteComisionista
		,Cliente.FechaRegistro
		,Cliente.Alias
		-- ,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 1) AS DeudaActual
		-- ,dbo.fnuClienteCreditoDeuda(Cliente.ClienteID, 2) AS DeudaVencido
		-- ,dbo.fnuClientePromedioPago(Cliente.ClienteID, 1) AS PromedioPagoTotal
		-- ,dbo.fnuClientePromedioPago(Cliente.ClienteID, 2) AS PromedioPagoTresMeses
		,Cliente.Acumulado AS AcumuladoH
		-- ,dbo.fnuClienteAcumulado(Cliente.ClienteID) AS Acumulado
		, Cliente.CobranzaContacto
		, Cliente.CobranzaObservacion
		, Cliente.MenorQue2000Efectivo
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

ALTER VIEW [dbo].[ClientesDatosView] AS
	SELECT
		c.ClienteID
		, c.Nombre
		, c.Calle + ' ' + c.NumeroExterior
			+ CASE WHEN ISNULL(c.NumeroInterior, '') = '' THEN '' ELSE ' - ' END
			+ ISNULL(c.NumeroInterior, '') AS Direccion
		, c.Colonia
		, c.CodigoPostal
		, m.NombreMunicipio AS Municipio
		, cd.NombreCiudad AS Ciudad
		, e.NombreEstado AS Estado
		, c.Telefono
		, c.Celular
		, cf.Rfc
		, c.TipoFormaPagoID
		, c.BancoID
		, c.CuentaBancaria
		, c.ListaDePrecios
		, c.TieneCredito
		, c.LimiteCredito AS LimiteDeCredito
		, c.Tolerancia
		, c.CobroPorEnvio
		, c.ImporteParaCobroPorEnvio
		, c.ImporteCobroPorEnvio
		, c.MenorQue2000Efectivo
	FROM
		Cliente c
		LEFT JOIN Municipio m ON m.MunicipioID = c.MunicipioID AND m.Estatus = 1
		LEFT JOIN Ciudad cd ON cd.CiudadID = c.CiudadID AND cd.Estatus = 1
		LEFT JOIN Estado e ON e.EstadoID = c.EstadoID AND e.Estatus = 1
		LEFT JOIN ClienteFacturacion cf ON cf.ClienteID = c.ClienteID AND cf.Estatus = 1
	WHERE c.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

