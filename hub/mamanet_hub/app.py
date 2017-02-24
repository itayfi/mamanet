import json
from flask import Flask, jsonify, request, abort
from collections import defaultdict
from expiringdict import ExpiringDict

from config import HOST, PORT, DEBUG


app = Flask('mamanet_hub')
app.debug = DEBUG
cache = dict()


@app.route('/')
def index():
    return jsonify(service='MamaNet Hub', version='1.0.0')


@app.route('/files')
def all_files():
	return json.dumps(cache.keys())


@app.route('/<string:file_md5>', methods=['GET'])
def file_info(file_md5):
    entry = cache.get(file_md5)
    if entry is None:
        abort(404)
    return jsonify(clients=entry.values())


@app.route('/<string:file_md5>', methods=['POST'])
def update_file_info(file_md5):
    data = request.get_json(True)
    cache.setdefault(file_md5, ExpiringDict(max_len=1000, max_age_seconds=10))[request.remote_addr] = {
        'ip': request.remote_addr,
        'port': data.get('port'),
        'availableFileParts': data.get('availableFileParts')
    }
    return json.dumps([f for f in cache[file_md5].values() if f['ip'] != request.remote_addr])


if __name__ == '__main__':
    app.run(host=HOST, port=PORT, debug=DEBUG)