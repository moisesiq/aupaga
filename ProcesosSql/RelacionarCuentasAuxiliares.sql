/*
 * Proceso para relacionar las Cuentas Auxiliares con las tablas de Proveedores, Clientes, Cuentas bancarias, etc.,
 * según corresponda
 * 
*/

DECLARE @CmProveedores INT = 26
DECLARE @CmBancos INT = 3
DECLARE @CmClientes INT = 57
DECLARE @CmUsSalarios INT = 4
DECLARE @CmUsTiempoExtra INT = 104
DECLARE @CmUsVacaciones INT = 105
DECLARE @CmUsPrimaVacacional INT = 106
DECLARE @CmUsPrimaDominical INT = 109
DECLARE @CmUsAguinaldo INT = 37
DECLARE @CmUsIndemnizacion INT = 43
DECLARE @CmUsPtu INT = 108
DECLARE @CmUsImss INT = 46
DECLARE @CmUsInfonavit INT = 44
DECLARE @CmUs2PorNomina INT = 5

-- Consultas
/* 
SELECT bc.NombreDeCuenta, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN BancoCuenta bc ON bc.NombreDeCuenta = cca.CuentaAuxiliar
WHERE
	cca.ContaCuentaDeMayorID = @CmBancos
	AND cca.RelacionID IS NULL

-- Clientes
SELECT c.Nombre, cca.CuentaAuxiliar, *  
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Cliente c ON c.Nombre = cca.CuentaAuxiliar AND c.Estatus = 1
WHERE 
	cca.ContaCuentaDeMayorID = @CmClientes
	AND cca.RelacionID IS NULL

-- Proveedores
SELECT pv.NombreProveedor, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Proveedor pv ON pv.NombreProveedor = cca.CuentaAuxiliar AND pv.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmProveedores
	AND cca.RelacionID IS NULL
-- Usuarios salarios
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsSalarios
	AND cca.RelacionID IS NULL
-- Usuarios tiempo extra
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsTiempoExtra
	AND cca.RelacionID IS NULL
-- Usuarios vacaciones
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsVacaciones
	AND cca.RelacionID IS NULL
-- Usuarios prima vacacional
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsPrimaVacacional
	AND cca.RelacionID IS NULL
-- Usuarios prima dominical
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsPrimaDominical
	AND cca.RelacionID IS NULL
-- Usuarios aguinaldo
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsAguinaldo
	AND cca.RelacionID IS NULL
-- Usuarios indemnización
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsIndemnizacion
	AND cca.RelacionID IS NULL
-- Usuarios ptu
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsPtu
	AND cca.RelacionID IS NULL
-- Usuarios imss
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsImss
	AND cca.RelacionID IS NULL
-- Usuarios infonavit
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUsInfonavit
	AND cca.RelacionID IS NULL
-- Usuarios 2 % sobre nómina
SELECT u.NombreUsuario, cca.CuentaAuxiliar, * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE
	cca.ContaCuentaDeMayorID = @CmUs2PorNomina
	AND cca.RelacionID IS NULL
*/
-- Consultas



-- Bancos
UPDATE ContaCuentaAuxiliar SET RelacionID = bc.BancoCuentaID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN BancoCuenta bc ON bc.NombreDeCuenta = cca.CuentaAuxiliar
WHERE cca.ContaCuentaDeMayorID = @CmBancos

-- Clientes
UPDATE ContaCuentaAuxiliar SET RelacionID = c.ClienteID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Cliente c ON c.Nombre = cca.CuentaAuxiliar AND c.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmClientes

-- Proveedores
UPDATE ContaCuentaAuxiliar SET RelacionID = pv.ProveedorID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Proveedor pv ON pv.NombreProveedor = cca.CuentaAuxiliar AND pv.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmProveedores

-- Usuarios salarios
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsSalarios
-- Usuarios tiempo extra
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsTiempoExtra
-- Usuarios vacaciones
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsVacaciones
-- Usuarios prima vacacional
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsPrimaVacacional
-- Usuarios prima dominical
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsPrimaDominical
-- Usuarios aguinaldo
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsAguinaldo
-- Usuarios indemnización
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsIndemnizacion
-- Usuarios ptu
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsPtu
-- Usuarios imss
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsImss
-- Usuarios infonavit
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUsInfonavit
-- Usuarios 2 % sobre nómina
UPDATE ContaCuentaAuxiliar SET RelacionID = u.UsuarioID
-- SELECT * 
FROM
	ContaCuentaAuxiliar cca
	LEFT JOIN Usuario u ON u.NombrePersona = cca.CuentaAuxiliar AND u.Estatus = 1
WHERE cca.ContaCuentaDeMayorID = @CmUs2PorNomina