using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif

namespace Beltz.Editor
{
    // Artwork is imported once; density variants and Android XML are reproducible build assets.
    public sealed class LauncherIcons : IPreprocessBuildWithReport
#if UNITY_ANDROID
        , IPostGenerateGradleAndroidProject
#endif
    {
        private const string Art = "Assets/Beltz/Art";
        private const string Master = "Assets/Beltz/Art/AppIcon.png";
        private const string Background = "Assets/Beltz/Art/AppIconBackground.png";
        private const string Mask = "Assets/Beltz/Art/AppIconMonochromeSource.png";
        private const string Output = Art + "/AndroidIcons";
        public int callbackOrder => 1100;

        [MenuItem("Beltz/Generate launcher icons")]
        public static void Configure()
        {
            Generate();
            AssetDatabase.Refresh();
            var master = AssetDatabase.LoadAssetAtPath<Texture2D>(Master);
            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { master }, IconKind.Application);
#if UNITY_ANDROID
            var background = AssetDatabase.LoadAssetAtPath<Texture2D>(Background);
            var foreground = AssetDatabase.LoadAssetAtPath<Texture2D>(Output + "/Foreground.png");
            foreach (var kind in PlayerSettings.GetSupportedIconKinds(NamedBuildTarget.Android))
            {
                var slots = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, kind);
                foreach (var slot in slots)
                {
                    if (kind == AndroidPlatformIconKind.Adaptive) slot.SetTextures(new[] { background, foreground });
                    else slot.SetTexture(master);
                }
                PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, kind, slots);
            }
#endif
            AssetDatabase.SaveAssets();
            Debug.Log("LAUNCHER_ICONS_READY: legacy, round, adaptive, monochrome; six densities.");
        }

        public void OnPreprocessBuild(BuildReport report) => Configure();

        private static Texture2D Load(string path)
        {
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path))) throw new BuildFailedException("Cannot read icon artwork: " + path);
            texture.wrapMode = TextureWrapMode.Clamp;
            return texture;
        }

        private static void Write(string path, byte[] bytes)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (!File.Exists(path) || !File.ReadAllBytes(path).SequenceEqual(bytes)) File.WriteAllBytes(path, bytes);
        }

        private static byte[] Resize(Texture2D source, int size, bool round = false)
        {
            var output = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float u = (x + .5f) / size, v = (y + .5f) / size;
                    var pixel = source.GetPixelBilinear(u, v);
                    if (round) pixel.a *= Mathf.Clamp01((.5f - Vector2.Distance(new Vector2(u, v), Vector2.one * .5f)) * size);
                    pixels[y * size + x] = pixel;
                }
            output.SetPixels(pixels); output.Apply();
            byte[] bytes = output.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(output);
            return bytes;
        }

        private static Texture2D SafeForeground(Texture2D mask)
        {
            var pixels = mask.GetPixels32();
            int minX = mask.width, minY = mask.height, maxX = 0, maxY = 0;
            for (int y = 0; y < mask.height; y++)
                for (int x = 0; x < mask.width; x++)
                    if (pixels[y * mask.width + x].a > 32)
                    { minX = Math.Min(minX, x); maxX = Math.Max(maxX, x); minY = Math.Min(minY, y); maxY = Math.Max(maxY, y); }
            if (minX > maxX) throw new BuildFailedException("The monochrome icon mask is empty.");
            var center = new Vector2((minX + maxX) * .5f, (minY + maxY) * .5f);
            float radius = 1;
            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                    if (pixels[y * mask.width + x].a > 32)
                        radius = Mathf.Max(radius, Vector2.Distance(new Vector2(x, y), center));
            const int size = 1024;
            // Leave more breathing room after the launcher crops the adaptive layers.
            float scale = size * .235f / radius;
            var foreground = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var output = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    var source = (new Vector2(x + .5f, y + .5f) - Vector2.one * size * .5f) / scale + center;
                    float alpha = source.x < 0 || source.y < 0 || source.x >= mask.width || source.y >= mask.height
                        ? 0 : mask.GetPixelBilinear(source.x / mask.width, source.y / mask.height).a;
                    output[y * size + x] = new Color(1, 1, 1, alpha);
                }
            foreground.SetPixels(output); foreground.Apply();
            return foreground;
        }

        private static void Generate()
        {
            var master = Load(Master);
            var background = Load(Background);
            var mask = Load(Mask);
            var foreground = SafeForeground(mask);
            try
            {
                Write(Output + "/Foreground.png", foreground.EncodeToPNG());
                Write(Output + "/Monochrome.png", foreground.EncodeToPNG());
                Write(Output + "/Store-512.png", Resize(master, 512));
                string[] densities = { "ldpi", "mdpi", "hdpi", "xhdpi", "xxhdpi", "xxxhdpi" };
                int[] legacy = { 36, 48, 72, 96, 144, 192 };
                int[] adaptive = { 81, 108, 162, 216, 324, 432 };
                for (int i = 0; i < densities.Length; i++)
                {
                    string folder = Output + "/res/mipmap-" + densities[i];
                    Write(folder + "/app_icon.png", Resize(master, legacy[i]));
                    Write(folder + "/app_icon_round.png", Resize(master, legacy[i], true));
                    Write(folder + "/ic_launcher_background.png", Resize(background, adaptive[i]));
                    Write(folder + "/ic_launcher_foreground.png", Resize(foreground, adaptive[i]));
                    Write(folder + "/ic_launcher_monochrome.png", Resize(foreground, adaptive[i]));
                }
                foreach (int api in new[] { 26, 33 })
                {
                    string xml = "<adaptive-icon xmlns:android=\"http://schemas.android.com/apk/res/android\">\n" +
                        "  <background android:drawable=\"@mipmap/ic_launcher_background\"/>\n" +
                        "  <foreground android:drawable=\"@mipmap/ic_launcher_foreground\"/>\n" +
                        (api >= 33 ? "  <monochrome android:drawable=\"@mipmap/ic_launcher_monochrome\"/>\n" : "") + "</adaptive-icon>\n";
                    foreach (string name in new[] { "app_icon", "app_icon_round" })
                        Write(Output + "/res/mipmap-anydpi-v" + api + "/" + name + ".xml", System.Text.Encoding.UTF8.GetBytes(xml));
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(master); UnityEngine.Object.DestroyImmediate(background);
                UnityEngine.Object.DestroyImmediate(mask); UnityEngine.Object.DestroyImmediate(foreground);
            }
        }

#if UNITY_ANDROID
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string launcher = Path.Combine(Directory.GetParent(path).FullName, "launcher", "src", "main");
            foreach (string file in Directory.GetFiles(Output + "/res", "*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".meta")) continue;
                string destination = Path.Combine(launcher, "res", Path.GetRelativePath(Output + "/res", file));
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.Copy(file, destination, true);
            }
            string manifestPath = Path.Combine(launcher, "AndroidManifest.xml");
            var manifest = XDocument.Load(manifestPath);
            XNamespace android = "http://schemas.android.com/apk/res/android";
            var application = manifest.Root.Element("application");
            application.SetAttributeValue(android + "icon", "@mipmap/app_icon");
            application.SetAttributeValue(android + "roundIcon", "@mipmap/app_icon_round");
            manifest.Save(manifestPath);
        }
#endif
    }
}

