using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkSpawnObject
{
	PLAYERLOBBY = 0,
	PLAYER = 1
}

[CreateAssetMenu(menuName = "My Assets/NetworkSpawnInfo")]
public class NetworkSpawnInfo : ScriptableObject
{
	// TODO: A serializableDictionary would help here...
	public List<GameObject> prefabList = new List<GameObject>();
}
