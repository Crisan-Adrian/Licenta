import tensorflow as tf
from keras import layers


def create_primitive_model(states, actions):
    model = tf.keras.Sequential()
    model.add(layers.Dense(actions, input_shape=(1, 2)))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))

    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))

    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(actions, activation='linear'))

    # model.add(layers.Dense(actions*2, input_shape=(1, 2)))
    # model.add(layers.Dense(actions*2, activation='relu'))
    # model.add(layers.Dense(actions, activation='relu'))
    # model.add(layers.Flatten())
    # model.add(layers.Dense(actions, activation='relu'))

    return model
