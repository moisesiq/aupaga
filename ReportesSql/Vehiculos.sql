/* *****************************************************************************
** Script que muestra un reporte de los vehículos registrados con sus motores y 
** años correspondientes
** Creado: 14/03/2014 Moisés
***************************************************************************** */

SELECT
	mr.MarcaID
	, mr.NombreMarca AS Marca
	, md.ModeloID
	, md.NombreModelo AS Modelo
	, mt.MotorID
	, mt.NombreMotor AS Motor
	, ma.MotorAnioID
	, ma.Anio
FROM
	Marca mr
	LEFT JOIN Modelo md ON md.MarcaID = mr.MarcaID AND md.Estatus = 1
	LEFT JOIN Motor mt ON mt.ModeloID = md.ModeloID AND mt.Estatus = 1
	LEFT JOIN MotorAnio ma ON ma.MotorID = mt.MotorID AND ma.Estatus = 1
ORDER BY
	Marca
	, Modelo
	, Motor
	, Anio