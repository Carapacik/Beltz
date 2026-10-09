using Beltz.Core;
using UnityEngine;

namespace Beltz.Unity
{
    public sealed class BoardView
    {
        public static readonly Color Active = new Color(1f,.95f,.82f);
        public static readonly Color Locked = new Color(.045f,.055f,.08f);
        readonly Transform root;
        readonly Sprite screw;
        readonly Material ribbonMaterial;
        readonly Material lineMaterial;
        LevelDefinition level;
        BeltView[] belts;
        SpriteRenderer[] shafts;
        LineRenderer[] arrows;
        LineRenderer[] arrowOutlines;
        SpriteRenderer[] rings;
        float[] angularVelocity;
        int hintShaft=-1;float shaftHintUntil;
        public void SetHintShaft(int index,float until){hintShaft=index;shaftHintUntil=until;}
        public float Radius { get; private set; }
        public BoardView(Transform parent, Sprite screw, Material ribbonMaterial)
        {
            root = new GameObject("Board").transform; root.SetParent(parent,false);
            this.screw = screw; this.ribbonMaterial = ribbonMaterial;
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
        }
        public void Load(LevelDefinition value)
        {
            if (belts != null) foreach (var belt in belts) belt.Dispose();
            for (int i=root.childCount-1;i>=0;i--) Object.Destroy(root.GetChild(i).gameObject);
            level=value; float spacing=100;
            hintShaft=-1;
            for(int i=0;i<level.Shafts.Count;i++) for(int j=0;j<i;j++) spacing=Mathf.Min(spacing,Vector2.Distance(Position(i),Position(j)));
            Radius = Mathf.Min(LevelGeometry.ShaftRadius,spacing/3.1f);
            shafts=new SpriteRenderer[level.Shafts.Count]; arrows=new LineRenderer[shafts.Length]; rings=new SpriteRenderer[shafts.Length]; angularVelocity=new float[shafts.Length];
            arrowOutlines=new LineRenderer[shafts.Length];
            for(int i=0;i<shafts.Length;i++)
            {
                var go=new GameObject($"Shaft {i}"); go.transform.SetParent(root,false); go.transform.localPosition=Position(i);
                var s=go.AddComponent<SpriteRenderer>(); s.sprite=screw; s.sortingOrder=5;
                // Source includes transparent margins; scale by the visible disc's approximate diameter.
                float size = Radius*2/screw.bounds.size.x*1.06f; go.transform.localScale=Vector3.one*size;
                s.color=level.Shafts[i].Interactive?Active:new Color(.10f,.12f,.16f); shafts[i]=s;
                var contour=new GameObject("Contrast outline");contour.transform.SetParent(go.transform,false);contour.transform.localScale=Vector3.one*1.075f;
                var outline=contour.AddComponent<SpriteRenderer>();outline.sprite=screw;outline.sortingOrder=4;outline.color=level.Shafts[i].Interactive?Locked:Active;
                var ring=new GameObject("Status"); ring.transform.SetParent(root,false); ring.transform.localPosition=Position(i);
                var sr=ring.AddComponent<SpriteRenderer>(); sr.sprite=screw; sr.sortingOrder=1;
                ring.transform.localScale=Vector3.one*size*1.15f; rings[i]=sr;
                var arrow=new GameObject($"Direction {i}"); arrow.transform.SetParent(root,false);
                var lr=arrow.AddComponent<LineRenderer>(); lr.sharedMaterial=lineMaterial; lr.useWorldSpace=false; lr.widthMultiplier=.032f; lr.positionCount=19; lr.sortingOrder=6;
                lr.startColor=lr.endColor=Locked;lr.sortingOrder=7; arrows[i]=lr;
                var arrowContour=new GameObject("Arrow contrast");arrowContour.transform.SetParent(root,false);
                var al=arrowContour.AddComponent<LineRenderer>();al.sharedMaterial=lineMaterial;al.useWorldSpace=false;al.widthMultiplier=.065f;al.positionCount=19;al.sortingOrder=6;al.startColor=al.endColor=Active;arrowOutlines[i]=al;
                if(level.Mode==GameMode.Belts || (level.Mode==GameMode.Mixed&&!level.Shafts[i].Interactive))
                {
                    var brace=new GameObject("Fixed direction brace");brace.transform.SetParent(root,false);
                    var bar=brace.AddComponent<LineRenderer>();bar.sharedMaterial=lineMaterial;bar.useWorldSpace=false;
                    bar.widthMultiplier=Radius*.32f;bar.positionCount=2;bar.sortingOrder=8;bar.numCapVertices=4;
                    bar.startColor=bar.endColor=new Color(.045f,.055f,.08f);
                    Vector2 offset=new Vector2(Radius*.86f,Radius*.48f);
                    bar.SetPosition(0,Position(i)-offset);bar.SetPosition(1,Position(i)+offset);
                }
            }
            belts=new BeltView[level.Belts.Count];
            for(int i=0;i<belts.Length;i++) {var b=level.Belts[i]; belts[i]=new BeltView(root,Position(b.A),Position(b.B),Radius*.97f,b.Crossed,ribbonMaterial);}
        }
        Vector2 Position(int index) => new Vector2(level.Shafts[index].X,level.Shafts[index].Y);
        public void Layout(Camera camera,Rect safeArea)
        {
            if(level==null)return;
            Vector2 lo=Position(0),hi=lo;
            for(int i=1;i<level.Shafts.Count;i++){lo=Vector2.Min(lo,Position(i));hi=Vector2.Max(hi,Position(i));}
            Vector2 padding=Vector2.one*(Radius*1.65f);lo-=padding;hi+=padding;
            Vector2 screenLo=safeArea.position+Vector2.Scale(safeArea.size,new Vector2(.09f,.12f));
            Vector2 screenHi=safeArea.position+Vector2.Scale(safeArea.size,new Vector2(.91f,.82f));
            Vector2 worldLo=camera.ScreenToWorldPoint(screenLo),worldHi=camera.ScreenToWorldPoint(screenHi);
            float scale=Mathf.Min((worldHi.x-worldLo.x)/(hi.x-lo.x),(worldHi.y-worldLo.y)/(hi.y-lo.y));
            scale=Mathf.Min(scale,1.5f);root.localScale=Vector3.one*scale;
            Vector2 center=(worldLo+worldHi)*.5f-(lo+hi)*.5f*scale;root.localPosition=new Vector3(center.x,center.y,0);
        }
        public void Tick(PuzzleState state,float dt,int hintMask,bool running=false)
        {
            for(int i=0;i<belts.Length;i++)
            {
                bool correct=PuzzleRules.IsBeltCorrect(level,state,i); var b=level.Belts[i];
                Color color=b.Interactive?Active:Locked;
                if((hintMask&(1<<i))!=0) color=Color.Lerp(color,new Color(1,.58f,.10f),.65f+.3f*Mathf.Sin(Time.unscaledTime*5));
                belts[i].SetCrossed(state.Crossed(i)); belts[i].Tick(dt,running&&correct,color,state.Clockwise(b.A)?1:-1,b.Interactive);
            }
            for(int i=0;i<shafts.Length;i++)
            {
                angularVelocity[i]=running?(state.Clockwise(i)?-64:64):0;
                shafts[i].transform.Rotate(0,0,angularVelocity[i]*dt);
                bool correct=true;
                for(int j=0;j<level.Belts.Count;j++) if((level.Belts[j].A==i||level.Belts[j].B==i)&&!PuzzleRules.IsBeltCorrect(level,state,j))correct=false;
                rings[i].color=correct?new Color(.75f,1,.94f,.75f):new Color(.10f,.12f,.2f,.28f);
                if(i==hintShaft&&Time.unscaledTime<shaftHintUntil)rings[i].color=Color.Lerp(new Color(1,.58f,.10f),Color.white,.5f+.5f*Mathf.Sin(Time.unscaledTime*6));
                // Choose the least occupied compass sector, keeping direction cues clear of belts.
                float bestAngle=0,bestScore=-1;
                for(int sector=0;sector<8;sector++)
                {
                    float angle=sector*Mathf.PI/4; Vector2 v=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle)); float score=2;
                    for(int j=0;j<level.Belts.Count;j++)
                    {var b=level.Belts[j]; if(b.A!=i&&b.B!=i)continue; Vector2 d=(Position(b.A==i?b.B:b.A)-Position(i)).normalized; score=Mathf.Min(score,1-Vector2.Dot(v,d));}
                    if(score>bestScore){bestScore=score;bestAngle=angle;}
                }
                float sign=state.Clockwise(i)?-1:1; Vector2 center=Position(i); float r=Radius*1.42f;
                for(int j=0;j<16;j++){float a=bestAngle+sign*(j/15f-.5f)*1.12f;arrows[i].SetPosition(j,center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r);}
                float end=bestAngle+sign*.56f;Vector2 tip=center+new Vector2(Mathf.Cos(end),Mathf.Sin(end))*r;
                Vector2 tangent=new Vector2(-Mathf.Sin(end),Mathf.Cos(end))*sign, radial=new Vector2(Mathf.Cos(end),Mathf.Sin(end));
                arrows[i].SetPosition(16,tip-tangent*.13f+radial*.075f);arrows[i].SetPosition(17,tip);arrows[i].SetPosition(18,tip-tangent*.13f-radial*.075f);
                for(int j=0;j<19;j++)arrowOutlines[i].SetPosition(j,arrows[i].GetPosition(j));
            }
        }
        public bool Pick(Vector2 p,out Move move)
        {
            p=root.InverseTransformPoint(p);
            move=default;
            for(int i=0;i<shafts.Length;i++)
                if(Vector2.Distance(p,Position(i))<Radius*1.35f)
                {
                    if(level.Mode==GameMode.Shafts && PuzzleRules.IncidentMask(level,i)!=0){move=new Move(MoveKind.ToggleLinkedBelts,i);return true;}
                    if((level.Mode==GameMode.Mixed||level.Mode==GameMode.Rotation)&&level.Shafts[i].Interactive){move=new Move(MoveKind.ReverseShaft,i);return true;}
                    return false;
                }
            if(level.Mode==GameMode.Shafts)return false;
            int nearest=-1;float distance=.28f;
            for(int i=0;i<belts.Length;i++)if(level.Belts[i].Interactive){float d=belts[i].Distance(p);if(d<distance){distance=d;nearest=i;}}
            if(nearest<0)return false;move=new Move(MoveKind.ToggleBelt,nearest);return true;
        }
        public void SetVisible(bool visible){if(root!=null)root.gameObject.SetActive(visible);}
        public void Dispose(){if(belts!=null)foreach(var b in belts)b.Dispose();Object.Destroy(lineMaterial);if(root!=null)Object.Destroy(root.gameObject);}
    }
}
