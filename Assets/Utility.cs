using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
﻿using System.Reflection;
﻿using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
﻿using UnityEngine;
using UnityEngine.UI;

public partial class Utility : Singleton<Utility>
{
    #region Fields

    #region Public

    public static YieldInstruction WaitForEndOfFrame = new WaitForEndOfFrame();
    public static YieldInstruction WaitForFixedUpdate = new WaitForFixedUpdate();

    public class ActionWithName
    {
        public Action<object> TheAction;
        public string TheName;
        public object Data;

        public ActionWithName(Action<object> action, string name,object data)
        {
            TheAction = action;
            TheName = name;
            Data = data;
        }
    }

    public static Action ServerTimeReadyEvent;

    #endregion

    #region Private

    private static double _deltaTimeBetweenLocalAndServer;

    private static readonly List<KeyValuePair<bool, object>?> BoolObjectKeyValueList =
        new List<KeyValuePair<bool, object>?>();

    private static readonly List<KeyValuePair<int, ActionWithName>> IndexActionKeyValueList =
        new List<KeyValuePair<int, ActionWithName>>();

    private static readonly Dictionary<Type, object> TypeStringDictionarys = new Dictionary<Type, object>();

    private static readonly Dictionary<Type, object> StringTypeDictionarys = new Dictionary<Type, object>();

    private static readonly object Lock = new object();
    static Queue<ActionWithName> actionWithNameQueue = new Queue<ActionWithName> ();
    #endregion

    #endregion

    #region Properties

    public static bool ServerTimeReady { get; protected set; }

    private string _gameVersion = "";

    public string GameVersion
    {
        get
        {
            if (string.IsNullOrEmpty(_gameVersion))
            {
                Debug.Log("_gameVersion is empty!");
            }
            return _gameVersion;
        }
    }

    #endregion

    #region Methods

    protected override void Awake()
    {
        base.Awake();
        _gameVersion = Application.version;
    }

    #region Public

    public static void SetValueAndSendChangeEvent<T>(T value, ref T field, Action<T> valueChangeAction)
    {
        if (value != null)
        {
            if (value.Equals(field))
            {
                return;
            }
            field = value;
            SafePostEvent(valueChangeAction, field);
            return;
        }
        if (field == null)
        {
            return;
        }
        field = default(T);
        SafePostEvent(valueChangeAction, field);
    }

    public static bool IsOneOf<T>(T value, params T[] values)
    {
        return values.Contains(value);
    }

    public static bool TryResetField<T>(ref T field, T value) where T : class
    {
        if (field != value)
        {
            return false;
        }
        field = null;
        return true;
    }

    public static void SetValueAndSendChangeEvent<T>(T value, ref T field, Action valueChangeAction)
    {
        SetValueAndSendChangeEvent(value, ref field, obj => SafePostEvent(valueChangeAction));
    }

    public static void ChangeTimeScale(float currentTimeScale, float timeScaleFactor)
    {
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = timeScaleFactor * currentTimeScale;
    }

    public static void ChangeTimeScaleLerp(float currentTimeScale, float timeScaleFactor)
    {
        UtilityStartCoroutine(ChangeTimeScaleLerpCoroutine(currentTimeScale, timeScaleFactor));
    }

