document.addEventListener('DOMContentLoaded', function () {
    const togglePassword = document.querySelector('#togglePassword');
    const password = document.querySelector('#password');
    const username = document.querySelector('#username');
    const loginBtn = document.getElementById('loginBtn');

    // Giới hạn ký tự nhập
    if (username) {
        username.setAttribute('maxlength', '50');
    }
    if (password) {
        password.setAttribute('maxlength', '50');
    }

    // Toggle hiển thị mật khẩu
    if (togglePassword && password) {
        togglePassword.addEventListener('click', function () {
            const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
            password.setAttribute('type', type);

            // Toggle icon
            if (type === 'password') {
                this.classList.remove('fa-eye-slash');
                this.classList.add('fa-eye');
            } else {
                this.classList.remove('fa-eye');
                this.classList.add('fa-eye-slash');
            }
        });
    }

    // Gọi callback từ reCAPTCHA
    window.recaptchaCallback = function () {
        if (loginBtn) loginBtn.disabled = false;
    };
});