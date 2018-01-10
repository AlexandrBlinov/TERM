// Добавляем адаптеры для заполнения полей ContactFIO и Phone

//var isUserHasDpdContract = !!$("#isUserHasDpdContract").val();
var isAdmin = !!$("#isAdmin").val();
//var isAdmin = false;
var passShowItemsOfSuppliers = false;
var passForDelivery = false;



/*
* Работа с товарами сторонних поставщиков
* + логика работы с товарами
*/
var supplierModule = (function () {

    // основное подразделение
    var mainLogistikDepartment = "00005",
        daysToDepartments = {
            "00112": 1, // СПб
            "00106": 3, // РД
            "00138": 7 // Екат
        };
    var selectorItems = '.cart-items  .js-cart-item';

    var $ld = $("#LogistikDepartment").val();

    var firstSupplierId = function () {
        var $res = $(selectorItems).filter(function () {
            var supplierId = $(this).data('supplierid');
            if (!!supplierId && supplierId != 0) return true;
        });

        if ($res.length) return $res.data('supplierid');
        return 0;
    };

    return {

        getfirstSupplierId: firstSupplierId,

        getLogistikDepartment: function () {
            return $ld;
        },
        checkAndGetNamesForSupplierItems: function () {

            var hasSupplierItems = false, namesOfSupplierProducts = [];

            $('.cart-items .data-table-a-text').filter(
                function () {
                    if (!!$(this).closest('.js-cart-item').data('supplierid'))
                        namesOfSupplierProducts.push($(this).text())
                });
            return namesOfSupplierProducts;
        },

        /*
        * проверить что есть хотя бы 1 товар стороннего поставщика
        */
        checkIfThereisSupplier: function () {
            var $result = $(selectorItems).filter(function () {
                var supplierId = $(this).data('supplierid');
                if (!!supplierId && supplierId != 0) return true;
            });
            return $result.length > 0;
        },

        /*
        * Получает Id Первого поставщика
        */
       

        checkIfThereisNotMainDepartment: function () {
            var $result = $(selectorItems).filter(function () {
                var departmentId = $(this).data('department-id');
                if (!!departmentId && departmentId != 5) return true;
            });
            return $result.length > 0;
        },

        /*
        * Товары со скольки департаментов в корзине
        */
        getNumberOfDepartments: function () {
          
            var arrayOfDepartments = [];
            $(selectorItems).filter(function () {
                var departmentId = $(this).data('department-id');
                if (!!departmentId && departmentId != 0 && arrayOfDepartments.indexOf(departmentId) < 0)
                    arrayOfDepartments.push(departmentId);
            });
            return arrayOfDepartments.length;

        },

        isMainLogistikDepartment: function () {
            return $ld === mainLogistikDepartment;
        },

        /*
        * выбрано в отгрузку а не резерв
        */
        isDelivery: function ()
        {
            return $('#isDelivery').is(':checked');
        },

        /*
        * Если клиент филиала и везем товар из Ярославля - считать число дней
        */
        getNumberOfDaysForDepartment: function () {

            var daysToDepartment = daysToDepartments[$ld];
            if (!daysToDepartment) throw "Unknown logistik department " + supplierModule.getLogistikDepartment();
            debugger;
            return daysToDepartments;
        },



        getNumberOfDaysFromSupplier: function (supplierId) {

            $.getJSON('/api/RestsOfSuppliersApi/GetSuppliers').done(function (data) {

                var $item = data.filter(function (item) { return item.Id == supplierId });
             //   console.log($item[0]);
             //   debugger;
                if ($item.length)
                    $('#daysOfDeliveryFromSupplierToMainDep').text($item[0].Days);
            });

        },


        createStringForSupplierItems: function (namesOfSupplierProducts) {
            var $daysEl = $('#daysOfDeliveryFromSupplier');
            var days = $daysEl.length > 0 ? $daysEl.val() : 0;

            var daysToStr = YstLocale.GetDays(days);

            var $result = $('<p/>');

            var $list = $('<ul class="list-group"/>');
            for (var i = 0; i <= namesOfSupplierProducts.length - 1; i++) {
                $list.append('<li class="list-group-item data-table-a-text">' + namesOfSupplierProducts[i] + '</li>');
            }
            $result.append('<h3> Внимание! Вы заказали товары </h3>').append($list).append(' с поставкой с удаленного склада.  Вы должны поставить этот товар <mark> в отгрузку </mark>, а не резерв. <br/> Срок доставки до Вашего склада составит ' + daysToStr + '.  ');

            return $result;
        }
    }
}());

