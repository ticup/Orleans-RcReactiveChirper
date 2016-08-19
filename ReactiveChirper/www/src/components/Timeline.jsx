var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');
var events = require('../lib/events');
var $ = require('jquery');

function formatDate(dateString) {
    var date = new Date(dateString);
    return date.getHours() + ":" + date.getMinutes();
}

module.exports = React.createClass({

    getInitialState: function () {
        return {posts: []};
    },

    componentDidMount: function () {
        events.emit('TimelineSubscribe', this.props.userName);
        events.on('TimelineResult', (timeline) => this.setState(timeline));
    },

    componentWillUnmount: function () {
        events.emit('TimlineUnsubscribe');
    },

    render: function () {
        var posts = this.state.posts.map((post) =>
            <li className="list-group-item" key={post.messageId}>
                [{formatDate(post.timestamp)}] {post.userName} : {post.text}
            </li>);

        return (
            <div>
                <h2>
                    Your Timeline
                </h2>
                <ul className="list-group">
                    {posts}
                </ul>
            </div>
        );
    }
});