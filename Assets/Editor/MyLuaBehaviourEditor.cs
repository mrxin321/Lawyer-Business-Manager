using System;
using System.Collections.Generic;
using CustomAssets.Scripts.XLuaScripts;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyLuaBehaviour))]
public class MyLuaBehaviourEditor : Editor
{
     public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mono Item List", GUILayout.Width(200));
            MyLuaBehaviour luaBehaviour = target as MyLuaBehaviour;
            Undo.RecordObject(target, "F");
            if (luaBehaviour.monos != null)
            {
                for (int i = 0; i < luaBehaviour.monos.Length; i++)
                {
                    List<Type> allowTypes = new List<Type>() {typeof(GameObject)};
                    int selectIndex = 0;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label((i + 1).ToString(), GUILayout.Width(20));
                    SetMonoItemName(luaBehaviour, i);
                    UnityEngine.Object obj = GetMonos(luaBehaviour, i);
                    if (obj != null)
                    {
                        Type currentType = obj.GetType();
                        GameObject go = null;
                        if (currentType == typeof(GameObject))
                            go = (GameObject) obj;
                        else if (currentType.IsSubclassOf(typeof(Component)))
                            go = ((Component) obj).gameObject;
                        Component[] cs = null;

                        if (go != null)
                        {
                            cs = go.GetComponents<Component>();

                            for (int j = 0; j < cs.Length; j++)
                            {
                                allowTypes.Add(cs[j].GetType());
                                if (obj == cs[j])
                                    selectIndex = j + 1;
                            }

                            selectIndex = EditorGUILayout.Popup(selectIndex, ConvertTypeArrayToStringArray(allowTypes));
                            if (selectIndex == 0)
                                AddMonoItem(luaBehaviour, i, go); //temp.monos[i] = go;
                            else
                                AddMonoItem(luaBehaviour, i, cs[selectIndex - 1]); //temp.monos[i] = cs[selectIndex - 1];
                        }

                        if (luaBehaviour.monoItemNames[i] == "")
                            luaBehaviour.monoItemNames[i] = obj.name;
                    }

                    AddMonoItem(luaBehaviour, i, EditorGUILayout.ObjectField(GetMonos(luaBehaviour, i), typeof(UnityEngine.Object), true));

                    var objfind = GetMonos(luaBehaviour, i);
                    if (luaBehaviour.monoItemNames.Count > i && luaBehaviour.monoItemNames[i] != "" && objfind != null && luaBehaviour.monoItemNames[i] != objfind.name)
                    {
                        if (GUILayout.Button("Auto", GUILayout.Width(50)))
                        {
                            luaBehaviour.monoItemNames[i] = "";
                        }
                    }

                    if (GUILayout.Button("Del", GUILayout.Width(40)))
                    {
                        luaBehaviour.monoItemNames.RemoveAt(i);
                        RemoveAtMonos(luaBehaviour, i);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Item"))
            {
                luaBehaviour.monoItemNames.Add("");
                AddMonoItem(luaBehaviour, -1, null);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        public void AddMonoItem(MyLuaBehaviour item, int i, UnityEngine.Object obj)
        {
            List<UnityEngine.Object> monos = null;

            if (item.monos != null)
                monos = new List<UnityEngine.Object>(item.monos);
            else
                monos = new List<UnityEngine.Object>();

            if (i < 0)
            {
                monos.Add(obj);
            }
            else
            {
                while (monos.Count <= i)
                    monos.Add(null);
                monos[i] = obj;
            }
            item.monos = monos.ToArray();
            
            //item.moniItemIndexs
        }
        
        public void RemoveAtMonos(MyLuaBehaviour refer, int index)
        {
            List<UnityEngine.Object> monos = new List<UnityEngine.Object>(refer.monos);
            monos.RemoveAt(index);
            refer.monos = monos.ToArray();
        }

        public UnityEngine.Object GetMonos(MyLuaBehaviour refer, int index)
        {
            if (index >= 0 && index < refer.monos.Length)
                return refer.monos[index];
            return null;
        }

        public void SetMonoItemName(MyLuaBehaviour monoItem, int i)
        {
            while (monoItem.monoItemNames.Count <= i)
                monoItem.monoItemNames.Add(null);
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.xMin = 30;
            rect.width = 70;
            monoItem.monoItemNames[i] = EditorGUI.TextField(rect, monoItem.monoItemNames[i]);
            GUILayout.Space(70);
        }
        
        public string[] ConvertTypeArrayToStringArray(List<Type> tps)
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < tps.Count; i++)
            {
                string s = tps[i].ToString();
                int index = s.LastIndexOf('.');
                if (index != -1)
                {
                    index += 1;
                    s = s.Substring(index);
                }
                temp.Add(s);
            }
            return temp.ToArray();
        }
}
