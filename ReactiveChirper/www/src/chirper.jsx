global.jQuery = require('jquery');
global.$ = global.jQuery;
var sr = require('./lib/jquery.signalR-2.2.1.min.js');
var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('./lib/routie');
var events = require('./lib/events');

var WebSocketClient = require('./WebSocketClient.js');

var Login = require('./components/Login.jsx');
var User = require('./components/User.jsx');
var ThemeButtons = require('./components/theme-buttons.jsx');

var DomContainer = document.getElementById('content');

ReactDOM.render(<ThemeButtons/>, document.getElementById('button-toggles-content'));

// TODO: username <-> userName

/* UI Routing */
// Login
routie('', function () {
    console.log("loading login page");
    ReactDOM.render(<Login />, DomContainer);
});

// User page (timeline/followers)
routie('/user/:username', function (username) {
    console.log("arrived at user page for " + username);
    ReactDOM.render(<User userName={username} />, DomContainer);
});

/* Event Handling - Server Interaction */
//var host = "ws://localhost:8081/ws";

// Declare a proxy to reference the hub.
//$.connection.hub.url = "http://localhost:8081/signalr";
var proxy = $.connection.chirperHub;
var hub = proxy.server;

proxy.client.TimelineResult = (timeline) => events.emit('TimelineResult', timeline);

proxy.client.FollowerResult = (followers) => events.emit('FollowerResult', followers);

connection.start()
    .done(function () { console.log('Now connected, connection ID=' + connection.id); })
    .fail(function () { console.log('Could not connect'); });

events.on('TimelineSubscribe', (username) =>
    hub.TimelineSubscribe(username));

events.on('TimelineUnsubscribe', (username) =>
    hub.TimelineUnsubscribe(username));

events.on('FollowerSubscribe', (username) =>
    hub.FollowerSubscribe(username));

events.on('FollowerUnsubscribe', (username) =>
    hub.FollowerUnsubscribe(username));


events.on('Follow', (username, toFollow) => {
    console.log("following");
    console.log({ username, toFollow });
    hub.Follow(Username, ToFollow);
});

events.on('NewMessage', (username, text) => {
    console.log("sending message");
    hub.NewMessage(username, text);
});

// Go to login page
routie('');