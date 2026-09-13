document.getElementById('bookSlotModal').addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    document.getElementById('bookSlotRoomId').value = button.getAttribute('data-room-id');
    document.getElementById('bookSlotDate').value = button.getAttribute('data-date');
    document.getElementById('bookSlotStartTime').value = button.getAttribute('data-start-time');
    document.getElementById('bookSlotEndTime').value = button.getAttribute('data-end-time');
    document.getElementById('bookSlotTimeZoneId').value = button.getAttribute('data-time-zone-id');
    document.getElementById('bookSlotLabel').textContent = button.getAttribute('data-label');
});

document.getElementById('createRecurringModal').addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    document.getElementById('recurRoomId').value = button.getAttribute('data-room-id');
    document.getElementById('recurStartDate').value = button.getAttribute('data-date');
    document.getElementById('recurStartTime').value = button.getAttribute('data-start-time');
    document.getElementById('recurEndTime').value = button.getAttribute('data-end-time');
    document.getElementById('recurTimeZoneId').value = button.getAttribute('data-time-zone-id');
    document.getElementById('recurSlotLabel').textContent = button.getAttribute('data-label');
});