    public static IEnumerator ChangeTimeScaleLerpCoroutine(float currentTimeScale, float timeScaleFactor)
    {
        while (Mathf.Abs(Time.timeScale - currentTimeScale) >= 0.001f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, currentTimeScale, Time.fixedUnscaledDeltaTime);
            Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, currentTimeScale * timeScaleFactor, Time.fixedUnscaledDeltaTime);
        }
        yield return null;
    }

   
    /// <summary>
    /// milliSeconds
    /// </summary>
    /// <returns></returns>
    public static long GetServerTime()
    {
        if (!ServerTimeReady)
        {
            throw new InvalidOperationException("Server Time is not ready!");
        }
        return (long)(GetTimeStamp(System.DateTime.Now) + _deltaTimeBetweenLocalAndServer);
    }

    public static long GetServerTimeUTC()
    {
        if (!ServerTimeReady)
        {
            throw new InvalidOperationException("Server Time is not ready!");
        }
        //   return (long)(GetTimeStamp(System.DateTime.Now) + _deltaTimeBetweenLocalAndServer);
        return (long)(GetTimeStamp(System.DateTime.UtcNow) + _deltaTimeBetweenLocalAndServer);
    }

    

    public static string FormatTimeToMinFromSeconds(int seconds)
    {
        string sec;
        var mini = "00";
        if (seconds >= 60)
        {
            var mintime = (seconds / 60);
            if (mintime > 9)
            {
                mini = mintime.ToString();
            }
            else
            {
                mini = "0" + mintime;
            }
        }
        var second = seconds % 60;
        if (second > 9)
        {
            sec = second.ToString();
        }
        else
        {
            sec = "0" + second;
        }
        return mini + ":" + sec;
    }

    
    public static T RandomEnum<T>()
    {
        var values = (T[])Enum.GetValues(typeof(T));
        return values[UnityEngine.Random.Range(0, values.Length)];
    }

    public static void ProcessEnums<T>(Action<T> actionToEnumItem)
    {
        foreach (T item in (T[]) Enum.GetValues(typeof(T)))
        {
            actionToEnumItem(item);
        }
    }

    public static T1 GetKeyFromValue<T1, T2>(Dictionary<T1, T2> dictionary, T2 v)
    {
        foreach (var pair in dictionary.Where(pair => pair.Value.Equals(v)))
        {
            return pair.Key;
        }
        return default(T1);
    }

    public static Dictionary<string, T> GetStringEnumDictionary<T>()
    {
        var dictionary = new Dictionary<string, T>();
        ProcessEnums<T>(type =>
            {
                dictionary.Add(type.ToString().ToLower(), type);
            });
        return dictionary;
    }

    public static Dictionary<T, string> GetEnumStringDictionary<T>()
    {
        var dictionary = new Dictionary<T, string>();
        ProcessEnums<T>(type =>
            {
                dictionary.Add(type, type.ToString().ToLower());
            });
        return dictionary;
    }

    public static string GetJsonKeyFromType<T>(T type)
    {
        var t = typeof(T);
        if (!TypeStringDictionarys.ContainsKey(t))
        {
            TypeStringDictionarys.Add(t, GetEnumStringDictionary<T>());
        }
        var dictionary = (Dictionary<T, string>)TypeStringDictionarys[t];
        return dictionary.ContainsKey(type) ? dictionary[type] : null;
    }

    public static T GetTypeFromJsonKey<T>(string jsonKey)
    {
        var t = typeof(T);
        if (!StringTypeDictionarys.ContainsKey(t))
        {
            StringTypeDictionarys.Add(t, GetStringEnumDictionary<T>());
        }
        var dictionary = (Dictionary<string, T>)StringTypeDictionarys[t];
        return dictionary.ContainsKey(jsonKey) ? dictionary[jsonKey] : default(T);
    }

    public static T GetBest<T>(T[] datas, Func<T, T, bool> compareFunc)
    {
        var best = datas[0];
        for (var i = 1; i < datas.Length; ++i)
        {
            if (compareFunc(best, datas[i]))
            {
                best = datas[i];
            }
        }
        return best;
    }

    public static IEnumerable<string> GetSubStrings(string input, string start, string end)
    {
        var r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
        var matches = r.Matches(input);
        return from Match match in matches
                        select match.Groups[1].Value;
    }

    public static void SafePostEvent(Action action)
    {
        if (action == null)
        {
            return;
        }
        action();
    }

    public static void SafePostEvent<T>(Action<T> action, T data)
    {
        if (action == null)
        {
            return;
        }
        action(data);
    }

    public static void SafePostEvent<T0, T1>(Action<T0, T1> action, T0 data0, T1 data1)
    {
        if (action == null)
        {
            return;
        }
        action(data0, data1);
    }

    public static void SafePostEvent<T0, T1, T2>(Action<T0, T1, T2> action, T0 data0, T1 data1, T2 data2)
    {
        if (action == null)
        {
            return;
        }
        action(data0, data1, data2);
    }

    public static void SafePostEvent<T0, T1, T2,T3>(Action<T0, T1, T2,T3> action, T0 data0, T1 data1, T2 data2, T3 data3)
    {
        if (action == null)
        {
            return;
        }
        action(data0, data1, data2, data3);
    }

    public static void SafePostEvent<T0, T1, T2, T3,T4>(Action<T0, T1, T2, T3, T4> action, T0 data0, T1 data1, T2 data2, T3 data3,T4 data4)
    {
        if (action == null)
        {
            return;
        }

        action(data0, data1, data2, data3, data4);
    }

    public static bool IsAnimatorAtState(Animator animator, string stateName, int layerIndex = 0)
    {
        return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
    }

    public static bool IsAllAnimatorsAtStates(params object[] animatorStatePairs)
    {
        var count = animatorStatePairs.Length / 2;
        for (var i = 0; i != count; ++i)
        {
            if (!IsAnimatorAtState((Animator)animatorStatePairs[2 * i], (string)animatorStatePairs[2 * i + 1]))
            {
                return false;
            }
        }
        return true;
    }

    public static void DestroyAll(List<Transform> transforms, Action<Transform> destroyMethod = null)
    {
        foreach (var trans in transforms)
        {
            Destroy(trans, destroyMethod);
        }
    }

    public static void Destroy(Transform trans, Action<Transform> destroyMethod)
    {
        if (destroyMethod != null)
        {
            destroyMethod(trans);
        }
        else if (Application.isPlaying)
        {
            Destroy(trans.gameObject);
        }
        else
        {
            DestroyImmediate(trans.gameObject);
        }
    }

    public static void DestroyAllChildren(Transform parent, Action<Transform> destroyMethod = null)
    {
        var children = parent.Cast<Transform>().ToList();
        /*foreach (var child in children)
        {
            child.SetParent(null);
        }*/
        DestroyAll(children, destroyMethod);
    }

    public static List<T> GetAllChildren<T>(Transform parent, Func<Transform, bool> func) where T : Component
    {
        var children = parent.GetComponentsInChildren<Transform>();
        List<T> tList = null;
        foreach (var child in children.Where(func))
        {
            if (tList == null)
            {
                tList = new List<T>();
            }
            tList.Add(child.GetComponent<T>());
        }
        return tList;
    }       

    public static void DestroyAllInScene()
    {
        foreach (var obj in GetAllSceneRootGameObjects())
        {
            Destroy(obj);
        }
    }

    public static void ActiveGameObjects(params object[] gameObjectActivePairs)
    {
        var count = gameObjectActivePairs.Length / 2;
        for (var i = 0; i != count; ++i)
        {
            ((GameObject)gameObjectActivePairs[2 * i]).SetActive((bool)gameObjectActivePairs[2 * i + 1]);
        }
    }

    public static void ActiveGameObjects(bool active, params GameObject[] gameObjects)
    {
        for (var i = 0; i != gameObjects.Length; ++i)
        {
            gameObjects[i].SetActive(active);
        }
    }

    public static void ActiveGameObjectsFromComponents(bool active, params Component[] components)
    {
        for (var i = 0; i != components.Length; ++i)
        {
            components[i].gameObject.SetActive(active);
        }
    }

    public static T AddChildAsLastSibling<T>(T prefab, Transform parent, Func<T, T> instantiateMethod = null)
        where T : Component
    {
        var instantiate = instantiateMethod ?? Instantiate;
        var child = instantiate(prefab);
        setChildSibling(child.transform, parent);
        return child;
    }

    public static void setChildSibling(Transform childTransform, Transform parent)
    {
        childTransform.SetParent(parent,false);
        //childTransform.localPosition = Vector3.zero;
        //childTransform.localScale = Vector3.one;
        //childTransform.localEulerAngles = Vector3.zero;
        //childTransform.SetAsLastSibling();
    }

    public static GameObject AddChildGameObjectAsLastSibling(GameObject prefab, Transform parent,
                                                             Func<Transform, Transform> instantiateMethod = null)
    {
        return AddChildAsLastSibling(prefab.transform, parent, instantiateMethod).gameObject;
    }

    public static Coroutine UtilityStartCoroutine(IEnumerator routine)
    {
        if (Instance != null)
            return Instance.StartCoroutine(routine);

        return null;
    }

    public static void UtilityStopCoroutine(Coroutine routine)
    {
        if (Instance != null&&routine!=null)
            Instance.StopCoroutine(routine);
    }

    public static void UtilityStopCoroutine(IEnumerator routine)
    {
        if (Instance != null)
            Instance.StopCoroutine(routine);
    }

    public static void DoNextFrame(Action callback)
    {
        if (Instance != null)
            Instance.StartCoroutine(WaitCoroutine(callback, WaitForEndOfFrame));
    }

    private static IEnumerator WaitCoroutine(Action callback, YieldInstruction wait)
    {
        yield return wait;
        callback.Invoke();
    }

    public static void DoWait(Action callback, float time,MonoBehaviour go)
    {
        DoWait(()=>{
            if(go != null)SafePostEvent(callback);
        },new WaitForSeconds(time));
    }

    public static void DoWait(Action callback, YieldInstruction wait)
    {
        if (Instance != null)
            Instance.StartCoroutine(WaitCoroutine(callback, wait));
    }

    private static IEnumerator WaitCoroutine(Action callback, CustomYieldInstruction wait)
    {
        yield return wait;
        callback.Invoke();
    } 

    public static void DoWait(Action callback, CustomYieldInstruction wait)
    {
        if (Instance != null)
            Instance.StartCoroutine(WaitCoroutine(callback, wait));
    }

    public static Coroutine DoUpdateCoroutine(Action callback, WaitForSeconds wait)
    {
        if (Instance != null)
            return Instance.StartCoroutine(DoUpdate(callback ,wait));

        return null;
    }

    private static IEnumerator DoUpdate(Action callback, WaitForSeconds wait)
    {
        while(true) 
        {
            yield return wait;
            callback();
        }
    }

    private static IEnumerator NextFrameCoroutine(Action callback)
    {
        yield return WaitForEndOfFrame;
        callback.Invoke();
    }

    public static void ActionOnLateUpdate(object obj, Action<object> action, string name = null)
    {
        actionWithNameQueue.Enqueue(new ActionWithName(action, name,obj));
//            lock (Lock)
//            {
//                var index = AssignBoolObjectKeyValueListIndex();
//                //Debug.Log("ActionWithData : " + name + " index : " + index);
//                BoolObjectKeyValueList[index] = new KeyValuePair<bool, object>(true, obj);
//                IndexActionKeyValueList.Add(new KeyValuePair<int, ActionWithName>(index, new ActionWithName(action, name)));
//            }
    }

    public static void ShowCanvasGroup(CanvasGroup canvasGroup, bool show)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.interactable = canvasGroup.blocksRaycasts = show;
    }

    public static void ShowCanvasGroup(GameObject canvasGroupGameObject, bool show)
    {
        ShowCanvasGroup(canvasGroupGameObject.GetComponent<CanvasGroup>(), show);
    }

    

    public static void SetRectTransformSize(RectTransform rectTransform, Vector2 size)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    

    public static float GetAngle(Vector2 from, Vector2 to)
    {
        var angle = Vector2.Angle(from, to);
        return to.y > 0.0f ? angle : -angle;
    }

    public static void SetAllMaskableGraphicsMaterialInChildren(GameObject root, Material material)
    {
        var childMaskableGraphics = root.GetComponentsInChildren<MaskableGraphic>(true);
        foreach (var maskableGraphic in childMaskableGraphics)
        {
            maskableGraphic.material = material;
        }
    }

    public static Transform TransferSpritesToImages(Transform root, Transform canvas)
    {
        var imagesRoot = new GameObject().AddComponent<RectTransform>();
        var canvasGroup = imagesRoot.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
        var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>();
        Array.Sort(spriteRenderers,
            (renderer0, renderer1) =>
                renderer0.transform.localPosition.z > renderer1.transform.localPosition.z ? -1 : 1);
        foreach (var spriteRenderer in spriteRenderers)
        {
            var image = new GameObject(spriteRenderer.name).AddComponent<Image>();
            image.sprite = spriteRenderer.sprite;
            image.transform.parent = imagesRoot;
            image.SetNativeSize();
        }
        imagesRoot.parent = canvas;
        imagesRoot.localPosition = Vector3.zero;
        imagesRoot.localScale = Vector3.one;
        return imagesRoot;
    }

    public static void WriteAllBytes(string path, byte[] bytes)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllBytes(path, bytes);
    }

    public static void AppendTextToFile(string path, string contents, Encoding encoding)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.AppendAllText(path, contents, encoding);
    }

    public static string ReadAllTextFromFile(string path)
    {
        return !File.Exists(path) ? string.Empty : File.ReadAllText(path);
    }

    


    public static void SerializeObject<T>(string path, T data)
    {
        StreamWriter writer = null;
        try
        {
            writer = new StreamWriter(path, false);
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, data);
        }
        catch (Exception e)
        {
            print(e.ToString());
        }
        finally
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
    }

    public static T DeserializeObject<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }
        FileStream reader = null;
        T data = null;
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            reader = new FileStream(path, FileMode.Open);
            data = serializer.Deserialize(reader) as T;
        }
        catch (Exception e)
        {
            print(e.ToString());
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
        return data;
    }

    public static byte[] SerializeObject(object pObj)
    {
        if (pObj == null)
        {
            return null;
        }
        var memory = new MemoryStream();
        var formatter = new BinaryFormatter();
        formatter.Serialize(memory, pObj);
        memory.Position = 0;
        var bytes = new byte[memory.Length];
        memory.Read(bytes, 0, bytes.Length);
        memory.Close();
        return bytes;
    }

    public static object DeserializeObject(byte[] pBytes)
    {
        if (pBytes == null)
        {
            return null;
        }
        var memory = new MemoryStream(pBytes) { Position = 0 };
        var formatter = new BinaryFormatter();
        var newOjb = formatter.Deserialize(memory);
        memory.Close();
        return newOjb;
    }

    

    public static void ProcessAllFilesInDirectory(string directoryPath, Action<string> action)
    {
        var directoryInfo = new DirectoryInfo(directoryPath);
        foreach (var file in directoryInfo.GetFiles())
        {
            action(file.FullName);
        }
        foreach (var directory in directoryInfo.GetDirectories())
        {
            ProcessAllFilesInDirectory(directory.FullName, action);
        }
    }

    public static void SetParentAndNormalized(Transform parent, Transform trans)
    {
        trans.SetParent(parent);
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        if(!trans.gameObject.activeSelf)
            trans.gameObject.SetActive(true);
    }

    public static T AddChildAsFirstSibling<T>(T prefab, Transform parent, Func<T, T> instantiateMethod = null)
        where T : Component
    {
        var child = AddChildAsLastSibling(prefab, parent, instantiateMethod);
        child.transform.SetAsFirstSibling();
        return child;
    }

    public static string GetLimitMaxLengthString(string originalString, int maxLength, string postString)
    {
        return originalString.Length > maxLength
            ? originalString.Substring(0, maxLength - postString.Length) + postString
            : originalString;
    }

    

    public static bool IsSubClassOf(Type type, Type baseType)
    {
        var b = type.BaseType;
        while (b != null)
        {
            if (b == baseType)
            {
                return true;
            }
            b = b.BaseType;
        }
        return false;
    }

    public static IEnumerable<Type> GetAllSubClassTypes<T>()
    {
        var subTypeQuery = from t in Assembly.GetExecutingAssembly().GetTypes()
                                    where IsSubClassOf(t, typeof(T))
                                    select t;
        return subTypeQuery;
    }

    #endregion

    #region Protected

    [SerializeField]
    protected double ProfilerThresholdTime = 0.0;

    protected void LateUpdate()
    {
        while (actionWithNameQueue.Count > 0)
        {
            
            var item = actionWithNameQueue.Dequeue();
            SafePostEvent(item.TheAction, item.Data);
        }
        
    }

    protected override void OnDestroy()
    {
        BoolObjectKeyValueList.Clear();
        IndexActionKeyValueList.Clear();
        TypeStringDictionarys.Clear();
        StringTypeDictionarys.Clear();

        Debug.Log("Clean up Utility statics finished.");

        base.OnDestroy();
    }

    public static void ApplicationQuit()
    {
//            CommonUtils.AddBILog("Loading", "ActionType:ApplicationOut", true, false);
        PlayerPrefs.Save();
//            CommonUtils.SendUserLog();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    

    protected static int AssignBoolObjectKeyValueListIndex()
    {
        var data = new KeyValuePair<bool, object>(false, null);
        for (var i = 0; i != BoolObjectKeyValueList.Count; ++i)
        {
            if (BoolObjectKeyValueList[i] != null)
            {
                continue;
            }
            BoolObjectKeyValueList[i] = data;
            return i;
        }
        BoolObjectKeyValueList.Add(data);
        return BoolObjectKeyValueList.Count - 1;
    }

    protected static void ActionWithData(int boolJsonObjectIndex, Action<object> action)
    {
        var jsonObjectKeyValue = BoolObjectKeyValueList[boolJsonObjectIndex];
        if (jsonObjectKeyValue == null)
        {
            Debug.LogError("ActionWithData  NullReferenceException : " + boolJsonObjectIndex);
            throw new NullReferenceException();
        }
        
        SafePostEvent(action, jsonObjectKeyValue.Value.Value);
        BoolObjectKeyValueList[boolJsonObjectIndex] = null;
    }
    protected static void ActionWithData(ActionWithName action)
    {
       
        SafePostEvent(action.TheAction, action.Data);
        
    }
    #endregion

    #endregion
}

