import { useState } from 'react';
import { Form, Button } from 'react-bootstrap';

const Login = ({ joinGame }) => {
    const [user, setUser] = useState();
    const [password, setPassword] = useState();

    return(
        <Form className='login'
            onSubmit={e => {
                e.preventDefault();
                joinGame(user,password);
            }} >
            <Form.Group>
                <Form.Control placeholder="name" onChange={e => setUser(e.target.value)} />
                <Form.Control placeholder="password" onChange={e => setPassword(e.target.value)} />
                <Button variant="success" type="submit" disabled={!user || !password}>Join</Button>
            </Form.Group>
        </Form>)
}

export default Login;