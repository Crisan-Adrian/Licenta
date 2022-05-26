from mpl_toolkits import mplot3d

import matplotlib.pyplot as plt
import numpy as np
from numpy import exp

primitiveDict = {
    0: 0,
    1: 0.25,
    2: 0.50,
    3: 1.00,
    4: 1.50,
    5: -0.25,
    6: -0.50,
    7: -1.00,
    8: -1.50
}


def normalize(input):
    normalized = input / 360
    return normalized


def fisheye(x, y):
    z = [0 for _ in x]
    for i in range(len(x)):
        current = x[i]
        target = y[i]
        delta = target - current
        _z = exp(abs(delta))
        current = normalize(current)
        if delta >= 0:
            target = 1 - (1 - current ** _z) ** (1 / _z)
        else:
            target = (1 - (1 - current) ** _z) ** (1 / _z)
        z[i] = target
    return z


def f(x, y):
    z = [0 for _ in x]
    for i in range(len(x)):
        z[i] = _f(x[i], y[i])
    return z


def _f(x, y):
    _z = 0
    stateDelta = y - x
    if stateDelta > 180 or stateDelta < -180:
        if y > 350:
            stateDelta = y - (360 + x)
        else:
            stateDelta = 360 + y - x
    delta = abs(stateDelta - primitiveDict[0])
    optimalDelta = delta
    for p in primitiveDict:
        candidate = abs(stateDelta - primitiveDict[p])
        if candidate < optimalDelta:
            optimalDelta = candidate
            _z = p
    return _z


size = 5000
x = np.random.uniform(0, 360, size=size)
y = np.random.uniform(-1.75, 1.75, size=size)
y = (x + y).tolist()
for i in range(len(y)):
    if y[i] > 360:
        y[i] -= 360
    if y[i] < 0:
        y[i] += 360
x = x.tolist()
z = f(x, y)
y = fisheye(x, y)
x = [normalize(i) for i in x]

ax = plt.axes(projection='3d')
ax.scatter(x, y, z, c=z, linewidth=0.5, cmap='viridis')
ax.view_init(elev=50., azim=-135)
# plt.show()
plt.savefig("Preprocessed_Primitives_Scatter")

# x = np.random.uniform(0, 360, size=size)
# y = np.random.uniform(-1.75, 1.75, size=size)
# y = (x + y).tolist()
# for i in range(len(y)):
#     if y[i] > 360:
#         y[i] -= 360
#     if y[i] < 0:
#         y[i] += 360
# x = [normalize(i) for i in x]
# y = [normalize(i) for i in y]
#
# plt.scatter(x, y, linewidths=0.5)
# plt.show()
