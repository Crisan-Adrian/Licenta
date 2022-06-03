import json
import os


class Repository:
    def __init__(self):
        self.filePath = "repository/repo.json"
        self.data = None
        self._initializeFile()

    def get_models(self):
        return self.data["models"]

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
                return False
        self.data["requests"].remove(finishedRequest)
        self._writeFile()
        return True

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
            for directory in dirs:
                for file in os.listdir(f"trained_models/{directory}"):
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
