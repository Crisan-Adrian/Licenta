import tensorflow as tf
from keras import layers


def create_position_model(actions):
    model = tf.keras.Sequential()
    model.add(layers.Dense(actions*3, input_shape=(1, 8*3)))
    model.add(layers.Dense(actions*3, activation='relu'))
    model.add(layers.Dense(actions*2, activation='relu'))
    model.add(layers.Dense(actions*2, activation='relu'))
    model.add(layers.Dense(actions, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(actions, activation='linear'))
    return model


def create_position_model2(actions):
    model = tf.keras.Sequential()
    model.add(layers.Dense(actions*3, input_shape=(1, 8*3)))
    model.add(layers.Dense(actions*2, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(actions, activation='linear'))
    return model