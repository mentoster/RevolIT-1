using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colt : MonoBehaviour
{
    [SerializeField] byte _bullets = 6;
    [SerializeField] Transform _shootPoint;
    float _damage = 30;
    bool _canShoot = true;
    AudioSource _audioSource;
    [Header("Sounds")]
    [SerializeField] float _reloadTime;
    [SerializeField] AudioClip _shootSound;
    [SerializeField] AudioClip _reloadSound;
    [SerializeField] AudioClip _emptySound;
    [Header("Effects")]
    Dictionary<string, GameObject> _effects;
    [SerializeField] List<string> _effectsName;
    [SerializeField] List<GameObject> _effect;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_effectsName.Count == _effect.Count)
            for (int i = 0; i < _effectsName.Count; ++i)
                _effects.Add(_effectsName[i], _effect[i]);
        else
            Debug.LogError($"Error, effectsName and effects must be same number! {_effectsName.Count} !={ _effect.Count} ");
    }
    private void Update()
    {
        if (!_canShoot)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();
        else if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }
    #region shoot
    public void Shoot()
    {
        if (_bullets > 0)
        {
            // animations from shoot
            RaycastHit hit;
            Debug.DrawRay(_shootPoint.position, _shootPoint.forward, Color.red, 1);
            if (Physics.Raycast(_shootPoint.position, _shootPoint.forward, out hit))
            {
                if (hit.transform.tag == "Player")
                    hit.transform.GetComponent<Target>().TakeDamage(_damage);
                else if (_effects.ContainsKey(hit.transform.tag))
                    Instantiate(_effects[hit.transform.tag], hit.point, Quaternion.LookRotation(hit.normal));
            }
            // _audioSource.PlayOneShot(_shootSound);
            --_bullets;
        }
        else
        {
            // _audioSource.PlayOneShot(_emptySound);
        }
    }
    #endregion
    #region reload
    public void Reload()
    {
        // start ReloadAnimation
        //
        // _audioSource.PlayOneShot(_reloadSound);
        _canShoot = false;
        StartCoroutine(ReloadTimer());
    }
    IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(_reloadTime);
        _canShoot = true;
    }
    #endregion

}
