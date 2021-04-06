using System.Collections;
using System.Collections.Generic;
using Assets.CustomAssets.Scripts.Foundation.Common;
using UnityEngine;
using XLua;

namespace CustomAssets.Scripts.XLuaScripts
{
    public class MyLuaBehaviour : MonoBehaviour
    {
        // Start is called before the first frame update
        [System.NonSerialized] public bool usingUpdate = false;
        [System.NonSerialized] public bool usingFixedUpdate = false;
        [System.NonSerialized] public bool usingLateUpdate = false;

        [SerializeField] protected string LuaBind;

        protected LuaTable table;

        //保存的lua 数据存取
        public LuaTable data { get; set; }

        protected LuaFunction Lua_Start;
        protected LuaFunction Lua_Update;
        protected LuaFunction Lua_FixedUpdate;
        protected LuaFunction Lua_LateUpdate;
        protected LuaFunction Lua_OnDestroy;
        protected LuaFunction Lua_OnEnable;
        protected LuaFunction Lua_OnDisable;
        protected LuaFunction Lua_OnPause;
        protected LuaFunction Lua_OnResume;
        protected LuaFunction Lua_SetData;

        public delegate void OnTriggerEnterDelegate(Collider other);

        public delegate void OnTriggerExitDelegate(Collider other);

        public delegate void OnTriggerEnter2DDelegate(Collider2D other);

        public delegate void OnTriggerExit2DDelegate(Collider2D other);

        public OnTriggerEnterDelegate OnTriggerEnterDeleg;
        public OnTriggerExitDelegate OnTriggerExitDeleg;

        public OnTriggerEnter2DDelegate OnTriggerEnter2DDeleg;
        public OnTriggerExit2DDelegate OnTriggerExit2DDeleg;

        private Dictionary<int, IEnumerator> _repeateEnumerators_Dic = new Dictionary<int, IEnumerator>();
        private object arg;

        #region 拖拽配置

        [HideInInspector] public Object[] monos;
        [HideInInspector] public List<string> monoItemNames = new List<string>();

        #endregion
    
        void Awake()
        {
            if(!string.IsNullOrEmpty(LuaBind))
            {
                LuaBridge.requireLuaFile.Call(LuaBind, this);
            }
        }        

        protected void Update()
        {
            if (usingUpdate)
            {
                CallMethod("Update");
                if (Lua_Update != null)
                    Lua_Update.Call(Time.deltaTime);
            }
        }

        protected void FixedUpdate()
        {
            if (usingFixedUpdate)
            {
                CallMethod("FixedUpdate");

                if (Lua_FixedUpdate != null)
                    Lua_FixedUpdate.Call();
            }
        }

        protected void LateUpdate()
        {
            if (usingLateUpdate)
            {
                CallMethod("LateUpdate");
                if (Lua_LateUpdate != null)
                    Lua_LateUpdate.Call();
            }
        }

        protected void OnDestroy()
        {
            CallMethod("OnDestroy");

            if (Lua_OnDestroy != null)
                Lua_OnDestroy.Call();
            if (table != null)
            {
                table.Dispose();
            }
        }

        protected void OnDisable()
        {
            CallMethod("OnDisable");
            if (Lua_OnDisable != null)
            {
                Lua_OnDisable.Call();
            }

            foreach (var item in _repeateEnumerators_Dic.Values)
            {
                if(item != null)
                    StopCoroutine(item);
            }
        }

        void OnEnable()
        {
            StartCoroutine(WaitFrameEnable());
            _repeateEnumerators_Dic.Clear();
        }

        private IEnumerator WaitFrameEnable()
        {
            yield return new WaitForEndOfFrame();
            CallMethod("OnEnable");
            if (Lua_OnEnable != null)
            {
                Lua_OnEnable.Call();
            }
        }


        protected void OnTriggerEnter(Collider other)
        {
            if (OnTriggerEnterDeleg != null)
                OnTriggerEnterDeleg(other);
        }

        protected void OnTriggerExit(Collider other)
        {
            if (OnTriggerExitDeleg != null)
                OnTriggerExitDeleg(other);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (OnTriggerEnter2DDeleg != null)
                OnTriggerEnter2DDeleg(other);
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (OnTriggerExit2DDeleg != null)
                OnTriggerExit2DDeleg(other);
        }

        public void RunCoroutine(YieldInstruction ins, LuaFunction func, object args)
        {
            StartCoroutine(doCoroutine(ins, func, args));
        }

