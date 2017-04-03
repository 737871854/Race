/*
 * Copyright (c) 
 * 
 * 文件名称：   Character.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/12/16 14:22:48
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class Character : MonoBehaviour
{
    public enum State
    {
        none = -1,
        normal,
        acc,
        left,
        right,
        over,
        show,
    }

    public State state = State.none;
    public Animator animator;
    private Material material;
    public bool isHide;
    public SkinnedMeshRenderer _smr;
    private bool isTransparent;

    void Awake()
    {
        MessageCenter.Instance.AddListener(MessageType.Message_Speed_Accle, OnACC);
    }

    void Start()
    {
        material = GameObject.Instantiate(_smr.material);
        _smr.material = material;
        EndTransparent();
    }

    void Destroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Speed_Accle, OnACC);
    }

    public void Transparent()
    {
        isTransparent = true;
        material.shader   = Shader.Find("FenXiang/liangbian");
        material.SetColor("_InnerColor", new Color(0.1f, 0.64f, 0.21f, 0.5f));
        material.SetColor("_RimColor", new Color(0.15f, 0.09f, 0.502f, 0.5f));
    }

    public void EndTransparent()
    {
        isTransparent = false;
        material.shader = Shader.Find("Q5/Unity/Unlit/Texture Alpha Control");
    }

    private void OnACC(Message message)
    {
        if ((bool)message["active"])
        {
            state = State.acc;
        }
        else
        {
            state = State.normal;
        }
    }

    private float alpha;
    void Update()
    {
        switch(state)
        {
            case State.none:
                state = State.normal;
                break;
            case State.normal:
                ChangeToNormal();
                break;
            case State.acc:
                ChangeToAcc();
                break;
            case State.left:
                ChangeToLeft();
                break;
            case State.right:
                ChangeToRight();
                break;
            case State.over:
                ChangeToOver();
                break;
            case State.show:
                ChangeToShow();
                break;
        }
        
        if (!isTransparent)
        {
            alpha = material.GetFloat("_Alpha");
            if (isHide)
            {
                if (alpha > 0)
                {
                    material.SetFloat("_Alpha", alpha - Time.deltaTime);
                }
                else
                {
                    animator.gameObject.SetActive(false);
                }
            }
            else
            {
                animator.gameObject.SetActive(true);
                if (alpha < 1)
                {
                    material.SetFloat("_Alpha", alpha + Time.deltaTime);
                }
            }
        }
    }


    void ChangeToNormal()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("normal"))
        {
            animator.Play("normal");
        }
    }

    void ChangeToAcc()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("acc"))
        {
            animator.Play("acc");
        }
    }

    void ChangeToLeft()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("left"))
        {
            animator.Play("left");
        }
    }

    void ChangeToRight()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("right"))
        {
            animator.Play("right");
        }
    }

    void ChangeToOver()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("over"))
        {
            animator.Play("over");
        }
    }

    void ChangeToShow()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("show"))
        {
            animator.Play("show");
        }
    }
}
