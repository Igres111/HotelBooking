(function () {
    var form = document.getElementById('availabilityForm');
    var results = document.getElementById('availabilityResults');
    if (!form || !results) {
        return;
    }

    var currentController = null;

    function loadUrl(url, historyMode) {
        if (currentController) {
            currentController.abort();
        }
        currentController = new AbortController();

        results.classList.add('loading');

        fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            signal: currentController.signal
        })
            .then(function (response) { return response.text(); })
            .then(function (html) {
                results.innerHTML = html;
                results.classList.remove('loading');
                if (historyMode === 'push') {
                    history.pushState({ availabilityUrl: url }, '', url);
                } else if (historyMode === 'replace') {
                    history.replaceState({ availabilityUrl: url }, '', url);
                }
            })
            .catch(function (error) {
                if (error.name !== 'AbortError') {
                    results.classList.remove('loading');
                    showAppToast(false, 'Could not load availability. Please try again.');
                }
            });
    }

    form.addEventListener('submit', function (event) {
        event.preventDefault();
        var params = new URLSearchParams(new FormData(form));
        loadUrl(form.action + '?' + params.toString(), 'push');
    });

    window.addEventListener('popstate', function () {
        loadUrl(window.location.href, null);
    });
})();

document.getElementById('bookSlotModal').addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    document.getElementById('bookSlotRoomId').value = button.getAttribute('data-room-id');
    document.getElementById('bookSlotDate').value = button.getAttribute('data-date');
    document.getElementById('bookSlotStartTime').value = button.getAttribute('data-start-time');
    document.getElementById('bookSlotEndTime').value = button.getAttribute('data-end-time');
    document.getElementById('bookSlotTimeZoneId').value = button.getAttribute('data-time-zone-id');
    document.getElementById('bookSlotLabel').textContent = button.getAttribute('data-label');
    hideModalMessage('bookSlotMessage');
});

document.getElementById('createRecurringModal').addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    document.getElementById('recurRoomId').value = button.getAttribute('data-room-id');
    document.getElementById('recurStartDate').value = button.getAttribute('data-date');
    document.getElementById('recurStartTime').value = button.getAttribute('data-start-time');
    document.getElementById('recurEndTime').value = button.getAttribute('data-end-time');
    document.getElementById('recurTimeZoneId').value = button.getAttribute('data-time-zone-id');
    document.getElementById('recurSlotLabel').textContent = button.getAttribute('data-label');
    hideModalMessage('recurMessage');
});

function hideModalMessage(messageId) {
    var el = document.getElementById(messageId);
    el.className = 'alert d-none';
    el.textContent = '';
}

function showModalMessage(messageId, success, message) {
    var el = document.getElementById(messageId);
    el.className = 'alert ' + (success ? 'alert-success' : 'alert-danger');
    el.textContent = message;
}

function submitBookingForm(form, messageId, onSuccess) {
    var submitButton = form.querySelector('button[type="submit"]');
    submitButton.disabled = true;

    fetch(form.action, {
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        body: new FormData(form)
    })
        .then(function (response) { return response.json(); })
        .then(function (data) {
            showModalMessage(messageId, data.success, data.message);
            if (data.success) {
                setTimeout(onSuccess, 1200);
            } else {
                submitButton.disabled = false;
            }
        })
        .catch(function () {
            showModalMessage(messageId, false, 'Something went wrong. Please try again.');
            submitButton.disabled = false;
        });
}

var bookSlotForm = document.querySelector('#bookSlotModal form');
bookSlotForm.addEventListener('submit', function (event) {
    event.preventDefault();
    submitBookingForm(bookSlotForm, 'bookSlotMessage', function () {
        bootstrap.Modal.getInstance(document.getElementById('bookSlotModal')).hide();
        bookSlotForm.reset();
        hideModalMessage('bookSlotMessage');
        bookSlotForm.querySelector('button[type="submit"]').disabled = false;
    });
});

var recurForm = document.querySelector('#createRecurringModal form');
recurForm.addEventListener('submit', function (event) {
    event.preventDefault();
    submitBookingForm(recurForm, 'recurMessage', function () {
        bootstrap.Modal.getInstance(document.getElementById('createRecurringModal')).hide();
        recurForm.reset();
        hideModalMessage('recurMessage');
        recurForm.querySelector('button[type="submit"]').disabled = false;
    });
});
