using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Token: 0x0200003E RID: 62
public class PhotoShooter : MonoBehaviour
{
	// Token: 0x06000124 RID: 292 RVA: 0x0000DB60 File Offset: 0x0000BD60
	public void CountdownAndMakePhoto()
	{
		base.StartCoroutine(this.CoCountdownAndMakePhoto());
	}

	// Token: 0x06000125 RID: 293 RVA: 0x0000DB70 File Offset: 0x0000BD70
	private IEnumerator CoCountdownAndMakePhoto()
	{
		bool flag = this.countdown != null && this.countdown.Length != 0;
		if (flag)
		{
			int num;
			for (int i = 0; i < this.countdown.Length; i = num + 1)
			{
				bool flag2 = this.countdown[i];
				if (flag2)
				{
					this.countdown[i].gameObject.SetActive(true);
				}
				yield return new WaitForSeconds(1f);
				bool flag3 = this.countdown[i];
				if (flag3)
				{
					this.countdown[i].gameObject.SetActive(false);
				}
				num = i;
			}
		}
		this.MakePhoto();
		yield return null;
		yield break;
	}

	// Token: 0x06000126 RID: 294 RVA: 0x0000DB7F File Offset: 0x0000BD7F
	public void MakePhoto()
	{
		this.MakePhoto(true);
	}

	// Token: 0x06000127 RID: 295 RVA: 0x0000DB8C File Offset: 0x0000BD8C
	public string MakePhoto(bool openIt)
	{
		int resWidth = Screen.width;
		int resHeight = Screen.height;
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		bool flag = this.infoText;
		if (flag)
		{
			this.infoText.text = string.Empty;
		}
		bool flag2 = this.backroundCamera && this.backroundCamera.enabled;
		if (flag2)
		{
			this.backroundCamera.targetTexture = rt;
			this.backroundCamera.Render();
			this.backroundCamera.targetTexture = null;
		}
		bool flag3 = this.backroundCamera2 && this.backroundCamera2.enabled;
		if (flag3)
		{
			this.backroundCamera2.targetTexture = rt;
			this.backroundCamera2.Render();
			this.backroundCamera2.targetTexture = null;
		}
		bool flag4 = this.foreroundCamera && this.foreroundCamera.enabled;
		if (flag4)
		{
			this.foreroundCamera.targetTexture = rt;
			this.foreroundCamera.Render();
			this.foreroundCamera.targetTexture = null;
		}
		RenderTexture prevActiveTex = RenderTexture.active;
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0f, 0f, (float)resWidth, (float)resHeight), 0, 0);
		screenShot.Apply();
		RenderTexture.active = prevActiveTex;
		Object.Destroy(rt);
		base.StartCoroutine(NetManager.Instance.PostPictureToServer(screenShot));
		ChangeingClothesManager.Instance.swapFaceManager.ShowShooterImage(screenShot);
		if (openIt)
		{
		}
		return null;
	}

	// Token: 0x040001E2 RID: 482
	[Tooltip("Camera used to render the background.")]
	public Camera backroundCamera;

	// Token: 0x040001E3 RID: 483
	[Tooltip("Camera used to render the background layer-2.")]
	public Camera backroundCamera2;

	// Token: 0x040001E4 RID: 484
	[Tooltip("Camera used to overlay the 3D-objects over the background.")]
	public Camera foreroundCamera;

	// Token: 0x040001E5 RID: 485
	[Tooltip("Array of sprite transforms that will be used for displaying the countdown until image shot.")]
	public Transform[] countdown;

	// Token: 0x040001E6 RID: 486
	[Tooltip("UI-Text used to display information messages.")]
	public Text infoText;
}
