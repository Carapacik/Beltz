using System;
using Beltz.Core;
using UnityEngine;
using UnityEngine.UI;
using static Beltz.Unity.UITheme;

namespace Beltz.Unity
{
    public sealed class GameUI
    {
        readonly GameRoot game;
        readonly GameObject root;
        readonly RectTransform safe,home,levels,intro,hud,nav,sheet;
        readonly UIFactory ui;
        readonly GameObject settings,win;
        readonly CanvasGroup settingsGroup,winGroup;
        readonly RectTransform winPanel;
        readonly RectTransform[] ornaments=new RectTransform[3];

        float winProgress;bool winVisible;
        readonly Image shade,musicTrack,soundTrack;
        readonly RectTransform musicKnob,soundKnob;
        readonly Text levelNumber,introNumber,introTitle,introBody,result,hint,levelSummary;
        readonly Text[] chapterStatus=new Text[4];
        readonly Image[] chapterTabs=new Image[4];
        readonly Text sectionHeading,unlockHint;
        int selectedChapter;
        float musicPosition,soundPosition;
        readonly Button[] levelButtons=new Button[CampaignProgress.ChapterSize];
        readonly Image[] levelImages=new Image[CampaignProgress.ChapterSize];
        readonly Text[] levelLabels=new Text[CampaignProgress.ChapterSize];
        readonly UIIcon[] levelLocks=new UIIcon[CampaignProgress.ChapterSize];
        readonly GameObject[] tutorialDemos=new GameObject[4];
        readonly Button undo,restart,help;
        readonly Image homeTab,levelsTab;
        Rect lastSafe;
        bool settingsOpen;
        float sheetProgress,hintUntil;
        public bool SettingsOpen=>settingsOpen;
        static readonly string[] Names={"Belts","Connections","Rotation","Mixed"};
        static readonly string[] Lessons={
            "Tap a belt to switch between open and crossed. Every belt is available.\n\nBraces hold the pulleys in place: tapping a pulley does nothing.",
            "Tap a pulley to switch all its connected colored belts at once. The pulley keeps its direction.\n\nBlack belts stay fixed. Tapping a belt directly does nothing in this section.",
            "Tap any pulley to reverse only its own direction. Its belts keep their shape.\n\nTap a colored belt to switch it separately. Black belts stay fixed.",
            "Tap colored pulleys and belts. A pulley reverses its direction; a belt switches its shape.\n\nBraced pulleys and black belts stay fixed. Solve the board using the available pieces."};
        public GameUI(GameRoot game)
        {
            this.game = game;
            ui = new UIFactory(game.FrostMaterial);
            root=new GameObject("Interface",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            var canvas=root.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingLayerName="Interface";canvas.sortingOrder=50;canvas.additionalShaderChannels|=AdditionalCanvasShaderChannels.TexCoord1|AdditionalCanvasShaderChannels.TexCoord2;
            var scaler=root.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1080,1920);scaler.matchWidthOrHeight=.5f;
            safe=ui.Rect("Safe area",root.transform,0,0,1,1);
            home=ui.Rect("Home",safe,0,0,1,1);levels=ui.Rect("Levels",safe,0,0,1,1);intro=ui.Rect("Chapter introduction",safe,0,0,1,1);hud=ui.Rect("Play HUD",safe,0,0,1,1);
            // Keep the central title and play action clear of decorative pulleys.
            ornaments[0]=Decoration(home,.13f,.76f,110);
            ornaments[1]=Decoration(home,.84f,.63f,80);
            ornaments[2]=Decoration(home,.19f,.27f,70);
            ui.Label(home,"BELTZ",.12f,.51f,.88f,.65f,116,true,White,TextAnchor.MiddleCenter);
            ui.IconButton(home,"settings",.815f,.906f,.935f,.974f,ToggleSettings);
            var play=ui.Rect("Play",home,.5f,.405f,.5f,.405f);play.sizeDelta=new Vector2(164,164);
            var playFill=ui.Fill(play,Accent,true);playFill.type=Image.Type.Simple;
            ui.Icon(play,"play",.12f,.12f,.88f,.88f);ui.Clickable(play,game.Continue);
            nav=ui.Panel("Navigation",safe,.19f,.048f,.81f,.113f);
            var a=ui.TextButton(nav,"Play",.015f,.08f,.49f,.92f,()=>{game.Audio.Click();game.ShowHome();});homeTab=a.GetComponent<Image>();
            var b=ui.TextButton(nav,"Levels",.51f,.08f,.985f,.92f,game.ShowLevels);levelsTab=b.GetComponent<Image>();
            ui.Label(levels,"Levels",.08f,.883f,.80f,.97f,66,true);
            ui.IconButton(levels,"settings",.815f,.906f,.935f,.974f,ToggleSettings);
            levelSummary=ui.Label(levels,"",.08f,.84f,.92f,.887f,27,false,Muted);
            var sections=ui.Panel("Section tabs",levels,.06f,.766f,.94f,.829f);
            for(int c=0;c<CampaignProgress.ChapterCount;c++)
            {
                int chapter=c;var tab=ui.TextButton(sections,"",.01f+c*.248f,.08f,.24f+c*.248f,.92f,()=>{game.Audio.Click();SelectChapter(chapter);});
                chapterTabs[c]=tab.GetComponent<Image>();chapterStatus[c]=tab.GetComponentInChildren<Text>();chapterStatus[c].fontSize=23;
            }
            sectionHeading=ui.Label(levels,"",.08f,.71f,.92f,.762f,36,true);
            var card=ui.Panel("Section levels",levels,.06f,.20f,.94f,.70f);
            for(int j=0;j<CampaignProgress.ChapterSize;j++)
            {
                int local=j;float x=.06f+(j%5)*.181f,top=.95f-(j/5)*.158f;
                var button=ui.TextButton(card,(j+1).ToString("00"),x,top-.13f,x+.155f,top,()=>game.OpenLevel(selectedChapter*CampaignProgress.ChapterSize+local));
                levelButtons[j]=button;levelImages[j]=button.GetComponent<Image>();levelLabels[j]=button.GetComponentInChildren<Text>();levelLabels[j].fontSize=34;levelLabels[j].font=ui.BoldFont;
                levelLocks[j]=ui.Icon(button.transform,"lock",.28f,.22f,.72f,.78f);levelLocks[j].color=Muted;
            }
            unlockHint=ui.Label(levels,"",.08f,.126f,.92f,.19f,25,false,Muted,TextAnchor.MiddleCenter);
            ui.IconButton(intro,"back",.06f,.906f,.18f,.974f,game.ShowLevels);
            introNumber=ui.Label(intro,"",.23f,.918f,.92f,.964f,26,false,White,TextAnchor.MiddleRight);
            introTitle=ui.Label(intro,"",.08f,.80f,.92f,.90f,76,true,White,TextAnchor.MiddleCenter);
            var demo=ui.Rect("Demonstration",intro,.02f,.48f,.98f,.80f);
            for(int c=0;c<4;c++)
            {
                var d=ui.Rect("Interactive tutorial "+c,demo,0,0,1,1);
                d.gameObject.AddComponent<Image>().color=Color.clear;
                d.gameObject.AddComponent<TutorialView>().Initialize(game,c);
                tutorialDemos[c]=d.gameObject;d.gameObject.SetActive(false);
            }
            introBody=ui.Label(intro,"",.085f,.235f,.915f,.485f,36,true,White,TextAnchor.UpperCenter);
            ui.Label(intro,"Open belt: same direction\nCrossed belt: opposite directions",.08f,.13f,.92f,.225f,31,true,White,TextAnchor.MiddleCenter);
            ui.TextButton(intro,"Let's play",.21f,.045f,.79f,.113f,game.AcceptIntro,true);
            // On the board there are only navigation, hint and settings controls.
            ui.IconButton(hud,"home",.06f,.906f,.18f,.974f,game.ShowHome);
            levelNumber=ui.Label(hud,"",.30f,.91f,.70f,.972f,42,true,White,TextAnchor.MiddleCenter);
            var tools=ui.Panel("Tools",hud,.67f,.903f,.94f,.977f);
            ui.IconButton(tools,"hint",.015f,.04f,.485f,.96f,game.Hint,false);
            ui.IconButton(tools,"settings",.515f,.04f,.985f,.96f,ToggleSettings,false);
            hint=ui.Label(hud,"",.10f,.825f,.90f,.89f,27,false,Ink,TextAnchor.MiddleCenter);hint.gameObject.SetActive(false);
            var solved=ui.Panel("Completed",safe,.075f,.295f,.925f,.68f);win=solved.gameObject;winPanel=solved;winGroup=win.AddComponent<CanvasGroup>();winGroup.alpha=0;
            ui.Icon(solved,"check",.41f,.74f,.59f,.94f).color=Accent;
            var victoryTitle=ui.Label(solved,"All in motion",.07f,.54f,.93f,.72f,50,true,Ink,TextAnchor.MiddleCenter);
            var victoryOutline=victoryTitle.gameObject.AddComponent<Outline>();victoryOutline.effectColor=Color.black;victoryOutline.effectDistance=new Vector2(1.5f,-1.5f);
            result=ui.Label(solved,"",.07f,.405f,.93f,.525f,36,true,new Color(.23f,.14f,.36f),TextAnchor.MiddleCenter);
            ui.TextButton(solved,"Continue",.08f,.205f,.92f,.365f,game.Next,true);
            var chooseLevel=ui.TextButton(solved,"Choose a level",.20f,.055f,.80f,.175f,game.ShowLevels);
            chooseLevel.GetComponent<Image>().color=new Color(1,1,1,.48f);
            chooseLevel.GetComponentInChildren<Text>().color=Accent;
            var overlay=ui.Rect("Settings overlay",root.transform,0,0,1,1);settings=overlay.gameObject;
            settingsGroup=settings.AddComponent<CanvasGroup>();settingsGroup.alpha=0;
            var shadeRect=ui.Rect("Dismiss backdrop",overlay,0,0,1,1);shade=shadeRect.gameObject.AddComponent<Image>();shade.color=new Color(.02f,.08f,.12f,0);
            shadeRect.gameObject.AddComponent<Button>().onClick.AddListener(ToggleSettings);
            // Safe-area anchored top sheet occupies about half the usable display.
            var sheetSafe=ui.Rect("Sheet safe area",overlay,0,0,1,1);sheet=ui.Panel("Settings sheet",sheetSafe,.025f,.44f,.975f,.99f);
            ui.Label(sheet,"Settings",.065f,.835f,.79f,.96f,48,true);
            ui.IconButton(sheet,"close",.825f,.84f,.955f,.965f,ToggleSettings);
            var group=ui.Rect("Audio rows",sheet,.045f,.44f,.955f,.81f);
            var musicRow=ui.Row(group,"music","Music",.51f,.99f,()=>{game.Audio.ToggleMusic();Refresh();});
            var soundRow=ui.Row(group,"sound","Sound",.01f,.49f,()=>{game.Audio.ToggleSound();Refresh();});
            musicTrack=ui.Switch(musicRow,out musicKnob);soundTrack=ui.Switch(soundRow,out soundKnob);
            var divider=ui.Rect("Separator",group,.16f,.498f,.96f,.502f).gameObject.AddComponent<Image>();divider.color=new Color(.12f,.22f,.26f,.10f);divider.raycastTarget=false;

            var actions=ui.Rect("Game actions",sheet,.05f,.235f,.95f,.36f);
            undo=ui.TextButton(actions,"Undo",0,0,.31f,1,game.Undo);
            restart=ui.TextButton(actions,"Restart",.345f,0,.655f,1,game.Restart);
            help=ui.TextButton(actions,"How to play",.69f,0,1,1,game.ShowTutorial);
            ui.TextButton(sheet,"Main menu",.18f,.06f,.82f,.185f,game.ShowHome);
            musicPosition=game.Audio.MusicEnabled?1:0;soundPosition=game.Audio.SoundEnabled?1:0;
            settings.SetActive(false);win.SetActive(false);
        }
        void AnimateSwitch(Image track,RectTransform knob,bool on,ref float position)
        {
            position=Mathf.Lerp(position,on?1:0,1-Mathf.Exp(-Time.unscaledDeltaTime*18));
            track.color=Color.Lerp(new Color(.38f,.43f,.48f,.27f),new Color(.20f,.78f,.35f),position);
            knob.anchoredPosition=new Vector2(47+50*position,0);
            knob.sizeDelta=new Vector2(82+4*Mathf.Sin(position*Mathf.PI),60);
        }
        public void SelectChapter(int chapter){selectedChapter=Mathf.Clamp(chapter,0,CampaignProgress.ChapterCount-1);Refresh();}
        RectTransform Decoration(Transform parent,float x,float y,float size)
        {
            var r=ui.Rect("Decorative pulley",parent,x,y,x,y);r.sizeDelta=new Vector2(size,size);
            var image=r.gameObject.AddComponent<Image>();image.sprite=game.PulleySprite;image.preserveAspect=true;
            image.color=new Color(BoardView.Active.r,BoardView.Active.g,BoardView.Active.b,.55f);image.raycastTarget=false;return r;
        }
        public void ShowHint(){hint.text=game.Notice;hintUntil=Time.unscaledTime+3;hint.gameObject.SetActive(true);}
        public void ToggleSettings(){settingsOpen=!settingsOpen;settings.SetActive(true);game.ModalOpen=true;game.Audio.Click();Refresh();}
        public void CloseSettings(){settingsOpen=false;RefreshModal();}
        void RefreshModal(){game.ModalOpen=settingsOpen||sheetProgress>.001f;}
        public void Tick()
        {
            Rect area=Screen.safeArea;
            if(area!=lastSafe&&Screen.width>0&&Screen.height>0)
            {
                lastSafe=area;safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);
                var parent=(RectTransform)sheet.parent;parent.anchorMin=safe.anchorMin;parent.anchorMax=safe.anchorMax;
            }
            AnimateSwitch(musicTrack,musicKnob,game.Audio.MusicEnabled,ref musicPosition);
            AnimateSwitch(soundTrack,soundKnob,game.Audio.SoundEnabled,ref soundPosition);
            sheetProgress=Mathf.MoveTowards(sheetProgress,settingsOpen?1:0,Time.unscaledDeltaTime/ .36f);
            float eased=1-Mathf.Pow(1-sheetProgress,3);
            sheet.anchoredPosition=new Vector2(0,(1-eased)*(sheet.rect.height+Screen.height*.1f));
            settingsGroup.alpha=Mathf.Clamp01(sheetProgress*4);shade.color=new Color(.025f,.08f,.12f,.23f*eased);
            settingsGroup.interactable=settingsOpen;settingsGroup.blocksRaycasts=sheetProgress>0||settingsOpen;
            RefreshModal();hud.gameObject.SetActive(game.ScreenState==GameScreen.Playing&&sheetProgress<=0&&!settingsOpen);if(!settingsOpen&&sheetProgress<=0)settings.SetActive(false);
            if(hint.gameObject.activeSelf&&Time.unscaledTime>hintUntil)hint.gameObject.SetActive(false);
            if(home.gameObject.activeSelf)for(int i=0;i<ornaments.Length;i++)ornaments[i].localEulerAngles=new Vector3(0,0,Time.unscaledTime*(i%2==0?12:-9));

