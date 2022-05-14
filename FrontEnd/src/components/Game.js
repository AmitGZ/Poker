import { useState, useEffect} from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import Table from './Table';


const Game = ({ joinRoom, rooms, sendMessage, messages, users ,page, createRoom, user}) => {
    return (
        <div>
            {
            (page ===  "Lobby")?
                <Lobby joinRoom={joinRoom} createRoom = {createRoom} rooms = {rooms} user = {user}/> :
                <Table joinRoom={joinRoom} sendMessage = {sendMessage} messages = {messages} users = {users}/>
            }
        </div>
    )
}

export default Game;