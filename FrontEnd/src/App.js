import { useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import Login from './components/Login';
import Game from './components/Game';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import background from './resources/background.png'

const App = () => {
  const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [users, setUsers] = useState([]);
  const [rooms,setRooms] = useState([]);
  const [user, setUser] = useState({});
  const [page,setPage] = useState('');

  const joinGame = async (username, password) => {
    try {
      //establishing connection
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:44382/poker")
        .configureLogging(LogLevel.Information)
        .build();

        connection.on("RoomStatus", (status) => {
          setPage(status.roomName);
        });

        connection.on("ReceiveMessage", (username, message) => {
          console.log(username );
          setMessages(messages => [...messages, { username, message }]);
        });

        connection.on("AllRoomsStatus", (status) => {
          console.log(status);
          setRooms(status.rooms);
        });

        // On receiving sign in confirmation/rejection
        connection.on("SignInStatus", (status) => {
          if(!status){
            alert("Incorrect username or password");
            return;
          }
          setPage("Lobby");
        });

        // Username and money
        connection.on("UserStatus", (status) => {
        setUser({
          username: status.username,
          money: status.money
          });
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
        await connection.start();
        await connection.invoke("SignIn",  username, password );
        setConnection(connection);
      } catch (e) {
        console.log(e);
      }
  }
  const joinRoom = async (roomId) => {
    try {    
      setMessages([]);      // clearing all messages on room leave  
      await connection.invoke("JoinRoom", roomId);      //invoking join to the new room
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

  return (
    <div style={{ zIndex : '-2',backgroundImage: `url(${background})` , height: '120%', width:'100%', position:'absolute'}}> 
      <div className='bounding-box'>
        <div className='background-gray'/>
        {( Object.keys(user).length !== 0 )&& // Check if user is defined
          <div>
            <h4 style= {{textAlign: "left", position:"absolute", padding:"10px"}}>Hello {user.username}!</h4>
            <h4 style= {{textAlign: "right", padding:"10px"}}>{user.money}$</h4>
          </div>
        }   
        <div className='app'>
          <h2>Poker Online</h2>
          <hr className='line' />
            {!connection ?
              <Login joinGame={joinGame} setPage = {setPage} /> : 
              <Game page ={page} 
                    joinRoom={joinRoom} 
                    rooms = {rooms} 
                    sendMessage = {sendMessage} 
                    messages = {messages} 
                    users = {users}
                    user ={user}
              />
            }
        </div>
      </div>
    </div>
  )
}

export default App;