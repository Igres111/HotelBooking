(function () {
    var overlay = document.getElementById('page-loading-overlay');
    if (!overlay) {
        return;
    }

    window.addEventListener('beforeunload', function () {
        overlay.style.display = 'flex';
    });

    window.addEventListener('pageshow', function (event) {
        if (event.persisted) {
            overlay.style.display = 'none';
        }
    });
})();

(function () {
    var toggle = document.getElementById('sidebarToggle');
    var sidebar = document.getElementById('sidebar');
    if (!toggle || !sidebar) {
        return;
    }

    toggle.addEventListener('click', function () {
        sidebar.classList.toggle('open');
    });

    document.addEventListener('click', function (event) {
        if (sidebar.classList.contains('open') && !sidebar.contains(event.target) && !toggle.contains(event.target)) {
            sidebar.classList.remove('open');
        }
    });
})();

document.addEventListener('submit', function (event) {
    var form = event.target;
    if (!form.classList || !form.classList.contains('disable-on-submit')) {
        return;
    }

    var submitButton = form.querySelector('button[type="submit"]');
    if (submitButton) {
        submitButton.disabled = true;
    }
});

function showAppToast(success, message) {
    var stack = document.querySelector('.toast-stack');
    if (!stack) {
        stack = document.createElement('div');
        stack.className = 'toast-stack';
        document.body.appendChild(stack);
    }

    var toast = document.createElement('div');
    toast.className = 'app-toast ' + (success ? 'app-toast-success' : 'app-toast-error');
    toast.textContent = message;
    stack.appendChild(toast);

    requestAnimationFrame(function () {
        toast.classList.add('show');
    });

    setTimeout(function () {
        toast.classList.remove('show');
        setTimeout(function () {
            toast.remove();
        }, 200);
    }, 3000);
}
