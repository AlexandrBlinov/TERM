var geocoder;
var map;
var marker;
var markers = [];
var autocomplete;

function initialize() {
    //Определение карты
    if ($('#map_canvas').size() == 0)
        return false;
    var latlng = new google.maps.LatLng(55.761864464318116, 37.62712369306644);
    var options = {
        zoom: 5,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    map = new google.maps.Map(document.getElementById("map_canvas"), options);

    //Определение геокодера
    geocoder = new google.maps.Geocoder();

    marker = new google.maps.Marker({
        map: map,
        draggable: true
    });
    var input = /** @type {HTMLInputElement} */(
      document.getElementById('Address'));


    var autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.bindTo('bounds', map);

    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        var place = autocomplete.getPlace(); //получаем место
        marker.setPosition(place.geometry.location);
        $("input[name=LatLng]").val(place.geometry.location);
        map.setCenter(place.geometry.location);
        map.setZoom(16);
        if (place.address_components[0].types[0] == "street_number") {
            $('#AddressIsValid').val(true);
        } else {
            $('#AddressIsValid').val(false);
        }
    });

    var address = document.getElementById('Address').value;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK && document.getElementById('Address').value) {
            map.setCenter(results[0].geometry.location);
            map.setZoom(16);
            marker.setPosition(results[0].geometry.location);
            if (results[0].address_components[0].types[0] == "street_number") {
                $('#AddressIsValid').val(true);
            } else {
                $('#AddressIsValid').val(false);
            }
        }
    });
    //Добавляем слушателя события обратного геокодирования для маркера при его перемещении  
    google.maps.event.addListener(marker, 'drag', function () {
        geocoder.geocode({ 'latLng': marker.getPosition() }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[0]) {
                    $('#Address').val(results[0].formatted_address);
                    $("input[name=LatLng]").val(marker.getPosition());
                    if (results[0].address_components[0].types[0] == "street_number") {
                        $('#AddressIsValid').val(true);
                    } else {
                        $('#AddressIsValid').val(false);
                    }
                }
            }
        });
    });

    google.maps.event.addListener(map, 'click', function (event) {

        // addMarker(event.latLng);
        marker.setPosition(event.latLng);
        map.setCenter(event.latLng);
        $("input[name=LatLng]").val(event.latLng);
        geocoder.geocode({ 'latLng': event.latLng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[0]) {
                    $('#Address').val(results[0].formatted_address);
                    if (results[0].address_components[0].types[0] == "street_number") {
                        $('#AddressIsValid').val(true);
                    } else {
                        $('#AddressIsValid').val(false);
                    }
                }
            }
        });
    });

    $(document).on("change", "#Country", function () {
        var country = $("#Country :selected").text();
        geocoder.geocode({ 'address': country }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                map.setZoom(5);
                marker.setPosition(results[0].geometry.location);
                $('#Address').val(results[0].formatted_address);
                if (results[0].address_components[0].types[0] == "street_number") {
                    $('#AddressIsValid').val(true);
                } else {
                    $('#AddressIsValid').val(false);
                }
            }
        });
    });
};

