using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {

    private Vector3 SlingshotMiddleVector;

    [HideInInspector]
    public GameState.SlingshotState slingshotState;

    //the left and right parts of the slingshot
    public Transform LeftSlingshotOrigin, RightSlingshotOrigin;

    //two line renderers to simulate the "strings" of the slingshot
    public LineRenderer left;
    public LineRenderer right;

    //this linerenderer will draw the projected trajectory of the thrown bird
    public LineRenderer TrajectoryLineRenderer;

   
    //the bird to throw
    public GameObject BirdToThrow;

    //the position of the bird tied to the slingshot
    public Transform BirdWaitPosition;

    public float ThrowSpeed;

    [HideInInspector]
    public float TimeSinceThrown;
    // Use this for initialization
    void Start () {
        left.sortingLayerName = "Foreground";
        right.sortingLayerName = "Foreground";
        TrajectoryLineRenderer.sortingLayerName = "Foreground";
       

        slingshotState = GameState.SlingshotState.Idle;
		Vector3 newLeftPos = LeftSlingshotOrigin.position + new Vector3 (-0.5f, 1, 0);
		Vector3 newRightPos = RightSlingshotOrigin.position + new Vector3 (0, 1, 0);
		left.SetPosition(0, newLeftPos);
		right.SetPosition(0, newRightPos);

        //pointing at the middle position of the two vectors
		SlingshotMiddleVector = new Vector3((newLeftPos.x + newRightPos.x) / 2,
			(newLeftPos.y + newRightPos.y) / 2, 0);
    }
	
	// Update is called once per frame
	void Update () {
        runState();
    }

    private void runState() {
        switch (slingshotState)
        {
            case GameState.SlingshotState.Idle:
                //fix bird's position
                InitializeBird();
                //display the slingshot "strings"
                DisplaySlingshotLineRenderers();
                if (Input.GetMouseButtonDown(0))
                {
                    //get the point on screen user has tapped
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    //if user has tapped onto the bird
                    if (BirdToThrow.GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(location))
                    {
                        slingshotState = GameState.SlingshotState.UserPulling;
                    }
                }
                break;
            case GameState.SlingshotState.UserPulling:
                DisplaySlingshotLineRenderers();

                if (Input.GetMouseButton(0))
                {
                    //get where user is tapping
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					location.z = 0;
				 
                    //we will let the user pull the bird up to a maximum distance
                    if (Vector3.Distance(location, SlingshotMiddleVector) > 1.5f)
                    {
                        //basic vector maths :)
                        var maxPosition = (location - SlingshotMiddleVector).normalized * 1.5f + SlingshotMiddleVector;
                        BirdToThrow.transform.position = maxPosition;
                    }
                    else
                    {
                        BirdToThrow.transform.position = location;
                    }
                    float distance = Vector3.Distance(SlingshotMiddleVector, BirdToThrow.transform.position);
                    //display projected trajectory based on the distance
                    DisplayTrajectoryLineRenderer2(distance);
                }
                else//user has removed the tap 
                {
                    SetTrajectoryLineRenderesActive(false);
                    //TrajectoryLineRenderer.isVisible = false;


                    //throw the bird!!!
                    TimeSinceThrown = Time.time;
                    float distance = Vector3.Distance(SlingshotMiddleVector, BirdToThrow.transform.position);
                    if (distance > 1)
                    {
                        SetSlingshotLineRenderersActive(false);
                        slingshotState = GameState.SlingshotState.BirdFlying;
                        ThrowBird(distance);
                    }
                    else//not pulled long enough, so reinitiate it
                    {
                        //distance/10 was found with trial and error :)
                        //animate the bird to the wait position
                        //BirdToThrow.transform.positionTo(distance / 10, //duration
                        //    BirdWaitPosition.transform.position). //final position
                        //    setOnCompleteHandler((x) =>
                        //    {
                        //        x.complete();
                        //        x.destroy();
                        //        InitializeBird();
                        //    });

                    }
                }
                break;
        }
    }

    private void InitializeBird() {
		BirdToThrow.transform.position = BirdWaitPosition.position;
		slingshotState = GameState.SlingshotState.Idle;
		SetSlingshotLineRenderersActive(true);
    }

    private void ThrowBird(float distance)
    {
        //get velocity
        Vector3 velocity = SlingshotMiddleVector - BirdToThrow.transform.position;
        BirdToThrow.GetComponent<Bird>().OnThrow(); //make the bird aware of it                 //Bird
        //                                        //old and alternative way
       // BirdToThrow.GetComponent<Rigidbody2D>().AddForce(new Vector2(v2.x, v2.y) * ThrowSpeed * distance * 300 * Time.deltaTime);

        BirdToThrow.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y) * ThrowSpeed * distance;


        ////notify interested parties that the bird was thrown
        //if (BirdThrown != null)
        //    BirdThrown(this, EventArgs.Empty);
    }

    void DisplaySlingshotLineRenderers()
    {
        left.SetPosition(1, BirdToThrow.transform.position);
        right.SetPosition(1, BirdToThrow.transform.position);
    }

    void SetSlingshotLineRenderersActive(bool active)
    {
        left.enabled = active;
        right.enabled = active;
    }

    void DisplayTrajectoryLineRenderer2(float distance)
    {
        SetTrajectoryLineRenderesActive(true);


        Vector3 v2 = SlingshotMiddleVector - BirdToThrow.transform.position;
        int segmentCount = 15;
        float segmentScale = 2;
        Vector2[] segments = new Vector2[segmentCount];

        // The first line point is wherever the player's cannon, etc is
        segments[0] = BirdToThrow.transform.position;

        // The initial velocity
        Vector2 segVelocity = new Vector2(v2.x, v2.y) * ThrowSpeed * distance;

        float angle = Vector2.Angle(segVelocity, new Vector2(1, 0));
        float time = segmentScale / segVelocity.magnitude;
        for (int i = 1; i < segmentCount; i++)
        {
            //x axis: spaceX = initialSpaceX + velocityX * time
            //y axis: spaceY = initialSpaceY + velocityY * time + 1/2 * accelerationY * time ^ 2
            //both (vector) space = initialSpace + velocity * time + 1/2 * acceleration * time ^ 2
            float time2 = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + segVelocity * time2 + 0.5f * Physics2D.gravity * Mathf.Pow(time2, 2);
        }

		TrajectoryLineRenderer.numPositions = segmentCount;
		//.SetVertexCount(segmentCount);
        for (int i = 0; i < segmentCount; i++)
            TrajectoryLineRenderer.SetPosition(i, segments[i]);


    }

    void SetTrajectoryLineRenderesActive(bool active)
    {
        TrajectoryLineRenderer.enabled = active;
    }
}
