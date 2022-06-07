def convert_predictions(predictions):
    articulations = ["left_leg_upper",
                     "left_leg_lower",
                     "right_leg_upper",
                     "right_leg_lower",
                     "left_arm_upper",
                     "left_arm_lower",
                     "right_arm_upper",
                     "right_arm_lower"]

    primitives = {
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

    converted = []
    for prediction in predictions:
        node = {}
        for i in range(len(prediction)):
            node[articulations[i]] = primitives[str(prediction[i])]

        converted.append(node)

    return converted
