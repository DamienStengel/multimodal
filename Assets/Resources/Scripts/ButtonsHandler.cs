using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsHandler : MonoBehaviour
{
    private Button move;
    private Button teleport;
    private Button place_object;
    
    private InteractionManager interactionManager;
    private Color defaultButtonColor;

    private void Start()
    {
        // Get button references
        move = GameObject.Find("Move").GetComponent<Button>();
        teleport = GameObject.Find("Teleport").GetComponent<Button>();
        place_object = GameObject.Find("Place object").GetComponent<Button>();
        
        // Get or add InteractionManager
        interactionManager = GameObject.Find("Controller").GetComponent<InteractionManager>();
        if (interactionManager == null)
        {
            interactionManager = GameObject.Find("Controller").AddComponent<InteractionManager>();
        }
        
        // Store default color
        defaultButtonColor = move.GetComponentInChildren<TextMeshProUGUI>().color;
        
        // Add button listeners
        move.onClick.AddListener(HandleMove);
        teleport.onClick.AddListener(HandleTeleport);
        place_object.onClick.AddListener(HandlePlaceObject);
        
        // Start in Move state
        UpdateButtonStates(InteractionManager.InteractionState.Move);
        HandleMove();
    }

    private void HandleMove()
    {
        UpdateButtonStates(InteractionManager.InteractionState.Move);
        interactionManager.SetInteractionState(InteractionManager.InteractionState.Move);
    }

    private void HandleTeleport()
    {
        UpdateButtonStates(InteractionManager.InteractionState.Teleport);
        interactionManager.SetInteractionState(InteractionManager.InteractionState.Teleport);
    }

    private void HandlePlaceObject()
    {
        UpdateButtonStates(InteractionManager.InteractionState.PlaceObject);
        interactionManager.SetInteractionState(InteractionManager.InteractionState.PlaceObject);
    }

    private void UpdateButtonStates(InteractionManager.InteractionState activeState)
    {
        // Reset all buttons to default
        ResetButton(move);
        ResetButton(teleport);
        ResetButton(place_object);
        
        // Highlight active button
        switch (activeState)
        {
            case InteractionManager.InteractionState.Move:
                HighlightButton(move);
                break;
            case InteractionManager.InteractionState.Teleport:
                HighlightButton(teleport);
                break;
            case InteractionManager.InteractionState.PlaceObject:
                HighlightButton(place_object);
                break;
        }
    }

    private void HighlightButton(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = Color.red;
        }
    }

    private void ResetButton(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = defaultButtonColor;
        }
    }
}