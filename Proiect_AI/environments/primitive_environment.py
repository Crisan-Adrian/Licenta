from typing import Optional, Union, Tuple

import gym
import numpy as np
from gym.core import ObsType, ActType
from gym.spaces import Discrete, Box, MultiDiscrete


def normalize(input):
    normalized = input / 360
    return normalized


class PrimitiveEnvironment(gym.Env):
    # Observable state: current model state, target model state, current node configuration, edit position (i)
    # Actions: 0 -> m the new primitive in the i-th position of the node vector
    def __init__(self, primitives, episodeLength=None, repeat=2):
        np.random.seed(42)
        if episodeLength is None:
            self.length = 1000
        else:
            self.length = episodeLength
        current = np.random.uniform(0, 360, size=self.length)
        target = np.random.uniform(-1.75, 1.75, size=self.length)
        self.target = (current + target).tolist()
        for i in range(len(self.target)):
            if self.target[i] > 360:
                self.target[i] -= 360
            if self.target[i] < 0:
                self.target[i] += 360
        self.current = current.tolist()
        self.action_space = Discrete(len(primitives))
        self.shape = (2,)
        self.observation_space = Box(shape=self.shape, high=360.0, low=0.0)
        self.primitives = primitives
        self.currentStep = 0
        self.repeated = 0
        self.repeat = repeat

    def step(self, action: ActType) -> Tuple[ObsType, float, bool, dict]:
        stateDelta = self.target[self.currentStep] - self.current[self.currentStep]
        if stateDelta > 180 or stateDelta < -180:
            if self.target[self.currentStep] > 350:
                stateDelta = self.target[self.currentStep] - (360 + self.current[self.currentStep])
            else:
                stateDelta = 360 + self.target[self.currentStep] - self.current[self.currentStep]
        delta = abs(stateDelta - self.primitives[str(action)])
        optimalDelta = delta
        worstDelta = delta
        for x in self.primitives:
            candidate = abs(stateDelta - self.primitives[x])
            if candidate < optimalDelta:
                optimalDelta = candidate
            if candidate > worstDelta:
                worstDelta = candidate
        # print(abs(delta - currentDelta), abs(currentDelta - optimalDelta), abs(delta - optimalDelta))

        reward = (delta - optimalDelta) / (worstDelta - optimalDelta)

        reward = (1 - reward)
        reward = 0 if reward < 0.5 else reward
        reward = reward ** 4
        reward = 10 * reward

        # print(str(action), optimalPrimitive, reward)
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
        state = [normalize(self.current[self.currentStep]), normalize(self.target[self.currentStep])]
        state = np.array(state)
        # print(state)

        info = {}

        return state, reward, done, info

    def get_shape(self):
        return self.shape

    def reset(self, *, seed: Optional[int] = None, return_info: bool = False, options: Optional[dict] = None) -> Union[
        ObsType, tuple[ObsType, dict]]:

        self.currentStep = 0
        self.repeated = 0
        current = np.random.uniform(0, 360, size=self.length)
        target = np.random.uniform(-1.75, 1.75, size=self.length)
        for i in range(len(self.target)):
            if self.target[i] > 360:
                self.target[i] -= 360
            if self.target[i] < 0:
                self.target[i] += 360
        self.target = (current + target).tolist()
        self.current = current.tolist()

        state = [normalize(self.current[self.currentStep]), normalize(self.target[self.currentStep])]
        state = np.array(state)
        # print(state)

        return state

    def render(self, mode="human"):
        pass