public partial class Utility
{

    /// <summary>
    /// the num should be less than 20
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string RomanNumberals(int num)
    {
        const string numerals = "0,Ⅰ,Ⅱ,Ⅲ,Ⅳ,Ⅴ,Ⅵ,Ⅶ,Ⅷ,Ⅸ,Ⅹ,Ⅺ,Ⅻ,XIII,XIV,XV,XVI,XVII,XVIII,XIX,XX";
        var arrs = numerals.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        return arrs.Length > num ? arrs[num] : string.Empty;
    }

    public static Vector3? GetRelatedPosition(Transform child, Transform parent)
    {   
        if (child == parent)
        {
            return Vector3.zero;
        }
        var transform = child.transform;
        var relatedPosition = transform.localPosition;
        while (transform != parent)
        {
            transform = transform.parent;
            if (transform == null)
            {
                return null;
            }
            relatedPosition += transform.localPosition;
        }
        return relatedPosition;
    }

    public static List<T0> AddChild<T0, T1>(List<T0> returnList, List<T1> compareList, T0 prefab, Transform parent,
                                            Func<T0, Transform, Func<T0, T0>, T0> addFunc, Func<T0, T0> instantiateMethod = null) where T0 : Component
    {
        return AddChild(returnList, compareList.Count, prefab, parent, addFunc, instantiateMethod);
    }
    public static Transform FindChild(Transform trans, string goName)
    {
        Transform child = trans.Find(goName);
        if (child != null)
            return child;

        Transform go = null;
        for (int i = 0; i < trans.childCount; i++)
        {
            child = trans.GetChild(i);
            go = FindChild(child, goName);
            if (go != null)
                return go;
        }
        return null;
    }
    public static List<T0> AddChild<T0>(List<T0> returnList, int number, T0 prefab, Transform parent,
                                        Func<T0, Transform, Func<T0, T0>, T0> addFunc, Func<T0, T0> instantiateMethod = null) where T0 : Component
    {
        while (returnList.Count < number)
        {
            returnList.Add(addFunc(prefab, parent, instantiateMethod));
        }
        return returnList;
    }

    public static void RefreshComponentList<T0, T1>(List<T0> componentList, List<T1> dataList, Action<T0, T1> action)
        where T0 : Component
    {
        RefreshComponentList(componentList, dataList.Count, (component, index) => action(component, dataList[index]));
    }

    public static void RefreshComponentList<T0>(List<T0> componentList, int number, Action<T0, int> action)
        where T0 : Component
    {
        var count = componentList.Count;
        for (var i = 0; i != count; ++i)
        {
            var component = componentList[i];
            var show = i < number;
            component.gameObject.SetActive(show);
            if (!show)
            {
                continue;
            }
            action(component, i);
        }
    }

    

    public void OnSetAlphaUpdate(float f)
    {
        var renderList = GetAllChildren<Renderer>(transform, transform1 => transform1.GetComponent<Renderer>() != null);
        if (renderList == null || renderList.Count == 0)
        {
            return;
        }
        foreach (var ren in renderList.Where(ren => ren.material.HasProperty("_TintAlpha")))
        {
            ren.material.SetFloat("_TintAlpha", 1 - f);
        }
    }

    public static void ChangePrefabInResources(ref GameObject gameObject, Transform parent, string originalPath,
                                               string path)
    {
        if (originalPath == path)
        {
            return;
        }
        ClearPrefabAssets(gameObject);
        gameObject = Instantiate(Resources.Load<GameObject>(path));
        var gameObjectTransform = gameObject.transform;
        gameObjectTransform.SetParent(parent);
        gameObjectTransform.localPosition = Vector3.zero;
        gameObjectTransform.localScale = Vector3.one;
    }

