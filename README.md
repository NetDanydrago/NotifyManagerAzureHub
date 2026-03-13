## Visión General

Este repositorio implementa una solución de notificaciones push centrada en **Azure Notification Hubs**.
La propuesta se divide en dos piezas:

- **API REST en .NET** que encapsula el acceso al hub y expone endpoints simples para registro de instalaciones y envío de notificaciones, diseñada con **Clean Architecture combinada con Vertical Slice**.
- **Cliente Blazor WebAssembly** con flujo mínimo para navegador: pedir permisos, suscribir al usuario y mostrar notificaciones cuando llegan desde el hub.

## 1) Azure Notification Hubs a través de la API REST

El proyecto usa Azure Notification Hubs como backend de mensajería push, pero evita acoplar al cliente con detalles de seguridad o firma SAS.
Toda la interacción con Azure se concentra en la API.

### Flujo principal

1. El cliente obtiene el permiso del navegador para notificaciones.
2. El cliente crea/renueva su suscripción Web Push (endpoint + llaves).
3. El cliente envía esa instalación a la API (`POST /api/notifications/subscriptions`).
4. La API transforma el payload y hace `PUT` a `/installations/{id}` de Azure Notification Hubs usando token SAS.
5. Cuando se envía una notificación (`POST /api/notifications/send`), la API publica en `/messages` del hub.
6. El Service Worker del cliente Blazor recibe el push y lo muestra como notificación del sistema.

### Por qué usar este enfoque

- **Seguridad**: el cliente no conoce ni maneja credenciales del hub.
- **Simplicidad**: el frontend solo consume endpoints REST del dominio.
- **Escalabilidad**: tags y plantillas permiten segmentar usuarios y evolucionar campañas.
- **Portabilidad**: el mismo núcleo `NotifyManager` puede exponer otros frontends o clientes (móviles, desktop).

### Estilo arquitectónico de la API

La API sigue una **combinación de Clean Architecture + Vertical Slice**:

- **Clean Architecture** para separar responsabilidades y aislar reglas de negocio de detalles de infraestructura (Azure Notification Hubs, generación de tokens SAS, HttpClient).
- **Vertical Slice** para organizar casos de uso de extremo a extremo (suscripción y envío) con flujos claros por feature, permitiendo evolucionar cada capacidad de forma independiente.
- Esta combinación permite:
  - Agregar nuevos casos de notificación sin romper contratos existentes.
  - Mantener cohesión por feature (DTO, Controller, Handler, extensiones relacionadas).
  - Testear y desplegar verticalmente cada slice.

## 2) Arquitectura de la API y features

**La arquitectura de la API está basada en Clean Architecture combinada con Vertical Slice.**

La API está compuesta por un host (`Api`) y una librería de dominio/aplicación (`NotifyManager`) con mapeo REST separado (`NotifyManager.Rest.Mappings`).

### Capas y responsabilidades

#### `Api` (Host)
- Arranque de ASP.NET Core.
- Configuración de CORS, OpenAPI y binding de opciones desde `appsettings.json`.
- Publicación de endpoints vía `UseNotifyManagerEndpoints()`.

#### `NotifyManager.Rest.Mappings` (Adaptador REST)
- Minimal APIs para mapear rutas HTTP a controladores de aplicación.
- Transforma `HttpRequest` en DTOs y delega ejecución.

#### `NotifyManager` (Core - Vertical Slices)
Organizado por casos de uso:

**Slice 1: Suscripción de instalación**
- `Dtos/SubscriptionDto.cs`: contrato de entrada.
- `Controllers/SubscribeController.cs`: orquestador de aplicación.
- `Internals/InputPorts/ISubscribeInputPort.cs`: puerto de entrada.
- `Handlers/SubscribeHandler.cs`: implementación del caso de uso (genera SAS token, serializa instalación, llama a Azure Notification Hubs).
- `Extensions/SubscriptionExtensions.cs`: lógica de transformación específica de suscripciones.

**Slice 2: Envío de notificación**
- `Dtos/NotificationDto.cs`: contrato de entrada.
- `Controllers/SendNotificationController.cs`: orquestador de aplicación.
- `Internals/InputPorts/ISendNotificationInputPort.cs`: puerto de entrada.
- `Handlers/SendNotificationHandler.cs`: implementación del caso de uso (construye payload, genera SAS token, publica en hub con tags).

