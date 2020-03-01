// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

// Add scrollspy to <body>

var divId;

$('.nav-link').click(function () {
    divId = $(this).attr('href');
    $('html, body').animate({
        scrollTop: $(divId).offset().top - 127
    }, 250);
});