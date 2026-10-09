using Beltz.Core;
using Beltz.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Beltz.Unity
{
    public enum GameScreen { Home, Levels, Introduction, Playing }
    public sealed class GameRoot : MonoBehaviour
    {
        public Sprite PulleySprite;
        public AudioClip Theme;
        public Material BackgroundMaterial, BeltMaterial, FrostMaterial;
        public GameSession Session {get;private set;}
        public GameAudio Audio {get;private set;}
        public CampaignProgress Progress {get;private set;}
        public GameScreen ScreenState {get;private set;}
        public int LevelIndex {get;private set;}
        public int Difficulty {get;private set;}
        public int Optimum {get;private set;}
        public bool ModalOpen {get;set;}
        public string Notice {get;private set;}="";
        public int PaletteIndex {get;private set;}
        public bool SmokeMode {get;private set;}
        public int IntroChapter {get;private set;}
        public bool SettingsOpen => ui!=null&&ui.SettingsOpen;
        public bool VictoryReady {get;private set;}
        float victoryElapsed;
        BoardView board;GameUI ui;Camera cameraView;Transform background;
        Material palette;
        Mesh backgroundMesh;
        int width,height,hintMask,pendingLevel;
        float hintUntil;
        Rect safeArea;
        bool wasSolved;
        BoardInput input;
        const string ProgressKey="Beltz2.Campaign.v2";
        void Start()
        {
            var arguments=System.Environment.GetCommandLineArgs();
            SmokeMode=System.Array.IndexOf(arguments,"--beltz-smoke")>=0||System.Array.IndexOf(arguments,"--beltz-screenshots")>=0;
            if(SmokeMode){Application.runInBackground=true;AudioListener.volume=0;gameObject.AddComponent<SmokeProbe>();}
            Application.targetFrameRate=60;
            cameraView=Camera.main;
            if(cameraView==null){var cam=new GameObject("Camera");cameraView=cam.AddComponent<Camera>();cam.tag="MainCamera";cam.AddComponent<AudioListener>();}
            cameraView.orthographic=true;cameraView.transform.position=new Vector3(0,0,-10);cameraView.clearFlags=CameraClearFlags.SolidColor;
            if(EventSystem.current==null){var e=new GameObject("Input");e.AddComponent<EventSystem>();e.AddComponent<InputSystemUIInputModule>();}
            palette=new Material(BackgroundMaterial);
            var quad=new GameObject("Gradient",typeof(MeshFilter),typeof(MeshRenderer));
            backgroundMesh=new Mesh {name="Gradient quad",vertices=new[] {new Vector3(-.5f,-.5f),new Vector3(-.5f,.5f),new Vector3(.5f,.5f),new Vector3(.5f,-.5f)},
                uv=new[] {Vector2.zero,Vector2.up,Vector2.one,Vector2.right},triangles=new[] {0,1,2,0,2,3}};
            quad.GetComponent<MeshFilter>().sharedMesh=backgroundMesh;
            quad.transform.position=new Vector3(0,0,5);quad.GetComponent<MeshRenderer>().sharedMaterial=palette;background=quad.transform;
            Audio=new GameAudio(gameObject,Theme);board=new BoardView(transform,PulleySprite,BeltMaterial);
            input = new BoardInput(cameraView, Audio.Unlock, OnBoardTap);
            int intros=SmokeMode?0:PlayerPrefs.GetInt("Beltz2.Intros",0);
            if(SmokeMode)Progress=new CampaignProgress();
            else if(PlayerPrefs.HasKey(ProgressKey))Progress=CampaignProgress.Import(PlayerPrefs.GetString(ProgressKey),intros);
            else {uint.TryParse(PlayerPrefs.GetString("Beltz2.Campaign.v1","0"),out uint previous);Progress=CampaignProgress.FromLegacy(previous,intros);}
            int index=0,difficulty=0;var saved=SmokeMode?null:SaveStore.Load(out index,out difficulty);
            LevelIndex=Progress.ContinueIndex;
            // Only restore a campaign session whose data belongs to this catalog version.
            if(saved!=null&&Progress.IsUnlocked(index)&&saved.Level.Id==Campaign.Get(index).Id){Session=saved;LevelIndex=index;}
            else Session=new GameSession(Campaign.Get(LevelIndex));
            ui=new GameUI(this);SetBoard();ShowHome();Resize();
        }
        void Update()
        {
            if(Session==null)return;
            if(width!=Screen.width||height!=Screen.height||safeArea!=Screen.safeArea)Resize();
            bool celebrating=ScreenState==GameScreen.Playing&&Session.Solved&&!VictoryReady&&!ModalOpen;
            float rotationTime=celebrating?Mathf.Min(Time.unscaledDeltaTime,1-victoryElapsed):0;
            board.Tick(Session.State,Mathf.Min(Time.unscaledDeltaTime,.04f),Time.unscaledTime<hintUntil?hintMask:0,ScreenState==GameScreen.Playing&&Session.Solved&&!ModalOpen);
            if(celebrating){victoryElapsed+=rotationTime;if(victoryElapsed>=1){VictoryReady=true;ui.Refresh();}}
            ui.Tick();Audio.Tick(ui.SettingsOpen,Time.unscaledDeltaTime);
            input.Tick(ScreenState == GameScreen.Playing && !Session.Solved && !ModalOpen);
            if(Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame)
            {if(SettingsOpen)ui.ToggleSettings();else if(ScreenState==GameScreen.Playing)ui.ToggleSettings();else ShowHome();}
        }
        public bool TryPlay(Move move)
        {
            if(ScreenState!=GameScreen.Playing||ModalOpen||!Session.Play(move))return false;
            Audio.Click();Changed();return true;
        }
        void OnBoardTap(Vector2 world)
        {
            if (board.Pick(world, out var move)) TryPlay(move);
        }
        void Resize()
        {
            width=Screen.width;height=Screen.height;safeArea=Screen.safeArea;
            float aspect=width/(float)Mathf.Max(height,1);cameraView.orthographicSize=Mathf.Max(6.4f,3.65f/aspect);
            background.localScale=new Vector3(cameraView.orthographicSize*2*aspect,cameraView.orthographicSize*2,1);board.Layout(cameraView,safeArea);
        }
        void SetBoard()
        {
            board.Load(Session.Level);board.Tick(Session.State,0,0);Optimum=PuzzleSolver.Solve(Session.Level,new PuzzleState(Session.Level))?.Length??-1;
            PaletteIndex=LevelIndex;Palette.Apply(palette,PaletteIndex);Resize();
            Notice="";hintMask=0;wasSolved=Session.Solved;victoryElapsed=0;VictoryReady=false;ui.Refresh();Save();
        }
        void Changed()
        {
            hintMask=0;Notice="";board.SetHintShaft(-1,0);
            if(Session.Solved&&!wasSolved){victoryElapsed=0;VictoryReady=false;Audio.Win();Progress.Complete(LevelIndex);}
            if(!Session.Solved){victoryElapsed=0;VictoryReady=false;}
            wasSolved=Session.Solved;ui.Refresh();Save();
        }
        void ScreenTo(GameScreen screen)
        {
            input?.Cancel();ScreenState=screen;board.SetVisible(screen==GameScreen.Playing);ui.CloseSettings();ui.Refresh();
        }
        public void ShowHome(){ScreenTo(GameScreen.Home);Save();}
        public void ShowLevels(){Audio.Click();ui.SelectChapter(LevelIndex/CampaignProgress.ChapterSize);ScreenTo(GameScreen.Levels);}
        public void Continue()
        {
            int index=Session.Solved?Progress.ContinueIndex:LevelIndex;
            OpenLevel(index);
        }
        public void OpenLevel(int index)
        {
            if(!Progress.IsUnlocked(index))return;
            Audio.Click();pendingLevel=index;
            if(!Progress.HasSeenIntro(index/CampaignProgress.ChapterSize))
            {IntroChapter=index/CampaignProgress.ChapterSize;ScreenTo(GameScreen.Introduction);return;}
            BeginLevel(index);
        }
        void BeginLevel(int index)
        {
            if(!Progress.IsUnlocked(index))return;
            if(LevelIndex!=index||Session.Solved){LevelIndex=index;Session=new GameSession(Campaign.Get(index));SetBoard();}
            ScreenTo(GameScreen.Playing);Save();
        }
        public void AcceptIntro(){Progress.SeeIntro(IntroChapter);BeginLevel(pendingLevel);}
        public void ShowTutorial(){pendingLevel=LevelIndex;IntroChapter=LevelIndex/CampaignProgress.ChapterSize;ScreenTo(GameScreen.Introduction);}
        public void Next()
        {
            int chapter=LevelIndex/CampaignProgress.ChapterSize,local=LevelIndex%CampaignProgress.ChapterSize;
            if(local==CampaignProgress.UnlockThreshold-1&&Progress.IsChapterUnlocked(chapter+1))OpenLevel((chapter+1)*CampaignProgress.ChapterSize);
            else if(local<CampaignProgress.ChapterSize-1)OpenLevel(LevelIndex+1);
            else if(Progress.CompletedCount<CampaignProgress.LevelCount)OpenLevel(Progress.ContinueIndex);
            else ShowLevels();
        }
        public void Undo(){Audio.Click();if(Session.Undo())Changed();}
        public void Restart(){Audio.Click();Session.Restart();Changed();ui.CloseSettings();}
        public void Hint()
        {
            Audio.Click();var result=PuzzleSolver.Solve(Session.Level,Session.State);
            if(result==null||result.Length==0)return;
            var move=result[0];hintMask=PuzzleRules.MoveMask(Session.Level,move);hintUntil=Time.unscaledTime+3;
            board.SetHintShaft(move.Kind==MoveKind.ToggleBelt?-1:move.Index,hintUntil);
            Notice=move.Kind==MoveKind.ToggleBelt?"Tap the highlighted belt":"Tap the highlighted pulley";ui.ShowHint();
        }
        public void ToggleSettings(){ui.ToggleSettings();}
        void Save()
        {
            if(Session==null||SmokeMode)return;
            SaveStore.Save(Session,LevelIndex,Difficulty);PlayerPrefs.SetString(ProgressKey,Progress.Export());
            PlayerPrefs.SetInt("Beltz2.Intros",Progress.IntroSeen);PlayerPrefs.Save();
        }
        void OnApplicationPause(bool paused){if(paused){input?.Cancel();Save();}}
        void OnApplicationFocus(bool focus){if(!focus)input?.Cancel();}
        void OnApplicationQuit(){Save();}
        void OnDestroy(){board?.Dispose();Audio?.Dispose();ui?.Dispose();if(palette!=null)Destroy(palette);if(backgroundMesh!=null)Destroy(backgroundMesh);}
    }
}
