using UnityEngine;
using System.Collections;
using System;
//using Windows.Kinect;


public class SkeletonOverlayer : MonoBehaviour 
{
//	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
//	public GUITexture backgroundImage;

	[Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
	public Camera foregroundCamera;
	
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Game object used to overlay the joints.")]
	public GameObject jointPrefab;

	[Tooltip("Line object used to overlay the bones.")]
	public LineRenderer linePrefab;
	//public float smoothFactor = 10f;

	//public UnityEngine.UI.Text debugText;
	
	private GameObject[] joints = null;
	private LineRenderer[] lines = null;

	private Quaternion initialRotation = Quaternion.identity;


	void Start()
	{
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			int jointsCount = manager.GetJointCount();

			if(jointPrefab)
			{
				// array holding the skeleton joints
				joints = new GameObject[jointsCount];
				
				for(int i = 0; i < joints.Length; i++)
				{
					joints[i] = Instantiate(jointPrefab) as GameObject;
					joints[i].transform.parent = transform;
					joints[i].name = ((KinectInterop.JointType)i).ToString();
					joints[i].SetActive(false);
				}
			}

			// array holding the skeleton lines
			lines = new LineRenderer[jointsCount];

//			if(linePrefab)
//			{
//				for(int i = 0; i < lines.Length; i++)
//				{
//					lines[i] = Instantiate(linePrefab) as LineRenderer;
//					lines[i].transform.parent = transform;
//					lines[i].gameObject.SetActive(false);
//				}
//			}
		}

		// always mirrored
		initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

		if (!foregroundCamera) 
		{
			// by default - the main camera
			foregroundCamera = Camera.main;
		}

        KinectManager.Instance.AddingUserEvent += AddingUserEvent;
        KinectManager.Instance.RemoveUserEvent += RemoveUserEvent;
	}

    private void AddingUserEvent(long obj)
    {
        CheckBodyHeight();
        CheckBodyHeight2();


    }

