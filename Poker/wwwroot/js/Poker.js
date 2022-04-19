"use strict";

//on connection
var connection = new signalR.HubConnectionBuilder().withUrl("/pokerHub").build();

//constants
var CARD_PROPORTION = 8;
var CARD_WIDTH = 500 / CARD_PROPORTION;
var CARD_HEIGHT = 726 / CARD_PROPORTION;
var CARD_SPACER = CARD_WIDTH / 6;
var CARD_SHIFT = 80;
var USER_SIZE = 80;

var CANVAS_WIDTH = 780;
var CANVAS_HEIGHT = 405;

var PLAYER_POSITIONS = [[390, 330], [620, 290], [590, 80], [190, 80], [160, 290]];


//todo in for
var CARD_POSITIONS = [
    [CANVAS_WIDTH / 2 - 2 *CARD_WIDTH, CANVAS_HEIGHT / 2],
    [CANVAS_WIDTH / 2 - 1 *CARD_WIDTH , CANVAS_HEIGHT / 2],
    [CANVAS_WIDTH / 2 - 0 * CARD_WIDTH, CANVAS_HEIGHT / 2],
    [CANVAS_WIDTH / 2 - 1 *CARD_WIDTH, CANVAS_HEIGHT / 2],
    [CANVAS_WIDTH / 2 - 2 * CARD_WIDTH, CANVAS_HEIGHT / 2]];
var Shapes = {
    hearts: 0,
    clubs: 1,
    diamonds: 2,
    spades: 3
};
var Vals = {
    two: 0,
    three: 1,
    four: 2,
    five: 3,
    six: 4,
    seven: 5,
    eight: 6,
    nine: 7,
    ten: 8,
    jack: 9,
    queen: 10,
    king: 11,
    ace: 12,
};

//frequent variables
var canvas = document.getElementById("canvas");
var userImage;
var backgroundImage;
var back_of_card;
var Cards = new Array(13);
for (var i = 0; i < Cards.length; i++) 
    Cards[i] = new Array(4);
var prom = new Promise(function (resolve, reject) {
    //loading user image
    userImage = document.createElement("img");
    userImage.src = "resources/user.png";

    //loading background image
    backgroundImage = document.createElement("img");
    backgroundImage.src = "resources/poker_table.png";
    backgroundImage.style = "position: absolute; z-index:0;"

    //loading back of card image
    back_of_card = document.createElement("img");
    back_of_card.src = "resources/Cards/back_of_card.png";
    back_of_card.width = CARD_WIDTH;
    back_of_card.height = CARD_HEIGHT;

    var vals = ['2', '3', '4', '5', '6', '7', '8', '9', '10', 'jack', 'queen', 'king', 'ace'];
    var shapes = ['hearts', 'clubs', 'diamonds', 'spades'];
    //loading card images
    for (let i = 0; i < 13; i++)
        for (let j = 0; j < 4; j++) {
            Cards[i][j] = document.createElement("img");
            Cards[i][j].src = '../../resources/Cards/' + vals[i] + '_of_' + shapes[j] + '.png';
            Cards[i][j].width = CARD_WIDTH;
            Cards[i][j].height = CARD_HEIGHT;
        }

    //resolving promises
    resolve();
});



connection.start().then(function () {
    document.getElementById("checkButton").disabled = false;
    prom.then(draw);
}).catch(function (err) {
    return console.error(err.toString());
});


var endTurn = function (event) {
    console.log(event.srcElement.id);
    connection.invoke("EndTurn").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
};


//todo put in for
document.getElementById("checkButton").addEventListener("click", endTurn);
document.getElementById("foldButton").addEventListener("click", endTurn);
document.getElementById("raiseButton").addEventListener("click", endTurn);
document.getElementById("callButton").addEventListener("click", endTurn);

connection.on("ReceiveStatus", function (GameStatus) {
    GameStatus = JSON.parse(GameStatus);
    let Status = GameStatus[0];
    let Players = GameStatus[1];
    let Player = Players.find(x => x.id == connection.connectionId);

    //render play
    if (Player.status == 0) {        //set playing player
        document.getElementById("checkButton").disabled = false;
    }
    else if (Player.status == 1) {  //set Not playing players
        document.getElementById("checkButton").disabled = true;
    }
    else if (Player.status == 2) {  //set Waiting players
        document.getElementById("checkButton").disabled = true;
    }
});

function draw() {
    //loading background image
    canvas.appendChild(backgroundImage);

    loadPlayer(0, Cards[0][0], Cards[0][0]);
    loadPlayer(2, Cards[4][2], Cards[3][2]);
}

function loadPlayer(position_index, card1, card2) {
    //adding user image
    var user = userImage.cloneNode(true);
    user.style = "width : " + USER_SIZE 
        + "px; position: absolute;"
        + "margin-left:" + (PLAYER_POSITIONS[position_index][0] - USER_SIZE / 2) + "px;"
        + "margin-top:" + (PLAYER_POSITIONS[position_index][1] - USER_SIZE / 2) + "px;";
    canvas.appendChild(user);

    //for overload
    if (card1 == undefined || card2 == undefined)
        return;

    loadCard(
        [PLAYER_POSITIONS[position_index][0] - CARD_SPACER, PLAYER_POSITIONS[position_index][1]],
        card1);

    loadCard(
        [PLAYER_POSITIONS[position_index][0] + CARD_SPACER, PLAYER_POSITIONS[position_index][1]],
        card2);
}

function loadCard(position, card) {
    var tmp = card.cloneNode(true);
    tmp.style = "width : " + CARD_WIDTH + ";"
        + "width : " + CARD_HEIGHT + ";"
        + "px; position: absolute;"
        + "margin-left:" + (position[0] - CARD_WIDTH / 2 + CARD_SHIFT) + "px;"
        + "margin-top:" + (position[1] - CARD_HEIGHT / 2) + "px;";
    canvas.appendChild(tmp);
}