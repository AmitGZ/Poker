import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import background from './resources/background.png'

ReactDOM.render(
  <React.StrictMode>
    <div style={{ zIndex : '-2',backgroundImage: `url(${background})` , height: '100%', width:'100%', position:'absolute'}}/> 
    <div className='background-box'/>
        <App />
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
