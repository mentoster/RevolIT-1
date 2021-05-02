using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colt : MonoBehaviour
{
    [SerializeField] byte _bullets = 6;
    byte _maxBullets;
    [SerializeField] Transform _shootPoint;
    [SerializeField] float _fireRate = 0.3f;
    [SerializeField] GameObject _shootEffect;
    [SerializeField] GameObject bullet;
    readonly float _damage = 30;
    bool _canShoot = true;
    AudioSource _audioSource;
    [Header("Sounds")]
    [SerializeField] float _reloadTime;
    [SerializeField] AudioClip _shootSound;
    [SerializeField] AudioClip _openReloadSound;
    [SerializeField] AudioClip _closeReloadSound;
    [SerializeField] AudioClip _emptySound;
    [Header("Effects")]
    Dictionary<string, GameObject> _effects;
    [SerializeField] List<string> _effectsName;
    [SerializeField] List<GameObject> _effect;

    private void Start()
    {
        _maxBullets = _bullets;
        _audioSource = GetComponent<AudioSource>();
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
            --_bullets;
            _audioSource.PlayOneShot(_shootSound);
            _shootEffect.SetActive(false);
            _shootEffect.SetActive(true);
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
            _canShoot = false;
            StartCoroutine(FireRateTimer());
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
    #endregion
    #region reload
    public void Reload()
    {
        if (_bullets != _maxBullets)
        {
            // start ReloadAnimation
            //
            _audioSource.PlayOneShot(_openReloadSound);
            _canShoot = false;
            _bullets = _maxBullets;
            StartCoroutine(ReloadTimer());
        }
    }
    IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(_reloadTime);
        _canShoot = true;
        _audioSource.PlayOneShot(_closeReloadSound);
        for (int i = 0; i < _maxBullets; ++i)
        {
            float r = 5f;
            float angle = Mathf.PI * 2 / i;
            Vector2 pos2d = new Vector2(Mathf.Sin(angle) * r, Mathf.Cos(angle) * r);
            Instantiate(bullet, new Vector3(pos2d.x, pos2d.y, 0), transform.rotation);
        }
    }
    #endregion

}
