using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class characterMovement : MonoBehaviourPunCallbacks
{
    public float speed = 12f;
    public bool moveClose = false;
    gameManager gameManager;
    TMPro.TMP_Text debug;
    CharacterController controller;
    Animator animator;
    PhotonView pv;
    GameObject playerBall;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
        //debug = GameObject.Find("debug").GetComponent<TMPro.TMP_Text>();
        gameManager = gameObject.transform.parent.parent.GetChild(0).GetComponent<gameManager>();
        playerBall = GameObject.Find("PlayerBall");
    }
    void Update()
    {
        if(pv.IsMine && !moveClose)
        {
            //print(transform.position.x + " " + transform.position.y + " " + transform.position.z);
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            if(x != 0 || z != 0)
            {
                animator.SetBool("moving", true);
            }
            else
            {
                animator.SetBool("moving", false);
            }
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed* Time.deltaTime);
        }
        else if(pv.IsMine && moveClose)
        {
            float x = Input.GetAxis("Horizontal");
            Vector3 move = Vector3.zero;
            if(x != 0)
            {
                animator.SetBool("moving", true);
                
            }
            else
            {
                animator.SetBool("moving", false);
            }
            
            // Vector3 move = playerBall.transform.position - transform.position;
            move = transform.right * x;
            if(transform.localPosition.x >= 5f || transform.localPosition.x <= -5f || transform.localPosition.z >= 3.1f || transform.localPosition.z <= -3.1f)
            {
                Vector3 displacement = new Vector3((playerBall.transform.position.x - transform.position.x), 0f, (playerBall.transform.position.z - transform.position.z));
                move = displacement + (transform.right * x);
            }
            controller.Move(move * (speed / 2)* Time.deltaTime);
        }
        
    }

}
