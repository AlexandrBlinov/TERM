function showNotification(data) {

    if (!data) return;
    var pathtopopup = '/seasonstock/popup';


    
    if (!Array.isArray(data) || data.length==0) return;
    var title = data[0].Title || '';
    var endDate = data[0].EndDate || '';
    var contents = data[0].Contents || '';
    var postId = parseInt(data[0].Id);
    var producerId = parseInt(data[0].ProducerId);

    
    if (!producerId || !postId) return;
    



    $.get(pathtopopup, {}, function (data) {
        var divtext = data;
        divtext = String.format(divtext, title, contents, postId, producerId);
        var $divtext = $(divtext);
        $divtext.css({display:'none'});

        $('body').prepend($divtext);
        $divtext.modal('show');

        $divtext.on('hidden.bs.modal', function () {
            $(this).remove();
        });
       
    });

}

function getActiveFirstPost() {
    var check = true;
    // если активные уведомления есть
    if ($('#popup-active-posts').length) return;
    var path = '/seasonstock/activeposts';
    $.getJSON(path, {}, function (data) {
        console.log(data);
        showNotification(data);
    });
    setTimeout(getActiveFirstPost, 600000);
}

/*test time out*/
$(document).ready(function () {
    var check = $('input#getPost').size();
    if (check==1) getActiveFirstPost();
});
/*end test*/

$(function () {

    $('#seasoncart').submit(function () {
        var activeSeasonId = $('.nav.nav-tabs li.active a').data('id');
        $('[name=activeSeasonId]').val(activeSeasonId);
      //  return false;
    })
  /*  setInterval(function () {
        refreshUpdates('footer #updates');
    }, 60000); */

   // setInterval(function () { getActiveFirstPost() }, 100000);
}()

);

