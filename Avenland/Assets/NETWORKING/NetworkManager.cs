using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private static uint nextNetworkId = 0;
    public static uint NextNetworkID => ++nextNetworkId;

    [SerializeField]
    private NetworkSpawnInfo spawnInfo;
    private Dictionary<uint, GameObject> networkedReferences = new Dictionary<uint, GameObject>();

    public bool GetReference( uint id, out GameObject obj ) {
        obj = null;
        if (networkedReferences.ContainsKey(id)) {
            obj = networkedReferences[id];
            return true;
		}
        return false;
	}

    public bool SpawnWithId(NetworkSpawnObject type, uint id, out GameObject obj ) {
        obj = null;
        if ( networkedReferences.ContainsKey(id)) {
            return false;
		}
        else {
            // assuming this doesn't crash...
            obj = GameObject.Instantiate(spawnInfo.prefabList[(int)type]);
            
            NetworkedBehaviour beh = obj.GetComponent<NetworkedBehaviour>();
            if ( beh == null ) {
                beh = obj.AddComponent<NetworkedBehaviour>();
			}
            beh.networkId = id;

            networkedReferences.Add(id, obj);

            return true;
		}
	}

    public bool ReplaceGameobjectWithNew(NetworkSpawnObject type, uint id, out GameObject obj)
    {
        obj = null;
        if (!networkedReferences.ContainsKey(id))
        {
            return false;
        }
        else
        {
            if(networkedReferences[id] != null)
            {
                NetworkedGamePlayer tempPlayer = networkedReferences[id].GetComponent<NetworkedGamePlayer>();
                if (tempPlayer != null) return false;
            }

            // assuming this doesn't crash...
            obj = GameObject.Instantiate(spawnInfo.prefabList[(int)type]);

            NetworkedBehaviour beh = obj.GetComponent<NetworkedBehaviour>();
            if (beh == null)
            {
                beh = obj.AddComponent<NetworkedBehaviour>();
            }
            beh.networkId = id;

            networkedReferences[id] = obj;

            return true;
        }
    }

    public void ClearAllNetworkReferences()
    {
        nextNetworkId = 0;
        networkedReferences.Clear();
    }

    public bool DestroyWithId(uint id) {
        if (networkedReferences.ContainsKey(id)) {
            Destroy(networkedReferences[id]);
            networkedReferences.Remove(id);
            return true;
        }
        else {
            return false;
        }
    }
}