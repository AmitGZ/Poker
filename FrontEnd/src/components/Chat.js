import SendMessageForm from './SendMessageForm';
import MessageContainer from './MessageContainer';
import ConnectedUsers from './ConnectedUsers';
import { Button } from 'react-bootstrap';

const Chat = ({ sendMessage, messages, users, closeConnection, joinRoom, page}) => (
    <div>
        <ConnectedUsers users={users} page = {page} />
        <div className='chat'>
            <MessageContainer messages={messages} />
            <SendMessageForm sendMessage={sendMessage} />
        </div>
    </div>
);

export default Chat;