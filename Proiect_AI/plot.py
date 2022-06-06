import datetime

import pandas as pd
import matplotlib.pyplot as plt

headers = ['reward']
plt.rcParams["figure.dpi"] = 200

df = pd.read_csv('train_rewards/position_model_0002.csv', names=headers)
# df['idx'] = range(1, len(df) + 1)
# print(df)

print(df.mean())


ax = df['reward'].plot(title='Reward per episode')
ax.set_xlabel("Episode")
ax.set_ylabel("Reward")
# plt.show()
plt.savefig("Reward_Position_Agent2.png")
