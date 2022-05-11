import { Button } from "react-bootstrap";

const Rooms = ({ rooms , joinRoom , setPage}) => (
    <div className='room-list'>
        <h4>Available Rooms</h4>
        <Button variant="primary" key = "new" onClick={() => {joinRoom("new")}}>New Room</Button>
        {

            rooms.map(room=>(
                <Button key = {room.roomId} onClick={() => {joinRoom(room.roomName)}}>{room.roomName} {room.numberOfPlayers}/5</Button>
            ))
        }
    </div>
);

export default Rooms;