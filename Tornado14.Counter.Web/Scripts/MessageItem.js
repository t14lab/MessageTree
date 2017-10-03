
/** @jsx React.DOM */
var Counter = React.createClass({
    getInitialState: function () {
        return { clickCount: 0 };
    },
    handleClick: function () {
        this.setState(function(state) {
            return {clickCount: state.clickCount + 1};
        });
    },
    render: function () {
        return (<li onClick={this.handleClick}>Click me! Number of clicks: {this.state.clickCount}</li>);
}
});



var MessageItem = React.createClass({
    render: function() {
        var name = this.props.product.stocked ?
            this.props.product.name :
            <span style={{color: '#A94442'}}>
                {this.props.product.name}
            </span>;
        return (
          <li className="list-group-item">
            <span className="badge">{this.props.product.price}</span>
{name}
</li>

        );
}
});