(function() {

    var enabledDates = [],
    today = new Date();
  
    $('.js-datefor-shipment').datetimepicker({
        format: 'DD.MM.YYYY',
        locale: YstLocale.GetLocale("Culture"),
        allowInputToggle: true,
        showTodayButton: true,
        defaultDate: today,
        minDate: today,
        showClear: false,
        enabledDates: enabledDates

    });

    

    // Подключаем iCheck Для radiobutton в корзине
    $('[type=radio],[type=checkbox]').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue'
    });


    if ($("#CityId").val())
        if ($('#fromTerminal').is(':checked')) {
            dpdModule.refreshListOfTerminals(true);
        } else {
            dpdModule.enableSuggestionsOnAddress();
        }

    var validationRule = 'requiredifnotcheched';
    $.validator.addMethod(validationRule, function(value, element, params) {
        if ($(element).val()) return true;
        var $targetElement = $('#' + params.id);
        return !$targetElement.is(":checked");

    });


    jQuery.validator.unobtrusive.adapters.add(validationRule, ['id'], function(options) {
        options.rules[validationRule] = { id: options.params.id }
        options.messages[validationRule] = options.message;
    });

    $('#PhoneNumberOfClient').mask('9'.repeat(11));

    
    $('#AddressId').change(function () { updateDatepicker() });

    var city = document.getElementById('City');
    if (city) city.oninput = function () { setValidateOnClient($(this), false); }
    var address = document.getElementById('Address');
    if (address)    address.oninput = function () { setValidateOnClient($(this), false); }
    

})();

/*
/* Устанавливаем дни доставки

*/
function updateDatepicker()
{
    var workingDepartmentCodes = ["00005", "00112", "00106"];
    var enabledDates = [];
    var picker = $('.js-datefor-shipment').data("DateTimePicker");
    var $ld = $("#LogistikDepartment").val();
    if (!$ld || workingDepartmentCodes.indexOf($ld)<0) return;
        
    var addressId = $('#AddressId').val();
    if (addressId && isAdmin)
    {   
        $.get('/orders/GetDatesOfShipment', { "addressId": addressId })
            .done(function (data) {

                var result = data.result;
                for (var i = 0; i < result.length; i++) {

                    var newDate = dateUtils.dateFromString(result[i]); 
                    enabledDates.push(newDate);
                 }

                
                picker.options({ 'enabledDates': enabledDates });
                if (Array.isArray(enabledDates) && enabledDates.length > 0) {
                    picker.date(enabledDates[0]);
                 //   console.log(enabledDates[0]);
                }
            })
            .fail(function (xhr, status) {
                console.error(xhr.responseText);
            });
                    
    }


}





