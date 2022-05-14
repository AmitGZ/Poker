import React from "react";
import Login from "./Login"
import NewRoomButton from "./NewRoomButton";
import { Button } from "react-bootstrap";
import Slider from 'react-input-slider';
import { useState } from "react";

const UserStats = ({ user }) => {
    return(
        <div>
        <h4 style= {{textAlign: "left", position:"absolute", padding:"10px"}}>Hello {user.username}!</h4>
        <h4 style= {{textAlign: "right", padding:"10px"}}>{user.money}$</h4>
        </div>
        );
};

export default UserStats;
