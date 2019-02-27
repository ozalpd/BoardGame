using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPiece : MonoBehaviour
{
    [SerializeField]
    private Material selectMaterial;

    [SerializeField]
    private Material cloneableMat;

    [SerializeField]
    private Material jumpableMat;

    public Material SelectMaterial { get { return selectMaterial; } }
    public Material CloneableMaterial { get { return cloneableMat; } }
    public Material JumpableMaterial { get { return jumpableMat; } }
}
