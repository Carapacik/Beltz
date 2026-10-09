using System;

using System.IO;

using Beltz.Core;

using Beltz.Content;

using Beltz.Unity;

using UnityEditor;

using UnityEditor.SceneManagement;

using UnityEditor.Build.Reporting;

using UnityEngine;

using UnityEngine.Rendering;



namespace Beltz.Editor

{

    public static class ProjectSetup

    {

        const string ScenePath="Assets/Beltz/Scenes/Game.unity";

        [MenuItem("Beltz/Create or refresh game scene")]

        public static void Create()

        {

            Directory.CreateDirectory("Assets/Beltz/Scenes");Directory.CreateDirectory("Assets/Beltz/Art");

            AssetDatabase.Refresh();

            var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/Beltz/Art/Pulley.png");

            if(importer==null)throw new InvalidOperationException("Generated pulley sprite is missing.");

            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;

            importer.spritePixelsPerUnit=256;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;

            importer.maxTextureSize=512;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();

            var audio=(AudioImporter)AssetImporter.GetAtPath("Assets/Beltz/Audio/MainTheme.mp3");

            var sample=audio.defaultSampleSettings;sample.loadType=AudioClipLoadType.CompressedInMemory;sample.compressionFormat=AudioCompressionFormat.Vorbis;sample.quality=.65f;audio.defaultSampleSettings=sample;audio.SaveAndReimport();

            var bg=CreateMaterial("Background","Beltz/Palette");var belt=CreateMaterial("Belt","Beltz/Belt");var frost=CreateMaterial("Frost","Beltz/FrostedUI");

            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);

            var root=new GameObject("Beltz").AddComponent<GameRoot>();root.PulleySprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Beltz/Art/Pulley.png");

            root.Theme=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Beltz/Audio/MainTheme.mp3");root.BackgroundMaterial=bg;root.BeltMaterial=belt;root.FrostMaterial=frost;

            var renderer=new SerializedObject(AssetDatabase.LoadMainAssetAtPath("Assets/Settings/Renderer2D.asset"));

            renderer.FindProperty("m_UseCameraSortingLayersTexture").boolValue=true;

            renderer.FindProperty("m_CameraSortingLayersTextureBound").intValue=0;

            renderer.FindProperty("m_CameraSortingLayerDownsamplingMethod").intValue=3;

            renderer.ApplyModifiedPropertiesWithoutUndo();

            var pipeline=new SerializedObject(AssetDatabase.LoadMainAssetAtPath("Assets/Settings/UniversalRP.asset"));

            pipeline.FindProperty("m_SupportsHDR").boolValue=false;pipeline.FindProperty("m_MSAA").intValue=2;pipeline.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};

            PlayerSettings.companyName="CarapacikSpace";PlayerSettings.productName="Beltz";PlayerSettings.bundleVersion="2.0";

            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.carapacik.beltz");

            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone,"com.carapacik.beltz");

            PlayerSettings.Android.bundleVersionCode=10;

            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel29;PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)37;

            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);

            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;

            PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;PlayerSettings.runInBackground=false;

            PlayerSettings.defaultScreenWidth=540;PlayerSettings.defaultScreenHeight=960;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;

            PlayerSettings.colorSpace=ColorSpace.Linear;

            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.OpenGLES3});

            var settings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);

            var input=settings.FindProperty("activeInputHandler");if(input!=null){input.intValue=1;settings.ApplyModifiedPropertiesWithoutUndo();}

            AssetDatabase.SaveAssets();Validate();Debug.Log("BELTZ_SETUP_OK");

        }

        static Material CreateMaterial(string name,string shader)

        {

            string path=$"Assets/Beltz/Art/{name}.mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);

            if(m==null){m=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(m,path);}return m;

        }

        [MenuItem("Beltz/Validate all levels")]

        public static void Validate()

        {

            int checkedCount=0;

            foreach(var level in LegacyLevels.Create()){Check(level);checkedCount++;}

            for(int i=0;i<CampaignProgress.LevelCount;i++){Check(Campaign.Get(i));checkedCount++;}

            for(int mode=0;mode<4;mode++)for(int seed=0;seed<32;seed++){Check(LevelGenerator.Generate((GameMode)mode,seed%3,seed));checkedCount++;}

            Debug.Log($"BELTZ_VALIDATION_OK: {checkedCount} levels under Unity runtime");

        }

        static void Check(LevelDefinition level)

        {

            var state=new PuzzleState(level);var moves=PuzzleSolver.Solve(level,state);

            if(moves==null||moves.Length==0)throw new InvalidOperationException("Invalid level "+level.Id);

            foreach(var m in moves)if(!PuzzleRules.Apply(level,state,m))throw new InvalidOperationException("Rejected move");

            if(PuzzleRules.ErrorMask(level,state)!=0)throw new InvalidOperationException("Not solved");

        }

        [MenuItem("Beltz/Build Windows preview")]

        public static void BuildWindows(){Build(BuildTarget.StandaloneWindows64,"Builds/Windows/Beltz.exe");}

        [MenuItem("Beltz/Build Windows release")]
        public static void BuildWindowsRelease(){Build(BuildTarget.StandaloneWindows64,"Builds/WindowsRelease/Beltz.exe",BuildOptions.None);}

        [MenuItem("Beltz/Build Web preview")]

        public static void BuildWeb(){Build(BuildTarget.WebGL,"Builds/WebGL/Web");}

        [MenuItem("Beltz/Build Web release")]
        public static void BuildWebRelease()
        {
            // Pages cannot configure Unity-specific Content-Encoding headers.
            PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback=true;
            PlayerSettings.WebGL.threadsSupport=false;
            PlayerSettings.WebGL.nameFilesAsHashes=true;
            Build(BuildTarget.WebGL,"Builds/WebGL/Web",BuildOptions.None);
            File.WriteAllText("Builds/WebGL/Web/.nojekyll",string.Empty);
            // GameCI validates custom builds using this exact success marker.
            Console.WriteLine("Build succeeded!");
        }

        public static void RefreshAndBuildWindows(){Create();BuildWindows();}

        [MenuItem("Beltz/Build Android APK")]

        public static void BuildAndroid(){PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel29;PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)37;AssetDatabase.SaveAssets();EditorUserBuildSettings.buildAppBundle=false;Build(BuildTarget.Android,$"Builds/Android/Beltz-{PlayerSettings.bundleVersion}.apk");}

        public static void BuildAndroidRelease(){PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel29;PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)37;EditorUserBuildSettings.buildAppBundle=false;Build(BuildTarget.Android,$"Builds/Android/Beltz-{PlayerSettings.bundleVersion}-release.apk",BuildOptions.None);}

        static void Build(BuildTarget target,string path,BuildOptions options=BuildOptions.Development)

        {

            if(!File.Exists(ScenePath))Create();Validate();BuildBranding.Apply(target);AssetDatabase.SaveAssets();Directory.CreateDirectory(Path.GetDirectoryName(path));

            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},target=target,locationPathName=path,options=options});

            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed: "+report.summary.result);

            Debug.Log("BELTZ_BUILD_OK: "+path);

        }

    }

}

