using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class photonPlayer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<PhotonView>().RPC("printCollision", RpcTarget.All, collision.collider.name);
        
    }

    [PunRPC]
    void changeParentForPlayers()
    {
        transform.parent = GameObject.Find("otherPlayers").transform;
        transform.GetChild(1).gameObject.SetActive(false);
    }

    [PunRPC]
    void printCollision(string name)
    {
        print(name);
    }
}
