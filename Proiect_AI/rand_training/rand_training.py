import datetime
import numpy as np
import pandas as pd
from keras.optimizer_v2.adam import Adam
from matplotlib import pyplot as plt
from rl.agents import DQNAgent
from rl.memory import SequentialMemory
from rl.policy import BoltzmannQPolicy, GreedyQPolicy, LinearAnnealedPolicy, EpsGreedyQPolicy

from environments import PrimitiveEnvironment
from models import create_primitive_model
from primitive_rand_environment import PrimitiveRandEnvironment
from custom_policy import MyPolicy

EPISODE_LENGTH = 500
REPEAT = 2
EPISODES = 50
ANNEAL_PERIOD = 40


def build_agent(model_p, actions_p):
    policy = LinearAnnealedPolicy(EpsGreedyQPolicy(), attr='eps', value_max=1., value_min=.1, value_test=.05,
                                  nb_steps=EPISODE_LENGTH * REPEAT * ANNEAL_PERIOD)
    test_policy = GreedyQPolicy()
    memory = SequentialMemory(limit=50000, window_length=1)
    dqnA = DQNAgent(model=model_p, memory=memory, policy=policy, test_policy=test_policy,
                    nb_actions=actions_p, enable_double_dqn=True,
                    nb_steps_warmup=EPISODE_LENGTH * REPEAT, target_model_update=0.1, batch_size=1000)
    return dqnA


dataSet = pd.read_csv("../datasets/dataSet_0000.csv")

test = np.array(dataSet)
n = test.shape[1] // 2

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

env = PrimitiveRandEnvironment(primitiveDict, episodeLength=5000)

# print(env.action_space)
# print(env.action_space.sample())
#
# print(env.observation_space)
# print(env.observation_space.sample())

actions = len(primitiveDict)
states = env.get_shape()
print(actions)

episodes = 10
for episode in range(1, episodes + 1):
    state = env.reset()
    done = False
    score = 0

    while not done:
        action = env.action_space.sample()
        n_state, reward, done, info = env.step(action)
        score += reward
    print(f'Episode: {episode}, Score:{score}')

model = create_primitive_model(states, actions)
target_model = create_primitive_model(states, actions)

model.summary()

trainEnv = PrimitiveRandEnvironment(primitiveDict, episodeLength=EPISODE_LENGTH, repeat=REPEAT)

dqn = build_agent(model, actions)
dqn.compile(Adam(learning_rate=10e-7))
scores = dqn.fit(trainEnv, nb_steps=EPISODE_LENGTH * REPEAT * EPISODES + 1, visualize=False, verbose=2)
dqn.save_weights('../trained_models/model_0005_', overwrite=True)

print(scores.history['episode_reward'])

filename = "train_rewards" + str(datetime.datetime.now()).replace(" ", "_").replace(':', '_') + "_rewards.csv"
f = open(filename, mode="w")
for x in scores.history['episode_reward']:
    f.write(str(x) + "\n")
f.close()
plt.plot(scores.history['episode_reward'])
plt.show()

testEnv = PrimitiveEnvironment(test, primitiveDict, n)
scores = dqn.test(testEnv, nb_episodes=1, visualize=False)
print(np.mean(scores.history['episode_reward']))

# print(trained_model.layers[1].get_weights())
t_model = dqn.model
for i in range(len(test)):
    test_predict_arr = test[i].tolist()
    print(test_predict_arr)
    results = []
    for j in range(n):
        test_predict = np.array([test_predict_arr[j], test_predict_arr[n + j]])
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
