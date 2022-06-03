import os
import signal
from threading import Thread

from flask import Flask, request, make_response, json, after_this_request
from training import train_primitive_model, train_position_model
from repository import repository

app = Flask(__name__)
trainModels = {
    "primitive": train_primitive_model,
    "position": train_position_model,
    "iteration": train_primitive_model
}

if __name__ == '__main__':
    app.env = 'development'
    app.run()


@app.get('/models')
def get_models():
    responseData = repository.get_models()
    response = json.dumps(responseData)
    resp = make_response(response, 200)
    return resp


@app.get('/models/<string:model>')
def get_model():
    resp = make_response("WIP", 200)
    return resp


@app.post('/models')
def post_model():
    data = json.loads(request.data)
    modelName = data['modelName']
    modelType = data['modelType']
    Thread(target=train_model, args=(modelName, modelType)).start()
    resp = make_response('Training started', 201)
    return resp


def train_model(modelName, modelType):
    trainModels[modelType](modelName)
    repository.add_model(modelName, modelType)


@app.delete('/models/')
def delete_model():
    data = json.loads(request.data)
    modelName = data['modelName']
    modelType = data['modelType']
    result = repository.delete_model(modelName, modelType)
    respT = make_response('ModelDeleted', 200)
    respF = make_response('Model not found', 404)
    if result:
        return respT
    else:
        return respF


@app.get('/requests')
def get_requests():
    resp = make_response('WIP', 200)
    return resp


@app.get('/requests/<int:requestID>')
def get_request(requestID):
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
