import tensorflow as tf
from keras import layers, models, mixed_precision


def create_iterator_model():
    model = tf.keras.Sequential()
    model.add(layers.Dense(32))
    return model
