﻿// Добавляем адаптеры для заполнения полей ContactFIO и Phone

var isUserHasDpdContract = !!$("#isUserHasDpdContract").val();
var isAdmin = !!$("#isAdmin").val();
//var isAdmin = false;
var passShowItemsOfSuppliers = false;
var mainDepCodes = ["00005","00112"];

(function() {

    var enabledDates = [],
    today = new Date();
  
    $('.js-datefor-shipment').datetimepicker({
        format: 'DD.MM.YYYY', locale: YstLocale.GetLocale("Culture"),
        allowInputToggle: true,
        showTodayButton: true,
        defaultDate: today,
        minDate: today,
        showClear: false,
        enabledDates: enabledDates

    });


        // событие при клике на input
        /*
    $('#DeliveryDate').click(function (event) {
        event.preventDefault();
        $('.js-datefor-shipment').data("DateTimePicker").show();
    });
    */
    
    /*
    $('.open-datetimepicker').click(function (event) {
        event.preventDefault();
        $('#datetimepicker').click();
    }); */

    // Подключаем iCheck Для radiobutton в корзине
    $('[type=radio],[type=checkbox]').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue'
    });


    if ($("#CityId").val())
        if ($('#fromTerminal').is(':checked')) {
            refreshListOfTerminals(true);
        } else {
            enableSuggestionsOnAddress();
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



})();

/*
/* Устанавливаем дни доставки

*/
function updateDatepicker()
{
    var enabledDates = [];
    var picker = $('.js-datefor-shipment').data("DateTimePicker");
    var $ld = $("#LogistikDepartment").val();
    if (!$ld ||  mainDepCodes.indexOf($ld)<0) return;
        
    var addressId = $('#AddressId').val();
    if (addressId && isAdmin)
    {       
        
        $.get('/orders/GetDatesOfShipment', { "addressId": addressId })
            .done(function (data) {

                var result = data.result;
                for (var i = 0; i < result.length; i++) {

                    var newDate = new Date(result[i].replace(/(\d{2}).(\d{2}).(\d{4})/, "$2/$1/$3"));
                    enabledDates.push(newDate);

                }

                picker.options({ 'enabledDates': enabledDates })
            })
            .fail(function (xhr, status) {
                console.error(xhr.responseText);
            });
                    
    }


}

supplierModule = {
    checkAndGetNamesForSupplierItems: function() {

        var hasSupplierItems = false, namesOfSupplierProducts = [];

        //  $('.cart-items tr').each(function () { hasSupplierItems = !!$(this).data('supplierid') || hasSupplierItems });

        //    var hasSupplierItems = false; var namesOfProducts = [];
        $('.cart-items tr .data-table-a-text').filter(function() { if (!!$(this).closest('tr').data('supplierid')) namesOfSupplierProducts.push($(this).text()) });
        return namesOfSupplierProducts;
    },
    createStringForSupplierItems: function(namesOfSupplierProducts) {
        var $daysEl = $('#daysOfDeliveryFromSupplier');
        var days = $daysEl.length > 0 ? $daysEl.val() : 0;

        var daysToStr=YstLocale.GetDays(days);
        
        var $result = $('<p/>');

        var $list = $('<ul class="list-group"/>');
        for (var i = 0; i <= namesOfSupplierProducts.length-1;i++) {
            $list.append('<li class="list-group-item data-table-a-text">' + namesOfSupplierProducts[i] + '</li>');
        }
        $result.append('<h3> Внимание! Вы заказали товары </h3>').append($list).append(' с поставкой с удаленного склада.  Вы должны поставить этот товар <mark> в отгрузку </mark>, а не резерв. <br/> Срок доставки до Вашего склада составит ' + daysToStr + '.  ');
      
        return $result;
    }
};



$("#shopping-cart-form").on("submit", function(e) {
 
    var dpdCondition = $('#isDelivery').is(':checked') && $('#IsDeliveryByTk').is(':checked');

            
    if (dpdCondition && !checkIfCanOrderDpdDeliveryDependingOnTime()) {
            
            
        $('#modalDpdCheckTime').modal();
        return false;
    } 
            


    if (!$(this).valid()) return false;

    // if (dpdCondition && !@isUserHasDpdContractJs) {
    if (dpdCondition && !isUserHasDpdContract) {
        $('#modalNotification').modal();
        return false;
    } else {

        // notification on ShowItemsOfSuppliers
        if (!passShowItemsOfSuppliers) {
          //  passShowItemsOfSuppliers = true;
            var namesOfSupplierProducts = supplierModule.checkAndGetNamesForSupplierItems();

            // если есть товары в пути то должна быть указана -> на доставку, а не резерв
            if (namesOfSupplierProducts.length && !$('#isDelivery').is(':checked')) {

                var stringOfSupplierProducts = supplierModule.createStringForSupplierItems(namesOfSupplierProducts);
                $('#modalFromSupplier').find('.modal-body p').text('').append(stringOfSupplierProducts);
                $('#modalFromSupplier').modal();
                return false; 
            }
        }

        var strWaitOrder = YstLocale.Get("waitorder");
        ShowLoading(strWaitOrder);

    }
});





// перерасчитываем стоимость доставки dpd
$(document).on("shoppingcart.changed",
    function() {
        // if (@isAdminJs && $('#CityId') && $('#CityId').length > 0) getCostOfDelivery();
        if (isAdmin && $('#CityId') && $('#CityId').length > 0) getCostOfDelivery();
    });

   
