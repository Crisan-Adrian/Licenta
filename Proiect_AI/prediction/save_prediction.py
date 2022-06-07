import json
import os


def save_prediction(predictions, fileName):
    predictionsDict = {"predictions": predictions}
    json_object = json.dumps(predictionsDict, indent=4)

    script_dir = os.path.dirname(__file__)
    path = f"..\\predictions\\{fileName}.json"
    abs_path = os.path.join(script_dir, path)

    with open(abs_path, "x") as f:
        f.write(json_object)
