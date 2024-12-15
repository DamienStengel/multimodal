using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FurnitureSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject furniturePanel;
    [SerializeField] private Transform buttonContainer;

    [Header("Button Settings")]
    [SerializeField] private float buttonWidth = 160f;
    [SerializeField] private float buttonHeight = 40f;
    [SerializeField] private float buttonSpacing = 10f;
    
    [System.Serializable]
    public class FurnitureItem
    {
        public string name = "Item";
        public GameObject prefab;
    }

    [Header("Furniture Options")]
    public FurnitureItem[] furnitureItems = new FurnitureItem[]{
        new FurnitureItem(), new FurnitureItem(), new FurnitureItem()
    };

    private InteractionManager interactionManager;

    void Start()
    {
        SetupContainer();
        interactionManager = GetComponent<InteractionManager>();
        CreateFurnitureButtons();
        furniturePanel.SetActive(false);
    }

    private void SetupContainer()
    {
        VerticalLayoutGroup verticalLayout = buttonContainer.GetComponent<VerticalLayoutGroup>();
        if (verticalLayout == null)
        {
            verticalLayout = buttonContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        }
        
        verticalLayout.spacing = buttonSpacing;
        verticalLayout.padding = new RectOffset(10, 10, 10, 10);
        verticalLayout.childAlignment = TextAnchor.UpperCenter;
        verticalLayout.childControlHeight = false;
        verticalLayout.childControlWidth = false;

        ContentSizeFitter sizeFitter = buttonContainer.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = buttonContainer.gameObject.AddComponent<ContentSizeFitter>();
        }
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void CreateFurnitureButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (FurnitureItem item in furnitureItems)
        {
            if (item.prefab != null)
            {
                // Create button as a UI element
                GameObject buttonObj = new GameObject(item.name + "_Button", typeof(RectTransform));
                buttonObj.transform.SetParent(buttonContainer, false);

                // Get the RectTransform and set size
                RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

                // Add Button component
                Button button = buttonObj.AddComponent<Button>();
                
                // Add Image component (button background)
                Image buttonImage = buttonObj.AddComponent<Image>();
                button.targetGraphic = buttonImage;
                buttonImage.color = new Color(1, 1, 1, 0.8f);

                // Create text as a UI element
                GameObject textObj = new GameObject("Text", typeof(RectTransform));
                textObj.transform.SetParent(buttonObj.transform, false);

                // Add TextMeshProUGUI component
                TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
                tmp.text = item.name;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontSize = 24;
                tmp.color = Color.black;

                // Set up text RectTransform
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                // Add click handler
                GameObject prefab = item.prefab;
                button.onClick.AddListener(() => SelectFurniture(prefab));
            }
        }
    }

    public void ShowFurniturePanel(bool show)
    {
        furniturePanel.SetActive(show);
    }

    private void SelectFurniture(GameObject prefab)
    {
        if (interactionManager != null)
        {
            interactionManager.SetFurniturePrefab(prefab);
            ShowFurniturePanel(false);
        }
    }
}