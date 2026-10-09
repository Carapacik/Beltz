using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;

namespace Beltz.Editor
{
    // Local signing inputs stay outside the repository and command-line arguments.
    public static class AndroidReleaseBuild
    {
        public static void Build()
        {
            string propertiesPath=Environment.GetEnvironmentVariable("BELTZ_SIGNING_PROPERTIES");
            string keystorePath=Environment.GetEnvironmentVariable("BELTZ_KEYSTORE_PATH");
            if(!File.Exists(propertiesPath)||!File.Exists(keystorePath))
                throw new BuildFailedException("Set BELTZ_SIGNING_PROPERTIES and BELTZ_KEYSTORE_PATH to existing local files.");
            var values=new Dictionary<string,string>();
            foreach(string line in File.ReadAllLines(propertiesPath))
            {
                string entry=line.Trim();
                if(entry.Length==0||entry.StartsWith("#")||entry.StartsWith("!"))continue;
                int separator=entry.IndexOf('=');
                if(separator>0)values[entry.Substring(0,separator).Trim()]=entry.Substring(separator+1).Trim();
            }
            foreach(string key in new[]{"storePassword","keyPassword","keyAlias"})
                if(!values.TryGetValue(key,out string value)||string.IsNullOrEmpty(value))
                    throw new BuildFailedException("Missing signing property: "+key);

            bool previousCustom=PlayerSettings.Android.useCustomKeystore;
            string previousStore=PlayerSettings.Android.keystoreName;
            string previousAlias=PlayerSettings.Android.keyaliasName;
            string previousStorePassword=PlayerSettings.Android.keystorePass;
            string previousKeyPassword=PlayerSettings.Android.keyaliasPass;
            bool previousBundle=EditorUserBuildSettings.buildAppBundle;
            try
            {
                PlayerSettings.Android.useCustomKeystore=true;
                PlayerSettings.Android.keystoreName=Path.GetFullPath(keystorePath);
                PlayerSettings.Android.keyaliasName=values["keyAlias"];
                PlayerSettings.Android.keystorePass=values["storePassword"];
                PlayerSettings.Android.keyaliasPass=values["keyPassword"];
                ProjectSetup.BuildAndroidRelease();
            }
            finally
            {
                PlayerSettings.Android.useCustomKeystore=previousCustom;
                PlayerSettings.Android.keystoreName=previousStore;
                PlayerSettings.Android.keyaliasName=previousAlias;
                PlayerSettings.Android.keystorePass=previousStorePassword;
                PlayerSettings.Android.keyaliasPass=previousKeyPassword;
                EditorUserBuildSettings.buildAppBundle=previousBundle;
                AssetDatabase.SaveAssets();
                values.Clear();
            }
        }
    }
}
