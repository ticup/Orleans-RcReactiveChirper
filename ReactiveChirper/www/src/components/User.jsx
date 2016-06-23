var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');

var Timeline = require('./Timeline.jsx');
var PostMessage = require('./PostMessage.jsx');
var FollowerList = require('./FollowerList.jsx');
var NewFollower = require('./NewFollower.jsx');

module.exports = React.createClass({

    render: function () {
        return <div className="container-fluid">
           <div className="row">
               <div className="col-md-9">
                    <Timeline username={this.props.username}/>
                    <PostMessage username={this.props.username} />
               </div>
                <div className="col-md-3">
                    <FollowerList username={this.props.username} />
                    <NewFollower username={this.props.username} />
                </div>
            </div>
           
        </div>
    }
});