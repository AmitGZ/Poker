import { useState } from 'react';
import { Form, Button } from 'react-bootstrap';

const NewRoomForm = ({ createRoom, user , enterMoney}) => {
    const [roomName, setRoomName] = useState();

    return(
        <Form className='login'
            onSubmit={e => {
                e.preventDefault();
                createRoom(roomName, enterMoney);
            }} >
            <Form.Group>
                <Form.Control placeholder="Room Name" onChange={e => setRoomName(e.target.value)} />
                <Button variant="success" type="submit" disabled={!roomName}>Join Room</Button>
            </Form.Group>
        </Form>)
};

export default NewRoomForm;