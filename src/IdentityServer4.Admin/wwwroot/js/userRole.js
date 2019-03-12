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
function addRole() {
    const userId = $('#userId').val();
    const role = $('.select2').val();
    if (role) {
        app.post("/api/user/" + userId + '/role/' + role, null, function () {
            window.location.reload();
        }, function (msg) {
            swal('Error', msg ? msg : 'Add failed', "error");
        });
    }
}