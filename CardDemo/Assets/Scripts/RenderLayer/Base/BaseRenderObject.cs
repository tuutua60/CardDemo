using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRenderObject : MonoBehaviour
{
    public virtual void OnDestroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
