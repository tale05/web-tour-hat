$(document).ready(function () {
    const $btn = $('#navbar-toggle');
    const $menu = $('#navbar-menu');
    let menuActive = false;

    $btn.on('click', function () {
        $menu.toggleClass('active');
        menuActive = $menu.hasClass('active');

        $btn.html(menuActive
            ? '<i class="fas fa-times"></i>'
            : '<i class="fas fa-bars"></i>');
    });

    $menu.on('click', 'a', function (e) {
        if (menuActive && !$(this).closest('li').hasClass('dropdown-category-for-hat-navbar')) {
            $menu.removeClass('active');
            $btn.html('<i class="fas fa-bars"></i>');
            menuActive = false;
        }
    });

    // Xử lý scroll
    function updateNavbarMenuPosition() {
        const scrollTop = $(window).scrollTop();
        const isMobile = window.innerWidth <= 767;

        if (isMobile) {
            if (scrollTop === 0) {
                $menu.addClass('scroll-top');
            } else {
                $menu.removeClass('scroll-top');
            }
        } else {
            $menu.removeClass('scroll-top');
        }
    }

    $(window).on('scroll resize', updateNavbarMenuPosition);
    updateNavbarMenuPosition();

    // --- Mới: Xử lý toggle dropdown con ở mobile ---
    $('.dropdown-category-for-hat-navbar > a').on('click', function (e) {
        if (window.innerWidth <= 767) {
            // Ngăn hành vi chuyển trang khi click vào mục cha
            /*e.preventDefault();*/
            $(this).parent().toggleClass('active');
        }
    });
});
