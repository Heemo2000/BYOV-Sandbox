using UnityEngine;

namespace Game.PlayerManagement
{
    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] meshes;

        public void EnableMeshes()
        {
            foreach(var mesh in meshes)
            {
                mesh.gameObject.SetActive(true);
                mesh.enabled = true;
            }
        }

        public void DisableMeshes()
        {
            foreach (var mesh in meshes)
            {
                mesh.gameObject.SetActive(false);
                mesh.enabled = false;
            }
        }
    }
}
