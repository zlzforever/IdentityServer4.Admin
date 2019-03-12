$(function () {

    $('#head-icon-uploader').on("change", function () {
        if (this.files && this.files[0]) {
            let reader = new FileReader();
            reader.onload = function (e) {
                $.ajax({
                    url: '/account/head-icon',
                    data: e.target.result,
                    headers: {
                        RequestVerificationToken: $('input[name$="__RequestVerificationToken"]').val()
                    },
                    method: 'POST',
                    success: function (result) {
                        $('#head-icon').attr('src', '/' + result + '?_=' + Math.round(new Date().getTime()))
                    }
                });
            };
            reader.readAsDataURL(this.files[0]);
        }
    });
});

