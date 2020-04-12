$(function(){

    myStorage = window.localStorage;
    localStorage.setItem('autoCount', 1);

    var req = new XMLHttpRequest();
    
    $('.bt-style').click(function(){
        if (localStorage.autoCount > 22) {
            alert("Мест нет");
            return;
        }
        req.responseType = 'json';
        req.open('POST', 'Parking/EnterUserByAuto?tariffName=basic&autoId=' + localStorage.autoCount, true);
        req.onload  = function() {
            var key = req.response;
            console.log(key.AutoId + ' ' + key.Tariff);
        };
        req.send();
        if (req.status == 200) {
            $('#enter').remove();
            $('.button').append('<button id=\'exit\' class="bt-style">ПОКИНУТЬ ПАРКОВУ</button>');
            $('.car').append('<div class="car' + localStorage.autoCount++ + '"></div>');
        }
    });
});