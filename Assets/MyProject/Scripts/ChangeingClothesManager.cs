using RenderHeads.Media.AVProMovieCapture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

// Token: 0x02000095 RID: 149
public class ChangeingClothesManager : MonoBehaviour
{
	// Token: 0x04000638 RID: 1592
	public static ChangeingClothesManager Instance;

	// Token: 0x04000639 RID: 1593
	public GameObject HlepTipGameObject;

	// Token: 0x0400063A RID: 1594
	public GameObject StandbyGameObject;

	// Token: 0x0400063B RID: 1595
	public UserBodyBlender userBodyBlender;

	// Token: 0x0400063C RID: 1596
	public EditorModel editorModel;

	// Token: 0x0400063D RID: 1597
	public float StandbyTime = 7f;

	// Token: 0x0400063E RID: 1598
	public bool IsUseStandby = false;

	// Token: 0x0400063F RID: 1599
	public RawImage ShotHeadImage;

	// Token: 0x04000640 RID: 1600
	public float ShotWidth = 300f;

	// Token: 0x04000641 RID: 1601
	public float ShotHeight = 350f;

	// Token: 0x04000642 RID: 1602
	private bool _isFirstDiscriminate = false;

	// Token: 0x04000643 RID: 1603
	private Coroutine _coroutine;

	// Token: 0x04000644 RID: 1604
	private Texture2D _screenShot;

	// Token: 0x04000645 RID: 1605
	private long _curId;

	// Token: 0x04000646 RID: 1606
	public Camera mainCamera;

	// Token: 0x04000647 RID: 1607
	public int playerIndex;

	// Token: 0x04000648 RID: 1608
	public SwapFaceManager swapFaceManager;

	

