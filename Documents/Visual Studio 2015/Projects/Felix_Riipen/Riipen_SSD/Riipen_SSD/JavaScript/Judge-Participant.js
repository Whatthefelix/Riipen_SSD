
$(function () {
  
    $(".body-content").removeClass('container').addClass('container-fluid');
    
    getFinalScore();


    //get popupover work for team member
    $(".teammember").popover({
        html: true,
        content: function () {
            return $('.team-member-div').html();
        },
        show: true
    });


   
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


