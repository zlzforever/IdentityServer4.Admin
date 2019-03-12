$(function () {
    let a = $('a.PostLogoutRedirectUri');
    if (a) {
        window.location = a.attr('href');
    } else {
        window.location = '/';
    }
});