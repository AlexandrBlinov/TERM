(function () {
    /*
    $.extend({
        form: function (url, data, method) {
            if (method == null) method = 'POST';
            if (data == null) data = {};

            var form = $("<form enctype='application/json'>").attr({
                method: method,
                action: url
            }).css({
                display: 'none'
            });

            var addData = function (name, data) {
                if ($.isArray(data)) {
                    for (var i = 0; i < data.length; i++) {
                        var value = data[i];
                        addData(name + '[]', value);
                    }
                } else if (typeof data === 'object') {
                    for (var key in data) {
                        if (data.hasOwnProperty(key)) {
                            addData(name + '[' + key + ']', data[key]);
                        }
                    }
                } else if (data != null) {
                    form.append($('<input>').attr({
                        type: 'hidden',
                        name: String(name),
                        value: String(data)
                    }));
                }
            };

            for (var key in data) {
                if (data.hasOwnProperty(key)) {
                    addData(key, data[key]);
                }
            }

            return form.appendTo('body');
        }
    });
    */

    String.format = function () {
        // The string containing the format items (e.g. "{0}")
        // will and always has to be the first argument.
        var theString = arguments[0];

        // start with the second argument (i = 1)
        for (var i = 1; i < arguments.length; i++) {
            // "gm" = RegEx options for Global search (more than one instance)
            // and for Multiline search
            var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
            theString = theString.replace(regEx, arguments[i]);
        }

        return theString;
    }


    function escapeHtml(text) {
        var map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };

        return text.replace(/[&<>"']/g, function (m) { return map[m]; });
    }

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "1500",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    setInterval(function () { $.get("/Home/Index", "", function () { }) }, 3600000);

}());


function AddToCart(productId, departmentId,supplierId) {

    var count = $('#order-' + productId).val();
    supplierId = supplierId || 0;

    if ($.isNumeric(productId) && productId > 0 && $.isNumeric(count)) {
        

        $.post("/ShoppingCart/AddToCart",
            {
                "id": productId,
                "departmentId": departmentId,
                "supplierId": supplierId,
                "count": count
            },
        function (data) {

            if (data.Success) {


                $("#cart-status-count").text(data.CartCount);
                
                $('#cart-total').text(data.CartTotal);
                toastr.success(data.Message);
                $('.AddLink[data-id = ' + productId + ']').addClass('invisible');
                $('.DelLink[data-id = ' + productId + ']').removeClass('invisible');
                $('#ProductName-' + productId).removeClass('a_text').addClass('add_text');
                $('#cart-weight').text(data.TotalWeight);
                $.event.trigger('shoppingcart.changed');
            } else
                toastr.error(data.Message);
        });
    }
}


function UpdateToCart() {
    var itemCount = $(this).val();
    var tr = $(this).closest('tr');
    var productId = tr.data("id");


    if (!$.isNumeric(itemCount) || itemCount > 200 || itemCount < 1) {

        itemCount = 4;
        $(this).val(itemCount);

    }

    $.post("/ShoppingCart/UpdateToCart", { "id": productId, "count": itemCount },
        function (data) {

            $("#cart-status-count").text(data.CartCount);
            $("#cart-count,.cart-count").text(data.CartCount);
            $("#cart-total").text(data.CartTotal);
            $("#cart-total-OfClient").text(data.CartTotalOfClient);
            var price = parseInt(tr.find('.item-price').text()) || 0;
            // console.log(price);
            tr.find('.item-sum').text(Math.round(price * itemCount).toFixed(2));
            var priceOfClient = parseInt(tr.find('.item-priceOfClient').text()) || 0;
            tr.find('.item-sumOfClient').text(Math.round(priceOfClient * itemCount).toFixed(2));
            $('#cart-weight').text(data.TotalWeight);
            $.event.trigger('shoppingcart.changed');
        });

}

function RemoveFromCart(productId) {
    $.post("/ShoppingCart/RemoveFromCart", { "id": productId },
            function (data) {
                var number_pp = 0;
                if (data.Success) {

                    toastr.success(data.Message);
                    $('tr[data-id=' + productId + ']').remove();

                    if (parseInt(data.CartCount) == 0) {
                        $('div#shopping-cart').remove();

                        $('h2').text(YstLocale.Get("cartempty"));
                        $("#cart-status-count").text(YstLocale.Get(" "));

                    } else {

                        $("#cart-status-count").text(data.CartCount);
                        
                        $('div#shopping-cart td:first-child').each(function () {
                            $(this).text(++number_pp);
                        });
                    }


                    $('#cart-total-OfClient').text(data.CartTotalOfClient);
                    $('#cart-total').text(data.CartTotal);
                    $('#cart-count,.cart-count').text(data.CartCount);

                    $('.AddLink[data-id = ' + productId + ']').removeClass('invisible');
                    $('.DelLink[data-id = ' + productId + ']').addClass('invisible');
                    $('#ProductName-' + productId).removeClass('add_text').addClass('a_text');
                    $('#cart-weight').text(data.TotalWeight);
                    $.event.trigger('shoppingcart.changed');

                } else {
                    toastr.error(data.Message);
                }
            });
}

function getDocHeight() {
    var D = document;
    return Math.max(
        D.body.scrollHeight, D.documentElement.scrollHeight,
        D.body.offsetHeight, D.documentElement.offsetHeight,
        D.body.clientHeight, D.documentElement.clientHeight
    );
}
function getCurrentY() { return (document.documentElement.scrollTop || document.body.scrollTop); }

function ShowLoading(message) {
    var background = $('<div id="loading-background">').css({
        'backgroundColor': '#000',
        'width': '100%',
        'height': '100%',
        'position': 'fixed',
        'z-index': '1',
        'top': '0',
        'left': '0',
        'opacity': '0.2'
    });

    var image = $('<img>').attr({
        'src': '/Content/img/big_loading.gif',
        'style': 'width: 100px;'
    });

    //'Идёт формирование заказа. Подождите, пожалуйста.'
    var text = $('<span>').text(message).css({
        'font-family': 'Verdana, Arial, Helvetica, sans-serif',
        'font-weight': 'bold',
        'font-size': '16px',
        'color': 'rgb(0, 136, 204)'
    });

    var loading = $('<div id="loading">').css({
        'z-index': '100',
        'width': '290px',
        'position': 'fixed',
        'top': '37%',
        'left': '37%',
        'text-align': 'center',
        'background-color': '#fff',
        'padding': '30px'
    }).html(image).append('<br><br>').append(text);


    $('body').prepend(loading).prepend(background);

}


function DisplayLoading(message) {

    var $background = $('<div class="background-over-window"></div>');

    var $loader = $('<div class="white-sheet"><div class="loader"></div></div>');
    $('body').prepend($loader).prepend($background);

}

//$(function () {

    $(".fancybox-btn").fancybox({
        helpers: {
            title: {
                type: 'inside',
                position: 'bottom'
            }
        },
        beforeShow: function () {
            this.title = $(this.element).data("title");
        }
    });

    $(".fancybox-btn-disks").fancybox({
        'transitionIn': 'none',
        'transitionOut': 'none',
        'titlePosition': 'over',
        'cyclic': true,
        'autoPlay': true,
        "type": "image"
    });

    $('.has-popover').popover({ trigger: "hover focus", placement: "bottom", html: true });

    $("table.clickable-rows tbody>tr").dblclick(function () {
        window.document.location = $(this).data("href");
    });

    $('.date').datetimepicker({
        format: 'DD.MM.YYYY', locale: YstLocale.GetLocale("Culture"),
        showTodayButton: true,
        //  daysOfWeekDisabled: [6, 7],
        showClear: true
    });


    $.validator.methods.date = function (value, element) {
        return this.optional(element) || (value.length > 0 && !/Invalid|NaN/.test(new Date(value.replace(/(\d{2}).(\d{2}).(\d{4})/, "$2/$1/$3"))));
    }


    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('.scrolltop:hidden').stop(true, true).fadeIn();
        } else {
            $('.scrolltop').stop(true, true).fadeOut();
        }
    });
    $(function () { $(".scroll").click(function () { $("html,body").animate({ scrollTop: $(".first-horizontal-menu").offset().top }, "1000"); return false }) })
