$(function () {

    $('.approve-message-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/ApproveMessage/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/Messages";
            },
            error: function () {
                alert('Error occured during approved.');
            }
        });
    });

    $('.reject-message-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/RejectMessage/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/Messages";
            },
            error: function () {
                alert('Error occured during reject.');
            }
        });
    });



});