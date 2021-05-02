
using System;
using System.Collections;
using UnityEngine;
[Serializable]
public class Reload : MonoBehaviour, Icolt
{
    [SerializeField]
    AudioClip _reloadSound;
    AudioSource _audioSource;
    byte _maxBullets = 6;
    [HideInInspector] public bool nowReload = true;
    [SerializeField] float _reloadTime = 3;
    public void Set(byte maxBullets, AudioSource audioSource)
    {
        _audioSource = audioSource;
        _maxBullets = maxBullets;
    }
    public byte Use(byte bullets)
    {
        if (!nowReload)
        {
            // start ReloadAnimation
            //
            _audioSource.PlayOneShot(_reloadSound);
            nowReload = true;
            StartCoroutine(ReloadTimer());
            return 6;
        }
        else return bullets;
    }
    IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(_reloadTime);
        nowReload = false;
    }
}
