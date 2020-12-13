using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
 
[CustomEditor(typeof(SwitchButton),true)]
[CanEditMultipleObjects]
public class SwitchButtonEditor : ButtonEditor
{
    private SerializedProperty ViewObj;
    private SerializedProperty UIName;
 
    protected override void OnEnable()
    {
        base.OnEnable();
        ViewObj = serializedObject.FindProperty("ViewObj");
        UIName = serializedObject.FindProperty("UIName");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();//空行
        serializedObject.Update();

        if(ViewObj != null)
        {
            var go = (GameObject)ViewObj.objectReferenceValue;
            if(go != null)UIName.stringValue = go.name;
            // ViewObj.objectReferenceValue = null;
        }
        

        EditorGUILayout.PropertyField(ViewObj);//显示我们创建的属性
        EditorGUILayout.PropertyField(UIName);//显示我们创建的属性
        serializedObject.ApplyModifiedProperties();
    }
}
