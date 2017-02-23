import json
import os
import time
from flask import Flask, Response, request

app = Flask(__name__, static_url_path='', static_folder='public')
app.add_url_rule('/', 'root', lambda: app.send_static_file('index.html'))


@app.route('/api/items', methods=['GET', 'POST', 'PUT'])
def items_handler():
    with open('items.json', 'r') as f:
        items = json.loads(f.read())

    if request.method in ('POST', 'PUT'):
        new_item = request.json
        old_item = [item for item in items if item['hash'] == new_item['hash']]
        if len(old_item) > 0:
            old_item[0].update(new_item)
        else:
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
