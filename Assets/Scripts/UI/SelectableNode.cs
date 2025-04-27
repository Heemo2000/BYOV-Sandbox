using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class SelectableNode : MonoBehaviour
    {
        private Selectable selectable;

        public void Select()
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }



        protected virtual void Awake()
        {
            selectable = GetComponent<Selectable>();
        }
        
    }
}
