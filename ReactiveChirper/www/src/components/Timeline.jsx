var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');
var events = require('../lib/events');
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
        var getTimeline = () => events.emit('get-timeline', this.props.userName);

        timer = setInterval(getTimeline, PULL_INTERVAL);
        events.on('timeline', (timeline) => this.setState(timeline));

        getTimeline();
    },

    componentWillUnmount: function () {
        if (timer) {
            clearInterval(timer);
        }
        events.removeListener('timeline', setTimeline);
    },

    render: function () {
        var posts = this.state.posts.map((post) =>
            <li className="list-group-item" key={post.messageId }>
                [{formatDate(post.timestamp)}] {post.userName} : {post.text}
            </li>);

        return <ul>
            {posts}
        </ul>
    }
});