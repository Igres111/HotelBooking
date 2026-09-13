document.getElementById('editRoomModal').addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var form = document.getElementById('editRoomForm');

    form.action = '/Admin/UpdateRoom/' + button.getAttribute('data-room-id');
    document.getElementById('editRoomName').value = button.getAttribute('data-name');
    document.getElementById('editRoomDescription').value = button.getAttribute('data-description') || '';
    document.getElementById('editRoomLocation').value = button.getAttribute('data-location');
    document.getElementById('editRoomCapacity').value = button.getAttribute('data-capacity');
    document.getElementById('editRoomOpeningTime').value = button.getAttribute('data-opening-time');
    document.getElementById('editRoomClosingTime').value = button.getAttribute('data-closing-time');
    document.getElementById('editRoomIsActive').checked = button.getAttribute('data-is-active') === 'true';
});
