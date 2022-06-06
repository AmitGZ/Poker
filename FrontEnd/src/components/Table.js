import { useState, useEffect } from 'react';
import { Form, Button } from 'react-bootstrap';
import Chat from './Chat';
import images_src from "../resources/index"
import { cardSuits, cardValues } from "./Cards.js"
import { CountdownCircleTimer } from 'react-countdown-circle-timer'
import { CircularProgressbar } from 'react-circular-progressbar';
import 'react-circular-progressbar/dist/styles.css';

const Table = ({ joinRoom, LeaveRoom, sendMessage, connection, messages, roomStatus, user}) => {
    // CONSTANTS
    const WIDTH = 946;
    const HEIGHT = Math.round(WIDTH *0.519);
    const USER_SIZE = 100;
    const POSITIONS = [[WIDTH/2, 400],[184,337],[217,84]];
    POSITIONS[3] = [WIDTH - POSITIONS[2][0] ,POSITIONS[2][1] ];
    POSITIONS[4] = [WIDTH - POSITIONS[1][0] ,POSITIONS[1][1] ];
    const TEXT_OFFSET = 77;
    const PLAYER_NUM = 5;
    const CARD_PROPORTIONS = 6;
    const CARD_WIDTH = 500/ CARD_PROPORTIONS;
    const CARD_HEIGHT = 726/ CARD_PROPORTIONS; 
    const CARD_OFFSET = [120,95];
    const CARD_POSITIONS = [];
    const CARD_SPACINGS = 6;
    for(var i =-2; i<3; i++){
        CARD_POSITIONS.push([WIDTH/2 + (i * (CARD_WIDTH+CARD_SPACINGS)), HEIGHT*17/40]);
    }
    const CHIP_WIDTH = 40;
    const CHIP_HEIGHT = 40; 

    //number of images to load
    var image_num = Object.keys(images_src).length;

    //used to keep track of how many images loaded
    const [loaded_num, setLoadedNum] = useState(0);

    // Asrray of loaded images
    const [loaded_img, setLoadedImg] = useState();

    const [Talking, setTalking] = useState(false);
    const [Dealer, setDealer] = useState(false);

    const [counter, setCounter] = useState(roomStatus.turnTime);

    useEffect(()=>{
        let myInterval = setInterval(() => {
                if (counter > 0 && roomStatus.stage != 0 && roomStatus.stage != 5) {
                    setCounter(counter - 1);
                }
                if (counter === 0) {
                    clearInterval(myInterval)
                } 
            }, 1000)
            return ()=> {
                clearInterval(myInterval);
              };
        });

    // TODO move to user class
    const drawUser = (context, position, user) => {
        // Drawing user image
        context.drawImage(loaded_img['user'],
        POSITIONS[position][0] - USER_SIZE/2,
        POSITIONS[position][1] - USER_SIZE/2,
        USER_SIZE,
        USER_SIZE);

        // Offset used to place cards to the right or left of the player
        var offset = 1
        if(position<3)
            offset = -1;

        // Checking game has started and user has cards
        if(roomStatus.stage > 0 && user.isActive == true){
            // Drawing cards
            if(user.cards.length == 2){
                for(var i = 0; i < 2; i++){
                    context.drawImage(loaded_img[cardValues[user.cards[i].value] + '_of_' + cardSuits[user.cards[i].suit]],
                    POSITIONS[position][0] - CARD_WIDTH/2 + offset* CARD_OFFSET[i],
                    POSITIONS[position][1] - CARD_HEIGHT/2,
                    CARD_WIDTH,
                    CARD_HEIGHT);
                }
            }
            else // Drawing back of cards
            {
                for(var i =0; i < 2; i++){
                    context.drawImage(loaded_img['back_of_card'],
                    POSITIONS[position][0] - CARD_WIDTH/2 + offset* CARD_OFFSET[i],
                    POSITIONS[position][1] - CARD_HEIGHT/2,
                    CARD_WIDTH,
                    CARD_HEIGHT);
                }
            }

            if(user.position == roomStatus.dealerPosition)
            {
                context.drawImage(loaded_img['dealer'],
                POSITIONS[position][0],
                POSITIONS[position][1],
                CHIP_WIDTH,
                CHIP_HEIGHT);
            }
        }

        // Writing User name and money
        context.font = "20px Arial";
        context.fillStyle = "white";
        context.textAlign = "center";
        context.backgroundColor = "white";
        context.fillText(user.username + "\n" + user.moneyInTable+'$' + ' ' + user.moneyInTurn,
        POSITIONS[position][0],
        POSITIONS[position][1] + TEXT_OFFSET
        );
    }

    const drawTableStatus = (context) => {
        // Loading cards on table
        if(roomStatus.cardsOnTable != undefined){ // TODO throw an exception?
            for(var i =0; i < roomStatus.cardsOnTable.length; i++){
                context.drawImage(loaded_img[cardValues[roomStatus.cardsOnTable[i].value] + '_of_' + cardSuits[roomStatus.cardsOnTable[i].suit]],
                CARD_POSITIONS[i][0] - CARD_WIDTH/2,
                CARD_POSITIONS[i][1] - CARD_HEIGHT/2,
                CARD_WIDTH,
                CARD_HEIGHT);
            }
        }

        // Writing pot amount
        context.font = "20px Arial";
        context.fillStyle = "white";
        context.textAlign = "center";
        context.backgroundColor = "white";
        context.fillText(roomStatus.pot +'$',
        WIDTH/2,
        HEIGHT*5/8);
    }

    // Loading all images upon window loading
    useEffect(()=>{
        //loading all assets
        setLoadedImg(()=>{
            var tmp = new Object();
            for(var src in images_src){
                tmp[src] = new Image();
                tmp[src].src = images_src[src];
                tmp[src].onload = ()=>{
                    setLoadedNum(loaded_num => loaded_num+1)
                }
            }
            return tmp;
        });
    },[])

    useEffect(()=>{
        // If not loaded yet return
        if(loaded_num < image_num)
            return;

        // Render
        var ctx = document.getElementById("myCanvas").getContext("2d");
        ctx.drawImage(loaded_img['poker_table'],0,0,WIDTH,HEIGHT)


        for(let i =0; i<roomStatus.users.length; i++){
            drawUser(ctx, (roomStatus.users[i].position - user.position +PLAYER_NUM) % PLAYER_NUM, roomStatus.users[i])
        }

        if(user.position == roomStatus.talkingPosition)
            setTalking(true);
        else
            setTalking(false);

        if(user.position == roomStatus.dealerPosition)
            setDealer(true);
        else
            setDealer(false);

        drawTableStatus(ctx);
        console.log(roomStatus.turnTime);
        setCounter(roomStatus.turnTime);

    },[loaded_num, roomStatus])
    

    return (
    <div>
        <div style = {{width:`${WIDTH}px`, height:`${HEIGHT}px`, position: 'relative'}}>
            <canvas
                style = {{position:'absolute'}}
                id="myCanvas"
                width={WIDTH}
                height={HEIGHT}>
            </canvas>

            <div style={{ position:'absolute', width: 120, height: 120,
            marginLeft :`${POSITIONS[(roomStatus.talkingPosition - user.position +PLAYER_NUM) % PLAYER_NUM][0] - 60}px`,
            marginTop : `${POSITIONS[(roomStatus.talkingPosition - user.position +PLAYER_NUM) % PLAYER_NUM][1] - 60}px`}}>
                { roomStatus.stage != 0 && roomStatus.stage != 5 &&
                 <CircularProgressbar value={counter} maxValue={roomStatus.turnTime} /> }
            </div>

            <div style = {{position:'absolute'}}>
                <Button style = {{position:'absolute'}} variant='danger' onClick={() => LeaveRoom()}>Leave Room</Button>
                    {(roomStatus.stage != 0 && roomStatus.stage != 5) &&
                    <div className='button-list' style = {{marginLeft :`${WIDTH*3/5}px`, marginTop :`${HEIGHT*2/3}px`}}>
                        
                        <Button 
                        disabled = {(!Talking) || (user.moneyInTurn < roomStatus.turnStake)}        
                        variant="dark" key = "Check" 
                        onClick={() =>{connection.invoke("ReceiveCheck")}}>
                        Check
                        </Button>
                        
                        <Button 
                        disabled = {(!Talking)  || (user.moneyInTurn == roomStatus.turnStake)} 
                        variant="dark" key = "Call" 
                        onClick={() => {connection.invoke("ReceiveCall")}}>
                        Call {(roomStatus.turnStake > user.moneyInTurn) ? (roomStatus.turnStake - user.moneyInTurn): null }
                        </Button>
                        
                        <Button 
                        disabled = {(!Talking) || (roomStatus.turnStake >= user.moneyInTable)} 
                        variant="dark" key = "Raise" 
                        onClick={() =>{connection.invoke("ReceiveRaise", 100)}}>
                        Raise
                        </Button>
                        
                        <Button disabled = {(!Talking)}
                        variant="dark" key = "Fold" 
                        onClick={() => {connection.invoke("ReceiveFold")}}>
                        Fold
                        </Button>

                    </div>
                    }
            </div>
        </div>
        <Chat sendMessage={sendMessage} messages={messages} users={roomStatus.users} joinRoom = {joinRoom}/>
    </div>
    )
}

export default Table;