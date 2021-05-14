using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using DG.Tweening;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts.Colt
{
    public class Colt : MonoBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] Transform _shootPoint;
        [SerializeField] Transform _trigger;
        [SerializeField] Drum _drum;
        float _scores = 0;
        [SerializeField] TMPro.TMP_Text _scoresText;
        Transform _drumOTransform;

        [Header("Gun settings")]
        [SerializeField] float _fireRate = 0.3f;
        float _shootAnimationSpeed;
        byte _maxBullets;
        byte _bullets = 6;
        [Range(0, 20)]


        readonly float _damage = 30;
        bool _canShoot = true;
        [SerializeField] float _reloadTime;
        AudioSource _audioSource;
        [Header("Sounds")]
        [SerializeField] AudioClip[] _shootSounds;
        AudioClip _shootSound;

        [SerializeField] AudioClip _emptySound;
        [Header("Effects")]
        [SerializeField] GameObject _shootEffect;
        Dictionary<string, GameObject> _effects;
        [SerializeField] List<string> _effectsName;
        [SerializeField] List<GameObject> _effect;

        // bhaptic
        [Header("Vr settings")]
        [SerializeField] SteamVR_Action_Boolean _fireAction;
        [SerializeField] SteamVR_Action_Boolean _reloadAction;
        Interactable _interactable;
        BhapticConnect _bhapticConnect;

        private void Start()
        {
            _maxBullets = _bullets;
            _audioSource = GetComponent<AudioSource>();
            _shootSound = _shootSounds[Random.Range(0, _shootSounds.Length)];
            _bhapticConnect = GetComponent<BhapticConnect>();
            _bhapticConnect.shootingPoint = _shootPoint;
            _interactable = GetComponent<Interactable>();
            _shootAnimationSpeed = _fireRate / 2;
            _drumOTransform = _drum.gameObject.transform;
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
                if (_fireAction[source].stateDown)
                {
                    Shoot();
                }
                else if (_reloadAction[source].stateDown)
                {
                    Reload();
                }
            }
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
                // particle effects
                _shootEffect.SetActive(false);
                _shootEffect.SetActive(true);
                // Trigger and drum effect
                ActivateTrigger();
                _drum.Shoot(_bullets, _fireRate);
                Debug.DrawRay(_shootPoint.position, _shootPoint.forward * 100, Color.red, 1);
                if (Physics.Raycast(_shootPoint.position, _shootPoint.forward * 100, out RaycastHit raycastHit, maxDistance: 1000))
                {
                    print($"107. Colt -> shoot at: {raycastHit.transform.gameObject}");
                    if (raycastHit.transform.tag == "Player")
                    {
                        var damageInfo = raycastHit.transform.GetComponent<TargetPart>().TakeDamage(_damage);
                        UpdateScore(damageInfo.Item1, damageInfo.Item2);
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
            _trigger.DOLocalRotate(new Vector3(0, 25, 0), _shootAnimationSpeed);
            StartCoroutine(ReturnTrigger());
        }

        IEnumerator ReturnTrigger()
        {
            yield return new WaitForSeconds(_shootAnimationSpeed);
            _trigger.DOLocalRotate(new Vector3(0, 0, 0), _fireRate);
        }
        void UpdateScore(bool kill, float addScores)
        {
            _scores += addScores;
            if (kill)
                _scores += 50;
            _scoresText.text = $"{_scores}";
        }
        #endregion
        #region reload
        public void Reload()
        {
            if (_bullets != _maxBullets)
            {
                // start ReloadAnimation
                //
                _drum.Open();
                _canShoot = false;
                _bullets = _maxBullets;
                StartCoroutine(EndReloadTimer());
            }

        }
        IEnumerator EndReloadTimer()
        {
            yield return new WaitForSeconds(_reloadTime);
            _canShoot = true;
            _drum.Close();
        }
        #endregion

    }
}
