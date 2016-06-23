var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');
var $ = require('jquery');

var timer;
var PULL_INTERVAL = 10000;

function formatDate(dateString) {
    var date = new Date(dateString);
    return date.getHours() + ":" + date.getMinutes();
}

module.exports = React.createClass({

    getInitialState: function () {
        return {posts: []};
    },

    componentDidMount: function () {
        var getTimeline = () => {
            $.get('/timeline/' + this.props.username, (data) => {
                console.log("received timeline ");
                console.log(data);
                this.setState(data);
            }, "json");
        }
        timer = setInterval(getTimeline, PULL_INTERVAL);
        getTimeline();
    },

    componentWillUnmount: function () {
        if (timer) {
            clearInterval(timer);
        }
    },

    render: function () {
        var posts = this.state.posts.map((post) => <li key={post.messageId}> [{formatDate(post.timestamp)}] {post.userName} : {post.text}</li>);
        return <ul>
            {posts}
        </ul>
    }
});