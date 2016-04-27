

$(function () {
    $('.multi-field-wrapper').each(function () {

        var $wrapper = $('.multi-fields', this);

        $(".add-criteria", $(this)).click(function (e) {
         
            var criteria = $('.multi-field:first-child', $wrapper).clone(true);
            criteria.appendTo($wrapper).find('input').val('').focus();
            if ($('.multi-field', $wrapper).length == 0) {
                var criteriaInput = $('<div class="multi-field criteria-list-item"<label for="criteria[0].name" class="sr-only"></label><input name="criteria[0].name" placeholder="New Criteria" type="text"><input name="criteria[0].description" placeholder="" type="text"><button type="button" class="remove-field multi-field">Remove</button></div>');
                $($wrapper).append(criteriaInput);
            }
        });


        $('.multi-field .remove-field', $wrapper).click(function () {
            if ($('.multi-field', $wrapper).length >1) {
                $(this).parent('.multi-field').remove();
            }
            
            else if ($('.multi-field', $wrapper).length == 1) {
                $('input[name="item.Name"]').val("");
                $('input[name="item.Description"]').val("");
            }
        });
        $('.multi-field .judge-remove-field', $wrapper).click(function () {
            if ($('.multi-field', $wrapper).length > 1) {
                $(this).parent('.multi-field').remove();
            }

            else if ($('.multi-field', $wrapper).length == 1) {
                $('input[name="item.FirstName"]').val("");
                $('input[name="item.LastName"]').val("");
                $('input[name="item.Email"]').val("");
            }
        });

    });
  
    $(function () {
        $('.multi-field-wrapper').each(function () {
            var $judgeWrapper = $('.multi-fields', this);

            $(".add-judge", $(this)).click(function (e) {

                var judge = $('.multi-field:first-child', $judgeWrapper).clone(true);
                judge.appendTo($judgeWrapper).find('input').val('').focus();
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
       
