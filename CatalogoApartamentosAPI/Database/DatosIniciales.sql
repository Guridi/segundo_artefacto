-- Datos de ejemplo. Ejecutar después de CrearBaseDatos.sql.
-- Solo inserta si los registros no existen, por lo que puede ejecutarse más de una vez.
USE CatalogoApartamentosDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Residencial WHERE Nombre = N'Residencial Los Almendros')
BEGIN
    INSERT INTO dbo.Residencial (Nombre, Ubicacion, Descripcion)
    VALUES (N'Residencial Los Almendros',
            N'Av. Independencia, Santo Domingo',
            N'Residencial cerrado con piscina, áreas verdes, seguridad 24/7 y parqueo techado.');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Residencial WHERE Nombre = N'Torres del Cibao')
BEGIN
    INSERT INTO dbo.Residencial (Nombre, Ubicacion, Descripcion)
    VALUES (N'Torres del Cibao',
            N'Av. Juan Pablo Duarte, Santiago de los Caballeros',
            N'Torre moderna con gimnasio, ascensor, planta eléctrica full y terraza social.');
END;
GO

DECLARE @Almendros INT = (SELECT Id FROM dbo.Residencial WHERE Nombre = N'Residencial Los Almendros');
DECLARE @Cibao INT = (SELECT Id FROM dbo.Residencial WHERE Nombre = N'Torres del Cibao');

INSERT INTO dbo.Apartamento (Numero, Habitaciones, Banos, Precio, Disponible, ResidencialId)
SELECT datos.Numero, datos.Habitaciones, datos.Banos, datos.Precio, datos.Disponible, datos.ResidencialId
FROM (VALUES
    (N'A-101', 2, 1, 4500000.00, 1, @Almendros),
    (N'A-202', 3, 2, 6200000.00, 1, @Almendros),
    (N'B-301', 3, 2, 5800000.00, 0, @Almendros),
    (N'1A',    1, 1, 3200000.00, 1, @Cibao),
    (N'5C',    2, 2, 4900000.00, 1, @Cibao),
    (N'PH-10', 4, 3, 9500000.00, 1, @Cibao)
) AS datos (Numero, Habitaciones, Banos, Precio, Disponible, ResidencialId)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Apartamento a
    WHERE a.Numero = datos.Numero AND a.ResidencialId = datos.ResidencialId
);
GO
