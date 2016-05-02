$(function () {
    var allScores = $('.radioScore');
    var length = allScores.length;
   

    $('.radioScore').click(function () {
        var sum = 0;
        var checkNum = 0;
       
        for (var i = 0; i < allScores.length; i++) {
            
            if ($(allScores[i]).is(':checked')) {                
                sum = parseInt(sum) + parseInt($(allScores[i]).val());
                checkNum++;
            }          
        }
       
        sum = sum / checkNum;
        sum = sum.toString() + "/7"
        ;        $(".yourScore").html(sum);
        
    });

 

   
   

});