using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CIFER.Tech.Utils.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CIFER.Tech.FbxAnimationAdjuster.Editor
{
    public class FbxAnimationImportAdjusterEditor : EditorWindow
    {
        public Object[] targetFbxs = new Object[0];
        private Vector2 _scrollPos;

        [MenuItem("CIFER.Tech/FbxAnimationImportAdjuster")]
        private static void Open()
        {
            var window = GetWindow<FbxAnimationImportAdjusterEditor>("FbxAnimationImportAdjuster");
            window.minSize = new Vector2(350f, 300f);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("選択中のFBXを登録する"))
                {
                    var selectObjects = Selection.objects;
                    if (selectObjects == null)
                        return;

                    targetFbxs = (from obj in selectObjects
                        let path = AssetDatabase.GetAssetPath(obj)
                        where string.Compare(Path.GetExtension(path), ".fbx", StringComparison.OrdinalIgnoreCase) == 0
                        select obj).ToArray();
                }

                if (GUILayout.Button("選択フォルダ内のFBXを登録する"))
                {
                    var buffer = new List<Object>();
                    foreach (var obj in Selection.objects)
                    {
                        var directoryPath = AssetDatabase.GetAssetPath(obj);
                        if (!Directory.Exists(directoryPath))
                            continue;

                        var fbxPath = Directory.GetFiles(directoryPath, "*.fbx", SearchOption.AllDirectories);
                        buffer.AddRange(fbxPath.Select(AssetDatabase.LoadMainAssetAtPath));
                    }

                    targetFbxs = buffer.ToArray();
                }
            }
            EditorGUILayout.EndHorizontal();


            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUI.skin.box);
            {
                CiferTechEditorUtils.ArrayApplyModifiedProperties(nameof(targetFbxs), "変換するFBXアニメーション", this);
            }
            EditorGUILayout.EndScrollView();

            if (targetFbxs.Length <= 0 || targetFbxs.All(target => target is null))
            {
                return;
            }

            var pathList = targetFbxs.Select(AssetDatabase.GetAssetPath).ToList();
            var fbxExtensionList = pathList.Select(path =>
                string.Compare(Path.GetExtension(path), ".fbx", StringComparison.OrdinalIgnoreCase) == 0).ToList();

            if (fbxExtensionList.All(t => !t))
            {
                EditorGUILayout.HelpBox("指定されたファイルは全てFBXではありません。", MessageType.Error);
                return;
            }

            if (fbxExtensionList.Any(t => !t))
            {
                EditorGUILayout.HelpBox("一部の指定されたファイルはFBXではありません。\r\nFBXファイルのみ処理します。", MessageType.Warning);
            }

            if (GUILayout.Button("変換"))
            {
                for (var i = 0; i < pathList.Count; i++)
                {
                    if (fbxExtensionList[i])
                    {
                        FbxAnimationImportAdjuster.InitializeSkeleton(pathList[i]);
                    }
                }
            }
        }
    }
}