    public static void ClearPrefabAssets(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }
        Destroy(gameObject);
    }

    private static bool StartUnloadUnusedAssets = false;
    public static bool ShouldStartUnloadUnusedAssets = false;

    protected void Update()
    {
        if (!StartUnloadUnusedAssets && ShouldStartUnloadUnusedAssets)
        {
            StartUnloadUnusedAssets = true;
            StartCoroutine(RegularUnloadUnusedAssetsCoroutine());
        }
        TotalTick -= Time.deltaTime ;
        if(TotalTick <= 0)
        {
            DoSchedule();
            TotalTick = TargetDuration;
        }
    } 

    /*
        定时器 用于数据刷新 start
    */
    private class Schedule{
        public Action Function;
        public float TargetTick;
        public float Tick;
        // bool Work;
    }
    private static float TargetDuration = 0.5f;
    private static float TotalTick = TargetDuration;//0.5秒执行一次
    private static Dictionary<Action,Schedule> ScheduleList = new Dictionary<Action,Schedule>();
    public static void DoSchedule()
    {
        Schedule schedule;
        foreach(var item in ScheduleList)
        {
            schedule = item.Value;
            schedule.Tick -=TargetDuration;
            if(schedule.Tick <= 0)
            {
                SafePostEvent(item.Value.Function);
                schedule.Tick = schedule.TargetTick;
            }    
        }
    }
    public static void RegisterSchedule(Action function,float tiemTick)
    {
        var schedule = new Schedule(); 
        schedule.Function = function;
        schedule.Tick = tiemTick; 
        schedule.TargetTick = tiemTick;

        if(!ScheduleList.ContainsKey(function))
            ScheduleList.Add(function,schedule);
    }
    //如果在定时器中关闭定时器 需要放到LateUpdate 传参true即可
    public static void UnRegisterSchedule(Action function,bool inAction = false)
    {
        if(inAction)
        {
            ActionOnLateUpdate(null,(data)=>{
                if(ScheduleList.ContainsKey(function))
                {
                    ScheduleList.Remove(function);
                }
            });
            return;
        }
        if(ScheduleList.ContainsKey(function))
        {
            ScheduleList.Remove(function);
        }
        
    }
    /*
        定时器 用于数据刷新 end
    */

    [SerializeField]
    protected float RegularUnloadUnusedAssetsCoroutineInterval = 300;

    private IEnumerator RegularUnloadUnusedAssetsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(RegularUnloadUnusedAssetsCoroutineInterval);
            Resources.UnloadUnusedAssets();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    public static string StringFormat<T>(string pattern, T[] paramArray)
    {
        var param = new object[paramArray.Length];
        for (var i = 0; i != param.Length; ++i)
        {
            param[i] = paramArray[i];
        }
        return string.Format(pattern, param);
    }

    public static Vector3 GetCanvasPosition(RectTransform rectTransform)
    {
        var canvasTransform = rectTransform.transform;
        if (canvasTransform.GetComponent<Canvas>() != null)
        {
            return Vector3.zero;
        }
        var position = canvasTransform.localPosition;
        while (true)
        {
            canvasTransform = canvasTransform.parent;
            if (canvasTransform.GetComponent<Canvas>() == null)
            {
                position += canvasTransform.localPosition;
            }
            else
            {
                break;
            }
        }
        return position;
    }
    
    public static Vector3 GetToCanvasScale(RectTransform rectTransform)
    {
        var canvasTransform = rectTransform.transform;
        if (canvasTransform.GetComponent<Canvas>() != null)
        {
            return Vector3.one;
        }
        var scale = canvasTransform.localScale;
        while (true)
        {
            canvasTransform = canvasTransform.parent;
            if (canvasTransform.GetComponent<Canvas>() == null)
            {
                var cs = canvasTransform.localScale;
                scale = new Vector3(cs.x * scale.x, cs.y * scale.y,
                    cs.z * scale.z); //canvasTransform.localPosition;
            }
            else
            {
                break;
            }
        }
        return scale;
    }
    
    public static Vector3 GetLocalPosFromWorldPos(RectTransform rectTransform, Vector3 worldPos, UnityEngine.Camera camera)
    {
        var parentRectList = new Dictionary<int, Transform>();
        var canvasTransform = rectTransform.transform;
        int index = 0;
        while (true)
        {
            if (canvasTransform.parent == null)
            {
                break;
            }
            canvasTransform = canvasTransform.parent;
            parentRectList.Add(index, canvasTransform);
            index++;
        }
        var position = camera.WorldToScreenPoint(worldPos);
        for (int i = 0; i < parentRectList.Count; i++)
        {
            position -= parentRectList[i].localPosition;
        }
        return position;
    }

    public static void GetCanvasPosFromCorner(RectTransform rectTransform, out Vector3 leftBottom, out Vector3 rightTop)
    {
        var canvasTransform = rectTransform.transform;
        leftBottom = rectTransform.localPosition + new Vector3(rectTransform.rect.min.x, rectTransform.rect.min.y);
        rightTop = rectTransform.localPosition + new Vector3(rectTransform.rect.max.x, rectTransform.rect.max.y);
        if (canvasTransform.GetComponent<Canvas>() != null)
        {
            return;
        }
        while (true)
        {
            canvasTransform = canvasTransform.parent;
            if (canvasTransform.GetComponent<Canvas>() == null)
            {
                leftBottom += canvasTransform.localPosition;
                rightTop += canvasTransform.localPosition;
            }
            else
            {
                break;
            }
        }
    }

    public static Vector2 GetScreenPosition(RectTransform rectTransform, UnityEngine.Camera cam = null)
    {
        if (cam == null)
        {
            cam = FindObjectOfType<UnityEngine.Camera>();
        }
        var canvasPosition = GetCanvasPosition(rectTransform);
        var sizeDelta = rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        var xRate = canvasPosition.x / sizeDelta.x;
        var yRate = canvasPosition.y / sizeDelta.y;
        return new Vector2((xRate + 0.5f) * cam.pixelWidth, (yRate + 0.5f) * cam.pixelHeight);
    }

    public static void GetScreenPositionFromCorner(RectTransform rectTransform, out Vector3 leftBottom, out Vector3 rightTop, UnityEngine.Camera cam = null)
    {
        if (cam == null)
        {
            cam = FindObjectOfType<UnityEngine.Camera>();
        }
        GetCanvasPosFromCorner(rectTransform, out leftBottom, out rightTop);
        var sizeDelta = rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        var xRateleft = leftBottom.x / sizeDelta.x;
        var yRateleft = leftBottom.y / sizeDelta.y;
        var xRateright = rightTop.x / sizeDelta.x;
        var yRateright = rightTop.y / sizeDelta.y;

        leftBottom = new Vector2((xRateleft + 0.5f) * cam.pixelWidth, (yRateleft + 0.5f) * cam.pixelHeight);
        rightTop = new Vector2((xRateright + 0.5f) * cam.pixelWidth, (yRateright + 0.5f) * cam.pixelHeight);
    }

    public static Vector2 GetSrceenPoint(RectTransform rectTransform, Vector3 localPos, UnityEngine.Camera cam = null)
    {
        if (cam == null)
        {
            cam = FindObjectOfType<UnityEngine.Camera>();
        }
        var canvasTransform = rectTransform.transform;
        if (canvasTransform.GetComponent<Canvas>() != null)
        {
            var sizeDelta1 = rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            var xRate1 = localPos.x / sizeDelta1.x;
            var yRate1 = localPos.y / sizeDelta1.y;
            return new Vector2((xRate1 + 0.5f) * cam.pixelWidth, (yRate1 + 0.5f) * cam.pixelHeight);
        }
        while (true)
        {
            canvasTransform = canvasTransform.parent;
            if (canvasTransform.GetComponent<Canvas>() == null)
            {
                localPos += canvasTransform.localPosition;
            }
            else
            {
                break;
            }
        }
        var sizeDelta = rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        var xRate = localPos.x / sizeDelta.x;
        var yRate = localPos.y / sizeDelta.y;
        return new Vector2((xRate + 0.5f) * cam.pixelWidth, (yRate + 0.5f) * cam.pixelHeight);
    }

    //针对3D的UI,通过屏幕坐标获取点击位置对于目标ui的相对坐标
    public static Vector3 GetLocalPosFromScreenPos(RectTransform canvasRect, RectTransform targetTransform, Vector3 screenPos)
    {
        var canvasLocalPos = GetCanvasLocalPosFromScreenPos(canvasRect, screenPos);
        var rectList = GetRectParentsList(canvasRect, targetTransform);
        var rectArr = rectList.ToArray();
        var templocalPos = Vector3.zero;
        templocalPos = canvasLocalPos;
        for (int i = rectArr.Length - 1; i >= 0; i--)
        {
            templocalPos -= rectArr[i].localPosition;
            templocalPos = new Vector3(templocalPos.x / rectArr[i].localScale.x, templocalPos.y / rectArr[i].localScale.y, templocalPos.z / rectArr[i].localScale.z);
        }
        return templocalPos;
    }

    public static List<RectTransform> GetRectParentsList(RectTransform canvasRect, RectTransform targetRect)
    {
        int i = 0;
        List<RectTransform> parentsList = new List<RectTransform>();
        RectTransform rect = targetRect;
        parentsList.Add(targetRect);
        while (true)
        {
            i++;
            if (i >= 10)
            {
                break;
            }
            if (rect.parent.GetComponent<RectTransform>() == canvasRect)
            {
                break;
            }
            else
            {
                var tempRect = rect.parent.GetComponent<RectTransform>();
                parentsList.Add(tempRect);
                rect = tempRect;
            }
        }
        return parentsList;
    }

    public static Vector3 GetCanvasLocalPosFromScreenPos(RectTransform canvasRect, Vector3 screenPos)
    {
        var screenW = Screen.width;
        var screenH = Screen.height;
        var canvasW = canvasRect.sizeDelta.x;
        var canvasH = canvasRect.sizeDelta.y;
        var x = screenPos.x / screenW * canvasW - canvasW / 2f;
        var y = screenPos.y / screenH * canvasH - canvasH / 2f;
        return new Vector3(x, y, 0);
    }

    public static void SendMessage(Func<string, GameObject> getGameObjectFunc, string gameObjectName, string message)
    {
        if (getGameObjectFunc == null || string.IsNullOrEmpty(gameObjectName) ||
            string.IsNullOrEmpty(message))
        {
            return;
        }
        var gO = getGameObjectFunc(gameObjectName);
        gO.SetActive(true);
        gO.SendMessage(message);
    }

    public static Vector3 GetCanvasUiScreenPos(RectTransform canvasRect, RectTransform uiRectTransform)
    {
        var rectList = GetRectParentsList(canvasRect, uiRectTransform);
        var rectArr = rectList.ToArray();
        var templocalPos = uiRectTransform.transform.localPosition;
        for (int i = rectArr.Length - 1; i >= 0; i--)
        {
            templocalPos += rectArr[i].localPosition;
            templocalPos = new Vector3(templocalPos.x / rectArr[i].localScale.x, templocalPos.y / rectArr[i].localScale.y, templocalPos.z / rectArr[i].localScale.z);
        }
        return GetCanvasScreenPosFromLocalPos(canvasRect, templocalPos);
    }

    public static Vector3 GetCanvasScreenPosFromLocalPos(RectTransform canvasRect, Vector3 localPos)
    {
        var screenW = Screen.width;
        var screenH = Screen.height;
        var canvasW = canvasRect.sizeDelta.x;
        var canvasH = canvasRect.sizeDelta.y;
        var x = (localPos.x + canvasW / 2f) / canvasW * screenW;
        var y = (localPos.y + canvasH / 2f) / canvasH * screenH;
        return new Vector3(x, y, 0);
    }

    public static List<T> ColliderWithRect<T>(Vector3 mPos, Vector2 dir, float width, float height,
                                              List<T> inputPoints) where T : Component
    {
        List<T> resultList = null;
        var dirTemp = new Vector2();
        foreach (var elem in inputPoints.Where(elem => elem != null))
        {
            dirTemp.x = elem.transform.position.x - mPos.x;
            dirTemp.y = elem.transform.position.z - mPos.z;
            dirTemp.Normalize();
            if (Vector2.Dot(dirTemp, dir) < 0)
            {
                continue;
            }
            var distance = Vector3.Distance(elem.transform.position, mPos);
            if (distance > height)
            {
                continue;
            }

            if (resultList == null)
            {
                resultList = new List<T>();
            }
            resultList.Add(elem);
        }
        return resultList;
    }

    public static List<T> ColliderWithSector<T>(Vector3 mPos, Vector2 dir, float radias, List<T> inputPoints,
                                                float range) where T : Component
    {
        List<T> resultList = null;
        var tempVec2 = new Vector2();
        foreach (var elem in inputPoints)
        {
            var position = elem.transform.position;
            var distance = Vector3.Distance(position, mPos);
            if (distance > radias)
            {
                continue;
            }
            tempVec2.x = position.x - mPos.x;
            tempVec2.y = position.z - mPos.z;
            var dotV = Mathf.Acos(Vector2.Dot(dir, tempVec2));
            if (!(dotV > range))
            {
                continue;
            }
            if (resultList == null)
            {
                resultList = new List<T>();
            }
            resultList.Add(elem);
        }
        return resultList;
    }

    public static List<T> ColliderWithRadius<T>(Vector3 mPos, float radias, List<T> inputPoints) where T : Component
    {
        List<T> resultList = null;
        foreach (var elem in inputPoints)
        {
            var position = elem.transform.position;
            var distance = Vector2.Distance(new Vector2(position.x, position.z), new Vector2(mPos.x, mPos.z));
            if (distance > radias)
            {
                continue;
            }

            if (resultList == null)
            {
                resultList = new List<T>();
            }
            resultList.Add(elem);
        }
        return resultList;
    }

   

    public static List<Vector3> ComputerPointListStraightLine(Vector3 startPoint, Vector3 endPoint, float stepDelta)
    {
        float distance = Vector3.Distance(startPoint, endPoint);
        int totalPoint = (int)(distance / stepDelta);

        List<Vector3> retList = new List<Vector3>();

        Vector3 deltaVec = (endPoint - startPoint) / totalPoint;

        for (int i = 0; i != totalPoint; ++i)
        {
            Vector3 newPoint = startPoint + deltaVec * i;
            retList.Add(newPoint);
        }
        retList.Add(endPoint);
        return retList;
    }

   

    private static long GetLeftTime(long time, long dayTime)
    {
        //            Debug.LogError("time===================" + time+"-------------------dayTime===="+dayTime);
        if (time >= dayTime)
        {
            var temptime = time - dayTime;
            return GetLeftTime(temptime, dayTime);
        }

        return time;
    }

    public static List<Vector3> ComputerPointListParabola(Vector3 startPoint, Vector3 endPoint,
                                                          float height, float stepDelta)
    {
        var distance = Vector3.Distance(startPoint, endPoint);
        var totalPoint = (int)(distance / stepDelta);

        var retList = new List<Vector3>();

        var midPoint = 0.5f * (startPoint + endPoint);

        var tempParam = Mathf.Pow(startPoint.x - midPoint.x, 2.0f) + Mathf.Pow(startPoint.z - midPoint.z, 2.0f);
        var paramA = -height / tempParam;

        var deltaVec = new Vector2(endPoint.x - startPoint.x, endPoint.z - startPoint.z);
        deltaVec /= totalPoint;

        for (var i = 0; i != totalPoint; ++i)
        {
            var newPoint = new Vector3
            {
                x = startPoint.x + deltaVec.x * i,
                z = startPoint.z + deltaVec.y * i
            };

            newPoint.y = height + paramA * (Mathf.Pow(newPoint.x - midPoint.x, 2.0f) + Mathf.Pow(newPoint.z - midPoint.z, 2.0f));
            retList.Add(newPoint);
        }
        retList.Add(endPoint);
        return retList;
    }

    public static Vector3 GetWorldPositionFromUIPosition(Vector3 uiPosition, UnityEngine.Camera camera, UnityEngine.Camera uiCamera)
    {
        Vector3 scr = RectTransformUtility.WorldToScreenPoint(uiCamera, uiPosition);
        return camera.ScreenToWorldPoint(scr);
    }

    


    public static int CurGuid = 1;

    public static int GetGlobalUniqeId()
    {
        return CurGuid++;
    }

    public static DateTime ConvertIntDateTime(double d)
    {
        var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return startTime.AddMilliseconds(d);
    }

    /// <summary>
    ///  毫秒
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static long GetTimeStamp(DateTime time)
    {
//            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
//            return (time.Ticks - startTime.Ticks) / 10000;

        TimeSpan span = (time - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
        return (long)span.TotalMilliseconds;
    }

    public static Vector2 ScreenPointToPercentVertorVector2(Vector2 screenPoint)
    {
        var x = screenPoint.x / Screen.width;
        var y = screenPoint.y / Screen.height;
        return new Vector2(x, y);
    }

    public static XmlNodeList SelectNodes(string xmlFilePath, string nodePath)
    {
        var doc = new XmlDocument();
        doc.Load(xmlFilePath);
        var root = doc.DocumentElement;
        return root != null ? root.SelectNodes(nodePath) : null;
    }

    public static string ReadFromFile(string fileFullPath)
    {
        var ret = string.Empty;
        if (!File.Exists(fileFullPath))
            return ret;

        try
        {
            using (var sr = new StreamReader(fileFullPath))
            {
                ret = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch (IOException)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }
        }
        return ret;
    }

    public static void WriteToFile(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (directory == null)
        {
            return;
        }
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        try
        {
            using (var sw = new StreamWriter(filePath, false))
            {
                sw.Write(content);
                sw.Close();
            }
        }
        catch (IOException)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    public static bool DeleteFile(string filePath)
    {
        if (!File.Exists(filePath))
            return false;
        File.Delete(filePath);
        return true;
    }

    

    public static Vector3 GetCameraPosition(UnityEngine.Camera camera, Vector3 targetPosition, Vector2 viewportPosition,
                                            Plane plane)
    {
        var cameraPosition = camera.transform.position;
        var ray = camera.ViewportPointToRay(new Vector3(viewportPosition.x, viewportPosition.y, 0));
        var distance = plane.GetDistanceToPoint(cameraPosition);
        var angle = Vector3.Angle(ray.direction, Vector3.down);
        var cosAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
        var d = distance / cosAngle;
        var point = cameraPosition - ray.direction.normalized * d;
        return cameraPosition + targetPosition - point;
    }

    public static void ActionOnObjects<T>(Action<T> action, IEnumerable<T> objects)
    {
        foreach (var o in objects)
        {
            SafePostEvent(action, o);
        }
    }

    public static void DoSomethingAfterSeconds(float time, Action doSomething)
    {
        UtilityStartCoroutine(DoSomethingAfterSecondsCoroutine(time, doSomething));
    }

    private static IEnumerator DoSomethingAfterSecondsCoroutine(float time, Action doSomething)
    {
        yield return new WaitForSeconds(time);
        SafePostEvent(doSomething);
    }

    public static Dictionary<int, string> CreateHashIntStringDictionary(params string[] names)
    {
        var dic = new Dictionary<int, string>();
        foreach (var n in names)
        {
            dic[Animator.StringToHash(n)] = n;
        }
        return dic;
    }

    public static IEnumerable<T> GetAllSceneComponents<T>() where T : Component
    {
#if UNITY_EDITOR
        return Resources.FindObjectsOfTypeAll<T>().Where(t => t.gameObject.scene.name != null);
#else
        return Resources.FindObjectsOfTypeAll<T>();
#endif
    }

    public static IEnumerable<GameObject> GetAllSceneGameObjects()
    {
#if UNITY_EDITOR
        return GetAllSceneComponents<Transform>().Select(trans => trans.gameObject);
#else
        return Resources.FindObjectsOfTypeAll<GameObject>();
#endif
    }

    public static IEnumerable<GameObject> GetAllSceneRootGameObjects()
    {
        return
            GetAllSceneComponents<Transform>()
                .Where(t => t.parent == null)
                .Select(t => t.gameObject);
    }


    // <summary>
    /// Determines if is point in polygon the specified pX pY polySides polyX polyY.
    /// </summary>
    /// <returns><c>true</c> if is point in polygon the specified pX pY polySides polyX polyY; otherwise, <c>false</c>.</returns>
    /// <param name="pX">P x.</param>
    /// <param name="pY">P y.</param>
    /// <param name="polySides">Poly sides.</param>
    /// <param name="polyX">Poly x.</param>
    /// <param name="polyY">Poly y.</param>/
    public static bool IsPointInPolygon(float pX, float pY, int polySides, float[] polyX, float[] polyY)
    {
        bool oddNodes = false;
        int i = 0, j = polySides - 1;
        for (i = 0; i < polySides; i++)
        {
            if ((polyY[i] < pY && polyY[j] >= pY || polyY[j] < pY && polyY[i] >= pY)
                && (polyX[i] <= pX || polyX[j] <= pX))
            {
                oddNodes ^= (polyX[i] + (pY - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]) < pX);
            } 
            j = i;
        }
        return oddNodes;
    }

    #if UNITY_EDITOR

    public static IEnumerable<GameObject> GetAllPrefabs()
    {
        return from assetPath in UnityEditor.AssetDatabase.GetAllAssetPaths()
                        where assetPath.EndsWith(".prefab")
                        select UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
    }

    public static IEnumerable<GameObject> GetAllPrefabsWithPath(string path)
    {
        return from assetPath in UnityEditor.AssetDatabase.GetAllAssetPaths()
                        where assetPath.EndsWith(".prefab")
                        where assetPath.StartsWith(path)
                        select UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
    }

    public static IEnumerable<string> GetAllAssetsWithSuffix(string suffix)
    {
        return from assetPath in UnityEditor.AssetDatabase.GetAllAssetPaths()
            where assetPath.EndsWith(suffix)
            select assetPath;
    }

    public static void RemoveUnnecessaryAnimatorForPrefabFunc(GameObject gameObject)
    {
        RemoveUnnecessaryComponentForPrefabFunc<Animator>(gameObject,
            animator => animator.runtimeAnimatorController == null);
    }

    public static void RemoveUnnecessaryMeshRendererForPrefabFunc(GameObject gameObject)
    {
        RemoveUnnecessaryComponentForPrefabFunc<MeshRenderer>(gameObject,
            renderer => renderer.GetComponent<MeshFilter>() == null && renderer.GetComponent<TextMesh>() == null);
    }

    public static void RemoveUnnecessaryComponentForPrefabFunc<T>(GameObject gameObject, Func<T, bool> boolFunc) where T : Component
    {
        if (gameObject == null)
        {
            return;
        }
        var components = gameObject.GetComponentsInChildren<T>(true);
        if (components == null || components.Length == 0 || !components.Any(boolFunc))
        {
            return;
        }
        var instance = Instantiate(gameObject);
        foreach (
            var component in
                instance.GetComponentsInChildren<T>(true)
                    .Where(boolFunc))
        {
            DestroyImmediate(component);
        }
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        UnityEditor.PrefabUtility.ReplacePrefab(instance, gameObject,
            UnityEditor.ReplacePrefabOptions.ConnectToPrefab | UnityEditor.ReplacePrefabOptions.ReplaceNameBased);
        DestroyImmediate(instance);
    }

    #endif

   

    public static Vector2 GetTouchPos()
    {
        Vector2 vt = Vector2.zero;
        #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))vt = Input.mousePosition;
        #else
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)vt = Input.GetTouch(0).position;
        #endif
        return vt;
    }
    public static Material GetMaterial(SpriteRenderer render)
    {

#if UNITY_EDITOR
        return render.material;
#else
    return render.sharedMaterial;
#endif
    }
    public static Material GetMaterial(Renderer render)
    {
#if UNITY_EDITOR
        return render.material;
#else
	return render.sharedMaterial;
#endif
    }
    /// <summary>
    /// 保留两位小数，去尾法。小于最小值在返回最小值，
    /// </summary>
    /// <param name="val"></param>
    /// <param name="len">小数位长度</param>
    /// <returns></returns>
    public static string GetFloatDecimal(float val, int len)
    {
        
        var pow = Mathf.Pow(10, len);
        var t = (int) (val *pow );
        if (t == 0&&val>float.Epsilon)
            t = 1;
        return (t / pow).ToString("F"+len);
    }

    /// <summary>
    /// 小数部分四舍五入,保留两位小数
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static float GetMathRoundDecimal(float val)
    {
        int integerNum = (int) val;//获取整数部分
        float decimalNum = (val - integerNum) * 100; //获取小数部分并前进两位，用于四舍五入
        float tempInjureHp = Mathf.Round(decimalNum)/100+integerNum;
        return tempInjureHp;
    }

    public static bool BetweenLineAndCircle(
        Vector2 circleCenter, float circleRadius,
        Vector2 point1, Vector2 point2)
    {
        return BetweenLineAndCircleNumber(circleCenter, circleRadius, point1, point2, out Vector2 p1,
            out Vector2 p2) ;
    }
    public static bool BetweenLineAndCircleNumber(
        Vector2 circleCenter, float circleRadius,
        Vector2 point1, Vector2 point2,
        out Vector2 intersection1, out Vector2 intersection2)
    {
        float t;

        var dx = point2.x - point1.x;
        var dy = point2.y - point1.y;

        var a = dx * dx + dy * dy;
        var b = 2 * (dx * (point1.x - circleCenter.x) + dy * (point1.y - circleCenter.y));
        var c = (point1.x - circleCenter.x) * (point1.x - circleCenter.x) + (point1.y - circleCenter.y) * (point1.y - circleCenter.y) - circleRadius * circleRadius;

        var determinate = b * b - 4 * a * c;
        if ((a <= 0.0000001) || (determinate < -0.0000001))
        {
            // No real solutions.
            intersection1 = Vector2.zero;
            intersection2 = Vector2.zero;
            return false;
        }
        if (determinate < 0.0000001 && determinate > -0.0000001)
        {
            // One solution.
            t = -b / (2 * a);
            intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
            if (!CheckPointInLine(point1, point2, intersection1))
            {
                intersection1 = Vector2.zero;
                intersection2 = Vector2.zero;
                return false;
            }
            intersection2 = Vector2.zero;
            return true;
        }
        
        // Two solutions.
        t = (float)((-b + Mathf.Sqrt(determinate)) / (2 * a));
        var p1=intersection1 = new Vector2(point1.x + t * dx, point1.y + t * dy);
        
        t = (float)((-b - Mathf.Sqrt(determinate)) / (2 * a));
        var p2=intersection2 = new Vector2(point1.x + t * dx, point1.y + t * dy);
        
        if (!CheckPointInLine(point1, point2, intersection1))
        {
            intersection1 = Vector2.zero;
        }
        if (!CheckPointInLine(point1, point2, intersection2))
        {
            intersection2 = Vector2.zero;
        }

        if (intersection1 == Vector2.zero && intersection2 == Vector2.zero)
        {
            if (CheckPointInLine(p1, p2, point1))
            {
                return true;
            }
            return false;
        }
        
        return true;
    }
    private static bool CheckPointInLine(Vector2 p1,Vector2 p2,Vector2 checkPoint)
    {
        var minX = Mathf.Min(p1.x, p2.x);
        var maxX = Mathf.Max(p1.x, p2.x);
        var minY = Mathf.Min(p1.y, p2.y);
        var maxY = Mathf.Max(p1.y, p2.y);
        return (checkPoint.x >= minX && checkPoint.x <= maxX && checkPoint.y >= minY && checkPoint.y <= maxY);
    }
}

