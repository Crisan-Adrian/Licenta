using System;
using System.Collections.Generic;
using System.Linq;
using NetworkDTO;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class ImitationRequestWindow : EditorWindow
    {
        private string _requestName = "";
        private Model _primitiveModel;
        private Model _positionModel;
        private Model _iterationModel;
        private ModelList _models;

        private List<Model> _primitiveModels;
        private List<Model> _positionModels;
        private List<Model> _iterationModels;

        private string[] _primitiveModelNames;
        private string[] _positionModelNames;
        private string[] _iterationModelNames;

        private int _primitiveIndex = 0;
        private int _positionIndex = 0;
        private int _iterationIndex = 0;
        
        public static RequestDTO Open(ModelList models)
        {
            ImitationRequestWindow window = CreateInstance<ImitationRequestWindow>();
            window.SetModels(models);
            window.ShowModal();

            RequestDTO requestModalDTO = new RequestDTO();
            requestModalDTO.requestName = window._requestName;
            requestModalDTO.models = new List<Model>() { window._primitiveModel, window._positionModel, window._iterationModel };
            
            return requestModalDTO;
        }
        
        public void SetModels(ModelList models)
        {
            _models = models;
            _primitiveModels = _models.models.Where(model => model.modelType == "primitive").ToList();
            _primitiveModelNames = _primitiveModels.Select(model => model.modelName).ToArray();
            _positionModels = _models.models.Where(model => model.modelType == "position").ToList();
            _positionModelNames = _positionModels.Select(model => model.modelName).ToArray();
            _iterationModels = _models.models.Where(model => model.modelType == "iteration").ToList();
            _iterationModelNames = _iterationModels.Select(model => model.modelName).ToArray();
        }

        public void OnGUI()
        {
            EditorGUILayout.Space();
            _requestName = EditorGUILayout.TextField("Request Name:", _requestName);

            _primitiveIndex = EditorGUILayout.Popup("Primitive Model", _primitiveIndex, _primitiveModelNames);
            _primitiveModel = _primitiveModels[_primitiveIndex];
            
            _positionIndex = EditorGUILayout.Popup("Position Model", _positionIndex, _positionModelNames);
            _positionModel = _positionModels[_positionIndex];
            
            _iterationIndex = EditorGUILayout.Popup("Iteration Model", _iterationIndex, _iterationModelNames);
            _iterationModel = _iterationModels[_iterationIndex];
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Request"))
            {
                Close();
            }
        }
    }
}