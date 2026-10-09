using UnityEngine;
using UnityEngine.UI;

namespace Beltz.Unity
{
    // A separate ring keeps the border crisp without duplicating the blurred fill.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ButtonBorder : MaskableGraphic
    {
        public Image Source;
        public Button Button;
        public override Texture mainTexture=>Texture2D.whiteTexture;

        void LateUpdate()
        {
            var tint=new Color(1,1,1,Button!=null&&Button.IsInteractable()?.68f:.18f);
            if(color!=tint)color=tint;
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();if(Source==null)return;
            var r=rectTransform.rect;
            float limit=Mathf.Min(r.width,r.height)*.5f;
            if(limit<=0)return;
            if(Source.type==Image.Type.Simple&&Source.preserveAspect)
                r=new Rect(r.center-Vector2.one*limit,Vector2.one*(limit*2));
            float radius=Source.type==Image.Type.Simple?limit:
                Mathf.Min(limit,63/(Source.pixelsPerUnit*Source.pixelsPerUnitMultiplier));
            const int segments=12,points=4*(segments+1);
            float inner=Mathf.Max(0,radius-3);
            for(int corner=0;corner<4;corner++)
            {
                var center=new Vector2(corner<2?r.xMax-radius:r.xMin+radius,
                    corner==0||corner==3?r.yMax-radius:r.yMin+radius);
                for(int step=0;step<=segments;step++)
                {
                    float angle=(90-corner*90-step*90f/segments)*Mathf.Deg2Rad;
                    var direction=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                    mesh.AddVert(center+direction*radius,color,Vector2.zero);
                    mesh.AddVert(center+direction*inner,color,Vector2.zero);
                }
            }
            for(int i=0;i<points;i++)
            {
                int next=(i+1)%points;
                mesh.AddTriangle(i*2,next*2,i*2+1);
                mesh.AddTriangle(i*2+1,next*2,next*2+1);
            }
        }
    }
}