public static class UtilityExtension
{
    public static void ChangeLayersRecursively(this Transform trans, string name)
    {
        var layer = LayerMask.NameToLayer(name);
        ChangeLayersByIndexEx(trans,layer);
    }

    public static void ChangeLayersByIndex(this Transform trans,int layer)
    {
        ChangeLayersByIndexEx(trans,layer);
    }

    public static void ChangeLayersByIndexJudge(this Transform trans,int layer)
    {
        if(trans.gameObject.layer != layer)ChangeLayersByIndexEx(trans,layer);
    }

    //降低消耗
    public static void ChangeLayersByIndexEx(this Transform trans,int layerIndex)
    {
        if(layerIndex != trans.gameObject.layer)trans.gameObject.layer = layerIndex;

        Transform[] transArray = trans.GetComponentsInChildren<Transform>(true);
        for(int i = 0;i<transArray.Length;i++)
        {
            if(transArray[i].gameObject.layer != layerIndex)
            {
                transArray[i].gameObject.layer = layerIndex;
            }
        }
    }

    public static T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null && Application.isPlaying)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    public static T GetComponentForce<T>(this GameObject obj) where T : Component 
    {
        T final = null;
        if (obj != null) 
        {
            final = obj.GetComponent<T>();
            if (final == null) 
            {
                final = obj.AddComponent<T>();
            }
        }
        return final;
    }

    public static void RemoveToRoot<T>(this Transform trans,Transform root,bool setRemote = false) where T :Component 
    {
        var childs = trans.GetComponentsInChildren<T>();
        foreach(var child in childs)
        {
            child.transform.SetParent(root);
            child.transform.localPosition = new Vector3(99999,99999,0);
        }
    }
    
    public static void SetTextWithEllipsis(this Text textComponent, string value)
    {
        var generator = new TextGenerator();
        var rectTransform = textComponent.GetComponent<RectTransform>();
        var settings = textComponent.GetGenerationSettings(rectTransform.rect.size);
        generator.Populate(value, settings);
 
        // trncate visible value and add ellipsis
        var characterCountVisible = generator.characterCountVisible;
        var updatedText = value;
        if (value.Length > characterCountVisible)
        {
            updatedText = value.Substring(0, characterCountVisible - 1);
            updatedText += "…";
        }
 
        // update text
        textComponent.text = updatedText;
    }
}