**Infraestructura compartida**
- `Helpers/SasTokenHelper.cs`: generación de tokens SAS para autenticación con Azure.
- `Extensions/ConnectionStringExtensions.cs`: parsing de connection string del hub.
- `Options/AzureNotificationHubOptions.cs`: configuración centralizada.

### Endpoints expuestos

#### Registrar o actualizar instalación
- **Método**: `POST`
- **Ruta**: `/api/notifications/subscriptions`
- **Body esperado** (ejemplo):

```json
{
  "installationId": "user-12345",
  "platform": "browser",
  "tags": ["user:12345", "web:users", "premium"],
  "webPushSubscription": {
    "endpoint": "https://fcm.googleapis.com/fcm/send/...",
    "p256dh": "BNcRd...",
    "auth": "tBH5..."
  },
  "payLoad": {
    "url": "$(url)",
    "user": "$(userid)"
  }
}
```

**Respuesta**:
```json
{
  "isSuccess": true,
  "message": null
}
```

#### Enviar notificación
- **Método**: `POST`
- **Ruta**: `/api/notifications/send`
- **Body esperado** (ejemplo):

```json
{
  "title": "Nueva actualización disponible",
  "body": "Hemos lanzado nuevas funcionalidades",
  "data": {
    "url": "/notifications",
    "source": "system",
    "priority": "high"
  },
  "tags": ["web:users"]
}
```

**Respuesta**:
```json
{
  "isSuccess": true,
  "message": null
}
```

### Features implementadas en la API

- ✅ Registro/actualización de instalaciones para Web Push en Azure Notification Hubs.
- ✅ Envío de notificaciones con payload de título, cuerpo y datos adicionales.
- ✅ Segmentación por tags (`ServiceBusNotification-Tags`).
- ✅ Generación de SAS token en servidor para autenticación con el hub.
- ✅ Manejo de errores con logging estructurado y propagación de estado de operación.
- ✅ Configuración desacoplada por `AzureNotificationHubOptions`.
- ✅ Inyección de dependencias con estrategia Scoped para handlers.
- ✅ Separación de concerns: controladores, casos de uso, infraestructura.

### Configuración mínima requerida

En `Api/appsettings.json`:

```json
{
  "AzureNotificationHubOptions": {
    "ConnectionString": "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyname>;SharedAccessKey=<key>",
    "HubName": "<nombre-del-hub>"
  }
}
```

### Preparación y ejecución

1. **Configurar Azure Notification Hubs** con `ConnectionString` y `HubName`.
2. **Iniciar la API**:

```bash
dotnet run --project Api
```

3. **Verificar** que la API responda en `https://localhost:7009` o `http://localhost:5121`.
4. **Probar endpoints** con herramientas como `curl`, Postman o el archivo `Api/Api.http`.

### Ejemplo de uso con curl

```bash
# Registrar instalación
curl -X POST https://localhost:7009/api/notifications/subscriptions \
  -H "Content-Type: application/json" \
  -d '{
    "installationId": "user-12345",
    "platform": "browser",
    "tags": ["user:12345"],
    "webPushSubscription": {
      "endpoint": "https://fcm.googleapis.com/...",
      "p256dh": "...",
      "auth": "..."
    },
    "payLoad": {}
  }'

# Enviar notificación
curl -X POST https://localhost:7009/api/notifications/send \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Hola",
    "body": "Mensaje de prueba",
    "data": {"url": "/home"},
    "tags": ["user:12345"]
  }'
```

## 3) Cliente Blazor simple (didáctico)

El cliente `Client.Blazor` es una implementación mínima para demostrar, de forma clara, cómo consumir la API y validar el ciclo completo de Web Push.

### Qué demuestra

- ✅ Solicitud de permisos del navegador (`Notification.requestPermission`).
- ✅ Suscripción push con `PushManager` + clave pública VAPID.
- ✅ Registro de la instalación en la API REST.
- ✅ Recepción y visualización de notificaciones en Service Worker.
- ✅ Conversión de llaves ArrayBuffer a Base64 para envío a backend.
- ✅ Manejo de estados de UI (permiso pendiente/concedido/denegado, suscrito/no suscrito).

### Componentes relevantes

**Página de demo de notificaciones:**
- [Client.Blazor/Pages/Notifications.razor](Client.Blazor/Pages/Notifications.razor)
- [Client.Blazor/Pages/Notifications.razor.cs](Client.Blazor/Pages/Notifications.razor.cs)

