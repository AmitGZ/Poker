import { useState } from 'react';
import { Form, Button } from 'react-bootstrap';
import ConnectedUsers from './ConnectedUsers';
import Rooms from './Rooms';

const Lobby = ({ joinRoom, rooms, createRoom, user}) => {

    return (
        <div className='available-rooms'>
            <Rooms rooms = {rooms} joinRoom = {joinRoom} createRoom = {createRoom} user = {user} />
        </div>
    )
}

export default Lobby;