        private IEnumerator doCoroutine(YieldInstruction ins, LuaFunction func, object args)
        {
            yield return ins;
            if (args != null)
            {
                func.Call(args);
            }
            else
            {
                func.Call();
            }
        }


        //设置执行的table对象
        public void SetBehaviour(LuaTable myTable)
        {
            table = myTable;

            table["this"] = this;
            table["transform"] = transform;
            table["gameObject"] = gameObject;
            CallMethod("Start");
        }


        //加载脚本文件
//        public void DoFile(string fn, System.Action complete = null)
//        {
//            StartCoroutine(DoMeFile(fn, complete));
//        }


//        IEnumerator DoMeFile(string fn, System.Action complete = null)
//        {
//            yield return new WaitForEndOfFrame();
//            while (env == null || !env.isReady)
//            {
//                yield return new WaitForEndOfFrame();
//            }
//
//            try
//            {
//                object chunk = env.DoFile(fn);
//
//                if (chunk != null && (chunk is LuaTable))
//                {
//                    SetBehaviour((LuaTable) chunk);
//                }
//
//            }
//            catch (System.Exception e)
//            {
//                Debug.LogError(FormatException(e), gameObject);
//            }
//
//            if (complete != null)
//                complete();
//        }

        public void SetChunk(LuaTable luaTable)
        {
            table = luaTable;
        }

        //获取绑定的lua脚本
        public LuaTable GetChunk()
        {
            return table;
        }

//        //设置lua脚本可直接使用变量
//        public void SetEnv(string key, object val, bool isGlobal)
//        {
//            if (isGlobal)
//            {
//                env[key] = val;
//            }
//            else
//            {
//                if (table != null)
//                {
//                    table[key] = val;
//                }
//            }
//        }
        //延迟执行
        public void LuaInvoke(float delaytime, LuaFunction func, params object[] args)
        {
            StartCoroutine(doInvoke(delaytime, func, args));
        }

        private IEnumerator doInvoke(float delaytime, LuaFunction func, params object[] args)
        {
            yield return new WaitForSeconds(delaytime);
            if (args != null)
            {
                func.Call(args);
            }
            else
            {
                func.Call();
            }
        }
        /// <summary>
        /// 重复延迟调用
        /// </summary>
        /// <param name="delaytime"></param>
        /// <param name="func"></param>
        /// <param name="RepeateCount">RepeateCount 小于0 为无限循环，等于0就不调用</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int RepeateLuaInvoke(float delaytime, LuaFunction func, int RepeateCount, params object[] args)
        {
            if (func == null) return -1;
            int count = _repeateEnumerators_Dic.Count;
            //count = count > 0 ? _repeateEnumerators_Dic.Count + 1 : _repeateEnumerators_Dic.Count;
            delaytime = delaytime <= 0 ? Time.deltaTime : delaytime;
            if (_repeateEnumerators_Dic.ContainsKey(count))
            {
                StopCoroutine(_repeateEnumerators_Dic[count]);
                _repeateEnumerators_Dic[count] = DoRepeateInvoke(delaytime, func, RepeateCount, args);
                StartCoroutine(_repeateEnumerators_Dic[count]);
            }
            else
            {
                _repeateEnumerators_Dic.Add(count,DoRepeateInvoke(delaytime, func, RepeateCount, args));
                StartCoroutine(_repeateEnumerators_Dic[count]);
            }

            return count;
        }
        
        public int RunRepeateLuaInvoke(int repeateId, int RepeateCount)
        {
            if (repeateId < 0) return -1;
            IEnumerator myIE;
            _repeateEnumerators_Dic.TryGetValue(repeateId, out myIE);
            if (myIE != null)
            {
                StartCoroutine(myIE);
            }
            //count = count > 0 ? _repeateEnumerators_Dic.Count + 1 : _repeateEnumerators_Dic.Count;

            return repeateId;

        }

        public void StopRepeateLuaInvoke(int id)
        {
            if (_repeateEnumerators_Dic.ContainsKey(id))
            {
                StopCoroutine(_repeateEnumerators_Dic[id]);
            }
        }
        
        public void CleanRepeateLuaInvoke(int RepeateId)
        {
            IEnumerator myIE;
            _repeateEnumerators_Dic.TryGetValue(RepeateId,out myIE);
            if (myIE != null)
            {
                StopCoroutine(myIE);
                _repeateEnumerators_Dic.Remove(RepeateId);
            }
        }