**Módulos JavaScript:**
- [Client.Blazor/wwwroot/js/notification-permissions.js](Client.Blazor/wwwroot/js/notification-permissions.js)
- [Client.Blazor/wwwroot/js/notification-subscriptions.js](Client.Blazor/wwwroot/js/notification-subscriptions.js)
- [Client.Blazor/wwwroot/js/notifications-module.js](Client.Blazor/wwwroot/js/notifications-module.js)

**Service Worker:**
- [Client.Blazor/wwwroot/service-worker.js](Client.Blazor/wwwroot/service-worker.js)

### Configuración del cliente

En `Client.Blazor/wwwroot/appsettings.json`:

```json
{
  "ApiBaseUrl": "https://localhost:7009",
  "WebPushOptions": {
    "VapidPublicKey": "<tu-clave-publica-vapid-desde-azure>"
  }
}
```

En `Client.Blazor/Program.cs`, configurar la URL base del HttpClient:

```csharp
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("https://localhost:7009") 
});
```

### Flujo de usuario en el cliente

1. **Paso 1**: Usuario hace clic en "Request Permission".
   - Llama a `Notification.requestPermission()`.
   - Actualiza UI con estado `granted`/`denied`.

2. **Paso 2**: Usuario hace clic en "Subscribe".
   - Registra Service Worker.
   - Llama a `PushManager.subscribe()` con VAPID key.
   - Serializa endpoint + llaves p256dh/auth.
   - Envía `POST` a `/api/notifications/subscriptions`.

3. **Paso 3**: Service Worker escucha eventos `push`.
   - Al recibir notificación, parsea JSON.
   - Muestra notificación del sistema con `showNotification()`.
   - Al hacer clic, abre o enfoca la aplicación.

### Importante sobre este cliente

- ⚠️ Este cliente Blazor está diseñado como **contenido didáctico** para entender y validar la API.
- Su objetivo es **enseñar el flujo end-to-end**, no cubrir todos los escenarios de UX, seguridad y observabilidad de un frontend empresarial.
- Ideal para pruebas de concepto, demos y aprendizaje.

## Uso en entornos productivos

### Estado de la API

✅ El diseño y la implementación del backend están pensados para ser **utilizados en escenarios productivos**, con:

- Enfoque **Clean Architecture + Vertical Slice**.
- Responsabilidades claras de configuración, seguridad de credenciales del hub y separación por capas.
- Handlers agnósticos de infraestructura HTTP (testeables unitariamente).
- Logging estructurado con `ILogger<T>`.

### Recomendaciones para despliegue real de la API

- 🔐 **Gestionar secretos** con Azure Key Vault o equivalente (no hardcodear connection strings).
- 🚧 **Restringir CORS** por dominio en lugar de `AllowAnyOrigin`.
- 🔑 **Agregar autenticación/autorización** a los endpoints de suscripción y envío (JWT, API Keys, Azure AD).
- 📊 **Implementar telemetría y trazabilidad** (Application Insights, OpenTelemetry).
- 🔄 **Definir políticas de retry/circuit breaker** para llamadas al hub (Polly).
- 📝 **Versionar contratos** y documentar SLA de los endpoints con Swagger/OpenAPI.
- ⚡ **Validar payloads** de entrada con FluentValidation o Data Annotations.
- 🏷️ **Establecer convenciones de tags** (ej: `user:{id}`, `role:{role}`, `region:{code}`).

### Estado del cliente Blazor

⚠️ El cliente web incluido se mantiene intencionalmente **simple y didáctico**.

Es ideal como referencia técnica y base de pruebas, pero para producción se recomienda robustecer:

- Gestión de identidad y relación real `usuario <-> installationId` (actualmente hardcodeado).
- Manejo de reintentos, expiración de suscripción y sincronización de tags.
- Mensajería UX más completa para estados de permiso/errores (toasts, modales).
- Hardening de Service Worker y estrategias de compatibilidad por navegador.
- Manejo de escenarios offline y sincronización en background.
- Analíticas de interacción con notificaciones (clicks, dismissals).

## Resumen ejecutivo

- ✅ **Azure Notification Hubs** se consume exclusivamente desde la API REST.
- ✅ La API concentra **seguridad, reglas de integración y segmentación por tags** bajo un enfoque **Clean Architecture + Vertical Slice**.
- ✅ El **cliente Blazor** muestra un flujo mínimo, claro y didáctico para validar la integración end-to-end.
- ✅ El **backend está orientado a uso productivo**; el **frontend actual está orientado a aprendizaje y demostración**.
- ✅ La arquitectura permite **evolución independiente** de casos de uso de notificación (nuevos canales, plantillas, segmentaciones).

