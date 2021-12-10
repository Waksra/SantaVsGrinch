using System;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    [SerializeField] private Animator[] animators;
    [SerializeField] private float hurtDecay = 100f;
    private float hurt;
    
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    private void Update()
    {
        if (hurt <= 0f)
            SetAnimatorFloat("Hit", 0f);
        else
        {
            hurt -= Time.deltaTime * hurtDecay;
            SetAnimatorFloat("Hit", hurt);
        }
    }

    public void SetAnimatorTrigger(string str)
    {
        foreach (var animator in animators)
        {
            animator.SetTrigger(str);
        }
    }

    public void SetAnimatorBool(string str, bool value)
    {
        foreach (var animator in animators)
        {
            animator.SetBool(str, value);
        }
    }
    
    public void SetAnimatorFloat(string str, float value)
    {
        foreach (var animator in animators)
        {
            animator.SetFloat(str, value);
        }
    }
    
    public void SetAnimatorInt(string str, int value)
    {
        foreach (var animator in animators)
        {
            animator.SetInteger(str, value);
        }
    }

    public void SetHitFloat(float value)
    {
        hurt = value;
    }
    
    public void SetDeadFloat(float value)
    {
        foreach (var animator in animators)
        {
            animator.SetFloat("Dead", value);
        }
    }
}
