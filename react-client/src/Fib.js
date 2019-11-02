import React, { Component } from 'react';
import axios from 'axios';

class Fib extends Component {
    state = {
        seenIndexes: [],
        values: [],
        index: ''
    };

    componentDidMount() {
        this.fetchValues();
        this.fetchIndexes();
    };

    async fetchIndexes() {
        const indexes = await axios.get('/api/numbers/all');
        console.log(indexes);
        this.setState({ seenIndexes: indexes.data });
    };

    async fetchValues() {
        const values = await axios.get('/api/numbers/current');
        console.log(values);
        this.setState({ values: values.data });
    };

    handleSubmit = async event => {
        event.preventDefault();

        await axios.post('/api/numbers', this.state.index, {
            headers: {
                'Content-Type': 'application/json'
            }
        });

        this.setState({index: ''});
    };

    renderSeenIndexes() {
        return this.state.seenIndexes.join(', ');
    };

    renderValues() {
        const entries = [];

        for(let obj of this.state.values) {
            entries.push(
                <div key={obj.Key}>
                    For index {obj.Key} I calculated {obj.Value}
                </div>
            );
        }

        return entries;
    };

    render() {
        return (
            <div>
                <form onSubmit={this.handleSubmit}>
                    <label>Enter your index:</label>
                    <input 
                        value={this.state.index} 
                        onChange={event => this.setState({index: event.target.value})}
                    />
                    <button>Submit</button>
                </form>

                <h3>Indexes I have seen:</h3>
                {this.renderSeenIndexes()}

                <h3>Calculated values:</h3>
                {this.renderValues()}
            </div>
        );
    };
}

export default Fib;