            if(winVisible)
            {
                winProgress=Mathf.MoveTowards(winProgress,1,Time.unscaledDeltaTime/.42f);
                float t=1-Mathf.Pow(1-winProgress,3);
                winGroup.alpha=Mathf.SmoothStep(0,1,winProgress);
                winPanel.localScale=Vector3.one*Mathf.Lerp(.86f,1,t);
                winGroup.interactable=winProgress>=.9f;
            }
        }
        public void Refresh()
        {
            if(game.Session==null)return;
            var screen=game.ScreenState;home.gameObject.SetActive(screen==GameScreen.Home);levels.gameObject.SetActive(screen==GameScreen.Levels);intro.gameObject.SetActive(screen==GameScreen.Introduction);hud.gameObject.SetActive(screen==GameScreen.Playing);
            nav.gameObject.SetActive(screen==GameScreen.Home||screen==GameScreen.Levels);
            homeTab.color=screen==GameScreen.Home?Accent:Color.clear;levelsTab.color=screen==GameScreen.Levels?Accent:Color.clear;
            int completed=game.Progress.CompletedCount;
            levelNumber.text=$"{game.LevelIndex/CampaignProgress.ChapterSize+1}-{game.LevelIndex%CampaignProgress.ChapterSize+1}";
            levelSummary.text=$"{completed} / {CampaignProgress.LevelCount} complete";
            for(int c=0;c<CampaignProgress.ChapterCount;c++)
            {
                bool available=game.Progress.IsChapterUnlocked(c);
                chapterStatus[c].text=$"{c+1} - {game.Progress.ChapterCompleted(c)}/{CampaignProgress.ChapterSize}";
                chapterStatus[c].color=selectedChapter==c?White:available?Ink:Muted;
                chapterTabs[c].color=selectedChapter==c?Accent:Color.clear;
            }
            sectionHeading.text=$"0{selectedChapter+1}  {Names[selectedChapter]}";
            bool open=game.Progress.IsChapterUnlocked(selectedChapter);
            int doneCount=game.Progress.ChapterCompleted(selectedChapter);
            unlockHint.text=!open?$"Complete 10 levels in {Names[selectedChapter-1]} to unlock.":selectedChapter==3?"Replay any completed level.":doneCount<10?$"{10-doneCount} more to unlock {Names[selectedChapter+1]}.":$"{Names[selectedChapter+1]} is open. Keep exploring this section too.";
            for(int j=0;j<CampaignProgress.ChapterSize;j++)
            {
                int i=selectedChapter*CampaignProgress.ChapterSize+j;bool unlocked=game.Progress.IsUnlocked(i),done=game.Progress.IsCompleted(i);
                levelButtons[j].interactable=unlocked;
                levelImages[j].color=unlocked&&done?Accent:(unlocked?new Color(1,1,1,.20f):new Color(1,1,1,.08f));
                levelLabels[j].color=done?White:Ink;levelLabels[j].gameObject.SetActive(unlocked);levelLocks[j].gameObject.SetActive(!unlocked);
            }
            introNumber.text=$"SECTION {game.IntroChapter+1} OF 4";introTitle.text=Names[game.IntroChapter];introBody.text=Lessons[game.IntroChapter];
            for(int i=0;i<4;i++)tutorialDemos[i].SetActive(i==game.IntroChapter);
            bool playing=screen==GameScreen.Playing;undo.interactable=playing&&game.Session.MoveCount>0;restart.interactable=playing;help.interactable=playing;
            bool showWin=playing&&game.Session.Solved&&game.VictoryReady&&!settingsOpen;
            if(showWin&&!winVisible){winProgress=0;winGroup.alpha=0;winPanel.localScale=Vector3.one*.86f;winGroup.interactable=false;}
            winVisible=showWin;win.SetActive(showWin);
            result.text=completed==CampaignProgress.LevelCount?"All 120 levels complete!":$"Level {game.LevelIndex/CampaignProgress.ChapterSize+1}-{game.LevelIndex%CampaignProgress.ChapterSize+1} / {game.Session.MoveCount} {(game.Session.MoveCount==1?"move":"moves")}";
        }
        public void Dispose(){UnityEngine.Object.Destroy(root);ui.Dispose();}
    }

}
