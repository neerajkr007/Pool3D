using UnityEngine;
using System.Collections;
using System;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class networkManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    public gameManager gameManager;
    public GameObject prev;
    public GameObject next;
    public GameObject gamemodesMenu;
    public GameObject matchMakingMenu;
    public GameObject startGameMenu;
    public GameObject errorMenu;
    public TMPro.TMP_Text info;
    public TMPro.TMP_InputField playerName;
    string RoomId;
    PhotonView pv;
    string joinType = "";
    void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = "v1.0";
            PhotonNetwork.ConnectUsingSettings();
        }
        pv = GetComponent<PhotonView>();
    }
    public override void OnConnectedToMaster()
    {
        print("connected to server. ");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("disconnected from server coz " + cause.ToString());
        //msg.text = cause.ToString();
    }

    public void MatchMaking()
    {
        if(playerName.text == "")
        {
            errorMenu.SetActive(true);
            return;
        }
        
        PhotonNetwork.JoinRandomRoom();
    }

    public void createRoom()
    {
        if (playerName.text == "")
        {
            errorMenu.SetActive(true);
            return;
        }
        joinType = "friends";
        RoomId = "room" + UnityEngine.Random.Range(1000, 9999).ToString();
        RoomOptions options = new RoomOptions();
        options.IsVisible = true;
        options.MaxPlayers = (byte)2;
        PhotonNetwork.CreateRoom(RoomId, options, TypedLobby.Default);
    }

    public void joinOrCreate()
    {
        RoomId = "dev";//+ UnityEngine.Random.Range(1000, 9999).ToString();
        RoomOptions options = new RoomOptions();
        options.IsVisible = true;
        options.MaxPlayers = (byte)2;
        PhotonNetwork.JoinOrCreateRoom(RoomId, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        print("Room created successfully, Id - " + RoomId);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("room creation failed coz " + message);
        Debug.Log("room creation failed coz " + message);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("joined the room");
        //info.text = PhotonNetwork.CurrentRoom.Name;
        prev.SetActive(false);
        next.SetActive(true);
        /*if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            prev.SetActive(false);
            next.SetActive(true);
        }
        else
        {
            if(joinType == "online")
            {
                gamemodesMenu.SetActive(false);
                matchMakingMenu.SetActive(true);
            }
        }*/
        //PhotonNetwork.NickName = playerName.text;
        PhotonNetwork.NickName = "Player " + PhotonNetwork.CurrentRoom.PlayerCount;
        Hashtable hash = new Hashtable();
        if(PhotonNetwork.IsMasterClient)
        {
            hash.Add("myTurn", true);
        }
        else
        {
            hash.Add("myTurn", false);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("failed coz " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("random room join failed");
        createRoom();
        joinType = "online";
        print("type changed");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       // info.text = (PhotonNetwork.PlayerList[0] + " " + PhotonNetwork.PlayerList[1]);
    }

    public void exitMatchmaking()
    {
        PhotonNetwork.LeaveRoom();
        gamemodesMenu.SetActive(true);
        matchMakingMenu.SetActive(false);
    }

    public void showStartGameMenu()
    {
        startGameMenu.SetActive(true);
        
    }

    public void hideStartGameMenu()
    {
        startGameMenu.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        startGameMenu.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        startGameMenu.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        startGameMenu.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
    }

    public void otherPlayerNotReady()
    {
        startGameMenu.transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        startGameMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        StartCoroutine(waitForCancel());
    }

    public void setReady()
    {
        pv.RPC("otherPlayerReady", RpcTarget.Others, PhotonNetwork.NickName);
        startGameMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        startGameMenu.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        startGameMenu.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        if(startGameMenu.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TMP_Text>().text == "")
        {
            startGameMenu.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TMP_Text>().text = PhotonNetwork.NickName;
            // for dev use only
            /*startGameMenu.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            gameManager.matchStarted = true;
            StartCoroutine(startMatch());*/
        }
        else
        {
            startGameMenu.transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<TMPro.TMP_Text>().text = PhotonNetwork.NickName;
            // start match
            startGameMenu.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            gameManager.matchStarted = true;
            StartCoroutine(startMatch());
        }
    }

    IEnumerator startMatch()
    {
        gameManager.justBeforeMatchStarts();
        yield return new WaitForSeconds(2f);
        hideStartGameMenu();
        gameManager.startGame();
    }

    IEnumerator waitForCancel()
    {
        yield return new WaitForSeconds(2f);
        hideStartGameMenu();
    }







    [PunRPC]
    void otherPlayerReady(string otherPlayerName)
    {
        if(startGameMenu.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TMP_Text>().text == "")
        {
            startGameMenu.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TMP_Text>().text = otherPlayerName;
        }
        else
        {
            gameManager.debug.text = startGameMenu.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TMP_Text>().text;
            startGameMenu.transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<TMPro.TMP_Text>().text = otherPlayerName;
            // start match
            startGameMenu.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            gameManager.matchStarted = true;
            StartCoroutine(startMatch());
        }
    }
}
