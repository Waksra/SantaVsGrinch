using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    private Animator animator;
    private MainMenuManager mainMenuManager;
    
    private bool isSelected;
    public bool IsSelected => isSelected;

    [SerializeField] private AudioClip soundEffect = default;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        mainMenuManager = FindObjectOfType<MainMenuManager>();
    }

    private void OnMouseOver()
    {
        if (isSelected) return;
        
        Activate();
        mainMenuManager.ChangeButtonIndexMouse(this);
    }

    public void Activate()
    {
        isSelected = true;
        animator.SetBool("Selected", true);
        // SoundManager.PlaySFX(soundEffect, Vector3.zero);
    }

    public void Deactivate()
    {
        isSelected = false;
        animator.SetBool("Selected", false);
    }
}
