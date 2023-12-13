using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Narratives : ScriptableObject
{
    [SerializeField]
    public List<string> text;

    public string focus;
}
