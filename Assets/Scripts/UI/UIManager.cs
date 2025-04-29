using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Page initialPage;
        [SerializeField]private bool showPageCount = false;
        private Stack<Page> pageStack;

        public int PageCount { get=> pageStack.Count; }

        private void Awake() {
            pageStack = new Stack<Page>();
        }
        private void Start()
        {
            if (initialPage != null)
            {
                PushPage(initialPage);
            }
        }

        private void Update() 
        {
            if(showPageCount)
            {
                Debug.Log("Page Count: " + PageCount);
            }    
        }

        public bool IsPageInStack(Page page)
        {
            return pageStack.Contains(page);
        }

        public bool IsPageOnTopOfStack(Page page)
        {
            return pageStack.Count > 0 && page == pageStack.Peek();
        }

        public void PushPage(Page page, bool playAnimation = true)
        {
            if(page == null)
            {
                Debug.LogError("Page is null!");
                return;
            }
            if(IsPageOnTopOfStack(page))
            {
                Debug.LogWarning("Cannot push same page over the top again.");
                return;
            }
            if(!page.gameObject.activeInHierarchy)
            {
                page.gameObject.SetActive(true);
            }
            page.Enter(playAnimation);

            if (pageStack.Count > 0)
            {
                Page currentPage = pageStack.Peek();

                if (currentPage.exitOnNewPagePush)
                {
                    currentPage.Exit(playAnimation);
                }
            }

            pageStack.Push(page);
        }

        public void PopPage(bool playAnimation = true)
        {
            if (pageStack.Count > 0)
            {
                Page page = pageStack.Pop();
                page.Exit(playAnimation);

                if(pageStack.Count > 0)
                {
                    Page newCurrentPage = pageStack.Peek();
                    if (newCurrentPage.exitOnNewPagePush)
                    {
                        newCurrentPage.Enter(playAnimation);
                    }
                }
                
            }
        }

        public void PopAllPages(bool playAnimation = true)
        {
            for (int i = 1; i < pageStack.Count; i++)
            {
                PopPage(playAnimation);
            }
        }
    }
}
