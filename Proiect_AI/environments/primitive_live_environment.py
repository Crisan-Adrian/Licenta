from typing import Optional, Union, Tuple

import gym
import numpy as np
from gym.core import ObsType, ActType
from gym.spaces import Discrete, Box, MultiDiscrete


def normalize(input):
    normalized = input / 360
    return normalized


class PrimitiveLiveEnvironment(gym.Env):
    # Observable state: current model state, target model state, current node configuration, edit position (i)
    # Actions: 0 -> m the new primitive in the i-th position of the node vector
    def __init__(self, dataset, primitives):
        np.random.seed(42)
        self.length = len(dataset)
        self.dataset = dataset
        self.current = dataset[0][0]
        self.target = dataset[0][1]
        self.action_space = Discrete(len(primitives))
        self.shape = (2,)
        self.observation_space = Box(shape=self.shape, high=360.0, low=0.0)
        self.primitives = primitives
        self.currentStep = 0

    def step(self, action: ActType) -> Tuple[ObsType, float, bool, dict]:
        stateDelta = self.target - self.current
        if stateDelta > 180 or stateDelta < -180:
            if self.target > 350:
                stateDelta = self.target - (360 + self.current)
            else:
                stateDelta = 360 + self.target - self.current
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

        reward = 1

        self.currentStep += 1

        done = self.currentStep == self.length
        if done:
            self.currentStep -= 1

        self.current = self.dataset[self.currentStep][0]
        self.target = self.dataset[self.currentStep][1]

        # print([self.current.tolist()[self.position], self.target.tolist()[self.position]])
        state = [normalize(self.current), normalize(self.target)]
        state = np.array(state)
        # print(state)

        info = {}

        return state, reward, done, info

    def get_shape(self):
        return self.shape

    def reset(self, *, seed: Optional[int] = None, return_info: bool = False, options: Optional[dict] = None) -> Union[
        ObsType, tuple[ObsType, dict]]:

        self.currentStep = 0
        self.current = self.dataset[0][0]
        self.target = self.dataset[0][1]

        state = [normalize(self.current), normalize(self.target)]
        state = np.array(state)
        # print(state)

        return state

    def render(self, mode="human"):
        pass
