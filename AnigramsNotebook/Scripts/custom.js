var $grid = $('.grid').imagesLoaded(function () {
    // init Masonry after all images have loaded
    $grid.masonry({
        itemSelector: '.grid-item',
        columnWidth: '.grid-sizer',
        fitWidth: true,
        gutter: 4,
        resize: false
    });
});

$(function () {
    //add class active to currently selected nav link
    $('.demo-navigation a.mdl-navigation__link').each(function () {
        var a = $(this);
        var href = a.prop('href');
        href = href.substring(href.indexOf('://')+3, href.length);
        var hrefSplit = href.split("/");
        var controller = hrefSplit[1];
        controller = controller.substring(0, controller.indexOf('?'));
        var color = a.find('i').css('color');

        if (window.location.href.indexOf(controller) !== -1) {
            a.css('color', '#fff').css('background-color', color);
            a.find('i').css('color', '#fff');
            a.find('span').css('color', '#fff');
        }
    });

    function insertTextAtPosition(element, textToInsert) {
        var $txt = $(element);
        var caretPos = $txt[0].selectionStart;
        var textAreaTxt = $txt.val();
        var txtToAdd = textToInsert;
        $txt.val(textAreaTxt.substring(0, caretPos) + txtToAdd + textAreaTxt.substring(caretPos));
    }

    $("#btnh3").click(function () {
        insertTextAtPosition("#Description", "<h3></h3>");
        $("#Description").focus();
    });

    $("#btnul").click(function () {
        insertTextAtPosition("#Description", "<ul>\n<li></li>\n</ul>");
        $("#Description").focus();
    });

    $("#btnli").click(function () {
        insertTextAtPosition("#Description", "<li></li>");
        $("#Description").focus();
    });

    $("#btnarrow").click(function () {
        insertTextAtPosition("#Description", "->");
        $("#Description").focus();
    });
});