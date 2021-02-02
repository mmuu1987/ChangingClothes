using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000031 RID: 49
public class UserBodyBlender : MonoBehaviour
{
	// Token: 0x060000BB RID: 187 RVA: 0x00009E75 File Offset: 0x00008075
	public void SetCopyToTexture(RenderTexture tex)
	{
		this.copyToTex = tex;
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00009E80 File Offset: 0x00008080
	private void OnEnable()
	{
		Camera thisCamera = base.gameObject.GetComponent<Camera>();
		bool flag = thisCamera;
		if (flag)
		{
			thisCamera.clearFlags = CameraClearFlags.Color;
		}
		bool flag2 = this.backrgoundCamera;
		if (flag2)
		{
			this.backrgoundCamera.gameObject.SetActive(false);
		}
		bool flag3 = this.backgroundCamera2;
		if (flag3)
		{
			this.backgroundCamera2.gameObject.SetActive(false);
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00009EF4 File Offset: 0x000080F4
	private void OnDisable()
	{
		Camera thisCamera = base.gameObject.GetComponent<Camera>();
		bool flag = thisCamera;
		if (flag)
		{
			thisCamera.clearFlags = CameraClearFlags.Depth;
		}
		bool flag2 = this.backrgoundCamera;
		if (flag2)
		{
			this.backrgoundCamera.gameObject.SetActive(true);
		}
		bool flag3 = this.backgroundCamera2;
		if (flag3)
		{
			this.backgroundCamera2.gameObject.SetActive(true);
		}
		this.kinectManager.AddingUserEvent -= this.AddingUserEvent;
		this.kinectManager.RemoveUserEvent -= this.RemoveUserEvent;
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00009F98 File Offset: 0x00008198
	private void AddingUserEvent(long obj)
	{
		this._backgroundRect = this.mainCamera.pixelRect;
		PortraitBackground portraitBack = PortraitBackground.Instance;
		bool flag = portraitBack && portraitBack.enabled;
		if (flag)
		{
			this._backgroundRect = portraitBack.GetBackgroundRect();
		}
		this._addUserId = obj;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00009FE7 File Offset: 0x000081E7
	public void ChangeBroundAlpha(float value)
	{
		this.userBlendMat.SetFloat("_HeadRadius", value);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00009FFC File Offset: 0x000081FC
	private void RemoveUserEvent(long obj)
	{
		this._addUserId = -1L;
		this.userBlendMat.SetFloat("_HeadRadius", 100f);
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0000A020 File Offset: 0x00008220
	private void UpdateHeadPos()
	{
		bool flag = this._addUserId > 0L;
		if (flag)
		{
			Vector3 pos = KinectManager.Instance.GetJointPosColorOverlay(this._addUserId, 3, this.mainCamera, this._backgroundRect);
			Vector3 screenPos = this.mainCamera.WorldToScreenPoint(pos);
			screenPos = new Vector3(screenPos.x / 1080f, screenPos.y / 1920f);
			this.userBlendMat.SetVector("_HeadScrPos", new Vector4(screenPos.x, (1f - screenPos.y) * 1.25f, screenPos.z, 0f));
		}
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x0000A0C4 File Offset: 0x000082C4
	private void Start()
	{
		this.kinectManager = KinectManager.Instance;
		this.backManager = BackgroundRemovalManager.Instance;
		bool flag = this.kinectManager && this.kinectManager.IsInitialized() && this.kinectManager.GetSensorData().sensorIntPlatform == KinectInterop.DepthSensorPlatform.KinectSDKv2;
		if (flag)
		{
			Shader userBlendShader = Shader.Find("Custom/UserBlendShader");
			KinectInterop.SensorData sensorData = this.kinectManager.GetSensorData();
			bool flag2 = userBlendShader != null && sensorData != null;
			if (flag2)
			{
				this.userBlendMat = new Material(userBlendShader);
				this.userBlendMat.SetFloat("_ColorResX", (float)sensorData.colorImageWidth);
				this.userBlendMat.SetFloat("_ColorResY", (float)sensorData.colorImageHeight);
				this.userBlendMat.SetFloat("_DepthResX", (float)sensorData.depthImageWidth);
				this.userBlendMat.SetFloat("_DepthResY", (float)sensorData.depthImageHeight);
				this.color2DepthCoords = new Vector2[sensorData.colorImageWidth * sensorData.colorImageHeight];
				this.color2DepthBuffer = new ComputeBuffer(sensorData.colorImageWidth * sensorData.colorImageHeight, 8);
				this.userBlendMat.SetBuffer("_DepthCoords", this.color2DepthBuffer);
				this.depthImageBufferData = new float[sensorData.depthImage.Length];
				this.depthImageBuffer = new ComputeBuffer(sensorData.depthImage.Length, 4);
				this.userBlendMat.SetBuffer("_DepthBuffer", this.depthImageBuffer);
				bool flag3 = this.backgroundImage;
				if (flag3)
				{
					this.userBlendMat.SetTexture("_BackTex", this.backgroundImage.texture);
					this.userBlendMat.SetFloat("_HeadRadius", 100f);
				}
			}
			else
			{
				base.gameObject.GetComponent<UserBodyBlender>().enabled = false;
			}
			this.kinectManager.AddingUserEvent += this.AddingUserEvent;
			this.kinectManager.RemoveUserEvent += this.RemoveUserEvent;
		}
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x0000A2D0 File Offset: 0x000084D0
	public void ChangeBackGround(int selected)
	{
		bool flag = this.backgroundImage;
		if (flag)
		{
			bool flag2 = selected == -1;
			if (flag2)
			{
				this.backgroundImage.texture = this.BackGroundTexs[0];
				this.userBlendMat.SetTexture("_BackTex", this.backgroundImage.texture);
			}
			else
			{
				Debug.Log("选择的背景贴图是： "+selected);

				this.backgroundImage.texture = this.BackGroundTexs[selected + 1];
				bool flag3 = this.userBlendMat != null;
				if (flag3)
				{
					this.userBlendMat.SetTexture("_BackTex", this.backgroundImage.texture);
				}
			}
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0000A374 File Offset: 0x00008574
	private void OnDestroy()
	{
		bool flag = this.color2DepthBuffer != null;
		if (flag)
		{
			this.color2DepthBuffer.Release();
			this.color2DepthBuffer = null;
		}
		bool flag2 = this.depthImageBuffer != null;
		if (flag2)
		{
			this.depthImageBuffer.Release();
			this.depthImageBuffer = null;
		}
		this.color2DepthCoords = null;
		this.depthImageBufferData = null;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x0000A3D4 File Offset: 0x000085D4
	private void Update()
	{
		bool flag = !this.shaderRectInited;
		if (flag)
		{
			PortraitBackground portraitBack = PortraitBackground.Instance;
			bool flag2 = portraitBack && portraitBack.IsInitialized();
			if (flag2)
			{
				this.shaderUvRect = portraitBack.GetShaderUvRect();
			}
			bool flag3 = this.userBlendMat != null;
			if (flag3)
			{
				this.userBlendMat.SetFloat("_ColorOfsX", this.shaderUvRect.x);
				this.userBlendMat.SetFloat("_ColorMulX", this.shaderUvRect.width);
				this.userBlendMat.SetFloat("_ColorOfsY", this.shaderUvRect.y);
				this.userBlendMat.SetFloat("_ColorMulY", this.shaderUvRect.height);
				this.userBlendMat.SetFloat("_Brightness", GlobalSettings.Brightness);
				this.userBlendMat.SetFloat("_Contrast", GlobalSettings.Contrast);
				this.userBlendMat.SetFloat("_Saturation", GlobalSettings.Saturation);
			}
			this.shaderRectInited = true;
		}
		bool flag4 = this.kinectManager && this.kinectManager.IsInitialized();
		if (flag4)
		{
			KinectInterop.SensorData sensorData = this.kinectManager.GetSensorData();
			bool flag5 = sensorData != null && sensorData.depthImage != null && (sensorData.colorImageTexture2D || sensorData.colorImageTexture) && this.userBlendMat != null && this.lastDepthFrameTime != sensorData.lastDepthFrameTime;
			if (flag5)
			{
				this.lastDepthFrameTime = sensorData.lastDepthFrameTime;
				bool flag6 = KinectInterop.MapColorFrameToDepthCoords(sensorData, ref this.color2DepthCoords);
				if (flag6)
				{
					this.color2DepthBuffer.SetData(this.color2DepthCoords);
				}
				for (int i = 0; i < sensorData.depthImage.Length; i++)
				{
					ushort depth = sensorData.depthImage[i];
					this.depthImageBufferData[i] = (float)depth;
				}
				this.depthImageBuffer.SetData(this.depthImageBufferData);
				Texture colorTex = (this.backManager && sensorData.color2DepthTexture) ? sensorData.color2DepthTexture : (sensorData.colorImageTexture2D ? sensorData.colorImageTexture2D : sensorData.colorImageTexture);
				this.userBlendMat.SetTexture("_ColorTex", colorTex);
			}
		}
		this.UpdateHeadPos();
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x0000A654 File Offset: 0x00008854
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		bool flag = this.userBlendMat != null;
		if (flag)
		{
			this.userBlendMat.SetFloat("_Threshold", this.depthThreshold);
			Graphics.Blit(source, destination, this.userBlendMat);
			bool flag2 = this.copyToTex != null;
			if (flag2)
			{
				Graphics.Blit(destination, this.copyToTex);
			}
		}
	}

	// Token: 0x04000155 RID: 341
	[Tooltip("Allowed depth distance between the user and the clothing model, in meters.")]
	[Range(-0.5f, 0.5f)]
	public float depthThreshold = 0.1f;

	// Token: 0x04000156 RID: 342
	[Tooltip("RawImage used to display the scene background.")]
	public RawImage backgroundImage;

	// Token: 0x04000157 RID: 343
	[Tooltip("Camera used to render the 1st scene background. This background camera gets disabled, when this component is enabled.")]
	public Camera backrgoundCamera;

	// Token: 0x04000158 RID: 344
	[Tooltip("Camera used to render the 2nd scene background (users). This background camera gets disabled, when this component is enabled.")]
	public Camera backgroundCamera2;

	// Token: 0x04000159 RID: 345
	public Material userBlendMat;

	// Token: 0x0400015A RID: 346
	private KinectManager kinectManager;

	// Token: 0x0400015B RID: 347
	private BackgroundRemovalManager backManager;

	// Token: 0x0400015C RID: 348
	private long lastDepthFrameTime;

	// Token: 0x0400015D RID: 349
	private Vector2[] color2DepthCoords;

	// Token: 0x0400015E RID: 350
	private ComputeBuffer color2DepthBuffer;

	// Token: 0x0400015F RID: 351
	private float[] depthImageBufferData;

	// Token: 0x04000160 RID: 352
	private ComputeBuffer depthImageBuffer;

	// Token: 0x04000161 RID: 353
	public Rect shaderUvRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04000162 RID: 354
	private bool shaderRectInited = false;

	// Token: 0x04000163 RID: 355
	private RenderTexture copyToTex;

	// Token: 0x04000164 RID: 356
	public Camera mainCamera;

	// Token: 0x04000165 RID: 357
	private Rect _backgroundRect;

	// Token: 0x04000166 RID: 358
	private long _addUserId = -1L;

	// Token: 0x04000167 RID: 359
	public Texture2D[] BackGroundTexs;
}