        private IEnumerator DoRepeateInvoke(float delaytime, LuaFunction func, int RepeateCount,params object[] args)
        {
            if (RepeateCount < 0)
            {
                while (true)
                {
                    yield return new WaitForSeconds(delaytime);
                    if (args != null)
                    {
                        func.Call(args);
                    }
                    else
                    {
                        func.Call();
                    }
                }
            }
            else
            {
                while (RepeateCount > 0)
                {
                    yield return new WaitForSeconds(delaytime);
                    if (args != null)
                    {
                        func.Call(args);
                    }
                    else
                    {
                        func.Call();
                    }

                    RepeateCount -= 1;
                }
            }

        }

        //协程
        public void RunCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
        {
            StartCoroutine(doCoroutine(ins, func, args));
        }

        public void CancelCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
        {
            StopCoroutine(doCoroutine(ins, func, args));
        }

        private IEnumerator doCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
        {
            yield return ins;
            if (args != null)
            {
                func.Call(args);
            }
            else
            {
                func.Call();
            }
        }

        /// <summary>
        /// 调用与该脚本绑定的Lua table 中的方法
        /// </summary>
        /// <param name="function"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CallMethod(string function, params object[] args)
        {
            if (table == null || table[function] == null || !(table[function] is LuaFunction)) return null;

            LuaFunction func = (LuaFunction) table[function];

            if (func == null) return null;
            try
            {
                if (args != null)
                {
                    return func.Call(table,args);
                }

                func.Call(table);
                return null;
            }
            catch (System.Exception e)
            {
                EDebug.LogWarning(FormatException(e), gameObject);
            }

            return null;
        }

        public object CallMethod(string function)
        {
            return CallMethod(function, table);
        }

        public static string FormatException(System.Exception e)
        {
            string source = (string.IsNullOrEmpty(e.Source))
                ? "<no source>"
                : e.Source.Substring(0, e.Source.Length - 2);
            return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
        }

        //挂接回调调用函数：一般用于jni或者invoke等操作

        public void SetStart(LuaFunction start)
        {
            Lua_Start = start;
        }

        public void SetData(int intData)
        {
            if(Lua_SetData != null)Lua_SetData.Call(intData);
        }

        public void SetData(string strData)
        {
            if(Lua_SetData != null)Lua_SetData.Call(strData);
        }

        public void SetData(LuaTable tableData)
        {
            if(Lua_SetData != null)Lua_SetData.Call(tableData);
        }

        public void SetData()
        {
            if(Lua_SetData != null)Lua_SetData.Call();
        }

        public void SetDataFunc(LuaFunction setData)
        {
            Lua_SetData = setData;
        }

        public void SetUpdate(LuaFunction update)
        {
            Lua_Update = update;
        }

        public void SetFixedUpdate(LuaFunction update)
        {
            Lua_FixedUpdate = update;
        }

        public void SetLateUpdate(LuaFunction update)
        {
            Lua_LateUpdate = update;
        }

        public void SetDestory(LuaFunction destory)
        {
            Lua_OnDestroy = destory;
        }

        public void SetEnable(LuaFunction enable)
        {
            Lua_OnEnable = enable;
        }

        public void SetDisable(LuaFunction disable)
        {
            Lua_OnDisable = disable;
        }


        public void Clear()
        {
            usingUpdate = false;
            usingFixedUpdate = false;
            usingLateUpdate = false;


            Lua_Start = null;
            Lua_Update = null;
            Lua_FixedUpdate = null;
            Lua_LateUpdate = null;
            Lua_OnDestroy = null;
            Lua_OnEnable = null;
            Lua_OnDisable = null;
            Lua_OnPause = null;
            Lua_OnResume = null;
        }
        public Object GetMonoItemByName(string n)
        {
            int index = monoItemNames.IndexOf(n);
            if (index == -1)
            {
                Debug.LogWarning(gameObject.name + "Item : not found the key [" + n + "]");
                return null;
            }
            else
                return GetMonoItemByIndex(index);
        }

        public Object Get(string n)
        {
            return GetMonoItemByName(n);
        }

        public Object Get(int index)
        {
            index = index - 1;
            if (index >= 0 && index < monos.Length)
            {
                return monos[index];
            } 
            else return null;
        }

        /// <summary>
        /// 更推荐用index 获取
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Object GetMonoItemByIndex(int index)
        {
            if (index >= 0 && index < monos.Length)
            {
                return monos[index];
            }
            else
            {
                Debug.LogWarning(gameObject.name + "Item : not found the key [" + index + "]");
                return null;
            }
        }
        
        
        public GameObject InstantiatePrefab(GameObject prefab, Transform parent)
        {
            GameObject go = Instantiate(prefab, parent);
            return go;
        }
    }
}