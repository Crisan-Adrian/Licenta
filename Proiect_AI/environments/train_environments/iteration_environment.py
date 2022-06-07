import random
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

def normalizeNode(_input, maxVal):
    normalized = _input / maxVal
    return normalized


def normalizeNodeL(_input, maxVal):
    normalized = []
    for x in _input:
        normalized.append(normalizeNode(x, maxVal))
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


class IterationEnvironment(gym.Env):
    # Observable state: current model state, target model state, current node configuration
    # Actions: 0 - stop iteration, 1 - continue iteration
    def __init__(self, primitives, n, episodeLength=None):
        np.random.seed(42)
        if episodeLength is None:
            self.length = 1000
        else:
            self.length = episodeLength
        self.n = n
        self.current = None
        self.target = None
        self.node = None
        self.setup()
        self.episodeStep = 0
        self.action_space = Discrete(2)
        self.shape = (2,)
        self.observation_space = Box(shape=self.shape, high=360.0, low=0.0)
        self.primitives = primitives
        self.currentStep = 0
        self.n = n

    def setup(self):
        current = np.random.uniform(0, 360, size=(self.length, self.n))
        target = np.random.uniform(-1.75, 1.75, size=(self.length, self.n))
        self.target = (current + target).tolist()
        for i in range(len(self.target)):
            for j in range(len(self.target[0])):
                if self.target[i][j] > 360:
                    self.target[i][j] -= 360
                if self.target[i][j] < 0:
                    self.target[i][j] += 360
        self.current = current.tolist()
        self.node = np.random.randint(0, 8, size=(self.length, self.n))

    def step(self, action: ActType) -> Tuple[ObsType, float, bool, dict]:
        canImprove = False

        currentL = self.current[self.currentStep]
        targetL = self.target[self.currentStep]
        nodeL = self.node[self.currentStep]
        for x in zip(currentL, targetL, nodeL):
            current = x[0]
            target = x[1]
            node = x[2]

            stateDelta = target - current
            if stateDelta > 180 or stateDelta < -180:
                if target > 350:
                    stateDelta = target - (360 + current)
                else:
                    stateDelta = 360 + target - current
            delta = abs(stateDelta - self.primitives[str(node)])
            for x in self.primitives:
                candidate = abs(stateDelta - self.primitives[x])
                if candidate < delta and x != str(node):
                    canImprove = True
                    break

        reward = 1 if ((canImprove and action == 1) or (not canImprove and action == 0)) else 0

        self.episodeStep += 1

        done = self.episodeStep == self.length

        current = self.current[self.currentStep]
        target = self.target[self.currentStep]
        node = self.node[self.currentStep]
        if canImprove:
            notOptimal = []
            for i in range(len(node)):
                if not self.is_optimal(node[i], current[i], target[i]):
                    notOptimal.append((i, node, current[i], target[i]))
            if len(notOptimal) != 0:
                index = random.randint(0, len(notOptimal) - 1)
                i, _node, _current, _target = notOptimal[index]
                self.set_optimal(i, _node, _current, _target)
        else:
            self.currentStep += 1

        current, target = preprocess(current, target)

        state = current + target + normalizeNodeL(node.tolist(), len(self.primitives))
        state = np.array(state)
        # print(state)

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

        state = current + target + normalizeNodeL(node.tolist(), len(self.primitives))
        state = np.array(state)

        return state

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

    def render(self, mode="human"):
        pass
