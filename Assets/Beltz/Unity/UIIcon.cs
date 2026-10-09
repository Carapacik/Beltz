using UnityEngine;
using UnityEngine.UI;

namespace Beltz.Unity
{
    // Shared vector icons with no icon font or plugin.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class UIIcon : MaskableGraphic
    {
        string kind;
        public string Kind { get=>kind; set { kind=value;SetVerticesDirty(); } }
        public override Texture mainTexture=>Texture2D.whiteTexture;
        VertexHelper mesh;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();mesh=vh;
            switch(Kind)
            {
                case "back": Path(62,22,34,50,62,78);break;
                case "close": Line(29,29,71,71);Line(29,71,71,29);break;
                case "play":Triangle(new Vector2(-13,26),new Vector2(25,0),new Vector2(-13,-26));break;
                case "next":Line(25,50,75,50);Path(53,28,75,50,53,72);break;
                case "check":Path(23,49,43,69,77,31);break;
                case "home":Path(21,45,50,20,79,45,79,79,59,79,59,57,41,57,41,79,21,79,21,45);break;
                case "levels":for(int i=0;i<4;i++){float x=22+(i%2)*34,y=22+(i/2)*34;Path(x,y,x+22,y,x+22,y+22,x,y+22,x,y);}break;
                case "settings":Pulley();break;
                case "hint":Arc(50,40,23,0,360);Line(38,61,40,72);Line(62,61,60,72);Line(40,73,60,73);Line(44,82,56,82);Line(50,4,50,10);Line(12,23,18,26);Line(82,26,88,23);break;
                case "lock":Path(32,43,32,31,37,22,50,18,63,22,68,31,68,43);Path(25,43,75,43,75,80,25,80,25,43);Line(50,56,50,67);break;
                case "undo":Arc(51,54,28,-130,120);Path(24,21,24,43,46,43);break;
                case "restart":Arc(50,50,29,-130,160);Path(21,18,23,41,44,37);break;
                case "music":Path(35,68,35,28,70,20,70,60);Circle(26,70,9);Circle(61,62,9);Line(36,40,69,32);break;
                case "sound":Path(19,39,34,39,51,24,51,76,34,61,19,61,19,39);Arc(51,50,21,-55,55);Arc(51,50,34,-55,55);break;
                case "palette":Circle(50,50,31);Circle(38,34,3);Circle(60,33,3);Circle(69,53,3);Circle(31,54,3);break;
                case "help":Arc(50,37,16,-180,80);Path(53,53,49,59,49,63);Circle(49,76,2);Circle(50,50,38);break;
            }
        }
        void Path(params float[] p){for(int i=2;i<p.Length;i+=2)Line(p[i-2],p[i-1],p[i],p[i+1]);}
        void Triangle(Vector2 a,Vector2 b,Vector2 c)
        {
            var rect=rectTransform.rect;float scale=Mathf.Min(rect.width,rect.height)/100;int v=mesh.currentVertCount;
            mesh.AddVert(rect.center+a*scale,color,Vector2.zero);mesh.AddVert(rect.center+b*scale,color,Vector2.zero);mesh.AddVert(rect.center+c*scale,color,Vector2.zero);mesh.AddTriangle(v,v+1,v+2);
        }
        void Pulley()
        {
            // Broad teeth and an open hub echo the app icon. The hole is actual
            // empty geometry so the frosted button remains visible through it.
            const int teeth=14,stepsPerTooth=16,count=teeth*stepsPerTooth;
            const float hole=12;
            var rect=rectTransform.rect;float scale=Mathf.Min(rect.width,rect.height)/100;
            if(scale<=0)return;
            float feather=.65f/scale;
            var transparent=color;transparent.a=0;
            for(int i=0;i<count;i++)
            {
                float a=i*Mathf.PI*2/count-Mathf.PI*.5f,b=(i+1)*Mathf.PI*2/count-Mathf.PI*.5f;
                var da=new Vector2(Mathf.Cos(a),Mathf.Sin(a));var db=new Vector2(Mathf.Cos(b),Mathf.Sin(b));
                float ra=ToothRadius(i,stepsPerTooth),rb=ToothRadius(i+1,stepsPerTooth);
                Quad(da*hole,da*ra,db*rb,db*hole,color,color);
                Quad(da*ra,da*(ra+feather),db*(rb+feather),db*rb,color,transparent);
                Quad(da*(hole-feather),da*hole,db*hole,db*(hole-feather),transparent,color);
            }
        }
        static float ToothRadius(int step,int stepsPerTooth)
        {
            float phase=Mathf.Abs(Mathf.Repeat(step/(float)stepsPerTooth+.5f,1)-.5f);
            return Mathf.Lerp(39,31,Mathf.SmoothStep(0,1,Mathf.InverseLerp(.25f,.39f,phase)));
        }
        void Quad(Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color inner,Color outer)
        {
            var rect=rectTransform.rect;float scale=Mathf.Min(rect.width,rect.height)/100;int v=mesh.currentVertCount;
            mesh.AddVert(rect.center+a*scale,inner,Vector2.zero);mesh.AddVert(rect.center+b*scale,outer,Vector2.zero);
            mesh.AddVert(rect.center+c*scale,outer,Vector2.zero);mesh.AddVert(rect.center+d*scale,inner,Vector2.zero);
            mesh.AddTriangle(v,v+1,v+2);mesh.AddTriangle(v,v+2,v+3);
        }
        void Circle(float x,float y,float r){Arc(x,y,r,0,360);}
        void Arc(float x,float y,float r,float from,float to){int n=Mathf.CeilToInt(Mathf.Abs(to-from)/10);for(int i=0;i<n;i++){float a=Mathf.Lerp(from,to,i/(float)n)*Mathf.Deg2Rad,b=Mathf.Lerp(from,to,(i+1)/(float)n)*Mathf.Deg2Rad;Line(x+Mathf.Cos(a)*r,y+Mathf.Sin(a)*r,x+Mathf.Cos(b)*r,y+Mathf.Sin(b)*r);}}
        void Line(float x1,float y1,float x2,float y2,float width=4)
        {
            var rect=rectTransform.rect;float scale=Mathf.Min(rect.width,rect.height)/100;
            Vector2 p=rect.center+new Vector2(x1-50,50-y1)*scale,q=rect.center+new Vector2(x2-50,50-y2)*scale;
            Vector2 n=new Vector2(-(q-p).y,(q-p).x).normalized*width*scale*.5f;int v=mesh.currentVertCount;
            mesh.AddVert(p-n,color,Vector2.zero);mesh.AddVert(p+n,color,Vector2.zero);mesh.AddVert(q+n,color,Vector2.zero);mesh.AddVert(q-n,color,Vector2.zero);
            mesh.AddTriangle(v,v+1,v+2);mesh.AddTriangle(v,v+2,v+3);
        }
    }
}
