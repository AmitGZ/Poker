import { useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import Lobby from './components/Lobby';
import Login from './components/Login';
import Chat from './components/Chat';
import Game from './components/Game';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import background from './resources/background.png'

const App = () => {
  const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);
  const [rooms,setRooms] = useState([]);
  const [page,setPage] = useState('');

  const joinGame = async (user) => {
    try {
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:44382/poker")
        .configureLogging(LogLevel.Information)
        .build();
  
        //on update from server
        connection.on("UsersInRoom", (users) => {
          setUsers(users);
        });

        connection.on("ReceivePage", (page) => {
          setPage(page);
        });

        connection.on("ReceiveRooms", (rooms) => {
          setRooms(rooms);
        });

        connection.on("ReceiveMessage", (user, message) => {
          setMessages(messages => [...messages, { user, message }]);
        });
        
        //resetting all hooks
        connection.onclose(e => {
          setConnection();
          setMessages([]);
          setUsers([]);
          setRooms([]);
          setPage();
        });
  
        //on initial connect move to Lobby
        let room ="Lobby"
        setPage(room)
        await connection.start();
        await connection.invoke("JoinGame", { user, room });
        setConnection(connection);
      } catch (e) {
        console.log(e);
      }
  }
  const joinRoom = async (room) => {
    try {
      setMessages([]) // clearing all messages on room leave
      await connection.invoke("JoinRoom", room);
    } catch (e) {
      console.log(e);
    }
  }

  const sendMessage = async (message) => {
    try {
      await connection.invoke("SendMessage", message);
    } catch (e) {
      console.log(e);
    }
  }

  const closeConnection = async () => {
    try {
      connection.closeConnection();
      setConnection('');
    } catch (e) {
      console.log(e);
    }
  }

  return (
    <div style={{ zIndex : '-2',backgroundImage: `url(${background})` , height: '120%', width:'100%', position:'absolute'}}> 
      <div className='bounding-box'>
        <div className='background-gray'/>
        <div className='app'>
          <h2>Poker Online</h2>
          <hr className='line' />
            {!connection ?
              <Login joinGame={joinGame} setPage = {setPage} /> : 
              <Game page ={page} joinRoom={joinRoom} rooms = {rooms} sendMessage = {sendMessage} messages = {messages} users = {users}/>
            }
        </div>
      </div>
    </div>
  )
}

export default App;