$("#shopping-cart-form").on("submit", function(e) {

    var dpdCondition = supplierModule.isDelivery() && $('#IsDeliveryByTk').is(':checked');

            
    if (dpdCondition && !dpdModule.checkIfCanOrderDpdDeliveryDependingOnTime()) {            
            
        $('#modalDpdCheckTime').modal();
        return false;
    } 
            


    if (!$(this).valid()) return false;

    debugger;
    // if (dpdCondition && !@isUserHasDpdContractJs) {
    if (dpdCondition && ! dpdModule.isUserHasDpdContract()) {
        $('#modalNotification').modal();
        return false;
    } else {

        // notification on ShowItemsOfSuppliers
        if (!passShowItemsOfSuppliers) {
          //  passShowItemsOfSuppliers = true;
            var namesOfSupplierProducts = supplierModule.checkAndGetNamesForSupplierItems();

            // если есть товары в пути то должна быть указана -> на доставку, а не резерв
            if (namesOfSupplierProducts.length && !supplierModule.isDelivery()) {

                var stringOfSupplierProducts = supplierModule.createStringForSupplierItems(namesOfSupplierProducts);
                $('#modalFromSupplier').find('.modal-body p').text('').append(stringOfSupplierProducts);
                $('#modalFromSupplier').modal();
                return false; 
            }
        }
        // если выбрана доставка
      
        if (supplierModule.isDelivery() && !passForDelivery) {
            // логистическое направление - Ярославль
            if (supplierModule.isMainLogistikDepartment()) {

                // + нет сторонних поставщиков
                if (!supplierModule.checkIfThereisSupplier()) {
                    // do nothing
                  //  console.log('case1');
                    passForDelivery = true;
                } else {
                    // + есть сторонние поставщики
                 //   console.log('case2');
                    // если уже стоит галка то повторно не спрашиваем
                    if (!$('input[name=optionSupplier]:checked').val()) {
                        var firstSid = supplierModule.getfirstSupplierId();
                        supplierModule.getNumberOfDaysFromSupplier(firstSid);
                        $('#modalChooseOptionWithSupplier').modal(
                            {
                                backdrop: 'static',
                                keyboard: true
                            }
                        );
                    }
                }
            } else
            // логистическое направление - филиал
            {
                // если товары с одного подразделения
                if (supplierModule.getNumberOfDepartments() == 1)

                    // отгрузка со склада филиала (согласно графику филиала)
                    if (supplierModule.checkIfThereisNotMainDepartment()) {
                        // do nothing
                        
                        passForDelivery = true;
                     //   console.log('case3');
                    }
                    // только Ярославль
                    else {
                        // Число дней из Ярославля в подразделение    

                        try {
                            var daystoDepartment = supplierModule.getNumberOfDaysForDepartment();
                        } catch (e)
                        {
                            console.error(e);
                            alert(e);
                            return false;
                        }
                      //  if (daystoDepartment > 0)
                        {

                            var deliveryDateStr = $("#DeliveryDate").val();

                            var date1 = dateUtils.dateFromString(deliveryDateStr);
                            var date2 = dateUtils.addDays(date1, daystoDepartment);

                            $("#DeliveryDate2").val(dateUtils.getFormattedDate(date2));
                            $("#CaseForLogistik").val("41");

                            passForDelivery = true;
                            //    console.log('case4');
                        } 
                    }
                // товары  c 2-х подразделений
                else {
                    //var daystoDepartment = supplierModule.getNumberOfDaysForDepartment();
                    try {
                        var daystoDepartment = supplierModule.getNumberOfDaysForDepartment();
                    } catch (e) {
                        console.error(e);
                        alert(e);
                        return false;
                    }
                   // if (daystoDepartment > 0)
                    {
                        $('#daysOfDeliveryFromDepartmentToMainDep').text(daystoDepartment);


                        $('#modalChooseOptionTwoDepartments').modal(
                            {
                                backdrop: 'static',
                                keyboard: true
                            }
                        );

                    }
                }
                
            }
         
            if (!passForDelivery) return false;
        }
       
       
       
        var strWaitOrder = YstLocale.Get("waitorder");
        ShowLoading(strWaitOrder);

    }
});




///
// выбор способа доставки
///

$('[name=WayOfDelivery]').on('ifChecked', function (event) {
    var $address_container = $('.address-container'),
        $tk_container = $('.tk-container'),
        $sdk = $('.selfDelivery-container'),
        $dcc = $('.dpd-calc-container');



    switch (event.target.id) {
        case 'isDpdDelivery': {
            
            $address_container.hide('slow');
            $tk_container.hide('slow');
            $sdk.hide('slow');
            $dcc.show();
            break;
        }
        case 'isSelfDelivery': {
            $address_container.hide('slow');
            $tk_container.hide('slow');
            $sdk.show('slow');
            dpdModule.refuseFromDelivery();
            break;
        }
        case 'isTransportCompany':
            {
             
                $address_container.hide();
                $tk_container.show('slow');
                $sdk.hide();
                dpdModule.refuseFromDelivery();
                break;

            }
        default: // isDeliveryByOwnTransport
            {
                $address_container.show('slow');
                $tk_container.hide('slow');
                $sdk.hide('slow');
                dpdModule.refuseFromDelivery();
            }
} });



