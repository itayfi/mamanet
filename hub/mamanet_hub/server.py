from tornado.wsgi import WSGIContainer
from tornado.ioloop import IOLoop
from tornado.httpserver import HTTPServer

from app import app
from config import PORT

if __name__ == "__main__":
    http_server = HTTPServer(WSGIContainer(app))

    http_server.listen(PORT)
    IOLoop.instance().start()
