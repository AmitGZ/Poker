import { useState, useEffect } from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import images_src from "../resources/index"


const Table = ({ joinRoom, sendMessage, messages, users, page}) => {
    //setting width and height for playing canvas
    const WIDTH = 946;
    const HEIGHT = Math.round(WIDTH *0.519);

    //number of images to load
    var image_num = Object.keys(images_src).length;

    //used to keep track of how many images loaded
    const [loaded_num, setLoadedNum] = useState(0);

    //array of loaded images
    const [loaded_img, setLoadedImg] = useState();

    //loading all images upon window loading
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
        //if not loaded yet return
        if(loaded_num < image_num)
            return;

        //render
        var ctx = document.getElementById("myCanvas").getContext("2d");
        ctx.drawImage(loaded_img['poker_table'],0,0,WIDTH,HEIGHT)
    },[loaded_num])

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
                <Button style = {{position:'absolute'}} variant='danger' onClick={() => joinRoom("Lobby")}>Leave Room</Button>
                <div className='button-list' style = {{marginLeft :`${WIDTH*3/5}px`, marginTop :`${HEIGHT*2/3}px`}}>
                    <Button  variant="dark" key = "Check" onClick={() =>{console.log("Check")}}>Check</Button>
                    <Button  variant="dark" key = "Call" onClick={() => {console.log("Call")}}>Call</Button>
                    <Button  variant="dark" key = "Raise" onClick={() =>{console.log("Raise")}}>Raise</Button>
                    <Button  variant="dark" key = "Fold" onClick={() => {console.log("Fold")}}>Fold</Button>
                </div>
            </div>
        </div>
        <Chat sendMessage={sendMessage} messages={messages} users={users} joinRoom = {joinRoom} page = {page}/>
    </div>
    )
}

export default Table;