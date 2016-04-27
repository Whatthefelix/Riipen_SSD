

$(function () {
    $('.multi-field-wrapper').each(function () {

        var $wrapper = $('.multi-fields', this);

        $(".add-criteria", $(this)).click(function (e) {
         
            var criteria = $('.multi-field:first-child', $wrapper).clone(true);
            criteria.appendTo($wrapper).find('input').val('').focus();
        });


        $('.multi-field .remove-field', $wrapper).click(function () {
            if ($('.multi-field', $wrapper).length > 1)
                $(this).parent('.multi-field').remove();
        });
    });
  
    $(function () {
        $('.multi-field-wrapper').each(function () {
            var $judgeWrapper = $('.multi-fields', this);

            $(".add-judge", $(this)).click(function (e) {
             
                var judge = $('.multi-field:first-child', $judgeWrapper).clone(true);
                console.log($($judgeWrapper));
                console.log($('.multi-field:first-child'));
                judge.appendTo($judgeWrapper).find('input').val('').focus();
            });

                    
            $('.multi-field .remove-field', $judgeWrapper).click(function () {
                if ($('.multi-field', $judgeWrapper).length > 1)
                    $(this).parent('.multi-field').remove();     
           
            });
        });
    });


    $('#uploadExcel').click(function () {
        $('#excelPreview').show();
    });

    $("#ContestForm").submit(function (e) {
        e.preventDefault();
        var judgeListItems = $('.judge-list-item');
        $(judgeListItems).each(function (index, element) {
            var children = $(element).children();
            $(children[0]).attr('name', 'judges[' + index + '].firstname');
            $(children[1]).attr('name', 'judges[' + index + '].lastname');
            $(children[2]).attr('name', 'judges[' + index + '].email');    
        })
        var criteriaListItems = $('.criteria-list-item');
        $(criteriaListItems).each(function (index, element) {
            var children = $(element).children();
            $(children[0]).attr('name', 'criteria[' + index + '].name');
            $(children[1]).attr('name', 'criteria[' + index + '].description');   
        });
        this.submit();
    });
});
       
