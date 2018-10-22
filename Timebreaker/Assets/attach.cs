using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attach : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.parent = transform;
    }

    void OnTriggerExit(Collider other)
    {
        other.gameObject.transform.parent = null;
    }
}
