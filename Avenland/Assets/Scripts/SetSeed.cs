using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSeed : MonoBehaviour
{
    public int seed;
    public bool randomizeSeed = false;

    public void SetNewSeed(int newSeed)
    {
        seed = newSeed;

        if (randomizeSeed)
        {
            seed = Random.Range(0,99999);
        }

        Random.InitState(seed);

        Debug.Log(seed);
    }
}
