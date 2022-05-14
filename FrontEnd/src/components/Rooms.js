import { Button } from "react-bootstrap";

const Rooms = ({ rooms , joinRoom , setPage}) => (
    <div className='room-list'>
        <h4>Available Rooms</h4>
        <Button variant="primary" key = "new" onClick={() => {joinRoom("new")}}>New Room</Button>
        {

            rooms.map(room=>(
                <Button key = {room._id} onClick={() => {joinRoom(room._id)}}>{room._name} {room._numberOfPlayers}/5</Button>
            ))
        }
    </div>
);

export default Rooms;