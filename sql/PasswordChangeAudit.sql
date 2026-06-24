-- Tabla para auditar intentos de cambio de contraseña (éxitos y fallos)
-- No debe contener contraseñas en claro ni hashes antiguos.
-- Ejecutar en la base de datos donde está la aplicación (DbNegocio)

CREATE TABLE PasswordChangeAudits (
    IdAudit INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NULL,                     -- FK posible a UsuarioLogin.IdUsuario
    IdPersona INT NULL,                     -- FK posible a Persona.IdPersona
    Usuario VARCHAR(100) NOT NULL,          -- documento/username
    Exito BIT NOT NULL,                     -- 1 = éxito, 0 = fallo
    FechaAttempt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Ip VARCHAR(45) NULL,
    UserAgent VARCHAR(250) NULL,
    IntentosFallidos INT NULL,
    Bloqueado BIT NULL,
    FechaBloqueo DATETIME2 NULL,
    MotivoBloqueo VARCHAR(250) NULL,
    Observacion VARCHAR(500) NULL
);

-- Índices para consultas comunes
CREATE INDEX IX_PasswordChangeAudits_Usuario ON PasswordChangeAudits(Usuario);
CREATE INDEX IX_PasswordChangeAudits_IdUsuario ON PasswordChangeAudits(IdUsuario);
CREATE INDEX IX_PasswordChangeAudits_Fecha ON PasswordChangeAudits(FechaAttempt);

-- Ejemplos de inserción:
-- Fallo de contraseña (incremento de intentos)
INSERT INTO PasswordChangeAudits (IdUsuario, IdPersona, Usuario, Exito, Ip, UserAgent, IntentosFallidos, Observacion)
VALUES (NULL, NULL, '0801199912345', 0, '192.0.2.1', 'Mozilla/5.0', 1, 'Contraseña actual incorrecta');

-- Bloqueo tras 3 intentos
INSERT INTO PasswordChangeAudits (IdUsuario, IdPersona, Usuario, Exito, Ip, UserAgent, IntentosFallidos, Bloqueado, FechaBloqueo, MotivoBloqueo, Observacion)
VALUES (123, 456, '0801199912345', 0, '192.0.2.1', 'Mozilla/5.0', 3, 1, SYSUTCDATETIME(), 'Tres intentos fallidos', 'Cuenta bloqueada por intentos fallidos');

-- Éxito cambio de contraseña
INSERT INTO PasswordChangeAudits (IdUsuario, IdPersona, Usuario, Exito, Ip, UserAgent, Observacion)
VALUES (123, 456, '0801199912345', 1, '192.0.2.1', 'Mozilla/5.0', 'Cambio de contraseña exitoso');

-- Consultas útiles:
-- Obtener últimos 20 intentos por usuario
-- SELECT TOP 20 * FROM PasswordChangeAudits WHERE Usuario = '0801199912345' ORDER BY FechaAttempt DESC;

-- Obtener intentos fallidos recientes (últimas 24 horas)
-- SELECT * FROM PasswordChangeAudits WHERE Exito = 0 AND FechaAttempt >= DATEADD(day, -1, SYSUTCDATETIME()) ORDER BY FechaAttempt DESC;

-- Nota: No almacenar contraseñas ni hashes antiguos en esta tabla por consideraciones de seguridad.
