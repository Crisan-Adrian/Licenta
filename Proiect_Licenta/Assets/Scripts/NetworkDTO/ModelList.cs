using System.Collections.Generic;

namespace NetworkDTO
{
    [System.Serializable]
    public class ModelList
    {
        public List<Model> models;
    }

    [System.Serializable]
    public class Model
    {
        public string modelName;
        public string modelType;
    }
}