/* Script con modificaciones para el módulo de ventas. Archivo 36
 * Creado: 2014/04/04
 * Subido: 2014/02/07
 */

/* *****************************************************************************
** Creación y modificación de tablas
***************************************************************************** */

BEGIN TRAN
BEGIN TRY

	-- Creación de Índices de Estatus y otros para rendiemiento
	CREATE INDEX Ix_Usuario_Estatus ON Usuario(Estatus)
	CREATE INDEX Ix_Venta_Estatus ON Venta(Estatus)
	CREATE INDEX Ix_VentaPago_Estatus ON VentaPago(Estatus)
	CREATE INDEX Ix_VentaPagoDetalle_Estatus ON VentaPagoDetalle(Estatus)
	CREATE INDEX Ix_VentaPagoDetalle_VentaPagoID ON VentaPagoDetalle(VentaPagoID)
	CREATE INDEX Ix_Cliente_Estatus ON Cliente(Estatus)
	CREATE INDEX Ix_VentaEstatus_Estatus ON VentaEstatus(Estatus)
	CREATE INDEX Ix_VentaFactura_Estatus ON VentaFactura(Estatus)
	CREATE INDEX Ix_VentaFacturaDetalle_Estatus ON VentaFacturaDetalle(Estatus)
	CREATE INDEX Ix_VentaFacturaDetalle_VentaFacturaID ON VentaFacturaDetalle(VentaFacturaID)
	CREATE INDEX Ix_VentaDetalle_Estatus ON VentaDetalle(Estatus)
	CREATE INDEX Ix_VentaDetalle_VentaID ON VentaDetalle(VentaID)

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
ALTER VIEW [dbo].[VentasACreditoView] AS
	SELECT
		vv.VentaID
		, vv.ClienteID
		, c.Nombre AS Cliente
		, vv.Folio
		, vv.Fecha
		, vv.VentaEstatusID
		, (vv.Fecha + c.DiasDeCredito) AS Vencimiento
		, vv.Total -- AS Importe
		, vv.Pagado -- AS Abono
		, ISNULL(vv.Total - vv.Pagado, 0) AS Restante -- AS Saldo
	FROM
		VentasView vv
		LEFT JOIN Cliente c ON c.ClienteID = vv.ClienteID AND c.Estatus = 1
	WHERE
		vv.ACredito = 1
		AND vv.VentaEstatusID IN (2, 3)
GO

/* *****************************************************************************
** Modificar procedimientos almacenados
***************************************************************************** */

