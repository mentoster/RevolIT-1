using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

interface ITarget
{
    Tuple<bool, float> TakeDamage(float damage);
}
public class Target : MonoBehaviour, ITarget
{
    [SerializeField] VRRig _VRRig;
    [SerializeField] GameObject _dieText;
    [SerializeField] Byte _dieTextTimer = 1;
    [SerializeField] Volume _volume;
    float health = 100;
    private void Start()
    {
        _volume = GameObject.Find("Global Volume").GetComponent<Volume>();
    }
    public Tuple<bool, float> TakeDamage(float damage)
    {
        print($"14. {gameObject} [{health}] -> take damage : {damage} -> now have {health - damage}");
        health -= damage;
        if (health < 0)
        {
            Die();
            return Tuple.Create(true, 0f);
        }
        return Tuple.Create(false, damage);
    }
    public void Die()
    {
        _dieText.SetActive(true);
        StartCoroutine(HideDieText());
        
        DOTween.To(() => _volume.weight, x => _volume.weight = x, 0, 1);

        _VRRig.head.Die();
        _VRRig.leftHand.Die();
        _VRRig.rightHand.Die();
    }
    IEnumerator HideDieText()
    {
        yield return new WaitForSeconds(_dieTextTimer);
        _dieText.SetActive(false);
    }
    public void Live()
    {
        DOTween.To(() => _volume.weight, x => _volume.weight = x, 1, 1);
    }
}
