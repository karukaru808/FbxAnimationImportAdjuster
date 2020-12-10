using UnityEditor;
using UnityEngine;

namespace CIFER.Tech.Utils.Editor
{
    public static class CiferTechEditorUtils
    {
        public static void ArrayApplyModifiedProperties<T>(string varName, string dispName, T target)
            where T : ScriptableObject
        {
            ScriptableObject scriptableObject = target;
            var so = new SerializedObject(scriptableObject);
            var sp = so.FindProperty(varName);
            EditorGUILayout.PropertyField(sp, new GUIContent(dispName), true, GUILayout.ExpandWidth(true));
            so.ApplyModifiedProperties();
        }
    }
}