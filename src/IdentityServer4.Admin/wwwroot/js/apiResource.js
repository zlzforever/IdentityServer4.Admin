function remove(id) {
    swal({
        title: "Sure to delete api resource?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.delete("/api/api-resource/" + id, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Delete failed', "error");
        });
    });
}

function disable(id) {
    swal({
        title: "Sure to disable api resource?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.put("/api/api-resource/" + id + '/disable', null, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Disable failed', "error");
        });
    });
}

function enable(id) {
    app.put("/api/api-resource/" + id + '/enable', null, function () {
        window.location.reload();
    }, function () {
        swal('Error', 'Enable failed', "error");
    });
}