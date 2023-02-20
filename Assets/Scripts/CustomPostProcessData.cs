#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif
using System;
// using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable]
    public class CustomPostProcessData : PostProcessData
    {
#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812")]
        internal class CreatePostProcessDataAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<PostProcessData>();
                AssetDatabase.CreateAsset(instance, pathName);
                ResourceReloader.ReloadAllNullIn(instance, UniversalRenderPipelineAsset.packagePath);
                Selection.activeObject = instance;
            }
        }

        [MenuItem("Assets/Create/Rendering/URP Post-process Data", priority = CoreUtils.Sections.section5 + CoreUtils.Priorities.assetsCreateRenderingMenuPriority)]
        static void CreatePostProcessData()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreatePostProcessDataAsset>(), "CustomPostProcessData.asset", null, null);
        }

        internal static PostProcessData GetDefaultPostProcessData()
        {
            var path = System.IO.Path.Combine(UniversalRenderPipelineAsset.packagePath, "Runtime/Data/PostProcessData.asset");
            return AssetDatabase.LoadAssetAtPath<PostProcessData>(path);
        }

#endif

        [Serializable, ReloadGroup]
        public sealed class CustomShaderResources
        {

        }
        // [Serializable, ReloadGroup]
        // public sealed class TextureResources
        // {
        //     // Pre-baked noise
        //     [Reload("Textures/BlueNoise16/L/LDR_LLL1_{0}.png", 0, 32)]
        //     public Texture2D[] blueNoise16LTex;

        //     // Post-processing
        //     [Reload(new[]
        //     {
        //         "Textures/FilmGrain/Thin01.png",
        //         "Textures/FilmGrain/Thin02.png",
        //         "Textures/FilmGrain/Medium01.png",
        //         "Textures/FilmGrain/Medium02.png",
        //         "Textures/FilmGrain/Medium03.png",
        //         "Textures/FilmGrain/Medium04.png",
        //         "Textures/FilmGrain/Medium05.png",
        //         "Textures/FilmGrain/Medium06.png",
        //         "Textures/FilmGrain/Large01.png",
        //         "Textures/FilmGrain/Large02.png"
        //     })]
        //     public Texture2D[] filmGrainTex;

        //     [Reload("Textures/SMAA/AreaTex.tga")]
        //     public Texture2D smaaAreaTex;

        //     [Reload("Textures/SMAA/SearchTex.tga")]
        //     public Texture2D smaaSearchTex;
        // }

        // public ShaderResources shaders;
        // public TextureResources textures;
    }
}
