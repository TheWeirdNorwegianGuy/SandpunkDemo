using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour {
    //mouse controll values
    float xRot = 0;
    float yRot = Mathf.PI / 2;
    float zoom = 5;

    //mouse sensitivity values
    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;
    public float mouseSensitivityScroll = 1;

    //tracking target values
    public GameObject target;
    Vector3 currentCenter;
    
    public float targetMoveLerp = 0.9f;

    //camera sliding
    public float slideRadius = 0.5f;
    public Vector3 camViewOffset;
    Vector3 cameraSlideOffset;

    //Camera boundry
    int boundryLayerMask;
    Ray boundryRay;
    RaycastHit boundryHit;

    public float cameraBufferSize = 0.2f;


    // Use this for initialization
    void Awake () {
        boundryLayerMask = LayerMask.GetMask("CamBoundry");
        Cursor.visible = false;
	}
	



	// Update is called once per frame
	void FixedUpdate () {
        //Get input
        cameraSlideOffset.x += Input.GetAxis("Mouse X") * mouseSensitivityX;
        cameraSlideOffset.y += Input.GetAxis("Mouse Y") * mouseSensitivityY;

        //Rotate if camera passes slide boundry
        if (cameraSlideOffset.magnitude > slideRadius) {
            xRot += cameraSlideOffset.x - (cameraSlideOffset.normalized.x * slideRadius);
            yRot += cameraSlideOffset.y - (cameraSlideOffset.normalized.y * slideRadius);
            cameraSlideOffset = cameraSlideOffset.normalized * slideRadius;
        }
        zoom += Input.GetAxis("Mouse ScrollWheel") * mouseSensitivityScroll;

        //clamp values
        yRot = Mathf.Clamp(yRot, 0.01f, Mathf.PI-0.01f);    //Clamp slightly below a half rotation to prevent rotation problems
        zoom = Mathf.Max(1, zoom);

        //move the center of rotation
        currentCenter = Vector3.Lerp(currentCenter, target.transform.position, targetMoveLerp);


        //calculate offset from center
        Vector3 camOffset;
        camOffset.x = Mathf.Sin(xRot) * Mathf.Sin(yRot) * zoom;
        camOffset.y = Mathf.Cos(yRot) * zoom;
        camOffset.z = Mathf.Cos(xRot) * Mathf.Sin(yRot) * zoom;
        
        //Keep the camera within boundries
        boundryRay.origin = currentCenter;
        boundryRay.direction = camOffset;

        if (Physics.SphereCast(boundryRay, cameraBufferSize, out boundryHit, camOffset.magnitude, boundryLayerMask)) {
            transform.position = boundryHit.point + boundryHit.normal * cameraBufferSize;

        } else {
            transform.position = currentCenter + camOffset;

        }


        //rotate camera towards center
        transform.LookAt(currentCenter);

        Vector3 camSlideOfsetRotated = Quaternion.LookRotation(transform.forward, transform.up) * camViewOffset;

        transform.LookAt(currentCenter + camSlideOfsetRotated);

    }


}
