import { useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import Login from './components/Login';
import Game from './components/Game';
import UserStats from './components/UserStats';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import background from './resources/background.png'
import { ConsoleLogger } from '@microsoft/signalr/dist/esm/Utils';

const App = () => {
  const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);
  const [rooms,setRooms] = useState([]);
  const [user, setUser] = useState({});

  const joinGame = async (username, password) => {
    try {
      //establishing connection
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:44382/poker")
        .configureLogging(LogLevel.Information)
        .build();

        connection.on("RoomStatus", (room) => {
          setUsers(room._players);
          // Add functionality
        });

        connection.on("ReceiveMessage", (username, message) => {
          console.log(username);
          setMessages(messages => [...messages, { username, message }]);
        });

        connection.on("AllRoomsStatus", (status) => {
          setRooms(status._rooms);
        });

        // On receiving sign in confirmation/rejection
        connection.on("Alert", (status) => {
          alert(status);
          return;
        });

        // Username and money
        connection.on("UserStatus", (status) => {
        setUser({
            username: status._username,
            money: status._money,
            roomId: status._roomId,
            position: status._position
          });
        });
        
        //resetting all hooks
        connection.onclose(e => {
          setConnection();
          setMessages([]);
          setUsers([]);
          setUser({});
          setRooms([]);
        });

        //on initial connect move to Lobby
        await connection.start();
        await connection.invoke("SignIn",  username, password );
        setConnection(connection);
      } catch (e) {
        console.log(e);
      }
  }

  const joinRoom = async (roomId, enterMoney) => {
    try {    
      setMessages([]);      // clearing all messages on room leave  
      await connection.invoke("JoinRoom", roomId, enterMoney);      //invoking join to the new room
    } catch (e) {
      console.log(e);
    }
  }

  const createRoom = async (roomName, enterMoney) => {
    try {
      await connection.invoke("createRoom", roomName, parseInt(enterMoney));      //invoking join to the new room
    } catch (e) {
      console.log(e);
    }
  }

  const sendMessage = async (message) => {
    try {
      await connection.invoke("SendMessage", message);      //invoking send message
    } catch (e) {
      console.log(e);
    }
  }

  //TODO implement
  const closeConnection = async () => {
    try {
      connection.closeConnection();
      setConnection('');
    } catch (e) {
      console.log(e);
    }
  }

  const LeaveRoom = async () => {
     //invoking send message
    await connection.invoke("LeaveRoom");   
    try {
    } catch (e) {
      console.log(e);
    }
  }

  return (
    <div style={{ zIndex : '-2',backgroundImage: `url(${background})` , height: '120%', width:'100%', position:'absolute'}}> 
      <div className='bounding-box'>
        <div className='background-gray'/>
        {connection && <UserStats user= {user}/>}
        <div className='app'>
          <h2>Poker Online</h2>
          <hr className='line' />
            {!connection ?
              <Login joinGame={joinGame} /> : 
              <Game joinRoom={joinRoom} 
                    rooms = {rooms} 
                    sendMessage = {sendMessage} 
                    messages = {messages} 
                    users = {users}
                    user ={user}
                    createRoom = {createRoom}
                    LeaveRoom = {LeaveRoom}
              />
            }
        </div>
      </div>
    </div>
  )
}

export default App;