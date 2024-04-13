using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface
{
    public class PageToggle : MonoBehaviour
    {
        public Page[] pages;
        public int defaultPage;
        
        [Serializable]
        public class Page
        {
            public Button activateButton;
            public GameObject page;
            public bool isActive;
        }
        
        private void Start()
        {
            foreach (Page page in pages)
            {
                page.activateButton.onClick.AddListener(() =>
                {
                    foreach (Page p in pages)
                    {
                        p.page.SetActive(p.activateButton == page.activateButton);
                        p.isActive = p.activateButton == page.activateButton;
                        p.activateButton.interactable = !p.isActive;
                    }
                });
                page.isActive = false;
                page.page.SetActive(false);
                page.activateButton.interactable = true;
            }
            
            pages[defaultPage].page.SetActive(true);
            pages[defaultPage].isActive = true;
            pages[defaultPage].activateButton.interactable = false;
        }
        
        private void SetDefaultPage()
        {
            foreach (Page page in pages)
            {
                page.isActive = false;
                page.page.SetActive(false);
                page.activateButton.interactable = true;
            }
            
            pages[defaultPage].page.SetActive(true);
            pages[defaultPage].isActive = true;
            pages[defaultPage].activateButton.interactable = false;
        }
    }
}