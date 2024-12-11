using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsHandler : MonoBehaviour
{
    private Button move;
    private Button teleport;
    private Button place_object;
    
    private VRTranslate vrTranslateScript;

    private void Start()
    {
        move = GameObject.Find("Move").GetComponent<Button>();
        teleport = GameObject.Find("Teleport").GetComponent<Button>();
        place_object = GameObject.Find("Place object").GetComponent<Button>();
        
        vrTranslateScript = GameObject.Find("Controller").GetComponent<VRTranslate>();
        vrTranslateScript.enabled = false;
        
        
        TextMeshProUGUI moveButtonText = move.GetComponentInChildren<TextMeshProUGUI>();

        move.onClick.AddListener(HandleMove);
        teleport.onClick.AddListener(HandleTeleport);
        place_object.onClick.AddListener(HandlePlaceObject);
    }

    // Handler methods for each button
    private void HandleMove()
    {
        Debug.Log("Move button clicked");
        
        // Toggle the script on/off
        vrTranslateScript.enabled = !vrTranslateScript.enabled;
        
        // Optional: Update button text to show state
        TextMeshProUGUI moveButtonText = move.GetComponentInChildren<TextMeshProUGUI>();
        if (moveButtonText != null)
        {
            moveButtonText.text = vrTranslateScript.enabled ? "Stop Move" : "Move";
            moveButtonText.color = vrTranslateScript.enabled ? Color.red : Color.black;
        }
    
    }

    private void HandleTeleport()
    {
        Debug.Log("Teleport button clicked");
    
    }

    private void HandlePlaceObject()
    {
        Debug.Log("Place Object button clicked");
    }
}

