var $ = require('jquery');
var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('./lib/routie');
var events = require('./lib/events');

var Login = require('./components/Login.jsx');
var User = require('./components/User.jsx');
var ThemeButtons = require('./components/theme-buttons.jsx');

var DomContainer = document.getElementById('content');

ReactDOM.render(<ThemeButtons/>, document.getElementById('button-toggles-content'));

// TODO: username <-> userName

/* Eventhing */
events.on('get-timeline', (username) =>
    $.get('/timeline/' + username, (timeline) => {
        console.log("got new timeline");
        console.log(timeline);
        events.emit('timeline', timeline);
    }, "json"));

events.on('get-followers', (username) =>
    $.get('/followers/' + username, (followers) => {
        events.emit('followers', followers);
    }, "json"));

events.on('follow', (username, toFollow) => {
    console.log("following");
    console.log({ username, toFollow });
    $.post('/follow', { username: username, toFollow: toFollow }, (followers) => {
        events.emit('get-followers', username);
        events.emit('get-timeline', username);
    }, "json")
});

events.on('new-message', (username, text) => {
    console.log("sending message");
    $.post('/message/new', { username, text }, (data) => {
        console.log("message posted");
        console.log(data);
        // get the new timeline to incorporate the new post
        events.emit('get-timeline', username);
    }, "json")
});

/* Routing */
// Login
routie('', function () {
    console.log("loading login page");
    React.render(<Login />, DomContainer);
});

// User page (timeline/followers)
routie('/user/:username', function (username) {
    console.log("arrived at user page for " + username);
    ReactDOM.render(<User userName={username } />, DomContainer);
});

// Go to login page
routie('');