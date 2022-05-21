from rl.policy import Policy, BoltzmannQPolicy, EpsGreedyQPolicy


class MyPolicy(Policy):
    """Implement the Boltzmann Q Policy

        Boltzmann Q Policy builds a probability law on q values and returns
        an action selected randomly according to this law.
        """

    def __init__(self, eps=0.6):
        self.eps = eps
        self.innerPolicy = EpsGreedyQPolicy(eps)
        self.step = 0

    def select_action(self, q_values):
        """Return the selected action

        # Arguments
            q_values (np.ndarray): List of the estimations of Q for each action

        # Returns
            Selection action
        """
        self.step += 1
        if self.step % 2000 == 0:
            print(q_values)
        return self.innerPolicy.select_action(q_values)

    def get_config(self):
        """Return configurations of BoltzmannQPolicy

        # Returns
            Dict of config
        """
        return self.innerPolicy.get_config()