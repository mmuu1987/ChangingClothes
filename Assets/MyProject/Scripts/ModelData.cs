using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class ModelData : MonoBehaviour
{
	// Token: 0x0600064D RID: 1613 RVA: 0x00045F02 File Offset: 0x00044102
	public IEnumerator Start()
	{
		bool flag = this.cloths.Count > 0;
		if (flag)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(0.2f);
			//bool flag2 = this.cloths.Count > 0;
			//if (flag2)
			//{
			//	foreach (Cloth cloth in this.cloths)
			//	{
			//		bool flag3 = cloth != null;
			//		if (flag3)
			//		{
			//			cloth.enabled = true;
			//		}
			//		cloth = null;
			//	}
			//	List<Cloth>.Enumerator enumerator = default(List<Cloth>.Enumerator);
			//}
		}
		yield return null;
		yield break;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00045F14 File Offset: 0x00044114
	public void SetHead(GameObject head,float targetPos,float time)
	{
		head.transform.parent = this.headTransform;
		head.transform.localPosition = this.HeadPos;
		head.transform.localRotation = Quaternion.Euler(new Vector3(this.HeadRotation.x, this.HeadRotation.y, this.HeadRotation.z));
		head.transform.localScale = this.HeadScale;
		base.transform.DOMoveZ(targetPos, time, false).SetEase(Ease.Linear);
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00002231 File Offset: 0x00000431
	public void SaveData()
	{
	}

	// Token: 0x0400069B RID: 1691
	public string Name;

	// Token: 0x0400069C RID: 1692
	public List<Cloth> cloths = new List<Cloth>();

	// Token: 0x0400069D RID: 1693
	public Transform headTransform;

	// Token: 0x0400069E RID: 1694
	public Vector3 HeadPos;

	// Token: 0x0400069F RID: 1695
	public Vector3 HeadRotation;

	// Token: 0x040006A0 RID: 1696
	public Vector3 HeadScale;
}
