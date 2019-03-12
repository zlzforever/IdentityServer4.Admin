$(function () {
    debugger;
    let a = $('a.PostLogoutRedirectUri');
    if (a && a.attr('href')) {
        window.location = a.attr('href');
    } else {
        window.location = '/';
    }
});