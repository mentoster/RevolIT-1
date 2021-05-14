using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPart : MonoBehaviour, ITarget

{
    [SerializeField] Target _target;

    public void TakeDamage(float damage) => _target.TakeDamage(damage);

}
