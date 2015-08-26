/* Script con modificaciones a la base de datos de Theos. Archivo 005
 * Creado: 2015/04/27
 * Subido: 2015/05/01
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

-- Nómina

ALTER TABLE Usuario ADD Activo BIT NULL
GO
UPDATE Usuario SET Activo = 1
ALTER TABLE Usuario ALTER COLUMN Activo BIT NOT NULL

CREATE TABLE NominaOficialCuenta (
	NominaOficialCuentaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, ContaCuentaAuxiliarID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaAuxiliar(ContaCuentaAuxiliarID)
	, Suma BIT NOT NULL
)

CREATE TABLE UsuarioNominaOficial (
	UsuarioNominaOficialID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, ContaCuentaAuxiliarID INT NOT NULL FOREIGN KEY REFERENCES ContaCuentaAuxiliar(ContaCuentaAuxiliarID)
	, Importe DECIMAL(12, 2) NOT NULL
)

CREATE TABLE UsuarioNomina (
	UsuarioNominaID INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
	, IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(UsuarioID)
	, SueldoFijo DECIMAL(12, 2) NOT NULL
	
	, CONSTRAINT Ix_IdUsuario UNIQUE (IdUsuario)
)

-- Contabilidad
ALTER TABLE ContaPoliza ADD Error BIT NULL

/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO

ALTER VIEW [dbo].[UsuariosView] AS
	SELECT 
		Usuario.UsuarioID
		,Usuario.NombrePersona
		,Usuario.NombreUsuario
		,Usuario.Contrasenia
		,STUFF((SELECT ', ' + Perfil.NombrePerfil
			FROM UsuarioPerfil
			INNER JOIN Perfil ON Perfil.PerfilID = UsuarioPerfil.PerfilID
			WHERE UsuarioPerfil.UsuarioID = Usuario.UsuarioID		
			FOR XML PATH('')
			), 1, 2, '') AS Perfil
		,CASE WHEN Usuario.Activo = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estatus
		,CAST(Usuario.UsuarioID AS VARCHAR) 
		+ Usuario.NombreUsuario
		+ Usuario.NombrePersona 
		+ CASE WHEN Usuario.Activo = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Busqueda
	FROM Usuario
	WHERE Estatus = 1
GO

ALTER VIEW [dbo].[UsuariosPermisosView] AS
	SELECT 
		u.UsuarioID
		, up.PerfilID
		, pf.NombrePerfil AS Perfil
		, p.PermisoID
		, p.NombrePermiso AS Permiso
	FROM
		Usuario u
		INNER JOIN UsuarioPerfil up ON up.UsuarioID = u.UsuarioID AND up.Estatus = 1
		LEFT JOIN Perfil pf ON pf.PerfilID = up.PerfilID AND pf.Estatus = 1
		INNER JOIN PerfilPermiso pp ON pp.PerfilID = up.PerfilID AND pp.Estatus = 1
		INNEr JOIN Permiso p ON p.PermisoID = pp.PermisoID AND p.Estatus = 1
	WHERE
		u.Estatus = 1
		AND u.Activo = 1
GO

ALTER VIEW [dbo].[PartesExistenciasView] AS
	SELECT
		pe.ParteExistenciaID		, p.ParteID		, p.NumeroParte AS NumeroDeParte		, p.NombreParte AS Descripcion		, p.ProveedorID		, pv.NombreProveedor AS Proveedor		, p.MarcaParteID AS MarcaID		, mp.NombreMarcaParte AS Marca		, p.LineaID		, l.NombreLinea AS Linea		, pp.Costo		, pp.CostoConDescuento		, pe.SucursalID		, pe.Existencia		-- , SUM(pe.Existencia) AS Existencia		, MAX(vd.FechaRegistro) AS UltimaVenta	FROM		Parte p		LEFT JOIN Proveedor pv ON pv.ProveedorID = p.ProveedorID AND pv.Estatus = 1		LEFT JOIN MarcaParte mp ON mp.MarcaParteID = p.MarcaParteID AND mp.Estatus = 1		LEFT JOIN Linea l ON l.LineaID = p.LineaID AND l.Estatus = 1		LEFT JOIN ParteExistencia pe ON pe.ParteID = p.ParteID AND pe.Estatus = 1		LEFT JOIN PartePrecio pp ON pp.ParteID = p.ParteID AND p.Estatus = 1		LEFT JOIN VentaDetalle vd ON vd.ParteID = p.ParteID AND vd.Estatus = 1	WHERE		p.Estatus = 1	GROUP BY		pe.ParteExistenciaID		, p.ParteID		, p.NumeroParte		, p.NombreParte		, p.ProveedorID		, pv.NombreProveedor		, p.MarcaParteID		, mp.NombreMarcaParte		, p.LineaID		, l.NombreLinea		, pp.Costo		, pp.CostoConDescuento		, pe.SucursalID
		, pe.Existencia
GO

ALTER VIEW [dbo].[ProveedoresPolizasView] AS
	SELECT
		pp.ProveedorPolizaID
		, pp.ProveedorID
		, p.NombreProveedor AS Proveedor
		, pp.FechaPago
		, pp.ImportePago
		, pp.UsuarioID
		, u.NombreUsuario AS Usuario
		, bc.NombreDeCuenta
		, tfp.NombreTipoFormaPago AS FormaDePago
		, ppdc.FolioDePago
	FROM
		ProveedorPoliza pp
		LEFT JOIN Proveedor p ON p.ProveedorID = pp.ProveedorID AND p.Estatus = 1
		LEFT JOIN Usuario u ON u.UsuarioID = pp.UsuarioID AND u.Estatus = 1
		-- Se agregan algunos datos de los pagos directos correspondientes
		LEFT JOIN (
			SELECT
				ProveedorPolizaID
				, MIN(TipoFormaPagoID) AS TipoFormaPagoID
				, MIN(BancoCuentaID) AS BancoCuentaID
				, MIN(FolioDePago) AS FolioDePago
				, SUM(Importe) AS PagoDirecto
			FROM ProveedorPolizaDetalle
			WHERE
				Estatus = 1
				AND OrigenID = 1
			GROUP BY ProveedorPolizaID
		) ppdc ON ppdc.ProveedorPolizaID = pp.ProveedorPolizaID
		LEFT JOIN BancoCuenta bc ON bc.BancoCuentaID = ppdc.BancoCuentaID
		LEFT JOIN TipoFormaPago tfp ON tfp.TipoFormaPagoID = ppdc.TipoFormaPagoID AND tfp.Estatus = 1
	WHERE pp.Estatus = 1
GO

ALTER VIEW [dbo].[ContaPolizasDetalleAvanzadoView] AS
	SELECT
		cpd.ContaPolizaDetalleID
		, cpd.ContaPolizaID
		, cp.Fecha AS FechaPoliza
		, cp.Origen AS OrigenPoliza
		, cp.SucursalID AS SucursalID
		, s.NombreSucursal AS Sucursal
		, cp.Concepto AS ConceptoPoliza
		, cp.Error
		, cpd.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, cca.CuentaContpaq
		, cca.CuentaSat
		, cpd.Cargo
		, cpd.Abono
		, cpd.Referencia
	FROM
		ContaPolizaDetalle cpd
		LEFT JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
		LEFT JOIN Sucursal s ON s.SucursalID = cp.SucursalID AND s.Estatus = 1
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cpd.ContaCuentaAuxiliarID
GO

-- DROP VIEW UsuariosNominaView
CREATE VIEW UsuariosNominaView AS
	SELECT
		un.UsuarioNominaID
		, un.IdUsuario AS UsuarioID
		, u.NombreUsuario AS Usuario
		, un.SueldoFijo
	FROM
		UsuarioNomina un
		INNER JOIN Usuario u ON u.UsuarioID = un.IdUsuario AND u.Activo = 1 AND u.Estatus = 1
GO

CREATE VIEW NominaOficialCuentasView AS
	SELECT
		noc.NominaOficialCuentaID
		, noc.ContaCuentaAuxiliarID
		, cca.CuentaAuxiliar
		, noc.Suma
	FROM
		NominaOficialCuenta noc
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = noc.ContaCuentaAuxiliarID
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

GO

ALTER PROCEDURE [dbo].[pauContaCuentasPolizas] (
	@Desde DATE
	, @Hasta DATE
) AS BEGIN
	SET NOCOUNT ON

	/* DECLARE @Desde DATE = '2014-04-01'
	DECLARE @Hasta DATE = '2014-04-30'
	*/

	-- Definición de variables tipo constante
	DECLARE @SucMatrizID INT = 1
	DECLARE @Suc02ID INT = 2
	DECLARE @Suc03ID INT = 3
	DECLARE @CtaActivo INT = 1
	DECLARE @CtaPasivo INT = 2
	DECLARE @CtaCapitalContable INT = 3
	DECLARE @CtaResultadosAcreedoras INT = 4
	DECLARE @CtaResultadosDeudoras INT = 5

	-- Variables calculadas para el proceso
	SET @Hasta = DATEADD(d, 1, @Hasta)

	SELECT
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, ISNULL(CONVERT(BIT, CASE WHEN pec.ContaCuentaAuxiliarID IS NULL THEN 0 ELSE 1 END), 0) AS Error
		, SUM(pc.Importe) AS Total
		, SUM(CASE WHEN pc.SucursalID = @SucMatrizID THEN pc.Importe ELSE 0.0 END) AS Matriz
		, SUM(CASE WHEN pc.SucursalID = @Suc02ID THEN pc.Importe ELSE 0.0 END) AS Suc02
		, SUM(CASE WHEN pc.SucursalID = @Suc03ID THEN pc.Importe ELSE 0.0 END) AS Suc03
	FROM
		ContaCuenta cc
		LEFT JOIN ContaSubcuenta cs ON cs.ContaCuentaID = cc.ContaCuentaID
		LEFT JOIN ContaCuentaDeMayor ccm ON ccm.ContaSubcuentaID = cs.ContaSubcuentaID
		LEFT JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaDeMayorID = ccm.ContaCuentaDeMayorID
		LEFT JOIN (
			SELECT
				cpd.ContaCuentaAuxiliarID
				, cp.Fecha
				, cp.SucursalID
				, CASE cs.ContaCuentaID
					WHEN @CtaActivo THEN (cpd.Cargo - cpd.Abono)
					WHEN @CtaResultadosDeudoras THEN (cpd.Cargo - cpd.Abono)
					WHEN @CtaPasivo THEN (cpd.Abono - cpd.Cargo)
					WHEN @CtaCapitalContable THEN (cpd.Abono - cpd.Cargo)
					WHEN @CtaResultadosAcreedoras THEN (cpd.Abono - cpd.Cargo)
					ELSE 0.0
				END	AS Importe
			FROM
				ContaPolizaDetalle cpd
				INNER JOIN ContaPoliza cp ON cp.ContaPolizaID = cpd.ContaPolizaID
				INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = cpd.ContaCuentaAuxiliarID
				INNER JOIN ContaCuentaDeMayor ccm ON ccm.ContaCuentaDeMayorID = cca.ContaCuentaDeMayorID
				INNER JOIN ContaSubcuenta cs ON cs.ContaSubcuentaID = ccm.ContaSubcuentaID
			WHERE (cp.Fecha >= @Desde AND cp.Fecha < @Hasta)
		) pc ON pc.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
		-- Se agrega union con las tablas de pólizas, para marcar las que tengan error
		LEFT JOIN (
			SELECT DISTINCT
				cpd.ContaCuentaAuxiliarID
			FROM
				ContaPoliza cp
				INNER JOIN ContaPolizaDetalle cpd ON cpd.ContaPolizaID = cp.ContaPolizaID
			WHERE cp.Error = 1
		) pec ON pec.ContaCuentaAuxiliarID = cca.ContaCuentaAuxiliarID
	GROUP BY
		cc.ContaCuentaID
		, cs.ContaSubcuentaID
		, ccm.ContaCuentaDeMayorID
		, cca.ContaCuentaAuxiliarID
		, cc.Cuenta
		, cs.Subcuenta
		, ccm.CuentaDeMayor
		, cca.CuentaAuxiliar
		, cc.CuentaSat
		, cs.CuentaSat
		, ccm.CuentaSat
		, cca.CuentaSat
		, pec.ContaCuentaAuxiliarID
	ORDER BY
		(cc.CuentaSat + cc.Cuenta)
		, (cs.CuentaSat + cs.Subcuenta)
		, (ccm.CuentaSat + ccm.CuentaDeMayor)
		, (cca.CuentaAuxiliar)

END
GO