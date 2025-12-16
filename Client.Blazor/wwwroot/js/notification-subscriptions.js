// Utilidad para conversión VAPID key
function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

// Utilidad para conversión ArrayBuffer a Base64
function arrayBufferToBase64(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

export class NotificationSubscriptionManager {
    constructor() {
        this.subscription = null;
    }

    // Obtener suscripción para Azure Notification Hub
    async getSubscription(vapidPublicKey) {
        const registration = await navigator.serviceWorker.ready;
        
        const permission = Notification.permission;
        if (permission !== 'granted') {
            throw new Error('Notification permission not granted');
        }

        // Obtener suscripción existente o crear nueva
        this.subscription = await registration.pushManager.getSubscription();
        
        if (!this.subscription) {
            this.subscription = await registration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: urlBase64ToUint8Array(vapidPublicKey)
            });
        }

        // Retornar datos para Azure
        return {
            endpoint: this.subscription.endpoint,
            p256dh: arrayBufferToBase64(this.subscription.getKey('p256dh')),
            auth: arrayBufferToBase64(this.subscription.getKey('auth'))
        };
    }
}
