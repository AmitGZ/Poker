import { useState, useEffect} from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';
import Chat from './Chat';
import Lobby from './Lobby';
import Table from './Table';


const Game = ({ joinRoom, rooms, sendMessage, messages, users ,page}) => {
    
    return (
        <div>
            {
            (page ===  "Lobby")?
                <Lobby joinRoom={joinRoom} rooms = {rooms}/> :
                <Table joinRoom={joinRoom} sendMessage = {sendMessage} messages = {messages} users = {users}  page = {page}/>
            }
        </div>
    )
}

export default Game;