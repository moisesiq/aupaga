/* Script con modificaciones para el módulo de ventas. Archivo 33
 * Creado: 2014/02/27
 * Subido: 2014/03/28
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	DROP TABLE AutorizacionProcesoPerfil

	-- CREATE INDEX Ix_NombrePermiso ON Permiso(NombrePermiso)
	ALTER TABLE Permiso ADD CONSTRAINT DF_Permiso_FechaRegistro DEFAULT GETDATE() FOR FechaRegistro
	INSERT INTO Permiso (NombrePermiso, MensajeDeError) VALUES
		('Autorizaciones.Ventas.Cobro.CreditoNoAplicable', 'No tienes permisos para autorizar un crédito cuando éste está vencido.')
		, ('Autorizaciones.Ventas.Cobro.NotaDeCreditoOtroCliente', 'No tienes permisos para autorizar el uso de notas de crédito en otros clientes.')
		, ('Autorizaciones.Ventas.Devoluciones.Agregar', 'No tienes permisos para autorizar Devoluciones / Cancelaciones.')
		, ('Autorizaciones.Ventas.FondoDeCaja.Diferencia', 'No tienes permisos para autorizar diferencias en el Fondo de Caja.')
		, ('Autorizaciones.Ventas.9500.PrecioFueraDeRango', 'No tienes permisos para autorizar precios fuera de rango en 9500.')
		, ('Autorizaciones.Ventas.9500.NoAnticipo', 'No tienes permisos para autorizar un 9500 sin anticipo.')
		, ('Autorizaciones.Ventas.NotasDeCredito.Agregar', 'No tienes permisos para autorizar el agregar Notas de Crédito.')
		, ('Autorizaciones.Ventas.Ingresos.Agregar', 'No tienes permisos para autorizar el agregar Ingresos.')
		, ('Autorizaciones.Ventas.Ingresos.Borrar', 'No tienes permisos para autorizar el borrar Ingresos.')
		, ('Autorizaciones.Ventas.Egresos.Agregar', 'No tienes permisos para autorizar el agregar Gastos.')
		, ('Autorizaciones.Ventas.Egresos.Borrar', 'No tienes permisos para autorizar el borrar Gastos.')
		, ('Autorizaciones.Ventas.Refuerzo.Agregar', 'No tienes permisos para autorizar un Refuerzo.')
		, ('Autorizaciones.Ventas.Resguardo.Agregar', 'No tienes permisos para autorizar un Resguardo.')
		, ('Autorizaciones.Ventas.Cambios.Agregar', 'No tienes permisos para autorizar cambios en una Venta.')
		, ('Autorizaciones.Ventas.CorteDeCaja.Diferencia', 'No tienes permisos para autorizar diferencias en el Corte de Caja.')
		, ('Sistema.VerOtrasSucursales', 'No tienes permisos para ver otras Sucursales.')

	COMMIT TRAN
END TRY
BEGIN CATCH
	PRINT 'Hubo un error al ejecutar el script:'
	PRINT ERROR_MESSAGE()
	ROLLBACK TRAN
	RETURN
END CATCH
GO

/* *****************************************************************************
** Modificar tablas
***************************************************************************** */



/* *****************************************************************************
** Crear Funciones
***************************************************************************** */



/* *****************************************************************************
** Crear / modificar Vistas
***************************************************************************** */

GO
-- DROP VIEW UsuariosPermisosView
CREATE VIEW UsuariosPermisosView AS
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
	WHERE u.Estatus = 1
GO

/* *****************************************************************************
** Crear / Modificar procedimientos almacenados
***************************************************************************** */