///
// выбор на отгрузку или в резерв
///
$('[name=IsDelivery]').on('ifChecked', function(event) {

    
    setValidateOnClient($('#Address'), false);
    var $deliveryDate = $("#DeliveryDate"),
        $deliverydate_container = $("#deliverydate_container"),
        $deliveryByTk_container = $('#deliveryByTk_container');

    var $address_container = $('.address-container');
    if (event.target.id === "isDelivery") {
        $deliverydate_container.removeClass('hidden');
// пока убираем
  updateDatepicker();
        if (dpdModule.isUserHasDpdContract())   $deliveryByTk_container.removeClass('hidden');
        $address_container.show('slow');
        $('.js-wayofdelivery').show('slow');
      ///// вариант с всплавающим окном
      /////  if (isAdmin) $('#modalConfirmation').modal();
      
    }
        // в резерв
    else {

        $deliverydate_container.addClass('hidden');
        $deliveryByTk_container.addClass('hidden');
        $address_container.hide();
        $deliveryDate.val('');
        $('#IsDeliveryByTk').iCheck('uncheck');
        $('.js-wayofdelivery').hide('slow');

    }

});



////////////////////
// Блок  Dpd
///////////////////


var dpdModule = (function () {

    var isUserHasDpdContract;
    function isUserHasDpdContract() {
        if (isUserHasDpdContract === undefined)  isUserHasDpdContract = !!$("#isUserHasDpdContract").val();
        return isUserHasDpdContract;
    }
    //
    // очистка полей при отказе от доставки
    //
    function refuseFromDelivery() {
        var $transportContainer = $('.dpd-calc-container');
        $transportContainer.find('input[type=text],input[type=hidden]').each(function () { $(this).val(''); });
        $('#cost-of-delivery').text('');
        $transportContainer.hide();
        //  debugger;

        setValidateOnClient($('#City'), false);
        setValidateOnClient($('#Address'), false);
    }


    function setAdditionalFieldsForDelivery(suggestion) {

        if (suggestion && suggestion.data) {
            //  debugger;
            $('#PostalCode').val(suggestion.data['postal_code'] || '');
            $('#StreetType').val(suggestion.data['street_type'] || '');
            $('#House').val(suggestion.data['house'] || '');
            $('#Street').val(suggestion.data['street'] || '');
            $('#BlockType').val(suggestion.data['block_type'] || '');


        }
        if ($('#PostalCode').val() && $('#StreetType').val() && $('#House').val() && $('#Street').val()) setValidateOnClient($('#Address'), true);


    }

    function loadDataToTerminalsDpd(termData) {

        for (var i = 0; i < termData.length; i++) {

            $('#TerminalsDpd').append($("<option/>", {
                value: termData[i].Code,
                text: termData[i].Address,
                data: { schedule: termData[i].Schedule }
            }));
        }

    }

    function refreshListOfTerminals(clearaddress) {
        $.getJSON("/api/dpdapi/getterminalsbycity", { 'kladr': $('#CityId').val() }).done(function (data) {

            refreshAllDpdElements(clearaddress);
            loadDataToTerminalsDpd(data);
            var selectedOption = $('#TerminalsDpd').find('option:selected');
            var schedule = selectedOption.data('schedule');
            if (schedule && schedule.length) $('#term-schedule').text(schedule);
        }
        );
    }

    //
    // обновляет отображение для dpd
    //
    function refreshAllDpdElements(clearaddress) {
        //      debugger;
        $('#TerminalsDpd').empty();
        $('#Address').val('');
        $('#cost-of-delivery').text('');
        //   setAdditionalFieldsForDelivery({});
        if ($('#fromTerminal').is(':checked')) {
            $('.dpd-address').hide();
            /*  if (clearaddress) */
            $('.dpd-terminals').show();

        } else {
            $('.dpd-address').show();
            $('.dpd-terminals').hide();

        }
        getCostOfDelivery();
    }

    /*
    * Получить стоимость доставки
    */
    function getCostOfDelivery() {
        var daysSuffix = 'дн.';
        var rouble = '\u20BD';
        var orderGuid = $('#OrderGuidIn1S').val() || null;
        var apicallPath = "/api/dpdapi/GetCostAndTimeOfDelivery";

        //if (!$('#IsDeliveryByTk').is(':checked')) return;
        if (!$('#isDpdDelivery').is(':checked')) return;

        $.getJSON(apicallPath, { 'cityId': $('#CityId').val(), terminalOrAddress: $('#fromTerminal').is(':checked'), orderGuid: orderGuid })
            .done(function (data) {

                var cost = data.Cost;
                var deliveryDiapazon = writeDiapazon(data.DeliveryTimeMin, data.DeliveryTimeMax);

                if (deliveryDiapazon.length > 0) {

                    $('#delivery-time').text(deliveryDiapazon + ' ' + daysSuffix);
                    $('.cartitem-deliverydays').each(function () { $(this).text(deliveryDiapazon + ' ' + daysSuffix) });
                }

                if (Number.parseInt(cost) && cost > 0) {
                    $('#cost-of-delivery').text(cost + rouble);

                    console.log(data);
                }

            }).error(function () { console.error('cant calculate cost of delivery') });

    }


    function enableSuggestionsOnAddress() {
        $("#Address").suggestions({
            serviceUrl: "https://suggestions.dadata.ru/suggestions/api/4_1/rs",
            token: "ee579981e9c0a8e389f8289f052f7c81fa83fada",
            type: "ADDRESS",
            count: 10,
            bounds: "street-house",
            constraints: { locations: { kladr_id: $('#CityId').val() } },

            onSelect: function (suggestion) {
                setAdditionalFieldsForDelivery(suggestion);

            }
        });
    }


    function checkIfCanOrderDpdDeliveryDependingOnTime() {
        try {
            var timeText = $.ajax({ type: 'HEAD', url: window.location.href.toString(), async: false }).getResponseHeader('Date');
            var dateText = $('#DeliveryDate').val();
            if (dateText.length > 0) {

                var date = new Date(dateText.replace(/(\d{2}).(\d{2}).(\d{4})/, "$2/$1/$3"));

                var time = new Date(timeText);
                if ((time.getHours() >= 14 && time.getDate() == date.getDate()) || date.getDay() == 6 || date.getDay() == 0) return false;
            }
        } catch (ex) {
            return true;
        }
        return true;
    }

    $('[name=TerminalOrAddress]').on('ifChecked', function (event) {
        if (event.target.id === "fromTerminal") refreshListOfTerminals();

        else refreshAllDpdElements();

    });

    // перерасчитываем стоимость доставки dpd
    $(document).on("shoppingcart.changed",
        function () {

            if (isAdmin && $('#CityId') && $('#CityId').length > 0) getCostOfDelivery();
        });


    $('#City').autocomplete({
        create: function () {
            // отрисовка - при вводе строки поиска выделение цветом и жирным шрифтом
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {

                var cleanTerm = this.term.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');

                // Build pipe separated string of terms to highlight
                var keywords = $.trim(cleanTerm).replace('  ', ' ').split(' ').join('|');

                // Get the new label text to use with matched terms wrapped
                // in a span tag with a class to do the highlighting
                var re = new RegExp(" (" + keywords + ")", "gi");
                var output = item.label.replace(re,
                    '<span class="ui-menu-item-highlight">$1</span>');


                return $("<li>")
                    .append($("<a>").html(output))
                    .appendTo(ul);

            };
        },
        // определяет, откуда берутся данные
        source: function (request, response) {
            $.ajax({
                url: "/api/dpdapi/get",
                dataType: "json",
                data: {
                    term: request.term,
                },
                success: function (data) {
                    var array = $.map(data, function (m) {
                        return {
                            label: m.Abbreviation + ". " + m.Name + ", " + m.RegionName,
                            value: m.Abbreviation + ". " + m.Name,
                            id: m.Id,
                            dpdCode: m.DpdCode,
                            RegionId: m.RegionId
                        };
                    });

                    response(array);
                },

            });
        },
        // при выборе города
        select: function (event, ui) {


            setValidateOnClient($(this), true);
            $('#CityId').val(ui.item.id);
            $('#RegionId').val(ui.item.RegionId);

            $('.cityDependingData').show();
            refreshListOfTerminals();

            enableSuggestionsOnAddress();

        },
        minLength: 3,
        delay: 200
    });

    return {
        'refreshListOfTerminals': refreshListOfTerminals,
        'enableSuggestionsOnAddress': enableSuggestionsOnAddress,
        'checkIfCanOrderDpdDeliveryDependingOnTime': checkIfCanOrderDpdDeliveryDependingOnTime,
        'refuseFromDelivery': refuseFromDelivery,
        'isUserHasDpdContract': isUserHasDpdContract
        
    }

}());



