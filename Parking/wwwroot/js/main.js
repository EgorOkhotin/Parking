$(function(){

    myStorage = window.localStorage;
    localStorage.setItem('autoCount', 7);

    var req = new XMLHttpRequest();
    var key;
    var postition;

    $('body').on("click", "#exit", function(){
        let currentAuto = localStorage.autoCount-1;
        req.open('GET', 'Parking/GetCost?autoId=' + currentAuto + '&token=' + key.token, false);
        req.send();
        console.log(req.status)
        if (req.status == 200) {
            let cost = JSON.parse(req.responseText);
            console.log(cost);

            req.open('POST', 'Parking/GetPay?token=' + key.token + '&cost=' + cost, false);
            req.send();
            console.log(req.responseText);

            $('#exit').remove();
            $('.box'+postition).remove();
            $('.button').append('<button id="enter" class="bt-style">ЗАНЯТЬ МЕСТО</button>');
            $('.message').append('<a class="textmessage">С вас ' + cost + 'р., Счастливого пути!</a>');
            localStorage.autoCount--;
        }
    });
    
    $('body').on("click", "#enter", function(){
        if (localStorage.autoCount > 22) {
            alert("Мест нет");
            return;
        }
        postition = randomInteger(localStorage.autoCount, 22);
        req.open('POST', 'Parking/EnterUserByAuto?tariffName=HIGH&autoId=' + localStorage.autoCount, false);
        req.send();
        console.log(req.status)
        if (req.status == 200) {
            console.log(req.responseText);
            key = JSON.parse(req.responseText);
            console.log(key.token)

            $('#enter').remove();
            $('.textmessage').remove();
            $('.button').append('<button id="exit" class="bt-style">ПОКИНУТЬ ПАРКОВУ</button>');
            $('.car').append('<div class="box' + postition + '"><div class="car' + postition + '"></div></div>');
            localStorage.autoCount++;
        }
    });
});

function randomInteger(min, max) {
    // получить случайное число от (min-0.5) до (max+0.5)
    let rand = min - 0.5 + Math.random() * (max - min + 1);
    return Math.round(rand);
  }