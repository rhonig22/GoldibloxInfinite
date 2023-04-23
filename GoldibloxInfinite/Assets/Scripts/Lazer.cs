using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    [SerializeField] GameObject lazerContainer;
    [SerializeField] GameObject lazerContainerOff;
    [SerializeField] bool startOff = false;
    private bool isOff = false;
    private float waitTime = 4f/3f;
    private bool isFlipped = false;
    private Vector3 normalScale = new Vector3(.5f, 1, 1);
    private Vector3 flippedScale = new Vector3(.5f, -1, 1);


    // Start is called before the first frame update
    void Start()
    {
        lazerContainer.SetActive(!startOff);
        lazerContainerOff.SetActive(startOff);
        isOff = startOff;
        StartCoroutine(SwitchLazerActive());
    }

    IEnumerator SwitchLazerActive()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            lazerContainer.SetActive(isOff);
            lazerContainerOff.SetActive(!isOff);
            isOff = !isOff;
        }
    }

    public void FlipLazer()
    {
        if (isFlipped)
            transform.localScale = normalScale;
        else
            transform.localScale = flippedScale;

        isFlipped = !isFlipped;
    }
}
