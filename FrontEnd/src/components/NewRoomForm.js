import { useState } from 'react';
import { Form, Button } from 'react-bootstrap';
import Slider from 'react-input-slider';

const NewRoomForm = ({ createRoom, user }) => {
    const [roomName, setRoomName] = useState();
    const [money, setMoney] = useState({ x: 10 });

    return(
        <Form className='login'
            onSubmit={e => {
                e.preventDefault();
                createRoom(roomName, money.x);
            }} >
            <Form.Group>
                <Form.Control placeholder="Room Name" onChange={e => setRoomName(e.target.value)} />
                <div>
                    <label> {money.x} $ </label>
                    <Slider xmin = {user.money/10} xmax = {user.money} x={money.x} onChange={({ x }) => setMoney(money => ({ ...money, x }))} />
                </div>
                <Button variant="success" type="submit" disabled={!roomName}>Join Room</Button>
            </Form.Group>
        </Form>)
}

export default NewRoomForm;