export class NotificationPermissionManager {
    // Verificar permisos actuales
    async checkPermission() {
        return Notification.permission; // 'default', 'granted', 'denied'
    }

    // Solicitar permisos de notificación
    async requestPermission() {
        await navigator.serviceWorker.ready;       
        const permission = await Notification.requestPermission();
        return permission === 'granted';
    }
}
