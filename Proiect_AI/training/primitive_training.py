from keras.optimizers import Adam
from matplotlib import pyplot as plt
from rl.agents import DQNAgent
from rl.memory import SequentialMemory
from rl.policy import GreedyQPolicy, LinearAnnealedPolicy, EpsGreedyQPolicy
from tensorflow import keras

from environments import PrimitiveEnvironment
from models import create_primitive_model

EPISODE_LENGTH = 500
REPEAT = 2
EPISODES = 1500
ANNEAL_PERIOD = 100
MEMORY = 20000
BATCH = 1000


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


def train_primitive_model(modelName):
    # print("Running")
    # x = 0
    # for x in range(0, 100000000):
    #     x **= 2
    # print(modelName)
    actions = len(primitiveDict)
    print(actions)

    model = create_primitive_model(actions)

    model.summary()

    trainEnv = PrimitiveEnvironment(primitiveDict, episodeLength=EPISODE_LENGTH, repeat=REPEAT)

    dqn = build_agent(model, actions)
    dqn.compile(Adam(learning_rate=10e-6))
    scores = dqn.fit(trainEnv, nb_steps=EPISODE_LENGTH * REPEAT * EPISODES + 1, visualize=False, verbose=0)
    dqn.save_weights(f'../trained_models/primitive_models/{modelName}', overwrite=True)

    filename = f"../train_rewards/{modelName}.csv"
    f = open(filename, mode="w")
    for x in scores.history['episode_reward']:
        f.write(str(x) + "\n")
    f.close()


def build_agent(model_p, actions_p):
    policy = LinearAnnealedPolicy(EpsGreedyQPolicy(), attr='eps', value_max=1., value_min=.1, value_test=.05,
                                  nb_steps=EPISODE_LENGTH * REPEAT * ANNEAL_PERIOD)
    test_policy = GreedyQPolicy()
    memory = SequentialMemory(limit=MEMORY, window_length=1)
    dqnA = DQNAgent(model=model_p, memory=memory, policy=policy, test_policy=test_policy,
                    nb_actions=actions_p, enable_double_dqn=True,
                    nb_steps_warmup=EPISODE_LENGTH * REPEAT, target_model_update=0.1, batch_size=BATCH)
    return dqnA


if __name__ == "__main__":
    env = PrimitiveEnvironment(primitiveDict, episodeLength=EPISODE_LENGTH)

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

    model = create_primitive_model(actions)

    model.summary()

    trainEnv = PrimitiveEnvironment(primitiveDict, episodeLength=EPISODE_LENGTH, repeat=REPEAT)

    lr_schedule = keras.optimizers.schedules.ExponentialDecay(
        initial_learning_rate=1e-5,
        decay_steps=1000,
        decay_rate=0.8)
    dqn = build_agent(model, actions)
    dqn.compile(Adam(learning_rate=10e-5))
    scores = dqn.fit(trainEnv, nb_steps=EPISODE_LENGTH * REPEAT * EPISODES + 1, visualize=False, verbose=2)
    dqn.save_weights('../trained_models/primitive_models/model_Small_NN_Exp2', overwrite=True)

    print(scores.history['episode_reward'])

    filename = "../train_rewards/Small_NN_Exp2_rewards.csv"
    f = open(filename, mode="w")
    for x in scores.history['episode_reward']:
        f.write(str(x) + "\n")
    f.close()
    plt.plot(scores.history['episode_reward'])
    plt.show()
