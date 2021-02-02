using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{

    public Cloth Cloth;
    // Start is called before the first frame update
    void Start()
    {
       // Cloth.damping = 0.2F;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlagFun()
    {
        Cloth.damping = 0.2F;
    }
}
