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
        return {followers: []};
    },

    componentDidMount: function () {
        events.emit('FollowerSubscribe', this.props.userName);
        events.on('FollowerResult', (followers) => this.setState(followers));
    },

    componentWillUnmount: function () {
        events.emit('FollowerUnsubscribe');
    },

    render: function () {
        var followers = this.state.followers.map((followerName) =>
            <li className="list-group-item" key={followerName}>
                {followerName}
            </li>);

        return (
            <div>
                <h2>Subscriptions</h2>
                <ul className="list-group">{followers}</ul>
           </div>
        );
           
    }
});