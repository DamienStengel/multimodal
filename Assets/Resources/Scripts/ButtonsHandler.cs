using UnityEngine;
using UnityEngine.UI;

public class ButtonsHandler : MonoBehaviour
{
    private Button move;
    private Button teleport;
    private Button place_object;

    private void Start()
    {
        move = GameObject.Find("Move").GetComponent<Button>();
        teleport = GameObject.Find("Teleport").GetComponent<Button>();
        place_object = GameObject.Find("Place object").GetComponent<Button>();

        move.onClick.AddListener(HandleMove);
        teleport.onClick.AddListener(HandleTeleport);
        place_object.onClick.AddListener(HandlePlaceObject);
    }

    // Handler methods for each button
    private void HandleMove()
    {
        Debug.Log("Move button clicked");
    
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