/*
    if ($(window).scrollTop() + $(window).height() + 100 >= getDocHeight())
        $('.scrolldown').hide();

    // скроллинг
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }



        if ($(window).scrollTop() + $(window).height() + 100 >= getDocHeight())
            $('.scrolldown').fadeOut();

        else
            $('.scrolldown').fadeIn();
    });

    $('.scrollup').click(function () {

        var scrollupTo = Math.max(getCurrentY() - window.innerHeight + $('div.container').height() || 0, 0);
        $("html, body").animate({ scrollTop: scrollupTo }, 600);

        return false;
    });
    $('.scrolldown').click(function () {

        var scrolldownTo = Math.max(Math.min($(document).height(), getCurrentY() + window.innerHeight - $('div.container').height() || 0), 0);
        // var scrolldownTo = Math.max(getCurrentY() - window.innerHeight + $('div.container').height() || 0, 0);
        $("html, body").animate({ scrollTop: scrolldownTo }, 600);

        return false;
    });

    */

    // изменение заказа
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

   


    $('form#user-agreement').on('change', '[type=checkbox][name=ConditionsAreAccepted]', function () {

        $(this).closest('form').submit();

    });

    // { start подборы по видам товаров (формируем ЧПУ) Lapenkov  

    var modulePodborsByParameters = ( function () {

        $('form#tyres-podbor').submit(function (evt) {

            var ItemsPerPage = $("#ItemsPerPageForm :selected").val();
            $("#ItemsPerPage").val(ItemsPerPage);

            var ship = $("input:radio:checked").val();
            $("#Ship").val(ship);

            var pricemin = $("#price-slider-min").val();
            var pricemax = $("#price-slider-max").val();
            $("#PriceMin").val(pricemin);
            $("#PriceMax").val(pricemax);

            var arrToUFU = [],
                pathObj = {},
                arrAddToPath = [],
                strArrToSerialize = ["ProducerId", "SeasonId", "Width", "Height", "Diametr", "Ship"];

            var initialArr = $(this).serializeArray();

            arrToUFU = initialArr.filter(function f(element) { return (element.value != '') && (strArrToSerialize.indexOf(element.name) >= 0) }); //.map(function (element) { Obj = {}; Obj[element.name] = element.value; return Obj; });

            arrToUFU.forEach(function (p) {
                pathObj[p.name] = p.value;
            });

            strArrToSerialize.forEach(function (p) {
                if (pathObj[p] === undefined) {
                    pathObj[p] = "all";
                }
            });

            // Работает для формы подбора и формы сезонного ассортимента
            var route_path = $(this).attr('action').match(/\w+\/\w+/);

            var newUrl = String.format('/{0}/{1}/{2}/{3}-{4}-R{5}/', route_path, pathObj['ProducerId'], pathObj['SeasonId'], pathObj['Width'], pathObj['Height'], pathObj['Diametr']);

            var addToPath = $('input[name=SortBy],input[name=DisplayView],input[name=Name], input[name=Article], input[name=SeasonPostId], input[name=ItemsPerPage], input[name=Ship], input[name=IsSet4Items], input[name=PriceMin], input[name=PriceMax]').filter(function () { return $.trim(this.value).length > 0 }).serialize();


            location.assign(newUrl + "?" + addToPath);
            return false;

        });


        $('form#disk-podbor').submit(function (evt) {

            var ItemsPerPage = $("#ItemsPerPageForm :selected").val();
            $("#ItemsPerPage").val(ItemsPerPage);
            var pricemin = $("#price-slider-min").val();
            var pricemax = $("#price-slider-max").val();
            $("#PriceMin").val(pricemin);
            $("#PriceMax").val(pricemax);
            var CarName = $("#CarsList :selected").val();
            $("#CarName").val(CarName);
            var AllOrByCarReplica = false;
            if ($('#65').prop('checked')) {
                AllOrByCarReplica = $('#ReplicaByAuto').prop('checked') || false;
            }
            $("#AllOrByCarReplica").val(AllOrByCarReplica);
            var arrToUFU = [],
                pathObj = {},
                arrAddToPath = [],
                strArrToSerialize = ["ProducerId", "Width", "Diametr", "Hole", "PCD", "ET", "DIA"];

            var initialArr = $(this).serializeArray();

            arrToUFU = initialArr.filter(function f(obj) { return (obj.value != "") && (strArrToSerialize.indexOf(obj.name) >= 0) });

            arrToUFU.forEach(function (p) {
                pathObj[p.name] = p.value;
            });

            strArrToSerialize.forEach(function (p) {
                if (pathObj[p] === undefined) {
                    pathObj[p] = "all";
                }
            });

            var route_path = $(this).attr('action').match(/\w+\/\w+/);

            var newUrl = String.format('/{0}/{1}/{2}x{3}_{4}x{5}_ET{6}_D{7}/', route_path, pathObj['ProducerId'], pathObj['Width'], pathObj['Diametr'], pathObj['Hole'], pathObj['PCD'], pathObj['ET'], pathObj['DIA']);

            var checkedProducers = $('#Brands input:checked');
            if (checkedProducers.length > 0) {
                ids = checkedProducers.map(function () {
                    return this.value;
                }).get().join(',');
                $("#Brands").val(ids);
            } else {
                $("#Brands").val(null);
            }

            var addToPath = $('input[name=SortBy],input[name=DisplayView],input[name=Name], input[name=Article], select[name=DiskColor],input[name=SeasonPostId], input[name=ItemsPerPage], input[name=PriceMin], input[name=PriceMax], input[name=Brands], input[name=FromRests], input[name=FromOnWay] ,input[name=OnlySale], input[name=IsSet4Items],input[name=CarName],input[name=AllOrByCarReplica],select[name=ETto]').filter(function () { return $.trim(this.value).length > 0 }).serialize();

            location.assign(newUrl + "?" + addToPath);
            return false;

        });




        $('form#acc-podbor').submit(function (evt) {
            var ItemsPerPage = $("#ItemsPerPageForm :selected").val();
            $("#ItemsPerPage").val(ItemsPerPage);
            var url = "/Home/Accs";
            var ids;

            var checkedProducers = $('#acc-categories input:checked');
            if (checkedProducers.length > 0) {
                ids = checkedProducers.map(function () {
                    return this.value;
                }).get().join(',');
                url += "/Categories-" + ids;
            }
            var addToPath = $('input[name=Name], input[name=Article], input[name=ItemsPerPage], select[name=ProducerId],input[name=OnlySale]').filter(function () { return $.trim(this.value).length > 0 }).serialize();
            location.assign(url + "?" + addToPath);
            //   location.href = url;
            return false;
        });

        //обнуление формы подбора
        $('a[name=clear]').click(function () {

            var maxValuePrice = 70000;
            var form = $(this).closest('form');

            form.find('select').each(function () { $(this).val($(this).find("option:first").val()) });
            $('input[type="checkbox"]').each(function () {
                $(this).prop("checked", false);
                $('#FromRests').prop("checked", true);
            });
            form.find('[name=Article]').val('');
            form.find('[name=PriceMin]').val(0);
            form.find('[name=PriceMax]').val(maxValuePrice);
            $('#price-slider-min').val(0);
            $('#price-slider-max').val(maxValuePrice);

            form.submit();
        });


        $(document).on('click', '.min_img', function () {
            var fullPath = $(this).data('src');
            $('#large_foto').empty().append('<img class="img_in_popup" src="' + fullPath + '">');
            $('#popup2').show('slow');
        });

        $("#slider-range").slider({
            range: true,
            step: $("#PriceStepSlide").text(),
            min: 0,
            max: $("#MaxPrice").text(),
            values: [$("#PriceMin").val(), $("#PriceMax").val()],
            slide: function (event, ui) {
                $("#price-slider-min").val(ui.values[0]);
                $("#price-slider-max").val(ui.values[1]);
            }
        });
        //$("#amount").val("$" + $("#slider-range").slider("values", 0) +
        //  " - $" + $("#slider-range").slider("values", 1));
        $("#price-slider-min").val($("#slider-range").slider("values", 0));
        $("#price-slider-max").val($("#slider-range").slider("values", 1));



        //New design----
        //Ценовой слайдер старт
        // $(function () {

        // });

        function dataChanged(obj, index) {
            //alert(obj.val());
            $("#slider-range").slider("values", index, obj.val());
        }

        $('#price-slider-min,#price-slider-max').each(function () {
            var elem = $(this);
            elem.data('oldVal', elem.val());
            elem.bind("propertychange change click keyup input paste", function (event) {
                if (elem.data('oldVal') != elem.val()) {
                    elem.data('oldVal', elem.val());
                    var $obj = $(this);
                    if ($obj.data('lock') !== true) {
                        $obj.data('lock', true);
                        setTimeout(function () {
                            var index = 1;
                            if ($obj.attr("id") == "price-slider-min") index = 0;
                            dataChanged($obj, index);
                            $obj.data('lock', false);
                        }, 500);
                    }
                }
            });
        });
    //Ценовой слайдер конец

    }());

    var modulePodborTyresDisksByAuto = (function () {

        function resetModels() {

            $('#model option').remove();

            $('#model').append($('<option />', { value: '-1', text: YstLocale.Get("model") }));

            $('#model').attr('disabled', 'disabled');
        }

        function resetYears() {
            $('#year option').remove();

            $('#year').append($('<option />', { value: '-1', text: YstLocale.Get("issueyear") }));

            $('#year').attr('disabled', 'disabled');
        }

        function resetEngines() {
            $('#engine option').remove();

            $('#engine').append($('<option />', { value: '-1', text: YstLocale.Get("modification") }));


            $('#engine').attr('disabled', 'disabled');
        }

        $("form#podborauto").find("select[name='brand']").change(function () {

            var value = $("#podborauto select[name='brand'] :selected").val();
            if (value.length == 0) { return; }
            var uri = '/api/productsapi/getmodels?' + $('#podborauto select').serialize();
            $("#div-exact-size").addClass('invisible');

            $.getJSON(uri, function (data) {
                resetModels();
                resetYears();
                resetEngines();

                $.each(data, function (key, item) {
                    $("#podborauto select[name='model']").append($('<option>', {
                        value: item, text: item
                    }));
                });
                $('#model').attr('disabled', false);
            });

        });

        $("form#podborauto").find("select[name='model']").change(function () {


            var value = $("#podborauto select[name='model'] :selected").val();
            if (value.length == 0) { return; }
            var uri = '/api/productsapi/getyears?' + $('#podborauto select').serialize();
            $("#div-exact-size").addClass('invisible');
            $.getJSON(uri, function (data) {
                resetYears();
                resetEngines();

                $.each(data, function (key, item) {
                    $("#podborauto select[name='year']").append($('<option>', {
                        value: item, text: item
                    }));
                });
                $('#year').attr('disabled', false);
            });

        });

        $("form#podborauto").find("select[name='year']").change(function () {

            var value = $("#podborauto select[name='year'] :selected").val();
            if (value.length == 0) { return; }
            var uri = '/api/productsapi/getengines?' + $('#podborauto select').serialize();
            $("#div-exact-size").addClass('invisible');
            $.getJSON(uri, function (data) {
                resetEngines();

                $.each(data, function (key, item) {
                    $("#podborauto select[name='engine']").append($('<option>', {
                        value: item, text: item
                    }));
                });
                $('#engine').attr('disabled', false);
            });

        });


        $("form#podborauto").find("select[name='engine']").change(function () {

            var arrObj = [];
            $("form#podborauto").find('select').each(function () { arrObj.push($(this).val()); });
            $("#div-exact-size").addClass('invisible');
            arrObj[3] = arrObj[3].replace(/\//gi, "~");
            var newUrl = String.format('/PodborAutoTyresDisks/Index/{0}/{1}/{2}/{3}', arrObj[0], arrObj[1], arrObj[2], arrObj[3]);

            location.assign(newUrl);
        });



        /*
        * обработка клика по резульатам подбора по авто
        */

        $('#podborautoresult').on('click', 'a', aPodborHandler);
        $('#AllWheels').on('click', function () {
            $('#SortCheckBoxes').removeClass("invisible");
            $('#podborautoresult td.activePodbor a').trigger('click');
        });
        function aPodborHandler(event) {
            event.preventDefault();
            var ptr = $("#podbortiporazmerresult");
            if ($('#table').hasClass("active")) {
                var dspl = "Table";
            } else {
                var dspl = "Plitka";
            }

            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");

            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = true;
            } else {
                var sale = false;
            }

            $("#AllWheels").addClass("active");
            $("#ForgedWheels").removeClass("active");
            $("#ForSortSale").removeClass("invisible");
            $('#SortCheckBoxes').removeClass("invisible");
            $("#sortBtnRow").removeClass("invisible");

            $('#cart-status').attr("href", "/ShoppingCart");
            // $("#ForgedWheels").attr("href", "/SeasonProduct/Disks/Forged");
            var exact_size = $('input[type=checkbox][name=exact-size]').prop('checked') || false === true ? 1 : 0;
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            var disks = this.href.indexOf("Disks");
            if (disks != -1) {
                $('#AllWheels').removeClass("invisible");
                $('#ForgedWheels').removeClass("invisible");
                $('#Seasons').addClass("invisible");
            } else {
                $('#AllWheels').addClass("invisible");
                $('#ForgedWheels').addClass("invisible");
                $('#Seasons').removeClass("invisible");
            }
            if (!$('.ForSortSale').hasClass("invisible")) {
                if (disks != -1) {
                    $('.ForSortSale').removeClass("invisible");
                } else {
                    $('.ForSortSale').addClass("invisible");
                }
            }
            var spar = this.href.indexOf("Spar");
            if (spar != -1) {
                $('#SortCheckBoxes').addClass("invisible");
                $('#AllWheels').addClass("invisible");
                $('#ForgedWheels').addClass("invisible");
                $("#sortBtnRow").addClass("invisible");
            }
            localStorage['selectedlink'] = this.pathname;
            ptr.empty().addClass('loading');
            ptr.load(this.href, { "exactsize": exact_size, "DisplayView": dspl, "OnlySale": sale, "FromRests": FromRests, "FromOnWay": FromOnWay }, function () {
                ptr.removeClass('loading');
                $(".count_add_to_cart").TouchSpin({
                    min: 1,
                    max: 200,
                    initval: 1,
                    buttondown_class: "btn btn-link touchspih-podbor-settings",
                    buttonup_class: "btn btn-link touchspih-podbor-settings"
                });
            });
            $("#div-exact-size").removeClass('invisible');
            $('table#podborauto').find('td.activePodbor').toggleClass('activePodbor');
            $(this).parent().addClass('activePodbor');


        }

        $('#exact-size').on('change', aPodborHandlerCheckBox);
        $('#FromRests').on('change', aPodborHandlerCheckBoxRestOnWay);
        $('#FromOnWay').on('change', aPodborHandlerCheckBoxRestOnWay);
        $('#ForgedWheels').on('click', aPodborHandlerForgedWheels);
        $('.season_change').on('click', aPodborHandlerSeaeonSelect);
        $('#complect').on('change', aPodborHandlerCheckBox);

        function aPodborHandlerForgedWheels(event) {
            var $ptr = $("#podbortiporazmerresult");
            $('#cart-status').attr("href", "/SeasonShoppingCart");
            var exact_size = $(this).prop('checked') || false === true ? 1 : 0;
            if ($('#table').hasClass("active")) {
                var dspl = "Table";
            } else {
                var dspl = "Plitka";
            }

            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");
            $("#AllWheels").removeClass("active");
            $("#ForgedWheels").addClass("active");
            $('#SortCheckBoxes').addClass("invisible");
            $("#sortBtnRow").addClass("invisible");
            // /Home/Disks/all/7x17_5x105_ET38_D56.6
            var arrUrlParam = $('td.activePodbor').find('a').attr('href').split('/');
            var arrParam = arrUrlParam[4].split('_');
            var width_diam = arrParam[0].split('x');
            var hole_pcd = arrParam[1].split('x');
            var et = arrParam[2].split('T')[1];
            var dia = arrParam[3].split('D')[1];

            var href = "/SeasonProduct/Disks/Forged?Width=" + width_diam[0] + "&Diametr=" + width_diam[1] + "&Hole=" + hole_pcd[0] + "&PCD=" + hole_pcd[1] + "&ET=" + et + "&DIA=" + dia;
            console.log(href);
            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": dspl }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                    $(".count_add_to_seasoncart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }

        function aPodborHandlerSeaeonSelect(event) {
            var $ptr = $("#podbortiporazmerresult");
            var complect = $('input[type=checkbox][name=complect]').prop('checked');
            var exact_size = $('input[type=checkbox][name=exact-size]').prop('checked') || false === true ? 1 : 0;
            if ($('#table').hasClass("active")) {
                var dspl = "Table";
            } else {
                var dspl = "Plitka";
            }
            var season = $(this).attr('name');
            console.log(season);
            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = true;
            } else {
                var sale = false;
            }
            var href = $('td.activePodbor').find('a').attr('href');
            href = href.replace("/all/all/", "/all/" + season + "/");
            console.log(href);
            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": dspl, "OnlySale": sale, "FromRests": FromRests, "FromOnWay": FromOnWay, "IsSet4Items": complect }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }

        function aPodborHandlerCheckBox(event) {
            var $ptr = $("#podbortiporazmerresult");
            var complect = $('input[type=checkbox][name=complect]').prop('checked');
            var exact_size = $('input[type=checkbox][name=exact-size]').prop('checked') || false === true ? 1 : 0;
            if ($('#table').hasClass("active")) {
                var dspl = "Table";
            } else {
                var dspl = "Plitka";
            }
            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = true;
            } else {
                var sale = false;
            }
            var href = $('td.activePodbor').find('a').attr('href');

            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": dspl, "OnlySale": sale, "FromRests": FromRests, "FromOnWay": FromOnWay, "IsSet4Items": complect }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }

        function aPodborHandlerCheckBoxRestOnWay(event) {
            var $ptr = $("#podbortiporazmerresult");
            var exact_size_bool = $('input[type=checkbox][name=exact-size]').prop('checked');
            if (exact_size_bool) {
                var exact_size = 1;
            } else {
                exact_size = 0;
            }

            if ($('#table').hasClass("active")) {
                var dspl = "Table";
            } else {
                var dspl = "Plitka";
            }

            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = true;
            } else {
                var sale = false;
            }
            var href = $('td.activePodbor').find('a').attr('href');

            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": dspl, "OnlySale": sale, "FromRests": FromRests, "FromOnWay": FromOnWay }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }

        $('#plitka').on('click', aPodborHandlerViewPl);

        function aPodborHandlerViewPl(event) {
            $('#plitka').addClass("active");
            $('#table').removeClass("active");
            var $ptr = $("#podbortiporazmerresult");
            var exact_size = $('input[type=checkbox][name=exact-size]').prop('checked') || false === true ? 1 : 0;
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = true;
            } else {
                var sale = false;
            }

            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");

            var href = $('td.activePodbor').find('a').attr('href');

            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": "Plitka", "OnlySale": sale, "FromRests": FromRests, "FromOnWay": FromOnWay }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }

        $('#table').on('click', aPodborHandlerViewTb);

        function aPodborHandlerViewTb(event) {
            $('#table').addClass("active");
            $('#plitka').removeClass("active");
            var $ptr = $("#podbortiporazmerresult");
            var exact_size = $('input[type=checkbox][name=exact-size]').prop('checked') || false === true ? 1 : 0;
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = true;
            } else {
                var sale = false;
            }

            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");

            var href = $('td.activePodbor').find('a').attr('href');

            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": "Table", "OnlySale": sale, "FromRests": FromRests, "FromOnWay": FromOnWay }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }

        $('#sort-sale').on('click', aPodborHandlerOnlySale);

        function aPodborHandlerOnlySale(event) {

            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = false;
                $('#sort-sale').removeClass("only-sale");
            } else {
                var sale = true;
                $('#sort-sale').addClass("only-sale");
            }

            $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-default");

            var $ptr = $("#podbortiporazmerresult");
            var exact_size = $('input[type=checkbox][name=exact-size]').prop('checked') || false === true ? 1 : 0;
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            if ($('#table').hasClass("active")) {
                var dspl = "Table";
            } else {
                var dspl = "Plitka";
            }

            var href = $('td.activePodbor').find('a').attr('href');

            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": dspl, "OnlySale": sale, "FromRests": FromRests, "FromOnWay": FromOnWay }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }




        $(document).on('click', 'a[name=sortBtn]', function () {
            var id = $(this).attr('data-sort');
            if (id == "NameAsc") {
                $(this).attr('data-sort', "NameDesc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortName").addClass("sort-icon-active-asc");
            }
            if (id == "NameDesc") {
                $(this).attr('data-sort', "NameAsc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortName").addClass("sort-icon-active-desc");
            }
            if (id == "DeliveryAsc") {
                $(this).attr('data-sort', "DeliveryDesc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortDelivery").addClass("sort-icon-active-asc");
            }
            if (id == "DeliveryDesc") {
                $(this).attr('data-sort', "DeliveryAsc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortDelivery").addClass("sort-icon-active-desc");
            }
            if (id == "AmountAsc") {
                $(this).attr('data-sort', "AmountDesc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortAmount").addClass("sort-icon-active-asc");
            }
            if (id == "AmountDesc") {
                $(this).attr('data-sort', "AmountAsc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortAmount").addClass("sort-icon-active-desc");
            }
            if (id == "PriceAsc") {
                $(this).attr('data-sort', "PriceDesc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortPrice").addClass("sort-icon-active-asc");
            }
            if (id == "PriceDesc") {
                $(this).attr('data-sort', "PriceAsc");
                $("span[name=sortIcon]").removeAttr("class").addClass("sort-icon-active-default");
                $("#sortPrice").addClass("sort-icon-active-desc");
            }
            aPodborHandlerSort(id);
        });

        function aPodborHandlerSort(id) {


            $('#table').addClass("active");
            $('#plitka').removeClass("active");

            $('a[name=sortBtn]').removeClass("active");
            var acive = '#' + id;
            $(acive).addClass("active");

            var $ptr = $("#podbortiporazmerresult");
            var exact_size = $('input[type=checkbox][name=exact-size]').prop('checked') || false === true ? 1 : 0;
            var FromRests = $('input[type=checkbox][name=FromRests]').prop('checked');
            var FromOnWay = $('input[type=checkbox][name=FromOnWay]').prop('checked');
            if ($('#sort-sale').hasClass("only-sale")) {
                var sale = true;
            } else {
                var sale = false;
            }

            if ($('#table').hasClass("active")) {
                var dspl = "Table";
            } else {
                var dspl = "Plitka";
            }

            var href = $('td.activePodbor').find('a').attr('href');

            if (href !== undefined) {
                $ptr.empty().addClass('loading');
                $ptr.load(href, { "exactsize": exact_size, "DisplayView": dspl, "OnlySale": sale, "SortBy": id, "FromRests": FromRests, "FromOnWay": FromOnWay }, function () {
                    $ptr.removeClass('loading');
                    $(".count_add_to_cart").TouchSpin({
                        min: 1,
                        max: 200,
                        initval: 1,
                        buttondown_class: "btn btn-link touchspih-podbor-settings",
                        buttonup_class: "btn btn-link touchspih-podbor-settings"
                    });
                });
            }

        }


    }());


