import os

from repository import Repository


class TestRepository:
    def __init__(self):
        script_dir = os.path.dirname(__file__)
        path = "test_repo.json"
        self.path = os.path.join(script_dir, path)
        self.models_dir = "test_models"
        self.repository = Repository(self.path, self.models_dir)

    def setup(self):
        self.repository = Repository(self.path, self.models_dir)

    def teardown(self):
        if os.path.exists("test_repo.json"):
            os.remove("test_repo.json")
        self.repository = None

    def test_model(self):
        self.setup()

        try:

            models = self.repository.get_models()
            assert (len(models["models"])) == 0

            modelName = "a"
            modelType = "primitive"

            found = self.repository.find_model(modelName, modelType)
            assert found == False

            deleted = self.repository.delete_model(modelName, modelType)
            assert deleted == False

            added = self.repository.add_model(modelName, modelType)
            assert added == True

            found = self.repository.find_model(modelName, modelType)
            assert found == True

            added = self.repository.add_model(modelName, modelType)
            assert added == False

            models = self.repository.get_models()
            assert (len(models["models"])) == 1

            deleted = self.repository.delete_model(modelName, modelType)
            assert deleted == True

            models = self.repository.get_models()
            assert (len(models["models"])) == 0

            print("test_model passed")

        except AssertionError as e:
            print("test_model failed")
            raise e

        self.teardown()

    def test_request(self):
        self.setup()

        try:

            requests = self.repository.get_requests()
            assert (len(requests)) == 0

            requestName = "a"

            found = self.repository.find_request(requestName)
            assert found == False

            request = self.repository.deliver_request(requestName)
            assert request is None

            deleted = self.repository.finish_request(requestName)
            assert deleted == False

            added = self.repository.add_request(requestName)
            assert added == True

            request = self.repository.find_request(requestName)
            assert request is not None
            assert request["requestName"] == requestName
            assert request["requestState"] == "NOT_FINISHED"

            added = self.repository.add_request(requestName)
            assert added == False

            models = self.repository.get_requests()
            assert (len(models["models"])) == 1

            request = self.repository.deliver_request(requestName)
            assert request is None

            deleted = self.repository.finish_request(requestName)
            assert deleted == True

            request = self.repository.deliver_request(requestName)
            assert request is not None
            assert (len(models["models"])) == 0

            print("test_request passed")

        except AssertionError as e:
            print("test_request failed")
            raise e

        self.teardown()

    def run_tests(self):
        self.test_model()
        self.test_request()
