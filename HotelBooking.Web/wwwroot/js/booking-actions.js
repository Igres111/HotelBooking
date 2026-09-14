(function () {
    document.addEventListener('submit', function (event) {
        var form = event.target;
        if (!form.classList || !form.classList.contains('booking-action-form')) {
            return;
        }

        event.preventDefault();

        var actionType = form.dataset.actionType;
        var row = form.closest('[data-booking-row]');
        var submitButton = form.querySelector('button[type="submit"]');
        if (submitButton) {
            submitButton.disabled = true;
        }

        fetch(form.action, {
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            body: new FormData(form)
        })
            .then(function (response) { return response.json(); })
            .then(function (data) {
                showAppToast(data.success, data.message);

                if (data.success && row) {
                    updateBookingRow(row, actionType, data.status);
                } else if (submitButton) {
                    submitButton.disabled = false;
                }
            })
            .catch(function () {
                showAppToast(false, 'Something went wrong. Please try again.');
                if (submitButton) {
                    submitButton.disabled = false;
                }
            });
    });

    function updateBookingRow(row, actionType, status) {
        var badge = row.querySelector('.status-badge');
        if (badge && status) {
            badge.textContent = status;
            badge.className = 'status-badge status-badge-' + status.toLowerCase();
        }

        var actionsContainer = row.querySelector('[data-booking-actions]');
        if (!actionsContainer) {
            return;
        }

        if (actionType === 'confirm') {
            actionsContainer.querySelectorAll('[data-action-type="confirm"], [data-action-type="reject"]').forEach(function (form) {
                form.remove();
            });
        } else {
            actionsContainer.querySelectorAll('form').forEach(function (form) {
                form.remove();
            });
        }

        if (actionsContainer.tagName === 'TD' && actionsContainer.querySelectorAll('form').length === 0 && !actionsContainer.textContent.trim()) {
            actionsContainer.textContent = '-';
        }
    }
})();
