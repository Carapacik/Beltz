using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Beltz.Core
{
    public enum GameMode { Belts, Shafts, Rotation, Mixed }
    public enum MoveKind { ToggleBelt, ToggleLinkedBelts, ReverseShaft }
    public readonly struct Move
    {
        public readonly MoveKind Kind;
        public readonly int Index;
        public Move(MoveKind kind, int index) { Kind = kind; Index = index; }
    }
    public readonly struct ShaftDef
    {
        public readonly float X, Y;
        public readonly bool Clockwise, Interactive;
        public ShaftDef(float x, float y, bool clockwise, bool interactive)
        { X = x; Y = y; Clockwise = clockwise; Interactive = interactive; }
    }
    public readonly struct BeltDef
    {
        public readonly int A, B;
        public readonly bool Crossed, Interactive;
        public BeltDef(int a, int b, bool crossed, bool interactive)
        { A = a; B = b; Crossed = crossed; Interactive = interactive; }
    }
    public sealed class LevelDefinition
    {
        public readonly string Id;
        public readonly GameMode Mode;
        public readonly ReadOnlyCollection<ShaftDef> Shafts;
        public readonly ReadOnlyCollection<BeltDef> Belts;
        public readonly int Seed;
        public const int GeneratorVersion = 3;
        public LevelDefinition(string id, GameMode mode, ShaftDef[] shafts, BeltDef[] belts, int seed = 0)
        {
            if (shafts == null || shafts.Length < 2 || belts == null || belts.Length < 1 || belts.Length > 16)
                throw new ArgumentException("A board needs at least two shafts and 1–16 belts.");
            if ((int)mode < 0 || (int)mode > 3) throw new ArgumentException("Unknown game mode.");
            var pairs = new HashSet<int>();
            for (int i = 0; i < shafts.Length; i++)
            {
                if ((mode == GameMode.Belts || mode == GameMode.Shafts) && shafts[i].Interactive)
                    throw new ArgumentException("Modes 1 and 2 have fixed shaft directions.");
                if (mode == GameMode.Rotation && !shafts[i].Interactive)
                    throw new ArgumentException("Mode 3 requires all shafts to be reversible.");
                if (float.IsNaN(shafts[i].X) || float.IsInfinity(shafts[i].X) || float.IsNaN(shafts[i].Y) || float.IsInfinity(shafts[i].Y))
                    throw new ArgumentException("Invalid shaft position.");
                for (int j = 0; j < i; j++)
                    if (Math.Abs(shafts[i].X - shafts[j].X) + Math.Abs(shafts[i].Y - shafts[j].Y) < .001f)
                        throw new ArgumentException("Shaft positions must be distinct.");
            }
            foreach (var b in belts)
            {
                if (mode == GameMode.Belts && !b.Interactive) throw new ArgumentException("Mode 1 requires all belts to be active.");
                if (b.A < 0 || b.B < 0 || b.A >= shafts.Length || b.B >= shafts.Length || b.A == b.B)
                    throw new ArgumentException("Invalid belt endpoints.");
                if (!pairs.Add(Math.Min(b.A, b.B) * shafts.Length + Math.Max(b.A, b.B)))
                    throw new ArgumentException("Duplicate connection.");
            }
            Id = id; Mode = mode; Seed = seed;
            Shafts = Array.AsReadOnly((ShaftDef[])shafts.Clone());
            Belts = Array.AsReadOnly((BeltDef[])belts.Clone());
        }
    }
    public sealed class PuzzleState
    {
        internal readonly bool[] Directions, Crossings;
        public bool Clockwise(int shaft) => Directions[shaft];
        public bool Crossed(int belt) => Crossings[belt];
        public PuzzleState(LevelDefinition level)
        {
            Directions = new bool[level.Shafts.Count]; Crossings = new bool[level.Belts.Count];
            for (int i = 0; i < Directions.Length; i++) Directions[i] = level.Shafts[i].Clockwise;
            for (int i = 0; i < Crossings.Length; i++) Crossings[i] = level.Belts[i].Crossed;
        }
    }
    public static class PuzzleRules
    {
        public static bool IsBeltCorrect(LevelDefinition level, PuzzleState state, int index)
        {
            var b = level.Belts[index];
            return state.Crossed(index) == (state.Clockwise(b.A) ^ state.Clockwise(b.B));
        }
        public static int ErrorMask(LevelDefinition level, PuzzleState state)
        {
            int mask = 0;
            for (int i = 0; i < level.Belts.Count; i++) if (!IsBeltCorrect(level, state, i)) mask |= 1 << i;
            return mask;
        }
        public static Move[] LegalMoves(LevelDefinition level)
        {
            var moves = new List<Move>();
            if (level.Mode != GameMode.Shafts)
                for (int i = 0; i < level.Belts.Count; i++)
                    if (level.Belts[i].Interactive) moves.Add(new Move(MoveKind.ToggleBelt, i));
            if (level.Mode != GameMode.Belts)
                for (int i = 0; i < level.Shafts.Count; i++)
                    if ((level.Mode == GameMode.Shafts || level.Shafts[i].Interactive) && IncidentMask(level, i) != 0)
                        moves.Add(new Move(level.Mode == GameMode.Shafts ? MoveKind.ToggleLinkedBelts : MoveKind.ReverseShaft, i));
            return moves.ToArray();
        }
        public static int IncidentMask(LevelDefinition level, int shaft)
        {
            int mask = 0;
            for (int i = 0; i < level.Belts.Count; i++)
                if ((level.Belts[i].A == shaft || level.Belts[i].B == shaft) &&
                    (level.Mode != GameMode.Shafts || level.Belts[i].Interactive)) mask |= 1 << i;
            return mask;
        }
        public static int MoveMask(LevelDefinition level, Move move) =>
            move.Kind == MoveKind.ToggleBelt ? 1 << move.Index : IncidentMask(level, move.Index);
        public static bool Apply(LevelDefinition level, PuzzleState state, Move move)
        {
            if (move.Kind == MoveKind.ToggleBelt)
            {
                if (level.Mode == GameMode.Shafts || move.Index < 0 || move.Index >= level.Belts.Count || !level.Belts[move.Index].Interactive) return false;
                state.Crossings[move.Index] ^= true;
            }
            else
            {
                if (move.Index < 0 || move.Index >= level.Shafts.Count ||
                    (level.Mode != GameMode.Shafts && !level.Shafts[move.Index].Interactive)) return false;
                if (move.Kind == MoveKind.ReverseShaft && (level.Mode == GameMode.Mixed || level.Mode == GameMode.Rotation))
                    state.Directions[move.Index] ^= true;
                else if (move.Kind == MoveKind.ToggleLinkedBelts && level.Mode == GameMode.Shafts)
                {
                    for (int i = 0; i < level.Belts.Count; i++)
                        if (level.Belts[i].Interactive && (level.Belts[i].A == move.Index || level.Belts[i].B == move.Index)) state.Crossings[i] ^= true;
                }
                else return false;
            }
            return true;
        }
    }
    public sealed class GameSession
    {
        public readonly LevelDefinition Level;
        public PuzzleState State { get; private set; }
        readonly List<Move> history = new List<Move>();
        public int MoveCount => history.Count;
        public bool Solved => PuzzleRules.ErrorMask(Level, State) == 0;
        public GameSession(LevelDefinition level) { Level = level; State = new PuzzleState(level); }
        public bool Play(Move move)
        {
            if (Solved || !PuzzleRules.Apply(Level, State, move)) return false;
            history.Add(move); return true;
        }
        public bool Undo()
        {
            if (history.Count == 0) return false;
            int last = history.Count - 1;
            PuzzleRules.Apply(Level, State, history[last]); history.RemoveAt(last); return true;
        }
        public void Restart() { State = new PuzzleState(Level); history.Clear(); }
        public Move[] History() => history.ToArray();
    }
}
