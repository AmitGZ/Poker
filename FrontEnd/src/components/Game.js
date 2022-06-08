import { useState, useEffect} from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import Table from './Table';


const Game = ({ joinRoom, LeaveRoom, rooms, sendMessage, messages, roomStatus , createRoom, user, connection}) => {

    return (
        <div>
            {
            (user.roomId == null)?
                <Lobby joinRoom={joinRoom} createRoom = {createRoom} rooms = {rooms} user = {user}/> :
                <Table joinRoom={joinRoom} connection = {connection} LeaveRoom = {LeaveRoom} sendMessage = {sendMessage}
                messages = {messages} roomStatus = {roomStatus} user = {user}/>
            }
        </div>
    )
}

export default Game;