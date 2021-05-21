//namespace DentedPixel{

using System;
using UnityEngine;

public class LTDescrOptional
{
    public AnimationCurve animationCurve;
    public Color color;
    public int initFrameCount;

    public Transform toTrans { get; set; }
    public Vector3 point { get; set; }
    public Vector3 axis { get; set; }
    public float lastVal { get; set; }
    public Quaternion origRotation { get; set; }
    public LTBezierPath path { get; set; }
    public LTSpline spline { get; set; }

    public LTRect ltRect { get; set; } // maybe get rid of this eventually

    public Action<float> onUpdateFloat { get; set; }
    public Action<float, float> onUpdateFloatRatio { get; set; }
    public Action<float, object> onUpdateFloatObject { get; set; }
    public Action<Vector2> onUpdateVector2 { get; set; }
    public Action<Vector3> onUpdateVector3 { get; set; }
    public Action<Vector3, object> onUpdateVector3Object { get; set; }
    public Action<Color> onUpdateColor { get; set; }
    public Action<Color, object> onUpdateColorObject { get; set; }
    public Action onComplete { get; set; }
    public Action<object> onCompleteObject { get; set; }
    public object onCompleteParam { get; set; }
    public object onUpdateParam { get; set; }
    public Action onStart { get; set; }


//	#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
//	public SpriteRenderer spriteRen { get; set; }
//	#endif
//
//	#if LEANTWEEN_1
//	public Hashtable optional;
//	#endif
//	#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
//	public RectTransform rectTransform;
//	public UnityEngine.UI.Text uiText;
//	public UnityEngine.UI.Image uiImage;
//	public UnityEngine.Sprite[] sprites;
//	#endif


    public void reset()
    {
        animationCurve = null;

        onUpdateFloat = null;
        onUpdateFloatRatio = null;
        onUpdateVector2 = null;
        onUpdateVector3 = null;
        onUpdateFloatObject = null;
        onUpdateVector3Object = null;
        onUpdateColor = null;
        onComplete = null;
        onCompleteObject = null;
        onCompleteParam = null;
        onStart = null;

        point = Vector3.zero;
        initFrameCount = 0;
    }

    public void callOnUpdate(float val, float ratioPassed)
    {
        if (onUpdateFloat != null)
            onUpdateFloat(val);

        if (onUpdateFloatRatio != null)
            onUpdateFloatRatio(val, ratioPassed);
        else if (onUpdateFloatObject != null)
            onUpdateFloatObject(val, onUpdateParam);
        else if (onUpdateVector3Object != null)
            onUpdateVector3Object(LTDescr.newVect, onUpdateParam);
        else if (onUpdateVector3 != null)
            onUpdateVector3(LTDescr.newVect);
        else if (onUpdateVector2 != null) onUpdateVector2(new Vector2(LTDescr.newVect.x, LTDescr.newVect.y));
    }
}

//}