$(document).ready(function () {
    $('#SuccPartnerSettings').on('click', function () {
        if($(this).attr("data-terminal")=='True'){
            
            window.location.href = "/Partner";
        } else {
            window.location.href = "/Home/Index";
        }
    })

    $('#checkboxVAT').change(function () {
        var isChecked = $(this).is(':checked');
        if (!isChecked) {
            
            $('#custom_duty_val').val(0);
            $('#vat_val').val(0);
        }
        $('.toggle_visible_onVat').toggleClass('invisible');
    });

    $("input[id='DaysToMainDepartment']").TouchSpin({
        min: 1,
        max: 45,
        initval: 1,
        buttondown_class: "btn btn-link touchspih_settings",
        buttonup_class: "btn btn-link touchspih_settings"
    });

    $("input[id='DaysToDepartment']").TouchSpin({
        min: 1,
        max: 45,
        initval: 1,
        buttondown_class: "btn btn-link touchspih_settings",
        buttonup_class: "btn btn-link touchspih_settings"
    });
 
    $(".count_add_to_cart").TouchSpin({
        min: 1,
        max: 200,
        initval: 1,
        buttondown_class: "btn btn-link touchspih-podbor-settings",
        buttonup_class: "btn btn-link touchspih-podbor-settings"
    });


    $(".count_add_to_seasoncart").each(function () {
        $(this).TouchSpin({
            min: parseInt($(this).attr('min')),
            max: 10000,
            initval: parseInt($(this).attr('min')),
            buttondown_class: "btn btn-link touchspih-podbor-settings",
            buttonup_class: "btn btn-link touchspih-podbor-settings"
        })
    })

    $(".count_change_seasonorder").each(function () {
        $(this).TouchSpin({
            min: 1,
            max: parseInt($(this).attr('max')),
            buttondown_class: "btn btn-link touchspih-podbor-settings",
            buttonup_class: "btn btn-link touchspih-podbor-settings"
        })
    })
    
    initialize();

    $("form#PointProfile").keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    $("form#CreateOwnProfile").keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    
});

$(document).on("change", ".change_in_cart", UpdateToCart);


$(document).ready(function () {
    
    if ($("#Country :selected").text() == "Россия") {
        $('#btn_input_address').removeClass("invisible");
        //$('.phone-number').mask("+7 (~~~) ~~~-~~~~");
        $('.phone-number').mask("+7(000) 000-0000");
        $('.additional_stock').removeClass("invisible");
    } else {
        $('#Address').removeAttr("disabled");
        $('#btn_input_address').addClass("invisible");
        $('.additional_stock').addClass("invisible");
        var $selector = $("#Country :selected");
        if (!$selector.length) return;
        var code_spit = $selector.val().split('_');
        var telephone_code = code_spit[0] || "+";

        // пока заглушка
        //     $('.phone-number').mask("+(" + telephone_code + ")~~~~~?~~~~~~~~~~~~~~~");
        $('.phone-number').mask("+(" + telephone_code + ")00000999999999999999");
        /*   if (telephone_code=="+") {
               $('.phone-number').mask(telephone_code + " ~~~~~?~~~~~~~~~~~~~~~");
           }else{
               $('.phone-number').mask("+" + telephone_code + " ~~~~~?~~~~~~~~~~~~~~~");
           }
           */
    }
});

$(document).on("change", "#Country", function () {
    var code_spit = $("#Country :selected").val().split('_');
    var telephone_code = code_spit[0] || "+";
    $('.phone-number').unmask();

    if ($("#Country :selected").text() == "Россия") {
        $('#btn_input_address').removeClass("invisible");
        //   $('#Address').attr("readonly", "readonly");
        $('.additional_stock').removeClass("invisible");
        $('.phone-number').val("");
        //  $('.phone-number').mask("+7 (~~~) ~~~-~~~~");
        $('.phone-number').mask("+7(000) 000-0000");
    } else {
        $('#btn_input_address').addClass("invisible");
        //  $('#Address').removeAttr("readonly");
        $('#UseDepartmentId').removeAttr("checked");
        $("#DepartmentId [value='-1']").attr("selected", "selected");
        $('.additional_stock').addClass("invisible");
        $('.phone-number').val("");
        $('.phone-number').mask("+(" + telephone_code + ")00000999999999999999");
    }

});

$(document).ready(function () {

    $('#Photo').on('change', function () {

        var data = new FormData();

        var files = $("#Photo").get(0).files;
        var name = $('#PointName').val().toString();

        if (files.length > 0) {
            data.append("UploadedImage", files[0]);
            data.append("UploadedImageName", name);
        }

        var ajaxRequest = $.ajax({
            type: "POST",
            url: "/api/ystapi/uploadfile",
            contentType: false,
            processData: false,
            data: data
        }).done(function (result) {
            result ? toastr.success(YstLocale.Get("sucdownloadfile")) : toastr.error(YstLocale.Get("errdownloadfile"));
            if (result) {
                $("#PhotoComp").attr("src", "/Content/pointphotos/" + name + ".jpg")
            }
        });

    });

});

