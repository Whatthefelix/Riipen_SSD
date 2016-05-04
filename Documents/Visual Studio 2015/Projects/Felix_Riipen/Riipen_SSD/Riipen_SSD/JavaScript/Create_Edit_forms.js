

$(function () {
    $('.multi-field-wrapper').each(function () {

        var $wrapper = $('.multi-fields', this);

        $(".add-criteria", $(this)).click(function (e) {
            var tempScrollTop = $(window).scrollTop();
            var criteria = $('.multi-field:first-child', $wrapper).clone(true);
            criteria.appendTo($wrapper).find('input').val('').focus();
            if ($('.multi-field', $wrapper).length == 0) {
                var criteriaInput = $('<div class="multi-field criteria-list-item"<label for="criteria[0].name" class="sr-only"></label><input name="criteria[0].name" placeholder="New Criteria" type="text"><input name="criteria[0].description" placeholder="" type="text"><button type="button" class="remove-field multi-field">Remove</button></div>');
                $($wrapper).append(criteriaInput);
            }
            $(window).scrollTop(tempScrollTop);
        });
 


        $('.multi-field .remove-field', $wrapper).click(function () {
            if ($('.multi-field', $wrapper).length >1) {
                $(this).parent('.multi-field').remove();
            }
            
            else if ($('.multi-field', $wrapper).length == 1) {
                $('input[name="criteria[0].name"]').val("");
                $('input[name="criteria[0].description"]').val("");
            }
        });
        $('.multi-field .judge-remove-field', $wrapper).click(function () {
            if ($('.multi-field', $wrapper).length > 1) {
                $(this).parent('.multi-field').remove();
            }

            else if ($('.multi-field', $wrapper).length == 1) {
                $('input[name="judges[0].firstname"]').val("");
                $('input[name="judges[0].lastname"]').val("");
                $('input[name="judges[0].email"]').val("");
            }
        });

    });
    $(function () {
        var $j = jQuery.noConflict();
        $j("#datepicker").datepicker();
    });
  
    $(function () {
        $('.multi-field-wrapper').each(function () {
            
        
            var $judgeWrapper = $('.multi-fields', this);
            $(".add-judge", $(this)).click(function (e) {
                var tempScrollTopJudge = $(window).scrollTop();
                var judge = $('.multi-field:first-child', $judgeWrapper).clone(true);
                judge.appendTo($judgeWrapper).find('input').val('').focus();
                $(window).scrollTop(tempScrollTopJudge);
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
       
