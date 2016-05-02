function getScore() {
    var allScores = $('.radioScore');
    var length = allScores.length;
    var sum = 0;
    var checkNum = 0;

    for (var i = 0; i < allScores.length; i++) {

        if ($(allScores[i]).is(':checked')) {
            sum = parseInt(sum) + parseInt($(allScores[i]).val());
            checkNum++;
        }
    }

    if (sum != 0) {
        sum = sum / checkNum;
        console.log(sum);
        sum = sum.toString() + "/7"
        ; $(".yourScore").html(sum);

    }


}

$(function () {
    getScore();
    $('.radioScore').click(function () {
        getScore();
    });


});