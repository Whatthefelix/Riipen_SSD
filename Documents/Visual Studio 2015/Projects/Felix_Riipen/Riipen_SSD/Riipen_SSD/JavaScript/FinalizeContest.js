

$(function () {
    $("ul.droptrue").sortable({
        connectWith: "ul"
    });

    $("#sortable1, #sortable2").disableSelection();


    //pick the winner

    $(".btn").click(function () {
        var teamIds = $("#sortable1 .TeamId");

        if (teamIds.length != 3) {
            alert("Please pick the first three teams");
            return false;
        } 
              
        var FirstId = $(teamIds[0]).html();
        var SecondId = $(teamIds[1]).html();
        var ThirdId = $(teamIds[2]).html();;
        
        $("input[name='FirstId']").val(FirstId);
        $("input[name='SecondId']").val(SecondId);
        $("input[name='ThirdId']").val(ThirdId);

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


