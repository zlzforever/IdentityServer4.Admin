function remove(roleId) {
    const userId = $('#userId').val();
    swal({
        title: "Sure to remove this role?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.delete("/api/user/" + userId + '/role/' + roleId, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Delete failed', "error");
        });
    });
}