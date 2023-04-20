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

    // Start is called before the first frame update
    void Start()
    {
        ResetPlayerLocation(DataManager.currentCheckPointIndex);
    }

    public void ResetPlayerLocation(int index)
    {
        Vector3 startPosition = transform.position;
        startPosition.z = player.transform.position.z;
        if (index < checkpoints.Length)
        {
            startPosition = checkpoints[index].transform.position;
        }
        
        player.transform.position = startPosition;
        if (isStartAtSouth)
        {
            player.GetComponent<Rigidbody2D>().velocity = southStartVelocity;
        }
        else
        {
            player.GetComponent<Rigidbody2D>().velocity = DataManager.playerVelocity;
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
}
