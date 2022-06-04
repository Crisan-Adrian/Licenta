from typing import Optional, Union, Tuple

import gym
import numpy as np
from gym.core import ObsType, ActType
from gym.spaces import Discrete, Box
from numpy import exp


def normalize(_input):
    normalized = _input / 360
    return normalized


def normalizeL(_input):
    normalized = []
    for x in _input:
        normalized.append(normalize(x))
    return normalized


def preprocess(current, target):
    for i in range(len(current)):
        delta = target[i] - current[i]
        _current = current[i]
        z = exp(abs(delta))
        _current = normalize(_current)
        if delta >= 0:
            _target = 1 - (1 - _current ** z) ** (1 / z)
        else:
            _target = (1 - (1 - _current) ** z) ** (1 / z)
        current[i] = _current
        target[i] = _target
    return current, target


class PositionEnvironment(gym.Env):
    current: list

    # Observable state: current model state, target model state, current node configuration
    # Actions: 0 -> n the vector position where next edit will be done
    def __init__(self, primitives, n, episodeLength=None):
        self.episodeStep = 0
        np.random.seed(42)
        if episodeLength is None:
            self.length = 1000
        else:
            self.length = episodeLength
        self.n = n
        self.current = []
        self.target = None
        self.node = None
        self.primitives = primitives
        self.setup()
        self.action_space = Discrete(n)
        self.shape = (2,)
        self.observation_space = Box(shape=self.shape, high=360.0, low=0.0)
        self.currentStep = 0

    def get_shape(self):
        return self.shape

    def is_optimal(self, node, current, target):
        stateDelta = target - current
        if stateDelta > 180 or stateDelta < -180:
            if target > 350:
                stateDelta = target - (360 + current)
            else:
                stateDelta = 360 + target - current
        delta = abs(stateDelta - self.primitives[str(node)])
        for x in self.primitives:
            candidate = abs(stateDelta - self.primitives[x])
            if candidate < delta:
                return False
        return True

    def set_optimal(self, index, node, current, target):
        stateDelta = target - current
        if stateDelta > 180 or stateDelta < -180:
            if target > 350:
                stateDelta = target - (360 + current)
            else:
                stateDelta = 360 + target - current
        delta = abs(stateDelta - self.primitives[str(node[index])])
        optimalPrimitive = node[index]
        optimalDelta = delta
        for x in self.primitives:
            candidate = abs(stateDelta - self.primitives[x])
            if candidate < optimalDelta:
                optimalDelta = candidate
                optimalPrimitive = x
        node[index] = optimalPrimitive

    def step(self, action: ActType) -> Tuple[ObsType, float, bool, dict]:
        canImprove = False
        setOptimal = False

        current = self.current[self.currentStep][action]
        target = self.target[self.currentStep][action]
        stateDelta = target - current
        if stateDelta > 180 or stateDelta < -180:
            if target > 350:
                stateDelta = target - (360 + current)
            else:
                stateDelta = 360 + target - current
        delta = abs(stateDelta - self.primitives[str(self.node[self.currentStep][action])])
        for x in self.primitives:
            candidate = abs(stateDelta - self.primitives[x])
            if candidate < delta and x != str(self.node[action]):
                canImprove = True
                self.set_optimal(action, self.node[self.currentStep], self.current[self.currentStep][action],
                                 self.target[self.currentStep][action])
                setOptimal = True
                break

        reward = 1 if canImprove else 0

        current = self.current[self.currentStep]
        target = self.target[self.currentStep]
        node = self.node[self.currentStep]

        self.episodeStep += 1
        if not setOptimal:
            for i in range(len(node)):
                if not self.is_optimal(node[i], current[i], target[i]):
                    self.set_optimal(i, node, current[i], target[i])
                else:
                    self.currentStep += 1
                    if self.currentStep == self.length:
                        self.currentStep = 0
                    current = self.current[self.currentStep]
                    target = self.target[self.currentStep]
                    node = self.node[self.currentStep]

        done = self.episodeStep >= self.length

        current, target = preprocess(current, target)

        state = current + target + node.tolist()
        state = np.array(state)

        info = {}

        return state, reward, done, info

    def reset(self, *, seed: Optional[int] = None, return_info: bool = False, options: Optional[dict] = None) -> Union[
        ObsType, tuple[ObsType, dict]]:

        self.currentStep = 0
        self.episodeStep = 0
        self.setup()

        current = self.current[self.currentStep]
        target = self.target[self.currentStep]
        node = self.node[self.currentStep]

        current, target = preprocess(current, target)

        state = current + target + node.tolist()
        state = np.array(state)

        return state

    def setup(self):
        current = np.random.uniform(0, 360, size=(self.length, self.n))
        target = np.random.uniform(-1.75, 1.75, size=(self.length, self.n))
        self.target = (current + target).tolist()
        for i in range(len(self.target)):
            for j in range(len(self.target[i])):
                if self.target[i][j] > 360:
                    self.target[i][j] -= 360
                if self.target[i][j] < 0:
                    self.target[i][j] += 360
        self.current = current.tolist()
        self.node = np.random.randint(0, len(self.primitives), size=(self.length, self.n))

    def render(self, mode="human"):
        pass
