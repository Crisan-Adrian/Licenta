import os
import signal
from threading import Thread

from flask import Flask, request, make_response, json

import prediction
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

    if not repository.find_model(modelName, modelType):
        Thread(target=train_model, args=(modelName, modelType)).start()
        resp = make_response('Training started', 201)
        return resp

    resp = make_response("A model with this type and name already exists", 500)
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
    respT = make_response('Model deleted', 200)
    respF = make_response('Model not found', 404)
    if result:
        return respT
    else:
        return respF


@app.get('/requests')
def get_requests():
    responseData = repository.get_requests()
    response = json.dumps(responseData)
    resp = make_response(response, 200)
    return resp


@app.get('/requests/<string:requestName>')
def get_request(requestName):
    resp = make_response(f'WIP {requestName}', 201)
    return resp


@app.post('/requests')
def post_request():
    data = json.loads(request.data)
    requestName = data['requestName']
    models = data['models']
    _request = repository.find_request(requestName)
    if _request is not None:
        resp = make_response(f'Request already exists; request status: {_request["requestState"]}', 500)
        return resp

    isValid = validate_models(models)
    if not isValid:
        resp = make_response('Invalid models', 500)
        return resp

    Thread(target=make_prediction, args=(requestName, models)).start()
    resp = make_response('Request started', 200)
    return resp


def validate_models(models):
    if len(models) != 3:
        return False
    try:
        primitive_model = [x for x in models if x["modelType"] == "primitive"][0]
        position_model = [x for x in models if x["modelType"] == "position"][0]
        iteration_model = [x for x in models if x["modelType"] == "iteration"][0]

        if not repository.find_model(primitive_model["modelName"], primitive_model["modelType"]):
            return False
        if not repository.find_model(position_model["modelName"], position_model["modelType"]):
            return False
        if not repository.find_model(iteration_model["modelName"], iteration_model["modelType"]):
            return False

        return True
    except ValueError:
        return False
    except IndexError:
        return False


def make_prediction(requestName, models):
    primitive_model = [x for x in models if x["modelType"] == "primitive"][0]
    position_model = [x for x in models if x["modelType"] == "position"][0]
    iteration_model = [x for x in models if x["modelType"] == "iteration"][0]

    repository.add_request(requestName)
    prediction.make_prediction(primitive_model, position_model, iteration_model)
    repository.find_request(requestName)


@app.get('/status')
def get_models():
    resp = make_response("OK", 200)
    return resp


@app.get('/shutdown')
def shutdown():
    sig = getattr(signal, "SIGKILL", signal.SIGTERM)
    os.kill(os.getpid(), sig)
