/* *****************************************************************************
** Proceso para cuadrar las existencias del kárdex con las existencias normales
** Creado: 23/12/2014 Moisés
***************************************************************************** */

DECLARE @UsuarioID INT = 1
DECLARE @OperacionID INT = 9

INSERT INTO ParteKardex (ParteID, OperacionID, SucursalID, Folio, Fecha, RealizoUsuarioID
	, Entidad, Origen, Destino, Cantidad, Importe, ExistenciaNueva)
	SELECT
		pd.ParteID
		, @OperacionID AS OperacionID
		, pd.SucursalID
		, NULL AS Folio
		, GETDATE() AS Fecha
		, @UsuarioID AS RealizoUsuarioID
		, '----' AS Entidad
		, '----' AS Origen
		, s.NombreSucursal AS Destino
		, (pd.Diferencia * -1) AS Cantidad
		, pp.Costo AS Importe
		, (pd.ExistenciaKardex - Diferencia) AS ExistenciaNueva
	FROM
		PartesDiferenciasEnExistenciaView pd
		LEFT JOIN Sucursal s ON s.SucursalID = pd.SucursalID AND s.Estatus = 1
		LEFT JOIN PartePrecio pp ON pp.ParteID = pd.ParteID AND pp.Estatus = 1
	ORDER BY
		pd.ParteID
		, pd.SucursalID