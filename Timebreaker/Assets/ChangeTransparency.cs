using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTransparency : MonoBehaviour {
    public bool _playerIsUnder;
    public float _transparency = 0.4f;
    private List<GameObject> _transparentable = new List<GameObject>();

	// Use this for initialization
	void Start () {
        _playerIsUnder = false;
        searchInChildren();
    }
	
	// Update is called once per frame
	void Update () {
		if(_playerIsUnder)
        {
            BecomeTransparent();
        }
        else
        {
            BecomeSolid();
        }
	}

    private void searchInChildren()
    {
        _transparentable.Clear();
        Transform parent = transform;
        for (int i = 0; i<parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.gameObject.layer == LayerMask.NameToLayer("Transparentable"))
            {
                _transparentable.Add(child.gameObject);
            }
        }
    }

    private void BecomeTransparent()
    {
        foreach (var item in _transparentable)
        {
            Renderer renderer = item.GetComponent<Renderer>();
            Color textureColor = renderer.material.color;
            if (textureColor.a > _transparency)
            {
                textureColor.a = textureColor.a - 0.025f;
            }           
            renderer.material.color = textureColor;
        }
    }

    private void BecomeSolid()
    {
        foreach (var item in _transparentable)
        {
            Renderer renderer = item.GetComponent<Renderer>();
            Color textureColor = renderer.material.color;
            if (textureColor.a < 1f)
            {
                textureColor.a = textureColor.a + 0.025f;
            }
            renderer.material.color = textureColor;
        }
    }
}
