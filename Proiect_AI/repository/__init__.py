import os

from .repository import Repository

script_dir = os.path.dirname(__file__)
abs_file_path = os.path.join(script_dir, "repo.json")
repository = Repository(abs_file_path, "trained_models")