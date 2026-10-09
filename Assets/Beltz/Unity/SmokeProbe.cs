using System.Collections;
using System.IO;
using Beltz.Core;
using UnityEngine;

namespace Beltz.Unity
{
    // Explicit smoke or screenshot runs use in-memory progress, never persisted.
    public sealed class SmokeProbe : MonoBehaviour
    {
        int errors;
        RenderTexture captureTarget;
        void OnEnable(){Application.logMessageReceived+=Log;}
        void OnDisable(){Application.logMessageReceived-=Log;if(captureTarget!=null){captureTarget.Release();Destroy(captureTarget);}}
        void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++;}
        IEnumerator Start()
        {
            var game=GetComponent<GameRoot>();
            string output=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../Artifacts"));Directory.CreateDirectory(output);
            // Hidden windows do not render automatically. Keep a persistent target
            // and request an explicit render for each screenshot.
            captureTarget=new RenderTexture(Screen.width,Screen.height,24,RenderTextureFormat.ARGB32);
            Camera.main.targetTexture=captureTarget;
            var canvas=Object.FindAnyObjectByType<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=Camera.main;canvas.planeDistance=1;canvas.overrideSorting=true;canvas.sortingLayerName="Interface";canvas.sortingOrder=50;
            for(int i=0;i<20;i++)yield return null;
            if(System.Array.IndexOf(System.Environment.GetCommandLineArgs(),"--beltz-screenshots")>=0)
            {
                yield return CaptureReadme(game);
                yield break;
            }
            yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"home.png"));
            game.OpenLevel(CampaignProgress.ChapterSize);
            if(game.ScreenState!=GameScreen.Home)Debug.LogError("Smoke: locked chapter opened");
            game.ShowLevels();yield return null;
            yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"levels-locked.png"));
            game.ShowHome();game.ToggleSettings();
            yield return new WaitForSeconds(.5f);
            if(game.Audio.MusicEnabled&&game.Audio.MusicPitch>.82f)Debug.LogError("Smoke: music pitch did not lower in settings");
            yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"settings-home.png"));
            game.ToggleSettings();yield return new WaitForSeconds(.5f);
            if(game.Audio.MusicPitch<.98f)Debug.LogError("Smoke: music pitch did not recover");
            for(int index=0;index<CampaignProgress.LevelCount;index++)
            {
                game.OpenLevel(index);
                if(index%CampaignProgress.ChapterSize==0)
                {
                    if(game.ScreenState!=GameScreen.Introduction)Debug.LogError("Smoke: missing chapter introduction");
                    yield return null;
                    var tutorial=Object.FindAnyObjectByType<TutorialView>();
                    if(tutorial==null)Debug.LogError("Smoke: missing interactive tutorial");
                    else
                    {
                        int before=PuzzleRules.ErrorMask(tutorial.Level,tutorial.State);
                        foreach(var move in PuzzleRules.LegalMoves(tutorial.Level))
                        {
                            if(!tutorial.TryMove(move)||PuzzleRules.ErrorMask(tutorial.Level,tutorial.State)==before)Debug.LogError("Smoke: tutorial move had no effect");
                            tutorial.TryMove(move);
                            if(PuzzleRules.ErrorMask(tutorial.Level,tutorial.State)!=before)Debug.LogError("Smoke: tutorial move did not reverse");
                        }
                        if(tutorial.Level.Mode!=GameMode.Belts&&tutorial.TryMove(new Move(MoveKind.ToggleBelt,1)))Debug.LogError("Smoke: locked tutorial belt changed");
                        var practiceShaft=GameObject.Find("Shaft 0").transform;
                        var practiceSolution=PuzzleSolver.Solve(tutorial.Level,tutorial.State);
                        foreach(var move in practiceSolution)tutorial.TryMove(move);
                        var practiceRotation=practiceShaft.localRotation;
                        yield return new WaitForSeconds(.1f);
                        if(Quaternion.Angle(practiceRotation,practiceShaft.localRotation)<1)Debug.LogError("Smoke: solved tutorial did not rotate");
                        tutorial.TryMove(practiceSolution[0]);
                        yield return null;
                        practiceRotation=practiceShaft.localRotation;
                        yield return new WaitForSeconds(.1f);
                        if(Quaternion.Angle(practiceRotation,practiceShaft.localRotation)>.01f)Debug.LogError("Smoke: incorrect tutorial kept rotating");
                    }
                    yield return new WaitForEndOfFrame();Capture(Path.Combine(output,$"intro-{index/CampaignProgress.ChapterSize+1}.png"));
                    game.AcceptIntro();
                }
                if(game.ScreenState!=GameScreen.Playing)Debug.LogError("Smoke: level did not open");
                string expectedNumber=$"{index/CampaignProgress.ChapterSize+1}-{index%CampaignProgress.ChapterSize+1}";
                bool numberVisible=false;
                foreach(var label in Object.FindObjectsByType<UnityEngine.UI.Text>())if(label.text==expectedNumber)
                {
                    numberVisible=true;
                    var point=RectTransformUtility.WorldToScreenPoint(canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera,label.rectTransform.position);
                    if(Mathf.Abs(point.x-Screen.width*.5f)>2)
                        Debug.LogError("Smoke: level header is not centered");
                }
                if(!numberVisible)Debug.LogError("Smoke: level header is missing "+expectedNumber);
                yield return null;
                var shaft=GameObject.Find("Shaft 0").transform;
                var idleRotation=shaft.localRotation;
                for(int i=0;i<12;i++)yield return null;
                if(Quaternion.Angle(idleRotation,shaft.localRotation)>.01f)Debug.LogError("Smoke: pulley rotated before solving");
                if(index%CampaignProgress.ChapterSize==0)
                {
                    yield return new WaitForEndOfFrame();Capture(Path.Combine(output,$"mode-{index/CampaignProgress.ChapterSize+1}.png"));
                    if(index==3*CampaignProgress.ChapterSize)
                    {
                        var background=GameObject.Find("Gradient").GetComponent<Renderer>().sharedMaterial;
                        for(int theme=0;theme<16;theme++)
                        {
                            Palette.Apply(background,theme);yield return null;yield return new WaitForEndOfFrame();
                            Capture(Path.Combine(output,$"contrast-{theme:00}.png"));
                        }
                        Palette.Apply(background,game.PaletteIndex);
                    }
                    game.ToggleSettings();yield return new WaitForSeconds(.5f);
                    if(!game.ModalOpen)Debug.LogError("Smoke: settings do not block game input");
                    yield return new WaitForEndOfFrame();Capture(Path.Combine(output,$"settings-{index/CampaignProgress.ChapterSize+1}.png"));
                    game.ToggleSettings();yield return new WaitForSeconds(.5f);
                    if(game.ModalOpen)Debug.LogError("Smoke: settings dismissal left input blocked");
                }
                var solution=PuzzleSolver.Solve(game.Session.Level,game.Session.State);
                if(solution==null){Debug.LogError("Smoke: no solution");continue;}
                foreach(var m in solution){if(!game.TryPlay(m))Debug.LogError("Smoke: rejected legal move");yield return null;}
                if(!game.Session.Solved)Debug.LogError("Smoke: not solved");
                if(!game.Progress.IsCompleted(index))Debug.LogError("Smoke: completion not saved");
                if(index==9)
                {
                    if(!game.Progress.IsUnlocked(30)||!game.Progress.IsUnlocked(10))Debug.LogError("Smoke: ten-level section gate failed");
                    game.Next();if(game.ScreenState!=GameScreen.Introduction||game.IntroChapter!=1)Debug.LogError("Smoke: next section was not offered after ten");
                    game.ShowLevels();yield return null;yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"section-unlocked.png"));
                }
                if(index==0)
                {
                    var completed=canvas.transform.Find("Safe area/Completed").GetComponent<CanvasGroup>();
                    if(completed.gameObject.activeSelf||game.VictoryReady)Debug.LogError("Smoke: victory appeared before rotation");
                    var celebrationStart=shaft.localRotation;
                    yield return new WaitForSeconds(.5f);
                    if(Quaternion.Angle(celebrationStart,shaft.localRotation)<1)Debug.LogError("Smoke: solved pulley did not rotate");
                    if(completed.gameObject.activeSelf||game.VictoryReady)Debug.LogError("Smoke: victory rotation ended too early");
                    yield return new WaitForSeconds(.55f);
                    if(!game.VictoryReady||!completed.gameObject.activeSelf||completed.alpha>=1)Debug.LogError("Smoke: victory did not fade after rotation");
                    yield return new WaitForSeconds(.5f);
                    if(completed.alpha<.99f||completed.transform.localScale.x<.99f)Debug.LogError("Smoke: victory animation did not finish");
                    var dialogRotation=shaft.localRotation;
                    yield return new WaitForSeconds(.1f);
                    if(Quaternion.Angle(dialogRotation,shaft.localRotation)<1)Debug.LogError("Smoke: pulley stopped rotating under victory dialog");
                    yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"completed.png"));
                }
            }
            game.ShowLevels();yield return null;yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"levels-complete.png"));
            game.OpenLevel(0);game.Hint();yield return null;
            if(game.Session.Solved)Debug.LogError("Smoke: replay did not reset completed level");
            var replay=PuzzleSolver.Solve(game.Session.Level,game.Session.State);game.TryPlay(replay[0]);game.Undo();
            if(game.Session.MoveCount!=0)Debug.LogError("Smoke: undo failed");
            game.Restart();
            Debug.Log($"BELTZ_SMOKE_COMPLETE errors={errors}");Application.Quit(errors==0?0:1);
        }
        IEnumerator CaptureReadme(GameRoot game)
        {
            if(Debug.isDebugBuild)Debug.LogError("Screenshots require a non-development build");
            string output=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../Screenshots"));
            Directory.CreateDirectory(output);
            yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"home.png"));
            game.Progress.Complete(0);game.Progress.SeeIntro(0);game.OpenLevel(1);
            yield return null;yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"level-2.png"));
            game.ShowTutorial();
            yield return null;yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"tutorial.png"));
            // The final section has 28 completed levels, one playable and one locked.
            for(int i=0;i<CampaignProgress.LevelCount-2;i++)game.Progress.Complete(i);
            for(int i=0;i<CampaignProgress.LevelCount-1;i++)
                if(!game.Progress.IsUnlocked(i))Debug.LogError("Screenshot: expected unlocked level "+i);
            if(game.Progress.IsUnlocked(CampaignProgress.LevelCount-1))Debug.LogError("Screenshot: final level must be locked");
            game.Progress.SeeIntro(3);game.OpenLevel(90);game.ShowLevels();
            yield return null;yield return new WaitForEndOfFrame();Capture(Path.Combine(output,"levels.png"));
            Debug.Log($"BELTZ_SCREENSHOTS_COMPLETE release={!Debug.isDebugBuild} errors={errors}");
            Application.Quit(errors==0?0:1);
        }
        void Capture(string path)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(Camera.main,
                new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest { destination=captureTarget });
            foreach(var text in Object.FindObjectsByType<UnityEngine.UI.Text>())
            {
                if(text.isActiveAndEnabled&&(text.fontStyle!=FontStyle.Normal||!text.font.name.StartsWith("Montserrat-")||text.fontSize*ReadableText.ScreenScale(text)<13.99f))
                    Debug.LogError("UI_TEXT_TOO_SMALL: "+text.text);
                if(text.isActiveAndEnabled&&text.preferredHeight>text.rectTransform.rect.height+2)
                    Debug.LogError($"UI_TEXT_OVERFLOW: {text.text} needs {text.preferredHeight}, has {text.rectTransform.rect.height}");
            }
            var previous=RenderTexture.active;RenderTexture.active=captureTarget;
            var texture=new Texture2D(captureTarget.width,captureTarget.height,TextureFormat.RGB24,false);
            texture.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);texture.Apply();File.WriteAllBytes(path,texture.EncodeToPNG());
            RenderTexture.active=previous;Object.Destroy(texture);
        }
    }
}
