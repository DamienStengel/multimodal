using UnityEngine;

public class VRTranslate : MonoBehaviour
{
    public float translate_gain = 1.0f;
    public float rotation_gain = 2.0f;
    public float m_speed = 0.01f;
    private float verticalRotation = 0f;
    
    GameObject playerCam;
    GameObject playerReal;
    private Vector3 lastRealPosition;
    
    void Start()
    {
        playerCam = GameObject.Find("PlayerCam");
        playerReal = GameObject.Find("PlayerReal");
        lastRealPosition = playerReal.transform.position;
        
        // Lock cursor for better mouse control
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseRotation();
        HandleMovement();

        // Escape to unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void HandleMouseRotation()
    {
        // Mouse input with rotation gain applied
        float mouseX = Input.GetAxis("Mouse X") * rotation_gain;
        float mouseY = Input.GetAxis("Mouse Y") * rotation_gain;

        // Horizontal rotation
        playerReal.transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (head movement)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Create an empty GameObject for camera only
        GameObject headRotation = new GameObject();
        headRotation.transform.position = playerReal.transform.position;
        headRotation.transform.rotation = playerReal.transform.rotation;
        headRotation.transform.Rotate(new Vector3(verticalRotation, 0, 0));

        // Apply the rotation to both real and virtual cameras
        playerReal.transform.rotation = Quaternion.Euler(0, headRotation.transform.eulerAngles.y, 0);
        
        // Update camera rotations
        smartCamDisplace();
        
        // Cleanup
        Destroy(headRotation);
    }

    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        // Forward/Backward movement along the ground plane
        if (Input.GetKey(KeyCode.Z))
        {
            Vector3 forward = playerReal.transform.forward;
            forward.y = 0; // Keep movement on ground plane
            moveDirection += forward.normalized;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 forward = playerReal.transform.forward;
            forward.y = 0; // Keep movement on ground plane
            moveDirection -= forward.normalized;
        }

        if (moveDirection != Vector3.zero)
        {
            // Move real player straight
            playerReal.transform.position += moveDirection.normalized * m_speed;

            // Calculate curved movement for virtual camera
            Vector3 virtualMoveDirection = playerCam.transform.forward;
            virtualMoveDirection.y = 0;
            playerCam.transform.position += virtualMoveDirection.normalized * m_speed * translate_gain;

            lastRealPosition = playerReal.transform.position;
        }
    }

    void smartCamDisplace()
    {
        rotateCam();
        curveCam();
    }

    void translateCam()
    {
        // Calculate the movement delta since last frame
        Vector3 realMovement = playerReal.transform.position - lastRealPosition;
        
        // Apply the translation gain
        Vector3 scaledMovement = realMovement * translate_gain;
        
        // Move the virtual camera by the same delta
        playerCam.transform.position += scaledMovement;
        
        // Update the last position for next frame
        lastRealPosition = playerReal.transform.position;
    }

    void rotateCam()
    {
        Vector3 realRotationAngles = playerReal.transform.eulerAngles;
        Vector3 scaledRotation = realRotationAngles;
    
        // Appliquer directement la rotation mise à l'échelle
        playerCam.transform.rotation = Quaternion.Euler(scaledRotation);
    }

    void curveCam()
{
    if (Input.GetKey(KeyCode.W))
    {
        // Définir le rayon de la zone limite (en mètres)
        float maxRadius = 1f;
        Vector3 center = new Vector3(-11.33f, 0, 0);
        
        // Calculer la distance et la direction par rapport au centre
        Vector3 playerToCenter = playerReal.transform.position - center;
        float distanceToCenter = playerToCenter.magnitude;
        
        // Direction de marche
        Vector3 walkDirection = playerReal.transform.forward;
        
        // Calculer l'angle entre la direction de marche et le vecteur vers le centre
        float angleToCenter = Vector3.Angle(walkDirection, -playerToCenter);
        
        Debug.Log(angleToCenter);
        
        // Facteur basé sur la distance (0 au centre, 1 à la limite)
        float distanceFactor = Mathf.Clamp01(distanceToCenter / maxRadius);

        // Base curvature (1/22)
        float baseCurvature = 1.0f/22.0f;
        
        if (distanceFactor > 0.7f)  // Si on est près de la limite
        {
            if (angleToCenter > 90)  // Si on s'éloigne du centre
            {
                // Augmenter progressivement la courbure jusqu'à 0.045 (max imperceptible)
                float curvature = Mathf.Lerp(baseCurvature, 0.045f, distanceFactor);
                
                // Déterminer la direction de la courbure
                if (Vector3.Cross(walkDirection, playerToCenter).y > 0)
                    curvature = -curvature;  // Inverser si nécessaire
                    
                // Appliquer la rotation avec la courbure dynamique
                playerReal.transform.Rotate(0, m_speed * curvature * Mathf.Rad2Deg, 0);
                playerCam.transform.Rotate(0, -m_speed * curvature * Mathf.Rad2Deg, 0);
            }
            else  // Si on retourne vers le centre
            {
                // Garder la courbure de base
                playerReal.transform.Rotate(0, m_speed * baseCurvature * Mathf.Rad2Deg, 0);
                playerCam.transform.Rotate(0, -m_speed * baseCurvature * Mathf.Rad2Deg, 0);
            }
        }
        else  // Zone centrale sûre
        {
            // Courbure normale
            playerReal.transform.Rotate(0, m_speed * baseCurvature * Mathf.Rad2Deg, 0);
            playerCam.transform.Rotate(0, -m_speed * baseCurvature * Mathf.Rad2Deg, 0);
        }
    }
}
}