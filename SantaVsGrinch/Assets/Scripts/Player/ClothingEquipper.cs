using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingEquipper : MonoBehaviour
{
    private Stitcher stitcher;

    [SerializeField] private GameObject playerAvatar = default;
    [SerializeField] private GameObject clothing = default;

    private void Start()
    {
        stitcher = new Stitcher();
        
        GameObject go = Instantiate(clothing);
        stitcher.Stitch(go, playerAvatar);
    }
}
