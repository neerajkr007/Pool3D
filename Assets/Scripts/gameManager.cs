using System.Collections;
using System.IO;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine.UI;
using System.Linq;

public class gameManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    public networkManager networkManager;
    public api api;
    public GameObject mainCam;
    public GameObject playerCam;
    public GameObject ballsParent;
    public GameObject removedBallsParent;
    public GameObject myBall;
    public GameObject[] balls = new GameObject[16];
    public GameObject blackBall;
    public TMPro.TMP_Text debug;
    public bool matchStarted = false;
    public bool matchOver = false;
    public GameObject playerHud;
    public Slider power;
    public bool myTurn = false;
    public GameObject alert;
    public GameObject scoreUI;




    GameObject playerPrefab;
    GameObject myPlayerParent;
    GameObject otherPlayersParent;
    GameObject myPlayer;
    GameObject otherPlayers;
    PhotonView pv;
    


    bool myPlayersReady = false;
    bool otherPlayersReady = false;


    Vector3[] initBallsPosition = new Vector3[16];
    Vector3[] afterRoundBallsPosition = new Vector3[16];
    Vector3 playerPos = new Vector3(-4.815691f, 2.08f, 0f);
    Vector3 playerCamPos = new Vector3(0f, 0.820000172f, 0.815691113f);//new Vector3(-4f, 2.9f, 0f);
    Vector3 viewerCamPos = new Vector3(0f, 1.973f, 0.25f);
    Vector3 viewerPos = new Vector3(0f, 2.08f, -3f);
    Vector3 force = new Vector3(1f, 0f, 0f);
    Vector3 direction;


    bool checkBallStopped = false;
    bool isBreak = true;
    bool myBallIn = false;
    string myBallsType = "";
    [SerializeField]
    bool[] ballsStopped = new bool[16];

    int spotBallsIn = 0;
    int stripeBallsIn = 0;
    [SerializeField]
    int myPoints = 0;
    [SerializeField]
    int opponentPoints = 0;
    bool iWon = false;



    bool once1 = true;
    bool once2 = true;
    void Start()
    {
        //instantiateBalls();
        playerPrefab = PhotonNetwork.Instantiate("player", new Vector3(0f, 2f, -6f), Quaternion.identity, 0);
        playerPrefab.transform.parent = GameObject.Find("myPlayer").transform;
        playerPrefab.GetComponent<PhotonView>().RPC("changeParentForPlayers", RpcTarget.OthersBuffered);
        myPlayer = playerPrefab;
        mainCam.SetActive(false);
        pv = GetComponent<PhotonView>();
        myPlayer.GetComponent<Recorder>().TransmitEnabled = true;
        scoreUI.GetComponent<Canvas>().worldCamera = myPlayer.transform.GetChild(1).GetComponent<Camera>();
        scoreUI.GetComponent<Canvas>().planeDistance = 1;
        scoreUI.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = PhotonNetwork.LocalPlayer.NickName;
        scoreUI.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().text = "games won : " + api.gamesWon;
        scoreUI.transform.GetChild(0).gameObject.SetActive(false);
        scoreUI.transform.GetChild(1).gameObject.SetActive(false);
        for(int i = 0; i < balls.Length; i++)
        {
            initBallsPosition[i] = balls[i].transform.localPosition;
        }
        //afterRoundBallsPositionSwap1();
    }

    void instantiateBalls()
    {
        for(int i = 0; i < 16; i++)
        {
            //balls[i] = Instantiate(ballsPrefab[i], ballsParent.transform.GetChild(i).position, Quaternion.identity);
            //balls[i].transform.parent = ballsParent.transform;
            // balls[i].GetComponent<PhotonView>().RPC("instantiateBallsRPC", RpcTarget.OthersBuffered);
        }
    }

    void FixedUpdate()
    {
        // if(matchStarted)
        // print(otherPlayersParent.transform.GetChild(0).GetChild(1).position);
        //print(ballsParent.transform.GetChild(0).GetComponent<Rigidbody>().velocity.x + "  " + ballsParent.transform.GetChild(0).GetComponent<Rigidbody>().velocity.y + "  " + ballsParent.transform.GetChild(0).GetComponent<Rigidbody>().velocity.z);
        //print(ballsParent.transform.GetChild(0).GetComponent<Rigidbody>().velocity.magnitude);
        updateScore();
        if(once1)
        {
            once1 = false;
            myPlayerParent = GameObject.Find("myPlayer");
            otherPlayersParent = GameObject.Find("otherPlayers");
        }
        if(myPlayerParent.transform.childCount != 0 && !matchStarted )// && PhotonNetwork.IsMasterClient)
        {
            if((-5f <= myPlayer.transform.position.x && myPlayer.transform.position.x <= 5f) && (-4f <= myPlayer.transform.position.z && myPlayer.transform.position.z <= 4f))
            {
                myPlayersReady = true;
            }
            else
            {
                myPlayersReady = false;
            }
        }
        if(otherPlayersParent.transform.childCount != 0 && !matchStarted)
        {
            if((-5f <= otherPlayersParent.transform.GetChild(0).position.x && otherPlayersParent.transform.GetChild(0).position.x <= 5f) && (-4f <= otherPlayersParent.transform.GetChild(0).position.z && otherPlayersParent.transform.GetChild(0).position.z <= 4f))
            {
                otherPlayersReady = true;
            }
            else
            {
                otherPlayersReady = false;
            }
        }
        if(myPlayersReady && otherPlayersReady)
        {
            if(once2)
            {
                once2 = false;
                networkManager.showStartGameMenu();
                myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().enabled = false;
            }
        }
        else
        {
            myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().enabled = true;
            networkManager.hideStartGameMenu();
            once2 = true;
        }

        if (myBall.transform.position.y > 0 && myBall.activeInHierarchy)
        {
            myBallIn = false;
        }
        else
        {
            myBallIn = true;
        }
        if (checkBallStopped)
        {
            //print("checking stop  ");// + myBall.GetComponent<ballStopped>().stopped + "   " + ballsParent.transform.GetChild(ballsParent.transform.childCount - 1).GetComponent<ballStopped>().stopped);
            for(int i = 0; i < ballsParent.transform.childCount; i++)
            {
                Rigidbody currentBallRigidbody = ballsParent.transform.GetChild(i).GetComponent<Rigidbody>();
                if(currentBallRigidbody.velocity.magnitude < 0.12f)// && currentBallRigidbody.velocity.magnitude != 0f)
                {
                    currentBallRigidbody.velocity = Vector3.zero;
                    currentBallRigidbody.angularVelocity = Vector3.zero;
                    ballsParent.transform.GetChild(i).GetComponent<ballStopped>().stopped = true;
                }
            }
            for(int i = 0; i < balls.Length; i++)
            {
                ballsStopped[i] = balls[i].GetComponent<ballStopped>().stopped;
                if(!balls[i].activeInHierarchy || balls[i].transform.localPosition.y < 0)
                {
                    ballsStopped[i] = true;
                }
                if(myBall.transform.position.y < 0)
                {
                    balls[15].GetComponent<ballStopped>().stopped = true;
                }
            }
            
            if (ballsStopped.All(x => x))
            {
                checkBallStopped = false;
                for(int i = 0; i < balls.Length; i++)
                {
                    balls[i].GetComponent<ballStopped>().stopped = false;
                    ballsStopped[i] = balls[i].GetComponent<ballStopped>().stopped;
                    balls[i].GetComponent<SphereCollider>().isTrigger = true;
                    balls[i].GetComponent<Rigidbody>().isKinematic = true;
                    balls[i].GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }
                stopBalls();
                if (isBreak)
                {
                    isBreak = false;
                    firstRoundOver();
                }
                else
                {
                    StartCoroutine(waitForASec());
                }
            }
        }
    }

    IEnumerator waitForASec()
    {
        yield return new WaitForSeconds(1f);
        roundOver();
    }

    void updateScore()
    {
        scoreUI.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "Your Score : " + myPoints;
        scoreUI.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "Opponent Score : " + opponentPoints;
    }

    public void notReady()
    {
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().enabled = true;
        networkManager.hideStartGameMenu();
        pv.RPC("notReadyRPC", RpcTarget.Others);
    }

    public void justBeforeMatchStarts()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].GetComponent<SphereCollider>().isTrigger = true;
            balls[i].GetComponent<Rigidbody>().isKinematic = true;
            balls[i].GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    public void startGame()
    {
        scoreUI.transform.GetChild(0).gameObject.SetActive(true);
        scoreUI.transform.GetChild(1).gameObject.SetActive(true);


        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().enabled = true;


        myPlayer.GetComponent<CharacterController>().enabled = false;
        // otherPlayersParent.transform.GetChild(0).GetComponent<PhotonTransformView>().m_SynchronizePosition = false;
        // myPlayer.GetComponent<PhotonTransformView>().m_SynchronizePosition = false;

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"])
        {
            placeTurnPlayer();
        }
        else
        {
            placeNotTurnPlayer();
        }

        myPlayer.GetComponent<CharacterController>().enabled = true;
        StartCoroutine(waitForASec2());
        //pv.RPC("replaceBalls", RpcTarget.All);
        
        // otherPlayersParent.transform.GetChild(0).GetComponent<PhotonTransformView>().m_SynchronizePosition = true;
        // myPlayer.GetComponent<PhotonTransformView>().m_SynchronizePosition = true;
    }

    IEnumerator waitForASec2()
    {
        stopBalls();
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].GetComponent<SphereCollider>().isTrigger = false;
            balls[i].GetComponent<Rigidbody>().isKinematic = false;
            balls[i].GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        stopBalls();
    }

    void placeTurnPlayer()
    {
        myPlayer.transform.position = playerPos;
        myPlayer.transform.eulerAngles = new Vector3(0f, 90f, 0f);
        myPlayer.GetComponent<characterMovement>().moveClose = true;


        myPlayer.transform.GetChild(1).localPosition = playerCamPos;
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().LookAtBall = true;
        myPlayer.transform.GetChild(1).eulerAngles = new Vector3(20.3f, 90f, 0f);


        playerHud.SetActive(true);
    }

    void placeNotTurnPlayer()
    {
        myPlayer.transform.position = viewerPos;
        myPlayer.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        myPlayer.GetComponent<characterMovement>().moveClose = false;

        myPlayer.transform.GetChild(1).localPosition = viewerCamPos; 
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().LookAtBall = false;
        
        playerHud.SetActive(false);
    }

    void stopBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            balls[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    public void shoot()
    {
        stopBalls();
        Vector3 direction = new Vector3((myBall.transform.position.x - myPlayer.transform.position.x), 0f, (myBall.transform.position.z - myPlayer.transform.position.z)).normalized;
        force = power.value/2f*direction;
        pv.RPC("shot", RpcTarget.All, force);
    }

    public void resetBall()
    {
        myBall.transform.position = new Vector3(-2.0f, 1.3f, 0.0f);
        
    }

    void firstRoundOver()
    {
        for (int i = 0; i < balls.Length - 1; i++)
        {
            if (!balls[i].activeInHierarchy && i < 7)
            {
                spotBallsIn++;
                balls[i].transform.parent = removedBallsParent.transform;
            }
            if(!balls[i].activeInHierarchy && i > 7)
            {
                stripeBallsIn++;
                balls[i].transform.parent = removedBallsParent.transform;
            }
        }
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] == true)
        {

            if (spotBallsIn >= stripeBallsIn && spotBallsIn != 0 && spotBallsIn != 0)
            {
                myBallsType = "spot";
                myPoints = spotBallsIn;
                opponentPoints = stripeBallsIn;
            }
            else if(spotBallsIn < stripeBallsIn && spotBallsIn != 0 && spotBallsIn != 0)
            {
                myBallsType = "stripe";
                myPoints = stripeBallsIn;
                opponentPoints = spotBallsIn;
            }
            else if (spotBallsIn == 0 && spotBallsIn == 0)
            {
                myBallsType = "spot";
                myPoints = spotBallsIn;
                opponentPoints = stripeBallsIn;
            }
            //print(spotBallsIn + "  " + stripeBallsIn + "  " + myBallsType);
        }
        else
        {
            if (spotBallsIn >= stripeBallsIn && spotBallsIn != 0 && spotBallsIn != 0)
            {
                myBallsType = "stripe";
                myPoints = stripeBallsIn;
                opponentPoints = spotBallsIn;
            }
            else if(spotBallsIn < stripeBallsIn && spotBallsIn != 0 && spotBallsIn != 0)
            {
                myBallsType = "spot";
                myPoints = spotBallsIn;
                opponentPoints = stripeBallsIn;
            }
            else if(spotBallsIn == 0 && spotBallsIn == 0)
            {
                myBallsType = "stripe";
                myPoints = stripeBallsIn;
                opponentPoints = spotBallsIn;
            }
        }
        alert.SetActive(true);
        alert.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = "you are - " + myBallsType;
        StartCoroutine(hideAlert());
        roundOver();
    }

    void roundOver()
    {
        resetMyBall();
        checkMatchOver();
        checkWinner();
        //afterRoundBallsPositionSwap1();

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] == false)
        {
            if (!blackBall.activeInHierarchy)
            {
                alert.SetActive(true);
                alert.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = "You won the match! (black ball foul)";
                iWon = true;
                matchOver = true;
                myPoints = 0;
                opponentPoints = 0;
                StartCoroutine(hideAlert());
                return;
            }
            PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] = true;
            if(myBall.transform.position.x > 0 && !myBallIn)
            {
                if(myBall.transform.position.z > 0)
                {
                    playerPos = new Vector3(5f, 2.08f, 3.1f);
                }
                else
                {
                    playerPos = new Vector3(5f, 2.08f, -3.1f);
                }
            }
            else if(myBall.transform.position.x <= 0 && !myBallIn)
            {
                if(myBall.transform.position.z > 0)
                {
                    playerPos = new Vector3(-5f, 2.08f, 3.1f);
                }
                else
                {
                    playerPos = new Vector3(-5f, 2.08f, -3.1f);
                }
            }
            if(myBallIn)
            {
                playerPos = new Vector3(-4.815691f, 2.08f, 0f);
                myBallIn = false;
            }
        }
        else
        {
            if (!blackBall.activeInHierarchy)
            {
                alert.SetActive(true);
                alert.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = "You lost the match! (black ball foul)";
                iWon = false;
                matchOver = true;
                myPoints = 0;
                opponentPoints = 0;
                StartCoroutine(hideAlert());
                return;
            }
            PhotonNetwork.LocalPlayer.CustomProperties["myTurn"] = false;
        }
        countScore();
        startGame();
    }

    void resetMyBall()
    {
        if (myBallIn)
        {
            myBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            myBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            myBall.SetActive(true);
            myBall.transform.position = new Vector3(-2.0f, 1.3f, 0.0f);
        }
    }


    void countScore()
    {
        for (int i = 0; i < balls.Length - 1; i++)
        {
            if (!balls[i].activeInHierarchy && i < 7 && balls[i].transform.parent == ballsParent.transform)
            {
                spotBallsIn++;
                if (myBallsType == "spot")
                {
                    myPoints++;
                }
                else
                {
                    opponentPoints++;
                }
                balls[i].transform.parent = removedBallsParent.transform;
            }
            if (!balls[i].activeInHierarchy && i > 7 && balls[i].transform.parent == ballsParent.transform)
            {
                stripeBallsIn++;
                if (myBallsType == "stripe")
                {
                    myPoints++;
                }
                else
                {
                    opponentPoints++;
                }
                balls[i].transform.parent = removedBallsParent.transform;
            }
        }

    }

    void checkMatchOver()
    {
        for (int i = 0; i < balls.Length - 1; i++)
        {
            if (balls[i].activeInHierarchy)
            {
                matchOver = false;
                break;
            }
            matchOver = true;
        }
    }

    void checkWinner()
    {
        if (matchOver)
        {
            alert.SetActive(true);
            if (myPoints > opponentPoints)
            {
                alert.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = "You won the match!";
                iWon = true;
            }
            else
            {
                alert.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = "You lost the match!";
                iWon = false;
            }
            StartCoroutine(hideAlert());
            return;
        }
    }

    IEnumerator afterShot()
    {
        yield return new WaitForSeconds(1f);
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().LookAtBall = false;
        //myPlayer.transform.eulerAngles = new Vector3(0f, 90f, 0f);
        playerHud.SetActive(false);
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().enabled = true;
        myPlayer.transform.GetChild(1).localPosition = viewerCamPos;
        myPlayer.GetComponent<characterMovement>().moveClose = false;
        checkBallStopped = true;
    }

    IEnumerator hideAlert()
    {
        if(matchOver)
        {
            matchStarted = false;
            if(iWon)
            {
                api.updateGamesWon();
            }
            resetPlayers();
            resetScore();
            resetBalls();
            yield return new WaitForSeconds(5f);
            alert.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(2f);
            alert.SetActive(false);
        }
    }

    void resetPlayers()
    {
        myPlayer.GetComponent<characterMovement>().moveClose = false;
        playerHud.SetActive(false);
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().enabled = true;
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().LookAtBall = false;
    }

    void resetBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].SetActive(true);
            balls[i].transform.SetSiblingIndex(i);
            balls[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            balls[i].GetComponent<Rigidbody>().angularVelocity= Vector3.zero;
            balls[i].transform.parent = ballsParent.transform;
            balls[i].transform.localPosition = initBallsPosition[i];
        }
        myPlayer.transform.GetChild(1).localPosition = viewerCamPos;
    }

    void resetScore()
    {
        myBallsType = "";
        myPoints = 0;
        opponentPoints = 0;
        scoreUI.transform.GetChild(0).gameObject.SetActive(false);
        scoreUI.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void updateGamesWon(int n)
    {
        scoreUI.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().text = "games won : " + n;
    }




    [PunRPC]
    void notReadyRPC()
    {
        myPlayer.transform.GetChild(1).GetComponent<mouseMovement>().enabled = true;
        networkManager.otherPlayerNotReady();
    }

    [PunRPC]
    void shot(Vector3 force)
    {
        myBall.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        StartCoroutine(afterShot());
    }
    

}
