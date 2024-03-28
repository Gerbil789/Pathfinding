using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCoontroll : MonoBehaviour
{
    Camera cam;
    [Range(1, 10)] [SerializeField] int zoomSensitivity = 5;

    [Range(5, 50)][SerializeField] int minZoom = 5;
    [Range(5, 50)][SerializeField] int maxZoom = 25;

    [Range(10, 50)][SerializeField] int speed = 25;

    [SerializeField] GameObject player;
    public bool lockCamera = false;

    private Vector3 dragOrigin;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)){
            lockCamera = !lockCamera;
        }

        if(lockCamera){
            transform.parent = player.transform;
            transform.localPosition = new Vector3(0,0,-10);
        }else{
            transform.parent = null;
        }

        HandleCameraZoom();
        HandleCameraMovement();
        ClampCamPos();
    }

    void HandleCameraZoom()
    {
        float zoomChangeAmount = 80f;
        if (Input.mouseScrollDelta.y > 0)
        {
            
            cam.orthographicSize -= zoomChangeAmount * Time.deltaTime * zoomSensitivity;

        }
        if (Input.mouseScrollDelta.y < 0)
        {
            cam.orthographicSize += zoomChangeAmount * Time.deltaTime * zoomSensitivity;
            ClampCamPos();
        }
    }

    void HandleCameraMovement()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lockCamera = false;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += difference;
        }

        if (Input.GetKey(KeyCode.A))
        {
            lockCamera = false;
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            lockCamera = false;
            transform.position += Vector3.up * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            lockCamera = false;
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            lockCamera = false;
            transform.position += Vector3.down * Time.deltaTime * speed;
        }
    }

    void ClampCamPos()
    {
        float newX = Mathf.Clamp(transform.position.x, 0f, MapManager.MapSize.x);
        float newY = Mathf.Clamp(transform.position.y, 0f, MapManager.MapSize.y);
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        cam.transform.position = new Vector3(newX, newY, cam.transform.position.z);
    }
}
