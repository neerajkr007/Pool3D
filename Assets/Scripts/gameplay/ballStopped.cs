using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballStopped : MonoBehaviour
{
    public bool stopped = false;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.name.Contains("hole"))
        {
            stopped = true;
            gameObject.SetActive(false);
        }
    }
}
