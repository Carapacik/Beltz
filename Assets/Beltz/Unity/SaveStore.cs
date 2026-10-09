using System;
using Beltz.Core;
using UnityEngine;

namespace Beltz.Unity
{
    public static class SaveStore
    {
        const string Key="Beltz2.Session.v1";
        [Serializable] public sealed class Data
        {
            public int version=2, mode, seed, difficulty, legacyIndex;
            public string id;
            public float[] x,y;
            public bool[] clockwise,shaftActive,crossed,beltActive;
            public int[] a,b,moveKind,moveIndex;
        }
        public static void Save(GameSession session,int legacyIndex,int difficulty)
        {
            var l=session.Level;var moves=session.History();
            var d=new Data {id=l.Id,mode=(int)l.Mode,seed=l.Seed,difficulty=difficulty,legacyIndex=legacyIndex,
                x=new float[l.Shafts.Count],y=new float[l.Shafts.Count],clockwise=new bool[l.Shafts.Count],shaftActive=new bool[l.Shafts.Count],
                crossed=new bool[l.Belts.Count],beltActive=new bool[l.Belts.Count],a=new int[l.Belts.Count],b=new int[l.Belts.Count],
                moveKind=new int[moves.Length],moveIndex=new int[moves.Length]};
            for(int i=0;i<l.Shafts.Count;i++){var s=l.Shafts[i];d.x[i]=s.X;d.y[i]=s.Y;d.clockwise[i]=s.Clockwise;d.shaftActive[i]=s.Interactive;}
            for(int i=0;i<l.Belts.Count;i++){var b=l.Belts[i];d.a[i]=b.A;d.b[i]=b.B;d.crossed[i]=b.Crossed;d.beltActive[i]=b.Interactive;}
            for(int i=0;i<moves.Length;i++){d.moveKind[i]=(int)moves[i].Kind;d.moveIndex[i]=moves[i].Index;}
            PlayerPrefs.SetString(Key,JsonUtility.ToJson(d));
        }
        public static GameSession Load(out int legacyIndex,out int difficulty)
        {
            legacyIndex=0;difficulty=0;
            if(!PlayerPrefs.HasKey(Key))return null;
            try
            {
                var d=JsonUtility.FromJson<Data>(PlayerPrefs.GetString(Key));
                if(d==null||(d.version!=1&&d.version!=2)||d.mode<0||d.mode>3||d.x==null||d.x.Length>32||d.a==null||d.a.Length>16||d.moveKind==null||d.moveKind.Length>10000)return null;
                var nodes=new ShaftDef[d.x.Length];var belts=new BeltDef[d.a.Length];
                for(int i=0;i<nodes.Length;i++)nodes[i]=new ShaftDef(d.x[i],d.y[i],d.clockwise[i],d.shaftActive[i]);
                for(int i=0;i<belts.Length;i++)belts[i]=new BeltDef(d.a[i],d.b[i],d.crossed[i],d.beltActive[i]);
                var session=new GameSession(new LevelDefinition(d.id,(GameMode)d.mode,nodes,belts,d.seed));
                for(int i=0;i<d.moveKind.Length;i++)if(!session.Play(new Move((MoveKind)d.moveKind[i],d.moveIndex[i])))return null;
                legacyIndex=d.version==1&&d.legacyIndex>=0?d.legacyIndex/8*CampaignProgress.ChapterSize+d.legacyIndex%8:d.legacyIndex;
                if(legacyIndex<0||legacyIndex>=CampaignProgress.LevelCount)return null;difficulty=Mathf.Clamp(d.difficulty,0,2);return session;
            }
            catch(Exception e) when(e is ArgumentException||e is IndexOutOfRangeException||e is NullReferenceException)
            { Debug.LogWarning("Saved session is invalid; starting a fresh board.");return null; }
        }
    }
}
