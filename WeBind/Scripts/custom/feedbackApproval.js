$(function () {

    $('.approve-feedback-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/ApproveFeedBack/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/WebinarFeedback";
            },
            error: function () {
                alert('Error occured during approved.');
            }
        });
    });

    $('.reject-feedback-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/RejectFeedBack/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/WebinarFeedback";
            },
            error: function () {
                alert('Error occured during reject.');
            }
        });
    });



});