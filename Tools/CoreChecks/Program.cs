using System;
using System.Diagnostics;
using Beltz.Core;
using Beltz.Content;

static class Program
{
    static int checks;
    static void Assert(bool value,string message) {checks++;if(!value)throw new Exception(message);}
    static void Main()
    {
        var timer=Stopwatch.StartNew(); int boards=0;
        foreach(var level in LegacyLevels.Create()) { Verify(level); boards++; }
        for(int mode=0;mode<4;mode++)for(int difficulty=0;difficulty<3;difficulty++)for(int seed=1;seed<=120;seed++)
        {
            var l=LevelGenerator.Generate((GameMode)mode,difficulty,seed); Verify(l);boards++;
            var other=LevelGenerator.Generate((GameMode)mode,difficulty,seed);
            Assert(Fingerprint(l)==Fingerprint(other),"Deterministic generation");
            if(mode==3) {Assert(l.Shafts[0].Interactive==false,"Locked shaft");Assert(l.Shafts[1].Interactive,"Active shaft");}
            if(mode==2) foreach(var shaft in l.Shafts)Assert(shaft.Interactive,"Mode 3 shaft is locked");
        }
        VerifyLockedRules(); VerifyShortest(); VerifyCampaign();
        Console.WriteLine($"PASS: {boards} base boards + 120 campaign boards; {checks} assertions, {timer.ElapsedMilliseconds} ms; progression and layout diversity verified.");
    }
    static void VerifyCampaign()
    {
        var progress=new CampaignProgress();
        Assert(progress.IsUnlocked(0)&&!progress.IsUnlocked(1),"Fresh campaign gate");
        Assert(!progress.Complete(30),"Cannot unlock a section out of order");
        for(int chapter=0;chapter<4;chapter++)
        {
            for(int local=0;local<30;local++)
            {
                int i=chapter*30+local;Verify(Campaign.Get(i));Assert((int)Campaign.Get(i).Mode==chapter,"Campaign mode order");
                Assert(progress.IsUnlocked(i),"Next campaign level locked");Assert(progress.Complete(i),"Complete unlocked level");
                Assert(progress.CompletedCount==i+1,"Total completion count");
                if(local==8)Assert(!progress.IsChapterUnlocked(chapter+1),"Section opened before ten completions");
                if(local==9&&chapter<3){Assert(progress.IsChapterUnlocked(chapter+1),"Section did not open after ten");Assert(progress.IsUnlocked(i+1),"Optional remaining levels became locked");}
            }
        }
        Assert(progress.CompletedCount==120&&progress.ContinueIndex==119,"All 120 levels recorded");
        Assert(!progress.IsUnlocked(-1)&&!progress.IsUnlocked(120),"Campaign bounds");
        progress.Complete(0);Assert(progress.CompletedCount==120,"Replays counted twice");
        for(int c=0;c<4;c++){Assert(!progress.HasSeenIntro(c),"Intro initially unseen");progress.SeeIntro(c);Assert(progress.HasSeenIntro(c),"Intro flag");}
        var restored=CampaignProgress.Import(progress.Export(),progress.IntroSeen);Assert(restored.CompletedCount==120&&restored.IntroSeen==15,"Progress round trip");
        var migrated=CampaignProgress.FromLegacy(uint.MaxValue,15);
        Assert(migrated.CompletedCount==32&&migrated.IsCompleted(30)&&migrated.IsCompleted(97),"Legacy progress remapped");
        Assert(!migrated.IsCompleted(8)&&!migrated.IsChapterUnlocked(1),"New levels and threshold after migration");
        var early=new CampaignProgress();for(int i=0;i<10;i++)early.Complete(i);
        Assert(early.ContinueIndex==30&&early.IsUnlocked(10),"Continue recommends newly opened section");
        for(int i=30;i<40;i++)early.Complete(i);
        Assert(early.ContinueIndex==60&&early.IsUnlocked(40),"Sections can open without finishing previous thirty");
        Assert(CampaignProgress.Import("bad save").CompletedCount==0,"Invalid progress fallback");
        for(int mode=0;mode<4;mode++)
        {
            var counts=new System.Collections.Generic.HashSet<int>();var edgeCounts=new System.Collections.Generic.HashSet<int>();var layouts=new System.Collections.Generic.HashSet<string>();
            for(int seed=1;seed<=30;seed++)
            {
                var l=LevelGenerator.Generate((GameMode)mode,1,seed);counts.Add(l.Shafts.Count);edgeCounts.Add(l.Belts.Count);
                string layout="";foreach(var n in l.Shafts)layout+=$"{n.X:R},{n.Y:R};";layouts.Add(layout);
                for(int i=0;i<l.Shafts.Count;i++)for(int j=0;j<i;j++){double dx=l.Shafts[i].X-l.Shafts[j].X,dy=l.Shafts[i].Y-l.Shafts[j].Y;Assert(dx*dx+dy*dy>=LevelGeometry.MinimumCenterDistance*LevelGeometry.MinimumCenterDistance-.00001,"Shaft spacing below minimum");}
            }
            Assert(counts.Count>=3&&edgeCounts.Count>=3&&layouts.Count==30,"Generator must vary node count, edge count and positions");
        }
    }
    static string Fingerprint(LevelDefinition l)
    {
        string s=l.Id; foreach(var n in l.Shafts)s+=$"|{n.X:R},{n.Y:R},{n.Clockwise},{n.Interactive}";
        foreach(var b in l.Belts)s+=$"|{b.A},{b.B},{b.Crossed},{b.Interactive}";return s;
    }
    static void Verify(LevelDefinition l)
    {
        if(l.Id.StartsWith("g"))for(int i=0;i<l.Shafts.Count;i++)for(int j=0;j<i;j++)
            Assert(LevelGeometry.DistanceSquared(l.Shafts[i].X,l.Shafts[i].Y,l.Shafts[j].X,l.Shafts[j].Y)>=LevelGeometry.MinimumCenterDistance*LevelGeometry.MinimumCenterDistance-.00001,"Generated clearance");
        var state=new PuzzleState(l); int start=PuzzleRules.ErrorMask(l,state);
        var solution=PuzzleSolver.Solve(l,state);Assert(solution!=null,$"Unsolvable {l.Id}");Assert(solution.Length>0,$"Initially solved {l.Id}");
        foreach(var move in PuzzleRules.LegalMoves(l))
        {
            Assert(PuzzleRules.Apply(l,state,move),"Legal move rejected");
            Assert(PuzzleRules.ErrorMask(l,state)==(start^PuzzleRules.MoveMask(l,move)),"Move quotient mismatch");
            Assert(PuzzleRules.Apply(l,state,move),"Reverse rejected");Assert(PuzzleRules.ErrorMask(l,state)==start,"Involution");
        }
        var session=new GameSession(l);
        foreach(var move in solution)Assert(session.Play(move),"Solution command rejected");
        Assert(session.Solved,"Solution did not win");Assert(!session.Play(solution[0]),"Moves allowed after win");
        for(int i=0;i<solution.Length;i++)Assert(session.Undo(),"Undo failed");
        Assert(PuzzleRules.ErrorMask(l,session.State)==start,"Undo did not restore");Assert(session.MoveCount==0,"Move history");
        session.Play(solution[0]);session.Restart();Assert(PuzzleRules.ErrorMask(l,session.State)==start,"Restart");
    }
    static void VerifyLockedRules()
    {
        foreach(GameMode mode in new[]{GameMode.Shafts,GameMode.Mixed})
        {
            var l=new LevelDefinition("locks",mode,new[]{new ShaftDef(0,0,true,mode==GameMode.Mixed),new ShaftDef(1,0,false,false),new ShaftDef(2,0,true,false)},
                new[]{new BeltDef(0,1,true,false),new BeltDef(0,2,true,true)});
            var s=new PuzzleState(l);Assert(!PuzzleRules.Apply(l,s,new Move(MoveKind.ToggleBelt,0)),"Locked belt changed directly");
            bool dir=s.Clockwise(0); bool free=s.Crossed(1); bool locked=s.Crossed(0);
            Assert(PuzzleRules.Apply(l,s,new Move(mode==GameMode.Shafts?MoveKind.ToggleLinkedBelts:MoveKind.ReverseShaft,0)),"Node move");
            Assert(s.Crossed(0)==locked,"Locked belt changed through shaft");
            Assert(s.Clockwise(0)==(mode==GameMode.Mixed?!dir:dir),"Wrong direction behavior");
            Assert(s.Crossed(1)==(mode==GameMode.Mixed?free:!free),"Mixed reversal changed a belt");
        }
    }
    static void VerifyShortest()
    {
        // Triangle in shaft mode: reachable masks have even parity; two bad edges need one move.
        for(int mask=0;mask<8;mask++)
        {
            var l=new LevelDefinition("triangle",GameMode.Shafts,new[]{new ShaftDef(0,0,true,false),new ShaftDef(1,0,true,false),new ShaftDef(0,1,true,false)},
                new[]{new BeltDef(0,1,(mask&1)!=0,true),new BeltDef(1,2,(mask&2)!=0,true),new BeltDef(2,0,(mask&4)!=0,true)});
            var solution=PuzzleSolver.Solve(l,new PuzzleState(l));int bits=((mask&1)!=0?1:0)+((mask&2)!=0?1:0)+((mask&4)!=0?1:0);
            Assert((solution!=null)==(bits%2==0),"Cycle reachability");
            if(solution!=null)Assert(solution.Length==(bits==0?0:1),"Optimal distance");
        }
    }
}
