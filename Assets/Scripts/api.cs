using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class api : MonoBehaviourPunCallbacks
{
    public TextAsset text;
    public int gamesWon;

    [Serializable]
    public class Root
    {
        public string _type;
        public long _version;
        public int __export_format;
        public DateTime __export_date;
        public string __export_source;
        public List<Resource> resources;
    }



    [Serializable]
    public class Param
    {
        public string description;
        public bool disabled;
        public string id;
        public string name;
        public string value;
        public string fileName;
        public string type;
    }

    [Serializable]
    public class Data
    {
        public string username;
        public string gender;
        public string character_id;
        public string intro_crib_tutorial_completed;
        public int pool_games_won;
    }

    [Serializable]
    public class Body
    {
        public string mimeType;
        public List<Param> @params;
        public Data data;
    }

    [Serializable]
    public class Header
    {
        public string id;
        public string name;
        public string value;
        public string description;
    }


    [Serializable]
    public class Authentication
    {
        public string type;
        public string token;
        public string prefix;
    }


    [Serializable]
    public class Resource
    {
        public string _id;
        public string parentId;
        public object modified;
        public object created;
        public string url;
        public string name;
        public string description;
        public string method;
        public Body body;
        public List<object> parameters;
        public List<Header> headers;
        public Authentication authentication;
        public double metaSortKey;
        public bool isPrivate;
        public bool settingStoreCookies;
        public bool settingSendCookies;
        public bool settingDisableRenderRequestBody;
        public bool settingEncodeUrl;
        public bool settingRebuildPath;
        public string settingFollowRedirects;
        public string _type;
        public object environmentPropertyOrder;
        public string scope;
        public object dataPropertyOrder;
        public object color;
        public string fileName;
        public string contents;
        public string contentType;
    }

    Root root;

    void Start()
    {
        if(!System.IO.File.Exists(Application.persistentDataPath + "/api" + PhotonNetwork.LocalPlayer.NickName + ".json"))
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/api" + PhotonNetwork.LocalPlayer.NickName + ".json", text.text);
        }
        processData(System.IO.File.ReadAllText(Application.persistentDataPath + "/api" + PhotonNetwork.LocalPlayer.NickName + ".json"));
        //StartCoroutine(getData());
    }

    void processData(string url)
    {
        root = JsonUtility.FromJson<Root>(url);
        gamesWon = root.resources[1].body.data.pool_games_won;
        GetComponent<gameManager>().updateGamesWon(gamesWon);
    }

    public void updateGamesWon()
    {
        root.resources[1].body.data.pool_games_won++;
        gamesWon++;
        string updatedJson = JsonUtility.ToJson(root);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/api" + PhotonNetwork.LocalPlayer.NickName + ".json", updatedJson);
        processData(System.IO.File.ReadAllText(Application.persistentDataPath + "/api" + PhotonNetwork.LocalPlayer.NickName + ".json"));
        
    }
}
