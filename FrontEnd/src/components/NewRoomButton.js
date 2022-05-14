import PropTypes from 'prop-types';
import { useState,useEffect } from 'react';
import { Button } from 'react-bootstrap';
import Login from './Login';
import NewRoomForm from "./NewRoomForm"


const NewRoomButton = ({createRoom, user, enterMoney}) => {
    let show ={
        state: true,
            color:'green',
            text:'New Room'
        }

    let hide ={
        state: false,
            color:'red',
            text:'Hide'
        }
        
    const [toggle,setToggle] = useState(show)

    function onClick(){
        setToggle(toggle.state?hide:show)
        console.log(user)
    }

    return (
    <div>
        <Button 
        variant="primary" 
        key = "new"
        onClick = {onClick}
        style = {{backgroundColor : toggle.color, width: "100%"}}
        className = 'btn' > {toggle.text}
        </Button>

        {toggle.state || <NewRoomForm createRoom = {createRoom} user = {user} enterMoney = {enterMoney}/>}
    </div>
    );
};

export default NewRoomButton;
  