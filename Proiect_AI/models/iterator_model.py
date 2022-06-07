import tensorflow as tf
from keras import layers, models, mixed_precision


def create_iterator_model(size):
    model = tf.keras.Sequential()
    model.add(layers.Dense(size*3, input_shape=(1, size*3)))
    model.add(layers.Dense(size*2, activation='relu'))
    model.add(layers.Flatten())
    model.add(layers.Dense(2, activation='linear'))
    return model
