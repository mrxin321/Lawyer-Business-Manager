using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

[System.Serializable]
public class Injection{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class TestLuaView: MonoBehaviour
{
    public TextAsset luaScript;
    public Injection[] injections;

    internal static LuaEnv luaEnv = new LuaEnv();
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;

    // public string abcd = "123";

    void Awake(){
        Debug.Log("进入到Awake");


        scriptEnv = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);

        this.LuaTestUse();

        foreach(var injection in injections){
            scriptEnv.Set(injection.name, injection.value);
        }

        Debug.Log("luaScript.text:===========" + luaScript.text);
        luaEnv.DoString(luaScript.text, "TestLuaView", scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);

        if(luaAwake != null){
            luaAwake();
        }

    }

    void Start(){
        if(luaStart != null){
            luaStart();
        }
    }

    void Update(){
        if(luaUpdate != null){
            luaUpdate();
        }

        if(Time.time - TestLuaView.lastGCTime > GCInterval){
            luaEnv.Tick();
            TestLuaView.lastGCTime = Time.time;
        }
    }

    void OnDestroy(){
        if(luaOnDestroy != null){
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        if(scriptEnv != null){
            scriptEnv.Dispose();
        }
        injections = null;
    }

    public void LuaTestUse(){
        Debug.Log("luaTestUse....................");
    }

    public static void StaticLuaTestUse(){
        Debug.Log("StaticLuaTestUse....................");
    }
    
}

