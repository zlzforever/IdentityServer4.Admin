$(function () {
    $('#Search').on('click', function () {
        window.location.href = '/user?q=' + $('#Keyword').val();
    });
});

function remove(userId) {
    swal({
        title: "Sure to delete this user?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.delete("/api/user/" + userId, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Delete failed', "error");
        });
    });
}

function disable(userId) {
    swal({
        title: "Sure to lock this user?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.put("/api/user/" + userId + '/disable', null, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Lock failed', "error");
        });
    });
}

function enable(userId) {
    app.put("/api/user/" + userId + '/enable', null, function () {
        window.location.reload();
    }, function () {
        swal('Error', 'Enable failed', "error");
    });
}