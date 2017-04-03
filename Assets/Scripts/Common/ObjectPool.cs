/**
 * Copyright (c) 2014,Need Corp. ltd
 * All rights reserved.
 * 
 * 文件名称：ObjectPool.cs
 * 简    述：对象池服务可以减少从头创建每个对象的系统开销。而且可以避免多余的内存垃圾产生。
 * 在激活对象时，它从池中提取。在停用对象时，它放回池中，等待下一个请求。
 * 创建标识：Terry  2014/5/23
 */
using UnityEngine;
using System;
using System.Collections;
using Need.Mx;

public interface IDynamicObject
{
    /// <summary>
    /// 可以用来创建实际的对象，如:建立数据库连接，并打开这个连接
    /// </summary>
    void Create(System.Object param);
    /// <summary>
    /// 使用户可以返回这个实际 的对象，如一个SqlConnection对象
    /// </summary>
    System.Object GetInnerObject();
    /// <summary>
    /// 用来判断用户自定义对象的有效性的，是对象池决定是否重新创建对象的标志
    /// </summary>
    bool IsValidate();
    /// <summary>
    /// 用户可以进行资源释放工作。 
    /// </summary>
    void Release();
}

public sealed class ObjectPool
{
    private class PoolItem
    {
        private IDynamicObject _object;
        /// <summary>
        /// 代表是否正在被用户使用
        /// </summary>
        private bool _bUsing;
        private Type _type;

        /// <summary>
        /// 创建内部持有对象的必要基本参数。
        /// </summary>
        private System.Object _CreateParam;

        public PoolItem(Type type, System.Object param)
        {
            _type = type;
            _CreateParam = param;
            Create();
        }

        private void Create()
        {
            _bUsing = false;
            _object = (IDynamicObject)System.Activator.CreateInstance(_type);
            _object.Create(_CreateParam);
        }

        public void Recreate()
        {
            _object.Release();
            Create();
        }

        public void Release()
        {
            _object.Release();
        }

        public System.Object InnerObject { get { return _object.GetInnerObject(); } }

        public int InnerObjectHashcode { get { return InnerObject.GetHashCode(); } }

        public bool IsValidate { get { return _object.IsValidate(); } }

        public bool Using
        {
            get { return _bUsing; }
            set { _bUsing = value; }
        }
    }// class PoolItem
    private int _nCapacity;
    private int _nCurrentSize;
    private Hashtable _listObjects;
    private ArrayList _listFreeIndex;
    private ArrayList _listUsingIndex;
    private Type _typeObject;
    private System.Object _objCreateParam;

    /// <summary>
    /// 构造指定类型的对象池，主要给具体对象池使用。
    /// </summary>
    /// <param name="type">对象的类型typeof</param>
    /// <param name="create_param">创建对象的参数</param>
    /// <param name="init_size">预创建的对象个数</param>
    /// <param name="capacity">预创建的对象池大小</param>
    public ObjectPool(Type type, System.Object create_param, int init_size, int capacity)
    {
        if (init_size < 0 || capacity < 1 || init_size > capacity)
        {
            throw (new Exception("Invalid parameter!"));
        }

        _nCapacity = capacity;
        _listObjects = new Hashtable(capacity);
        _listFreeIndex = new ArrayList(capacity);
        _listUsingIndex = new ArrayList(capacity);
        _typeObject = type;
        _objCreateParam = create_param;

        for (int i = 0; i < init_size; i++)
        {
            PoolItem pitem = new PoolItem(type, create_param);
            _listObjects.Add(pitem.InnerObjectHashcode, pitem);
            _listFreeIndex.Add(pitem.InnerObjectHashcode);
        }

        _nCurrentSize = _listObjects.Count;
    }

    /// <summary>
    /// foreach的会产生一些内存垃圾，Release()还是少调用比较好。
    /// </summary>
    public void Release()
    {
        lock (this)
        {
            foreach (DictionaryEntry de in _listObjects)
            {
                ((PoolItem)de.Value).Release();
            }
            _listObjects.Clear();
            _listFreeIndex.Clear();
            _listUsingIndex.Clear();
        }
    }

    /// <summary>
    /// 当前对象池中所拥有的对象个数。 Terry
    /// </summary>
    public int CurrentSize
    {
        get { return _nCurrentSize; }
    }

    public int ActiveCount
    {
        get { return _listUsingIndex.Count; }
    }

    public System.Object GetOne()
    {
        lock (this)
        {
            if (_listFreeIndex.Count == 0)
            {
                if (_nCurrentSize == _nCapacity)
                {
                    UnityEngine.Debug.LogError("ObjectPool has no more capacity to create new object");
                    return null;
                }
                PoolItem pnewitem = new PoolItem(_typeObject, _objCreateParam);
                Debug.Log("Create New PoolItem!! ID:" + pnewitem.InnerObjectHashcode);
                _listObjects.Add(pnewitem.InnerObjectHashcode, pnewitem);
                _listFreeIndex.Add(pnewitem.InnerObjectHashcode);
                _nCurrentSize++;
            }
            //Debug.Log("UsingIndex Add Object :" + _listFreeIndex[0]);
            int nFreeIndex = (int)_listFreeIndex[0];
            PoolItem pitem = (PoolItem)_listObjects[nFreeIndex];
            _listFreeIndex.RemoveAt(0);
            _listUsingIndex.Add(nFreeIndex);
            //Debug.Log("UsingIndex Add Object :" + nFreeIndex);
            if (!pitem.IsValidate)
            {//这里需要注意，Recreate()很可能会改变InnerObject的HashCode，
                //所以考虑在这里,使用
                pitem.Recreate();

                if (nFreeIndex != pitem.InnerObjectHashcode)
                {
                    _listObjects.Remove(nFreeIndex);
                    _listUsingIndex.Remove(nFreeIndex);
                    nFreeIndex = pitem.InnerObjectHashcode;
                    _listObjects.Add(nFreeIndex, pitem);
                    _listUsingIndex.Add(nFreeIndex);
                }
            }
            pitem.Using = true;
            return pitem.InnerObject;
        }
    }

    public void FreeObject(System.Object obj)
    {
        lock (this)
        {
            int key = obj.GetHashCode();
            if (_listObjects.ContainsKey(key))
            {
                PoolItem item = (PoolItem)_listObjects[key];
                item.Using = false;
                _listUsingIndex.Remove(key);
                _listFreeIndex.Add(key);
            }
            else
            {
                //throw new InvalidOperationException("试图归还一个非法的资源!ID:" + obj.GetHashCode() + obj);
                Debug.LogWarning("试图归还一个非法的资源!ID:" + key + obj + obj.GetHashCode());
            }
        }
    }

    public int DecreaseSize(int size)
    {
        int nDecrease = size;
        lock (this)
        {
            if (nDecrease <= 0)
            {
                return 0;
            }
            if (nDecrease > _listFreeIndex.Count)
            {
                nDecrease = _listFreeIndex.Count;
            }

            for (int i = 0; i < nDecrease; i++)
            {
                _listObjects.Remove(_listFreeIndex[i]);
            }

            _listFreeIndex.Clear();
            _listUsingIndex.Clear();

            foreach (DictionaryEntry de in _listObjects)
            {
                PoolItem pitem = (PoolItem)de.Value;
                if (pitem.Using)
                {
                    _listUsingIndex.Add(pitem.InnerObjectHashcode);
                }
                else
                {
                    _listFreeIndex.Add(pitem.InnerObjectHashcode);
                }
            }
        }
        _nCurrentSize -= nDecrease;
        return nDecrease;
    }
}
