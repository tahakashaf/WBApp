$(function () {

    $('.approve-campusrequest-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/ApproveCampusRequest/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/CampusRequest";
            },
            error: function () {
                alert('Error occured during approved.');
            }
        });
    });

    $('.reject-campusrequest-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/RejectCampusRequest/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/CampusRequest";
            },
            error: function () {
                alert('Error occured during reject.');
            }
        });
    });



});