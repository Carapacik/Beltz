using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Beltz.Editor
{
    // Lets local tooling request a build in an already-open editor, without closing
    // the user's window or attempting a second Unity instance for the same project.
    [InitializeOnLoad]
    public static class BuildRequest
    {
        const string Request="Temp/BeltzBuildRequest.txt",Result="Temp/BeltzBuildResult.txt";
        static double nextCheck;
        static BuildRequest(){EditorApplication.update+=Tick;}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<nextCheck)return;
            nextCheck=EditorApplication.timeSinceStartup+1;
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||BuildPipeline.isBuildingPlayer||!File.Exists(Request))return;
            AssetDatabase.Refresh();
            if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            string operation=File.ReadAllText(Request).Trim();File.Delete(Request);
            try
            {
                if(operation=="android")ProjectSetup.BuildAndroid();
                else if(operation=="windows")ProjectSetup.BuildWindows();
                else if(operation=="web")ProjectSetup.BuildWeb();
                else if(operation=="branding")BuildBranding.ApplyCurrent();
                else if(operation=="validate")ProjectSetup.Validate();
                else throw new ArgumentException("Unknown build request: "+operation);
                File.WriteAllText(Result,"SUCCESS "+operation);Debug.Log("BELTZ_REQUEST_OK: "+operation);
            }
            catch(Exception e){File.WriteAllText(Result,"FAILED "+operation+"\n"+e);Debug.LogException(e);}
        }
    }
}
