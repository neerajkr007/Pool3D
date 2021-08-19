using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

    public class Param
    {
        public string description { get; set; }
        public bool disabled { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string fileName { get; set; }
        public string type { get; set; }
    }

    public class Body
    {
        public string mimeType { get; set; }
        public List<Param> @params { get; set; }
        public string text { get; set; }
    }

    public class Header
    {
        public string id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string description { get; set; }
    }

    public class Authentication
    {
        public string type { get; set; }
        public string token { get; set; }
        public string prefix { get; set; }
    }
    public class Cooky
    {
        public string key { get; set; }
        public string value { get; set; }
        public string domain { get; set; }
        public string path { get; set; }
        public bool httpOnly { get; set; }
        public bool hostOnly { get; set; }
        public string creation { get; set; }
        public string lastAccessed { get; set; }
        public string id { get; set; }
        public string? expires { get; set; }
        public bool? secure { get; set; }
        public List<string> extensions { get; set; }
        public int? maxAge { get; set; }
    }

    public class Resource
    {
        public string _id { get; set; }
        public string parentId { get; set; }
        public object modified { get; set; }
        public object created { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string method { get; set; }
        public Body body { get; set; }
        public List<object> parameters { get; set; }
        public List<Header> headers { get; set; }
        public Authentication authentication { get; set; }
        public double metaSortKey { get; set; }
        public bool isPrivate { get; set; }
        public bool settingStoreCookies { get; set; }
        public bool settingSendCookies { get; set; }
        public bool settingDisableRenderRequestBody { get; set; }
        public bool settingEncodeUrl { get; set; }
        public bool settingRebuildPath { get; set; }
        public string settingFollowRedirects { get; set; }
        public string _type { get; set; }
        public object environmentPropertyOrder { get; set; }
        public string scope { get; set; }
        public object dataPropertyOrder { get; set; }
        public object color { get; set; }
        public List<Cooky> cookies { get; set; }
        public string fileName { get; set; }
        public string contents { get; set; }
        public string contentType { get; set; }
    }

    public class Root
    {
        public string _type { get; set; }
        public int __export_format { get; set; }
        public string __export_date { get; set; }
        public string __export_source { get; set; }
        public List<Resource> resources { get; set; }
    }

    public class api: MonoBehaviour
    {
        public string file = "C:/UNITY/poolGame/Assets/api/InsomniaExport_PoolGame.json";
        public static api read()
        {
            string jsonData = File.ReadAllText(file);
            print(jsonData);
        }
    }


