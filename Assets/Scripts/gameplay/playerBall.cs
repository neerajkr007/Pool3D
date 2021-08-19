using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class playerBall : MonoBehaviourPunCallbacks
{
    //public GameObject ballsParent;
    Rigidbody rb;
    Vector3 force = new Vector3(5f, 0f, 0f);
    Vector3 direction;
    
    [SerializeField]
    GameObject ballsParent;
    GameObject gameManager;

    void Start()
    {
        gameManager = GameObject.Find("gameManager");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDown()
    {
        /*gameManager.GetComponent<PhotonView>().RPC("shot", RpcTarget.Others, force);
        rb.AddForce(force, ForceMode.Impulse);*/
        //rigidbody.AddForceAtPosition(transform.position, force, ForceMode.Impulse);
    }
    
    [PunRPC]
    public void instantiateBallsRPC()
    {
        // print("rpc " + PhotonNetwork.IsMasterClient);
        // GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        // ballsParent = GameObject.Find("balls");
        // transform.parent = ballsParent.transform;
        // GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    
}
