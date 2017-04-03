/*
 * Copyright (c) 
 * 
 * 文件名称：   EffectBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/19 16:36:58
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectBehaviour : MonoBehaviour
{

    public class EffectSpawnParameters
    {
        Transform spawnTransform;
        Vector3 spawnOffset;
        Vector3 spawnPoint;
        bool shouldParentToTransform;

        public Transform SpawnTransform { get { return spawnTransform; } }
        public Vector3 SpawnOffset      { get { return spawnOffset; } }
        public bool ParentToTransform   { get { return shouldParentToTransform; } }
        public Vector3 SpawnPoint       { get { return spawnPoint; } set { spawnPoint = value; } }

        public EffectSpawnParameters (Transform _spawnTransform, Vector3 _spawnOffset, bool _parentToTransform)
        {
            spawnTransform          = _spawnTransform;
            spawnOffset             = _spawnOffset;
            shouldParentToTransform = _parentToTransform;
        }
    }

    public SamplePoolManager.ObjectType effectType;
    public List<ParticleSystem> particleSysList;
    private float maxDuration;
    private bool isLoop;

    void Awake()
    {
        particleSysList = new List<ParticleSystem>();
        for (int i = 0; i < transform.childCount; ++i )
        {
            ParticleSystem sys = transform.GetChild(i).GetComponent<ParticleSystem>();
            if (null != sys)
            {
                particleSysList.Add(sys);
                sys.Stop(true);
            }
        }
    }


    public void Spawn(EffectSpawnParameters param)
    {
        transform.position = param.SpawnTransform.position + param.SpawnOffset;
        transform.rotation = param.SpawnTransform.rotation;

        if (param.ParentToTransform)
        {
            transform.SetParent(param.SpawnTransform);
        }

        if (!transform.gameObject.activeInHierarchy)
        {
            return;
        }

        if (null != particleSysList)
        {
            for (int i = 0; i < particleSysList.Count; ++i )
            {
                if (particleSysList[i].loop)
                {
                    isLoop = true;
                }
                particleSysList[i].Clear(true);
                particleSysList[i].Play(true);
                if (particleSysList[i].duration > maxDuration)
                {
                    maxDuration = particleSysList[i].duration;
                }
            }
            StartCoroutine(Release());
        }
        else
        {
            StartCoroutine(ReleaseNull());
        }
    }

    public void SpawnInPoint(Vector3 spawnPoint)
    {
        transform.position = spawnPoint;
        if (null != particleSysList)
        {
            for (int i = 0; i < particleSysList.Count; ++i)
            {
                if (particleSysList[i].loop)
                {
                    isLoop = true;
                }
                particleSysList[i].Clear(true);
                particleSysList[i].Play(true);
                if (particleSysList[i].duration > maxDuration)
                {
                    maxDuration = particleSysList[i].duration;
                }
            }
            StartCoroutine(Release());
        }
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(maxDuration);

        StartCoroutine(ReleaseParticles());
    }

    IEnumerator ReleaseParticles()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isLoop)
        {
            for (int i = 0; i < particleSysList.Count; ++i)
            {
                particleSysList[i].Stop(true);
            }
            SamplePoolManager.Instance.EffectFinshed(gameObject);
        }
    }

    IEnumerator ReleaseNull()
    {
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(ReleaseParticles());
    }


    // Update is called once per frame
    void Update()
    {
        if (GameMode.Instance.Mode == Need.Mx.RunMode.GameOver)
        {
            SamplePoolManager.Instance.EffectFinshed(gameObject);
        }
    }

}