/*

 public static string DateTime(long ut, string format)
    {
        return string.Format(format, Consts.UnixTime.AddSeconds(ut));
    }

    public static DateTime GetDateTime(long ut)
    {
       // return Consts.UnixTime.AddSeconds(ut);
       var dt = Consts.UnixTime.AddSeconds(ut);
       var utcTime =  TimeZone.CurrentTimeZone.ToUniversalTime(dt);
       return utcTime;
    }

    public static void SetSeverTime(long serverTime)
    {
        _deltaTimeBetweenLocalAndServer = serverTime - GetTimeStamp(System.DateTime.Now);
        ServerTimeReady = true;
        SafePostEvent(ServerTimeReadyEvent);

        //跨天检测
        var date = GetDateTime(GetServerTime() / 1000);

        var left = 24 * 60 * 60 - (date.Hour * 60 * 60 + date.Minute * 60 + date.Second);

        //加2秒延迟
        Utility.DoWait(()=>{
            Debug.LogFormat("wtf 跨天了 ========================= {0}",left);
            Utility.SafePostEvent(EventHelper.NewDayAction);
        },left + 2,Instance);
    }

public static string SmartNumberString(long number)
    {
        return number >= Consts.Million ? NumberSimlify(number) : CommaNumberString(number);
    }

    public static string CommaNumberString(long number)
    {
        return number == 0 ? number.ToString() : number.ToString(Consts.WesternNumberFormatString);
    }

    public static string NumberSimlify(long num, int decimalNumber = 1)
    {
        string result;
        if (num < Consts.Kilo)
        {
            result = num.ToString("G");
        }
        else
        {
            var i = 0;
            double number = num;
            while ((number /= Consts.Kilo) >= Consts.Kilo)
            {
                ++i;
            }
            var decimalValue = Math.Pow(10, decimalNumber);
            var format = "{0:F" + decimalNumber + "}" + Consts.KmbArray[i];
            result = string.Format(format, Math.Floor(number * decimalValue) / decimalValue);
        }
        return result;
    }

    public static string GetReadableFilesize(long size)
    {
        int i = 0;
        var sizefloat = (float)size;
        while (sizefloat >= Consts.BytesUnit)
        {
            sizefloat /= Consts.BytesUnit;
            i++;
        }
        return sizefloat.ToString("0.0") + Consts.BytesArray[i];
    }

    public static double GetUnixTotalSecondsFromDateTime(DateTime dateTime)
    {
        return (dateTime - Consts.UnixTime).TotalSeconds;
    }

    1天 10:20:10
    public static string GetFormatTime1(int seconds)
    {
        //秒
        var second = seconds % 60;
        //分
        var mini = (seconds / 60) % 60;
        //时
        var hour = (seconds / (60 * 60)) % 24;
        //天
        var day = (seconds / (60 * 60 * 24));

        var timeStr = "";
        if(day > 0)
        {
            timeStr += string.Format("{0}{1} ", day, LangUtil.GetString("UI_Common_Day"));
        }  
        var hourStr = hour >= 0 && hour < 10?"0"+hour.ToString():hour.ToString();
        var miniStr = mini >= 0 && mini < 10?"0"+mini.ToString():mini.ToString();
        var secondStr = second >= 0 && second < 10?"0"+second.ToString():second.ToString();

        timeStr += string.Format("{0}:{1}:{2}",hourStr,miniStr,secondStr);
        
        return timeStr;
    }

    //精确到几天,后面的都不显示
    public static string FormatTimeToDayFromSeconds(int seconds)
    {
        var timeSpan = new TimeSpan(0, 0, seconds);
        if (timeSpan.Days == 0)
            return FormatTimeFromSeconds(seconds);
        else
            return string.Format("{0}{1}", timeSpan.Days, LangUtil.GetString("UI_Common_Day"));
    }
    
    public static string FormatTimeFromSeconds(int seconds)
    {
        var timeSpan = new TimeSpan(0, 0, seconds);
        var timeStr = string.Format("{0}", timeSpan);
        if (timeSpan.Days > 0)
        {
            timeStr = timeStr.Replace(".", LangUtil.GetString("UI_Common_Day") + " ");
        }
        else if (timeSpan.Hours == 0)
        {
            timeStr = timeSpan.ToString().Substring(3);
        }
        else
        {
            timeStr = timeSpan.ToString();
        }
        return timeStr;
    }
public static string FormatCoordFromVector2(Vector2 vec)
    {
        return Consts.Xcoord + Consts.Spacing + vec.x + Consts.Spacing + Consts.Spacing + Consts.Spacing + Consts.Spacing + Consts.Ycoord + Consts.Spacing + vec.y;
    }

public static void SetPosition(Transform targetTransform, string axis, bool isLocal, params float[] positions)
    {
        var position = isLocal ? targetTransform.localPosition : targetTransform.position;
        for (var i = 0; i != positions.Length; ++i)
        {
            switch (axis[i])
            {
                case Consts.XChar:
                    position.x = positions[i];
                    break;
                case Consts.YChar:
                    position.y = positions[i];
                    break;
                case Consts.ZChar:
                    position.z = positions[i];
                    break;
            }
        }
        if (isLocal)
        {
            targetTransform.localPosition = position;
        }
        else
        {
            targetTransform.position = position;
        }
    }
public static void SetRectTransform(RectTransform rectTransform, float x, float y, float w, float h)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        SetPosition(rectTransform, "xy", true, x, y);
    }
public static string GetFileMd5(string path)
    {
        if (!File.Exists(path))
        { 
            return string.Empty;
        }
        var file = new FileStream(path, FileMode.Open);
        MD5 md5 = new MD5CryptoServiceProvider();
        var retVal = md5.ComputeHash(file);
        file.Close();
        var sb = new StringBuilder();
        foreach (var t in retVal)
        {
            sb.Append(t.ToString(Consts.X2String));
        }
        return sb.ToString();
    }

    public static string GetStringMd5(string str)
    {
        var bytValue = Encoding.UTF8.GetBytes(str);
        return GetBytesMd5(bytValue);
    }

    public static string GetBytesMd5(byte[] bytes)
    {
        var md5 = new MD5CryptoServiceProvider();
        var bytHash = md5.ComputeHash(bytes);
        md5.Clear();
        var sTemp = bytHash.Aggregate(Consts.EmptyString, (current, t) => current + t.ToString(Consts.X2String));
        return sTemp.ToLower();
    }
public static void GetBytesAync(string url, Action<string, string, byte[]> downloadComplete)
    {
        UtilityStartCoroutine(GetBytesCoroutine(url, downloadComplete));
    }

    public static IEnumerator GetBytesCoroutine(string url, Action<string, string, byte[]> downloadComplete)
    {
        var www = new WWW(url + Consts.QuestionMarkChar + Guid.NewGuid());
        yield return www;
        if (www.error != null)
        {
            SafePostEvent(downloadComplete, url, www.error, null);
            yield break;
        }
        SafePostEvent(downloadComplete, url, null, www.bytes);
    }

    public static IEnumerator DownloadFile(string url, string savePath)
    {
        yield return UtilityStartCoroutine(GetBytesCoroutine(url, (useUrl, error, bytes) =>
                {
                    if (error != null)
                    {
                        Debug.LogWarning(url + Consts.BlankChar + Consts.ErrorString + Consts.ColonChar + error);
                        return;
                    }
                    WriteAllBytes(savePath, bytes);
                }));
    }

    public static void ProcessTextEachLines(string text, Action<string> processLineAction)
    {
        if (text == null)
        {
            return;
        }
        var length = text.Length;
        var beginIndex = 0;
        var lastChar = text[length - 1];
        for (var i = 0; i < length; i++)
        {
            if (text[i] != Consts.ReturnChar && text[i] != Consts.NewlineChar)
            {
                continue;
            }
            var count = i - beginIndex;
            string line;
            if (count > 0)
            {
                line = text.Substring(beginIndex, count);
            }
            else
            {
                continue;
            }
            beginIndex = i + 1;
            while (text[beginIndex] == Consts.ReturnChar || text[beginIndex] == Consts.NewlineChar)
            {
                ++beginIndex;
                if (beginIndex >= text.Length)
                    break;
            }
            processLineAction(line);
        }
        if (lastChar != Consts.ReturnChar && lastChar != Consts.NewlineChar)
        {
            processLineAction(text.Substring(beginIndex, length - 1 - beginIndex));
        }
    }
public static Direction GetDirection(int originalX, int originalY, int targetX, int targetY)
    {
        var vec = new Vector2(targetX, targetY) - new Vector2(originalX, originalY);
        var angle = Vector2.Angle(vec, Vector2.right);
        Direction direction;
        if (vec.y < 0)
        {
            angle = -angle;
        }
        if (angle >= -135 && angle < -45)
        {
            direction = Direction.Down;
        }
        else if (angle >= -45 && angle < 45)
        {
            direction = Direction.Right;
        }
        else if (angle >= 45 && angle < 135)
        {
            direction = Direction.Up;
        }
        else
        {
            direction = Direction.Left;
        }
        return direction;
    }
protected static string GetTimePartString(int t, string tString)
    {
        return t > 0 ? t + tString : Consts.EmptyString;
    }
public static void ProcessRawImageNumbersPair(RawImageNumbersPair rawImageNumbersPair,
                                                  StringIntIntPair stringIntIntPair, Func<string, Texture> getItemTexture, Color enableColor,
                                                  Color disableColor)
    {
        var itemTexture = getItemTexture(stringIntIntPair.String);
        var returnTexture = rawImageNumbersPair.Refresh(itemTexture, stringIntIntPair.Int0, stringIntIntPair.Int1,
                                enableColor, disableColor, Consts.SlashChar.ToString());
        if (returnTexture != null)
        {
            Resources.UnloadAsset(returnTexture);
        }
    }

    public static void ProcessRawImageNumbersPair(RawImageNumbersPair rawImageNumbersPair, string textureName,
                                                  int number, Func<string, Texture> getItemTexture)
    {
        ProcessRawImageNumbersPair(rawImageNumbersPair,
            new StringIntIntPair { Int0 = 0, Int1 = number, String = textureName }, getItemTexture, Color.white,
            Color.white);
    }

    public static void ProcessRawImageNumbersPair(RawImageNumbersPair rawImageNumbersPair,
                                                  StringIntIntPair stringIntIntPair, Func<string, Texture> getItemTexture)
    {
        ProcessRawImageNumbersPair(rawImageNumbersPair, stringIntIntPair, getItemTexture, Color.white, Color.red);
    }

    public static void ProcessLabelNumberPair(LabelNumberPair labelNumberPair, StringIntPair stringIntPair)
    {
        labelNumberPair.Refresh(stringIntPair);
    }

    public static void ProcessRawImageChangeTexture(RawImage rawImage, string path)
    {
        rawImage.texture = Resources.Load<Texture>(path);
    }
 public static void MoveToStraightLine(GameObject bulletPrefab, Vector3 source, Transform target,
                                          float? time, float? speed, bool canbeTraced, Vector3 initTransform, Action onFinish)
    {
        float duration = 0;
        if (null == time)
        {
            var f = Vector3.Distance(target.position, source) / speed;
            if (f != null)
                duration = (float)f;
        }
        else
        {
            duration = (float)time;
        }

        StraightLineTrace comptCurve = bulletPrefab.AddComponent<StraightLineTrace>();
        comptCurve.Duration = duration;
        comptCurve.StartPosition = source;
        comptCurve.StartTransform = null;
        comptCurve.EndTransform = target;
        comptCurve.WhetherTrace = canbeTraced;
        comptCurve.InitTransform = initTransform;
        comptCurve.enabled = true;
    }
 public static void MoveToParabola(GameObject bulletPrefab, Vector3 source, Transform target, float height, float? time, float? speed, bool canbeTraced, Action onFinish)
    {
        float duration = 0;
        if (null == time)
        {
            var f = Vector3.Distance(target.position, source) / speed;
            if (f != null)
                duration = (float)f;
        }
        else
        {
            duration = (float)time;
        }

        CurveTrace comptCurve = bulletPrefab.AddComponent<CurveTrace>();
        comptCurve.Duration = duration;
        comptCurve.StartPosition = source;
        comptCurve.StartTransform = null;
        comptCurve.EndTransform = target;

        comptCurve.WhetherTrace = canbeTraced;
        comptCurve.enabled = true;
    }
    public static DayType GetLightDayType(out float time)
    {
//            if (Circadian.IsFirst)
//            {
//                time = 10;
//                return DayType.Day;
//            }
        var serverTime = GetServerTime();
        DateTime now = System.DateTime.Now;
        DateTime zeroTime = new DateTime(now.Year, now.Month, now.Day);
        var zeroMilSeconds = GetTimeStamp(zeroTime);
        var offsetMilliSeconds = serverTime - zeroMilSeconds;
        var offsetMintues = offsetMilliSeconds / 1000;
        var min = offsetMintues; //offsetMintues % 60;
        DayType currentDayType ;
        var dayTime = GdsManager.Instance.GetGds<GdsDaynightsystem>().GetItemById((int) DayType.Day).Time;
        var duskTime = GdsManager.Instance.GetGds<GdsDaynightsystem>().GetItemById((int) DayType.Dusk).Time;
        var nightTime = GdsManager.Instance.GetGds<GdsDaynightsystem>().GetItemById((int) DayType.Night).Time;
        var dawnTime = GdsManager.Instance.GetGds<GdsDaynightsystem>().GetItemById((int) DayType.Dawn).Time;
        var tempMin = min% (dayTime + duskTime + nightTime + dawnTime);
                    //Debug.LogError("tempMin=================" + tempMin + "---dayTime==" + dayTime+"----------duskTime==="+duskTime+"---------nightTime=="+nightTime+"---dawnTime==="+dawnTime);
        if (0 <= tempMin && tempMin <= dayTime)
        {
            currentDayType = DayType.Day;
            tempMin = dayTime - tempMin;
        }
        else if (dayTime < tempMin && tempMin <= dayTime + duskTime)
        {
            //currentDayType = DayType.Dusk;
            currentDayType = DayType.Dusk;
            tempMin = dayTime+ duskTime-tempMin;
        }
        else if (dayTime + duskTime < tempMin && tempMin <= dayTime + duskTime + nightTime)
        {
            currentDayType = DayType.Night;
            tempMin = (dayTime + duskTime+nightTime)-tempMin;
        }
        else
        {
            tempMin = (dayTime + duskTime+nightTime+dawnTime)-tempMin;
            //currentDayType = DayType.Dawn;
            currentDayType = DayType.Dawn;
        }

        time = tempMin+1;
        if (time < 10)
            time = 10;
        if (PlayerData.DayNightSystemStop)
        {
            return PlayerData.StopDayType;
        }
        return currentDayType;
    }
public static Vector2 GetUIPositionFromWorldPosition(Vector3 worldPosition, UnityEngine.Camera camera, UnityEngine.Camera uiCamera)
    {
        Vector3 screenPosition = new Vector3();
        if (camera != null)
            screenPosition = camera.WorldToScreenPoint(worldPosition);

        Vector2 screePos = new Vector2(screenPosition.x,screenPosition.y);
        Vector2 retPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasInfo.Instance.MainCanvas.transform as RectTransform, screePos, uiCamera, out retPos);
        return retPos;
    }

    public static Vector2 WorldPositionToCanvasPosition(Vector3 worldPosition, UnityEngine.Camera camera)
    {
        Vector3 screenPosition = new Vector3();
        if (camera != null)
        {
            screenPosition = camera.WorldToViewportPoint(worldPosition);
        }
        Vector2 canvasPosition = new Vector2(
            ((screenPosition.x * CanvasInfo.CanvasSizeDelta.x) - (CanvasInfo.CanvasSizeDelta.x * 0.5f)),
            ((screenPosition.y * CanvasInfo.CanvasSizeDelta.y) - (CanvasInfo.CanvasSizeDelta.y * 0.5f)));

        return canvasPosition;
    }
    
    public static Vector2 GetCanvasPositionFromWorldPosition(Vector3 worldPosition, UnityEngine.Camera camera)
    {
        Vector3 screenPosition = new Vector3();
        if (camera != null)
            screenPosition = camera.WorldToScreenPoint(worldPosition);
        return new Vector2(CanvasInfo.CanvasSizeDelta.x * (screenPosition.x / Screen.width - 0.5f),
            CanvasInfo.CanvasSizeDelta.y * (screenPosition.y / Screen.height - 0.5f));
    }

    public static Vector2 GetCanvasPositionZeroZFromWorldPosition(Vector3 worldPosition, UnityEngine.Camera camera)
    {
        Vector3 screenPosition = new Vector3();
        if (camera != null)
        {
            if (worldPosition.z < camera.transform.position.z)
                worldPosition.z = camera.transform.position.z;
            screenPosition = camera.WorldToScreenPoint(worldPosition);
        }
        
        return new Vector2(CanvasInfo.CanvasSizeDelta.x * (screenPosition.x / Screen.width - 0.5f),
            CanvasInfo.CanvasSizeDelta.y * (screenPosition.y / Screen.height - 0.5f));
    }
public static bool CheckResourceCollect(WorldPoint wp)
    {
        if (wp.Type == WorldPointType.PointtypeResourceCan)
        {
            if (StepUnlock.Instance.GetUnLockDatas.Count > 0)
            {
                for (int i = 0; i < StepUnlock.Instance.GetUnLockDatas.Count; i++)
                {
                    if (StepUnlock.Instance.GetUnLockDatas[i].ResType == ResourceType.Can)
                    {
                        return false;
                    }
                }
            }
        }
        else if (wp.Type == WorldPointType.PointtypeResourceOil)
        {
            if (StepUnlock.Instance.GetUnLockDatas.Count > 0)
            {
                for (int i = 0; i < StepUnlock.Instance.GetUnLockDatas.Count; i++)
                {
                    if (StepUnlock.Instance.GetUnLockDatas[i].ResType == ResourceType.Oil)
                    {
                        return false;
                    }
                }
            }
        }
        else if (wp.Type == WorldPointType.PointtypeResourceSteel)
        {
            if (StepUnlock.Instance.GetUnLockDatas.Count > 0)
            {
                for (int i = 0; i < StepUnlock.Instance.GetUnLockDatas.Count; i++)
                {
                    if (StepUnlock.Instance.GetUnLockDatas[i].ResType == ResourceType.Steel)
                    {
                        return false;
                    }
                }
            }
        }
        else if (wp.Type == WorldPointType.PointtypeResourceTombarthite)
        {
            if (StepUnlock.Instance.GetUnLockDatas.Count > 0)
            {
                for (int i = 0; i < StepUnlock.Instance.GetUnLockDatas.Count; i++)
                {
                    if (StepUnlock.Instance.GetUnLockDatas[i].ResType == ResourceType.Tombarthite)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
 ui根据目标点偏移 锚点0.5 0.5
    @origin 默认偏移向上或者向下 true为向上
    public static void OffSetView(RectTransform rect,RectTransform targetRect,Vector3 pos,float offset = 0,bool origin = false)
    {
        var targetPos = targetRect.transform.InverseTransformPoint ( pos ) ;
        targetPos = new Vector3(targetPos.x,targetPos.y,0);
        
        var rectSize = rect.sizeDelta;

        var halfWidthRect = rectSize.x / 2;
        var halfHeightRect = rectSize.y / 2;

        var canvasW =  CanvasInfo.CanvasSizeDelta.x;
        var canvasH =  CanvasInfo.CanvasSizeDelta.y;

        var halfWidth = canvasW / 2f;
        var halfHeight = canvasH / 2f;

        var factor = origin?1:-1;
        targetPos += new Vector3(0,(halfHeightRect + offset) * factor,0);            //默认往下偏移半个高度

        /*矫正
        var offsetx = 0f;
        var offsety = 0f;
        //如果超出屏幕下方
        if(targetPos.y - halfHeightRect < -halfHeight)offsety = 2*halfHeightRect + 2*offset;
            
        //超出左边屏幕
        if(targetPos.x - halfWidthRect < -halfWidth)offsetx = -halfWidth - (targetPos.x - halfWidthRect);
       
        //超出右边屏幕
        if(targetPos.x + halfWidthRect > halfWidth)offsetx = halfWidth - (targetPos.x + halfWidthRect) ;

        rect.localPosition = targetPos + new Vector3(offsetx,offsety,0);

    }   

*/