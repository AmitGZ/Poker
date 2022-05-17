import { useState, useEffect } from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import images_src from "../resources/index"


const Table = ({ joinRoom, LeaveRoom, sendMessage, messages, users, user}) => {
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
    const CARD_OFFSET = [95,120];
    const CARD_POSITIONS = [];
    const CARD_SPACINGS = 6;
    for(var i =-2; i<3; i++){
        CARD_POSITIONS.push([WIDTH/2 + (i * (CARD_WIDTH+CARD_SPACINGS)), HEIGHT*9/20]);
    }

    //number of images to load
    var image_num = Object.keys(images_src).length;

    //used to keep track of how many images loaded
    const [loaded_num, setLoadedNum] = useState(0);

    // Asrray of loaded images
    const [loaded_img, setLoadedImg] = useState();

    const drawUser = (context, position, user) => {
        // Drawing user image
        context.drawImage(loaded_img['user'],
        POSITIONS[position][0] - USER_SIZE/2,
        POSITIONS[position][1] - USER_SIZE/2,
        USER_SIZE,
        USER_SIZE);

        var offset = 1
        if(position<3)
            offset = -1;

        // Drawing cards
        for(var i =0; i<2; i++){
            context.drawImage(loaded_img['ten_of_hearts'],
            POSITIONS[position][0] - CARD_WIDTH/2 + offset* CARD_OFFSET[i],
            POSITIONS[position][1] - CARD_HEIGHT/2,
            CARD_WIDTH,
            CARD_HEIGHT);
        }

        // Writing User name and money
        context.font = "20px Arial";
        context.fillStyle = "white";
        context.textAlign = "center";
        context.backgroundColor = "white";
        context.fillText(user.username + "\n" + user.moneyInTable+'$',
        POSITIONS[position][0],
        POSITIONS[position][1] + TEXT_OFFSET
        );
    }
    const drawTableCards = (context) => {
        // Loading cards on table
        for(var i =0; i<5; i++){
            context.drawImage(loaded_img['ace_of_spades'],
            CARD_POSITIONS[i][0] - CARD_WIDTH/2,
            CARD_POSITIONS[i][1] - CARD_HEIGHT/2,
            CARD_WIDTH,
            CARD_HEIGHT);
        }
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


        for(let i =0; i<users.length; i++){
            drawUser(ctx, (users[i].position - user.position +PLAYER_NUM) % PLAYER_NUM, users[i])
        }

        drawTableCards(ctx);

    },[loaded_num, users])

    return (
    <div>
        <div style = {{width:`${WIDTH}px`, height:`${HEIGHT}px`, position: 'relative'}}>
            <canvas
                style = {{position:'absolute'}}
                id="myCanvas"
                width={WIDTH}
                height={HEIGHT}>
            </canvas>
            <div style = {{position:'absolute'}}>
                <Button style = {{position:'absolute'}} variant='danger' onClick={() => LeaveRoom()}>Leave Room</Button>
                <div className='button-list' style = {{marginLeft :`${WIDTH*3/5}px`, marginTop :`${HEIGHT*2/3}px`}}>
                    <Button  variant="dark" key = "Check" onClick={() =>{console.log("Check")}}>Check</Button>
                    <Button  variant="dark" key = "Call" onClick={() => {console.log("Call")}}>Call</Button>
                    <Button  variant="dark" key = "Raise" onClick={() =>{console.log("Raise")}}>Raise</Button>
                    <Button  variant="dark" key = "Fold" onClick={() => {console.log("Fold")}}>Fold</Button>
                </div>
            </div>
        </div>
        <Chat sendMessage={sendMessage} messages={messages} users={users} joinRoom = {joinRoom}/>
    </div>
    )
}

export default Table;