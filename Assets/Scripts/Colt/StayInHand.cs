
using System.Collections;
using Assets.Scripts.Colt;
using UnityEngine;

public class StayInHand : MonoBehaviour
{

    Colt _colt;
    Transform _parent;
    private void Start()
    {
        _colt = gameObject.GetComponent<Colt>();
    }
    public void TakeInHand()
    {
        _colt.enabled = true;
        // get hand transform
        _parent = transform.parent;
        _parent.GetComponent<MeshRenderer>().enabled = false;
    }
    public void DetachedFromHand()
    {
        transform.parent = _parent;
    }
}
