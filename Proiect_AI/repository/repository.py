import json


class Repository:
    def __init__(self):
        self.filePath = "repository/repo.json"
        self.data = None
        self._readFile()

    def get_models(self):
        return self.data["models"]

    def add_model(self, modelName, modelType):
        model = {"modelType": modelType, "modelName": modelName}
        self.data["models"].append(model)
        self._writeFile()

    def delete_model(self, modelName, modelType):
        model = {"modelType": modelType, "modelName": modelName}
        try:
            self.data["models"].remove(model)
        except ValueError as _:
            return False
        self._writeFile()
        return True

    def get_requests(self):
        pass

    def add_request(self):
        pass

    def finish_request(self, requestName):
        pass

    def deliver_request(self, requestName):
        pass

    def _writeFile(self):
        json_object = json.dumps(self.data, indent=4)

        with open(self.filePath, "w") as f:
            f.write(json_object)

    def _readFile(self):
        with open(self.filePath) as f:
            self.data = json.load(f)
