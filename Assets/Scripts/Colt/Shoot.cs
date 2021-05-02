using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Shoot : MonoBehaviour, Icolt
{
    [SerializeField] Transform _shootPoint;
    public byte Use(byte bullets)
    {

        return --bullets;
    }
}
