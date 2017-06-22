using UnityEngine;

// <summary>
// Border positioning.
// Create and moves colliders around the field of camera view so that player can`t escape from camera.
// On object: Camera Boarder. Make it child to Main Camera object.
// Uses: -.
// </summary>


public class BoardersPositioning : MonoBehaviour
{

    private float colDepth = 4f; //Inner value for creating colliders
    private float zPosition = 0f;//Inner value for creating colliders
    private Vector2 screenSize;
    private Vector3 cameraPos;
    private Transform[] Colliders;

    private string[] Names;

    void Start()
    {
        Names = new string[4] { "TopCollider", "BottomCollider", "RightCollider", "LeftCollider" };
        Colliders = new Transform[4];

        for (int i = 0; i < 4; i++)
        {
            Colliders[i] = new GameObject().transform;                  //Generate our empty objects
            Colliders[i].name = Names[i];                               //Name our objects 
            Colliders[i].gameObject.AddComponent<BoxCollider2D>();      //Add the colliders
            Colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;//Changing collider type to trigger
            Colliders[i].parent = transform;                            //Make them the child of whatever object this script is on, preferably on the Camera so the objects move with the camera without extra scripting
        }

        //Generate world space point information for position and scale calculations
        cameraPos = Camera.main.transform.position;
        screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

        //Change our scale and positions to match the edges of the screen...   
        Colliders[2].localScale = new Vector3(colDepth, screenSize.y * 2, colDepth);
        Colliders[2].position = new Vector3(cameraPos.x + screenSize.x + (Colliders[2].localScale.x * 0.5f), cameraPos.y, zPosition);
        Colliders[3].localScale = new Vector3(colDepth, screenSize.y * 2, colDepth);
        Colliders[3].position = new Vector3(cameraPos.x - screenSize.x - (Colliders[3].localScale.x * 0.5f), cameraPos.y, zPosition);
        Colliders[0].localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
        Colliders[0].position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (Colliders[0].localScale.y * 0.5f), zPosition);
        Colliders[1].localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
        Colliders[1].position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (Colliders[1].localScale.y * 0.5f), zPosition);
    }
}