	// Token: 0x060005F9 RID: 1529 RVA: 0x00043950 File Offset: 0x00041B50
	private void Awake()
	{
		bool flag = ChangeingClothesManager.Instance != null;
		if (flag)
		{
			throw new UnityException("单例没有释放");
		}
		ChangeingClothesManager.Instance = this;
		GlobalSettings.ReadXml();
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00043984 File Offset: 0x00041B84
	private void Start()
	{
		KinectManager.Instance.AddingUserEvent += this.AddingUserEvent;
		KinectManager.Instance.RemoveUserEvent += this.RemoveUserEvent;
		NetManager.Instance.SwapFaceCompletedEvent += this.SwapFaceCompletedEvent;
		NetManager.Instance.RecevieUDPDataEvent += this.RecevieUDPDataEvent;
		

		this._isFirstDiscriminate = this.IsUseStandby;
		bool flag = !this.IsUseStandby;
		if (flag)
		{
			base.StartCoroutine(this.CanceStandby(null));
		}
		else
		{
			this.StartStandby();
		}

		Screen.SetResolution(2160, 3840, true);
		
	}

   

    // Token: 0x060005FB RID: 1531 RVA: 0x00043A34 File Offset: 0x00041C34
    private void RecevieUDPDataEvent(string obj)
	{
		bool flag = !string.IsNullOrEmpty(obj);
		if (flag)
		{
			bool flag2 = obj == "close";
			if (flag2)
			{
				this.CloseKinectTrack();
			}
			else
			{
				bool flag3 = obj == "open";
				if (flag3)
				{
					this.OpenKinectTrack();
				}
				else
				{
					Debug.LogError("收到了错误指令 " + obj);
				}
			}
		}
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00043A96 File Offset: 0x00041C96
	private void CloseKinectTrack()
	{
		KinectManager.Instance.maxTrackedUsers = 0;
		this.StartStandby();
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00043AAB File Offset: 0x00041CAB
	private void OpenKinectTrack()
	{
		KinectManager.Instance.maxTrackedUsers = 1;
		this.StartStandby();
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00043AC0 File Offset: 0x00041CC0
	private void AddingUserEvent(long obj)
	{
		bool isFirstDiscriminate = this._isFirstDiscriminate;
		if (isFirstDiscriminate)
		{
			base.StartCoroutine(this.CanceStandby(new Action(this.StartComputeStandby)));
			this._isFirstDiscriminate = false;
			Debug.Log("待机");
		}
		else
		{
			this.HlepTipGameObject.SetActive(false);
			//Debug.Log("识别到人物");
			this.userBodyBlender.ChangeBackGround(-1);
			this.StopComputeStandby();
			this.CheckBodyHeight();
			this.GetHeadImage(obj);
			this.swapFaceManager.ShowRoleUi(true, obj);
			this._curId = obj;
            MyInternaction.Instance.EnableHand(true,1f,true);
		}
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00043B5C File Offset: 0x00041D5C
	private void RemoveUserEvent(long obj)
	{
		this.HlepTipGameObject.SetActive(true);
		this.userBodyBlender.ChangeBackGround(-1);
		this.StartComputeStandby();

        MyInternaction.Instance.MouseLeftImage.rectTransform.anchoredPosition = Vector2.zero;
        MyInternaction.Instance.MouseRightImage.rectTransform.anchoredPosition = Vector2.zero;
		this.swapFaceManager.ShowRoleUi(false, obj);
		bool flag = this._curId == obj;
		if (flag)
		{
			this._curId = -1L;
		}
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00043BAE File Offset: 0x00041DAE
	private void StartStandby()
	{
		this.StandbyGameObject.SetActive(true);
		this.HlepTipGameObject.SetActive(false);
		KinectManager.Instance.playerCalibrationPose = KinectGestures.Gestures.None;
		KinectManager.Instance.ClearKinectUsers();
		this._isFirstDiscriminate = true;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x00043BE8 File Offset: 0x00041DE8
	private IEnumerator CanceStandby(Action action)
	{
		yield return new WaitForEndOfFrame();
		this.StandbyGameObject.SetActive(false);
		this.HlepTipGameObject.SetActive(true);
		KinectManager.Instance.ClearKinectUsers();
		this.StopComputeStandby();
		KinectManager.Instance.playerCalibrationPose = KinectGestures.Gestures.ObliqueHand;
		this.userBodyBlender.ChangeBackGround(-1);
		bool flag = action != null;
		if (flag)
		{
			action();
		}
		yield break;
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00043C00 File Offset: 0x00041E00
	private void StartComputeStandby()
	{
		bool flag = !this.IsUseStandby;
		if (!flag)
		{
			bool flag2 = this._coroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._coroutine);
			}
			this._coroutine = base.StartCoroutine(GlobalSettings.WaitTime(this.StandbyTime, new Action(this.StartStandby)));
		}
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00043C5C File Offset: 0x00041E5C
	private void StopComputeStandby()
	{
		bool flag = this._coroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._coroutine);
		}
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x00043C84 File Offset: 0x00041E84
	private void OnDestroy()
	{
		bool flag = KinectManager.Instance != null;
		if (flag)
		{
			KinectManager.Instance.AddingUserEvent -= this.AddingUserEvent;
			KinectManager.Instance.RemoveUserEvent -= this.RemoveUserEvent;
		}
		NetManager.Instance.SwapFaceCompletedEvent -= this.SwapFaceCompletedEvent;
		NetManager.Instance.RecevieUDPDataEvent -= this.RecevieUDPDataEvent;
		
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00043D00 File Offset: 0x00041F00
	private void CheckUserRange(float x, float y, float z)
	{
		bool flag = KinectManager.Instance.GetAllUserIds().Count > 0;
		if (flag)
		{
			List<long> ids = KinectManager.Instance.GetAllUserIds();
			foreach (long id in ids)
			{
				Vector3 pos = KinectManager.Instance.GetUserPosition(id);
				bool flag2 = Mathf.Abs(pos.x) > x;
				if (flag2)
				{
					KinectManager.Instance.ClearKinectUsers();
				}
			}
		}
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00043D9C File Offset: 0x00041F9C
	public float CheckBodyHeight()
	{
		KinectManager manager = KinectManager.Instance;
		bool flag = manager && manager.IsInitialized() && this.mainCamera;
		if (flag)
		{
			Rect backgroundRect = this.mainCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;
			bool flag2 = portraitBack && portraitBack.enabled;
			if (flag2)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}
			bool flag3 = manager.IsUserDetected(this.playerIndex);
			if (flag3)
			{
				long userId = manager.GetUserIdByIndex(this.playerIndex);
				Vector3 neckVector3_2 = manager.GetJointPosColorOverlay(userId, 2, this.mainCamera, backgroundRect);
				Vector3 headVector3_3 = manager.GetJointPosColorOverlay(userId, 3, this.mainCamera, backgroundRect);
				Vector3 spineShoulderVector3_20 = manager.GetJointPosColorOverlay(userId, 20, this.mainCamera, backgroundRect);
				Vector3 spineMidVector3_ = manager.GetJointPosColorOverlay(userId, 1, this.mainCamera, backgroundRect);
				Vector3 spineBaseVector3_0 = manager.GetJointPosColorOverlay(userId, 0, this.mainCamera, backgroundRect);
				Vector3 kneeLeftVector3_13 = manager.GetJointPosColorOverlay(userId, 13, this.mainCamera, backgroundRect);
				Vector3 ankleLeftVector3_14 = manager.GetJointPosColorOverlay(userId, 14, this.mainCamera, backgroundRect);
				float d = Vector3.Distance(headVector3_3, neckVector3_2);
				float d2 = Vector3.Distance(neckVector3_2, spineShoulderVector3_20);
				float d3 = Vector3.Distance(spineShoulderVector3_20, spineMidVector3_);
				float d4 = Vector3.Distance(spineMidVector3_, spineBaseVector3_0);
				float d5 = Vector3.Distance(spineBaseVector3_0, kneeLeftVector3_13);
				float d6 = Vector3.Distance(kneeLeftVector3_13, ankleLeftVector3_14);
				float height = d + d2 + d3 + d4 + d5 + d6;
				//Debug.Log(string.Concat(new object[]
				//{
				//	"身高是 is ",
				//	height,
				//	"  d1=",
				//	d,
				//	"  d2=",
				//	d2,
				//	"  d3=",
				//	d3,
				//	"  d4=",
				//	d4,
				//	"  d5=",
				//	d5,
				//	"  d6=",
				//	d6
				//}));
			}
		}
		this.CheckUserRange(0.5f, 0.45f, 3f);
		return 0f;
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00043FBC File Offset: 0x000421BC
	private void GeneraterFailed(string error)
	{
		this.swapFaceManager.LoadingTip(true, error);
		base.StartCoroutine(this.WaitTime(3f, delegate
		{
			KinectManager.Instance.ClearKinectUsers();
		}));
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x00044009 File Offset: 0x00042209
	private IEnumerator WaitTime(float time, Action action)
	{
		yield return new WaitForSeconds(time);
		bool flag = action != null;
		if (flag)
		{
			action();
		}
		yield break;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00044028 File Offset: 0x00042228
	private void SwapFaceCompletedEvent(byte[] data)
	{
		bool flag = this._curId == -1L;
		if (!flag)
		{
			bool flag2 = data == null;
			if (flag2)
			{
				this.GeneraterFailed("网络断开");
			}
			else
			{
				bool flag3 = data.Length <= 256;
				if (flag3)
				{
					this.GeneraterFailed("脸部数据错误");
				}
				else
				{
					try
					{
						BinaryFormatter formatter = new BinaryFormatter();
						MemoryStream rems = new MemoryStream(data);
						HeadInfo headInfo = (HeadInfo)formatter.Deserialize(rems);
						GameObject newHead = new GameObject("newHead");
						Mesh meshNew = new Mesh();
						
						meshNew.vertices = HandleHeadVertexData(headInfo.GetVertices());
						meshNew.uv = headInfo.GetUVs();
						meshNew.triangles = headInfo.GetTriangles();
						meshNew.RecalculateTangents();
						meshNew.RecalculateNormals();
						Texture2D texNew = new Texture2D(4, 4);
						texNew.LoadImage(headInfo.TexBytes);
						texNew.Apply();
						MeshFilter mfNew = newHead.AddComponent<MeshFilter>();
						mfNew.mesh = meshNew;
						MeshRenderer mrNew = newHead.AddComponent<MeshRenderer>();
						mrNew.material = new Material(Shader.Find("AvatarUnlitShader"))
						{
							renderQueue = 2000
						};
						mrNew.material.mainTexture = texNew;
						rems.Dispose();
						this.userBodyBlender.ChangeBroundAlpha(0f);
						this.swapFaceManager.LoadModel(newHead, this._curId);
					}
					catch (Exception e)
					{
						Debug.Log(e.ToString());
						this.GeneraterFailed("未知错误");
					}
				}
			}
		}
	}
	/// <summary>
	/// 处理头部定点数据   
	/// </summary>
	private Vector3 [] HandleHeadVertexData(Vector3 [] data)
    {

		#region 数据1
		int[] data1 = new int[24];
		data1[0] = 546;
		data1[1] = 547;
		data1[2] = 548;
		data1[3] = 438;
		data1[4] = 435;
		data1[5] = 545;
		data1[6] = 544;
		data1[7] = 588;
		data1[8] = 728;
		data1[9] = 1206;
		data1[10] = 1203;
		data1[11] = 1202;
		data1[12] = 1046;
		data1[13] = 1049;
		data1[14] = 1160;
		data1[15] = 1159;
		data1[16] = 1158;
		data1[17] = 1169;
		data1[18] = 1166;
		data1[19] = 1165;
		data1[20] = 727;
		data1[21] = 558;
		data1[22] = 550;
		data1[23] = 549;
		#endregion

	
		//求得中心点
		Vector3 center1 = GetCenter(data1, data);

		float scale = 0.15f;


		for (int i = 0; i < data.Length; i++)
		{
			foreach (var item in data1)
			{
				if (item - 1 == i)
				{
					data[i] += new Vector3(0f, -0.05f, 0f);//向下偏移
					//然后缩放
					Vector3 dir = center1 - data[i] ;
					dir.y = 0f;
					dir *= scale;
					data[i] += dir;
					data[i] += new Vector3(0f, 0f, 0.005f);
				}
			}
		}



		#region 数据2
		int[] data2 = new int[24];
		data2[0] = 560;
		data2[1] = 463;
		data2[2] = 456;
		data2[3] = 457;
		data2[4] = 437;
		data2[5] = 436;
		data2[6] = 586;
		data2[7] = 579;
		data2[8] = 580;
		data2[9] = 729;
		data2[10] = 1195;
		data2[11] = 1194;
		data2[12] = 1201;
		data2[13] = 1047;
		data2[14] = 1048;
		data2[15] = 1070;
		data2[16] = 1069;
		data2[17] = 1076;
		data2[18] = 1168;
		data2[19] = 1167;
		data2[20] = 1164;
		data2[21] = 1170;
		data2[22] = 557;
		data2[23] = 559;
		#endregion

		//求得中心点
		Vector3 center2 = GetCenter(data2, data);

		float scale2 = 0.2f;


		for (int i = 0; i < data.Length; i++)
		{
			foreach (var item in data2)
			{
				if (item - 1 == i)
				{
					data[i] += new Vector3(0f, -0.01f, 0f);//向下偏移
														   //然后缩放
					Vector3 dir = center2 - data[i];
					dir.y = 0f;
					dir *= scale2;
					data[i] += dir;
				}
			}
		}


		#region 数据3
		int[] data3 = new int[25];
		data3[0] = 312;
		data3[1] = 303;
		data3[2] = 304;
		data3[3] = 451;
		data3[4] = 462;
		data3[5] = 461;
		data3[6] = 585;
		data3[7] = 582;
		data3[8] = 581;
		data3[9] = 1205;
		data3[10] = 1196;
		data3[11] = 1197;
		data3[12] = 1200;
		data3[13] = 1074;
		data3[14] = 1075;
		data3[15] = 1064;
		data3[16] = 902;
		data3[17] = 901;
		data3[18] = 910;
		data3[19] = 1163;
		data3[20] = 556;
		data3[21] = 553;
		data3[22] = 1162;
		data3[23] = 554;
		data3[24] = 552;
		#endregion


		//求得中心点  
		Vector3 center3 = GetCenter(data3, data);

		float scale3 = 0.1f;


		for (int i = 0; i < data.Length; i++)
		{
			foreach (var item in data3)
			{
				if (item - 1 == i)
				{
					data[i] += new Vector3(0f, -0.01f, 0f);//向下偏移
														   //然后缩放
					Vector3 dir = center3 - data[i];
					dir.y = 0f;
					dir *= scale3;
					data[i] += dir;
				}
			}
		}

		//
		data[1072 - 1].x += 0.01f;
		data[1071 - 1].x += 0.01f;
		data[1073 - 1].x += 0.01f;

		data[1074 - 1].x += 0.01f;
		data[1200 - 1].x += 0.01f;



		data[459 - 1].x -= 0.01f;
		data[458 - 1].x -= 0.02f;
		data[460 - 1].x -= 0.01f;

		data[462 - 1].x -= 0.01f;
		data[461 - 1].x -= 0.01f;


		data[1075 - 1].x += 0.01f;
		//data[553 - 1].z += 0.02f;
		//data[556 - 1].z += 0.02f;
		//data[1163 - 1].z += 0.02f;

		return data;





	}
	private Vector3  GetCenter(int [] indexData,Vector3 [] data)
    {
		float x = 0f;
		float y = 0f;
		float z = 0f;
		//求出中心点
		for (int i = 0; i < data.Length; i++)
		{
			foreach (var item in indexData)
			{
				if (item - 1 == i)
				{
					//data[i] += new Vector3(0f, -0.1f, 0f);//向下偏移
					x += data[i].x;
					y += data[i].y;
					z += data[i].z;

				}
			}
		}
		x = x / indexData.Length;
		y = y / indexData.Length;
		z = z / indexData.Length;

		//求得中心点
		Vector3 center = new Vector3(x, y, z);

		return center;
	}
	
	// Token: 0x0600060A RID: 1546 RVA: 0x000441C8 File Offset: 0x000423C8
	public void StartSwapFace()
    {
        MyInternaction.Instance.EnableHand(false,0f,false);
		NetManager.Instance.SendFaceDataMessage(this._screenShot);
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000441DC File Offset: 0x000423DC
	private void GetHeadImage(long obj)
	{
		Rect backgroundRect = this.mainCamera.pixelRect;
		PortraitBackground portraitBack = PortraitBackground.Instance;
		bool flag = portraitBack && portraitBack.enabled;
		if (flag)
		{
			backgroundRect = portraitBack.GetBackgroundRect();
		}
		Vector3 pos = KinectManager.Instance.GetJointPosColorOverlay(obj, 3, this.mainCamera, backgroundRect);
		Vector3 screenPos = this.mainCamera.WorldToScreenPoint(pos);
		//Debug.Log(string.Concat(new object[]
		//{
		//	"screenPos  ",
		//	screenPos,
		//	"   Screen.width is",
		//	Screen.width
		//}));
		Rect rect = Rect.zero;
		bool flag2 = this.ShotHeadImage == null;
		if (!flag2)
		{
			bool flag3 = screenPos.x + this.ShotWidth / 2f > (float)Screen.width || screenPos.y + this.ShotHeight / 2f > (float)Screen.height;
			if (flag3)
			{
				bool flag4 = this._coroutine != null;
				if (flag4)
				{
					base.StopCoroutine(this._coroutine);
				}
				this._coroutine = base.StartCoroutine(this.WaitTime(1f, delegate
				{
					this.AddingUserEvent(obj);
				}));
				Debug.Log("重新开始截图头像");
			}
			else
			{
				rect = new Rect(screenPos.x - this.ShotWidth / 2f, screenPos.y - this.ShotHeight / 2f, this.ShotWidth, this.ShotHeight);
				bool flag5 = this._coroutine != null;
				if (flag5)
				{
					base.StopCoroutine(this._coroutine);
				}
				this._coroutine = base.StartCoroutine(this.CaptureScreenshot2(rect));
			}
		}
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x000443A9 File Offset: 0x000425A9
	private IEnumerator CaptureScreenshot2(Rect rect)
	{
		Object.Destroy(this._screenShot);
		this._screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		this._screenShot.ReadPixels(rect, 0, 0);
		this._screenShot.Apply();
		this.ShotHeadImage.texture = this._screenShot;
		this.ShotHeadImage.rectTransform.sizeDelta = new Vector2(this.ShotWidth / 2f, this.ShotHeight / 2f);
		bool flag = this._coroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._coroutine);
		}
		yield break;
	}

	
	// Token: 0x0600060D RID: 1549 RVA: 0x000443C0 File Offset: 0x000425C0
	private void Update()
	{
		this.CheckUserRange(0.65f, 0.45f, 3f);
		bool keyDown = Input.GetKeyDown(KeyCode.O);
		if (keyDown)
		{
			this.editorModel.gameObject.SetActive(true);
		}
		else
		{
			bool keyDown2 = Input.GetKeyDown(KeyCode.C);
			if (keyDown2)
			{
				this.editorModel.gameObject.SetActive(false);
			}
		}
	}


}
