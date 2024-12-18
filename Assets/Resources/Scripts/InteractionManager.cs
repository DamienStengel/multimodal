using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
    public enum InteractionState
    {
        Move,
        Teleport,
        PlaceObject
    }

    [Header("References")]
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private GameObject furniturePrefab;
    [SerializeField] private AudioClip interactionSound;
    
    [Header("Visual Settings")]
    [SerializeField] private Color highlightColor = new Color(0.5f, 0.8f, 1f, 0.5f);
    [SerializeField] private float maxRayDistance = 100f;
    [SerializeField] private float previewObjectAlpha = 0.5f;
    
    [Header("UI")]
    [SerializeField] private FurnitureSelectionUI furnitureSelection;

    private InteractionState currentState = InteractionState.Move;
    private GameObject selectedTile;
    private GameObject previewObject;
    private Camera playerCamera;
    private GameObject playerReal;
    private VRTranslate moveScript;
    private AudioSource audioSource;
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();
    private Vector3 virtualToRealOffset; // Tracks the offset between virtual and real spaces

    private void Awake()
    {
        InitializeComponents();
    }

    private void OnDestroy()
    {
        CleanupMaterials();
    }

    private void InitializeComponents()
    {
        // Find and cache required components
        playerCamera = GameObject.Find("PlayerCam").GetComponent<Camera>();
        playerReal = GameObject.Find("PlayerReal");
        moveScript = GameObject.Find("Controller").GetComponent<VRTranslate>();
        
        if (playerCamera == null || playerReal == null)
        {
            Debug.LogError("InteractionManager: Required player objects not found!");
            enabled = false;
            return;
        }

        // Initialize the offset between virtual and real spaces
        virtualToRealOffset = playerCamera.transform.position - playerReal.transform.position;

        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // Initially disable movement
        if (moveScript != null)
        {
            moveScript.enabled = false;
        }
        else
        {
            Debug.LogError("InteractionManager: VRTranslate component not found!");
            enabled = false;
        }
    }

    private void Update()
    {
        if (currentState != InteractionState.Move)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, floorLayer))
        {
            HandleTileInteraction(hit);
        }
        else
        {
            ClearSelection();
        }
    }

    private void HandleTileInteraction(RaycastHit hit)
    {
        GameObject hitTile = hit.collider.gameObject;

        if (selectedTile != hitTile)
        {
            UpdateTileSelection(hitTile);
            UpdatePreviewObject(hit.point);
        }

        if (Input.GetMouseButtonDown(0))
        {
            ExecuteInteraction(hit.point);
        }
    }
    
    public void SetFurniturePrefab(GameObject newPrefab)
    {
        furniturePrefab = newPrefab;
    
        // If we're in place object mode, update the preview
        if (currentState == InteractionState.PlaceObject)
        {
            CleanupPreviewObject();
            CreatePreviewObject();
        }
    }
    
    public void SetInteractionState(InteractionState newState)
    {
        currentState = newState;
        ClearSelection();
        CleanupPreviewObject();
    
        moveScript.enabled = (newState == InteractionState.Move);
    
        // Show/hide furniture selection panel
        if (newState == InteractionState.PlaceObject)
        {
            furnitureSelection.ShowFurniturePanel(true);
        }
        else
        {
            furnitureSelection.ShowFurniturePanel(false);
        }
    }


    private void UpdateTileSelection(GameObject newTile)
    {
        if (selectedTile != null)
        {
            RestoreOriginalMaterials(selectedTile);
        }

        selectedTile = newTile;
        if (selectedTile != null)
        {
            ApplyHighlightMaterials(selectedTile);
        }
    }

    private void UpdatePreviewObject(Vector3 position)
    {
        if (currentState == InteractionState.PlaceObject && previewObject != null)
        {
            previewObject.transform.position = position;
        }
    }

    private void ExecuteInteraction(Vector3 position)
    {
        switch (currentState)
        {
            case InteractionState.Teleport:
                TeleportToTile(position);
                break;
            case InteractionState.PlaceObject:
                PlaceFurnitureAtPosition(position);
                break;
        }
        PlayFeedbackSound();
    }

    private void TeleportToTile(Vector3 position)
    {
        if (playerCamera != null && playerReal != null)
        {
            // Calculate how much we want to move in the virtual space
            Vector3 virtualMovement = position - playerCamera.transform.position;
            virtualMovement.y = 0; // Keep vertical position unchanged
            
            // Move the virtual camera
            playerCamera.transform.position += virtualMovement;
            
            // Update the real player position to maintain the proper offset
            Vector3 newRealPosition = playerCamera.transform.position - virtualToRealOffset;
            playerReal.transform.position = new Vector3(
                newRealPosition.x,
                playerReal.transform.position.y, // Keep original Y
                newRealPosition.z
            );
            
            // Update the offset for future calculations
            virtualToRealOffset = playerCamera.transform.position - playerReal.transform.position;
        }
    }

    private void PlaceFurnitureAtPosition(Vector3 position)
    {
        if (furniturePrefab != null)
        {
            GameObject furniture = Instantiate(furniturePrefab, position, Quaternion.identity);
            Renderer renderer = furniture.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (Material material in renderer.materials)
                {
                    material.color = Color.white;
                }
            }
        }
    }

    private void ApplyHighlightMaterials(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Store original materials if not already stored
            if (!originalMaterials.ContainsKey(obj))
            {
                originalMaterials[obj] = renderer.materials;
            }

            // Create and apply highlighted materials
            Material[] highlightedMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                highlightedMaterials[i] = new Material(renderer.materials[i]);
                highlightedMaterials[i].color = highlightColor;
            }
            renderer.materials = highlightedMaterials;
        }
    }

    private void RestoreOriginalMaterials(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && originalMaterials.ContainsKey(obj))
        {
            renderer.materials = originalMaterials[obj];
        }
    }

    private void CreatePreviewObject()
    {
        if (furniturePrefab != null)
        {
            previewObject = Instantiate(furniturePrefab);
            Renderer renderer = previewObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (Material material in renderer.materials)
                {
                    Color previewColor = material.color;
                    previewColor.a = previewObjectAlpha;
                    material.color = previewColor;
                }
            }
        }
    }

    private void ClearSelection()
    {
        if (selectedTile != null)
        {
            RestoreOriginalMaterials(selectedTile);
            selectedTile = null;
        }
    }

    private void CleanupPreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    private void CleanupMaterials()
    {
        foreach (var materials in originalMaterials.Values)
        {
            foreach (var material in materials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }
        originalMaterials.Clear();
    }

    private void PlayFeedbackSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
    }
}