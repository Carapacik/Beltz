using System;
using System.Collections.Generic;

namespace Beltz.Core
{
    public static class LevelGeometry
    {
        public const float ShaftRadius=.43f;
        public const float MinimumCenterDistance=3* ShaftRadius;
        public static ShaftDef[] Sample(int count,ref StableRandom rng)
        {
            var points=new ShaftDef[count];
            for(int i=0;i<count;i++)
            {
                bool placed=false;
                for(int attempt=0;attempt<1024;attempt++)
                {
                    float x=(rng.Range(1000001)/1000000f-.5f)*5.4f;
                    float y=(rng.Range(1000001)/1000000f-.5f)*7.2f;
                    bool clear=true;
                    for(int j=0;j<i;j++)if(DistanceSquared(x,y,points[j].X,points[j].Y)<MinimumCenterDistance*MinimumCenterDistance){clear=false;break;}
                    if(!clear)continue;points[i]=new ShaftDef(x,y,false,false);placed=true;break;
                }
                if(!placed)throw new InvalidOperationException("Could not sample a separated shaft layout.");
            }
            return points;
        }
        public static List<(int a,int b)> Connect(ShaftDef[] points,int extras,ref StableRandom rng)
        {
            // A Euclidean minimum spanning tree connects arbitrary coordinates.
            // Extra unobstructed edges vary the topology without passing through shafts.
            var edges=new List<(int a,int b)>();var connected=new bool[points.Length];connected[0]=true;
            for(int n=1;n<points.Length;n++)
            {
                int a=-1,b=-1;float best=float.MaxValue;
                for(int i=0;i<points.Length;i++)if(connected[i])for(int j=0;j<points.Length;j++)if(!connected[j])
                {float d=DistanceSquared(points[i].X,points[i].Y,points[j].X,points[j].Y);if(d<best){best=d;a=i;b=j;}}
                edges.Add((a,b));connected[b]=true;
            }
            var candidates=new List<(int a,int b)>();
            for(int a=0;a<points.Length;a++)for(int b=a+1;b<points.Length;b++)
            {
                bool clear=true;
                foreach(var e in edges)if((e.a==a&&e.b==b)||(e.a==b&&e.b==a)){clear=false;break;}
                for(int i=0;i<points.Length&&clear;i++)if(i!=a&&i!=b&&SegmentDistance(points[i],points[a],points[b])<ShaftRadius*1.6f)clear=false;
                if(clear)candidates.Add((a,b));
            }
            while(extras-->0&&candidates.Count>0&&edges.Count<12){int pick=rng.Range(candidates.Count);edges.Add(candidates[pick]);candidates.RemoveAt(pick);}
            return edges;
        }
        public static float DistanceSquared(float x,float y,float x2,float y2){float dx=x-x2,dy=y-y2;return dx*dx+dy*dy;}
        static float SegmentDistance(ShaftDef p,ShaftDef a,ShaftDef b)
        {
            float dx=b.X-a.X,dy=b.Y-a.Y,t=Math.Max(0,Math.Min(1,((p.X-a.X)*dx+(p.Y-a.Y)*dy)/(dx*dx+dy*dy)));
            return (float)Math.Sqrt(DistanceSquared(p.X,p.Y,a.X+dx*t,a.Y+dy*t));
        }
    }
}
