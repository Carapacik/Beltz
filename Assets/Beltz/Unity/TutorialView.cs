using Beltz.Core;
using Beltz.Content;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beltz.Unity
{
    // An isolated practice board shares game rules and rendering, never campaign state.
    public sealed class TutorialView : MonoBehaviour, IPointerClickHandler
    {
        GameRoot game;
        BoardView board;
        public LevelDefinition Level {get;private set;}
        public PuzzleState State {get;private set;}
        readonly Vector3[] corners=new Vector3[4];
        public void Initialize(GameRoot root,int chapter)
        {
            game=root;Level=Tutorials.Get(chapter);State=new PuzzleState(Level);
            board=new BoardView(game.transform,game.PulleySprite,game.BeltMaterial);
            board.Load(Level);board.SetVisible(isActiveAndEnabled);
        }
        void OnEnable(){if(Level!=null){State=new PuzzleState(Level);board.SetVisible(true);}}
        void OnDisable(){board?.SetVisible(false);}
        void LateUpdate()
        {
            if(board==null)return;
            ((RectTransform)transform).GetWorldCorners(corners);
            var canvas=GetComponentInParent<Canvas>();var camera=canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;
            Vector2 lo=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);
            Vector2 hi=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
            // Expand the supplied area to compensate for the gameplay board margins.
            Vector2 size=(hi-lo)/new Vector2(.82f,.70f);
            board.Layout(Camera.main,new Rect(lo-Vector2.Scale(size,new Vector2(.09f,.12f)),size));
            // A correct practice board runs until the player changes it again.
            board.Tick(State,Mathf.Min(Time.unscaledDeltaTime,.04f),0,PuzzleRules.ErrorMask(Level,State)==0&&!game.ModalOpen);
        }
        public bool TryMove(Move move)
        {
            if(!PuzzleRules.Apply(Level,State,move))return false;
            game.Audio.Unlock();game.Audio.Click();return true;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if(game.ModalOpen||eventData.dragging)return;
            Vector2 world=Camera.main.ScreenToWorldPoint(eventData.position);
            if(board.Pick(world,out var move))TryMove(move);
        }
        void OnDestroy(){board?.Dispose();}
    }
}
