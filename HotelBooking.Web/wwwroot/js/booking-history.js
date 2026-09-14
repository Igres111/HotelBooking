document.getElementById('bookingHistoryModal').addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var bookingId = button.getAttribute('data-booking-id');
    var body = document.getElementById('bookingHistoryModalBody');

    body.innerHTML = '<div class="empty-state"><p>Loading...</p></div>';

    fetch('/Admin/BookingHistoryEntries/' + bookingId, {
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
        .then(function (response) { return response.text(); })
        .then(function (html) {
            body.innerHTML = html;
        })
        .catch(function () {
            showAppToast(false, 'Could not load booking history.');
        });
});