$(document).on("change", ".count_change", function () {
    var itemCount = $(this).val();
    var maxcount = parseInt($(this).attr('max')) || 0;
    if (itemCount <= 0 || (itemCount > maxcount)) {
        itemCount = maxcount;
        $(this).val(maxcount);
    }
    var tr = $(this).closest('tr');
    var pr_prod = parseInt(tr.find('.pr_prod').text()) || 0;
    var pr_sum = parseInt(tr.find('.pr_sum').text()) || 0;
    var count_all = parseInt($('.count_all').text()) || 0;
    var pr_all = 0;
    var item_all = 0;
    //меняем итоговую сумму по одному продукту
    tr.find('.pr_sum').text(Math.round(pr_prod * itemCount).toFixed(2));
    //получаем и меняем итоговую сумму по всему заказу
    $('.pr_sum').each(function () {
        pr_all = pr_all + parseInt($(this).text());

    });
    $('.pr_all').text(Math.round(pr_all).toFixed(2));
    //получаем и меняем итоговое количество товаров по всему заказу
    $('.count_change').each(function () {
        item_all = item_all + parseInt($(this).val());
    });
    $('.count_all').text(item_all);
});

$(document).on("click", ".one_sales", function () {
    var check = $('.for_main_sales').find('.active').not(this);
    if (check.length == 0) {
        $(this).addClass("active");
    }
});

$(document).on("click", "#priceBaseDisks", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#priceZakupDisks", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#priceRecDisks", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#priceDelDisks", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#producerDelDisks", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_disks tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#priceBaseTyres", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#priceZakupTyres", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#priceRecTyres", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#priceDelTyres", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#producerDelTyres", function () {
    var th_num = $(this).parents('td').index();
    var discount = $(this).parents('td').find('input[type=number]').val();
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function () {
            var checkbox = $(this).find('input[type="checkbox"]').prop('checked', false);
            var input = $(this).find('input[type=number]').prop("disabled", true);
            current_tr.data("active_name", "");
        })
    })
    $('#pricing_rules_tyres tr').each(function () {
        var current_tr = $(this);
        $(this).children('td').each(function (td_num) {
            if (td_num == th_num) {
                var checkbox = $(this).find('input[type="checkbox"]').prop('checked', true);
                var input = $(this).find('input[type=number]').prop("disabled", false).val(discount);
                current_tr.data("active_name", checkbox.attr("name"));
            }
        })
    })
});

$(document).on("click", "#checkboxVAT", function () {
    var checked = $(this).prop("checked");
    if (checked) {
        $('#customDuty').removeClass("invisible");
        $('#customDutyVal').removeClass("invisible");
        $('#vat').removeClass("invisible");
        $('#vatVal').removeClass("invisible");
    } else {
        $('#customDuty').addClass("invisible");
        $('#customDutyVal').addClass("invisible");
        $('#vat').addClass("invisible");
        $('#vatVal').addClass("invisible");
    }
});


$(document).on("click", "#65", function () {
    var checked = $(this).prop("checked");
    if (checked) {
        $('.podbor_replica').removeAttr("hidden");
    } else {
        $('.podbor_replica').attr("hidden", "hidden");
    }

});

$(document).on("click", 'input[name="replica_all_byauto"]', function () {
    var checked = $('#AllReplica').prop("checked");
    if (checked) {
        $('#CarsList').attr("disabled", "disabled");
    } else {
        $('#CarsList').removeAttr("disabled");
    }
});

/*$(document).on("mouseover", ".DelLink", function () {

    $(this).text(YstLocale.Get("clear")).addClass("delete_text");

});

$(document).on("mouseout", ".DelLink", function () {

    $(this).text(YstLocale.Get("added")).removeClass("delete_text");

});*/