(function () {
    var form = document.getElementById('roomSearchForm');
    var results = document.getElementById('roomResults');
    var debounceTimer = null;
    var currentController = null;

    function loadUrl(url, historyMode) {
        if (currentController) {
            currentController.abort();
        }
        currentController = new AbortController();

        fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            signal: currentController.signal
        })
            .then(function (response) { return response.text(); })
            .then(function (html) {
                results.innerHTML = html;
                if (historyMode === 'push') {
                    history.pushState({ roomCatalogUrl: url }, '', url);
                } else if (historyMode === 'replace') {
                    history.replaceState({ roomCatalogUrl: url }, '', url);
                }
            })
            .catch(function (error) {
                if (error.name !== 'AbortError') {
                    showAppToast(false, 'Could not load rooms. Please try again.');
                }
            });
    }

    function submitSearch(historyMode) {
        var params = new URLSearchParams(new FormData(form));
        loadUrl(form.action + '?' + params.toString(), historyMode);
    }

    form.addEventListener('submit', function (event) {
        event.preventDefault();
        clearTimeout(debounceTimer);
        submitSearch('push');
    });

    form.addEventListener('input', function (event) {
        if (event.target.tagName !== 'INPUT') {
            return;
        }
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
            submitSearch('replace');
        }, 400);
    });

    document.addEventListener('click', function (event) {
        var link = event.target.closest('a.room-catalog-ajax-link');
        if (!link) {
            return;
        }
        if (link.origin !== window.location.origin) {
            return;
        }
        event.preventDefault();
        clearTimeout(debounceTimer);
        loadUrl(link.href, 'push');
    });

    window.addEventListener('popstate', function () {
        clearTimeout(debounceTimer);
        loadUrl(window.location.href, null);
    });
})();
