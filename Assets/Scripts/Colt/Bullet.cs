using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] AudioClip groundSound;
    AudioSource _audioSource;


    private void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }
    
}
