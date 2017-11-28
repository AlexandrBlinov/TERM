$(document).ready(function () {
    //при выборе вида товара отображаем нужный шаблон
    $("#tyre").on('click', function () {
        var maxValue = 0;
        $(".tyres").each(function () {
            var i = $(this).data("rowtyre") || 0;
            if (i > maxValue) {
                maxValue = i;
            }
        });
        $('#claim_items').append($('#hidden_tmpl_tyre').html().replace(/\[i]/g, maxValue + 1));
        $('.date').datetimepicker({
            format: 'DD.MM.YYYY', locale: YstLocale.GetLocale("Culture"),
            showTodayButton: true,
            showClear: true
        });
    });
    $("#disk").on('click', function () {
        var maxValue = 0;
        $(".disks").each(function () {
            var i = $(this).data("rowdisk") || 0;
            if (i > maxValue) {
                maxValue = i;
            }
        });
        $('#claim_items').append($('#hidden_tmpl_disk').html().replace(/\[i]/g, maxValue + 1));
        $('.date').datetimepicker({
            format: 'DD.MM.YYYY', locale: YstLocale.GetLocale("Culture"),
            showTodayButton: true,
            showClear: true
        });
    });
    $("#akb").on('click', function () {
        var maxValue = 0;
        $(".akb").each(function () {
            var i = $(this).data("rowakb") || 0;
            if (i > maxValue) {
                maxValue = i;
            }
        });
        $('#claim_items').append($('#hidden_tmpl_akb').html().replace(/\[i]/g, maxValue + 1));
        $('.date').datetimepicker({
            format: 'DD.MM.YYYY', locale: YstLocale.GetLocale("Culture"),
            showTodayButton: true,
            showClear: true
        });
    });
    $("#acc").on('click', function () {
        var maxValue = 0;
        $(".accs").each(function () {
            var i = $(this).data("rowacc") || 0;
            if (i > maxValue) {
                maxValue = i;
            }
        });
        $('#claim_items').append($('#hidden_tmpl_acc').html().replace(/\[i]/g, maxValue + 1));
        $('.date').datetimepicker({
            format: 'DD.MM.YYYY', locale: YstLocale.GetLocale("Culture"),
            showTodayButton: true,
            showClear: true
        });
    });
    //конец
    //отображение информации обавтомобиле для шины...при выборе состояния б/у
    $('body').on('change', '[id ^= "tyres_condition"]', function () {
        var id = $(this).attr('id');
        var item = id.split('_')[2];
        if ($(this).val() == 2) {
            $('.tyres_carInfo_' + item).removeClass("invisible");
        } else {
            $('.tyres_carInfo_' + item).addClass("invisible");
        }
    });
    //конец
    //допуск к заполнению карточки для акб
    $('body').on('change', '.guaranteeDoc', function () {
        var id = $(this).attr('id');
        var item = id.split('_')[2];
        var selected_prop = $('#akb_guaranteeDoc_' + item).val();
        if (selected_prop == 2 && $(this).data("doctype") == 0) {
            $(this).next().removeClass("invisible");
            $(this).next().append("<span class=\"glyphicon glyphicon-exclamation-sign\"></span> Для обработки обращения необходимо предоставить правильно заполненный гарантийный талон");
        } else if (selected_prop != 2 && $(this).data("doctype") == 0) {
            $(this).next().addClass("invisible");
            $(this).next().empty();
        }
        var selected_prop1 = $('#akb_guaranteeDoc1_' + item).val();
        if (selected_prop1 == 1 && $(this).data("doctype") == 1) {
            $(this).next().removeClass("invisible");
            $(this).next().append("<span class=\"glyphicon glyphicon-exclamation-sign\"></span> Обращение не расматривается при наличии следов вскрытия АКБ");
        } else if (selected_prop1 != 1 && $(this).data("doctype") == 1) {
            $(this).next().addClass("invisible");
            $(this).next().empty();
        }
        var selected_prop2 = $('#akb_guaranteeDoc2_' + item).val();
        if (selected_prop2 == 2 && $(this).data("doctype") == 2) {
            $(this).next().removeClass("invisible");
            $(this).next().append("<span class=\"glyphicon glyphicon-exclamation-sign\"></span> Обращение рассматривается при условии сохранения заводской маркировки и голограммы ЯрШинТорг");
        } else if (selected_prop2 != 2 && $(this).data("doctype") == 2) {
            $(this).next().addClass("invisible");
            $(this).next().empty();
        }
        var selected_prop3 = $('#akb_guaranteeDoc3_' + item).val();
        if (selected_prop3 == 1 && $(this).data("doctype") == 3) {
            $(this).next().removeClass("invisible");
            $(this).next().append("<span class=\"glyphicon glyphicon-exclamation-sign\"></span> Обращение не расматривается при наличии следов механического повреждения аккумулятора");
        } else if (selected_prop3 != 1 && $(this).data("doctype") == 3) {
            $(this).next().addClass("invisible");
            $(this).next().empty();
        }
        var selected_prop4 = $('#akb_guaranteeDoc4_' + item).val();
        if (selected_prop4 == 1 && $(this).data("doctype") == 4) {
            $(this).next().removeClass("invisible");
            $(this).next().append("<span class=\"glyphicon glyphicon-exclamation-sign\"></span> Обращение не расматривается если уровень электролита находится ниже отметки min");
        } else if (selected_prop4 != 1 && $(this).data("doctype") == 4) {
            $(this).next().addClass("invisible");
            $(this).next().empty();
        }
        if (selected_prop == 1 && selected_prop1 == 2 && selected_prop2 == 1 && selected_prop3 == 2 && selected_prop4 == 2) {
            $('.akb-doc_fields_' + item).removeClass("invisible");
        } else {
            $('.akb-doc_fields_' + item).addClass("invisible");
        }
    });
    //отображение информации обавтомобиле для акб...при выборе состояния б/у
    $('body').on('change', '[id ^= "akb-condition"]', function () {
        var id = $(this).attr('id');
        var item = id.split('_')[1];
        if ($(this).val() == 2) {
            $('.akb_carInfo_' + item).removeClass("invisible");
        } else {
            $('.akb_carInfo_' + item).addClass("invisible");
        }
    });
    //конец
    //автокомлит для товаров по коду
    $('body').on('click', '.code-autocomplete', function () {
        var $el = $(this);
        $el.autocomplete({
            source: function (request, response) {

                $.ajax({
                    url: "/api/productsapi/getproductsbycode",
                    dataType: "json",
                    data: {
                        term: request.term,
                    },
                    success: function (data) {
                        var array = $.map(data, function (m) {
                            return {
                                label: m.Label,
                                value: m.Value
                            };
                        });

                        response(array);
                    },

                });
            },
            minLength: 6,
            select: function (event, ui) {
                $el.closest('.panel-body').find('.item-product-name').val(ui.item.label);
            }
        });
    });
    //конец
    //проверка и отправка данных
    $('#formpost').on('click', function () {
        var form = $("#createClaim").closest("form");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        if (!$("#createClaim").valid()) return;
        var claimsitems = [];
        var error = false;
        $("#claim_items").children('div').each(function (row) {
            if ($(this).hasClass('tyres')) {
                var numberOf = $(this).data("rowtyre");
                claimsitems[row] = {
                    "DocNumber": $('#tyres-doc_number_' + numberOf).val(),
                    "DocDate": $('#tyres-doc_date_' + numberOf).val(),
                    "ProductId": $('#tyres-code_' + numberOf).val(),
                    "Name": $('#tyres-name_' + numberOf).val(),
                    "Count": $('#tyres-count_' + numberOf).val(),
                    "DateOfManufacture": $('#tyres-date_' + numberOf).val(),
                    "SerialNumber": $('#tyres-serial_' + numberOf).val(),
                    "Defect": $('#tyres_defect_' + numberOf + ' option:selected').val(),
                    "DefectDescription": $('#tyres_defect_' + numberOf + ' option:selected').text(),
                    "DetailedDescriptionDefect": $('#tyres_defect_full_' + numberOf).val(),
                    "Condition": $('#tyres_condition_' + numberOf + ' option:selected').val(),
                    "ConditionDescription": $('#tyres_condition_' + numberOf + ' option:selected').text(),
                    "Auto": $('#tyres_auto_' + numberOf).val(),
                    "TireRunning": $('#tyres_run_' + numberOf).val(),
                    "Pressure": $('#tyres_pressure_' + numberOf + ' option:selected').val(),
                    "Position": $('#tyres_position_' + numberOf).val(),
                    "WheelWidth": $('#tyres_wheel_width_' + numberOf + ' option:selected').val(),
                    "WheelDiametr": $('#tyres_wheel_diametr_' + numberOf + ' option:selected').val(),
                    "ProductType": 1
                }
            }
            if ($(this).hasClass('disks')) {
                var numberOf = $(this).data("rowdisk");
                claimsitems[row] = {
                    "DocNumber": $('#disks-doc_number_' + numberOf).val(),
                    "DocDate": $('#disks-doc_date_' + numberOf).val(),
                    "ProductId": $('#disks-code_' + numberOf).val(),
                    "Name": $('#disks-name_' + numberOf).val(),
                    "Count": $('#disks-count_' + numberOf).val(),
                    "DateOfManufacture": $('#disks-date_' + numberOf).val(),
                    "Defect": $('#disks_defect_' + numberOf + ' option:selected').val(),
                    "DefectDescription": $('#disks_defect_' + numberOf + ' option:selected').text(),
                    "DetailedDescriptionDefect": $('#disks_defect_full_' + numberOf).val(),
                    "Condition": $('#disks_condition_' + numberOf + ' option:selected').val(),
                    "ConditionDescription": $('#disks_condition_' + numberOf + ' option:selected').text(),
                    "ProductType": 2
                }
            }
            if ($(this).hasClass('accs')) {
                var numberOf = $(this).data("rowacc");
                claimsitems[row] = {
                    "DocNumber": $('#accs-doc_number_' + numberOf).val(),
                    "DocDate": $('#accs-doc_date_' + numberOf).val(),
                    "ProductId": $('#accs-code_' + numberOf).val(),
                    "Name": $('#accs-name_' + numberOf).val(),
                    "Count": $('#accs-count_' + numberOf).val(),
                    "DateOfManufacture": $('#accs-date_' + numberOf).val(),
                    "Defect": $('#accs_defect_' + numberOf + ' option:selected').val(),
                    "DefectDescription": $('#accs_defect_' + numberOf + ' option:selected').text(),
                    "DetailedDescriptionDefect": $('#accs_defect_full_' + numberOf).val(),
                    "Condition": $('#accs_condition_' + numberOf + ' option:selected').val(),
                    "ConditionDescription": $('#accs_condition_' + numberOf + ' option:selected').text(),
                    "ProductType": 4
                }
            }
            if ($(this).hasClass('akb')) {
                var numberOf = $(this).data("rowakb");
                if ($('.akb-doc_fields_' + numberOf).hasClass('invisible')) error = true;
                claimsitems[row] = {
                    "DocNumber": $('#akb-doc_number_' + numberOf).val(),
                    "DocDate": $('#akb-doc_date_' + numberOf).val(),
                    "DefectDate": $('#akb-defect_date_' + numberOf).val(),
                    "EndSaleDate": $('#akb-sale_date_' + numberOf).val(),
                    "ProductId": $('#akb-code_' + numberOf).val(),
                    "Name": $('#akb-name_' + numberOf).val(),
                    "Count": $('#akb-count_' + numberOf).val(),
                    "DateOfManufacture": $('#akb-date_' + numberOf).val(),
                    "Defect": 99,
                    "DefectDescription": $('#akb-defect_' + numberOf).val(),
                    "DetailedDescriptionDefect": $('#akb-defect_full_' + numberOf).val(),
                    "Condition": $('#akb-condition_' + numberOf + ' option:selected').val(),
                    "ConditionDescription": $('#akb-condition_' + numberOf + ' option:selected').text(),
                    "Auto": $('#akb-auto_' + numberOf).val(),
                    "AutoYear": $('#akb-auto_year_' + numberOf).val(),
                    "AutoEngine": $('#akb-auto_engine_' + numberOf).val(),
                    "ProductType": 3
                }
            }
        });
        if (error) return;
        var dataValue = {
            ClaimItems: claimsitems,
            ContactFIO: $('#Fio').val(),
            ContactPhone: $('#Phone').val()
        };
        ShowLoading(YstLocale.Get("plwait"));
        $.ajax({
            url: '/Claims/SaveClaimData',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(dataValue),
            contentType: 'application/json; charset=utf-8'
        }).done(function (result) {
            if (result.result) {
                $('#loading').remove();
                $('#loading-background').remove();
                $(".modal-dialog").addClass("center-modal-notify");
                $('#idModalToNotifySuccess').modal();
                $('#alert-container').fadeOut();
            }
            else {
                $('#loading').remove();
                $('#loading-background').remove();
                $(".modal-dialog").addClass("center-modal-notify");
                $('#idModalToNotifyFailure').modal();
                $(".toError").empty();
                $(".toError").append(result.message);
            }

        }).fail(function () {
            $('#loading').remove();
            $('#loading-background').remove();
            $(".modal-dialog").addClass("center-modal-notify");
            $('#idModalToNotifyFailure').modal();
            $(".toError").empty();
            $(".toError").append(result.message);
        })
    });

    $('body').on('click', '#SuccPartnerSettings', function () {
        window.location.assign('/Claims');
    });

    $('body').on('click', '.glyphicon-remove', function () {
        var data = $(this).data("remove");
        $('#' + data).remove();
    });
});