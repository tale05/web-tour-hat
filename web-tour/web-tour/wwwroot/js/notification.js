function showNotification(type, message) {
    const container = document.getElementById('notification-container');

    const notification = document.createElement('div');
    notification.classList.add('notification', type);

    let iconHtml = '';
    switch (type) {
        case 'error':
            iconHtml = '<i class="fas fa-times-circle"></i>';
            break;
        case 'success':
            iconHtml = '<i class="fas fa-check-circle"></i>';
            break;
        case 'warning':
            iconHtml = '<i class="fas fa-exclamation-triangle"></i>';
            break;
        case 'info':
            iconHtml = '<i class="fas fa-info-circle"></i>';
            break;
        default:
            iconHtml = '';
    }

    notification.innerHTML = `${iconHtml} <span>${message}</span>`;
    container.appendChild(notification);

    setTimeout(() => {
        notification.remove();
    }, 3000);
}