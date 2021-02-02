using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Token: 0x0200002F RID: 47
public class ModelSelector : MonoBehaviour
{
	// Token: 0x060000A9 RID: 169 RVA: 0x00008BCC File Offset: 0x00006DCC
	public void SetActiveSelector(bool bActive)
	{
		this.activeSelector = bActive;
		bool flag = this.MyDressingUI;
		if (flag)
		{
			this.MyDressingUI.SetActive(bActive);
		}
		this.activeSelector = bActive;
		bool flag2 = this.dressingMenu;
		if (flag2)
		{
		}
		bool flag3 = !this.activeSelector && !this.keepSelectedModel;
		if (flag3)
		{
			this.DestroySelectedModel();
		}
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00008C38 File Offset: 0x00006E38
	public GameObject GetSelectedModel()
	{
		return this.selModel;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00008C50 File Offset: 0x00006E50
	public void DestroySelectedModel()
	{
		bool flag = this.selModel;
		if (flag)
		{
			AvatarController ac = this.selModel.GetComponent<AvatarController>();
			KinectManager km = KinectManager.Instance;
			bool flag2 = ac != null && km != null;
			if (flag2)
			{
				km.avatarControllers.Remove(ac);
			}
			bool flag3 = this.selHat != null;
			if (flag3)
			{
				this.selHat.SetActive(false);
			}
			Object.Destroy(this.selModel);
			this.selModel = null;
			this.selHat = null;
			this.prevSelected = -1;
		}
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00008CE8 File Offset: 0x00006EE8
	public void SelectNextModel()
	{
		this.selected++;
		bool flag = this.selected >= this.numberOfModels;
		if (flag)
		{
			this.selected = 0;
		}
		this.OnDressingItemSelected(this.selected);
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00008D30 File Offset: 0x00006F30
	public void SelectPrevModel()
	{
		this.selected--;
		bool flag = this.selected < 0;
		if (flag)
		{
			this.selected = this.numberOfModels - 1;
		}
		this.OnDressingItemSelected(this.selected);
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00008D74 File Offset: 0x00006F74
	private void Start()
	{
		bool flag = this.dressingMenu;
		if (flag)
		{
			Transform dressingHeaderText = this.dressingMenu.transform.Find("Header/Text");
			bool flag2 = dressingHeaderText;
			if (flag2)
			{
				this.dressingMenuTitle = dressingHeaderText.gameObject.GetComponent<Text>();
			}
			Transform dressingViewportContent = this.dressingMenu.transform.Find("Scroll View/Viewport/Content");
			bool flag3 = dressingViewportContent;
			if (flag3)
			{
				this.dressingMenuContent = dressingViewportContent.gameObject.GetComponent<RectTransform>();
			}
		}
		this.modelNames = new string[this.numberOfModels];
		this.modelThumbs = new Texture2D[this.numberOfModels];
		this.modelSprites = new Sprite[this.numberOfModels];
		this.modelHat = new GameObject[this.numberOfModels];
		this.dressingPanels.Clear();
		for (int i = 0; i < this.numberOfModels; i++)
		{
			this.modelNames[i] = string.Format("{0:0000}", i);
			string previewPath = this.modelCategory + "/" + this.modelNames[i] + "/preview";
			string hatPath = this.modelCategory + "/" + this.modelNames[i] + "/Hat";
			Texture2D resPreview = Resources.Load(previewPath, typeof(Texture2D)) as Texture2D;
			bool flag4 = resPreview == null;
			if (flag4)
			{
			}
			this.modelThumbs[i] = resPreview;
			this.modelSprites[i] = Sprite.Create(resPreview, new Rect(0f, 0f, (float)resPreview.width, (float)resPreview.height), new Vector2(0.5f, 0.5f));
			GameObject go = Resources.Load<GameObject>(hatPath);
			GameObject hat = Object.Instantiate<GameObject>(go, null);
			hat.name = this.modelNames[i] + "_hat";
			hat.gameObject.SetActive(false);
			//hat.GetComponent<ModelHatController>().foregroundCamera = Camera.main;
			this.modelHat[i] = hat;
			this.InstantiateDressingItem(i);
		}
		this.InintDressingItem();
		bool flag5 = this.numberOfModels > 0;
		if (flag5)
		{
		}
		bool flag6 = this.dressingMenuTitle;
		if (flag6)
		{
			this.dressingMenuTitle.text = this.modelCategory;
		}
		this.curScaleFactor = this.bodyScaleFactor + this.bodyWidthFactor + this.armScaleFactor + this.legScaleFactor;
		this.curModelOffset = this.verticalOffset + this.forwardOffset + (this.applyMuscleLimits ? 1f : 0f);
		this.SetActiveSelector(false);
		this.editorModel.UpdateEvent += this.SetInfo;
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00009044 File Offset: 0x00007244
	private void Update()
	{
		bool flag = this.activeSelector && this.selected >= 0 && this.selected < this.modelNames.Length && this.prevSelected != this.selected;
		if (flag)
		{
			KinectManager kinectManager = KinectManager.Instance;
			bool flag2 = kinectManager && kinectManager.IsInitialized() && kinectManager.IsUserDetected(this.playerIndex);
			if (flag2)
			{
			}
		}
		bool flag3 = this.selModel != null;
		if (flag3)
		{
			float curMuscleLimits = this.applyMuscleLimits ? 1f : 0f;
			bool flag4 = Mathf.Abs(this.curModelOffset - (this.verticalOffset + this.forwardOffset + curMuscleLimits)) >= 0.001f;
			if (flag4)
			{
				this.curModelOffset = this.verticalOffset + this.forwardOffset + curMuscleLimits;
				AvatarController ac = this.selModel.GetComponent<AvatarController>();
				bool flag5 = ac != null;
				if (flag5)
				{
					ac.verticalOffset = this.verticalOffset;
					ac.forwardOffset = this.forwardOffset;
					ac.applyMuscleLimits = this.applyMuscleLimits;
				}
			}
			bool flag6 = Mathf.Abs(this.curScaleFactor - (this.bodyScaleFactor + this.bodyWidthFactor + this.armScaleFactor + this.legScaleFactor)) >= 0.001f;
			if (flag6)
			{
				this.curScaleFactor = this.bodyScaleFactor + this.bodyWidthFactor + this.armScaleFactor + this.legScaleFactor;
				AvatarScaler scaler = this.selModel.GetComponent<AvatarScaler>();
				bool flag7 = scaler != null;
				if (flag7)
				{
					scaler.continuousScaling = this.continuousScaling;
					scaler.bodyScaleFactor = this.bodyScaleFactor;
					scaler.bodyWidthFactor = this.bodyWidthFactor;
					scaler.armScaleFactor = this.armScaleFactor;
					scaler.legScaleFactor = this.legScaleFactor;
				}
			}
		}
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00009228 File Offset: 0x00007428
	private Texture2D CreatePreviewTexture(byte[] btImage)
	{
		Texture2D tex = new Texture2D(4, 4);
		bool flag = btImage != null;
		if (flag)
		{
			tex.LoadImage(btImage);
		}
		return tex;
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00009258 File Offset: 0x00007458
	private void InintDressingItem()
	{
		bool flag = this.MyDressingUI;
		if (flag)
		{
			Button nextTrigger = this.MyDressingUI.transform.Find("Next/NextBtn").GetComponent<Button>();
			EventTrigger.Entry nextEntry = new EventTrigger.Entry();
			nextEntry.eventID = EventTriggerType.Select;
			Image roleNextimg = this.MyDressingUI.transform.Find("Next/Role").GetComponent<Image>();
			Image rolePrevioustimg = this.MyDressingUI.transform.Find("Previous/Role").GetComponent<Image>();
			nextTrigger.onClick.AddListener(delegate ()
			{
				this.selected++;
				int i = this.selected % this.modelSprites.Length;
				Debug.Log(string.Concat(new object[]
				{
					"选择的ID is ",
					this.selected,
					"   k is  ",
					i
				}));
				this.OnDressingItemSelected(i);
				bool flag2 = i >= this.modelSprites.Length - 1;
				if (flag2)
				{
					roleNextimg.sprite = this.modelSprites[0];
					rolePrevioustimg.sprite = this.modelSprites[this.modelSprites.Length - 2];
				}
				else
				{
					bool flag3 = i - 1 < 0;
					if (flag3)
					{
						rolePrevioustimg.sprite = this.modelSprites[this.modelSprites.Length - 1];
						roleNextimg.sprite = this.modelSprites[i + 1];
					}
					else
					{
						roleNextimg.sprite = this.modelSprites[i + 1];
						rolePrevioustimg.sprite = this.modelSprites[i - 1];
					}
				}
			});
			Button previousTrigger = this.MyDressingUI.transform.Find("Previous/PreviousBtn").GetComponent<Button>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Select;
			previousTrigger.onClick.AddListener(delegate ()
			{
				this.selected--;
				int i = this.selected % this.modelSprites.Length;
				Debug.Log(string.Concat(new object[]
				{
					"选择的ID is ",
					this.selected,
					"   k is  ",
					i
				}));
				this.OnDressingItemSelected(i);
				bool flag2 = i >= this.modelSprites.Length - 1;
				if (flag2)
				{
					roleNextimg.sprite = this.modelSprites[0];
					rolePrevioustimg.sprite = this.modelSprites[this.modelSprites.Length - 2];
				}
				else
				{
					bool flag3 = i - 1 < 0;
					if (flag3)
					{
						rolePrevioustimg.sprite = this.modelSprites[this.modelSprites.Length - 1];
						roleNextimg.sprite = this.modelSprites[i + 1];
					}
					else
					{
						roleNextimg.sprite = this.modelSprites[i + 1];
						rolePrevioustimg.sprite = this.modelSprites[i - 1];
					}
				}
			});
			this.selected = this.modelSprites.Length * 10000;
			this.OnDressingItemSelected(0);
		}
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x0000936C File Offset: 0x0000756C
	public void RestData()
	{
		bool flag = this.MyDressingUI;
		if (flag)
		{
			this.selected = this.modelSprites.Length * 10000;
			Image roleNextimg = this.MyDressingUI.transform.Find("Next/Role").GetComponent<Image>();
			Image rolePrevioustimg = this.MyDressingUI.transform.Find("Previous/Role").GetComponent<Image>();
			roleNextimg.sprite = this.modelSprites[1];
			rolePrevioustimg.sprite = this.modelSprites[this.modelSprites.Length - 1];
		}
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x000093FC File Offset: 0x000075FC
	private void InstantiateDressingItem(int i)
	{
		bool flag = !this.dressingItemPrefab && i >= 0 && i < this.numberOfModels;
		if (!flag)
		{
			GameObject dressingItemInstance = Object.Instantiate<GameObject>(this.dressingItemPrefab);
			GameObject dressingImageObj = dressingItemInstance.transform.Find("DressingImagePanel").gameObject;
			dressingImageObj.GetComponentInChildren<RawImage>().texture = this.modelThumbs[i];
			bool flag2 = !string.IsNullOrEmpty(this.modelNames[i]);
			if (flag2)
			{
				EventTrigger trigger = dressingItemInstance.GetComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.Select;
				entry.callback.AddListener(delegate (BaseEventData eventData)
				{
					this.OnDressingItemSelected(i);
				});
				trigger.triggers.Add(entry);
			}
			bool flag3 = this.dressingMenuContent;
			if (flag3)
			{
				dressingItemInstance.transform.SetParent(this.dressingMenuContent, false);
			}
			this.dressingPanels.Add(dressingItemInstance);
		}
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x0000951C File Offset: 0x0000771C
	public void OnDressingItemSelected(int i)
	{
		bool flag = i >= 0 && i < this.modelNames.Length && this.prevSelected != i;
		if (flag)
		{
			this.LoadDressingModel(this.modelNames[i], i);
		}
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00009560 File Offset: 0x00007760
	public void SetInfo()
	{
		AvatarController ac = this.selModel.GetComponent<AvatarController>();
		bool flag = ac != null;
		if (flag)
		{
			ac.verticalOffset = this.CurInfo.Vertacal;
			ac.forwardOffset = this.CurInfo.Farward;
		}
		AvatarScaler scaler = this.selModel.GetComponent<AvatarScaler>();
		bool flag2 = scaler != null;
		if (flag2)
		{
			scaler.bodyScaleFactor = this.CurInfo.BodyScale;
			scaler.legScaleFactor = this.CurInfo.LegScale;
			scaler.armScaleFactor = this.CurInfo.ArmScale;
		}
		ModelHatController mhc = this.selHat.GetComponent<ModelHatController>();
		bool flag3 = mhc != null;
		if (flag3)
		{
			mhc.verticalOffset = this.CurInfo.HatHeight;
		}
		bool flag4 = FacetrackingManager.Instance != null;
		if (flag4)
		{
			//FacetrackingManager.Instance.Shifting = this.CurInfo.Shifting;
			//FacetrackingManager.Instance.modelMeshScale = this.CurInfo.HeadScale;
		}
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00009660 File Offset: 0x00007860
	private void LoadDressingModel(string modelDir, int select)
	{
		string modelPath = this.modelCategory + "/" + modelDir + "/model";
		Object modelPrefab = Resources.Load(modelPath, typeof(GameObject));
		bool flag = modelPrefab == null;
		if (!flag)
		{
			bool flag2 = this.selModel != null;
			if (flag2)
			{
				Object.Destroy(this.selModel);
			}
			for (int i = 0; i < this.modelHat.Length; i++)
			{
				bool flag3 = i == select;
				if (flag3)
				{
					this.selHat = this.modelHat[select];
				}
				else
				{
					this.modelHat[i].SetActive(false);
				}
			}
			this.UserBodyBlender.ChangeBackGround(this.selected);
			this.selModel = (GameObject)Object.Instantiate(modelPrefab, Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
			this.selModel.name = "Model" + modelDir;
			ModelData data = this.selModel.GetComponent<ModelData>();
			bool flag4 = data == null;
			if (flag4)
			{
				throw new UnityException("没有找到模型数据");
			}
			this.CurInfo = GlobalSettings.GetRoleInfo(data.Name);
			bool flag5 = this.CurInfo == null;
			if (flag5)
			{
				throw new UnityException("没有找到角色数据，查找的角色是：" + data.Name);
			}
			this.editorModel.SetInfo(this.CurInfo);
			this.selHat.GetComponent<ModelHatController>().verticalOffset = this.CurInfo.HatHeight;
			bool flag6 = FacetrackingManager.Instance != null;
			if (flag6)
			{
				//FacetrackingManager.Instance.Shifting = this.CurInfo.Shifting;
				//FacetrackingManager.Instance.modelMeshScale = this.CurInfo.HeadScale;
			}
			AvatarController ac = this.selModel.GetComponent<AvatarController>();
			bool flag7 = ac == null;
			if (flag7)
			{
				ac = this.selModel.AddComponent<AvatarController>();
				ac.playerIndex = this.playerIndex;
				ac.mirroredMovement = true;
				ac.verticalMovement = true;
				ac.applyMuscleLimits = this.applyMuscleLimits;
				ac.verticalOffset = this.verticalOffset;
				ac.forwardOffset = this.forwardOffset;
				ac.smoothFactor = 0f;
				ac.verticalOffset = this.CurInfo.Vertacal;
				ac.forwardOffset = this.CurInfo.Farward;
				ac.fingerOrientations = true;
				ac.applyMuscleLimits = true;
			}
			ac.posRelativeToCamera = this.modelRelativeToCamera;
			ac.posRelOverlayColor = (this.foregroundCamera != null);
			KinectManager km = KinectManager.Instance;
			bool flag8 = km && km.IsInitialized();
			if (flag8)
			{
				long userId = km.GetUserIdByIndex(this.playerIndex);
				bool flag9 = userId != 0L;
				if (flag9)
				{
					ac.SuccessfulCalibration(userId, false);
				}
				MonoBehaviour[] monoScripts = Object.FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];
				km.avatarControllers.Clear();
				foreach (MonoBehaviour monoScript in monoScripts)
				{
					bool flag10 = monoScript is AvatarController && monoScript.enabled;
					if (flag10)
					{
						AvatarController avatar = (AvatarController)monoScript;
						km.avatarControllers.Add(avatar);
					}
				}
			}
			AvatarScaler scaler = this.selModel.GetComponent<AvatarScaler>();
			bool flag11 = scaler == null;
			if (flag11)
			{
				scaler = this.selModel.AddComponent<AvatarScaler>();
				scaler.playerIndex = this.playerIndex;
				scaler.mirroredAvatar = true;
				scaler.continuousScaling = this.continuousScaling;
				scaler.bodyScaleFactor = this.bodyScaleFactor;
				scaler.bodyWidthFactor = this.bodyWidthFactor;
				scaler.armScaleFactor = this.armScaleFactor;
				scaler.legScaleFactor = this.legScaleFactor;
				scaler.bodyScaleFactor = this.CurInfo.BodyScale;
				scaler.legScaleFactor = this.CurInfo.LegScale;
				scaler.armScaleFactor = this.CurInfo.ArmScale;
			}
			scaler.foregroundCamera = this.foregroundCamera;
		}
	}

	// Token: 0x0400012C RID: 300
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	// Token: 0x0400012D RID: 301
	[Tooltip("The model category. Used for model discovery and title of the category menu.")]
	public string modelCategory = "Clothing";

	// Token: 0x0400012E RID: 302
	[Tooltip("Total number of the available clothing models.")]
	public int numberOfModels = 3;

	// Token: 0x0400012F RID: 303
	[Tooltip("Reference to the dresing menu.")]
	public RectTransform dressingMenu;

	// Token: 0x04000130 RID: 304
	[Tooltip("Reference to the dresing menu-item prefab.")]
	public GameObject dressingItemPrefab;

	// Token: 0x04000131 RID: 305
	[Tooltip("Makes the initial model position relative to this camera, to be equal to the player's position, relative to the sensor.")]
	public Camera modelRelativeToCamera = null;

	// Token: 0x04000132 RID: 306
	[Tooltip("Camera used to estimate the overlay position of the model over the background.")]
	public Camera foregroundCamera;

	// Token: 0x04000133 RID: 307
	[Tooltip("Whether to keep the selected model, when the model category gets changed.")]
	public bool keepSelectedModel = true;

	// Token: 0x04000134 RID: 308
	[Tooltip("Whether the scale is updated continuously or just once, after the calibration pose.")]
	public bool continuousScaling = true;

	// Token: 0x04000135 RID: 309
	[Tooltip("Full body scale factor (incl. height, arms and legs) that might be used for fine tuning of body-scale.")]
	[Range(0f, 2f)]
	public float bodyScaleFactor = 1f;

	// Token: 0x04000136 RID: 310
	[Tooltip("Body width scale factor that might be used for fine tuning of the width scale. If set to 0, the body-scale factor will be used for the width, too.")]
	[Range(0f, 2f)]
	public float bodyWidthFactor = 1f;

	// Token: 0x04000137 RID: 311
	[Tooltip("Additional scale factor for arms that might be used for fine tuning of arm-scale.")]
	[Range(0f, 2f)]
	public float armScaleFactor = 1f;

	// Token: 0x04000138 RID: 312
	[Tooltip("Additional scale factor for legs that might be used for fine tuning of leg-scale.")]
	[Range(0f, 2f)]
	public float legScaleFactor = 1f;

	// Token: 0x04000139 RID: 313
	[Tooltip("Vertical offset of the avatar with respect to the position of user's spine-base.")]
	[Range(-0.5f, 0.5f)]
	public float verticalOffset = 0f;

	// Token: 0x0400013A RID: 314
	[Tooltip("Forward (Z) offset of the avatar with respect to the position of user's spine-base.")]
	[Range(-0.5f, 0.5f)]
	public float forwardOffset = 0f;

	// Token: 0x0400013B RID: 315
	[Tooltip("Whether to apply the humanoid model's muscle limits to the avatar, or not.")]
	private bool applyMuscleLimits = false;

	// Token: 0x0400013C RID: 316
	[Tooltip("Gender filter of this model selector.")]
	public UserGender modelGender = UserGender.Unisex;

	// Token: 0x0400013D RID: 317
	[Tooltip("Minimum age filter of this model selector.")]
	public float minimumAge = 0f;

	// Token: 0x0400013E RID: 318
	[Tooltip("Maximum age filter of this model selector.")]
	public float maximumAge = 1000f;

	// Token: 0x0400013F RID: 319
	[HideInInspector]
	public bool activeSelector = false;

	// Token: 0x04000140 RID: 320
	public UserBodyBlender UserBodyBlender;

	// Token: 0x04000141 RID: 321
	public EditorModel editorModel;

	// Token: 0x04000142 RID: 322
	private Text dressingMenuTitle;

	// Token: 0x04000143 RID: 323
	private RectTransform dressingMenuContent;

	// Token: 0x04000144 RID: 324
	private List<GameObject> dressingPanels = new List<GameObject>();

	// Token: 0x04000145 RID: 325
	private string[] modelNames;

	// Token: 0x04000146 RID: 326
	private Texture2D[] modelThumbs;

	// Token: 0x04000147 RID: 327
	private Sprite[] modelSprites;

	// Token: 0x04000148 RID: 328
	private GameObject[] modelHat;

	// Token: 0x04000149 RID: 329
	private Vector2 scroll;

	// Token: 0x0400014A RID: 330
	private int selected = -1;

	// Token: 0x0400014B RID: 331
	private int prevSelected = -1;

	// Token: 0x0400014C RID: 332
	public GameObject selModel;

	// Token: 0x0400014D RID: 333
	public GameObject selHat;

	// Token: 0x0400014E RID: 334
	public RoleInfo CurInfo;

	// Token: 0x0400014F RID: 335
	private float curScaleFactor = 0f;

	// Token: 0x04000150 RID: 336
	private float curModelOffset = 0f;

	// Token: 0x04000151 RID: 337
	public GameObject MyDressingUI;
}
