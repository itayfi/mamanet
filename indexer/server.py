import json
import os
import time
import re
from flask import Flask, Response, request, jsonify

app = Flask(__name__, static_url_path='', static_folder='public')
app.add_url_rule('/', 'root', lambda: app.send_static_file('index.html'))


@app.route('/api/items')
def items_handler():
    with open('items.json', 'r') as f:
        items = json.loads(f.read())
    
    result = []
    
    for h, data in items.iteritems():
        data = dict((k[1:], v) for k, v in data.iteritems())
        data['hash'] = h
        data['downloadName'] = re.sub(r'\.\w+$', '.mamanet', data['fullName'])
        result.append(data)

    return Response(
        json.dumps(result),
        mimetype='application/json',
        headers={
            'Cache-Control': 'no-cache',
            'Access-Control-Allow-Origin': '*'
        }
    )

@app.route('/upload')
def upload():
    data = request.get_json()
    hash = ''.join(map(chr, data['hash'])).encode('hex')
    with open('items.json', 'r') as f:
        items = json.loads(f.read())
    if hash in items:
        return 'File already exists', 400
    items[hash] = data
    with open('items.json', 'w') as f:
        f.write(json.dumps(items, indent=4, separators=(',', ': ')))
    return Response(status=200)


@app.route('/download/<string:hash>/<string:name>')
def download(hash, name):
    with open('items.json', 'r') as f:
        items = json.loads(f.read())
    data = json.dumps(items[hash])
    return Response(data, mimetype="application/octatestream")


if __name__ == '__main__':
    app.run('0.0.0.0', port=int(os.environ.get("PORT", 3000)), debug=True)
