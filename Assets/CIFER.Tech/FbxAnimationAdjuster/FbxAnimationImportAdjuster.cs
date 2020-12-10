using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CIFER.Tech.FbxAnimationAdjuster
{
    public static class FbxAnimationImportAdjuster
    {
        public static void InitializeSkeleton(string path)
        {
            //PathからAssetImporter(ModelImporter)を取得する
            if (!(AssetImporter.GetAtPath(path) is ModelImporter modelImporter))
                return;

            //AnimationTypeをHumanにしないとhumanDescriptionが読めなくて積むので変更
            modelImporter.animationType = ModelImporterAnimationType.Human;
            modelImporter.SaveAndReimport();

            //RotationをQuaternion.identityにしてSkeletonBone（モデルに含めるボーンTransformのリスト）をコピー
            var humanDescription = modelImporter.humanDescription;
            humanDescription.skeleton = humanDescription.skeleton.Select(skeletonBone =>
            {
                skeletonBone.rotation = Quaternion.identity;
                return skeletonBone;
            }).ToArray();

            modelImporter.humanDescription = humanDescription;
            modelImporter.SaveAndReimport();

            //AnimationClipの出力
            var animationClips = AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                .Where(obj => obj is AnimationClip);
            foreach (var clip in animationClips)
            {
                var newClip = Object.Instantiate(clip);
                AssetDatabase.CreateAsset(newClip,
                    $"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}_{clip.name}.anim");
            }

            AssetDatabase.Refresh();
        }
    }
}