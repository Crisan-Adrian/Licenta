using System.Collections.Generic;

namespace NetworkDTO
{
    [System.Serializable]
    public class RequestDTO
    {
        public string requestName;
        public List<Model> models;
        public string observations;
    }
}