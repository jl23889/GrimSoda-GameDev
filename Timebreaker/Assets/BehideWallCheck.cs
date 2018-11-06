using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehideWallCheck : MonoBehaviour {
    private LayerMask _layerMask;

	// Use this for initialization
	void Start () {
        _layerMask = LayerMask.GetMask("Default");
    }
	
	// Update is called once per frame
	void Update () {

        WallChecker();

    }

    private void WallChecker()
    {
        RaycastHit[] hits;

        hits = Physics.RaycastAll(transform.position, Camera.main.transform.forward * -1, 200f, _layerMask);
        if (hits.Length != 0)
        {
            Debug.DrawRay(transform.position, Camera.main.transform.forward * -1 * hits[0].distance, Color.yellow);
            for (int i =0; i<hits.Length ; i++)
            {
                if (hits[i].collider.gameObject.tag == "Transparentable")
                {
                    ChangeTransparency _wall = hits[i].collider.gameObject.GetComponentInParent<ChangeTransparency>();
                    _wall._playerIsUnder = true;
                }
            }
        }
        else
        {
            Debug.DrawRay(transform.position, Camera.main.transform.forward * -1 * 1000f, Color.white);
        }
    }


}
