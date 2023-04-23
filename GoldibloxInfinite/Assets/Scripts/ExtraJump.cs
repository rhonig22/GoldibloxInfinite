using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    [SerializeField] Animator animator;
    private float initialPause = 0;

    private void Start()
    {
        StartCoroutine(Pause());
    }

    private IEnumerator Pause()
    {
        initialPause = Random.Range(0f, 1f);
        yield return new WaitForSeconds(initialPause);
        animator.SetTrigger("Start");
    }
}
