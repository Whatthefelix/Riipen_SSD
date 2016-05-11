//judge form to dynamically show judge score
/*
function getScore(firstLoad) {
    var allScores = $('.radioScore');
    var length = allScores.length;
    var sum = 0;
    var checkNum = 0;

    for (var i = 0; i < allScores.length; i++) {

        
        if ($(allScores[i]).is(':checked')) {
            sum = parseInt(sum) + parseInt($(allScores[i]).val());

            if (firstLoad) {
                //getcheckstart
                var getCheckStar = $(allScores[i]).parent().children("i");
                $(getCheckStar).addClass('hasChecked');
                $(getCheckStar).removeClass('fa-star-o').addClass("fa-star").addClass('star-hover');
                $(getCheckStar).parent().prevAll().children('i').removeClass('fa-star-o').addClass("fa-star").addClass('star-hover');
                $(getCheckStar).parent().nextAll().children('i').removeClass('fa-star').addClass("fa-star-o").removeClass('star-hover');
            }
           

            checkNum++;
        }
    }

    firstLoad = false;

    if (sum != 0) {
        sum = sum / checkNum;
        sum = Math.round(sum * 100) / 100;
        sum = sum.toString() + "/7";
        $(".yourScore").html(sum);

    }

}
*/


$(function () {
    $("ul.droptrue").sortable({
        connectWith: "ul"
    });

    $("#sortable1, #sortable2").disableSelection();


    //pick the winner

    $(".btn").click(function () {
        var teamIds = $('.TeamId:first').html();
        
       $("input[name='TeamId']").val(teamIds);
        console.log($("input[name='TeamId']").val());
  
    });

   
});


$(function () {

    $(".body-content").removeClass('container').addClass('container-fluid');

    getFinalScore();

   






    $('.range').change(function () {
        //  $(this).siblings('.side-score').html($(this).val());
        getFinalScore();
    });


    //get all score values
    function getFinalScore() {
        var sum = 0;
        var allRanges = $('.range');

        for (var i = 0; i < allRanges.length; i++) {
            sum = parseInt(sum) + parseInt($(allRanges[i]).val());

            $(allRanges[i]).siblings('.side-score').html($(allRanges[i]).val());


        }

        sum = sum / allRanges.length;
        sum = Math.round(sum * 100) / 100;
        sum = sum.toString() + " / 7";
        $(".yourScore").html(sum);
    }

});


