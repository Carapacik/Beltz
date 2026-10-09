using UnityEngine;

namespace Beltz.Unity
{
    // A fixed-size ribbon mesh. No physics bodies, colliders or per-frame allocation.
    public sealed class BeltView
    {
        const int Samples = 28;
        readonly Mesh mesh;
        readonly Material material;
        readonly Vector3[] vertices = new Vector3[Samples * 4];
        readonly Vector2[] uv = new Vector2[Samples * 4];
        readonly Vector2 a, b;
        readonly float radius;
        readonly GameObject root;
        float crossing, target, velocity, kick, phase;
        public Vector2 Center => (a + b) * .5f;
        public BeltView(Transform parent, Vector2 a, Vector2 b, float radius, bool crossed, Material shared)
        {
            this.a = a; this.b = b; this.radius = radius; crossing = target = crossed ? 1 : 0;
            root = new GameObject("Belt ribbon"); root.transform.SetParent(parent, false);
            mesh = new Mesh { name = "Belt ribbon" }; mesh.MarkDynamic();
            root.AddComponent<MeshFilter>().sharedMesh = mesh;
            material = new Material(shared);
            var renderer = root.AddComponent<MeshRenderer>(); renderer.sharedMaterial = material; renderer.sortingOrder = 2;
            var triangles = new int[(Samples - 1) * 12]; int at = 0;
            for (int branch = 0; branch < 2; branch++)
                for (int j = 0; j < Samples - 1; j++)
                {
                    int v = branch * Samples * 2 + j * 2;
                    triangles[at++] = v; triangles[at++] = v + 2; triangles[at++] = v + 1;
                    triangles[at++] = v + 1; triangles[at++] = v + 2; triangles[at++] = v + 3;
                }
            mesh.vertices = vertices; mesh.uv = uv; mesh.triangles = triangles;
            Tick(0, false, Color.black, 1);
        }
        public void SetCrossed(bool value)
        {
            float next = value ? 1 : 0;
            if (target != next) { target = next; kick = .23f; }
        }
        public float Distance(Vector2 point)
        {
            // The entire interior of the belt is touchable, plus a forgiving
            // margin around the rendered branches. Shaft hit testing has priority.
            Vector2 axis=b-a;
            float along=Mathf.Clamp01(Vector2.Dot(point-a,axis)/Mathf.Max(.0001f,axis.sqrMagnitude));
            float best=Mathf.Max(0,Vector2.Distance(point,a+axis*along)-radius);
            for (int branch = 0; branch < 2; branch++)
                for (int j = 0; j < Samples - 1; j++)
                {
                    int index = branch * Samples * 2 + j * 2;
                    Vector2 p = (vertices[index] + vertices[index + 1]) * .5f;
                    Vector2 q = (vertices[index + 2] + vertices[index + 3]) * .5f;
                    Vector2 d = q - p;
                    float t = Mathf.Clamp01(Vector2.Dot(point - p, d) / Mathf.Max(.0001f, d.sqrMagnitude));
                    best = Mathf.Min(best, Vector2.Distance(point, p + d * t));
                }
            return best;
        }
        public void Tick(float dt, bool moving, Color color, float direction,bool active=false)
        {
            if(dt>0)crossing = Mathf.SmoothDamp(crossing, target, ref velocity, .17f, 100, dt);
            kick *= Mathf.Exp(-dt * 7);
            if (moving) phase += dt * direction * .48f;
            material.SetFloat("_Phase", phase); material.SetColor("_Color", color);
            material.SetColor("_OutlineColor",active?BoardView.Locked:BoardView.Active);
            Vector2 axis = (b - a).normalized, normal = new Vector2(-axis.y, axis.x);
            float distance = Vector2.Distance(a, b);
            float tangentX = Mathf.Clamp(2 * radius / distance, 0, .9f);
            float tangentY = Mathf.Sqrt(1 - tangentX * tangentX);
            for (int branch = 0; branch < 2; branch++)
            {
                float side = branch == 0 ? 1 : -1;
                Vector2 openStart = a + normal * radius * side, openEnd = b + normal * radius * side;
                Vector2 crossStart = a + axis * (radius * tangentX) + normal * (radius * tangentY * side);
                Vector2 crossEnd = b - axis * (radius * tangentX) - normal * (radius * tangentY * side);
                Vector2 start = Vector2.Lerp(openStart, crossStart, crossing), end = Vector2.Lerp(openEnd, crossEnd, crossing);
                Vector2 stripNormal = new Vector2(-(end-start).y,(end-start).x).normalized;
                float length = Vector2.Distance(start,end);
                for (int j = 0; j < Samples; j++)
                {
                    float t = j / (float)(Samples - 1);
                    // Fixed endpoints, transient slack and a damped recoil wave.
                    float bow = Mathf.Sin(t*Mathf.PI) * (Mathf.Sin(crossing*Mathf.PI)*.24f + kick*Mathf.Sin(t*Mathf.PI*3 + kick*16));
                    Vector2 p = Vector2.Lerp(start,end,t) + stripNormal * (side*bow);
                    int v = branch * Samples * 2 + j * 2;
                    float width = .065f;
                    vertices[v] = new Vector3(p.x-stripNormal.x*width,p.y-stripNormal.y*width, branch == 0 ? -.04f : 0);
                    vertices[v+1] = new Vector3(p.x+stripNormal.x*width,p.y+stripNormal.y*width, branch == 0 ? -.04f : 0);
                    uv[v] = new Vector2(t*length*(branch==0?1:-1),0); uv[v+1] = new Vector2(uv[v].x,1);
                }
            }
            mesh.vertices = vertices; mesh.uv = uv; mesh.RecalculateBounds();
        }
        public void Dispose() { Object.Destroy(mesh); Object.Destroy(material); Object.Destroy(root); }
    }
}
