from keras.optimizers import Adam
from matplotlib import pyplot as plt
from rl.agents import DQNAgent
from rl.memory import SequentialMemory
from rl.policy import GreedyQPolicy, LinearAnnealedPolicy, EpsGreedyQPolicy

from environments import PositionEnvironment
from models import create_position_model2

EPISODE_LENGTH = 1000
EPISODES = 1000
ANNEAL_PERIOD = 100
MEMORY = 20000
BATCH = 1000
N = 8

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


def build_agent(model_p, actions_p):
    policy = LinearAnnealedPolicy(EpsGreedyQPolicy(), attr='eps', value_max=1., value_min=.1, value_test=.05,
                                  nb_steps=EPISODE_LENGTH * ANNEAL_PERIOD)
    test_policy = GreedyQPolicy()
    memory = SequentialMemory(limit=MEMORY, window_length=1)
    dqnA = DQNAgent(model=model_p, memory=memory, policy=policy, test_policy=test_policy,
                    nb_actions=actions_p, enable_double_dqn=True,
                    nb_steps_warmup=EPISODE_LENGTH, target_model_update=0.1, batch_size=BATCH)
    return dqnA


def train_position_model(modelName):
    # print("Running2")
    # x = 0
    # for x in range(0, 100000000):
    #     x **= 2
    # print(modelName)
    env = PositionEnvironment(primitiveDict, 8, episodeLength=EPISODE_LENGTH)

    actions = N
    print(actions)

    episodes = 10
    for episode in range(1, episodes + 1):
        env.reset()
        done = False
        score = 0

        while not done:
            action = env.action_space.sample()
            n_state, reward, done, info = env.step(action)
            score += reward
        print(f'Episode: {episode}, Score:{score}')

    model = create_position_model2(actions)

    model.summary()

    trainEnv = PositionEnvironment(primitiveDict, N, episodeLength=EPISODE_LENGTH)

    dqn = build_agent(model, actions)
    dqn.compile(Adam(learning_rate=10e-5))
    scores = dqn.fit(trainEnv, nb_steps=EPISODE_LENGTH * EPISODES + 1, visualize=False, verbose=2)
    dqn.save_weights(f'../trained_models/position_models/{modelName}', overwrite=True)

    print(scores.history['episode_reward'])

    filename = f"../train_rewards/{modelName}.csv"
    f = open(filename, mode="w")
    for x in scores.history['episode_reward']:
        f.write(str(x) + "\n")
    f.close()
    plt.plot(scores.history['episode_reward'])
    plt.show()


if __name__ == "__main__":
    train_position_model("position_model_0001")
