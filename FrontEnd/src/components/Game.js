import { useState, useEffect} from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import Table from './Table';


const Game = ({ joinRoom, LeaveRoom, rooms, sendMessage, messages, users , createRoom, user}) => {
    console.log(user);
    return (
        <div>
            <div>
                <h4 style= {{textAlign: "left", position:"absolute", padding:"10px"}}>Hello {user.username}!</h4>
                <h4 style= {{textAlign: "right", padding:"10px"}}>{user.money}$</h4>
            </div>
            {
            (user.roomId == null)?
                <Lobby joinRoom={joinRoom} createRoom = {createRoom} rooms = {rooms} user = {user}/> :
                <Table joinRoom={joinRoom} LeaveRoom = {LeaveRoom} sendMessage = {sendMessage} messages = {messages} users = {users}/>
            }
        </div>
    )
}

export default Game;