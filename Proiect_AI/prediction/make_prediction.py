import numpy as np
from numpy import exp

from models import create_primitive_model, create_position_model2, create_iterator_model
from rl.agents import DQNAgent
from rl.memory import SequentialMemory
from rl.policy import GreedyQPolicy
from keras.optimizers import SGD


def make_prediction(primitive_model_name, position_model_name, iteration_model_name, data):
    primitiveDict = {
        '0': 0,
        '1': 0.25,
        '2': 0.5,
        '3': 1,
        '4': 1.5,
        '5': -0.25,
        '6': -0.5,
        '7': -1,
        '8': -1.5,
    }
    primitive_model = create_primitive_model(len(primitiveDict))
    position_model = create_position_model2(8)
    iteration_model = create_iterator_model(8)

    primitive_agent = build_agent(primitive_model, len(primitiveDict))
    primitive_agent.compile(SGD(learning_rate=10e-6))
    primitive_agent.load_weights(f'../trained_models/primitive_models/{primitive_model_name}')

    position_agent = build_agent(position_model, 8)
    position_agent.compile(SGD(learning_rate=10e-6))
    position_agent.load_weights(f'../trained_models/position_models/{position_model_name}')

    iteration_agent = build_agent(iteration_model, 2)
    iteration_agent.compile(SGD(learning_rate=10e-6))
    iteration_agent.load_weights(f'../trained_models/iteration_models/{iteration_model_name}')

    predictions = []
    for index in range(len(data)):
        continueIteration = True
        current = data[index]
        target = data[index]
        node = np.random.randint(0, len(primitiveDict), size=(1, 8))
        while continueIteration:
            index = choose_position(position_agent, current, target, node)

            newPrimitive = choose_primitive(primitive_agent, current[index], target[index])

            node[index] = newPrimitive

            continueIterationResult = choose_iteration(iteration_agent, current, target, node)

            continueIteration = (continueIterationResult == 1)

        predictions.append(node)

    return predictions

    # print("Running")
    # x = 0
    # for x in range(0, 100000000):
    #     x **= 2
    # print("Prediction Done")


def build_agent(model_p, actions_p):
    policy = GreedyQPolicy()
    memory = SequentialMemory(limit=1, window_length=1)
    dqnA = DQNAgent(model=model_p, memory=memory, policy=policy, test_policy=policy,
                    nb_actions=actions_p, enable_double_dqn=True,
                    nb_steps_warmup=0, target_model_update=0.1, batch_size=1)
    return dqnA


def choose_iteration(agent, current, target, node):
    _node = normalizeNodeL(node.tolist(), 9)
    _current = current
    _target = target
    for i in range(len(current)):
        delta = target[i] - current[i]
        z = exp(abs(delta))
        _current_i = normalize(current[i])
        if delta >= 0:
            _target_i = 1 - (1 - _current_i ** z) ** (1 / z)
        else:
            _target_i = (1 - (1 - _current_i) ** z) ** (1 / z)
        _current[i] = _current_i
        _target[i] = _target_i
    predict_data = _current + _target + _node
    predict_data = np.array(predict_data)

    result = agent.forward(predict_data)
    return result


def choose_position(agent, current, target, node):
    _node = normalizeNodeL(node.tolist(), 9)
    _current = current
    _target = target
    for i in range(len(current)):
        delta = target[i] - current[i]
        z = exp(abs(delta))
        _current_i = normalize(current[i])
        if delta >= 0:
            _target_i = 1 - (1 - _current_i ** z) ** (1 / z)
        else:
            _target_i = (1 - (1 - _current_i) ** z) ** (1 / z)
        _current[i] = _current_i
        _target[i] = _target_i
    predict_data = _current + _target + _node
    predict_data = np.array(predict_data)

    result = agent.forward(predict_data)
    return result


def choose_primitive(agent, current, target):
    delta = target - current
    z = exp(abs(delta))
    _current = normalize(current)
    if delta >= 0:
        _target = 1 - (1 - current ** z) ** (1 / z)
    else:
        _target = (1 - (1 - current) ** z) ** (1 / z)
    predict_data = np.array([_current, _target])

    result = agent.forward(predict_data)
    return result


def normalize(_input):
    normalized = _input / 360
    return normalized


def normalizeNode(_input, maxVal):
    normalized = _input / maxVal
    return normalized


def normalizeNodeL(_input, maxVal):
    normalized = []
    for x in _input:
        normalized.append(normalizeNode(x, maxVal))
    return normalized
