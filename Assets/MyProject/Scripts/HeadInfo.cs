using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000098 RID: 152
[Serializable]
public class HeadInfo
{
	// Token: 0x06000611 RID: 1553 RVA: 0x0004445C File Offset: 0x0004265C
	public Vector3[] GetVertices()
	{
		List<Vector3> temps = new List<Vector3>();
		foreach (Vertices vertices in this.Vertices)
		{
			Vector3 temp = Vector3.zero;
			temp.x = vertices.vertices_x;
			temp.y = vertices.vertices_y;
			temp.z = vertices.vertices_z;
			temps.Add(temp);
		}
		return temps.ToArray();
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x000444F4 File Offset: 0x000426F4
	public Vector2[] GetUVs()
	{
		List<Vector2> temps = new List<Vector2>();
		foreach (UVs uv in this.Uvs)
		{
			Vector3 temp = Vector3.zero;
			temp.x = uv.UV_x;
			temp.y = uv.UV_y;
			temps.Add(temp);
		}
		return temps.ToArray();
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00044584 File Offset: 0x00042784
	public int[] GetTriangles()
	{
		return this.Triangles.ToArray();
	}

	// Token: 0x0400064E RID: 1614
	public List<Vertices> Vertices;

	// Token: 0x0400064F RID: 1615
	public List<UVs> Uvs;

	// Token: 0x04000650 RID: 1616
	public List<int> Triangles;

	// Token: 0x04000651 RID: 1617
	public byte[] TexBytes;
}
[Serializable]
public class UVs
{
	// Token: 0x0400064C RID: 1612
	public float UV_x;

	// Token: 0x0400064D RID: 1613
	public float UV_y;
}
[Serializable]
public class Vertices
{
	// Token: 0x04000649 RID: 1609
	public float vertices_x;

	// Token: 0x0400064A RID: 1610
	public float vertices_y;

	// Token: 0x0400064B RID: 1611
	public float vertices_z;
}

