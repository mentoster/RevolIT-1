﻿using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Colt : MonoBehaviour
{
    [SerializeField] byte _bullets = 6;
    byte _maxBullets;
    [SerializeField] Transform _shootPoint;
    [SerializeField] float _fireRate = 0.3f;
    [SerializeField] GameObject _shootEffect;
    [SerializeField] Drum _drum;

    readonly float _damage = 30;
    bool _canShoot = true;
    [SerializeField] float _reloadTime;
    AudioSource _audioSource;
    [Header("Sounds")]
    [SerializeField] AudioClip[] _shootSounds;
    AudioClip _shootSound;

    [SerializeField] AudioClip _emptySound;
    [Header("Effects")]
    Dictionary<string, GameObject> _effects;
    [SerializeField] List<string> _effectsName;
    [SerializeField] List<GameObject> _effect;

    // bhaptic
    BhapticConnect _bhapticConnect;
    [Header("Vr settings")]
    [SerializeField] SteamVR_Action_Boolean fireAction;
    Interactable _interactable;

    private void Start()
    {
        _maxBullets = _bullets;
        _audioSource = GetComponent<AudioSource>();
        _shootSound = _shootSounds[Random.Range(0, _shootSounds.Length)];
        _bhapticConnect = GetComponent<BhapticConnect>();
        _bhapticConnect.shootingPoint = _shootPoint;
        _interactable = GetComponent<Interactable>();
        if (_audioSource == null)
            Debug.LogError("No audiosource!");
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
        if (_interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources source = _interactable.attachedToHand.handType;
            if (fireAction[source].stateDown)
            {
                Shoot();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && _bullets != _maxBullets)
        {
            Reload();
        }
    }
    #region shoot
    public void Shoot()
    {
        if (_bullets > 0)
        {
            --_bullets;
            _audioSource.PlayOneShot(_shootSound);

            // particle effects
            _shootEffect.SetActive(false);
            _shootEffect.SetActive(true);

            Debug.DrawRay(_shootPoint.position, _shootPoint.forward * 100, Color.red, 1);
            if (Physics.Raycast(_shootPoint.position, _shootPoint.forward * 100, out RaycastHit raycastHit, maxDistance: 1000))
            {
                if (raycastHit.transform.tag == "Player")
                {
                    // raycastHit.transform.GetComponent<Target>().TakeDamage(_damage);
                    _bhapticConnect.Play(raycastHit: raycastHit);
                }
                // else if (_effects.ContainsKey(raycastHit.transform.tag))
                // Instantiate(_effects[raycastHit.transform.tag], raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
            }

            StartCoroutine(FireRateTimer());
            _canShoot = false;
        }
        else
        {
            _audioSource.PlayOneShot(_emptySound);
        }
    }
    IEnumerator FireRateTimer()
    {
        yield return new WaitForSeconds(_fireRate);
        _canShoot = true;
    }
    void ActivateTrigger()
    {

    }
    IEnumerator TweenRotation(Transform trans, Quaternion destRot, float speed, float threshold)
    {
        float angleDist = Quaternion.Angle(trans.rotation, destRot);

        while (angleDist > threshold)
        {
            trans.rotation = Quaternion.RotateTowards(trans.rotation, destRot, Time.deltaTime * speed);
            yield return null;

            angleDist = Quaternion.Angle(trans.rotation, destRot);
        }
    }
    #endregion
    #region reload
    public void Reload()
    {
        // start ReloadAnimation
        //
        _drum.Open();
        _canShoot = false;
        _bullets = _maxBullets;
        StartCoroutine(EndReloadTimer());

    }
    IEnumerator EndReloadTimer()
    {
        yield return new WaitForSeconds(_reloadTime);
        _canShoot = true;
        _drum.Close();
    }
    #endregion

}
