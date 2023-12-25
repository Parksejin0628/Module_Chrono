using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform playerTr;
    public Transform cameraTr;
    public Transform cameraArea;
    public float followSpeed = 0.5f;
    
    void Start()
    {
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        cameraTr = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 playerPosition = new Vector3(playerTr.position.x, playerTr.position.y, cameraTr.position.z);
        Vector3 nextCameraPos = Vector3.Lerp(playerPosition, cameraTr.position, followSpeed);
        float cameraSizeY = cameraTr.GetComponent<Camera>().orthographicSize;
        float cameraSizeX = cameraSizeY * Screen.width / Screen.height;
        Debug.Log(cameraSizeY);
        nextCameraPos.x = Mathf.Clamp(nextCameraPos.x, cameraArea.position.x - (cameraArea.localScale.x / 2 - cameraSizeX), cameraArea.position.x + (cameraArea.localScale.x / 2 - cameraSizeX));
        nextCameraPos.y = Mathf.Clamp(nextCameraPos.y, cameraArea.position.y - (cameraArea.localScale.y / 2 - cameraSizeY), cameraArea.position.y + (cameraArea.localScale.y / 2 - cameraSizeY));
        cameraTr.position = nextCameraPos;
      
    }
    
}
