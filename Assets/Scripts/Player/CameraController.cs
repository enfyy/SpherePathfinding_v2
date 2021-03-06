using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform planet;

    [SerializeField] private Transform _followTarget;
    private Vector3 _followPosition;
    
    
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LockMouse();
        CameraEdgeScroll();
    }

    void CameraEdgeScroll()
    {
        var edgeWidth  = Mathf.RoundToInt(Screen.width * 0.02f);
        var edgeHeight = Mathf.RoundToInt(Screen.height * 0.02f);
        var mousePos = Input.mousePosition;
        if (mousePos.x >= Screen.width - edgeWidth)
        {
            // Right Edge
        } 
        if (mousePos.y > Screen.height - edgeHeight)
        {
            // Top Edge
        } 
        if (mousePos.x < 0 + edgeWidth)
        {
            // Left Edge
        } 
        if (mousePos.y < 0 + edgeHeight)
        {
            // Bot Edge
        } 
    }

    void LockMouse()
    {
        if (Input.GetKey(KeyCode.Space))
            Cursor.lockState = CursorLockMode.Confined;
        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("X: " + Input.mousePosition.x + " || Y: " + Input.mousePosition.y);
            Debug.Log("Width: " + Screen.width + " || Height: " + Screen.height);
        }
            
    }
    
    
}
