/* Script con modificaciones a la base de datos de Theos. Archivo 007
 * Creado: 2015/05/07
 * Subido: 2015/05/07
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- Nómina
ALTER TABLE NominaUsuario ADD SuperoMinimo BIT NULL
GO
UPDATE NominaUsuario SET SuperoMinimo = 1
ALTER TABLE NominaUsuario ALTER COLUMN SuperoMinimo BIT NOT NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[NominaUsuariosView] AS
	SELECT
		nu.NominaUsuarioID
		, n.NominaID
		, n.Semana
		, n.BancoCuentaID
		, nu.IdUsuario AS UsuarioID
		, u.NombreUsuario AS Usuario
		, nu.SucursalID
		, s.NombreSucursal AS Sucursal
		, nuoc.Total AS TotalOficial
		, nu.SuperoMinimo
		, nu.SueldoFijo
		, nu.SueldoVariable
		, nu.Sueldo9500
		, nu.SueldoMinimo
		, nu.Bono
		, nu.Adicional
		, (nu.Sueldo9500 + nu.Bono + nu.Adicional +
			CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
			ELSE nu.SueldoMinimo END) AS TotalSueldo
		, ((nu.Sueldo9500 + nu.Bono + nu.Adicional +
			CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
			ELSE nu.SueldoMinimo END)
			- nuoc.Total) AS Diferencia
		, nu.Tickets
		, nu.Adelanto
		, nu.MinutosTarde
		, nu.Otros
		, (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros) AS TotalDescuentos
		, (((nu.Sueldo9500 + nu.Bono + nu.Adicional +
			CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
			ELSE nu.SueldoMinimo END)
			- nuoc.Total)
			- (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros))
			AS Liquido
	FROM
		NominaUsuario nu
		LEFT JOIN Nomina n ON n.NominaID = nu.NominaID
		LEFT JOIN (
			SELECT
				NominaID
				, IdUsuario
				, SUM(CASE WHEN Suma = 1 THEN Importe ELSE Importe * -1 END) AS Total
			FROM NominaUsuarioOficial
			GROUP BY
				NominaID
				, IdUsuario
		) nuoc ON nuoc.IdUsuario = nu.IdUsuario AND nuoc.NominaID = nu.NominaID
		LEFT JOIN Usuario u ON u.UsuarioID = nu.IdUsuario AND u.Estatus = 1
		LEFT JOIN Sucursal s ON s.SucursalID = nu.SucursalID AND s.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

