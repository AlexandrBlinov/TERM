/*function AddToSeasonCartAllOrder(postId) {

    $('table>tbody').find('tr').each(function (row) {

       var code = $(this).find('.Code').text().trim();
       var count = $(this).find('.Count').text().trim();

       if ($.isNumeric(code) && code > 0 && $.isNumeric(count)) {

           $.ajax('/SeasonShoppingCart/AddToCart', {
               'async': false,
               'method': 'post',
               'data': { "id": code, "count": count },
               'success': function (data) {
                   if (data.Success) {
                       
                   } else
                       toastr.error(data.Message);
               }

           })
       }
    });

    $.post("/seasonstock/editnotification", { "postid": postId },
        function (data) {

            if (data.Success) {
                window.location.href = data.Message;
            } else
                toastr.error(data.Message);
        });

    

} */


function getValueById(Id)
{ return (document.getElementById(Id).value);}

// действует при установлении DontShowZakupPrice
function EnableDisableZakupPrice()
{
    
    var checked = $("#DontShowZakupPrice").prop("checked");
    if (checked) {
        $("input[type=checkbox][name=zakup]:checked").prop("checked", false).closest('tr').each(function () {
            $(this).data("active_name", "recommended");
            $(this).find("input[type=checkbox][name=recommend]").each(function ()
            { $(this).prop("checked", true) });
        });
        
    }

    $("input[type=checkbox][name=zakup]").prop("disabled", checked);
}

function RemoveZakupPrice()
{
    $("td.zakup, th.zakup").remove();
}

function callbackfindElementsInstockByName(element)
{

    var idtosearch = $(element).val();
    if (idtosearch)
        $('.elements_in_stock td:nth-child(1)').each(function () {
            if ($(this).text().indexOf(idtosearch) >= 0)
                $(this).parent('tr').addClass('found-by-id');
        });

}

