using UnityEngine;

public class ExampleSpline : MonoBehaviour
{
    public Transform[] trans;

    private float iter;
    private GameObject ltLogo;
    private GameObject ltLogo2;

    private LTSpline spline;

    private void Start()
    {
        spline = new LTSpline(new[]
            {trans[0].position, trans[1].position, trans[2].position, trans[3].position, trans[4].position});
        ltLogo = GameObject.Find("LeanTweenLogo1");
        ltLogo2 = GameObject.Find("LeanTweenLogo2");

        LeanTween.moveSpline(ltLogo2, spline.pts, 1f).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong()
            .setOrientToPath(true);

        var zoomInPath_LT = LeanTween.moveSpline(ltLogo2,
            new[] {Vector3.zero, Vector3.zero, new Vector3(1, 1, 1), new Vector3(2, 1, 1), new Vector3(2, 1, 1)}, 1.5f);
        zoomInPath_LT.setUseEstimatedTime(true);
    }

    private void Update()
    {
        // Iterating over path
        ltLogo.transform.position = spline.point(iter /*(Time.time*1000)%1000 * 1.0 / 1000.0 */);

        iter += Time.deltaTime * 0.1f;
        if (iter > 1.0f)
            iter = 0.0f;
    }

    private void OnDrawGizmos()
    {
        if (spline != null)
            spline.gizmoDraw(); // debug aid to be able to see the path in the scene inspector
    }
}