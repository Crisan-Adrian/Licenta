model = "../trained_models/model_0008"

# testEnv = PrimitiveDSEnvironment(test, primitiveDict, n)
# scores = dqn.test(testEnv, nb_episodes=1, visualize=False)
# print(np.mean(scores.history['episode_reward']))
#
# print(trained_model.layers[1].get_weights())
# for i in range(len(test)):
#     test_predict_arr = test[i].tolist()
#     print(test_predict_arr)
#     results = []
#     for j in range(n):
#         test_predict = np.array([test_predict_arr[j], test_predict_arr[n + j]])
#         # test_predict1 = np.expand_dims(test_predict1, axis=0)
#         # test_predict1 = np.expand_dims(test_predict1, axis=0)
#
#         # predict_1 = np.array(test_predict.tolist() + [0.0, 1.0])
#         # predict_1 = np.expand_dims(predict_1, axis=0)
#         # predict_1 = np.expand_dims(predict_1, axis=0)
#
#         # print(test_predict1)
#         # print(predict_1.shape)
#         # print(test_predict2)
#         # print(predict_2.shape)
#
#         # trained_model.summary()
#
#         result = dqn.forward(test_predict)
#
#         # print(result_1, np.argmax(result_1))
#         # print(result_2, np.argmax(result_2))
#         results.append(primitiveDict[str(result)])
#     print(results)
#     print()
