$(function () {
    var $participantsTableBody = $('#participants-table > tbody');

    $("#file-upload").change(function () {

        if (!$('#file-upload')[0].files.length) {
            return;
        }

        $("#file-upload").parse({
            config: {
                header: true,
                skipEmptyLines: true,
                complete: function (results, file) {
                    $participantsTableBody.empty();
                    var data = results["data"];
                    for (var i = 0; i < data.length; i++) {
                        $participantsTableBody.append("<tr class=''>"
                        + "<td>" + data[i]["TeamName"] + "</td>"
                        + "<td>" + data[i]["FirstName"] + "</td>"
                        + "<td>" + data[i]["LastName"] + "</td>"
                        + "<td>" + data[i]["Email"] + "</td>"
                        + "<td>" + data[i]["RiipenUrl"] + "</td>"
                        + "</tr>");
                    }
                }
            },
        });
    });
});