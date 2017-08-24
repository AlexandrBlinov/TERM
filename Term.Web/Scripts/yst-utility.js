
function getParametersFromPath(path)
{
    //var path ="products/tyres/{ProducerId}/{SeasonId}/{Width}-{Height}-r{Diametr}";
    var temp_arr = path.match(/\{\w+\}/g);
    var result_arr = [];
    temp_arr.forEach(function (p) { result_arr.push(p.replace(/[\{\}]/g, "")); })
    return result_arr;
}


function escapeHtml(unsafe) {
    return unsafe
         .replace(/&/g, "&amp;")
         .replace(/</g, "&lt;")
         .replace(/>/g, "&gt;")
         .replace(/"/g, "&quot;")
         .replace(/'/g, "&#039;");
}


function writeDiapazon(num1, num2) {
    if (Number.parseInt(num1) && num2 > 0 && Number.parseInt(num2) && num2 > 0) {
        if (num1 === num2) return num1.toString();
        return num1 + "-" + num2;
    }
    return "";
}




(function () {
    // Function works with array and params passed via comma
    String.format = function () {

        if (arguments.length < 2) return "";
        var theString = arguments[0];
        if (arguments.length === 2 && Array.isArray(arguments[1])) {
            var arr = arguments[1];
            for (var i = 0; i < arr.length; i++) {
                var regEx = new RegExp("\\{" + i + "\\}", "gm");

                theString = theString.replace(regEx, arr[i]);

            }
        }
        else {

            for (var i = 1; i < arguments.length; i++) {
                var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");

                theString = theString.replace(regEx, arguments[i]);

            }
        }

        return theString;
    }


    
  

}());

   
 