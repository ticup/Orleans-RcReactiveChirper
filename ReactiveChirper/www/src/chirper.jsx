var http = require('./lib/http');
var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('./lib/routie');
var events = require('eventthing');

var Login = require('./components/Login.jsx');
var User = require('./components/User.jsx');
var ThemeButtons = require('./components/theme-buttons.jsx');

var timer;

var DomContainer = document.getElementById('content');

ReactDOM.render(<ThemeButtons/>, document.getElementById('button-toggles-content'));

// continually poll the timeline
function loadDashboardCounters(){
    http.get('/timeline', function(err, data){
        dashboardCounters = data;
        events.emit('timeline', data);
    });
}
//setInterval(loadDashboardCounters, 5000);
//loadDashboardCounters();


function renderLoading(){
    ReactDOM.render(<span>Loading...</span>, DomContainer);
}

function resetPull() {
    events.clearAll();
    clearInterval(timer);
}

routie('', function () {
    console.log("loading login page");
    resetPull();
    React.render(<Login />, DomContainer);

    //var render = function(){
    //    React.render(<Login />, DOMcontainer);
    //}

    // events.on('dashboard-counters', render);

    //loadDashboardCounters();
});



routie('/user/:username', function (username) {
    console.log("arrived at user page for " + username);
    resetPull();
    ReactDOM.render(<User username={username} />, DomContainer);

        // http.get('/HistoricalStats/' + host, function(err, data) {

    //timer = setInterval(loadData, 5000);

    //loadData();
});

routie('');