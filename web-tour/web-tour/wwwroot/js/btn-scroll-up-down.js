document.addEventListener('DOMContentLoaded', function () {
    // Tạo nút cuộn lên đầu trang
    const btnTop = document.createElement('button');
    btnTop.innerText = "↑";
    btnTop.id = "scrollToTopBtn";
    btnTop.style.position = "fixed";
    btnTop.style.right = "24px";
    btnTop.style.bottom = "24px";
    btnTop.style.zIndex = "1000";
    btnTop.style.display = "none";
    btnTop.style.padding = "10px 18px";
    btnTop.style.borderRadius = "8px";
    btnTop.style.background = "#FFC107";
    btnTop.style.color = "#fff";
    btnTop.style.border = "none";
    btnTop.style.cursor = "pointer";
    btnTop.style.boxShadow = "0 2px 8px rgba(0,0,0,0.12)";
    btnTop.style.fontSize = "1rem";

    document.body.appendChild(btnTop);

    // Sự kiện click cuộn lên đầu
    btnTop.addEventListener('click', function () {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });

    // Cập nhật hiển thị nút khi cuộn trang
    function updateButtonVisibility() {
        const scrolled = window.scrollY || window.pageYOffset;
        btnTop.style.display = scrolled > 100 ? 'block' : 'none';
    }

    window.addEventListener('scroll', updateButtonVisibility);
    window.addEventListener('resize', updateButtonVisibility);
    updateButtonVisibility(); // chạy lúc load
});