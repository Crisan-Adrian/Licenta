import datetime

import pandas as pd
import matplotlib.pyplot as plt

headers = ['reward']
plt.rcParams["figure.dpi"] = 200

df = pd.read_csv('training/train_rewards2022-05-23_01_12_03.232095_rewards.csv', names=headers)
# df['idx'] = range(1, len(df) + 1)
# print(df)

print(df.mean())


ax = df['reward'].plot(title='Reward per episode')
ax.set_xlabel("Episode")
ax.set_ylabel("Reward")
# plt.show()
plt.savefig("Reward_Plot_Annealed_Eps.png")
