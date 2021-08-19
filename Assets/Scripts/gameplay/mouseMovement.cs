using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mouseMovement : MonoBehaviourPunCallbacks
{
    public float sensi = 200f;
    public bool LookAtBall = false;
    Transform player;
    PhotonView pv;
    gameManager gameManager;
    GameObject playerBall;


    float horizontalRotation = 0f;
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        player = transform.parent;
        pv = transform.parent.GetComponent<PhotonView>();
        gameManager = gameObject.transform.parent.parent.parent.GetChild(0).GetComponent<gameManager>();
        playerBall = GameObject.Find("PlayerBall");
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine && !LookAtBall)
        {
            float x = Input.GetAxis("Mouse X") * sensi * Time.deltaTime;
            float y = Input.GetAxis("Mouse Y") * sensi * Time.deltaTime;

            horizontalRotation -= y;
            horizontalRotation = Mathf.Clamp(horizontalRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(horizontalRotation, 0f, 0f);

            player.Rotate(Vector3.up * x);
        }
        else if(pv.IsMine && LookAtBall)
        {
            if((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"])
            {
                transform.parent.LookAt(playerBall.transform);
                transform.parent.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }
}
