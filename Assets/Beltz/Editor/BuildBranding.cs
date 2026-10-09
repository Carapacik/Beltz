using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Beltz.Editor
{
    public sealed class BuildBranding : IPreprocessBuildWithReport
    {
        [Serializable] public sealed class Identity { public string productName; public string companyName; }
        [Serializable] public sealed class Configuration { public Identity android; public Identity web; public Identity desktop; }
        public int callbackOrder => -1000;

        public void OnPreprocessBuild(BuildReport report) => Apply(report.summary.platform);

        [MenuItem("Beltz/Apply branding and icons")]
        public static void ApplyCurrent()
        {
            Apply(EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.SaveAssets();
        }

        public static void Apply(BuildTarget target)
        {
            var config = JsonUtility.FromJson<Configuration>(File.ReadAllText("ProjectSettings/BeltzBranding.json"));
            var identity = target == BuildTarget.Android ? config.android : target == BuildTarget.WebGL ? config.web : config.desktop;
            if (identity == null || string.IsNullOrWhiteSpace(identity.productName) || string.IsNullOrWhiteSpace(identity.companyName))
                throw new BuildFailedException("Product and company names are required in BeltzBranding.json.");
            PlayerSettings.productName = identity.productName;
            PlayerSettings.companyName = identity.companyName;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.carapacik.beltz");
            PlayerSettings.WebGL.template = "PROJECT:Beltz";
            LauncherIcons.Configure();
            Debug.Log($"BELTZ_BRANDING_OK: {target}, {identity.productName}, {identity.companyName}");
        }

    }

    public sealed class AppIconImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            bool icon = assetPath.StartsWith("Assets/Beltz/Art/AndroidIcons/") ||
                assetPath == "Assets/Beltz/Art/AppIcon.png" ||
                assetPath == "Assets/Beltz/Art/AppIconBackground.png" ||
                assetPath == "Assets/Beltz/Art/AppIconMonochromeSource.png";
            if (!icon || !assetPath.EndsWith(".png")) return;
            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
        }
    }
}
