-- Añade la dificultad elegida en la app móvil ('facil' o 'dificil') a cada sesión.
-- Es una columna opcional (NULL): las sesiones antiguas y las del escritorio no cambian.
-- Se puede ejecutar varias veces sin problema.
IF COL_LENGTH('dbo.sesion', 'dificultad') IS NULL
    ALTER TABLE dbo.sesion ADD dificultad NVARCHAR(10) NULL;
