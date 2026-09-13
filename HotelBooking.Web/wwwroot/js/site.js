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
