/*****************************************************************************
 * Script para llenar la nueva tabla de proveedores descuentos ganancias
 * en base a las tablas anteriores
 * Moi - 2015/09/14
 *****************************************************************************/

BEGIN TRAN

INSERT INTO ProveedorParteGanancia (ProveedorID, MarcaParteID, LineaID, DescuentoFactura1, DescuentoFactura2
	, DescuentoFactura3, DescuentoArticulo1, DescuentoArticulo2, DescuentoArticulo3, PorcentajeDeGanancia1
	, PorcentajeDeGanancia2, PorcentajeDeGanancia3, PorcentajeDeGanancia4, PorcentajeDeGanancia5)
SELECT
	pg.ProveedorID
	, pg.MarcaParteID
	, pg.LineaID
	, ISNULL(pmpf.DescuentoUno, 0.0)
	, ISNULL(pmpf.DescuentoDos, 0.0)
	, ISNULL(pmpf.DescuentoTres, 0.0)
	, ISNULL(pmpa.DescuentoUno, 0.0)
	, ISNULL(pmpa.DescuentoDos, 0.0)
	, ISNULL(pmpa.DescuentoTres, 0.0)
	, ISNULL(pg.PorcentajeUno, 0.0)
	, ISNULL(pg.PorcentajeDos, 0.0)
	, ISNULL(pg.PorcentajeTres, 0.0)
	, ISNULL(pg.PorcentajeCuatro, 0.0)
	, ISNULL(pg.PorcentajeCinco, 0.0)
FROM
	ProveedorGanancia pg
	LEFT JOIN ProveedorMarcaParte pmpf ON pmpf.ProveedorID = pg.ProveedorID AND pmpf.MarcaParteID = pg.MarcaParteID 
		AND pmpf.ImpactaFactura = 1 AND pmpf.Estatus = 1
	LEFT JOIN ProveedorMarcaParte pmpa ON pmpa.ProveedorID = pg.ProveedorID AND pmpa.MarcaParteID = pg.MarcaParteID 
		AND pmpa.ImpactaArticulo = 1 AND pmpa.Estatus = 1
WHERE pg.Estatus = 1

-- order by ProveedorID, MarcaParteID, LineaID
-- select * from proveedormarcaparte
-- select * from proveedorganancia
-- select * from proveedorparteganancia

ROLLBACK TRAN