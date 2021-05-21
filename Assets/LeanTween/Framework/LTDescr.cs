//namespace DentedPixel{

using System;
using UnityEngine;
using UnityEngine.UI;

/**
 * Internal Representation of a Tween
 * <br>
 *     <br>
 *         This class represents all of the optional parameters you can pass to a method (it also represents the internal
 *         representation of the tween).
 *         <br>
 *             <br>
 *                 <strong id='optional'>Optional Parameters</strong> are passed at the end of every method:
 *                 <br>
 *                     <br>
 *                         &nbsp;&nbsp;<i>Example:</i>
 *                         <br>
 *                             &nbsp;&nbsp;LeanTween.moveX( gameObject, 1f, 1f).setEase(
 *                             <a href="LeanTweenType.html">LeanTweenType</a>.easeInQuad ).setDelay(1f);
 *                             <br>
 *                                 <br>
 *                                     You can pass the optional parameters in any order, and chain on as many as you
 *                                     wish.
 *                                     <br>
 *                                         You can also <strong>pass parameters at a later time</strong> by saving a
 *                                         reference to what is returned.
 *                                         <br>
 *                                             <br>
 *                                                 Retrieve a <strong>unique id</strong> for the tween by using the "id"
 *                                                 property. You can pass this to LeanTween.pause, LeanTween.resume,
 *                                                 LeanTween.cancel, LeanTween.isTweening methods
 *                                                 <br>
 *                                                     <br>
 *                                                         &nbsp;&nbsp;<h4>Example:</h4>
 *                                                         &nbsp;&nbsp;int id = LeanTween.moveX(gameObject, 1f, 3f).id;
 *                                                         <br>
 *                                                             <div style="color:gray">
 *                                                                 &nbsp;&nbsp;// pause a specific
 *                                                                 tween
 *                                                             </div>
 *                                                             &nbsp;&nbsp;LeanTween.pause(id);
 *                                                             <br>
 *                                                                 <div style="color:gray">&nbsp;&nbsp;// resume later</div>
 *                                                                 &nbsp;&nbsp;LeanTween.resume(id);
 *                                                                 <br>
 *                                                                     <div style="color:gray">
 *                                                                         &nbsp;&nbsp;// check if it
 *                                                                         is tweening before kicking of a new tween
 *                                                                     </div>
 *                                                                     &nbsp;&nbsp;if( LeanTween.isTweening( id ) ){
 *                                                                     <br>
 *                                                                         &nbsp;&nbsp; &nbsp;&nbsp;	LeanTween.cancel( id
 *                                                                         );
 *                                                                         <br>
 *                                                                             &nbsp;&nbsp; &nbsp;&nbsp;
 *                                                                             LeanTween.moveZ(gameObject, 10f, 3f);
 *                                                                             <br>
 *                                                                                 &nbsp;&nbsp;}
 *                                                                                 <br>
 *                                                                                     @class LTDescr
 *                                                                                     @constructor
 */
public class LTDescr
{
    public delegate void ActionMethodDelegate();

    public delegate Vector3 EaseTypeDelegate();

    public static float val;
    public static float dt;
    public static Vector3 newVect;
    private uint _id;

    public LTDescrOptional _optional = new LTDescrOptional();
    public uint counter = uint.MaxValue;
    public float delay;
    public bool destroyOnComplete;
    internal Vector3 diff;
    internal Vector3 diffDiv2;
    public float direction;
    public float directionLast;

    public EaseTypeDelegate easeMethod;
    private LeanTweenType easeType;
    internal Vector3 fromInternal;
    public bool hasExtraOnCompletes;
    public bool hasInitiliazed;
    public bool hasPhysics;

    public bool hasUpdateCallback;
    public float lastVal;
    public int loopCount;
    public LeanTweenType loopType;
    public bool onCompleteOnRepeat;
    public bool onCompleteOnStart;
    public float overshoot;
    public float passed;
    public float period;
    public float ratioPassed;
    public float scale;
    public float speed;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
    public SpriteRenderer spriteRen;
#endif
    public float time;
    public bool toggle;
    internal Vector3 toInternal;
    public Transform trans;
    public TweenAction type;
    public bool useEstimatedTime;
    public bool useFrames;
    public bool useManualTime;
    public bool useRecursion;
    public bool usesNormalDt;

    public Vector3 from
    {
        get => fromInternal;
        set => fromInternal = value;
    }

    public Vector3 to
    {
        get => toInternal;
        set => toInternal = value;
    }

    public ActionMethodDelegate easeInternal { get; set; }
    public ActionMethodDelegate initInternal { get; set; }

    // Convenience Getters
    public Transform toTrans => optional.toTrans;

    public int uniqueId
    {
        get
        {
            var toId = _id | (counter << 16);

            /*uint backId = toId & 0xFFFF;
            uint backCounter = toId >> 16;
            if(_id!=backId || backCounter!=counter){
                Debug.LogError("BAD CONVERSION toId:"+_id);
            }*/

            return (int) toId;
        }
    }

    public int id => uniqueId;

    public LTDescrOptional optional
    {
        get => _optional;
        set => _optional = value;
    }

    public override string ToString()
    {
        return (trans != null ? "name:" + trans.gameObject.name : "gameObject:null") + " toggle:" + toggle +
               " passed:" + passed + " time:" + time + " delay:" + delay + " direction:" + direction + " from:" + from +
               " to:" + to + " diff:" + diff + " type:" + type + " ease:" + easeType + " useEstimatedTime:" +
               useEstimatedTime + " id:" + id + " hasInitiliazed:" + hasInitiliazed;
    }

    [Obsolete("Use 'LeanTween.cancel( id )' instead")]
    public LTDescr cancel(GameObject gameObject)
    {
        // Debug.Log("canceling id:"+this._id+" this.uniqueId:"+this.uniqueId+" go:"+this.trans.gameObject);
        if (gameObject == trans.gameObject)
            LeanTween.removeTween((int) _id, uniqueId);
        return this;
    }

    public void reset()
    {
        toggle = useRecursion = usesNormalDt = true;
        trans = null;
        spriteRen = null;
        passed = delay = lastVal = 0.0f;
        hasUpdateCallback = useEstimatedTime = useFrames = hasInitiliazed = onCompleteOnRepeat =
            destroyOnComplete = onCompleteOnStart = useManualTime = hasExtraOnCompletes = false;
        easeType = LeanTweenType.linear;
        loopType = LeanTweenType.once;
        loopCount = 0;
        direction = directionLast = overshoot = scale = 1.0f;
        period = 0.3f;
        speed = -1f;
        easeMethod = easeLinear;
        from = to = Vector3.zero;
        _optional.reset();
    }

    // Initialize and Internal Methods

    public LTDescr setFollow()
    {
        type = TweenAction.FOLLOW;
        return this;
    }

    public LTDescr setMoveX()
    {
        type = TweenAction.MOVE_X;
        initInternal = () => { fromInternal.x = trans.position.x; };
        easeInternal = () => { trans.position = new Vector3(easeMethod().x, trans.position.y, trans.position.z); };
        return this;
    }

    public LTDescr setMoveY()
    {
        type = TweenAction.MOVE_Y;
        initInternal = () => { fromInternal.x = trans.position.y; };
        easeInternal = () => { trans.position = new Vector3(trans.position.x, easeMethod().x, trans.position.z); };
        return this;
    }

    public LTDescr setMoveZ()
    {
        type = TweenAction.MOVE_Z;
        initInternal = () => { fromInternal.x = trans.position.z; };
        ;
        easeInternal = () => { trans.position = new Vector3(trans.position.x, trans.position.y, easeMethod().x); };
        return this;
    }

    public LTDescr setMoveLocalX()
    {
        type = TweenAction.MOVE_LOCAL_X;
        initInternal = () => { fromInternal.x = trans.localPosition.x; };
        easeInternal = () =>
        {
            trans.localPosition = new Vector3(easeMethod().x, trans.localPosition.y, trans.localPosition.z);
        };
        return this;
    }

    public LTDescr setMoveLocalY()
    {
        type = TweenAction.MOVE_LOCAL_Y;
        initInternal = () => { fromInternal.x = trans.localPosition.y; };
        easeInternal = () =>
        {
            trans.localPosition = new Vector3(trans.localPosition.x, easeMethod().x, trans.localPosition.z);
        };
        return this;
    }

