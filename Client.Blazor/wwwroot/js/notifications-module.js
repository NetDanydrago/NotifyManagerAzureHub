// Función para manejar eventos push
function handlePushEvent(event) {
    console.log('Push notification received:', event);

    let data = {};

    if (event.data) {
        try {
            data = event.data.json();
        } catch (e) {
            console.warn('Failed to parse notification data as JSON:', e);
            data = { message: event.data.text() };
        }
    }

    const options = {
        body: data.message || data.body || 'Nueva notificación',
        icon: data.icon || '/icon-192.png',
        badge: data.badge || '/badge-72.png',
        tag: data.tag || 'azure-notification',
        data: data,
        requireInteraction: data.requireInteraction || false,
        actions: data.actions || []
    };

    return self.registration.showNotification(
        data.title || 'Notificación',
        options
    );
}

// Función para manejar clics en notificaciones
function handleNotificationClick(event) {
    console.log('Notification clicked:', event.notification);

    event.notification.close();

    const urlToOpen = event.notification.data?.url || '/';

    return clients.matchAll({
        includeUncontrolled: true,
        type: 'window'
    }).then(clientList => {
        // Si ya hay una ventana de la app abierta, enfocarla
        for (const client of clientList) {
            if (client.url.includes(self.location.origin) && 'focus' in client) {
                return client.focus();
            }
        }
        // Si no hay ventana abierta, abrir una nueva
        if (clients.openWindow) {
            return clients.openWindow(urlToOpen);
        }
    });
}

// Función para manejar cierre de notificaciones
function handleNotificationClose(event) {
    console.log('Notification closed:', event.notification.tag);
    // Opcional: registrar analíticas o limpiar datos
}

// Función para registrar los event listeners de notificaciones
function registerNotificationHandlers() {
    self.addEventListener('push', event => {
        event.waitUntil(handlePushEvent(event));
    });

    self.addEventListener('notificationclick', event => {
        event.waitUntil(handleNotificationClick(event));
    });

    self.addEventListener('notificationclose', event => {
        handleNotificationClose(event);
    });

    console.log('Notification handlers registered');
}

if (typeof self !== 'undefined' && self.importScripts) {
    // Estamos en un service worker context
    self.NotificationModule = {
        registerHandlers: registerNotificationHandlers,
        handlePush: handlePushEvent,
        handleClick: handleNotificationClick,
        handleClose: handleNotificationClose
    };
}