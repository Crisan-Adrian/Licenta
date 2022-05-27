import numpy as np
import pandas as pd
from rl.agents import DQNAgent
from rl.memory import SequentialMemory
from rl.policy import GreedyQPolicy
from keras.optimizers import SGD

from environments import PrimitiveLiveEnvironment
from models import create_primitive_model


np.set_printoptions(precision=2)


def build_agent(model_p, actions_p):
    policy = GreedyQPolicy()
    memory = SequentialMemory(limit=1, window_length=1)
    dqnA = DQNAgent(model=model_p, memory=memory, policy=policy, test_policy=policy,
                    nb_actions=actions_p, enable_double_dqn=True,
                    nb_steps_warmup=0, target_model_update=0.1, batch_size=1)
    return dqnA


def normalize(input):
    normalized = input / 360
    return normalized


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

actions = len(primitiveDict)

modelV = "trained_models/model_Preprocessing_Exp_0001"

dataSet = pd.read_csv("datasets/dataSet_0000.csv")
test = np.array(dataSet)
n = test.shape[1] // 2
testEnv = PrimitiveLiveEnvironment(test, primitiveDict)
states = testEnv.get_shape()
model = create_primitive_model(states, actions)
dqn = build_agent(model, actions)
dqn.compile(SGD(learning_rate=10e-7))
scores = dqn.test(testEnv, nb_episodes=1, visualize=False)

for test_predict_arr in test:
    print(test_predict_arr[:8])
    print(test_predict_arr[8:])
    results = []
    for j in range(n):
        test_predict = np.array([normalize(test_predict_arr[j]), normalize(test_predict_arr[n + j])])
        # test_predict1 = np.expand_dims(test_predict1, axis=0)
        # test_predict1 = np.expand_dims(test_predict1, axis=0)

        # predict_1 = np.array(test_predict.tolist() + [0.0, 1.0])
        # predict_1 = np.expand_dims(predict_1, axis=0)
        # predict_1 = np.expand_dims(predict_1, axis=0)

        # print(test_predict1)
        # print(predict_1.shape)
        # print(test_predict2)
        # print(predict_2.shape)

        # trained_model.summary()

        result = dqn.forward(test_predict)

        # print(result_1, np.argmax(result_1))
        # print(result_2, np.argmax(result_2))
        results.append(primitiveDict[str(result)])
    print(results)
    print()
