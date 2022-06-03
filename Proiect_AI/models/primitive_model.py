import tensorflow as tf
from keras import layers


def create_primitive_model3(actions):
    model = tf.keras.Sequential()
    model.add(layers.Dense(actions, input_shape=(1, 2)))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))

    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(actions, activation='linear'))

    return model


def create_primitive_model2(actions):
    model = tf.keras.Sequential()
    model.add(layers.Dense(actions, input_shape=(1, 2)))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))

    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))

    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(actions, activation='linear'))

    return model


def create_primitive_model(actions):
    model = tf.keras.Sequential()
    model.add(layers.Dense(actions, input_shape=(1, 2)))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(actions, activation='linear'))
    return model