$(function () {
    
    /*
    Клик по сроку доставки из подборщика дисков
    */
    $(document).on('mouseover', '.productonway-hint', function () {
        $element = $(this);
        var id = $(this).data('id');
        if(!id) return;
        var path = '/api/onwayitems/' + id;
        $.getJSON(path, function (data) {
         
            if (!data || data.length === 0) return;
            var resultArr = $.map(data, function (p) { return { numberOfDays: p.NumberOfDays, count: p.Count } });
            
           var contents = '<table class=\'product-hint\'>';

           contents += '<thead><tr><th>Число дней</th><th>Кол-во</th></tr></thead>';

            for (var i = 0; i < resultArr.length; i++) {
                contents += '<tr><td>' + resultArr[i].numberOfDays + '</td>' + '<td>' + resultArr[i].count + '</td></tr>';
            }

            contents += '</table>';

            $element.popover({
                'content': contents,
                placement: 'left',
                'html': true,
                trigger: 'hover focus'
            }).popover('toggle');
     
        });
        

    });


   
    //$('.removefromseasoncart').on("click", function () { RemoveFromSeasonCart(this); return false; });

    // подтверждаем удаление партнерской точки


    $("[name=findelementsInStock]").change(function () { callbackfindElementsInstockByName(this); }).keydown(function () { callbackfindElementsInstockByName(this); });


    $('[data-toggle="tooltip"]').tooltip();
  
    $('.collapse').on('shown.bs.collapse', function () {
        $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
    }).on('hidden.bs.collapse', function () {
        $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
    });

    $('table#table-points tr').click(function () {
        window.location.assign('/Partner/Edit?PointId=' + $(this).data('id'));

    });

    $("#ButDeletePartnerPoint").on('click', function () {
        var pointId = parseInt($(this).data('partnerpointid'))||0;
       // console.log(partnerpointid);
        $('#idModalToDeletePoint').modal('hide');
        
        if (pointId == 0) return;

        $.post("/Partner/Delete", { "PointId": pointId }, function (data)
        {
            if (!data.Success) {
                toastr.error(data.Message);
                evt.stopPropagation();
            }
            else
                window.location.reload(true); // success
        });
       
    });

    $('.delete-point').on('click', function (evt) {
        var partnerPointId = $(this).data('point');
        var modalWindow = $('#idModalToDeletePoint');

        modalWindow.find('#ButDeletePartnerPoint').data('partnerpointid', partnerPointId);
        modalWindow.find('span').text(partnerPointId);
        modalWindow.modal();
        evt.stopPropagation();

        
    });

    $('#ButBlockPartnerPoint').on('click', function () {
        var pointId = parseInt($(this).data('partnerpointid')) || 0;
        var action =false;
        console.log({ "PointId": pointId, "action": action });
        $.post("/Partner/DisableEnable", { "PointId": pointId, "action": action }, function (data) {
            if (!data.Success)
                toastr.error(data.Message);

            else
                window.location.reload(true); // success

        });
        return false;
    }
    );

    $('.disable-point').on('click', function (evt) {
        var partnerPointId = $(this).data('point');
        var modalWindow = $('#idModalToBlockPoint');

        modalWindow.find('#ButBlockPartnerPoint').data('partnerpointid', partnerPointId);
        modalWindow.find('span').text(partnerPointId);
        modalWindow.modal();
        evt.stopPropagation();


    });

    $('.enable-point').on('click', function ()
    {
        var pointId = parseInt($(this).data('point')) || 0;
        var action = $(this).hasClass('disable-point') ? false: true;
        console.log({ "PointId": pointId, "action": action });
        $.post("/Partner/DisableEnable", { "PointId": pointId ,"action":action }, function (data) {
            if (!data.Success)
                toastr.error(data.Message);
            
            else
                  window.location.reload(true); // success
            
        });
        return false;
    }
    );
    // показать / скрыть колонку закупочных цен
    $("form#PointProfile").on('click', '#DontShowZakupPrice', EnableDisableZakupPrice);

    // доступность статуса доп подразделения

    $("form#PointProfile,form#CreatePointProfile,form#CreateOwnProfile").on('click', '#UseDepartmentId', function () {
        var DEF_NUMBER_OF_DAYS = 5;
        var status = $(this).prop("checked");
        $("#DepartmentId").prop("disabled", !status).val(-1);
        $("#DaysToDepartment").prop("disabled", !status);

        if (status)
            $("#DaysToDepartment").val(DEF_NUMBER_OF_DAYS);
        else
            $("#DaysToDepartment").val('');
    });

    $("#sale-direction").find('button').click(function () {
        $(this).toggleClass("active")
    }

        );

    //создание собственной точки (первая регистрация)
    $("form#CreateOwnProfile").on('submit', function (e) {
       // if (!$(this).valid()) return;


        var accObj = { 'zakup': 1, 'base': 2, 'recommend': 3, 'dont_show_price': 4, 'dont_show_producer': 5 };

        var ArrPricingRules = [];
        $('#pricing_rules_disks, #pricing_rules_tyres, #pricing_rules_bat, #pricing_rules_acc').find('tbody tr').each(function (row) {

            var discount = 0;
            if ($(this).data('active_name') == "zakup")
                discount = parseInt($(this).find("input[type=number][name=zakup]").val()) || 0;

            if ($(this).data('active_name') == "base")
                discount = parseInt($(this).find("input[type=number][name=base]").val()) || 0;


            ArrPricingRules[row] = {
                "number": row,
                "ProducerId": parseInt($(this).attr('id')) || 0,
                "PriceType": $(this).data('active_name'),
                "Discount": discount,
                "PType": accObj[$(this).data('active_name')] || 3
            }

        });

        var arrFilteredPricingRules = ArrPricingRules.filter(function (rec) { return (rec.PType != null || rec.ProducerId == 0); });
        // console.log(arrFilteredPricingRules);
        /*if (!$("#LatLng").val()) {
            showAlert(YstLocale.Get("marker"), false);
            return false;
        }*/
        var dataValue = { 'PricingRules': arrFilteredPricingRules,
            'CompanyName': getValueById('CompanyName'),
            'ContactFIO': getValueById('ContactFIO'),
            'DaysToMainDepartment' : parseInt($('#DaysToMainDepartment').val()) || 1,
            'DepartmentId' : parseInt($('#DepartmentId').val()) || -1,
            'DaysToDepartment' :parseInt($('#DaysToDepartment').val()) || 1,
            'Country':getValueById('Country'),
            'Address':getValueById('Address'),
            'PhoneNumber': getValueById('PhoneNumber'),
            'KeyWord': getValueById('KeyWord'),
            'WebSite': getValueById('WebSite'),
            'Email': $('#Email').val(),
            'AddressForDelivery': getValueById('AddressForDelivery'),
            'Language': getValueById('Language'),
            'LatLng' : 1
        };
        var buttons = $("#sale-direction").find("button");

        //dataValue.SaleDirection = buttons.eq(0).hasClass("active") + buttons.eq(1).hasClass("active") * 2 + buttons.eq(2).hasClass("active") * 4 + buttons.eq(3).hasClass("active") * 8;
        dataValue.SaleDirection = $("#IsOpt").is(":checked") + $("#IsRetail").is(":checked") * 2 + $("#IsInetShop").is(":checked") * 4 + $("#IsEndCustomer").is(":checked") * 8;

        if ($("#IsInetShop").is(":checked")) {
            if ($("#WebSite").val().length == 0) {
                showAlert(YstLocale.Get("errsite"), false);
                return false;
            }
        }

        /*if ($('#AddressIsValid').val() == "false") {
            if (dataValue.Country == "7_1") {
                showAlert("Адрес введён некорректно! Не указан дом!", false);
                return false;
            }
        }*/

        if (dataValue.SaleDirection == 0) {
            showAlert(YstLocale.Get("errsaledir"), false);
            return false;
        }

        if (dataValue.DepartmentId != -1) {
            if (dataValue.DaysToMainDepartment == dataValue.DaysToDepartment) {
                showAlert(YstLocale.Get("errdays"), false);
                return false;
            }
        }
        $(this).find('[name="SaleDirection"]').val(dataValue.SaleDirection);
        $(this).find('[name="pricingrules"]').val(JSON.stringify(arrFilteredPricingRules));
       
       // console.log(dataValue);

     //   return false;
       
    });

    //создание дочерней точки
    $("form#CreatePointProfile").on('submit', function (e) {

        if (!$(this).valid()) return;

        var accObj = { 'zakup': 1, 'base': 2, 'recommend': 3, 'dont_show_price': 4, 'dont_show_producer': 5 };

        var ArrPricingRules = [];
        $('#pricing_rules_disks, #pricing_rules_tyres, #pricing_rules_bat, #pricing_rules_acc').find('tbody tr').each(function (row) {

            var discount = 0;
            if ($(this).data('active_name') == "zakup")
                discount = parseInt($(this).find("input[type=number][name=zakup]").val()) || 0;

            if ($(this).data('active_name') == "base")
                discount = parseInt($(this).find("input[type=number][name=base]").val()) || 0;


            ArrPricingRules[row] = {
                "number": row,
                "ProducerId": parseInt($(this).attr('id')) || 0,
                "PriceType": $(this).data('active_name'),
                "Discount": discount,
                "PType": accObj[$(this).data('active_name')] || 3
            }

        });

        var arrFilteredPricingRules = ArrPricingRules.filter(function (rec) { return (rec.PType != null || rec.ProducerId == 0); });
        // console.log(arrFilteredPricingRules);


        var dataValue = { PricingRules: arrFilteredPricingRules};
               
        /* Fill in additional properties  */
        dataValue.contactFIO = $('#ContactFIO').val();
        dataValue.DaysToMainDepartment = parseInt($('#DaysToMainDepartment').val()) || 1;
        dataValue.DepartmentId = parseInt($('#DepartmentId').val()) || -1;
        dataValue.DaysToDepartment = parseInt($('#DaysToDepartment').val()) || 1;
      
        console.log(JSON.stringify(dataValue));

        //$(this).find('[name="pricing_rules"]').val(JSON.stringify(dataValue));
        $(this).find('[name="pricingrules"]').val(JSON.stringify(arrFilteredPricingRules));
        //$(this).submit();

       // return false;
    });

    // партнерская точка: сохраняем ценовые правила 
    
    $("form#PointProfile").on('submit', function (e) {

        e.preventDefault();

        if (!$(this).valid()) return;

        var accObj = { 'zakup': 1, 'base': 2, 'recommend': 3, 'dont_show_price': 4, 'dont_show_producer': 5 };

        var ArrPricingRules = [];
        $('#pricing_rules_disks, #pricing_rules_tyres, #pricing_rules_bat, #pricing_rules_acc').find('tbody tr').each(function (row) {

            var discount = 0;
            if ($(this).data('active_name') == "zakup")
                discount = parseInt($(this).find("input[type=number][name=zakup]").val())||0;

            if ($(this).data('active_name') == "base")
                discount = parseInt($(this).find("input[type=number][name=base]").val())||0;

            
            ArrPricingRules[row] = {
                "number": row,
                "ProducerId": parseInt($(this).attr('id'))||0,
                "PriceType": $(this).data('active_name'),
                "Discount": discount,
                "PType": accObj[$(this).data('active_name')] || 3
            }

        });


       
        

        // var arrFilteredPricingRules = ArrPricingRules.filter(function (rec) { return (rec.PriceType != "recommend"|| rec.ProducerId==0); });

        var arrFilteredPricingRules = ArrPricingRules.filter(function (rec) { return (rec.PType != null || rec.ProducerId == 0); });
       // console.log(arrFilteredPricingRules);

        var pointId = $('#PointId').val()||0;
        if (pointId == 0) return; 

        var dataValue = { PricingRules: arrFilteredPricingRules, PointId: pointId };

        /* Additional properties to ajax */
        dataValue.contactFIO =  $('#ContactFIO').val();
        dataValue.phoneNumber = $('#PhoneNumber').val();
        dataValue.Address = $('#Address').val();
        dataValue.Email = $('#Email').val(); 
        dataValue.DaysToMainDepartment = parseInt($('#DaysToMainDepartment').val())||1;
        dataValue.DepartmentId = parseInt($('#DepartmentId').val())||-1;
        dataValue.DaysToDepartment = parseInt($('#DaysToDepartment').val()) || 1;
        dataValue.ConditionsAreAccepted = $("#ConditionsAreAccepted").is(":checked");
        dataValue.KeyWord = $("#KeyWord").val();

        dataValue.CompanyName = $("#CompanyName").val();
        dataValue.WebSite = $("#WebSite").val();
        dataValue.LatLng = 1;
        dataValue.AddressForDelivery = $("#AddressForDelivery").val();
        dataValue.Country = $('#Country :selected').val();
        dataValue.Language = $('#Language :selected').val();
        
        //  dataValue.SaleDirection = $("#IsOpt").is(":checked") + $("#IsRetail").is(":checked") * 2 + $("#IsInetShop").is(":checked") * 4;
        var buttons = $("#sale-direction").find("button");

        //dataValue.SaleDirection = buttons.eq(0).hasClass("active") + buttons.eq(1).hasClass("active") * 2 + buttons.eq(2).hasClass("active") * 4 + buttons.eq(3).hasClass("active") * 8;
        dataValue.SaleDirection = $("#IsOpt").is(":checked") + $("#IsRetail").is(":checked") * 2 + $("#IsInetShop").is(":checked") * 4 + $("#IsEndCustomer").is(":checked") * 8;
        //{--added 20150701
        dataValue.CustomDutyVal = parseInt($('#custom_duty_val').val()) || 0;
        dataValue.VatVal = parseInt($('#vat_val').val()) || 0;
        //}--added 20150701

        /*if ($("#IsInetShop").is(":checked")) {
            if (!$("#compname").hasClass('invisible') && $("#WebSite").val().length == 0) {
                showAlert(YstLocale.Get("errsite"), false);
                return false;
            }
        }*/

        if (!$("#compname").hasClass('invisible')) {
            /*if (!$("#LatLng").val()) {
                showAlert(YstLocale.Get("marker"), false);
                return false;
            }*/
            if (dataValue.SaleDirection == 0) {
                showAlert(YstLocale.Get("errsaledir"), false);
                return false;
            }
        }

        /*if (!$("#compname").hasClass('invisible') && $('#AddressIsValid').val() == "false") {
            if (dataValue.Country == "7_1") {
                showAlert("Адрес введён некорректно! Не указан дом!", false);
                return false;
            }
        }*/

        if (dataValue.DepartmentId != -1) {
            if (dataValue.DaysToMainDepartment == dataValue.DaysToDepartment) {
                showAlert(YstLocale.Get("errdays"), false);
                return false;
            }
        }

        //dataValue.SaleDirection = $("#IsOpt").is(":checked") + $("#IsRetail").is(":checked") * 2 + $("#IsInetShop").is(":checked") * 4;
        ShowLoading(YstLocale.Get("plwait"));
        $.ajax({
            url: '/Partner/SavePointProfile',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(dataValue),
            contentType: 'application/json; charset=utf-8'
        }).done(function (result) {

            if (!result.Success) {
                $('#loading').remove();
                $(".modal-dialog").addClass("center-modal-notify");
                $('#idModalToNotifyFailure').modal();
               
            }
            else {
                $('#loading').remove();
                $(".modal-dialog").addClass("center-modal-notify");
                $('#idModalToNotifySuccess').modal();
                $('#alert-container').fadeOut();
            }
           
        }).fail(function () {

            {
                $(".modal-dialog").addClass("center-modal-notify");
                $('#idModalToNotifyFailure').modal();
            }
        })
        
    });
    //сообщение об ошибке (направление продаж,ваш сайт)
    function showAlert(text,status)
    {
        var str;
        if (status==false)
            str = "<div class='alert alert-danger alert-error'> <a href='#' class='close' data-dismiss='alert'>&times;</a> <strong>Ошибка!</strong>" + text + "</div>";
        else
        str = "<div class='alert alert-success'> <a href='#' class='close' data-dismiss='alert'>&times;</a> <strong>Успех!</strong> "+text+ "</div>";
        $('#alert-container').empty().append(str);
    }

    // партнерская точка: в таблице цен при установке галок обновляем доступность 

    $('#pricing_rules_disks, #pricing_rules_tyres').find("input[type = checkbox]").on('click', function () {

      //  $(this).prop({ disabled: true });
        var current_tr = $(this).closest('tr');

        var check = current_tr.find("input[type=checkbox]:checked").not(this);
        if (check.length == 0) {
            $(this).prop("checked", true);
            return;
        }
     //   current_tr.find("input[type=checkbox]").not(this).prop("disabled", false);
        current_tr.find("input[type=checkbox]:checked").not(this).prop("checked", false);
      //  console.log($(this).attr("name"));

        var base_checked = current_tr.find("input[type=checkbox][name=base]").prop("checked");

        current_tr.find("input[type=number][name=base]").prop("disabled", !base_checked);

        var zakup_checked = current_tr.find("input[type=checkbox][name=zakup]").prop("checked");
        current_tr.find("input[type=number][name=zakup]").prop("disabled", !zakup_checked);
       
        if ($(this).is(":checked"))
            current_tr.data("active_name", $(this).attr("name"));
        else
            current_tr.data("active_name", "");

    });

   

    });