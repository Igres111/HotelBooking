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
