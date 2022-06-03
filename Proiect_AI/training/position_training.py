from environments import PositionEnvironment

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


def train_position_model(modelName):
    print("Running2")
    x = 0
    for x in range(0, 100000000):
        x **= 2
    print(modelName)

# while True:
#     env = PositionEnvironment(primitiveDict, 11, episodeLength=10)
