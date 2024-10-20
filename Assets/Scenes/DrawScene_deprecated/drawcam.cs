using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

public class drawcam : MonoBehaviour
{
    private Camera Cam;
    [SerializeField] Camera WorldCam;
    [SerializeField] GameObject Brush;
    [SerializeField] GameObject Eraser;

    Vector3 resetPos = new(-5000f, -5000f, 0f);
    bool brushing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cam = GetComponent<Camera>();
        Cam.clearFlags = CameraClearFlags.Nothing;
    }

    // Update is called once per frame
    void Update()
    {
        handle_brush();
        handle_eraser();
    }


    void handle_brush() 
    {
        brushing = false;
        Brush.transform.position = resetPos;
        if(Input.GetMouseButton(0))
        {
            Vector2 mousepoint = WorldCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 brushpos = new(mousepoint.x, mousepoint.y, 0f);
            Brush.transform.position = brushpos;
            brushing = true;
        }
    }

    void handle_eraser() 
    {
        Eraser.transform.position = resetPos;
        if(brushing) return;
        if(Input.GetMouseButton(1))
        {
            Vector2 mousepoint = WorldCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 brushpos = new(mousepoint.x, mousepoint.y, 0f);
            Eraser.transform.position = brushpos;
        }
    }
}