function setValidateOnClient($element, flag) {

    //var $element = $(selector);
    if (flag) {
        $element.after('<span class="glyphicon glyphicon-ok form-control-feedback"></span>');
        $element.closest('.form-group').addClass('has-success has-feedback');
    } else {
        $element.next('.glyphicon-ok').remove();
        $element.closest('.form-group').removeClass('has-success has-feedback');
    }

}







/*
* Обработка выбора частичная отгрузка или после полного прихода товаров
*/

$('#modalChooseOptionWithSupplier .modal-footer button').on('click', function (e) {

    if (!$('[name=optionSupplier]').is(':checked')) return false;

    $('#modalChooseOptionWithSupplier').modal('hide');

    var result = $('#isPartialDeliveryForSupplier').is(":checked");
        
        var deliveryDateStr = $("#DeliveryDate").val();
        var daysToAddStr = $('#daysOfDeliveryFromSupplierToMainDep').text();

        var daysToAdd = parseInt(daysToAddStr) || 0;
       //debugger;
       if (daysToAdd>0) {
          
            var date1 = dateUtils.dateFromString(deliveryDateStr);
            var date2 = dateUtils.addDays(date1, daysToAdd);

            $("#DeliveryDate2").val(dateUtils.getFormattedDate(date2));

            $("#CaseForLogistik").val('2' + (result ? "1" : "2"));
            console.log($("#CaseForLogistik").val());
            passForDelivery = true;
            $("#shopping-cart-form").submit();
        }
      
});

/*
* Обработка выбора частичная отгрузка или после полного прихода товаров
*/
$('#modalChooseOptionTwoDepartments .modal-footer button').on('click', function (e) {

    if (!$('[name=optionDepartment]').is(':checked')) return false;

    $('#modalChooseOptionTwoDepartments').modal('hide');

    var result = $('#isPartialDeliveryForDepartment').is(":checked");

    var deliveryDateStr = $("#DeliveryDate").val();
    var daysToAddStr = $('#daysOfDeliveryFromDepartmentToMainDep').text();
    var daysToAdd = parseInt(daysToAddStr) || 0;

    //debugger;
    if (daysToAdd>0) {

        var date1 = dateUtils.dateFromString(deliveryDateStr);
        var date2 = dateUtils.addDays(date1, parseInt(daysToAdd) || 0);
        
        $("#DeliveryDate2").val(dateUtils.getFormattedDate(date2));

        $("#CaseForLogistik").val('5' + (result ? "1" : "2"));
        
     //   console.log($("#CaseForLogistik").val());
        passForDelivery = true;
        $("#shopping-cart-form").submit();
    }

});

