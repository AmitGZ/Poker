import React from "react";
import Login from "./Login"
import NewRoomButton from "./NewRoomButton";
import { Button } from "react-bootstrap";

const Rooms = ({ rooms , joinRoom, createRoom, user}) => (
    <div id = "room-list" className='room-list'>
        <h4 id = "availableRooms">Available Rooms</h4>
        <NewRoomButton createRoom = {createRoom} user = {user}/>
        {
            rooms.map(room=>(
                <Button key = {room._id} onClick={() => {joinRoom(room._id)}}>{room._name} {room._numberOfPlayers}/5</Button>
            ))
        }
    </div>
);

export default Rooms;