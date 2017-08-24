var YstLocale = function () {

    var dictDays = {
        1: 'день',
        2: 'дня',
        3: 'дня',
        4: 'дня',
        5: 'дней',
        6: 'дней'
    };

    var dictionary = {
        "default": {
            "cart": "Order cart",
            "cartempty": "Cart is empty",
            "waitorder": "Please wait... Is the creation of order",
            "waitchang": "Please wait. The order is being changed",
            "model": "Model",
            "issueyear": "Year of issue",
            "modification": "Modification",
            "tyres": "Tyres",
            "disks": "Wheels",
            "zavod": "OEM",
            "zamena": "Replacement",
            "clear": "Clear",
            "added": "Added",
            "errdownloadfile": "Error loading file.",
            "sucdownloadfile": "File successfully downloaded.",
            "errsaledir": "Fill in sales market!",
            "errsite": "Fill in web site!",
            "marker": "Set marker on map!",
            "errdays": "Delivery time with additional stock can not be equal to the term of delivery from the main stock!",
            "plwait": "Please wait...",
            'seasoncart': 'Production order cart',
            'seasoncartempty': 'Production order cart is empty',
            'selectbrands': 'Select brands',
            'brandschosen': 'Brands chosen'
        },
        "ru": {
            "cart": "Корзина",
            "cartempty": "Корзина пуста",
            "waitorder": "Пожалуйста, подождите... Идет создание заказа",
            "waitchang": "Пожалуйста подождите... Идет изменения заказа",
            "model": "Модель",
            "issueyear": "Год выпуска",
            "modification": "Модификация",
            "tyres": "Шины",
            "disks": "Диски",
            "zavod": "Завод",
            "zamena": "Замена",
            "clear": "Удалить",
            "added": "Добавлено",
            "errdownloadfile": "Ошибка при загрузке файла.",
            "sucdownloadfile": "Файл успешно загружен.",
            "errsaledir": "Выберите направление продаж вашей компании!",
            "errsite": "Заполните поле сайт вашей компании!",
            "marker": "Для корректного определения адреса установите маркер на карте!",
            "errdays": "Срок доставки с дополнительного склада не может быть равен сроку доставки с основного!",
            "plwait": "Пожалуйста, подождите...",
            'seasoncart': 'Сезонная корзина',
            'seasoncartempty': 'Сезонная корзина пуста',
            'selectbrands': 'Выберите бренды',
            'brandschosen': 'Брендов выбрано'
        }
    };

    var strCulture = "Culture";

    function getCookie(name) {
        var matches = document.cookie.match(new RegExp("(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"));
        return matches ? decodeURIComponent(matches[1]) : undefined
    }


    var getLocale = function (culture) {
        var result = getCookie(culture) || "default";
        if (result !== "default") {
            result = result.substr(0, 2).toLowerCase();
        }

        return (result);
    }

    var getMoney = function (val) {
        var language = getLocale(strCulture) || "default";
        return Globalize.format(val, "c2", language);
    }

    var getDays = function(days) {
        return days +' ' +(dictDays[days] || 'дн.');
    }

    //
    // возвращает слово из словаря соответствующей локали YstLocale.Get('clear') или ''
    //
    var get = function (val) {

        // from browser
        //var language = (window.navigator.userLanguage || window.navigator.language).substr(0, 2).toLowerCase();

        // from cookies
        var language = getLocale(strCulture) || "default";

        if (dictionary[language] === undefined)
            language = "default";
        console.log(language);
        return dictionary[language][val.toLowerCase()] || "";
    };
    
    return {
        'Get': get,
        'GetLocale': getLocale,
        'GetMoney': getMoney,
        'GetDays':getDays
    };
}();