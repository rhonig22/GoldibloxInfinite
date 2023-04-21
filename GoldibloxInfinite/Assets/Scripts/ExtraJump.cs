using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    private bool up = true;
    private float initialPause = 0;

    private void Start()
    {
        initialPause = Random.Range(0f, .5f);
        StartCoroutine(FlipDiff());
    }

    // Update is called once per frame
    void Update()
    {
        if (initialPause == 0)
        {
            float diff = Time.deltaTime * (up ? .5f : -.5f);
            transform.Translate(0, diff, 0);
        }
    }

    private IEnumerator FlipDiff()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f + initialPause);
            initialPause = 0;
            up = !up;
        }
    }
}
