import json
from flask import Flask, jsonify, request
from collections import defaultdict
from expiringdict import ExpiringDict

from config import HOST, PORT, DEBUG


app = Flask('mamanet_hub')
app.debug = DEBUG
cache = defaultdict(lambda: ExpiringDict(max_len=1000, max_age_seconds=60))


@app.route('/')
def index():
    return jsonify(service='MamaNet Hub', version='1.0.0')


@app.route('/<string:file_md5>', methods=['GET'])
def file_info(file_md5):
    return jsonify(clients=cache[file_md5].values())


@app.route('/<string:file_md5>', methods=['POST'])
def update_file_info(file_md5):
    data = request.get_json(True)
    cache[file_md5][request.remote_addr] = {
        'ip': request.remote_addr,
        'port': data.get('port'),
        'availableFileParts': data.get('availableFileParts')
    }
    return json.dumps(cache[file_md5].values())


if __name__ == '__main__':
    app.run(host=HOST, port=PORT, debug=DEBUG)