

$(document).on("change", ".change_in_seasoncart", UpdateToSeasonCart);
$(document).on("click", ".removefromseasoncart", RemoveFromSeasonCart);


function AddToSeasonCart(productId) {

    var count = $('#order-' + productId).val();


    if ($.isNumeric(productId) && productId > 0 && $.isNumeric(count)) {
        // Perform the ajax post
        $.post("/SeasonShoppingCart/AddToCart", { "id": productId, "count": count },
        function (data) {

            if (data.Success) {


                $("#cart-status-count").text(data.CartCount);


                $('#cart-total').text(data.CartTotal);
                toastr.success(data.Message);
               $('.AddLink[data-id = ' + productId + ']').addClass('invisible');
                $('.DelLink[data-id = ' + productId + ']').removeClass('invisible');
                $('#ProductName-' + productId).removeClass('a_text').addClass('add_text');
            } else
                toastr.error(data.Message);
        });
    }
}


function RemoveFromSeasonCart() {
    
    
    var parent = $(this).closest('tr');

    var productId=parent.data('id');
    var factory = parent.data('factory');
    var selector = '[data-factory-totals=' + factory + ']';

    $.post("/SeasonShoppingCart/RemoveFromCart", { "id": productId },
            function (data) {
                if (data.Success) {

                    toastr.success(data.Message);
                    $('tr[data-id=' + productId + ']').remove();

                    if (parseInt(data.CartCount) == 0) {
                        $('div#shopping-cart').remove();

                        $('h2').text(YstLocale.Get("seasoncartempty"));
                        $("#cart-status-count").text(" ");

                    } else {

                        $("#cart-status-count").text(data.CartCount);

                        var number_pp = 0;
                        $('div#shopping-cart td.number_pp').each(function () {
                            number_pp++;

                            $(this).text(number_pp);
                        });
                    }

                 
                    $('#cart-total').text(data.CartTotal);
                    $('#cart-count').text(data.CartCount);

                    if (data.CartCount === 0) location.reload();
                    if (factory && factory.length) {
                        if (data.CountByFactory === 0 || data.SumByFactory === 0) $(selector).remove();
                        else
                        {
                        //    debugger;
                            $(selector).find('.count').text(data.CountByFactory);
                            $(selector).find('.sum').text(data.SumByFactory);
                        }
                    }
                    
             
                } else {
                    toastr.error(data.Message);
                }
            });
}


function RemoveFromSeasonCart_Podbor(id) {
    
    var productId = id;
    
    $.post("/SeasonShoppingCart/RemoveFromCart", { 'id': productId },
            function (data) {
                if (data.Success) {
                    toastr.success(data.Message);
                    $('.AddLink[data-id = ' + productId + ']').removeClass('invisible');
                    $('.DelLink[data-id = ' + productId + ']').addClass('invisible');
                    $("#cart-status-count").text(data.CartCount);
                    $('#ProductName-' + productId).removeClass('add_text').addClass('a_text');
                }
                else toastr.error(data.Message);
            });
}

function UpdateToSeasonCart() {
  
    var itemCount = $(this).val();
    var tr = $(this).closest('tr');
    // id of product
    var productId = tr.data("id");

    // factory of product
    var factory = tr.data("factory");

   // var count_by_factory=sum_by_factory = 0;

    var updateTotalsByGroup = function (count_by_factory, sum_by_factory) {
        var selector = '[data-factory-totals=' + factory+']';
    
    if (factory) {
            
        
            $(selector).find('.count').text(count_by_factory);
            $(selector).find('.sum').text(sum_by_factory);
        
    }
    }
    if (!$.isNumeric(itemCount)  || itemCount < 1) {

        itemCount = 80;
        $(this).val(itemCount);

    }

    
    $.post("/SeasonShoppingCart/UpdateToCart", { "id": productId, "count": itemCount },
        function (data) {
            $("#cart-status-count").text(data.CartCount);
            $("#cart-count").text(data.CartCount);
            $("#cart-total").text(data.CartTotal);
            $("#cart-weight").text(data.CartWeight);
            $("#cart-volume").text(data.CartVolume);
            
            
            var price =data.ItemPrice;
           
            tr.find('.item-sum').text(Math.round(price * itemCount).toFixed(2));
           
            if ($('[data-factory-totals]').length )  updateTotalsByGroup(data.CountByFactory,data.SumByFactory);
        });

}

$(function () {

    $('#seasoncart').submit(function () {
        var $selector = $('.yst-wheeltypes li.active [data-id]');
        if ($selector.length) $('#ActiveWheelType').val($selector.data('id'));
        console.log($('#ActiveWheelType').val());
    //    return false;
    });
    
   
}());