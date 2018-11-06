using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehideWallCheck : MonoBehaviour {
    private LayerMask _layerMask;
    private List<ChangeTransparency> _changed = new List<ChangeTransparency>();

	// Use this for initialization
	void Start () {
        _layerMask = LayerMask.GetMask("Default");
        _changed.Clear();
    }
	
	// Update is called once per frame
	void Update () {
        foreach (var item in _changed)
        {
            item._playerIsUnder = false;
        }
        _changed.Clear();
        WallChecker();
    }

    private void WallChecker()
    {
        RaycastHit[] hits;

        hits = Physics.RaycastAll(transform.position+Vector3.up*2f, Camera.main.transform.forward * -1, 200f, _layerMask);
        if (hits.Length != 0)
        {
            Debug.DrawRay(transform.position + Vector3.up * 2f, Camera.main.transform.forward * -1 * hits[0].distance, Color.yellow);
            for (int i =0; i<hits.Length ; i++)
            {
                if (hits[i].collider.gameObject.tag == "Transparentable")
                {
                    ChangeTransparency _wall = hits[i].collider.gameObject.GetComponentInParent<ChangeTransparency>();
                    _changed.Add(_wall);
                    _wall._playerIsUnder = true;
                }
            }
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 2f, Camera.main.transform.forward * -1 * 1000f, Color.white);
        }
    }


}
