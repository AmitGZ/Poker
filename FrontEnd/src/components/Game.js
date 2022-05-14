import { useState, useEffect} from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import Table from './Table';


const Game = ({ joinRoom, LeaveRoom, rooms, sendMessage, messages, users , createRoom, user}) => {
    return (
        <div>
            {
            (user.roomId == null)?
                <Lobby joinRoom={joinRoom} createRoom = {createRoom} rooms = {rooms} user = {user}/> :
                <Table joinRoom={joinRoom} LeaveRoom = {LeaveRoom} sendMessage = {sendMessage}
                messages = {messages} users = {users} user = {user}/>
            }
        </div>
    )
}

export default Game;