/*
    // { всплывашка картинки при нажатии

   


    $('#popup2').click(function () {
        //$('#popup2').hide('slow');
        $(this).hide('slow');
    });

    $('.open_popup_th').click(function () {
        var src = $(this).attr('src');
        $('#large_foto').empty().append('<img class="img_in_popup" src="' + src + '">');
        $('#popup2').show('slow');
    });

    // } всплывашка картинки при нажатии
*/



    $("#SaveAdress").on("click", function () {
        $('input#Address').val($('#address').text());
    });

  


 


    var moduleAccPodbor = (function () {
        $('#resetAccsButton').click(function () {
            var form = $(this).closest('form');
            form.find('select').each(function () { $(this).val($(this).find("option:first").val()) });
            form.find('input[type="checkbox"]').each(function () { $(this).prop("checked", false) });
            form.submit();
        });

        $('.multiplue').click(function () {
            var control = $(this);
            var folder = control.data('child');
            $('div[data-level=2]').each(function () {
                if ($(this).data('parent') == folder) {
                    if ($(this).is(':hidden')) {
                        $(this).removeAttr("hidden");
                        control.removeClass("glyphicon-plus");
                        control.addClass("glyphicon-minus");
                    }
                    else {
                        $(this).attr("hidden", "hidden");
                        control.removeClass("glyphicon-minus");
                        control.addClass("glyphicon-plus");
                    }
                }
            });
        });

        $('input[type="checkbox"].accs-category').change(function () {
            var form = $(this).closest('form');
            var control = $(this);
            if (control.prev(".multiplue").length != 0) {
                var folder = control.prev(".multiplue").data("child");
                $('div[data-level=2]').each(function () {
                    if ($(this).data('parent') == folder) {
                        if ($(this).is(':hidden')) {
                            $(this).removeAttr("hidden");
                            control.prev(".multiplue").removeClass("glyphicon-plus");
                            control.prev(".multiplue").addClass("glyphicon-minus");
                            $(this).find('input[type=checkbox]').attr("checked", true);
                        }
                    }

                });
            }
            form.submit();
        });
    }());

    $(document).on("change", ".change-in-cart", UpdateToCart);








    var moduleAkbPodborByAuto = ( function () {

        function resetAkbModels() {
            $('#carModel option').remove();
            $('#carModel').append($('<option />', { value: '-1', text: YstLocale.Get('model') }));
            $('#carModel').attr('disabled', 'disabled');
        }

        function resetYears() {
            $('#year option').remove();

            $('#year').append($('<option />', { value: '-1', text: YstLocale.Get("issueyear") }));

            $('#year').attr('disabled', 'disabled');
        }

        function resetEngines() {
            $('#engine option').remove();

            $('#engine').append($('<option />', { value: '-1', text: YstLocale.Get("modification") }));


            $('#engine').attr('disabled', 'disabled');
        }

        var formpodborakb = $("form#podborakb");

        formpodborakb.find("select[name='brand']").change(function () {
            console.log('brand');
            var value = $("#podborakb select[name='brand'] :selected").val();
            if (value.length == 0) return;
            var uri = '/podborakb/getmodels?' + $('#podborakb select').serialize();
            console.log(uri);

            $.getJSON(uri, function (data) {
                resetAkbModels();
                resetYears();
                resetEngines();
                $.each(data, function (key, item) {
                    $("#podborakb select[name='carModel']").append($('<option>', {
                        value: item, text: item
                    }));
                });
                $('#carModel').attr('disabled', false);
            });

        });

        formpodborakb.find("select[name='carModel']").change(function () {

            var value = $("#podborakb select[name='carModel'] :selected").val();
            if (value.length == 0) return;
            var uri = '/podborakb/getyears?' + $('#podborakb select').serialize();

            $.getJSON(uri, function (data) {
                resetYears();
                resetEngines();
                //  $("#podborauto select[name='year']").append('<option>'+YstLocale.Get('issueYear')+ '</option>');
                $.each(data, function (key, item) {
                    $("#podborakb select[name='year']").append($('<option>', {
                        value: item, text: item
                    }));
                });
                $('#year').attr('disabled', false);
            });

        });


        formpodborakb.find("select[name='year']").change(function () {


            var value = $("#podborakb select[name=year] :selected").val();
            if (value.length == 0) return;
            var uri = '/podborakb/getengines?' + $('#podborakb select').serialize();

            $.getJSON(uri, function (data) {
                resetEngines();
                // $("#podborauto select[name='engine']").append('<option>' + YstLocale.Get('modification') + '</option>');
                $.each(data, function (key, item) {
                    $("#podborakb select[name=engine]").append($('<option>', {
                        value: item, text: item
                    }));
                });
                $('#engine').attr('disabled', false);
            });

        });


        formpodborakb.find("select[name=engine]").change(function () {
            var value = $("#podborakb select[name=engine] :selected").val();
            if (value.length == 0) return;
            var url = '/PodborAkb/Index/?' + $('#podborakb select').serializeArray().map(function (p) { return p.value }).join('&');
            // console.log(url.toLowerCase());
            location.assign(url.toLowerCase());
        });
        // } akb by auto
        //-----------end

    }());


    // begin uploader for excel
    $(document).on('change', '.btn-file :file', function () {

        var input = $(this),
        numFiles = input.get(0).files ? input.get(0).files.length : 1,
        label = input.val().replace(/\\/g, '/').replace(/.*\//, '');

        console.log(input);
        input.trigger('fileselect', [numFiles, label]);
    });

    $('.btn-file :file').on('fileselect', function (event, numFiles, label) {
        $(this).closest('form').submit();

    });
    // -- end uploader for excel

//});


    $('.open_order_tr').on('click', function (e) {

        var productId = $(this).data("idlink");

        if ($('tr[data-idsource = ' + productId + ']').hasClass("invisible")) {
            $('tr[data-idsource = ' + productId + ']').removeClass('invisible');
        } else {
            $('tr[data-idsource = ' + productId + ']').addClass('invisible');
        }
        
    });






