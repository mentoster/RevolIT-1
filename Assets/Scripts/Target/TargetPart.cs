using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPart : MonoBehaviour, ITarget

{
    [SerializeField] Target _target;
    [SerializeField] float _bonus=1;

    public Tuple<bool, float> TakeDamage(float damage)
    {
        return _target.TakeDamage(damage * _bonus);
    }

}
