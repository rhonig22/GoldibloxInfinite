using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] checkpoints;
    [SerializeField] private GameObject player;
    [SerializeField] public bool isStartAtSouth = false;
    private Vector2 southStartVelocity = Vector2.up * 10;
    private GameObject playerInstance;
    private float transitionWaitTime = .25f;

    // Start is called before the first frame update
    void Start()
    {
        playerInstance = Instantiate(player);
        ResetPlayerLocation(DataManager.currentCheckPointIndex);
        StartCoroutine(StartPlayer());
    }

    public void ResetPlayerLocation(int index)
    {
        Vector3 startPosition = transform.position;
        startPosition.z = playerInstance.transform.position.z;
        if (index < checkpoints.Length)
        {
            startPosition = checkpoints[index].transform.position;
        }

        playerInstance.transform.position = startPosition;
        if (isStartAtSouth)
        {
            playerInstance.GetComponent<Rigidbody2D>().velocity = southStartVelocity;
        }
        else
        {
            playerInstance.GetComponent<Rigidbody2D>().velocity = DataManager.playerVelocity;
        }
    }

    public void SetCheckPoint(GameObject checkpoint)
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] == checkpoint)
                DataManager.Instance.SetCheckPointIndex(i);
        }
    }

    IEnumerator StartPlayer()
    {
        yield return new WaitForSeconds(transitionWaitTime);
        playerInstance.GetComponent<Rigidbody2D>().simulated = true;
    }
}