//
// очистка полей при отказе от доставки
//
function refuseFromDelivery() {
    var $transportContainer = $('.dpd-calc-container');
    $transportContainer.find('input[type=text],input[type=hidden]').each(function() { $(this).val(''); });
    $('#cost-of-delivery').text('');
    $transportContainer.hide();
    //  debugger;

    setValidateOnClient($('#City'), false);
    setValidateOnClient($('#Address'), false);
}

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
            refuseFromDelivery();
            break;
        }
        case 'isTransportCompany':
            {
             
                $address_container.hide();
                $tk_container.show('slow');
                $sdk.hide();
                refuseFromDelivery();
                break;

            }
        default: // isDeliveryByOwnTransport
            {
                $address_container.show('slow');
                $tk_container.hide('slow');
                $sdk.hide('slow');
                refuseFromDelivery();
            }
} });

// пока убираем
$('#AddressId').change(function () { updateDatepicker() });

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
        if (isUserHasDpdContract)   $deliveryByTk_container.removeClass('hidden');
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
$('[name=TerminalOrAddress]').on('ifChecked', function(event) {
    if (event.target.id === "fromTerminal") refreshListOfTerminals();

    else refreshAllDpdElements();

});



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

function getCostOfDelivery() {
    var daysSuffix = 'дн.';
    var rouble = '\u20BD';
    var orderGuid = $('#OrderGuidIn1S').val() || null;
    var apicallPath = "/api/dpdapi/GetCostAndTimeOfDelivery";

    //if (!$('#IsDeliveryByTk').is(':checked')) return;
    if (!$('#isDpdDelivery').is(':checked')) return;

    $.getJSON(apicallPath, { 'cityId': $('#CityId').val(), terminalOrAddress: $('#fromTerminal').is(':checked'), orderGuid: orderGuid })
        .done(function(data) {

            var cost = data.Cost;
            var deliveryDiapazon = writeDiapazon(data.DeliveryTimeMin, data.DeliveryTimeMax);

            if (deliveryDiapazon.length > 0) {

                $('#delivery-time').text(deliveryDiapazon + ' ' + daysSuffix);
                $('.cartitem-deliverydays').each(function() { $(this).text(deliveryDiapazon + ' ' + daysSuffix) });
            }

            if (Number.parseInt(cost) && cost > 0) {
                $('#cost-of-delivery').text(cost + rouble);

                console.log(data);
            }

        }).error(function() { console.error('cant calculate cost of delivery') });

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


function refreshListOfTerminals(clearaddress) {
    $.getJSON("/api/dpdapi/getterminalsbycity", { 'kladr': $('#CityId').val() }).done(function(data) {

        refreshAllDpdElements(clearaddress);
        loadDataToTerminalsDpd(data);
        var selectedOption = $('#TerminalsDpd').find('option:selected');
        var schedule = selectedOption.data('schedule');
        if (schedule && schedule.length) $('#term-schedule').text(schedule);
    }
    );
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

function enableSuggestionsOnAddress() {
    $("#Address").suggestions({
        serviceUrl: "https://suggestions.dadata.ru/suggestions/api/4_1/rs",
        token: "ee579981e9c0a8e389f8289f052f7c81fa83fada",
        type: "ADDRESS",
        count: 10,
        bounds: "street-house",
        constraints: { locations: { kladr_id: $('#CityId').val() } },

        onSelect: function(suggestion) {
            setAdditionalFieldsForDelivery(suggestion);

        }
    });
}


document.getElementById('City').oninput = function() { setValidateOnClient($(this), false); }

document.getElementById('Address').oninput = function() { setValidateOnClient($(this), false); }


$('#City').autocomplete({
    create: function() {
        // отрисовка - при вводе строки поиска выделение цветом и жирным шрифтом
        $(this).data('ui-autocomplete')._renderItem = function(ul, item) {

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
    source: function(request, response) {
        $.ajax({
            url: "/api/dpdapi/get",
            dataType: "json",
            data: {
                term: request.term,
            },
            success: function(data) {
                var array = $.map(data, function(m) {
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
    select: function(event, ui) {


        setValidateOnClient($(this), true);
        $('#CityId').val(ui.item.id);
        $('#RegionId').val(ui.item.RegionId);

        //$(this).insertAfter('<span class="glyphicon glyphicon-ok form-control-feedback"></span>);

        $('.cityDependingData').show();
        refreshListOfTerminals();

        enableSuggestionsOnAddress();

    },
    minLength: 3,
    delay: 200
});

/*
$('#modalFromSupplier').on('hidden.bs.modal', function() {
    $("#shopping-cart-form").submit();
}); */

/*
$('#modalFromSupplier .modal-footer button').on('click', function (e) {
    var $target = $(e.target);
    // если Да, то сабмитим форму
    if ($target.val() == 1) 
        {
            passShowItemsOfSuppliers = true;
            $("#shopping-cart-form").submit();
        }

});
*/

 /*
$('#modalConfirmation .modal-footer button').on('click', function(e) {
    var $target = $(e.target), // Clicked button element
        $deliveryByTk_container = $('#deliveryByTk_container');
    // если Да
    if ($target.val() == 1) {

        //if (@isAdminJs) $deliveryByTk_container.removeClass('hidden');
        if (isAdmin) $deliveryByTk_container.removeClass('hidden');
        $('[name=IsDeliveryByTk]').iCheck('check');

        //if (!@isUserHasDpdContractJs) $('#modalNotification').modal();
        if (!isUserHasDpdContract) $('#modalNotification').modal();
    } else
        $('[name=IsDeliveryByTk]').iCheck('uncheck');

});
*/