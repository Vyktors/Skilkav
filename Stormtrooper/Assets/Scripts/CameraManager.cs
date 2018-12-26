using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Script has to be attach to a Camera, to make it works, you need to have an object(player1) to follow linked in the inspector  */
/*smoothSpeed change the camera speed, closer the value 0, more the camera stick to the player1 or the exact center of the two player*/
/*The positionY axis can be unlocked*/

public class CameraManager : MonoBehaviour
{
    [Header("Players")]
    [Tooltip("Set the objects that the camera follows")]
    public GameObject player1;   //Object(player1) that the camera follows
    public GameObject player2;   //Object(player2) 

    [Header("Camera's position settings")]
    [Tooltip("Speed the camera will catch with the object's position in the X axis")]
    public float smoothSpeedX;   //Speed on X axis
    [Tooltip("Speed the camera will catch with the object's position in the Y axis")]
    public float smoothSpeedY;   //Speed on Y axis
    [Tooltip("Disable the camera to move on the Y axis")]
    public bool yAxisUnlock;      //Enable the camera to move on the Y axis
    [Tooltip("Set the height of the camera if it's locked on")]
    public float yHeight;       //Set the height of the camera if it's locked on

    [Header("Camera's size settings ")]
    public float minLengthSize = 5;
    public float maxLengthSize = 10f;

    private Camera cam;
    private Vector2 velocity;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        SetPosition();
        SetSize();
    }

    void FixedUpdate()
    {
        SetPosition();
        SetSize();
    }

    private void SetPosition()
    {
        //Determine the middle point beetween the two players
        float middleX = (player1.transform.position.x + player2.transform.position.x) * 0.5f;
        float middleY = (player1.transform.position.y + player2.transform.position.y) * 0.5f;
        Vector3 middle = new Vector3(middleX, middleY);

        float posX = Mathf.SmoothDamp(transform.position.x, middle.x, ref velocity.x, smoothSpeedX); //Current position, target position, current velocity, smoothSpeed

        if (yAxisUnlock)
        {
            float posY = Mathf.SmoothDamp(transform.position.y, middle.y, ref velocity.y, smoothSpeedY);

            transform.position = new Vector3(posX, posY, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(posX, yHeight, transform.position.z);
        }
    }

    //** Make more funcion
    private void SetSize()
    {
        float newSize;
        //Distance beetween players
        float lengthX = Mathf.Abs(player1.transform.position.x - player2.transform.position.x);
        float lengthY = Mathf.Abs(player1.transform.position.y - player2.transform.position.y);

        //Math function to get good ratio beetween the distance and the size, smoothly
        float newSizeX = Mathf.MoveTowards(cam.orthographicSize, lengthX * .5f - 2.5f, smoothSpeedX); //** Get a better way to get good values, not hardcoded
        float newSizeY = Mathf.MoveTowards(cam.orthographicSize, lengthY * .5f + 1f , smoothSpeedY);
        
        //Take the greater Size
        if (newSizeX > newSizeY)
        {
            newSize = newSizeX;
        }
        else{
            newSize = newSizeY;
        }       

        //If the newSize is out of limits, change newSize for the limit itself.
        if (newSize > maxLengthSize)
        {
            newSize = Mathf.MoveTowards(cam.orthographicSize, maxLengthSize, smoothSpeedX);
        }
        if (newSize < minLengthSize)
        {
            newSize = Mathf.MoveTowards(cam.orthographicSize, minLengthSize, smoothSpeedX);
        }
        cam.orthographicSize = newSize;
    }
}
