/* Script con modificaciones a la base de datos de Theos. Archivo 011
 * Creado: 2015/05/14
 * Subido: 2015/05/18
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- Contabilidad

ALTER TABLE ContaPoliza ADD RelacionID INT NULL
GO
UPDATE ContaPoliza SET RelacionID = pd.RelacionID FROM
	ContaPoliza p
	LEFT JOIN ContaPolizaDetalle pd ON pd.ContaPolizaID = p.ContaPolizaID

ALTER TABLE ContaPolizaDetalle DROP COLUMN RelacionID

ALTER TABLE NominaOficialCuenta ADD GeneraGasto BIT NULL

-- 

ALTER TABLE MarcaAttachFile ALTER COLUMN NombreArchivo NVARCHAR(256) NULL
ALTER TABLE LineaAttachFile ALTER COLUMN NombreArchivo NVARCHAR(256) NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vista
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
		, (
			nuoc.Total -
			(nu.Sueldo9500 + nu.Bono + nu.Adicional +
				CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
				ELSE nu.SueldoMinimo END)
			) AS Diferencia
		, nu.Tickets
		, nu.Adelanto
		, nu.MinutosTarde
		, nu.Otros
		, (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros) AS TotalDescuentos
		, (
			(nuoc.Total -
				(nu.Sueldo9500 + nu.Bono + nu.Adicional +
				CASE WHEN nu.SuperoMinimo = 1 THEN (nu.SueldoFijo + nu.SueldoVariable)
				ELSE nu.SueldoMinimo END))
			- (nu.Tickets + nu.Adelanto + nu.MinutosTarde + nu.Otros)
			) AS Liquido
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

ALTER VIEW [dbo].[NominaOficialCuentasView] AS
	SELECT
		noc.NominaOficialCuentaID
		, noc.ContaCuentaDeMayorID
		, ccm.CuentaDeMayor
		, noc.Suma
		, noc.GeneraGasto
	FROM
		NominaOficialCuenta noc
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = noc.ContaCuentaDeMayorID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

