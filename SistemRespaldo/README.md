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

El sistema utiliza **dos archivos de configuración** con la misma estructura clave para comunicarse con la base de datos de administración y ejecutar los comandos de respaldo. Ambos archivos deben mantenerse sincronizados con los mismos datos para garantizar que tanto la interfaz web como la aplicación de escritorio operen correctamente.

| Archivo | Ruta Relativa | Propósito |
|---------|---------------|-----------|
| `config.json` | `SistemaRespaldo.UI.Escritorio/config.json` | Configura el motor de ejecución de respaldos (Timer, MySQL y MongoDB). |
| `appsettings.json` | `SistemaRespaldo.UI.WEB/appsettings.json` | Configura el panel web de administración (historial, registro de bases de datos, horarios). |

---

### 📝 Guía Detallada de Parámetros JSON

A continuación se detalla qué significa cada propiedad dentro de los archivos de configuración y qué debes colocar en tu entorno local:

#### Bloque `"ConfiguracionServidor"`
Este bloque define la conexión a la base de datos MySQL central (`SistemaRespaldos`) donde el sistema guarda las credenciales de las bases de datos a respaldar, los horarios y los logs de historial.

*   **`Servidor`**: Dirección IP o dominio del servidor de base de datos MySQL (por ejemplo: `"127.0.0.1"` o `"localhost"`).
*   **`Puerto`**: Puerto en el que escucha tu servidor MySQL (por defecto: `"3306"`).
*   **`Usuario`**: Tu usuario de MySQL (por ejemplo: `"root"`). Debe tener permisos de lectura y escritura.
*   **`Password`**: La contraseña correspondiente al usuario de MySQL especificado.
*   **`BaseDatosConfig`**: El nombre de la base de datos de control del sistema (debe ser `"SistemaRespaldos"`, que es la base de datos creada por los scripts del paso 1).

#### Bloque `"Rutas"`
Este bloque indica al sistema dónde guardar físicamente los respaldos creados y dónde localizar las herramientas externas indispensables para realizarlos.

*   **`RutaGuardadoRespaldos`**: Carpeta local de tu computadora donde se descargarán y almacenarán los archivos generados (`.sql` de MySQL o archivos comprimidos/carpetas de MongoDB).
    *   *Ejemplo en Windows*: `"C:\\RespaldosMySQL\\"` (Asegúrate de que la carpeta exista o créala manualmente).
    *   *Ejemplo en Linux*: `"/home/usuario/Desktop/Respaldos/"`.
*   **`RutaMysqlDump`**: Ruta absoluta directa al ejecutable `mysqldump` (o `mysqldump.exe`). Este binario es propio de la instalación de MySQL y se encarga de exportar la estructura y datos.
*   **`RutaMongoDump`**: Ruta absoluta directa al ejecutable `mongodump` (o `mongodump.exe`). Este binario se descarga con las MongoDB Database Tools y se encarga de respaldar bases de datos NoSQL MongoDB.

---

### 🪟 Plantilla de Configuración para Windows

Edita tus archivos `config.json` y `appsettings.json` estructurándolos de la siguiente manera. **Reemplaza los valores de ejemplo por tus datos reales:**

```json
{
  "ConfiguracionServidor": {
    "Servidor": "127.0.0.1",
    "Puerto": "3306",
    "Usuario": "TU_USUARIO_MYSQL",
    "Password": "TU_CONTRASENA_MYSQL",
    "BaseDatosConfig": "SistemaRespaldos"
  },
  "Rutas": {
    "RutaGuardadoRespaldos": "C:\\Ruta\\De\\Tu\\Carpeta\\Respaldos\\",
    "RutaMysqlDump": "C:\\Program Files\\MySQL\\MySQL Server 8.0\\bin\\mysqldump.exe",
    "RutaMongoDump": "C:\\Program Files\\MongoDB\\Tools\\100\\bin\\mongodump.exe"
  }
}
```

> [!WARNING]
> **Doble barra invertida (`\\`) obligatoria en Windows:** En la sintaxis JSON, la barra invertida simple (`\`) es un carácter de escape. Si pones `"C:\Respaldos\"`, el programa fallará al leer la configuración. Debes usar obligatoriamente `"C:\\Respaldos\\"` para cada directorio en Windows.

---

### 🐧 Plantilla de Configuración para Linux

En Linux, la estructura es idéntica pero utiliza barras inclinadas simples (`/`) y rutas nativas:

```json
{
  "ConfiguracionServidor": {
    "Servidor": "127.0.0.1",
    "Puerto": "3306",
    "Usuario": "TU_USUARIO_MYSQL",
    "Password": "TU_CONTRASENA_MYSQL",
    "BaseDatosConfig": "SistemaRespaldos"
  },
  "Rutas": {
    "RutaGuardadoRespaldos": "/home/tu_usuario/Respaldos/",
    "RutaMysqlDump": "/usr/bin/mysqldump",
    "RutaMongoDump": "/usr/bin/mongodump"
  }
}
```

---

### 🔍 ¿Cómo encontrar las rutas correctas de los ejecutables?

Si no estás seguro de dónde están instalados los motores en tu máquina, sigue estos pasos para ubicarlos:

#### 1. Para `mysqldump` (MySQL)
*   **En Windows (Instalación Oficial)**: Generalmente está en `C:\Program Files\MySQL\MySQL Server X.Y\bin\mysqldump.exe` (donde X.Y es la versión, ej. 8.0).
*   **En Windows (XAMPP)**: Se encuentra en `C:\xampp\mysql\bin\mysqldump.exe`.
*   **En Windows (Laragon)**: Se encuentra en `C:\laragon\bin\mysql\mysql-X.Y.Z-winx64\bin\mysqldump.exe`.
*   **En Linux**: Abre una terminal y escribe `which mysqldump`. Copia la ruta que te devuelva (usualmente `/usr/bin/mysqldump`).

#### 2. Para `mongodump` (MongoDB)
*   **En Windows (Herramientas Oficiales)**: Típicamente está en `C:\Program Files\MongoDB\Tools\100\bin\mongodump.exe` o dentro del directorio de MongoDB Database Tools descargado.
*   **En Linux**: Abre una terminal y escribe `which mongodump`. Copia la ruta (usualmente `/usr/bin/mongodump`).

---

### 🚨 Reglas de Oro para Evitar Errores

Para garantizar el correcto funcionamiento del sistema, sigue estrictamente estas directrices:

1.  **Sincronización:** Si modificas el puerto, contraseña o usuario de MySQL en el `config.json` de la app de escritorio, **debes hacer el mismo cambio** en el `appsettings.json` del panel web. Si no lo haces, la web no mostrará los datos o el motor de escritorio no podrá leer la programación.
2.  **Existencia de la carpeta de respaldos:** El sistema **no crea** automáticamente la carpeta indicada en `RutaGuardadoRespaldos`. Debes asegurarte de crearla físicamente en tu disco duro antes de ejecutar el primer respaldo.
3.  **Permisos de Escritura:** La carpeta especificada en `RutaGuardadoRespaldos` debe poseer permisos de escritura completos para el usuario que esté ejecutando las aplicaciones.
4.  **No confundas las bases de datos:**
    *   `BaseDatosConfig` (en el JSON) es únicamente para la base de datos de control interno (`SistemaRespaldos`).
    *   Las bases de datos que deseas respaldar (tus proyectos personales, bases de prueba, etc.) se agregan directamente **desde el Panel Web**, no se escriben en los JSON.

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
