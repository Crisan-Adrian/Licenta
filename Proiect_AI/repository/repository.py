import json
import os


class Repository:
    def __init__(self, path, models_dir):
        self.filePath = path
        self.data = None
        self.models_dir = models_dir
        self._initializeFile()

    def get_models(self):
        return {"models": self.data["models"]}

    def get_model(self, modelName, modelType):
        for model in self.data["models"]:
            if model["modelType"] == modelType and model["modelName"] == modelName:
                return model
        return None

    def add_model(self, modelName, modelType):
        newModel = {"modelType": modelType, "modelName": modelName}
        for model in self.data["models"]:
            if model["modelType"] == modelType and model["modelName"] == modelName:
                return False
        self.data["models"].append(newModel)
        self._writeFile()
        return True

    def find_model(self, modelName, modelType):
        for model in self.data["models"]:
            if model["modelType"] == modelType and model["modelName"] == modelName:
                return True
        return False

    def delete_model(self, modelName, modelType):
        model = {"modelType": modelType, "modelName": modelName}
        try:
            self.data["models"].remove(model)
        except ValueError as _:
            return False
        self._writeFile()
        return True

    def get_requests(self):
        return self.data["requests"]

    def add_request(self, requestName):
        print(requestName)
        newRequest = {"requestName": requestName, "requestState": "NOT_FINISHED"}
        for request in self.data["requests"]:
            if request["requestName"] == requestName:
                return False
        self.data["requests"].append(newRequest)
        self._writeFile()
        return True

    def find_request(self, requestName):
        for request in self.data["requests"]:
            if request["requestName"] == requestName:
                return request
        return None

    def finish_request(self, requestName):
        for request in self.data["requests"]:
            if request["requestName"] == requestName:
                request["requestState"] = "FINISHED"
                self._writeFile()
                return True
        return False

    def deliver_request(self, requestName):
        finishedRequest = None
        for request in self.data["requests"]:
            if request["requestName"] == requestName and request["requestState"] == "FINISHED":
                finishedRequest = request
                break
            if request["requestName"] == requestName and request["requestState"] == "NOT_FINISHED":
                return None
        if finishedRequest is not None:
            self.data["requests"].remove(finishedRequest)
            self._writeFile()
            return finishedRequest
        return None

    def _writeFile(self):
        json_object = json.dumps(self.data, indent=4)

        with open(self.filePath, "w") as f:
            f.write(json_object)

    def _readFile(self):
        with open(self.filePath) as f:
            self.data = json.load(f)

    def _initializeFile(self):
        try:
            f = open(self.filePath, "x")
            dirs = {"iteration_models": "iteration", "position_models": "position", "primitive_models": "primitive"}
            models = []
            fileContent = {"models": models, "requests": []}
            script_dir = os.path.dirname(__file__)
            path = f"..\\{self.models_dir}"
            dir_path = os.path.join(script_dir, path)
            for directory in dirs:
                for file in os.listdir(f"{dir_path}\\{directory}"):
                    if file.endswith(".index"):
                        modelName = file.split(".")[0]
                        models.append({"modelName": modelName, "modelType": dirs[directory]})
            print(json.dumps(fileContent))
            f.write(json.dumps(fileContent))
            f.close()
        except FileExistsError:
            pass
        finally:
            self._readFile()