    public LTDescr setMoveLocalZ()
    {
        type = TweenAction.MOVE_LOCAL_Z;
        initInternal = () => { fromInternal.x = trans.localPosition.z; };
        easeInternal = () =>
        {
            trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, easeMethod().x);
        };
        return this;
    }

    private void initFromInternal()
    {
        fromInternal.x = 0;
    }

    public LTDescr setOffset(Vector3 offset)
    {
        toInternal = offset;
        return this;
    }

    public LTDescr setMoveCurved()
    {
        type = TweenAction.MOVE_CURVED;
        initInternal = initFromInternal;
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            if (_optional.path.orientToPath)
            {
                if (_optional.path.orientToPath2d)
                    _optional.path.place2d(trans, val);
                else
                    _optional.path.place(trans, val);
            }
            else
            {
                trans.position = _optional.path.point(val);
            }
        };
        return this;
    }

    public LTDescr setMoveCurvedLocal()
    {
        type = TweenAction.MOVE_CURVED_LOCAL;
        initInternal = initFromInternal;
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            if (_optional.path.orientToPath)
            {
                if (_optional.path.orientToPath2d)
                    _optional.path.placeLocal2d(trans, val);
                else
                    _optional.path.placeLocal(trans, val);
            }
            else
            {
                trans.localPosition = _optional.path.point(val);
            }
        };
        return this;
    }

    public LTDescr setMoveSpline()
    {
        type = TweenAction.MOVE_SPLINE;
        initInternal = initFromInternal;
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            if (_optional.spline.orientToPath)
            {
                if (_optional.spline.orientToPath2d)
                    _optional.spline.place2d(trans, val);
                else
                    _optional.spline.place(trans, val);
            }
            else
            {
                trans.position = _optional.spline.point(val);
            }
        };
        return this;
    }

    public LTDescr setMoveSplineLocal()
    {
        type = TweenAction.MOVE_SPLINE_LOCAL;
        initInternal = initFromInternal;
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            if (_optional.spline.orientToPath)
            {
                if (_optional.spline.orientToPath2d)
                    _optional.spline.placeLocal2d(trans, val);
                else
                    _optional.spline.placeLocal(trans, val);
            }
            else
            {
                trans.localPosition = _optional.spline.point(val);
            }
        };
        return this;
    }

    public LTDescr setScaleX()
    {
        type = TweenAction.SCALE_X;
        initInternal = () => { fromInternal.x = trans.localScale.x; };
        easeInternal = () =>
        {
            trans.localScale = new Vector3(easeMethod().x, trans.localScale.y, trans.localScale.z);
        };
        return this;
    }

    public LTDescr setScaleY()
    {
        type = TweenAction.SCALE_Y;
        initInternal = () => { fromInternal.x = trans.localScale.y; };
        easeInternal = () =>
        {
            trans.localScale = new Vector3(trans.localScale.x, easeMethod().x, trans.localScale.z);
        };
        return this;
    }

    public LTDescr setScaleZ()
    {
        type = TweenAction.SCALE_Z;
        initInternal = () => { fromInternal.x = trans.localScale.z; };
        easeInternal = () =>
        {
            trans.localScale = new Vector3(trans.localScale.x, trans.localScale.y, easeMethod().x);
        };
        return this;
    }

    public LTDescr setRotateX()
    {
        type = TweenAction.ROTATE_X;
        initInternal = () =>
        {
            fromInternal.x = trans.eulerAngles.x;
            toInternal.x = LeanTween.closestRot(fromInternal.x, toInternal.x);
        };
        easeInternal = () =>
        {
            trans.eulerAngles = new Vector3(easeMethod().x, trans.eulerAngles.y, trans.eulerAngles.z);
        };
        return this;
    }

    public LTDescr setRotateY()
    {
        type = TweenAction.ROTATE_Y;
        initInternal = () =>
        {
            fromInternal.x = trans.eulerAngles.y;
            toInternal.x = LeanTween.closestRot(fromInternal.x, toInternal.x);
        };
        easeInternal = () =>
        {
            trans.eulerAngles = new Vector3(trans.eulerAngles.x, easeMethod().x, trans.eulerAngles.z);
        };
        return this;
    }

    public LTDescr setRotateZ()
    {
        type = TweenAction.ROTATE_Z;
        initInternal = () =>
        {
            fromInternal.x = trans.eulerAngles.z;
            toInternal.x = LeanTween.closestRot(fromInternal.x, toInternal.x);
        };
        easeInternal = () =>
        {
            trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, easeMethod().x);
        };
        return this;
    }

    public LTDescr setRotateAround()
    {
        type = TweenAction.ROTATE_AROUND;
        initInternal = () =>
        {
            fromInternal.x = 0f;
            _optional.origRotation = trans.rotation;
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var origPos = trans.localPosition;
            var rotateAroundPt = trans.TransformPoint(_optional.point);
            // Debug.Log("this._optional.point:"+this._optional.point);
            trans.RotateAround(rotateAroundPt, _optional.axis, -_optional.lastVal);
            var diff = origPos - trans.localPosition;

            trans.localPosition =
                origPos - diff; // Subtract the amount the object has been shifted over by the rotate, to get it back to it's orginal position
            trans.rotation = _optional.origRotation;

            rotateAroundPt = trans.TransformPoint(_optional.point);
            trans.RotateAround(rotateAroundPt, _optional.axis, val);

            _optional.lastVal = val;
        };
        return this;
    }

    public LTDescr setRotateAroundLocal()
    {
        type = TweenAction.ROTATE_AROUND_LOCAL;
        initInternal = () =>
        {
            fromInternal.x = 0f;
            _optional.origRotation = trans.localRotation;
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var origPos = trans.localPosition;
            trans.RotateAround(trans.TransformPoint(_optional.point), trans.TransformDirection(_optional.axis),
                -_optional.lastVal);
            var diff = origPos - trans.localPosition;

            trans.localPosition =
                origPos - diff; // Subtract the amount the object has been shifted over by the rotate, to get it back to it's orginal position
            trans.localRotation = _optional.origRotation;
            var rotateAroundPt = trans.TransformPoint(_optional.point);
            trans.RotateAround(rotateAroundPt, trans.TransformDirection(_optional.axis), val);

            _optional.lastVal = val;
        };
        return this;
    }

    public LTDescr setAlpha()
    {
        type = TweenAction.ALPHA;
        initInternal = () =>
        {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			if(trans.gameObject.renderer){ this.fromInternal.x =
 trans.gameObject.renderer.material.color.a; }else if(trans.childCount>0){ foreach (Transform child in trans) { if(child.gameObject.renderer!=null){ Color col
 = child.gameObject.renderer.material.color; this.fromInternal.x = col.a; break; }}}
			this.easeInternal = this.alpha;
			break;
#else
            var ren = trans.GetComponent<SpriteRenderer>();
            if (ren != null)
            {
                fromInternal.x = ren.color.a;
            }
            else
            {
                if (trans.GetComponent<Renderer>() != null &&
                    trans.GetComponent<Renderer>().material.HasProperty("_Color"))
                {
                    fromInternal.x = trans.GetComponent<Renderer>().material.color.a;
                }
                else if (trans.GetComponent<Renderer>() != null &&
                         trans.GetComponent<Renderer>().material.HasProperty("_TintColor"))
                {
                    var col = trans.GetComponent<Renderer>().material.GetColor("_TintColor");
                    fromInternal.x = col.a;
                }
                else if (trans.childCount > 0)
                {
                    foreach (Transform child in trans)
                        if (child.gameObject.GetComponent<Renderer>() != null)
                        {
                            var col = child.gameObject.GetComponent<Renderer>().material.color;
                            fromInternal.x = col.a;
                            break;
                        }
                }
            }
#endif

            easeInternal = () =>
            {
                val = easeMethod().x;
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				alphaRecursive(this.trans, val, this.useRecursion);
#else
                if (spriteRen != null)
                {
                    spriteRen.color = new Color(spriteRen.color.r, spriteRen.color.g, spriteRen.color.b, val);
                    alphaRecursiveSprite(trans, val);
                }
                else
                {
                    alphaRecursive(trans, val, useRecursion);
                }
#endif
            };
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			alphaRecursive(this.trans, val, this.useRecursion);
#else
            if (spriteRen != null)
            {
                spriteRen.color = new Color(spriteRen.color.r, spriteRen.color.g, spriteRen.color.b, val);
                alphaRecursiveSprite(trans, val);
            }
            else
            {
                alphaRecursive(trans, val, useRecursion);
            }
#endif
        };
        return this;
    }

    public LTDescr setTextAlpha()
    {
        type = TweenAction.TEXT_ALPHA;
        initInternal = () =>
        {
            uiText = trans.GetComponent<Text>();
            fromInternal.x = uiText != null ? uiText.color.a : 1f;
        };
        easeInternal = () => { textAlphaRecursive(trans, easeMethod().x, useRecursion); };
        return this;
    }

    public LTDescr setAlphaVertex()
    {
        type = TweenAction.ALPHA_VERTEX;
        initInternal = () => { fromInternal.x = trans.GetComponent<MeshFilter>().mesh.colors32[0].a; };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var mesh = trans.GetComponent<MeshFilter>().mesh;
            var vertices = mesh.vertices;
            var colors = new Color32[vertices.Length];
            if (colors.Length == 0)
            {
                //MaxFW fix: add vertex colors if the mesh doesn't have any             
                var transparentWhiteColor32 = new Color32(0xff, 0xff, 0xff, 0x00);
                colors = new Color32[mesh.vertices.Length];
                for (var k = 0; k < colors.Length; k++)
                    colors[k] = transparentWhiteColor32;
                mesh.colors32 = colors;
            } // fix end

            var c = mesh.colors32[0];
            c = new Color(c.r, c.g, c.b, val);
            for (var k = 0; k < vertices.Length; k++)
                colors[k] = c;
            mesh.colors32 = colors;
        };
        return this;
    }

    public LTDescr setColor()
    {
        type = TweenAction.COLOR;
        initInternal = () =>
        {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			if(trans.gameObject.renderer){
			this.setFromColor( trans.gameObject.renderer.material.color );
			}else if(trans.childCount>0){
			foreach (Transform child in trans) {
			if(child.gameObject.renderer!=null){
			this.setFromColor( child.gameObject.renderer.material.color );
			break;
			}
			}
			}
#else
            var renColor = trans.GetComponent<SpriteRenderer>();
            if (renColor != null)
            {
                setFromColor(renColor.color);
            }
            else
            {
                if (trans.GetComponent<Renderer>() != null &&
                    trans.GetComponent<Renderer>().material.HasProperty("_Color"))
                {
                    var col = trans.GetComponent<Renderer>().material.color;
                    setFromColor(col);
                }
                else if (trans.GetComponent<Renderer>() != null &&
                         trans.GetComponent<Renderer>().material.HasProperty("_TintColor"))
                {
                    var col = trans.GetComponent<Renderer>().material.GetColor("_TintColor");
                    setFromColor(col);
                }
                else if (trans.childCount > 0)
                {
                    foreach (Transform child in trans)
                        if (child.gameObject.GetComponent<Renderer>() != null)
                        {
                            var col = child.gameObject.GetComponent<Renderer>().material.color;
                            setFromColor(col);
                            break;
                        }
                }
            }
#endif
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var toColor = tweenColor(this, val);

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2

            if (spriteRen != null)
            {
                spriteRen.color = toColor;
                colorRecursiveSprite(trans, toColor);
            }
            else
            {
#endif
                // Debug.Log("val:"+val+" tween:"+tween+" tween.diff:"+tween.diff);
                if (type == TweenAction.COLOR)
                    colorRecursive(trans, toColor, useRecursion);

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            }
#endif
            if (dt != 0f && _optional.onUpdateColor != null)
                _optional.onUpdateColor(toColor);
            else if (dt != 0f && _optional.onUpdateColorObject != null)
                _optional.onUpdateColorObject(toColor, _optional.onUpdateParam);
        };
        return this;
    }

    public LTDescr setCallbackColor()
    {
        type = TweenAction.CALLBACK_COLOR;
        initInternal = () => { diff = new Vector3(1.0f, 0.0f, 0.0f); };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var toColor = tweenColor(this, val);

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            if (spriteRen != null)
            {
                spriteRen.color = toColor;
                colorRecursiveSprite(trans, toColor);
            }
            else
            {
#endif
                // Debug.Log("val:"+val+" tween:"+tween+" tween.diff:"+tween.diff);
                if (type == TweenAction.COLOR)
                    colorRecursive(trans, toColor, useRecursion);

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            }
#endif
            if (dt != 0f && _optional.onUpdateColor != null)
                _optional.onUpdateColor(toColor);
            else if (dt != 0f && _optional.onUpdateColorObject != null)
                _optional.onUpdateColorObject(toColor, _optional.onUpdateParam);
        };
        return this;
    }

    private void callback()
    {
        newVect = easeMethod();
        val = newVect.x;
    }

    public LTDescr setCallback()
    {
        type = TweenAction.CALLBACK;
        initInternal = () => { };
        easeInternal = callback;
        return this;
    }

    public LTDescr setValue3()
    {
        type = TweenAction.VALUE3;
        initInternal = () => { };
        easeInternal = callback;
        return this;
    }

    public LTDescr setMove()
    {
        type = TweenAction.MOVE;
        initInternal = () => { from = trans.position; };
        easeInternal = () =>
        {
            newVect = easeMethod();
            trans.position = newVect;
        };
        return this;
    }

    public LTDescr setMoveLocal()
    {
        type = TweenAction.MOVE_LOCAL;
        initInternal = () => { from = trans.localPosition; };
        easeInternal = () =>
        {
            newVect = easeMethod();
            trans.localPosition = newVect;
        };
        return this;
    }

    public LTDescr setMoveToTransform()
    {
        type = TweenAction.MOVE_TO_TRANSFORM;
        initInternal = () => { from = trans.position; };
        easeInternal = () =>
        {
            to = _optional.toTrans.position;
            diff = to - from;
            diffDiv2 = diff * 0.5f;

            newVect = easeMethod();
            trans.position = newVect;
        };
        return this;
    }

    public LTDescr setRotate()
    {
        type = TweenAction.ROTATE;
        initInternal = () =>
        {
            from = trans.eulerAngles;
            to = new Vector3(LeanTween.closestRot(fromInternal.x, toInternal.x), LeanTween.closestRot(from.y, to.y),
                LeanTween.closestRot(from.z, to.z));
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            trans.eulerAngles = newVect;
        };
        return this;
    }

    public LTDescr setRotateLocal()
    {
        type = TweenAction.ROTATE_LOCAL;
        initInternal = () =>
        {
            from = trans.localEulerAngles;
            to = new Vector3(LeanTween.closestRot(fromInternal.x, toInternal.x), LeanTween.closestRot(from.y, to.y),
                LeanTween.closestRot(from.z, to.z));
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            trans.localEulerAngles = newVect;
        };
        return this;
    }

    public LTDescr setScale()
    {
        type = TweenAction.SCALE;
        initInternal = () => { from = trans.localScale; };
        easeInternal = () =>
        {
            newVect = easeMethod();
            trans.localScale = newVect;
        };
        return this;
    }

    public LTDescr setGUIMove()
    {
        type = TweenAction.GUI_MOVE;
        initInternal = () => { from = new Vector3(_optional.ltRect.rect.x, _optional.ltRect.rect.y, 0); };
        easeInternal = () =>
        {
            var v = easeMethod();
            _optional.ltRect.rect = new Rect(v.x, v.y, _optional.ltRect.rect.width, _optional.ltRect.rect.height);
        };
        return this;
    }

    public LTDescr setGUIMoveMargin()
    {
        type = TweenAction.GUI_MOVE_MARGIN;
        initInternal = () => { from = new Vector2(_optional.ltRect.margin.x, _optional.ltRect.margin.y); };
        easeInternal = () =>
        {
            var v = easeMethod();
            _optional.ltRect.margin = new Vector2(v.x, v.y);
        };
        return this;
    }

    public LTDescr setGUIScale()
    {
        type = TweenAction.GUI_SCALE;
        initInternal = () => { from = new Vector3(_optional.ltRect.rect.width, _optional.ltRect.rect.height, 0); };
        easeInternal = () =>
        {
            var v = easeMethod();
            _optional.ltRect.rect = new Rect(_optional.ltRect.rect.x, _optional.ltRect.rect.y, v.x, v.y);
        };
        return this;
    }

    public LTDescr setGUIAlpha()
    {
        type = TweenAction.GUI_ALPHA;
        initInternal = () => { fromInternal.x = _optional.ltRect.alpha; };
        easeInternal = () => { _optional.ltRect.alpha = easeMethod().x; };
        return this;
    }

    public LTDescr setGUIRotate()
    {
        type = TweenAction.GUI_ROTATE;
        initInternal = () =>
        {
            if (_optional.ltRect.rotateEnabled == false)
            {
                _optional.ltRect.rotateEnabled = true;
                _optional.ltRect.resetForRotation();
            }

            fromInternal.x = _optional.ltRect.rotation;
        };
        easeInternal = () => { _optional.ltRect.rotation = easeMethod().x; };
        return this;
    }

    public LTDescr setDelayedSound()
    {
        type = TweenAction.DELAYED_SOUND;
        initInternal = () => { hasExtraOnCompletes = true; };
        easeInternal = callback;
        return this;
    }

    public LTDescr setTarget(Transform trans)
    {
        optional.toTrans = trans;
        return this;
    }

    private void init()
    {
        hasInitiliazed = true;

        usesNormalDt =
            !(useEstimatedTime || useManualTime ||
              useFrames); // only set this to true if it uses non of the other timing modes

        if (useFrames)
            optional.initFrameCount = Time.frameCount;

        if (time <= 0f) // avoid dividing by zero
            time = Mathf.Epsilon;

        if (initInternal != null)
            initInternal();

        diff = to - from;
        diffDiv2 = diff * 0.5f;

        if (_optional.onStart != null)
            _optional.onStart();

        if (onCompleteOnStart)
            callOnCompletes();

        if (speed >= 0) initSpeed();
    }

    private void initSpeed()
    {
        if (type == TweenAction.MOVE_CURVED || type == TweenAction.MOVE_CURVED_LOCAL)
            time = _optional.path.distance / speed;
        else if (type == TweenAction.MOVE_SPLINE || type == TweenAction.MOVE_SPLINE_LOCAL)
            time = _optional.spline.distance / speed;
        else
            time = (to - from).magnitude / speed;
    }

    /**
     * If you need a tween to happen immediately instead of waiting for the next Update call, you can force it with this method
     * 
     * @method updateNow
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 0f ).updateNow();
     */
    public LTDescr updateNow()
    {
        updateInternal();
        return this;
    }

    public bool updateInternal()
    {
        var directionLocal = direction;
        if (usesNormalDt)
        {
            dt = LeanTween.dtActual;
        }
        else if (useEstimatedTime)
        {
            dt = LeanTween.dtEstimated;
        }
        else if (useFrames)
        {
            dt = optional.initFrameCount == 0 ? 0 : 1;
            optional.initFrameCount = Time.frameCount;
        }
        else if (useManualTime)
        {
            dt = LeanTween.dtManual;
        }

//		Debug.Log ("tween:" + this+ " dt:"+dt);
        if (delay <= 0f && directionLocal != 0f)
        {
            if (trans == null)
                return true;

            // initialize if has not done so yet
            if (!hasInitiliazed)
                init();

            dt = dt * directionLocal;
            passed += dt;

            passed = Mathf.Clamp(passed, 0f, time);

            ratioPassed =
                passed / time; // need to clamp when finished so it will finish at the exact spot and not overshoot

            easeInternal();

            if (hasUpdateCallback)
                _optional.callOnUpdate(val, ratioPassed);

            var isTweenFinished = directionLocal > 0f ? passed >= time : passed <= 0f;
            //			Debug.Log("lt "+this+" dt:"+dt+" fin:"+isTweenFinished);
            if (isTweenFinished)
            {
                // increment or flip tween
                loopCount--;
                if (loopType == LeanTweenType.pingPong)
                    direction = 0.0f - directionLocal;
                else
                    passed = Mathf.Epsilon;

                isTweenFinished =
                    loopCount == 0 || loopType == LeanTweenType.once; // only return true if it is fully complete

                if (isTweenFinished == false && onCompleteOnRepeat && hasExtraOnCompletes)
                    callOnCompletes(); // this only gets called if onCompleteOnRepeat is set to true, otherwise LeanTween class takes care of calling it

                return isTweenFinished;
            }
        }
        else
        {
            delay -= dt;
        }

        return false;
    }

    public void callOnCompletes()
    {
        if (type == TweenAction.GUI_ROTATE)
            _optional.ltRect.rotateFinished = true;

        if (type == TweenAction.DELAYED_SOUND)
            AudioSource.PlayClipAtPoint((AudioClip) _optional.onCompleteParam, to, from.x);
        if (_optional.onComplete != null)
            _optional.onComplete();
        else if (_optional.onCompleteObject != null) _optional.onCompleteObject(_optional.onCompleteParam);
    }

    // Helper Methods

    public LTDescr setFromColor(Color col)
    {
        from = new Vector3(0.0f, col.a, 0.0f);
        diff = new Vector3(1.0f, 0.0f, 0.0f);
        _optional.axis = new Vector3(col.r, col.g, col.b);
        return this;
    }

    private static void alphaRecursive(Transform transform, float val, bool useRecursion = true)
    {
        var renderer = transform.gameObject.GetComponent<Renderer>();
        if (renderer != null)
            foreach (var mat in renderer.materials)
                if (mat.HasProperty("_Color"))
                {
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, val);
                }
                else if (mat.HasProperty("_TintColor"))
                {
                    var col = mat.GetColor("_TintColor");
                    mat.SetColor("_TintColor", new Color(col.r, col.g, col.b, val));
                }

        if (useRecursion && transform.childCount > 0)
            foreach (Transform child in transform)
                alphaRecursive(child, val);
    }

    private static void colorRecursive(Transform transform, Color toColor, bool useRecursion = true)
    {
        var ren = transform.gameObject.GetComponent<Renderer>();
        if (ren != null)
            foreach (var mat in ren.materials)
                mat.color = toColor;
        if (useRecursion && transform.childCount > 0)
            foreach (Transform child in transform)
                colorRecursive(child, toColor);
    }

    private static Color tweenColor(LTDescr tween, float val)
    {
        var diff3 = tween._optional.point - tween._optional.axis;
        var diffAlpha = tween.to.y - tween.from.y;
        return new Color(tween._optional.axis.x + diff3.x * val, tween._optional.axis.y + diff3.y * val,
            tween._optional.axis.z + diff3.z * val, tween.from.y + diffAlpha * val);
    }

    /**
     * Pause a tween
     * 
     * @method pause
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     */
    public LTDescr pause()
    {
        if (direction != 0.0f)
        {
            // check if tween is already paused
            directionLast = direction;
            direction = 0.0f;
        }

        return this;
    }

    /**
     * Resume a paused tween
     * 
     * @method resume
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     */
    public LTDescr resume()
    {
        direction = directionLast;

        return this;
    }

    /**
     * Set Axis optional axis for tweens where it is relevant
     * 
     * @method setAxis
     * @param {Vector3} axis either the tween rotates around, or the direction it faces in the case of setOrientToPath
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true).setAxis(Vector3.forward);
     */
    public LTDescr setAxis(Vector3 axis)
    {
        _optional.axis = axis;
        return this;
    }

    /**
     * Delay the start of a tween
     * 
     * @method setDelay
     * @param {float} float time The time to complete the tween in
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setDelay( 1.5f );
     */
    public LTDescr setDelay(float delay)
    {
        this.delay = delay;

        return this;
    }

    /**
     * Set the type of easing used for the tween.
     * <br>
     *     <ul>
     *         <li><a href="LeanTweenType.html">List of all the ease types</a>.</li>
     *         <li>
     *             <a href="http://www.robertpenner.com/easing/easing_demo.html">
     *                 This page helps visualize the different
     *                 easing equations
     *             </a>
     *         </li>
     *     </ul>
     *     @method setEase
     *     @param {LeanTweenType} easeType:LeanTweenType the easing type to use
     *     @return {LTDescr} LTDescr an object that distinguishes the tween
     *     @example
     *     LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
     */
    public LTDescr setEase(LeanTweenType easeType)
    {
        switch (easeType)
        {
            case LeanTweenType.linear:
                setEaseLinear();
                break;
            case LeanTweenType.easeOutQuad:
                setEaseOutQuad();
                break;
            case LeanTweenType.easeInQuad:
                setEaseInQuad();
                break;
            case LeanTweenType.easeInOutQuad:
                setEaseInOutQuad();
                break;
            case LeanTweenType.easeInCubic:
                setEaseInCubic();
                break;
            case LeanTweenType.easeOutCubic:
                setEaseOutCubic();
                break;
            case LeanTweenType.easeInOutCubic:
                setEaseInOutCubic();
                break;
            case LeanTweenType.easeInQuart:
                setEaseInQuart();
                break;
            case LeanTweenType.easeOutQuart:
                setEaseOutQuart();
                break;
            case LeanTweenType.easeInOutQuart:
                setEaseInOutQuart();
                break;
            case LeanTweenType.easeInQuint:
                setEaseInQuint();
                break;
            case LeanTweenType.easeOutQuint:
                setEaseOutQuint();
                break;
            case LeanTweenType.easeInOutQuint:
                setEaseInOutQuint();
                break;
            case LeanTweenType.easeInSine:
                setEaseInSine();
                break;
            case LeanTweenType.easeOutSine:
                setEaseOutSine();
                break;
            case LeanTweenType.easeInOutSine:
                setEaseInOutSine();
                break;
            case LeanTweenType.easeInExpo:
                setEaseInExpo();
                break;
            case LeanTweenType.easeOutExpo:
                setEaseOutExpo();
                break;
            case LeanTweenType.easeInOutExpo:
                setEaseInOutExpo();
                break;
            case LeanTweenType.easeInCirc:
                setEaseInCirc();
                break;
            case LeanTweenType.easeOutCirc:
                setEaseOutCirc();
                break;
            case LeanTweenType.easeInOutCirc:
                setEaseInOutCirc();
                break;
            case LeanTweenType.easeInBounce:
                setEaseInBounce();
                break;
            case LeanTweenType.easeOutBounce:
                setEaseOutBounce();
                break;
            case LeanTweenType.easeInOutBounce:
                setEaseInOutBounce();
                break;
            case LeanTweenType.easeInBack:
                setEaseInBack();
                break;
            case LeanTweenType.easeOutBack:
                setEaseOutBack();
                break;
            case LeanTweenType.easeInOutBack:
                setEaseInOutBack();
                break;
            case LeanTweenType.easeInElastic:
                setEaseInElastic();
                break;
            case LeanTweenType.easeOutElastic:
                setEaseOutElastic();
                break;
            case LeanTweenType.easeInOutElastic:
                setEaseInOutElastic();
                break;
            case LeanTweenType.punch:
                setEasePunch();
                break;
            case LeanTweenType.easeShake:
                setEaseShake();
                break;
            case LeanTweenType.easeSpring:
                setEaseSpring();
                break;
            default:
                setEaseLinear();
                break;
        }

        return this;
    }

    public LTDescr setEaseLinear()
    {
        easeType = LeanTweenType.linear;
        easeMethod = easeLinear;
        return this;
    }

    public LTDescr setEaseSpring()
    {
        easeType = LeanTweenType.easeSpring;
        easeMethod = easeSpring;
        return this;
    }

    public LTDescr setEaseInQuad()
    {
        easeType = LeanTweenType.easeInQuad;
        easeMethod = easeInQuad;
        return this;
    }

    public LTDescr setEaseOutQuad()
    {
        easeType = LeanTweenType.easeOutQuad;
        easeMethod = easeOutQuad;
        return this;
    }

    public LTDescr setEaseInOutQuad()
    {
        easeType = LeanTweenType.easeInOutQuad;
        easeMethod = easeInOutQuad;
        return this;
    }

    public LTDescr setEaseInCubic()
    {
        easeType = LeanTweenType.easeInCubic;
        easeMethod = easeInCubic;
        return this;
    }

    public LTDescr setEaseOutCubic()
    {
        easeType = LeanTweenType.easeOutCubic;
        easeMethod = easeOutCubic;
        return this;
    }

    public LTDescr setEaseInOutCubic()
    {
        easeType = LeanTweenType.easeInOutCubic;
        easeMethod = easeInOutCubic;
        return this;
    }

    public LTDescr setEaseInQuart()
    {
        easeType = LeanTweenType.easeInQuart;
        easeMethod = easeInQuart;
        return this;
    }

    public LTDescr setEaseOutQuart()
    {
        easeType = LeanTweenType.easeOutQuart;
        easeMethod = easeOutQuart;
        return this;
    }

    public LTDescr setEaseInOutQuart()
    {
        easeType = LeanTweenType.easeInOutQuart;
        easeMethod = easeInOutQuart;
        return this;
    }

    public LTDescr setEaseInQuint()
    {
        easeType = LeanTweenType.easeInQuint;
        easeMethod = easeInQuint;
        return this;
    }

    public LTDescr setEaseOutQuint()
    {
        easeType = LeanTweenType.easeOutQuint;
        easeMethod = easeOutQuint;
        return this;
    }

    public LTDescr setEaseInOutQuint()
    {
        easeType = LeanTweenType.easeInOutQuint;
        easeMethod = easeInOutQuint;
        return this;
    }

    public LTDescr setEaseInSine()
    {
        easeType = LeanTweenType.easeInSine;
        easeMethod = easeInSine;
        return this;
    }

    public LTDescr setEaseOutSine()
    {
        easeType = LeanTweenType.easeOutSine;
        easeMethod = easeOutSine;
        return this;
    }

    public LTDescr setEaseInOutSine()
    {
        easeType = LeanTweenType.easeInOutSine;
        easeMethod = easeInOutSine;
        return this;
    }

    public LTDescr setEaseInExpo()
    {
        easeType = LeanTweenType.easeInExpo;
        easeMethod = easeInExpo;
        return this;
    }

    public LTDescr setEaseOutExpo()
    {
        easeType = LeanTweenType.easeOutExpo;
        easeMethod = easeOutExpo;
        return this;
    }

    public LTDescr setEaseInOutExpo()
    {
        easeType = LeanTweenType.easeInOutExpo;
        easeMethod = easeInOutExpo;
        return this;
    }

    public LTDescr setEaseInCirc()
    {
        easeType = LeanTweenType.easeInCirc;
        easeMethod = easeInCirc;
        return this;
    }

    public LTDescr setEaseOutCirc()
    {
        easeType = LeanTweenType.easeOutCirc;
        easeMethod = easeOutCirc;
        return this;
    }

    public LTDescr setEaseInOutCirc()
    {
        easeType = LeanTweenType.easeInOutCirc;
        easeMethod = easeInOutCirc;
        return this;
    }

    public LTDescr setEaseInBounce()
    {
        easeType = LeanTweenType.easeInBounce;
        easeMethod = easeInBounce;
        return this;
    }

    public LTDescr setEaseOutBounce()
    {
        easeType = LeanTweenType.easeOutBounce;
        easeMethod = easeOutBounce;
        return this;
    }

    public LTDescr setEaseInOutBounce()
    {
        easeType = LeanTweenType.easeInOutBounce;
        easeMethod = easeInOutBounce;
        return this;
    }

    public LTDescr setEaseInBack()
    {
        easeType = LeanTweenType.easeInBack;
        easeMethod = easeInBack;
        return this;
    }

    public LTDescr setEaseOutBack()
    {
        easeType = LeanTweenType.easeOutBack;
        easeMethod = easeOutBack;
        return this;
    }

    public LTDescr setEaseInOutBack()
    {
        easeType = LeanTweenType.easeInOutBack;
        easeMethod = easeInOutBack;
        return this;
    }

    public LTDescr setEaseInElastic()
    {
        easeType = LeanTweenType.easeInElastic;
        easeMethod = easeInElastic;
        return this;
    }

    public LTDescr setEaseOutElastic()
    {
        easeType = LeanTweenType.easeOutElastic;
        easeMethod = easeOutElastic;
        return this;
    }

    public LTDescr setEaseInOutElastic()
    {
        easeType = LeanTweenType.easeInOutElastic;
        easeMethod = easeInOutElastic;
        return this;
    }

    public LTDescr setEasePunch()
    {
        _optional.animationCurve = LeanTween.punch;
        toInternal.x = from.x + to.x;
        easeMethod = tweenOnCurve;
        return this;
    }

    public LTDescr setEaseShake()
    {
        _optional.animationCurve = LeanTween.shake;
        toInternal.x = from.x + to.x;
        easeMethod = tweenOnCurve;
        return this;
    }

    private Vector3 tweenOnCurve()
    {
        return new Vector3(from.x + diff.x * _optional.animationCurve.Evaluate(ratioPassed),
            from.y + diff.y * _optional.animationCurve.Evaluate(ratioPassed),
            from.z + diff.z * _optional.animationCurve.Evaluate(ratioPassed));
    }

    // Vector3 Ease Methods

    private Vector3 easeInOutQuad()
    {
        val = ratioPassed * 2f;

        if (val < 1f)
        {
            val = val * val;
            return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
        }

        val = (1f - val) * (val - 3f) + 1f;
        return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
    }

    private Vector3 easeInQuad()
    {
        val = ratioPassed * ratioPassed;
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeOutQuad()
    {
        val = ratioPassed;
        val = -val * (val - 2f);
        return diff * val + from;
    }

    private Vector3 easeLinear()
    {
        val = ratioPassed;
        return new Vector3(from.x + diff.x * val, from.y + diff.y * val, from.z + diff.z * val);
    }

    private Vector3 easeSpring()
    {
        val = Mathf.Clamp01(ratioPassed);
        val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) *
              (1f + 1.2f * (1f - val));
        return from + diff * val;
    }

    private Vector3 easeInCubic()
    {
        val = ratioPassed * ratioPassed * ratioPassed;
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeOutCubic()
    {
        val = ratioPassed - 1f;
        val = val * val * val + 1;
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeInOutCubic()
    {
        val = ratioPassed * 2f;
        if (val < 1f)
        {
            val = val * val * val;
            return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
        }

        val -= 2f;
        val = val * val * val + 2f;
        return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
    }

    private Vector3 easeInQuart()
    {
        val = ratioPassed * ratioPassed * ratioPassed * ratioPassed;
        return diff * val + from;
    }

    private Vector3 easeOutQuart()
    {
        val = ratioPassed - 1f;
        val = -(val * val * val * val - 1);
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeInOutQuart()
    {
        val = ratioPassed * 2f;
        if (val < 1f)
        {
            val = val * val * val * val;
            return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
        }

        val -= 2f;
//		val = (val * val * val * val - 2f);
        return -diffDiv2 * (val * val * val * val - 2f) + from;
    }

    private Vector3 easeInQuint()
    {
        val = ratioPassed;
        val = val * val * val * val * val;
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeOutQuint()
    {
        val = ratioPassed - 1f;
        val = val * val * val * val * val + 1f;
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeInOutQuint()
    {
        val = ratioPassed * 2f;
        if (val < 1f)
        {
            val = val * val * val * val * val;
            return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
        }

        val -= 2f;
        val = val * val * val * val * val + 2f;
        return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
    }

    private Vector3 easeInSine()
    {
        val = -Mathf.Cos(ratioPassed * LeanTween.PI_DIV2);
        return new Vector3(diff.x * val + diff.x + from.x, diff.y * val + diff.y + from.y,
            diff.z * val + diff.z + from.z);
    }

    private Vector3 easeOutSine()
    {
        val = Mathf.Sin(ratioPassed * LeanTween.PI_DIV2);
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeInOutSine()
    {
        val = -(Mathf.Cos(Mathf.PI * ratioPassed) - 1f);
        return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
    }

    private Vector3 easeInExpo()
    {
        val = Mathf.Pow(2f, 10f * (ratioPassed - 1f));
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeOutExpo()
    {
        val = -Mathf.Pow(2f, -10f * ratioPassed) + 1f;
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeInOutExpo()
    {
        val = ratioPassed * 2f;
        if (val < 1) return diffDiv2 * Mathf.Pow(2, 10 * (val - 1)) + from;
        val--;
        return diffDiv2 * (-Mathf.Pow(2, -10 * val) + 2) + from;
    }

    private Vector3 easeInCirc()
    {
        val = -(Mathf.Sqrt(1f - ratioPassed * ratioPassed) - 1f);
        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeOutCirc()
    {
        val = ratioPassed - 1f;
        val = Mathf.Sqrt(1f - val * val);

        return new Vector3(diff.x * val + from.x, diff.y * val + from.y, diff.z * val + from.z);
    }

    private Vector3 easeInOutCirc()
    {
        val = ratioPassed * 2f;
        if (val < 1f)
        {
            val = -(Mathf.Sqrt(1f - val * val) - 1f);
            return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
        }

        val -= 2f;
        val = Mathf.Sqrt(1f - val * val) + 1f;
        return new Vector3(diffDiv2.x * val + from.x, diffDiv2.y * val + from.y, diffDiv2.z * val + from.z);
    }

    private Vector3 easeInBounce()
    {
        val = ratioPassed;
        val = 1f - val;
        return new Vector3(diff.x - LeanTween.easeOutBounce(0, diff.x, val) + from.x,
            diff.y - LeanTween.easeOutBounce(0, diff.y, val) + from.y,
            diff.z - LeanTween.easeOutBounce(0, diff.z, val) + from.z);
    }

    private Vector3 easeOutBounce()
    {
        val = ratioPassed;
        float valM, valN; // bounce values
        if (val < (valM = 1 - 1.75f * overshoot / 2.75f))
        {
            val = 1 / valM / valM * val * val;
        }
        else if (val < (valN = 1 - .75f * overshoot / 2.75f))
        {
            val -= (valM + valN) / 2;
            // first bounce, height: 1/4
            val = 7.5625f * val * val + 1 - .25f * overshoot * overshoot;
        }
        else if (val < (valM = 1 - .25f * overshoot / 2.75f))
        {
            val -= (valM + valN) / 2;
            // second bounce, height: 1/16
            val = 7.5625f * val * val + 1 - .0625f * overshoot * overshoot;
        }
        else
        {
            // valN = 1
            val -= (valM + 1) / 2;
            // third bounce, height: 1/64
            val = 7.5625f * val * val + 1 - .015625f * overshoot * overshoot;
        }

        return diff * val + from;
    }

    private Vector3 easeInOutBounce()
    {
        val = ratioPassed * 2f;
        if (val < 1f)
            return new Vector3(LeanTween.easeInBounce(0, diff.x, val) * 0.5f + @from.x,
                LeanTween.easeInBounce(0, diff.y, val) * 0.5f + @from.y,
                LeanTween.easeInBounce(0, diff.z, val) * 0.5f + @from.z);
        val = val - 1f;
        return new Vector3(LeanTween.easeOutBounce(0, diff.x, val) * 0.5f + diffDiv2.x + from.x,
            LeanTween.easeOutBounce(0, diff.y, val) * 0.5f + diffDiv2.y + from.y,
            LeanTween.easeOutBounce(0, diff.z, val) * 0.5f + diffDiv2.z + from.z);
    }

    private Vector3 easeInBack()
    {
        val = ratioPassed;
        val /= 1;
        var s = 1.70158f * overshoot;
        return diff * val * val * ((s + 1) * val - s) + from;
    }

    private Vector3 easeOutBack()
    {
        var s = 1.70158f * overshoot;
        val = ratioPassed / 1 - 1;
        val = val * val * ((s + 1) * val + s) + 1;
        return diff * val + from;
    }

    private Vector3 easeInOutBack()
    {
        var s = 1.70158f * overshoot;
        val = ratioPassed * 2f;
        if (val < 1)
        {
            s *= 1.525f * overshoot;
            return diffDiv2 * (val * val * ((s + 1) * val - s)) + from;
        }

        val -= 2;
        s *= 1.525f * overshoot;
        val = val * val * ((s + 1) * val + s) + 2;
        return diffDiv2 * val + from;
    }

    private Vector3 easeInElastic()
    {
        return new Vector3(LeanTween.easeInElastic(from.x, to.x, ratioPassed, overshoot, period),
            LeanTween.easeInElastic(from.y, to.y, ratioPassed, overshoot, period),
            LeanTween.easeInElastic(from.z, to.z, ratioPassed, overshoot, period));
    }

    private Vector3 easeOutElastic()
    {
        return new Vector3(LeanTween.easeOutElastic(from.x, to.x, ratioPassed, overshoot, period),
            LeanTween.easeOutElastic(from.y, to.y, ratioPassed, overshoot, period),
            LeanTween.easeOutElastic(from.z, to.z, ratioPassed, overshoot, period));
    }

    private Vector3 easeInOutElastic()
    {
        return new Vector3(LeanTween.easeInOutElastic(from.x, to.x, ratioPassed, overshoot, period),
            LeanTween.easeInOutElastic(from.y, to.y, ratioPassed, overshoot, period),
            LeanTween.easeInOutElastic(from.z, to.z, ratioPassed, overshoot, period));
    }

    /**
     * Set how far past a tween will overshoot  for certain ease types (compatible:  easeInBack, easeInOutBack, easeOutBack, easeOutElastic, easeInElastic, easeInOutElastic).
     * <br>
     *     @method setOvershoot
     *     @param {float} overshoot:float how far past the destination it will go before settling in
     *     @return {LTDescr} LTDescr an object that distinguishes the tween
     *     @example
     *     LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeOutBack ).setOvershoot(2f);
     */
    public LTDescr setOvershoot(float overshoot)
    {
        this.overshoot = overshoot;
        return this;
    }

    /**
     * Set how short the iterations are for certain ease types (compatible: easeOutElastic, easeInElastic, easeInOutElastic).
     * <br>
     *     @method setPeriod
     *     @param {float} period:float how short the iterations are that the tween will animate at (default 0.3f)
     *     @return {LTDescr} LTDescr an object that distinguishes the tween
     *     @example
     *     LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeOutElastic ).setPeriod(0.3f);
     */
    public LTDescr setPeriod(float period)
    {
        this.period = period;
        return this;
    }

    /**
     * Set how large the effect is for certain ease types (compatible: punch, shake, animation curves).
     * <br>
     *     @method setScale
     *     @param {float} scale:float how much the ease will be multiplied by (default 1f)
     *     @return {LTDescr} LTDescr an object that distinguishes the tween
     *     @example
     *     LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.punch ).setScale(2f);
     */
    public LTDescr setScale(float scale)
    {
        this.scale = scale;
        return this;
    }

    /**
     * Set the type of easing used for the tween with a custom curve.
     * <br>
     *     @method setEase (AnimationCurve)
     *     @param {AnimationCurve} easeDefinition:AnimationCurve an
     *     <a href="http://docs.unity3d.com/Documentation/ScriptReference/AnimationCurve.html" target="_blank">AnimationCure</a>
     *     that describes the type of easing you want, this is great for when you want a unique type of movement
     *     @return {LTDescr} LTDescr an object that distinguishes the tween
     *     @example
     *     LeanTween.moveX(gameObject, 5f, 2.0f ).setEase( LeanTweenType.easeInBounce );
     */
    public LTDescr setEase(AnimationCurve easeCurve)
    {
        _optional.animationCurve = easeCurve;
        easeMethod = tweenOnCurve;
        easeType = LeanTweenType.animationCurve;
        return this;
    }

    /**
     * Set the end that the GameObject is tweening towards
     * @method setTo
     * @param {Vector3} to:Vector3 point at which you want the tween to reach
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LTDescr descr = LeanTween.move( cube, Vector3.up, new Vector3(1f,3f,0f), 1.0f ).setEase( LeanTweenType.easeInOutBounce );
     * <br>
     *     // Later your want to change your destination or your destiation is constantly moving
     *     <br>
     *         descr.setTo( new Vector3(5f,10f,3f) );<br>
     */
    public LTDescr setTo(Vector3 to)
    {
        if (hasInitiliazed)
        {
            this.to = to;
            diff = to - from;
        }
        else
        {
            this.to = to;
        }

        return this;
    }

    public LTDescr setTo(Transform to)
    {
        _optional.toTrans = to;
        return this;
    }

    /**
     * Set the beginning of the tween
     * @method setFrom
     * @param {Vector3} from:Vector3 the point you would like the tween to start at
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LTDescr descr = LeanTween.move( cube, Vector3.up, new Vector3(1f,3f,0f), 1.0f ).setFrom( new Vector3(5f,10f,3f) );
     * <br>
     */
    public LTDescr setFrom(Vector3 from)
    {
        if (trans) init();
        this.from = from;
        // this.hasInitiliazed = true; // this is set, so that the "from" value isn't overwritten later on when the tween starts
        diff = to - this.from;
        diffDiv2 = diff * 0.5f;
        return this;
    }

    public LTDescr setFrom(float from)
    {
        return setFrom(new Vector3(from, 0f, 0f));
    }

    public LTDescr setDiff(Vector3 diff)
    {
        this.diff = diff;
        return this;
    }

    public LTDescr setHasInitialized(bool has)
    {
        hasInitiliazed = has;
        return this;
    }

    public LTDescr setId(uint id, uint global_counter)
    {
        _id = id;
        counter = global_counter;
        // Debug.Log("Global counter:"+global_counter);
        return this;
    }

    /**
     * Set the point of time the tween will start in
     * @method setPassed
     * @param {float} passedTime:float the length of time in seconds the tween will start in
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * int tweenId = LeanTween.moveX(gameObject, 5f, 2.0f ).id;
     * <br>
     *     // Later
     *     <br>
     *         LTDescr descr = description( tweenId );
     *         <br>
     *             descr.setPassed( 1f );<br>
     */
    public LTDescr setPassed(float passed)
    {
        this.passed = passed;
        return this;
    }

    /**
     * Set the finish time of the tween
     * @method setTime
     * @param {float} finishTime:float the length of time in seconds you wish the tween to complete in
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * int tweenId = LeanTween.moveX(gameObject, 5f, 2.0f ).id;
     * <br>
     *     // Later
     *     <br>
     *         LTDescr descr = description( tweenId );
     *         <br>
     *             descr.setTime( 1f );<br>
     */
    public LTDescr setTime(float time)
    {
        var passedTimeRatio = passed / this.time;
        passed = time * passedTimeRatio;
        this.time = time;
        return this;
    }

    /**
     * Set the finish time of the tween
     * @method setSpeed
     * @param {float} speed:float the speed in unity units per second you wish the object to travel (overrides the given time)
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveLocalZ( gameObject, 10f, 1f).setSpeed(0.2f) // the given time is ignored when speed is set
     * <br>
     */
    public LTDescr setSpeed(float speed)
    {
        this.speed = speed;
        if (hasInitiliazed)
            initSpeed();
        return this;
    }

    /**
     * Set the tween to repeat a number of times.
     * @method setRepeat
     * @param {int} repeatNum:int the number of times to repeat the tween. -1 to repeat infinite times
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 10 ).setLoopPingPong();
     */
    public LTDescr setRepeat(int repeat)
    {
        loopCount = repeat;
        if (repeat > 1 && loopType == LeanTweenType.once || repeat < 0 && loopType == LeanTweenType.once)
            loopType = LeanTweenType.clamp;
        if (type == TweenAction.CALLBACK || type == TweenAction.CALLBACK_COLOR) setOnCompleteOnRepeat(true);
        return this;
    }

    public LTDescr setLoopType(LeanTweenType loopType)
    {
        this.loopType = loopType;
        return this;
    }

    public LTDescr setUseEstimatedTime(bool useEstimatedTime)
    {
        this.useEstimatedTime = useEstimatedTime;
        usesNormalDt = false;
        return this;
    }

    /**
     * Set ignore time scale when tweening an object when you want the animation to be time-scale independent (ignores the Time.timeScale value). Great for pause screens, when you want all other action to be stopped (or slowed down)
     * @method setIgnoreTimeScale
     * @param {bool} useUnScaledTime:bool whether to use the unscaled time or not
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setIgnoreTimeScale( true );
     */
    public LTDescr setIgnoreTimeScale(bool useUnScaledTime)
    {
        useEstimatedTime = useUnScaledTime;
        usesNormalDt = false;
        return this;
    }

    /**
     * Use frames when tweening an object, when you don't want the animation to be time-frame independent...
     * @method setUseFrames
     * @param {bool} useFrames:bool whether to use estimated time or not
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setRepeat( 2 ).setUseFrames( true );
     */
    public LTDescr setUseFrames(bool useFrames)
    {
        this.useFrames = useFrames;
        usesNormalDt = false;
        return this;
    }

    public LTDescr setUseManualTime(bool useManualTime)
    {
        this.useManualTime = useManualTime;
        usesNormalDt = false;
        return this;
    }

    public LTDescr setLoopCount(int loopCount)
    {
        loopType = LeanTweenType.clamp;
        this.loopCount = loopCount;
        return this;
    }

    /**
     * No looping involved, just run once (the default)
     * @method setLoopOnce
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopOnce();
     */
    public LTDescr setLoopOnce()
    {
        loopType = LeanTweenType.once;
        return this;
    }

    /**
     * When the animation gets to the end it starts back at where it began
     * @method setLoopClamp
     * @param {int} loops:int (defaults to -1) how many times you want the loop to happen (-1 for an infinite number of times)
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopClamp( 2 );
     */
    public LTDescr setLoopClamp()
    {
        loopType = LeanTweenType.clamp;
        if (loopCount == 0)
            loopCount = -1;
        return this;
    }

    public LTDescr setLoopClamp(int loops)
    {
        loopCount = loops;
        return this;
    }

    /**
     * When the animation gets to the end it then tweens back to where it started (and on, and on)
     * @method setLoopPingPong
     * @param {int} loops:int (defaults to -1) how many times you want the loop to happen in both directions (-1 for an infinite number of times). Passing a value of 1 will cause the object to go towards and back from it's destination once.
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setLoopPingPong( 2 );
     */
    public LTDescr setLoopPingPong()
    {
        loopType = LeanTweenType.pingPong;
        if (loopCount == 0)
            loopCount = -1;
        return this;
    }

    public LTDescr setLoopPingPong(int loops)
    {
        loopType = LeanTweenType.pingPong;
        loopCount = loops == -1 ? loops : loops * 2;
        return this;
    }

    /**
     * Have a method called when the tween finishes
     * @method setOnComplete
     * @param {Action} onComplete:Action the method that should be called when the tween is finished ex: tweenFinished(){ }
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished );
     */
    public LTDescr setOnComplete(Action onComplete)
    {
        _optional.onComplete = onComplete;
        hasExtraOnCompletes = true;
        return this;
    }

    /**
     * Have a method called when the tween finishes
     * @method setOnComplete (object)
     * @param {Action
     * <object>
     *     } onComplete:Action
     *     <object>
     *         the method that should be called when the tween is finished ex: tweenFinished( object myObj ){ }
     *         @return {LTDescr} LTDescr an object that distinguishes the tween
     *         @example
     *         object tweenFinishedObj = "hi" as object;
     *         LeanTween.moveX(gameObject, 5f, 2.0f ).setOnComplete( tweenFinished, tweenFinishedObj );
     */
    public LTDescr setOnComplete(Action<object> onComplete)
    {
        _optional.onCompleteObject = onComplete;
        hasExtraOnCompletes = true;
        return this;
    }

    public LTDescr setOnComplete(Action<object> onComplete, object onCompleteParam)
    {
        _optional.onCompleteObject = onComplete;
        hasExtraOnCompletes = true;
        if (onCompleteParam != null)
            _optional.onCompleteParam = onCompleteParam;
        return this;
    }

    /**
     * * Pass an object to along with the onComplete Function
     * * @method setOnCompleteParam
     * * @param {object} onComplete:object an object that 
     * * @return {LTDescr} LTDescr an object that distinguishes the tween
     * * @example
     * * LeanTween.delayedCall(1.5f, enterMiniGameStart).setOnCompleteParam( new object[]{""+5} );
     * <br>
     *     <br>
     *         * void enterMiniGameStart( object val ){
     *         <br>
     *             * &nbsp;object[] arr = (object [])val;
     *             <br>
     *                 * &nbsp;int lvl = int.Parse((string)arr[0]);
     *                 <br>
     *                     * }<br>
     */
    public LTDescr setOnCompleteParam(object onCompleteParam)
    {
        _optional.onCompleteParam = onCompleteParam;
        hasExtraOnCompletes = true;
        return this;
    }


    /**
     * Have a method called on each frame that the tween is being animated (passes a float value)
     * @method setOnUpdate
     * @param {Action
     * <float>
     *     } onUpdate:Action
     *     <float>
     *         a method that will be called on every frame with the float value of the tweened object
     *         @return {LTDescr} LTDescr an object that distinguishes the tween
     *         @example
     *         LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );
     *         <br>
     *             <br>
     *                 void tweenMoved( float val ){ }<br>
     */
    public LTDescr setOnUpdate(Action<float> onUpdate)
    {
        _optional.onUpdateFloat = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    public LTDescr setOnUpdateRatio(Action<float, float> onUpdate)
    {
        _optional.onUpdateFloatRatio = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    public LTDescr setOnUpdateObject(Action<float, object> onUpdate)
    {
        _optional.onUpdateFloatObject = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    public LTDescr setOnUpdateVector2(Action<Vector2> onUpdate)
    {
        _optional.onUpdateVector2 = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    public LTDescr setOnUpdateVector3(Action<Vector3> onUpdate)
    {
        _optional.onUpdateVector3 = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    public LTDescr setOnUpdateColor(Action<Color> onUpdate)
    {
        _optional.onUpdateColor = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    public LTDescr setOnUpdateColor(Action<Color, object> onUpdate)
    {
        _optional.onUpdateColorObject = onUpdate;
        hasUpdateCallback = true;
        return this;
    }


    /**
     * Have an object passed along with the onUpdate method
     * @method setOnUpdateParam
     * @param {object} onUpdateParam:object an object that will be passed along with the onUpdate method
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );
     * <br>
     *     <br>
     *         void tweenMoved( float val, object obj ){ }<br>
     */
    public LTDescr setOnUpdateParam(object onUpdateParam)
    {
        _optional.onUpdateParam = onUpdateParam;
        return this;
    }

    /**
     * While tweening along a curve, set this property to true, to be perpendicalur to the path it is moving upon
     * @method setOrientToPath
     * @param {bool} doesOrient:bool whether the gameobject will orient to the path it is animating along
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath(true).setAxis(Vector3.forward);
     * <br>
     */
    public LTDescr setOrientToPath(bool doesOrient)
    {
        if (type == TweenAction.MOVE_CURVED || type == TweenAction.MOVE_CURVED_LOCAL)
        {
            if (_optional.path == null)
                _optional.path = new LTBezierPath();
            _optional.path.orientToPath = doesOrient;
        }
        else
        {
            _optional.spline.orientToPath = doesOrient;
        }

        return this;
    }

    /**
     * While tweening along a curve, set this property to true, to be perpendicalur to the path it is moving upon
     * @method setOrientToPath2d
     * @param {bool} doesOrient:bool whether the gameobject will orient to the path it is animating along
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.move( ltLogo, path, 1.0f ).setEase(LeanTweenType.easeOutQuad).setOrientToPath2d(true).setAxis(Vector3.forward);
     * <br>
     */
    public LTDescr setOrientToPath2d(bool doesOrient2d)
    {
        setOrientToPath(doesOrient2d);
        if (type == TweenAction.MOVE_CURVED || type == TweenAction.MOVE_CURVED_LOCAL)
            _optional.path.orientToPath2d = doesOrient2d;
        else
            _optional.spline.orientToPath2d = doesOrient2d;
        return this;
    }

    public LTDescr setRect(LTRect rect)
    {
        _optional.ltRect = rect;
        return this;
    }

    public LTDescr setRect(Rect rect)
    {
        _optional.ltRect = new LTRect(rect);
        return this;
    }

    public LTDescr setPath(LTBezierPath path)
    {
        _optional.path = path;
        return this;
    }

    /**
     * Set the point at which the GameObject will be rotated around
     * @method setPoint
     * @param {Vector3} point:Vector3 point at which you want the object to rotate around (local space)
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.rotateAround( cube, Vector3.up, 360.0f, 1.0f ) .setPoint( new Vector3(1f,0f,0f) ) .setEase( LeanTweenType.easeInOutBounce );
     * <br>
     */
    public LTDescr setPoint(Vector3 point)
    {
        _optional.point = point;
        return this;
    }

    public LTDescr setDestroyOnComplete(bool doesDestroy)
    {
        destroyOnComplete = doesDestroy;
        return this;
    }

    public LTDescr setAudio(object audio)
    {
        _optional.onCompleteParam = audio;
        return this;
    }

    /**
     * Set the onComplete method to be called at the end of every loop cycle (also applies to the delayedCall method)
     * @method setOnCompleteOnRepeat
     * @param {bool} isOn:bool does call onComplete on every loop cycle
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.delayedCall(gameObject,0.3f, delayedMethod).setRepeat(4).setOnCompleteOnRepeat(true);
     */
    public LTDescr setOnCompleteOnRepeat(bool isOn)
    {
        onCompleteOnRepeat = isOn;
        return this;
    }

    /**
     * Set the onComplete method to be called at the beginning of the tween (it will still be called when it is completed as well)
     * @method setOnCompleteOnStart
     * @param {bool} isOn:bool does call onComplete at the start of the tween
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * LeanTween.delayedCall(gameObject, 2f, ()=>{
     * <br>
     *     // Flash an object 5 times
     *     &nbsp;LeanTween.alpha(gameObject, 0f, 1f);
     *     <br>
     *         &nbsp;LeanTween.alpha(gameObject, 1f, 0f).setDelay(1f);
     *         <br>
     *             }).setOnCompleteOnStart(true).setRepeat(5);<br>
     */
    public LTDescr setOnCompleteOnStart(bool isOn)
    {
        onCompleteOnStart = isOn;
        return this;
    }

    /**
     * Have a method called when the tween starts
     * @method setOnStart
     * @param {Action
     * <
     * >
     * } onStart:Action
     * <
     * >
     * the method that should be called when the tween is starting ex: tweenStarted( ){ }
     * @return {LTDescr} LTDescr an object that distinguishes the tween
     * @example
     * <i>C#:</i>
     * <br>
     *     LeanTween.moveX(gameObject, 5f, 2.0f ).setOnStart( ()=>{ Debug.Log("I started!"); });
     *     <i>Javascript:</i>
     *     <br>
     *         LeanTween.moveX(gameObject, 5f, 2.0f ).setOnStart( function(){ Debug.Log("I started!"); } );
     */
    public LTDescr setOnStart(Action onStart)
    {
        _optional.onStart = onStart;
        return this;
    }

    /**
     * * Set the direction of a tween -1f for backwards 1f for forwards (currently only bezier and spline paths are supported)
     * * @method setDirection
     * * @param {float} direction:float the direction that the tween should run, -1f for backwards 1f for forwards
     * * @return {LTDescr} LTDescr an object that distinguishes the tween
     * * @example
     * * LeanTween.moveSpline(gameObject, new Vector3[]{new Vector3(0f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,0f),new Vector3(1f,0f,1f)}, 1.5f).setDirection(-1f);
     * <br>
     */
    public LTDescr setDirection(float direction)
    {
        if (this.direction != -1f && this.direction != 1f)
        {
            Debug.LogWarning("You have passed an incorrect direction of '" + direction +
                             "', direction must be -1f or 1f");
            return this;
        }

        if (this.direction != direction)
        {
            // Debug.Log("reverse path:"+this.path+" spline:"+this._optional.spline+" hasInitiliazed:"+this.hasInitiliazed);
            if (hasInitiliazed)
            {
                this.direction = direction;
            }
            else
            {
                if (_optional.path != null)
                    _optional.path = new LTBezierPath(LTUtility.reverse(_optional.path.pts));
                else if (_optional.spline != null)
                    _optional.spline = new LTSpline(LTUtility.reverse(_optional.spline.pts));
                // this.passed = this.time - this.passed;
            }
        }

        return this;
    }

    /**
     * * Set whether or not the tween will recursively effect an objects children in the hierarchy
     * * @method setRecursive
     * * @param {bool} useRecursion:bool whether the tween will recursively effect an objects children in the hierarchy
     * * @return {LTDescr} LTDescr an object that distinguishes the tween
     * * @example
     * * LeanTween.alpha(gameObject, 0f, 1f).setRecursive(true);
     * <br>
     */
    public LTDescr setRecursive(bool useRecursion)
    {
        this.useRecursion = useRecursion;

        return this;
    }

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
    public RectTransform rectTransform;
    public Text uiText;
    public Image uiImage;
    public RawImage rawImage;
    public Sprite[] sprites;
#endif


#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5

    public LTDescr setTextColor()
    {
        type = TweenAction.TEXT_COLOR;
        initInternal = () =>
        {
            uiText = trans.GetComponent<Text>();
            setFromColor(uiText != null ? uiText.color : Color.white);
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var toColor = tweenColor(this, val);
            uiText.color = toColor;
            if (dt != 0f && _optional.onUpdateColor != null)
                _optional.onUpdateColor(toColor);

            if (useRecursion && trans.childCount > 0)
                textColorRecursive(trans, toColor);
        };
        return this;
    }

    public LTDescr setCanvasAlpha()
    {
        type = TweenAction.CANVAS_ALPHA;
        initInternal = () =>
        {
            uiImage = trans.GetComponent<Image>();
            if (uiImage != null)
            {
                fromInternal.x = uiImage.color.a;
            }
            else
            {
                rawImage = trans.GetComponent<RawImage>();
                if (rawImage != null)
                    fromInternal.x = rawImage.color.a;
                else
                    fromInternal.x = 1f;
            }
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            if (uiImage != null)
            {
                var c = uiImage.color;
                c.a = val;
                uiImage.color = c;
            }
            else if (rawImage != null)
            {
                var c = rawImage.color;
                c.a = val;
                rawImage.color = c;
            }

            if (useRecursion)
            {
                alphaRecursive(rectTransform, val);
                textAlphaChildrenRecursive(rectTransform, val);
            }
        };
        return this;
    }

    public LTDescr setCanvasGroupAlpha()
    {
        type = TweenAction.CANVASGROUP_ALPHA;
        initInternal = () => { fromInternal.x = trans.GetComponent<CanvasGroup>().alpha; };
        easeInternal = () => { trans.GetComponent<CanvasGroup>().alpha = easeMethod().x; };
        return this;
    }

    public LTDescr setCanvasColor()
    {
        type = TweenAction.CANVAS_COLOR;
        initInternal = () =>
        {
            uiImage = trans.GetComponent<Image>();
            if (uiImage == null)
            {
                rawImage = trans.GetComponent<RawImage>();
                setFromColor(rawImage != null ? rawImage.color : Color.white);
            }
            else
            {
                setFromColor(uiImage.color);
            }
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var toColor = tweenColor(this, val);
            if (uiImage != null)
                uiImage.color = toColor;
            else if (rawImage != null) rawImage.color = toColor;

            if (dt != 0f && _optional.onUpdateColor != null)
                _optional.onUpdateColor(toColor);

            if (useRecursion)
                colorRecursive(rectTransform, toColor);
        };
        return this;
    }

    public LTDescr setCanvasMoveX()
    {
        type = TweenAction.CANVAS_MOVE_X;
        initInternal = () => { fromInternal.x = rectTransform.anchoredPosition3D.x; };
        easeInternal = () =>
        {
            var c = rectTransform.anchoredPosition3D;
            rectTransform.anchoredPosition3D = new Vector3(easeMethod().x, c.y, c.z);
        };
        return this;
    }

    public LTDescr setCanvasMoveY()
    {
        type = TweenAction.CANVAS_MOVE_Y;
        initInternal = () => { fromInternal.x = rectTransform.anchoredPosition3D.y; };
        easeInternal = () =>
        {
            var c = rectTransform.anchoredPosition3D;
            rectTransform.anchoredPosition3D = new Vector3(c.x, easeMethod().x, c.z);
        };
        return this;
    }

    public LTDescr setCanvasMoveZ()
    {
        type = TweenAction.CANVAS_MOVE_Z;
        initInternal = () => { fromInternal.x = rectTransform.anchoredPosition3D.z; };
        easeInternal = () =>
        {
            var c = rectTransform.anchoredPosition3D;
            rectTransform.anchoredPosition3D = new Vector3(c.x, c.y, easeMethod().x);
        };
        return this;
    }

    private void initCanvasRotateAround()
    {
        lastVal = 0.0f;
        fromInternal.x = 0.0f;
        _optional.origRotation = rectTransform.rotation;
    }

    public LTDescr setCanvasRotateAround()
    {
        type = TweenAction.CANVAS_ROTATEAROUND;
        initInternal = initCanvasRotateAround;
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var rect = rectTransform;
            var origPos = rect.localPosition;
            rect.RotateAround(rect.TransformPoint(_optional.point), _optional.axis, -val);
            var diff = origPos - rect.localPosition;

            rect.localPosition =
                origPos - diff; // Subtract the amount the object has been shifted over by the rotate, to get it back to it's orginal position
            rect.rotation = _optional.origRotation;
            rect.RotateAround(rect.TransformPoint(_optional.point), _optional.axis, val);
        };
        return this;
    }

    public LTDescr setCanvasRotateAroundLocal()
    {
        type = TweenAction.CANVAS_ROTATEAROUND_LOCAL;
        initInternal = initCanvasRotateAround;
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var rect = rectTransform;
            var origPos = rect.localPosition;
            rect.RotateAround(rect.TransformPoint(_optional.point), rect.TransformDirection(_optional.axis), -val);
            var diff = origPos - rect.localPosition;

            rect.localPosition =
                origPos - diff; // Subtract the amount the object has been shifted over by the rotate, to get it back to it's orginal position
            rect.rotation = _optional.origRotation;
            rect.RotateAround(rect.TransformPoint(_optional.point), rect.TransformDirection(_optional.axis), val);
        };
        return this;
    }

    public LTDescr setCanvasPlaySprite()
    {
        type = TweenAction.CANVAS_PLAYSPRITE;
        initInternal = () =>
        {
            uiImage = trans.GetComponent<Image>();
            fromInternal.x = 0f;
        };
        easeInternal = () =>
        {
            newVect = easeMethod();
            val = newVect.x;
            var frame = (int) Mathf.Round(val);
            uiImage.sprite = sprites[frame];
        };
        return this;
    }

    public LTDescr setCanvasMove()
    {
        type = TweenAction.CANVAS_MOVE;
        initInternal = () => { fromInternal = rectTransform.anchoredPosition3D; };
        easeInternal = () => { rectTransform.anchoredPosition3D = easeMethod(); };
        return this;
    }

    public LTDescr setCanvasScale()
    {
        type = TweenAction.CANVAS_SCALE;
        initInternal = () => { from = rectTransform.localScale; };
        easeInternal = () => { rectTransform.localScale = easeMethod(); };
        return this;
    }

    public LTDescr setCanvasSizeDelta()
    {
        type = TweenAction.CANVAS_SIZEDELTA;
        initInternal = () => { from = rectTransform.sizeDelta; };
        easeInternal = () => { rectTransform.sizeDelta = easeMethod(); };
        return this;
    }
#endif

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5

    private static void alphaRecursive(RectTransform rectTransform, float val, int recursiveLevel = 0)
    {
        if (rectTransform.childCount > 0)
            foreach (RectTransform child in rectTransform)
            {
                MaskableGraphic uiImage = child.GetComponent<Image>();
                if (uiImage != null)
                {
                    var c = uiImage.color;
                    c.a = val;
                    uiImage.color = c;
                }
                else
                {
                    uiImage = child.GetComponent<RawImage>();
                    if (uiImage != null)
                    {
                        var c = uiImage.color;
                        c.a = val;
                        uiImage.color = c;
                    }
                }

                alphaRecursive(child, val, recursiveLevel + 1);
            }
    }

    private static void alphaRecursiveSprite(Transform transform, float val)
    {
        if (transform.childCount > 0)
            foreach (Transform child in transform)
            {
                var ren = child.GetComponent<SpriteRenderer>();
                if (ren != null)
                    ren.color = new Color(ren.color.r, ren.color.g, ren.color.b, val);
                alphaRecursiveSprite(child, val);
            }
    }

    private static void colorRecursiveSprite(Transform transform, Color toColor)
    {
        if (transform.childCount > 0)
            foreach (Transform child in transform)
            {
                var ren = transform.gameObject.GetComponent<SpriteRenderer>();
                if (ren != null)
                    ren.color = toColor;
                colorRecursiveSprite(child, toColor);
            }
    }

    private static void colorRecursive(RectTransform rectTransform, Color toColor)
    {
        if (rectTransform.childCount > 0)
            foreach (RectTransform child in rectTransform)
            {
                MaskableGraphic uiImage = child.GetComponent<Image>();
                if (uiImage != null)
                {
                    uiImage.color = toColor;
                }
                else
                {
                    uiImage = child.GetComponent<RawImage>();
                    if (uiImage != null)
                        uiImage.color = toColor;
                }

                colorRecursive(child, toColor);
            }
    }

    private static void textAlphaChildrenRecursive(Transform trans, float val, bool useRecursion = true)
    {
        if (useRecursion && trans.childCount > 0)
            foreach (Transform child in trans)
            {
                var uiText = child.GetComponent<Text>();
                if (uiText != null)
                {
                    var c = uiText.color;
                    c.a = val;
                    uiText.color = c;
                }

                textAlphaChildrenRecursive(child, val);
            }
    }

    private static void textAlphaRecursive(Transform trans, float val, bool useRecursion = true)
    {
        var uiText = trans.GetComponent<Text>();
        if (uiText != null)
        {
            var c = uiText.color;
            c.a = val;
            uiText.color = c;
        }

        if (useRecursion && trans.childCount > 0)
            foreach (Transform child in trans)
                textAlphaRecursive(child, val);
    }

    private static void textColorRecursive(Transform trans, Color toColor)
    {
        if (trans.childCount > 0)
            foreach (Transform child in trans)
            {
                var uiText = child.GetComponent<Text>();
                if (uiText != null) uiText.color = toColor;
                textColorRecursive(child, toColor);
            }
    }
#endif

#if !UNITY_FLASH

    public LTDescr setOnUpdate(Action<Color> onUpdate)
    {
        _optional.onUpdateColor = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    public LTDescr setOnUpdate(Action<Color, object> onUpdate)
    {
        _optional.onUpdateColorObject = onUpdate;
        hasUpdateCallback = true;
        return this;
    }

    /**
     * Have a method called on each frame that the tween is being animated (passes a float value and a object)
     * @method setOnUpdate (object)
     * @param {Action
     * <float, object>
     *     } onUpdate:Action
     *     <float, object>
     *         a method that will be called on every frame with the float value of the tweened object, and an object of the
     *         person's choosing
     *         @return {LTDescr} LTDescr an object that distinguishes the tween
     *         @example
     *         LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved ).setOnUpdateParam( myObject );
     *         <br>
     *             <br>
     *                 void tweenMoved( float val, object obj ){ }<br>
     */
    public LTDescr setOnUpdate(Action<float, object> onUpdate, object onUpdateParam = null)
    {
        _optional.onUpdateFloatObject = onUpdate;
        hasUpdateCallback = true;
        if (onUpdateParam != null)
            _optional.onUpdateParam = onUpdateParam;
        return this;
    }

    public LTDescr setOnUpdate(Action<Vector3, object> onUpdate, object onUpdateParam = null)
    {
        _optional.onUpdateVector3Object = onUpdate;
        hasUpdateCallback = true;
        if (onUpdateParam != null)
            _optional.onUpdateParam = onUpdateParam;
        return this;
    }

    public LTDescr setOnUpdate(Action<Vector2> onUpdate, object onUpdateParam = null)
    {
        _optional.onUpdateVector2 = onUpdate;
        hasUpdateCallback = true;
        if (onUpdateParam != null)
            _optional.onUpdateParam = onUpdateParam;
        return this;
    }

    /**
     * Have a method called on each frame that the tween is being animated (passes a float value)
     * @method setOnUpdate (Vector3)
     * @param {Action
     * <Vector3>
     *     } onUpdate:Action
     *     <Vector3>
     *         a method that will be called on every frame with the float value of the tweened object
     *         @return {LTDescr} LTDescr an object that distinguishes the tween
     *         @example
     *         LeanTween.moveX(gameObject, 5f, 2.0f ).setOnUpdate( tweenMoved );
     *         <br>
     *             <br>
     *                 void tweenMoved( Vector3 val ){ }<br>
     */
    public LTDescr setOnUpdate(Action<Vector3> onUpdate, object onUpdateParam = null)
    {
        _optional.onUpdateVector3 = onUpdate;
        hasUpdateCallback = true;
        if (onUpdateParam != null)
            _optional.onUpdateParam = onUpdateParam;
        return this;
    }
#endif

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
    public LTDescr setRect(RectTransform rect)
    {
        rectTransform = rect;
        return this;
    }

    public LTDescr setSprites(Sprite[] sprites)
    {
        this.sprites = sprites;
        return this;
    }

    public LTDescr setFrameRate(float frameRate)
    {
        time = sprites.Length / frameRate;
        return this;
    }
#endif
}

//}