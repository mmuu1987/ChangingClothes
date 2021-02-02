using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// Token: 0x020000A2 RID: 162
public class UIAnimation : MonoBehaviour
{
	// Token: 0x0600068D RID: 1677 RVA: 0x000474A0 File Offset: 0x000456A0
	private void Awake()
	{
		this.images = base.GetComponentsInChildren<Image>();
		this.originalVector3 = new Vector3[this.images.Length];
		for (int i = 0; i < this.images.Length; i++)
		{
			this.originalVector3[i] = this.images[i].rectTransform.anchoredPosition;
		}
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x00002231 File Offset: 0x00000431
	private void Start()
	{
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0004750C File Offset: 0x0004570C  
	private void OnEnable()
	{
		foreach (Image image in this.images)
		{
			float rangePos = (float)Random.Range(2500, 3500);
			float rangeTime = Random.Range(30f, 65f);
			image.rectTransform.DOAnchorPosY(rangePos, rangeTime, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
			float ran = Random.Range(0f, 1f);
			int i = 1;
			bool flag = ran <= 0.5f;
			if (flag)
			{
				i = -1;
			}
			image.rectTransform.DOLocalRotate(new Vector3(0f, 0f, 30f * (float)i), 10f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
		}
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x000475D8 File Offset: 0x000457D8
	private void OnDisable()
	{
		for (int i = 0; i < this.images.Length; i++)
		{
			this.images[i].rectTransform.anchoredPosition = this.originalVector3[i];
			this.images[i].rectTransform.DOKill(false);
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00002231 File Offset: 0x00000431
	private void Update()
	{
	}

	// Token: 0x040006D9 RID: 1753
	private Vector3[] originalVector3;

	// Token: 0x040006DA RID: 1754
	private Image[] images;
}
