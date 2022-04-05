"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/pokerHub").build();

connection.start().then(function () {
    document.getElementById("checkButton").disabled = true;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("checkButton").addEventListener("click",
    function (event) {
        connection.invoke("EndTurn").catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });

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


connection.on("ReceiveStatus", function (players) {
    players = JSON.parse(players)
    var li = document.getElementById("userList");
    li.textContent = '';
    for (let i = 0; i < players.length; i++)
        li.textContent += `\nid = ${players[i].id}\n`;
    document.getElementById("messagesList").appendChild(li);
});