window.addEventListener('load', function () {
    const overlay = document.getElementById('loading-overlay');
    if (overlay) {
        overlay.style.transition = 'opacity 0.5s ease';
        overlay.style.opacity = '0';
        setTimeout(function () {
            overlay.classList.add('hidden');
        }, 500);
    }
});