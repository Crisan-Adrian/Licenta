import os
import signal
from flask import Flask, make_response

app = Flask(__name__)

# primitiveDict={
#     '0': 0,
#     '1': 0.25,
#     '2': 0.5,
#     '3': 1,
#     '4': 1.5,
#     '5': -0.25,
#     '6': -0.5,
#     '7': -1,
#     '8': -1.5,
# }

if __name__ == '__main__':
    app.env = 'development'
    app.run()


@app.get('/models')
def get_models():
    resp = make_response('WIP', 200)
    return resp


@app.get('/models/<string:model>')
def get_model(model):
    resp = make_response(f'WIP {model}', 200)
    return resp


@app.post('/models')
def post_model():
    resp = make_response('WIP', 201)
    return resp


@app.delete('/models/<string:model>')
def delete_model(model):
    resp = make_response(f'WIP {model}', 200)
    return resp


@app.get('/datasets')
def get_datasets():
    resp = make_response('WIP', 200)
    return resp


@app.get('/datasets/<string:dataset>')
def get_dataset(dataset):
    resp = make_response(f'WIP {dataset}', 201)
    return resp


@app.post('/datasets')
def post_dataset():
    resp = make_response('WIP', 201)
    return resp


@app.delete('/datasets/<int:requestID>')
def delete_dataset(dataset):
    resp = make_response(f'WIP {dataset}', 200)
    return resp


@app.get('/cached-requests')
def get_cached_requests():
    resp = make_response('WIP', 200)
    return resp


@app.get('/cached-requests/<int:requestID>')
def get_cached_request(requestID):
    resp = make_response(f'WIP {requestID}', 201)
    return resp


@app.post('/requests/')
def post_request():
    resp = make_response('WIP', 101)
    return resp


@app.get('/shutdown')
def shutdown():
    sig = getattr(signal, "SIGKILL", signal.SIGTERM)
    os.kill(os.getpid(), sig)
