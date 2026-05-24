# 🗄️ Sistema de Respaldos Automáticos N-Capas

**Aplicación de escritorio y web para la gestión automatizada de respaldos de bases de datos MySQL y MongoDB**, desarrollada con arquitectura de N-Capas en C# (.NET).

---

## 📋 Descripción General

Este sistema permite:

- **Programar respaldos automáticos** de múltiples bases de datos MySQL y MongoDB desde una interfaz web.
- **Ejecutar respaldos manuales** desde la aplicación de escritorio (Windows Forms).
- **Monitorear el historial** de respaldos realizados con estado de éxito o error.
- **Descargar archivos** de respaldo directamente desde la interfaz web.
- **Configurar horarios** para la ejecución automática de respaldos.
- **Motor dual**: Soporta respaldos con `mysqldump` (MySQL) y `mongodump` (MongoDB).

### 🏗️ Arquitectura N-Capas

| Capa | Proyecto | Responsabilidad |
|------|----------|-----------------|
| **Entidades (EN)** | `SistemaRespaldo.EN` | Clases POCO: `HistorialLog`, `BaseDatos`, `ConfiguracionRespaldo`, `Horario` |
| **Acceso a Datos (DAL)** | `SistemaRespaldo.DAL` | Conexión y consultas directas a MySQL (`ConsultasDAL`, `WebDAL`) |
| **Lógica de Negocio (BL)** | `SistemaRespaldo.BL` | Validaciones y reglas de negocio (`WebBL`) |
| **UI Escritorio** | `SistemaRespaldo.UI.Escritorio` | Windows Forms: motor de respaldos, timer, auto-arranque |
| **UI Web** | `SistemaRespaldo.UI.WEB` | ASP.NET Core MVC: historial, configuración, descarga de archivos |

---

## ⚙️ Configuración Inicial

### 1. Base de Datos MySQL

Ejecuta los siguientes scripts SQL **en orden** sobre tu servidor MySQL:

```sql
-- 1. Esquema principal
Script_MySQL_Respaldos.sql

-- 2. Soporte MongoDB (columnas TipoMotor y CadenaConexion)
Script_MongoDB_Update_Dia10.sql

-- 3. Columna TipoMotor en HistorialLogs (Día 12)
Script_Dia12_TipoMotor_HistorialLogs.sql
```

### 2. Archivo de Configuración (`config.json` / `appsettings.json`)

El sistema utiliza **dos archivos de configuración** con la misma estructura:

| Archivo | Ubicación | Usado por |
|---------|-----------|-----------|
| `config.json` | `SistemaRespaldo.UI.Escritorio/` | Aplicación de escritorio |
| `appsettings.json` | `SistemaRespaldo.UI.WEB/` | Interfaz web |

---

### 🐧 Configuración para Linux

Edita `config.json` y/o `appsettings.json` con estas rutas:

```json
{
  "ConfiguracionServidor": {
    "Servidor": "127.0.0.1",
    "Puerto": "3306",
    "Usuario": "tu_usuario_mysql",
    "Password": "tu_contraseña",
    "BaseDatosConfig": "SistemaRespaldos"
  },
  "Rutas": {
    "RutaGuardadoRespaldos": "/home/tu_usuario/Desktop/Respaldos/",
    "RutaMysqlDump": "/usr/bin/mysqldump"
  }
}
```

> **Nota:** En Linux, `mysqldump` suele estar en `/usr/bin/mysqldump`. Puedes verificarlo ejecutando:
> ```bash
> which mysqldump
> ```

### 🪟 Configuración para Windows

Edita `config.json` y/o `appsettings.json` con estas rutas:

```json
{
  "ConfiguracionServidor": {
    "Servidor": "127.0.0.1",
    "Puerto": "3306",
    "Usuario": "root",
    "Password": "tu_contraseña",
    "BaseDatosConfig": "SistemaRespaldos"
  },
  "Rutas": {
    "RutaGuardadoRespaldos": "C:\\RespaldosMySQL\\",
    "RutaMysqlDump": "C:\\Program Files\\MySQL\\MySQL Server 8.0\\bin\\mysqldump.exe"
  }
}
```

