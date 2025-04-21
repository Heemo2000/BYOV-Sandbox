using System.Collections.Generic;
using UnityEngine;
using Game.InversionOfControlManagement;

namespace Game.CameraManagement
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private BaseCamera initialCamera;
        private List<BaseCamera> cameras;
        
        public void MakeCameraImportant(BaseCamera camera)
        {
            for(int i = 0; i < cameras.Count; i++)
            {
                var current = cameras[i];
                if(current.GetHashCode() == camera.GetHashCode())
                {
                    Debug.Log("Making camera (" + camera.transform.name + ") important");
                    current.SetPriority(10);
                }
                else
                {
                    current.SetPriority(0);
                }
            }
        }

        public void MakeInitialCameraImportant()
        {
            MakeCameraImportant(initialCamera);
        }
        private void Awake()
        {
            cameras = new List<BaseCamera>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            var cameraInstances = GameObject.FindObjectsByType(typeof(BaseCamera), FindObjectsSortMode.InstanceID);

            for(int i = 0; i < cameraInstances.Length; i++)
            {
                var camera = cameraInstances[i] as BaseCamera;
                if(cameras.IndexOf(camera) == -1)
                {
                    cameras.Add(camera);
                }
            }

            
            if(initialCamera != null)
            {
                MakeInitialCameraImportant();
            }

            ServiceLocator.ForSceneOf(this).Register<CameraManager>(this);
        }
    }
}
