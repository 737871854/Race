/*
 * Copyright (c) 
 * 
 * 文件名称：   SpawnableObjectClass.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/24 11:44:09
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnableObjectClass
{
    #region Public Members
    public float _startMinSpawnDistance;
    public float _startMaxSpawnDistance;
    public float _endMinSpawnDistance;
    public float _endMaxSpawnDistance;
    public int _priority;
    public SamplePoolManager.ObjectType _type;
    public bool _alwaysSpawn = false;
    public bool _avoidSpecialEvents = false;
    #endregion

    #region Private Members
    private float _newObjectDistance = 0.0f;
    #endregion

    public float NextSpawnObjectDistance { get { return _newObjectDistance; } set { _newObjectDistance = value; } }

    public SpawnableObjectClass()
    {

    }

    public SpawnableObjectClass(SamplePoolManager.ObjectType type, int priority, float startMinSpawnDistance, float startMaxSpawnDistance, float endMinSpawnDistance, float endMaxSpawnDistance)
    {
        _type                  = type;
        _newObjectDistance     = -1;
        _priority              = priority;
        _startMaxSpawnDistance = startMaxSpawnDistance;
        _startMinSpawnDistance = startMinSpawnDistance;
        _endMaxSpawnDistance   = endMaxSpawnDistance;
        _endMinSpawnDistance   = endMinSpawnDistance;
    }

    public void Reset()
    {
        _newObjectDistance = -1;
    }
}
