      var ListItem = React.createClass({
        calcSize: function (size) {
          if (size < 1024) {
            return `${size} B`;
          }
          size = Math.round(size / 1024);
          if (size < 1024) {
            return `${size} KB`;
          }
          size = Math.round(size / 1024);
          if (size < 1024) {
            return `${size} MB`;
          }
          size = Math.round(size / 1024);
          return `${size} GB`;
        },
        render: function () {
          var description, size;
          description = <div className="description">{this.props.children}</div>;
          size = this.calcSize(this.props.size);
          return <li className="item">
            <strong>{this.props.name}</strong>
            <div className="item-hash">{this.props.hash}</div>
            {description}
            <ul className="inline-list">
              <li>
                  <a href={"/download/" + this.props.hash + "/" + this.props.downloadName}>
                      <i className="fa fa-download"></i>
                      <span>Download</span>
                  </a>
              </li>
              <li>
                  <span className="item-size"> ({size})</span>
              </li>
            </ul>
          </li>;
        }
      });
      var List = React.createClass({
        loadItemsFromServer: function () {
          var self = this;
          $.getJSON('/api/items', function (items) {
            self.setState({data: items});
          });
        },
        getInitialState: function () {
          return {data: []}
        },
        componentDidMount: function () {
          this.loadItemsFromServer();
          setInterval(this.loadItemsFromServer, this.props.pollInterval || 2000);
        },
        render: function () {
          var data = this.state.data;
          var self = this;
          if (this.props.filter != '') {
            var regex = new RegExp(this.props.filter.replace(/\s+/g, '|'), 'i');
            data = data.filter(function (item) {
              return regex.test(item.fullName) || regex.test(item.description);
            });
          }
          var items = data.map(function (item) {
            var changed = function (diff) {
              item.description = diff.description;
              $.ajax({
                url: '/api/items/' + item.hash,
                method: 'PUT',
                data: item
              });
            };
            return <ListItem key={item.hash} hash={item.hash} size={item.size} name={item.fullName} downloadName={item.downloadName} onChange={changed}>{item.description}</ListItem>
          });
          if (items.length == 0) {
            return <div className="no-results">No results found</div>;
          }
          return <ul className="list">
            {items}
          </ul>;
        }
      });
      var ListFilter = React.createClass({
        getInitialState: function () {
          return {filter: ''};
        },
        handleFilterChange: function (e) {
          this.setState({filter: e.target.value});
        },
        render: function () {
          return <div>
            <input id="filter-box" type="text" value={this.state.filter} onChange={this.handleFilterChange} placeholder="Filter" />
            <List url={this.props.url} filter={this.state.filter} />
          </div>;
        }
      });
      ReactDOM.render(
        <div>
          <h1>MamanNet Indexer</h1>
          <ListFilter url="/api/items" />
        </div>,
        document.getElementById('content')
        );
