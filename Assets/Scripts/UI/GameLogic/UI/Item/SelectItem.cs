/*
 * Copyright (c) 
 * 
 * 文件名称：   SelectItem.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/17 10:44:53
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;

public class SelectItem : MonoBehaviour
{
    private SelectLogic _parent;
    private List<Image> _image;
    private float _v = 0;
    private Vector3 _p, _s;
    private float _sv;
    private List<Color> _color;

    public EnvirType type;
    public ParticleSystem particle0;
    public GameObject particle1;
    public Image green;
    public Image yellow;
    public GameObject selected;
    public Image self;

    public float V
    {
        get
        {
            return _v;
        }
        set
        {
            _v = value;
        }
    }

    public void Init(SelectLogic selectLogic)
    {
        _image = new List<Image>();
        _color = new List<Color>();
        if (transform.GetComponent<Image>() != null)
        {
            _image.Add(transform.GetComponent<Image>());
            _color.Add(_image[0].color);
        }
        for (int i = 0; i < transform.childCount; i++ )
        {
            Image img = transform.GetChild(i).GetComponent<Image>();
            if (null != img)
            {
                _image.Add(img);
                _color.Add(img.color);
            }
        }
        _parent = selectLogic;
    }

    public void Drag(float value)
    {
        _v += value;
        _p = transform.localPosition;
        _p.x = _parent.GetPosition(_v);
        transform.localPosition = _p;

        for (int i = 0; i < _color.Count; i++ )
        {
            Color c = _color[i];
            c.a = _parent.GetApa(_v);
            _color[i] = c;
            _image[i].color = _color[i];

        }
        _sv  = _parent.GetScale(_v);
        _s.x = _sv;
        _s.y = _sv;
        _s.z = 1;
        transform.localScale = _s;
    }
}
