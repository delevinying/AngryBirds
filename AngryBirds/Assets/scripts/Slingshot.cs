using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.Xml.Serialization;

public class Slingshot : MonoBehaviour {

	//弹弓中间坐标
	private Vector3 SlingshotMiddleVector;
	[HideInInspector]
	public GameState.SlingshotState slingshotState;
	//弹弓左，右
	public Transform LeftSlingshotOrigin, RightSlingshotOrigin;

	//弹弓左右两条线
	public LineRenderer left;
	public LineRenderer right;

	//表现弹道
	public LineRenderer TrajectoryLineRenderer;


	//丢出去的石头
	public GameObject BirdToThrow;

	//起始的石头所在坐标
	public Transform BirdWaitPosition;
	//丢出的速度
	public float ThrowSpeed;


	[HideInInspector]
	public float TimeSinceThrown;

	// 
	void Start () {
		left.sortingLayerName = "Foreground";
		right.sortingLayerName = "Foreground";
		TrajectoryLineRenderer.sortingLayerName = "Foreground";
		initGame ();
	}

	private void initGame(){
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
		case GameState.SlingshotState.Idle://起始，默认状态
			InitializeBird();//初始化石头
			DisplaySlingshotLineRenderers();//展示绘制弹弓的线条
			if (Input.GetMouseButtonDown(0))//点击左键
			{
				//鼠标的坐标点
				Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//用这种方式判断是否点击的是石头
				if (BirdToThrow.GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(location))
				{
					//如果是的话，就改变弹弓的状态，为拉伸状态
					slingshotState = GameState.SlingshotState.UserPulling;
				}
			}
			break;
		case GameState.SlingshotState.ReSet://起始，默认状态
			TraceLog.traceLog("Sling","Update","ReSet");
			break;
		case GameState.SlingshotState.UserPulling:
			DisplaySlingshotLineRenderers();//展示绘制弹弓的线条
			if (Input.GetMouseButton(0))//移动中的点击，未释放
			{
				Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				location.z = 0;
				//判断拉伸的距离，这里理解为力度大，飞的远，力度小于最小，则不飞出
				if (Vector3.Distance(location, SlingshotMiddleVector) > 1.5f)
				{
					//被拉伸的最远距离，形成一个圆圈
					var maxPosition = (location - SlingshotMiddleVector).normalized * 1.5f + SlingshotMiddleVector;
					BirdToThrow.transform.position = maxPosition;
				}
				else
				{
					//力度小，保留原来坐标
					BirdToThrow.transform.position = location;
				}
				//坐标点距离，石头与弹弓中间点的距离
				float distance = Vector3.Distance(SlingshotMiddleVector, BirdToThrow.transform.position);
				//展示飞行弹道
				DisplayTrajectoryLineRenderer2(distance);
			}
			else 
			{
				//释放了鼠标左键
				//不再展示预览弹道
				SetTrajectoryLineRenderesActive(false);
				//记录丢出的时间
				TimeSinceThrown = Time.time;
				//丢出一段时间后，弹弓线不需要再展示
				float distance = Vector3.Distance(SlingshotMiddleVector, BirdToThrow.transform.position);
				if (distance > 1)
				{
					//隐藏弹弓的线条
					SetSlingshotLineRenderersActive(false);
					//状态变为石头飞行
					slingshotState = GameState.SlingshotState.BirdFlying;
					ThrowBird(distance);
				}else{//留在执行动画，弹弓线条的动画
					//distance/10 was found with trial and error :)
					//animate the bird to the wait position
					/**BirdToThrow.transform.positionTo(distance / 10, //duration
                            BirdWaitPosition.transform.position). //final position
                            setOnCompleteHandler((x) =>
                            {
                                x.complete();
                                x.destroy();
                                InitializeBird();
                            });**/
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


	//执行丢石头
	private void ThrowBird(float distance)
	{
		//速度
		Vector3 velocity = SlingshotMiddleVector - BirdToThrow.transform.position;
		BirdToThrow.GetComponent<Bird>().OnThrow(); 
		BirdToThrow.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y) * ThrowSpeed * distance;
		//石头被丢出
		//if (BirdThrown != null)
		// BirdThrown(this, EventArgs.Empty);
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

	//展示弹道
	void DisplayTrajectoryLineRenderer2(float distance)
	{
		SetTrajectoryLineRenderesActive(true);
		//相减向量
		Vector3 v2 = SlingshotMiddleVector - BirdToThrow.transform.position;
		//15个坐标点，用来绘制曲线
		int segmentCount = 15;
		Vector2[] segments = new Vector2[segmentCount];
		//第一个点就是石头的坐标
		segments[0] = BirdToThrow.transform.position;

		//速度
		Vector2 segVelocity = new Vector2(v2.x, v2.y) * ThrowSpeed * distance;
		//计算其他点
		for (int i = 1; i < segmentCount; i++)
		{
			float time2 = i * Time.fixedDeltaTime * 5;
			segments[i] = segments[0] + segVelocity * time2 + 0.5f * Physics2D.gravity * Mathf.Pow(time2, 2);
		}
		//为弹道设置坐标数量，并将他赋予
		TrajectoryLineRenderer.numPositions = segmentCount;
		for (int i = 0; i < segmentCount; i++) {
			TrajectoryLineRenderer.SetPosition (i, segments [i]);
		}
	}

	void SetTrajectoryLineRenderesActive(bool active)
	{
		TrajectoryLineRenderer.enabled = active;
	}
}
