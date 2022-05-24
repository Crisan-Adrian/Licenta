import random
from typing import Optional, Union, Tuple

import gym
import numpy as np
from gym.core import ObsType, ActType
from gym.spaces import Discrete, Box


def normalize(_input):
    normalized = _input / 360
    return normalized


def normalizeL(_input):
    normalized = []
    for x in _input:
        normalized.append(normalize(x))
    return normalized


class PositionEnvironment(gym.Env):
    # Observable state: current model state, target model state, current node configuration
    # Actions: 0 -> n the vector position where next edit will be done
    def __init__(self, primitives, n, episodeLength=None, repeat=1):
        np.random.seed(42)
        if episodeLength is None:
            self.length = 1000
        else:
            self.length = episodeLength
        current = np.random.uniform(0, 360, size=(self.length, n))
        target = np.random.uniform(-1.75, 1.75, size=(self.length, n))
        self.target = (current + target).tolist()
        for i in range(len(self.target)):
            for j in range(len(self.target[0])):
                if self.target[i][j] > 360:
                    self.target[i][j] -= 360
                if self.target[i][j] < 0:
                    self.target[i][j] += 360
        self.current = current.tolist()
        self.node = np.random.randint(0, 8, size=(self.length, n))
        self.action_space = Discrete(len(primitives))
        self.shape = (2,)
        self.observation_space = Box(shape=self.shape, high=360.0, low=0.0)
        self.primitives = primitives
        self.currentStep = 0
        self.repeated = 0
        self.repeat = repeat
        self.n = n

    def step(self, action: ActType) -> Tuple[ObsType, float, bool, dict]:
        canImprove = False

        current = self.current[self.currentStep][action]
        target = self.target[self.currentStep][action]
        stateDelta = target - current
        if stateDelta > 180 or stateDelta < -180:
            if target > 350:
                stateDelta = target - (360 + current)
            else:
                stateDelta = 360 + target - current
        delta = abs(stateDelta - self.primitives[str(self.node[action])])
        for x in self.primitives:
            candidate = abs(stateDelta - self.primitives[x])
            if candidate < delta and x != str(self.node[action]):
                canImprove = True
                break

        reward = 1 if canImprove else 0

        # print()
        self.repeated += 1
        # self._setNode(action)
        if self.repeated >= self.repeat:
            self.currentStep += 1
            self.repeated = 0

        done = self.currentStep == self.length
        if done:
            self.currentStep -= 1

        # print([self.current.tolist()[self.position], self.target.tolist()[self.position]])
        current = self.current[self.currentStep]
        target = self.target[self.currentStep]
        state = [normalizeL(current), normalizeL(target),
                 self.node[self.currentStep]]
        state = np.array(state)
        # print(state)

        info = {}

        return state, reward, done, info

    def reset(self, *, seed: Optional[int] = None, return_info: bool = False, options: Optional[dict] = None) -> Union[
        ObsType, tuple[ObsType, dict]]:

        self.currentStep = 0
        self.repeated = 0
        current = np.random.uniform(0, 360, size=(self.length, self.n))
        target = np.random.uniform(-1.75, 1.75, size=(self.length, self.n))
        self.target = (current + target).tolist()
        for i in range(len(self.target)):
            if self.target[i] > 360:
                self.target[i] -= 360
            if self.target[i] < 0:
                self.target[i] += 360
        self.current = current.tolist()
        self.node = np.random.randint(0, 8, size=(self.length, self.n))

        current = self.current[self.currentStep]
        target = self.target[self.currentStep]
        state = [normalizeL(current), normalizeL(target),
                 self.node[self.currentStep]]
        state = np.array(state)

        return state

    def render(self, mode="human"):
        pass
