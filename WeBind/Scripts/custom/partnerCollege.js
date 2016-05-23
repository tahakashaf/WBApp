$(function () {

    $('.setcampus-message-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/SetPartner/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/PartnerColleges";
            },
            error: function () {
                alert('Error occured during approved.');
            }
        });
    });

    $('.unsetcampus-message-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "http://webind.in/Admin/UnSetPartner/" + id,
            dataType: "json",
            success: function (data) {
                //alert('Approved');
                window.location.href = "http://webind.in/Admin/PartnerColleges";
            },
            error: function () {
                alert('Error occured during reject.');
            }
        });
    });



});