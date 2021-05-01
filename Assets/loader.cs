using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("starter");
    }

    IEnumerator starter()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }
}
