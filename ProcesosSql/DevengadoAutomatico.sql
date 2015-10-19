/*****************************************************************************
 * Script para insertar los registros de devengado a las cuentas que tienen 
 * devengado automático, en base a las tablas anteriores
 * Moi - 2015/10/18
 *****************************************************************************/

-- Para borrar los datos
/*
DELETE FROM ContaEgresoDevengadoEspecial WHERE ContaEgresoID IN 
	(SELECT ContaEgresoID FROM ContaEgreso WHERE ContaCuentaAuxiliarID IN (11,1397,229,209,1393,1394,1396,91,1392,1395,67,32,166,155,344,123,66,34,156,1776,1474,1771,1944,1470,1471,1473,1468,1469,1472,693,1777,1485,1772,1945,1481,1482,1484,1479,1480,1483,320,319,318,835,162,724,7,103,10,389,269,212,447,446,448,8,444,445,105,727,725,726,94,366,140,183,228,208,477,1352,476,478,77,474,475,374,378,373,404,540,1428,541,363,538,539,13,43,1976,1988,19,274,321,125,1338,113,265,112,289,130,26,122,33,27,291,2181,2032,2246,2031,37,35,38,278,368,367,167))
DELETE FROM ContaEgresoDevengado WHERE ContaEgresoID IN 
	(SELECT ContaEgresoID FROM ContaEgreso WHERE ContaCuentaAuxiliarID IN (11,1397,229,209,1393,1394,1396,91,1392,1395,67,32,166,155,344,123,66,34,156,1776,1474,1771,1944,1470,1471,1473,1468,1469,1472,693,1777,1485,1772,1945,1481,1482,1484,1479,1480,1483,320,319,318,835,162,724,7,103,10,389,269,212,447,446,448,8,444,445,105,727,725,726,94,366,140,183,228,208,477,1352,476,478,77,474,475,374,378,373,404,540,1428,541,363,538,539,13,43,1976,1988,19,274,321,125,1338,113,265,112,289,130,26,122,33,27,291,2181,2032,2246,2031,37,35,38,278,368,367,167))
*/

-- Para las de devengado especial (facilito :D)
-- INSERT INTO ContaEgresoDevengadoEspecial (ContaEgresoID, Fecha, DuenioID, Importe)
SELECT
	ce.ContaEgresoID
	, ce.Fecha
	, de.DuenioID
	, ce.Importe
FROM
	ContaEgreso ce
	INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
	LEFT JOIN ContaCuentaAuxiliarDevengadoEspecial de ON de.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
WHERE
	cca.DevengarAutEsp = 1
	AND cca.ContaCuentaAuxiliarID IN (11,1397,229,209,1393,1394,1396,91,1392,1395,67,32,166,155,344,123,66,34,156,1776,1474,1771,1944,1470,1471,1473,1468,1469,1472,693,1777,1485,1772,1945,1481,1482,1484,1479,1480,1483,320,319,318,835,162,724,7,103,10,389,269,212,447,446,448,8,444,445,105,727,725,726,94,366,140,183,228,208,477,1352,476,478,77,474,475,374,378,373,404,540,1428,541,363,538,539,13,43,1976,1988,19,274,321,125,1338,113,265,112,289,130,26,122,33,27,291,2181,2032,2246,2031,37,35,38,278,368,367,167)

-- Para las de devengado normal (no facilito :( ahh! siempre no fue tan difícil ':D)
-- INSERT INTO ContaEgresoDevengado (ContaEgresoID, Fecha, Importe, SucursalID, RealizoUsuarioID)
SELECT
	ce.ContaEgresoID
	, ce.Fecha
	, (ce.Importe * (da.Porcentaje / 100)) AS Importe
	, da.SucursalID
	, ce.RealizoUsuarioID
FROM
	ContaEgreso ce
	INNER JOIN ContaCuentaAuxiliar cca ON cca.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
	LEFT JOIN ContaCuentaAuxiliarDevengadoAutomatico da ON da.ContaCuentaAuxiliarID = ce.ContaCuentaAuxiliarID
		AND da.Porcentaje > 0
WHERE
	cca.DevengarAut = 1
	AND cca.ContaCuentaAuxiliarID IN (11,1397,229,209,1393,1394,1396,91,1392,1395,67,32,166,155,344,123,66,34,156,1776,1474,1771,1944,1470,1471,1473,1468,1469,1472,693,1777,1485,1772,1945,1481,1482,1484,1479,1480,1483,320,319,318,835,162,724,7,103,10,389,269,212,447,446,448,8,444,445,105,727,725,726,94,366,140,183,228,208,477,1352,476,478,77,474,475,374,378,373,404,540,1428,541,363,538,539,13,43,1976,1988,19,274,321,125,1338,113,265,112,289,130,26,122,33,27,291,2181,2032,2246,2031,37,35,38,278,368,367,167)