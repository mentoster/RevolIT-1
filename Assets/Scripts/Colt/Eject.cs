using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eject : MonoBehaviour
{
    [SerializeField] float _pushPower = 0.01f;
    private void OnTriggerEnter(Collider other)
    {
        print($"Eject {other.gameObject}");
        if (other.gameObject.CompareTag("Bullet"))
        {
            print($"Eject {other.gameObject}");
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            // delete all another powers
            rb.isKinematic = true;
            rb.isKinematic = false;
            rb.AddForce(new Vector3(-_pushPower * Random.Range(0.7f, 1.5f), _pushPower * Random.Range(1f, 1.5f), 0), ForceMode.Force);
            rb.useGravity = true;
            other.gameObject.transform.parent = null;
            other.gameObject.GetComponent<CapsuleCollider>().enabled=true;
        }
    }

}
