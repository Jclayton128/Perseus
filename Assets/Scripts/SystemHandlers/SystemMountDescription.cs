using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMountDescription : MonoBehaviour
{
    public bool ShouldBeReflected = true;
    public Vector3 MirroredXPosition
    {
        get => new Vector3(-transform.localPosition.x*2, 0, transform.localPosition.z);
    }


}
