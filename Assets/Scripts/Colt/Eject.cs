using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eject : MonoBehaviour
{
    [SerializeField] float _pushPower = 0.01f;
    [SerializeField] float _deleteTimer = 3;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            print($"Eject {other.gameObject}");
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            other.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            rb.AddForce(new Vector3(-_pushPower, _pushPower / 2, 0), ForceMode.Force);
            rb.useGravity = true;
            other.gameObject.transform.parent = null;
            other.gameObject.GetComponent<Bullet>().DeleteTimer(_deleteTimer);
        }
    }

}
