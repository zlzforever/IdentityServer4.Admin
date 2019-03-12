function remove(id) {
    swal({
        title: "Sure to delete this identity resource?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.delete("/api/identity-resource/" + id, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Delete failed', "error");
        });
    });
}

function disable(id) {
    swal({
        title: "Sure to disable identity resource?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.put("/api/identity-resource/" + id + '/disable', null, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Disable failed', "error");
        });
    });
}

function enable(id) {
    app.put("/api/identity-resource/" + id + '/enable', null, function () {
        window.location.reload();
    }, function () {
        swal('Error', 'Enable failed', "error");
    });
}