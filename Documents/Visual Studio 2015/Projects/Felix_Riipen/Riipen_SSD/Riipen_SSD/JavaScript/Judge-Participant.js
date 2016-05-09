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
  
    $(".body-content").removeClass('container').addClass('container-fluid');
    
    getFinalScore();

    //show the fault when page load


    /*
    firstLoad = true;
    getScore(firstLoad);
    


    //star style for raido button

    $(".star").mouseenter(function () {
        $(this).removeClass('fa-star-o').addClass("fa-star").addClass('star-hover');
        $(this).parent().prevAll().children('i').removeClass('fa-star-o').addClass("fa-star").addClass('star-hover');
        $(this).parent().nextAll().children('i').removeClass('fa-star').addClass("fa-star-o").removeClass('star-hover');
    }).mouseleave(function () {

        //find the checked
        var getChecked = null;

        if ($(this).hasClass('hasChecked')) {

            getChecked = $(this);

        } else {

            var allStars = $(this).parent().siblings().children('i');
            for (var i = 0; i < allStars.length; i++) {

                if ($(allStars[i]).hasClass('hasChecked')) {
                    getChecked = allStars[i];
                    break;
                }
            }
        }

        if (getChecked != null) {
            $(getChecked).removeClass('fa-star-o').addClass("fa-star").addClass('star-hover');
            $(getChecked).parent().prevAll().children('i').removeClass('fa-star-o').addClass("fa-star").addClass('star-hover');
            $(getChecked).parent().nextAll().children('i').removeClass('fa-star').addClass("fa-star-o").removeClass('star-hover');
        } else {
            $(this).removeClass('fa-star').addClass("fa-star-o").removeClass('star-hover');
            $(this).parent().siblings().children('i').removeClass('fa-star').addClass("fa-star-o").removeClass('star-hover');
        }



    });

    $(".star").click(function () {
        $(this).parent().siblings().children('i').removeClass('hasChecked');
        $(this).addClass("hasChecked");
        var needCheced = $(this).parent().children('.radioScore');
        $(needCheced).prop("checked", true);
        getScore(firstLoad);
    });


    $(".submit-form").click(function () {       

        var checkScores = $('.radioScore');
        var length = checkScores.length;
        var checkNumber = 0;
        for (var i = 0; i < checkScores.length; i++) {
            if ($(checkScores[i]).is(':checked')) {
                checkNumber++;
                
            }
        }

        if (checkNumber != length / 7) {
            $(".validateScoreInput").html("Please give all criterion a score before you save or submit.");
            setInterval(function () {
                $(".validateScoreInput").html("");

            }, 10000);
            return false;
        }
    });
   
   */
    //show range slide score for each criteria
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


