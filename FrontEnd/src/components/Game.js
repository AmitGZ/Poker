import { useState, useEffect} from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import images from "../resources/index"


const Game = ({ joinRoom, rooms, sendMessage, messages, users ,page}) => {
    const width = 946;
    const height = Math.round(width *0.519);
    useEffect(() => {
        var canvas = document.getElementById("myCanvas");
        if(!canvas) return;
        var ctx = canvas.getContext("2d");
        var img = new Image();
        img.src = images.poker_table;
        img.onload = ()=> {
            ctx.drawImage(img,0,0,width,height)
        }
      }, [page]);

    return (
        <div>
            {
            (page ===  "Lobby")?
                <Lobby joinRoom={joinRoom} rooms = {rooms}/> :
                <div>
                    <div className='leave-room'>
                        <Button variant='danger' onClick={() => joinRoom("Lobby")}>Leave Room</Button>
                    </div>
                    <canvas
                      id="myCanvas"
                      width="1000"
                      height="519">
                    </canvas>
                    <Chat sendMessage={sendMessage} messages={messages} users={users} joinRoom = {joinRoom}/>
                </div>
            }
        </div>
    )
}

export default Game;