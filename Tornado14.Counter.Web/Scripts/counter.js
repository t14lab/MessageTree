
/** @jsx React.DOM */

var ProductCategoryRow = React.createClass({
    render: function() {
        return (<li className="list-group-item list-group-item-warning">{this.props.category}</li>);
    }
});

var ProductRow = React.createClass({
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

var ProductTable = React.createClass({
    render: function() {
        console.log(this.props);
        var rows = [];
        var lastCategory = null;
        this.props.products.forEach(function(product) {
            if (product.name.indexOf(this.props.filterText) === -1 || (!product.stocked && this.props.inStockOnly)) {
                return;
            }
            if (product.category !== lastCategory) {
                rows.push(<ProductCategoryRow category={product.category} key={product.category} />);
        }
            rows.push(<ProductRow product={product} key={product.name} />);
        lastCategory = product.category;
    }.bind(this));
return (
<ul className="list-group">
    {rows}
</ul>
);
}
});

var SearchBar = React.createClass({
    handleChange: function() {
        this.props.onUserInput(
            this.refs.filterTextInput.getDOMNode().value,
            this.refs.inStockOnlyInput.getDOMNode().checked
        );
    },
    render: function() {
        return (
            <form>
                <div class="form-group">
                <input
                    className="form-control"
                    name="text-basic" 
                    type="text"
                    placeholder="Search..."
                    value={this.props.filterText}
                    ref="filterTextInput"
                    onChange={this.handleChange}
                />
                </div>
                <div class="checkbox"><label>
                    <input
                        type="checkbox"
                        checked={this.props.inStockOnly}
                        ref="inStockOnlyInput"
                        onChange={this.handleChange}
                    />
{' '}
                    Only show products in stock
                </label></div>
            </form>
        );
}
});

var FilterableProductTable = React.createClass({
    getInitialState: function() {
        return {
            filterText: '',
            inStockOnly: false
        };
    },

    handleUserInput: function(filterText, inStockOnly) {
        this.setState({
            filterText: filterText,
            inStockOnly: inStockOnly
        });
    },

    render: function() {
        return (
            <div>
                <SearchBar
                    filterText={this.state.filterText}
                    inStockOnly={this.state.inStockOnly}
                    onUserInput={this.handleUserInput}
                />
                <ProductTable
                    products={this.props.products}
                    filterText={this.state.filterText}
                    inStockOnly={this.state.inStockOnly}
                />
            </div>
        );
}
});


var PRODUCTS = [
  {category: 'Sporting Goods', price: '$49.99', stocked: true, name: 'Football'},
  {category: 'Sporting Goods', price: '$9.99', stocked: true, name: 'Baseball'},
  {category: 'Sporting Goods', price: '$29.99', stocked: false, name: 'Basketball'},
  {category: 'Electronics', price: '$99.99', stocked: true, name: 'iPod Touch'},
  {category: 'Electronics', price: '$399.99', stocked: false, name: 'iPhone 5'},
  {category: 'Electronics', price: '$199.99', stocked: true, name: 'Nexus 7'}
];

React.render(React.createElement(FilterableProductTable, { products: PRODUCTS }), document.getElementById('Content'));

/*
var T14Counter = (function (window, undefined) {

    function getValues() {
        $.ajax({
            cache: false,
            type: "GET",
            timeout: 5000,
            url: "",
            success: function (msg) {
                
            },
            error: function (msg) {
                
            }
        });
    }

    function post() {
        alert('my other method');
    }

    // explicitly return public methods when this object is instantiated
    return {
        getValues: getValues,
        post: post
    };

})(window);
 */