    private void RemoveUserEvent(long obj)
    {

    }

	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized() && foregroundCamera)
		{
//			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
//			if(backgroundImage && (backgroundImage.texture == null))
//			{
//				backgroundImage.texture = manager.GetUsersClrTex();
//			}

			// get the background rectangle (use the portrait background, if available)
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;
			
			if(portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}

			// overlay all joints in the skeleton
			if(manager.IsUserDetected(playerIndex))
			{
				long userId = manager.GetUserIdByIndex(playerIndex);
				int jointsCount = manager.GetJointCount();

				for(int i = 0; i < jointsCount; i++)
				{
					int joint = i;

					if(manager.IsJointTracked(userId, joint))
					{
						Vector3 posJoint = manager.GetJointPosColorOverlay(userId, joint, foregroundCamera, backgroundRect);
						//Vector3 posJoint = manager.GetJointPosition(userId, joint);

						if(joints != null)
						{
							// overlay the joint
							if(posJoint != Vector3.zero)
							{
//								if(debugText && joint == 0)
//								{
//									debugText.text = string.Format("{0} - {1}\nRealPos: {2}", 
//									                                       (KinectInterop.JointType)joint, posJoint,
//									                                       manager.GetJointPosition(userId, joint));
//								}
								
								joints[i].SetActive(true);
								joints[i].transform.position = posJoint;

								Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
								rotJoint = initialRotation * rotJoint;
								joints[i].transform.rotation = rotJoint;
							}
							else
							{
								joints[i].SetActive(false);
							}
						}

						if(lines[i] == null && linePrefab != null)
						{
							lines[i] = Instantiate(linePrefab) as LineRenderer;
							lines[i].transform.parent = transform;
							lines[i].gameObject.SetActive(false);
						}

						if(lines[i] != null)
						{
							// overlay the line to the parent joint
							int jointParent = (int)manager.GetParentJoint((KinectInterop.JointType)joint);
							Vector3 posParent = manager.GetJointPosColorOverlay(userId, jointParent, foregroundCamera, backgroundRect);

							if(posJoint != Vector3.zero && posParent != Vector3.zero)
							{
								lines[i].gameObject.SetActive(true);
								
								//lines[i].SetVertexCount(2);
								lines[i].SetPosition(0, posParent);
								lines[i].SetPosition(1, posJoint);
							}
							else
							{
								lines[i].gameObject.SetActive(false);
							}
						}
						
					}
					else
					{
						if(joints != null)
						{
							joints[i].SetActive(false);
						}
						
						if(lines[i] != null)
						{
							lines[i].gameObject.SetActive(false);
						}
					}
				}
            }
		}
	}
	public float CheckBodyHeight()
	{

		KinectManager manager = KinectManager.Instance;

		if (manager && manager.IsInitialized() && foregroundCamera)
		{
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;

			if (portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}

			// overlay all joints in the skeleton
			if (manager.IsUserDetected(playerIndex))
			{
				long userId = manager.GetUserIdByIndex(playerIndex);


				Vector3 neckVector3_2 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.Neck, foregroundCamera, backgroundRect);

				Vector3 headVector3_3 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.Head, foregroundCamera, backgroundRect);

				Vector3 spineShoulderVector3_20 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.SpineShoulder, foregroundCamera, backgroundRect);


				Vector3 spineMidVector3_1 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.SpineMid, foregroundCamera, backgroundRect);


				Vector3 spineBaseVector3_0 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.SpineBase, foregroundCamera, backgroundRect);


				Vector3 kneeLeftVector3_13 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.KneeLeft, foregroundCamera, backgroundRect);

				Vector3 ankleLeftVector3_14 = manager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.AnkleLeft, foregroundCamera, backgroundRect);

				float d1 = Vector3.Distance(headVector3_3, neckVector3_2);

				float d2 = Vector3.Distance(neckVector3_2, spineShoulderVector3_20);

				float d3 = Vector3.Distance(spineShoulderVector3_20, spineMidVector3_1);

				float d4 = Vector3.Distance(spineMidVector3_1, spineBaseVector3_0);

				float d5 = Vector3.Distance(spineBaseVector3_0, kneeLeftVector3_13);

				float d6 = Vector3.Distance(kneeLeftVector3_13, ankleLeftVector3_14);

				float height = d1 + d2 + d3 + d4 + d5 + d6;

				//height *= 1.06667F;

				Debug.Log("身高是 is " + height + "  d1=" + d1 + "  d2=" + d2 + "  d3=" + d3 + "  d4=" + d4 + "  d5=" + d5 + "  d6=" + d6);

			}
		}
		return 0f;
	}

    public float CheckBodyHeight2()
    {
		KinectManager manager = KinectManager.Instance;

		if (manager && manager.IsInitialized() && foregroundCamera)
		{
			//			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			//			if(backgroundImage && (backgroundImage.texture == null))
			//			{
			//				backgroundImage.texture = manager.GetUsersClrTex();
			//			}

			// get the background rectangle (use the portrait background, if available)
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;

			if (portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}

			// overlay all joints in the skeleton
			if (manager.IsUserDetected(playerIndex))
			{
				long userId = manager.GetUserIdByIndex(playerIndex);
				int jointsCount = manager.GetJointCount();

				for (int i = 0; i < jointsCount; i++)
				{
					int joint = i;

					if (manager.IsJointTracked(userId, joint))
					{
						Vector3 posJoint = manager.GetJointPosColorOverlay(userId, joint, foregroundCamera, backgroundRect);
						//Vector3 posJoint = manager.GetJointPosition(userId, joint);

						if (joints != null)
						{
							// overlay the joint
							if (posJoint != Vector3.zero)
							{
								//								if(debugText && joint == 0)
								//								{
								//									debugText.text = string.Format("{0} - {1}\nRealPos: {2}", 
								//									                                       (KinectInterop.JointType)joint, posJoint,
								//									                                       manager.GetJointPosition(userId, joint));
								//								}

								joints[i].SetActive(true);
								joints[i].transform.position = posJoint;

								Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
								rotJoint = initialRotation * rotJoint;
								joints[i].transform.rotation = rotJoint;
							}
							else
							{
								joints[i].SetActive(false);
							}
						}

						if (lines[i] == null && linePrefab != null)
						{
							lines[i] = Instantiate(linePrefab) as LineRenderer;
							lines[i].transform.parent = transform;
							lines[i].gameObject.SetActive(false);
						}

						if (lines[i] != null)
						{
							// overlay the line to the parent joint
							int jointParent = (int)manager.GetParentJoint((KinectInterop.JointType)joint);
							Vector3 posParent = manager.GetJointPosColorOverlay(userId, jointParent, foregroundCamera, backgroundRect);

							if (posJoint != Vector3.zero && posParent != Vector3.zero)
							{
								lines[i].gameObject.SetActive(true);

								//lines[i].SetVertexCount(2);
								lines[i].SetPosition(0, posParent);
								lines[i].SetPosition(1, posJoint);
							}
							else
							{
								lines[i].gameObject.SetActive(false);
							}
						}

					}
					else
					{
						if (joints != null)
						{
							joints[i].SetActive(false);
						}

						if (lines[i] != null)
						{
							lines[i].gameObject.SetActive(false);
						}
					}
				}
				float d1 = Vector3.Distance(joints[3].transform.position, joints[2].transform.position);
				float d2 = Vector3.Distance(joints[2].transform.position, joints[20].transform.position);
				float d3 = Vector3.Distance(joints[20].transform.position, joints[1].transform.position);
				float d4 = Vector3.Distance(joints[1].transform.position, joints[0].transform.position);
				float d5 = Vector3.Distance(joints[0].transform.position, joints[13].transform.position);
				float d6 = Vector3.Distance(joints[13].transform.position, joints[14].transform.position);

				float height = d1 + d2 + d3 + d4 + d5 + d6;

				Debug.Log("身高是 is " + height + "  d1=" + d1 + "  d2=" + d2 + "  d3=" + d3 + "  d4=" + d4 + "  d5=" + d5 + "  d6=" + d6);

                return height;

            }


		}

        return 0f;
    }
}
