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
        return {followers: []};
    },

    componentDidMount: function () {
        var getFollowers = () => events.emit('get-followers', this.props.userName);

        timer = setInterval(getFollowers, PULL_INTERVAL);
        events.on('followers', (followers) => this.setState(followers));

        getFollowers();
    },

    componentWillUnmount: function () {
        if (timer) {
            clearInterval(timer);
        }
        events.removeListener('followers');
    },

    render: function () {
        var followers = this.state.followers.map((followerName) =>
            <li className="list-group-item" key={followerName}>
                {followerName}
            </li>);

        return (
            <div>
                <h2>Subscriptions</h2>
                <ul>{followers}</ul>
           </div>
        );
           
    }
});