> **⚠️ Importante en Windows:** Usa doble barra invertida (`\\`) en las rutas JSON, ya que la barra simple (`\`) es un carácter de escape en JSON.

---

## 🛣️ Agregar `mysqldump` y `mongodump` al PATH en Windows

Si al ejecutar el motor de respaldos obtienes un error como *"El sistema no puede encontrar el archivo especificado"*, es porque el ejecutable no está en las variables de entorno. Sigue estos pasos:

### Paso 1: Localiza la carpeta `bin` de MySQL

La ruta típica es:
```
C:\Program Files\MySQL\MySQL Server 8.0\bin\
```

Si usas XAMPP:
```
C:\xampp\mysql\bin\
```

### Paso 2: Localiza la carpeta `bin` de MongoDB Tools

Descarga las [MongoDB Database Tools](https://www.mongodb.com/try/download/database-tools) si aún no las tienes. La ruta típica es:
```
C:\Program Files\MongoDB\Tools\100\bin\
```

### Paso 3: Abre las Variables de Entorno

1. Presiona `Win + R`, escribe `sysdm.cpl` y presiona Enter.
2. Ve a la pestaña **"Opciones avanzadas"** (Advanced).
3. Haz clic en **"Variables de entorno..."** (Environment Variables).

### Paso 4: Edita la variable `Path`

1. En la sección **"Variables del sistema"**, busca la variable llamada `Path` y haz doble clic.
2. Haz clic en **"Nuevo"** y agrega la ruta de la carpeta `bin` de MySQL:
   ```
   C:\Program Files\MySQL\MySQL Server 8.0\bin\
   ```
3. Haz clic en **"Nuevo"** de nuevo y agrega la ruta de MongoDB Tools:
   ```
   C:\Program Files\MongoDB\Tools\100\bin\
   ```
4. Acepta todos los diálogos con **"Aceptar"**.

### Paso 5: Verifica la instalación

Abre una **nueva** ventana de CMD o PowerShell y ejecuta:

```cmd
mysqldump --version
mongodump --version
```

Si ambos comandos muestran su versión, la configuración es correcta. ✅

> **⚠️ Nota:** Debes abrir una **nueva** ventana de terminal después de modificar las variables. Las ventanas abiertas previamente no detectarán los cambios.

---

## 🧪 Tutorial: Probar Errores Forzados

Para validar que el sistema captura correctamente los errores, puedes forzar escenarios de fallo:

### Prueba 1: URI de MongoDB Falsa

1. Desde la interfaz web, registra una nueva base de datos con estos datos:
   - **Nombre:** `pruebaErrorMongo`
   - **Motor:** `MongoDB`
   - **Cadena de Conexión:** `mongodb://servidorfalso:27017/baseinexistente`
2. Ejecuta un respaldo (manual o esperando al timer).
3. **Resultado esperado:** En el historial debe aparecer un registro con:
   - Estado: `Falló ❌`
   - Mensaje: Indicando el error de conexión de mongodump.

### Prueba 2: Base de Datos MySQL Inexistente

1. Registra una base de datos con nombre `basedatos_que_no_existe` y motor `MySQL`.
2. Ejecuta un respaldo.
3. **Resultado esperado:** Estado `Falló ❌` con un mensaje de error de mysqldump indicando que la base de datos no existe.

### Prueba 3: Ruta de `mysqldump` Inválida

1. En el archivo `config.json`, cambia temporalmente la ruta:
   ```json
   "RutaMysqlDump": "/ruta/falsa/mysqldump"
   ```
2. Reinicia la aplicación de escritorio e intenta un respaldo manual.
3. **Resultado esperado:** Error crítico indicando que no se pudo encontrar el ejecutable.
4. **⚠️ Recuerda** restaurar la ruta correcta después de la prueba.

### Prueba 4: Permisos Insuficientes en la Ruta de Guardado

1. Cambia la ruta de guardado a una carpeta sin permisos de escritura:
   ```json
   "RutaGuardadoRespaldos": "/root/SinPermisos/"
   ```
2. Ejecuta un respaldo.
3. **Resultado esperado:** Error de permisos capturado en el historial.
4. **⚠️ Recuerda** restaurar la ruta correcta después de la prueba.

---

## 🚀 Ejecución del Proyecto

### Interfaz Web (ASP.NET Core MVC)

```bash
cd SistemaRespaldo.UI.WEB
dotnet run
```

Accede a: `https://localhost:5001/Respaldos` o `http://localhost:5000/Respaldos`

### Aplicación de Escritorio (Windows Forms)

Abre el proyecto en Visual Studio y ejecuta `SistemaRespaldo.UI.Escritorio` como proyecto de inicio.

> La aplicación se minimiza a la bandeja del sistema y ejecuta respaldos automáticos según los horarios programados.

---

## 📁 Estructura del Proyecto

```
SistemRespaldo/
│
├── SistemaRespaldo.EN/              # Entidades (modelos de datos)
│   ├── HistorialLog.cs
│   ├── BaseDatos.cs
│   ├── ConfiguracionRespaldo.cs
│   └── Horario.cs
│
├── SistemaRespaldo.DAL/             # Acceso a datos (MySQL)
│   ├── ConfiguracionHelper.cs
│   ├── ConsultasDAL.cs
│   └── WebDAL.cs
│
├── SistemaRespaldo.BL/              # Lógica de negocio
│   └── WebBL.cs
│
├── SistemaRespaldo.UI.Escritorio/   # App de escritorio (motor de respaldos)
│   ├── Form1.cs
│   ├── RespaldoMotor.cs             # Motor MySQL (mysqldump)
│   ├── RespaldoMongoMotor.cs        # Motor MongoDB (mongodump)
│   ├── ConfiguracionMotor.cs
│   └── config.json
│
├── SistemaRespaldo.UI.WEB/          # Interfaz web (ASP.NET Core MVC)
│   ├── Controllers/
│   │   └── RespaldosController.cs
│   ├── Views/
│   │   └── Respaldos/
│   │       └── Index.cshtml
│   └── appsettings.json
│
├── Script_MySQL_Respaldos.sql                # Esquema base
├── Script_MongoDB_Update_Dia10.sql           # Soporte multi-motor
├── Script_Dia12_TipoMotor_HistorialLogs.sql  # TipoMotor en historial
└── README.md
```

---

## 👥 Autores

- **Alex** — Interfaz Web, DAL, Historial de Logs
- **Pineda** — Motor de Respaldos, Soporte MongoDB, Aplicación de Escritorio

---

## 📄 Licencia

Proyecto académico — Universidad. Todos los derechos reservados.
