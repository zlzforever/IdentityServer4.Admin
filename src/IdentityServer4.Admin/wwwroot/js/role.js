$(function () {
    $('#Search').on('click', function () {
        window.location.href = '/role?q=' + $('#Keyword').val();
    });
});

function remove(id) {
    swal({
        title: "Sure to remove this role?",
        type: "warning",
        showCancelButton: true
    }, function () {
        app.delete("/api/role/" + id, function () {
            window.location.reload();
        }, function () {
            swal('Error', 'Delete failed', "error");
        });
    });
}
