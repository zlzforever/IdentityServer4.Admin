function remove(resourceId, id) {
    swal({
        title: "Sure to delete this api scope?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.delete("/api/api-resource/" + resourceId + "/scope/" + id, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Delete failed', "error");
        });
    });
}
