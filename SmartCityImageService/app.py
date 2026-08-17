import tornado.ioloop
import tornado.web

from Utils.add_detection_to_image import AddDetectionToImage


def make_app():
    return tornado.web.Application([
        ("/v1/adddetectionstoimage", AddDetectionToImage),
    ])


if __name__ == "__main__":
    app = make_app()
    app.listen(8888)
    tornado.ioloop.IOLoop.current().start()
