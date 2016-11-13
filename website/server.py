# This file provided by Facebook is for non-commercial testing and evaluation
# purposes only. Facebook reserves all rights not expressly granted.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
# FACEBOOK BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
# ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
# WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

import json
import os
import time
from flask import Flask, Response, request

app = Flask(__name__, static_url_path='', static_folder='public')
app.add_url_rule('/', 'root', lambda: app.send_static_file('index.html'))


@app.route('/api/items', methods=['GET', 'POST'])
def items_handler():
    with open('items.json', 'r') as f:
        items = json.loads(f.read())

    if request.method == 'POST':
        new_item = request.form.to_dict()
        new_item['id'] = int(time.time() * 1000)
        items.append(new_item)

        with open('items.json', 'w') as f:
            f.write(json.dumps(items, indent=4, separators=(',', ': ')))

    return Response(
        json.dumps(items),
        mimetype='application/json',
        headers={
            'Cache-Control': 'no-cache',
            'Access-Control-Allow-Origin': '*'
        }
    )

@app.route('/api/items/<string:hash_>', methods=['PUT'])
def item_change_handler(hash_):
    with open('items.json', 'r') as f:
        items = json.loads(f.read())
    for item in items:
        if item['hash'] == hash_:
            item.update(request.form.to_dict())
    with open('items.json', 'w') as f:
        f.write(json.dumps(items, indent=4, separators=(',', ': ')))
    return Response(status=200)



if __name__ == '__main__':
    app.run(port=int(os.environ.get("PORT", 3000)), debug=True)
