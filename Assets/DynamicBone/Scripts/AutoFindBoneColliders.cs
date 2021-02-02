using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFindBoneColliders : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DynamicBone[] db = this.GetComponentsInChildren<DynamicBone>();

        DynamicBoneCollider [] dbc = this.GetComponentsInChildren<DynamicBoneCollider>();

        foreach (DynamicBone dynamicBone in db)
        {
            dynamicBone.m_Colliders = new List<DynamicBoneColliderBase>(dbc);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
