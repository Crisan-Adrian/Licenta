import tensorflow as tf
from keras import layers, models, mixed_precision


def create_iterator_model(actions):
    model = tf.keras.Sequential()
    model.add(layers.Dense(actions*3, input_shape=(1, 8*3)))
    model.add(layers.Dense(actions*2, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(2, activation='linear'))
    return model
