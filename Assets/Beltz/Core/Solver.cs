using System;
using System.Collections.Generic;

namespace Beltz.Core
{
    // Each legal move XORs a fixed set of consistency bits. Since victory only
    // depends on these bits, BFS on this quotient is exact for all four modes.
    public static class PuzzleSolver
    {
        public static Move[] Solve(LevelDefinition level, PuzzleState state)
        {
            int start = PuzzleRules.ErrorMask(level, state);
            if (start == 0) return Array.Empty<Move>();
            int size = 1 << level.Belts.Count;
            var parent = new int[size]; var via = new int[size]; var queue = new int[size];
            for (int i = 0; i < size; i++) parent[i] = -1;
            var moves = PuzzleRules.LegalMoves(level);
            var masks = new int[moves.Length];
            for (int i = 0; i < masks.Length; i++) masks[i] = PuzzleRules.MoveMask(level, moves[i]);
            int head = 0, tail = 0; queue[tail++] = start; parent[start] = start;
            while (head < tail)
            {
                int current = queue[head++];
                for (int i = 0; i < moves.Length; i++)
                {
                    int next = current ^ masks[i];
                    if (parent[next] >= 0) continue;
                    parent[next] = current; via[next] = i;
                    if (next == 0)
                    {
                        var result = new List<Move>();
                        for (int p = 0; p != start; p = parent[p]) result.Add(moves[via[p]]);
                        result.Reverse(); return result.ToArray();
                    }
                    queue[tail++] = next;
                }
            }
            return null;
        }
    }
    public struct StableRandom
    {
        uint value;
        public StableRandom(int seed) { value = unchecked((uint)seed) ^ 0x9e3779b9u; if (value == 0) value = 1; }
        public uint Next() { value ^= value << 13; value ^= value >> 17; value ^= value << 5; return value; }
        public int Range(int max) => (int)(Next() % (uint)max);
    }
    public static class LevelGenerator
    {
        public static LevelDefinition Generate(GameMode mode, int difficulty, int seed)
        {
            difficulty = Math.Max(0, Math.Min(2, difficulty));
            var rng = new StableRandom(seed);
            int count = 4 + difficulty + rng.Range(3);
            var positions=LevelGeometry.Sample(count,ref rng);
            var edges=LevelGeometry.Connect(positions,rng.Range(difficulty+3),ref rng);
            int desired = mode == GameMode.Shafts ? difficulty + 2 : difficulty + 3;
            LevelDefinition best = null; int bestDistance = -1;
            // Bounded work; each generated candidate has a construction witness.
            for (int attempt = 0; attempt < 96; attempt++)
            {
                var shafts = new ShaftDef[count];
                for (int i = 0; i < count; i++)
                {
                    bool active = mode == GameMode.Rotation || (mode == GameMode.Mixed && i % 3 != 0);
                    shafts[i] = new ShaftDef(positions[i].X,positions[i].Y,rng.Range(2) == 0, active);
                }
                var belts = new List<BeltDef>();
                for (int i = 0; i < edges.Count; i++)
                {
                    int a=edges[i].a,next=edges[i].b;
                    belts.Add(new BeltDef(a, next, shafts[a].Clockwise ^ shafts[next].Clockwise,
                        mode == GameMode.Belts || (mode == GameMode.Shafts && (difficulty == 0 || i % 4 != 0)) ||
                        ((mode == GameMode.Mixed || mode == GameMode.Rotation) && i % 3 == 0)));
                }
                var solved = new LevelDefinition("generated", mode, shafts, belts.ToArray(), seed);
                var state = new PuzzleState(solved); var moves = PuzzleRules.LegalMoves(solved);
                for (int j = 0; j < 4 + rng.Range(15); j++) PuzzleRules.Apply(solved, state, moves[rng.Range(moves.Length)]);
                for (int i = 0; i < shafts.Length; i++) shafts[i] = new ShaftDef(shafts[i].X, shafts[i].Y, state.Clockwise(i), shafts[i].Interactive);
                for (int i = 0; i < belts.Count; i++) belts[i] = new BeltDef(belts[i].A, belts[i].B, state.Crossed(i), belts[i].Interactive);
                var candidate = new LevelDefinition($"g{LevelDefinition.GeneratorVersion}-{(int)mode}-{difficulty}-{seed}", mode, shafts, belts.ToArray(), seed);
                var solution = PuzzleSolver.Solve(candidate, new PuzzleState(candidate));
                if (solution == null || solution.Length == 0) continue;
                if (best == null || Math.Abs(solution.Length - desired) < Math.Abs(bestDistance - desired))
                { best = candidate; bestDistance = solution.Length; }
                if (solution.Length == desired) return candidate;
            }
            if (best == null) throw new InvalidOperationException("Generator could not construct a non-solved board.");
            return best; // Use the measured optimum, not the scramble length.
        }
    }
}
