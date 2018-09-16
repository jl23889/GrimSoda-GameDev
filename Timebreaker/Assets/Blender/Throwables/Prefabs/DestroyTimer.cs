using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

    public float timeToDestroy = 3;

    // Use this for initialization
    void Start () {
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSecondsRealtime(timeToDestroy);
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update () {
		
	}
}
