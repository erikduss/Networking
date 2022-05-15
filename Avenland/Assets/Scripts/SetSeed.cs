using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSeed : MonoBehaviour
{
    public int seed;
    public bool randomizeSeed;

    private void Awake()
    {
        if (randomizeSeed)
        {
            seed = Random.Range(0,99999);
        }

        Random.InitState(seed);

        Debug.Log(seed);
    }
}
