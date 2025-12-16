// Importar módulo de notificaciones desde la nueva ubicación
importScripts('./js/notifications-module.js');

// Registrar handlers de notificaciones
if (self.NotificationModule) {
    self.NotificationModule.registerHandlers();
}



// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', () => { });


