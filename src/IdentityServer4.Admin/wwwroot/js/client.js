function remove(id) {
    swal({
        title: "Sure to delete this client?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.delete("/api/client/" + id, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Delete failed', "error");
        });
    });
}