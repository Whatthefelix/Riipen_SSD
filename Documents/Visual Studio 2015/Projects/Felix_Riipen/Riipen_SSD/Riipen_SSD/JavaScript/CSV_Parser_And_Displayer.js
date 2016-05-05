$(function () {
    var $participants = $('.participants');
    $("#file-upload").change(function () {

        if (!$('#file-upload')[0].files.length) {
            return;
        }

        $("#file-upload").parse({
            config: {
                header: true,
                skipEmptyLines: true,
                complete: function (results, file) {
                    $participants.empty();
                    var data = results["data"];
                    for (var i = 0; i < data.length; i++) {
                        $participants.append("<div class='multi-field participant-list-item form-inline'>"
                        + "<input class='form-control' type='text' value='" + data[i]["TeamName"] + "'/>"
                        + "<input class='form-control' type='text' value='" + data[i]["FirstName"] + "'/>"
                        + "<input class='form-control' type='text' value='" + data[i]["LastName"] + "'/>"
                        + "<input class='form-control' type='text' value='" + data[i]["Email"] + "'/>"
                        + "<button type='button' class='participant-remove-field btn btn-warning'>Remove</button>"
                        + "</div>");
                    }
                }
            },
        });
        $(this).prop("value", "");
    });
});