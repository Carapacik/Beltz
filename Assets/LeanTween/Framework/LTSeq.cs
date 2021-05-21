using System;
using UnityEngine;

/**
 * Internal Representation of a Sequence
 * <br>
 *     <br>
 *         &nbsp;&nbsp;<h4>Example:</h4>
 *         var seq = LeanTween.sequence();
 *         <br>
 *             seq.append(1f); <span style="color:gray">// delay everything one second</span>
 *             <br>
 *                 seq.append( () => { <span style="color:gray">// fire an event before start</span>
 *                 <br>
 *                     &nbsp;Debug.Log("I have started");
 *                     <br>
 *                         });
 *                         <br>
 *                             seq.append( LeanTween.move(cube1, Vector3.one * 10f, 1f) );
 *                             <span style="color:gray">// do a tween</span>
 *                             <br>
 *                                 seq.append( (object obj) => { <span style="color:gray">// fire event after tween</span>
 *                                 <br>
 *                                     &nbsp;var dict = obj as Dictionary
 *                                     <string, string>
 *                                         ;
 *                                         <br>
 *                                             &nbsp;Debug.Log("We are done now obj value:"+dict["hi"]);
 *                                             <br>
 *                                                 }, new Dictionary
 *                                                 <string, string>
 *                                                     (){ {"hi","sup"} } );
 *                                                     <br>
 *                                                         @class LTSeq
 *                                                         @constructor
 */
public class LTSeq
{
    private uint _id;

    public uint counter;

    public LTSeq current;

    private int debugIter;

    public LTSeq previous;

    public float timeScale;

    public bool toggle;

    public float totalDelay;

    public LTDescr tween;

    public int id
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

    public void reset()
    {
        previous = null;
        tween = null;
        totalDelay = 0f;
    }

    public void init(uint id, uint global_counter)
    {
        reset();
        _id = id;

        counter = global_counter;

        current = this;
    }

    private LTSeq addOn()
    {
        current.toggle = true;
        var lastCurrent = current;
        current = LeanTween.sequence();
        // Debug.Log("this.current:" + this.current.id + " lastCurrent:" + lastCurrent.id);
        current.previous = lastCurrent;
        lastCurrent.toggle = false;
        current.totalDelay = lastCurrent.totalDelay;
        current.debugIter = lastCurrent.debugIter + 1;
        return current;
    }

    private float addPreviousDelays()
    {
//		Debug.Log("delay:"+delay+" count:"+this.current.count+" this.current.totalDelay:"+this.current.totalDelay);

        var prev = current.previous;

        if (prev != null && prev.tween != null) return current.totalDelay + prev.tween.time;
        return current.totalDelay;
    }

    /**
     * Add a time delay to the sequence
     * @method append (delay)
     * @param {float} delay:float amount of time to add to the sequence
     * @return {LTSeq} LTDescr an object that distinguishes the tween
     * var seq = LeanTween.sequence();
     * <br>
     *     seq.append(1f); // delay everything one second
     *     <br>
     *         seq.append( LeanTween.move(cube1, Vector3.one * 10f, 1f) ); // do a tween<br>
     */
    public LTSeq append(float delay)
    {
        current.totalDelay += delay;

        return current;
    }

    /**
     * Add a time delay to the sequence
     * @method append (method)
     * @param {System.Action} callback:System.Action method you want to be called
     * @return {LTSeq} LTSeq an object that you can add tweens, methods and time on to
     * @example
     * var seq = LeanTween.sequence();
     * <br>
     *     seq.append( () => { // fire an event before start
     *     <br>
     *         &nbsp;Debug.Log("I have started");
     *         <br>
     *             });
     *             <br>
     *                 seq.append( LeanTween.move(cube1, Vector3.one * 10f, 1f) ); // do a tween
     *                 <br>
     *                     seq.append( () => { // fire event after tween
     *                     <br>
     *                         &nbsp;Debug.Log("We are done now");
     *                         <br>
     *                             });;<br>
     */
    public LTSeq append(Action callback)
    {
        var newTween = LeanTween.delayedCall(0f, callback);
//		Debug.Log("newTween:" + newTween);
        return append(newTween);
    }

    /**
     * Add a time delay to the sequence
     * @method add (method(object))
     * @param {System.Action} callback:System.Action method you want to be called
     * @return {LTSeq} LTSeq an object that you can add tweens, methods and time on to
     * @example
     * var seq = LeanTween.sequence();
     * <br>
     *     seq.append( () => { // fire an event before start
     *     <br>
     *         &nbsp;Debug.Log("I have started");
     *         <br>
     *             });
     *             <br>
     *                 seq.append( LeanTween.move(cube1, Vector3.one * 10f, 1f) ); // do a tween
     *                 <br>
     *                     seq.append((object obj) => { // fire event after tween
     *                     &nbsp;var dict = obj as Dictionary
     *                     <string, string>
     *                         ;
     *                         &nbsp;Debug.Log("We are done now obj value:"+dict["hi"]);
     *                         &nbsp;}, new Dictionary<string, string>(){ {"hi","sup"} } );
     */
    public LTSeq append(Action<object> callback, object obj)
    {
        append(LeanTween.delayedCall(0f, callback).setOnCompleteParam(obj));

        return addOn();
    }

    public LTSeq append(GameObject gameObject, Action callback)
    {
        append(LeanTween.delayedCall(gameObject, 0f, callback));

        return addOn();
    }

    public LTSeq append(GameObject gameObject, Action<object> callback, object obj)
    {
        append(LeanTween.delayedCall(gameObject, 0f, callback).setOnCompleteParam(obj));

        return addOn();
    }

    /**
     * Retrieve a sequencer object where you can easily chain together tweens and methods one after another
     * 
     * @method add (tween)
     * @return {LTSeq} LTSeq an object that you can add tweens, methods and time on to
     * @example
     * var seq = LeanTween.sequence();
     * <br>
     *     seq.append( LeanTween.move(cube1, Vector3.one * 10f, 1f) ); // do a move tween
     *     <br>
     *         seq.append( LeanTween.rotateAround( avatar1, Vector3.forward, 360f, 1f ) ); // then do a rotate tween<br>
     */
    public LTSeq append(LTDescr tween)
    {
        current.tween = tween;

//		Debug.Log("tween:" + tween + " delay:" + this.current.totalDelay);

        current.totalDelay = addPreviousDelays();

        tween.setDelay(current.totalDelay);

        return addOn();
    }

    public LTSeq insert(LTDescr tween)
    {
        current.tween = tween;

        tween.setDelay(addPreviousDelays());

        return addOn();
    }


    public LTSeq setScale(float timeScale)
    {
//		Debug.Log("this.current:" + this.current.previous.debugIter+" tween:"+this.current.previous.tween);
        setScaleRecursive(current, timeScale, 500);

        return addOn();
    }

    private void setScaleRecursive(LTSeq seq, float timeScale, int count)
    {
        if (count > 0)
        {
            this.timeScale = timeScale;

//			Debug.Log("seq.count:" + count + " seq.tween:" + seq.tween);
            seq.totalDelay *= timeScale;
            if (seq.tween != null)
            {
//			Debug.Log("seq.tween.time * timeScale:" + seq.tween.time * timeScale + " seq.totalDelay:"+seq.totalDelay +" time:"+seq.tween.time+" seq.tween.delay:"+seq.tween.delay);
                if (seq.tween.time != 0f)
                    seq.tween.setTime(seq.tween.time * timeScale);
                seq.tween.setDelay(seq.tween.delay * timeScale);
            }

            if (seq.previous != null)
                setScaleRecursive(seq.previous, timeScale, count - 1);
        }
    }

    public LTSeq reverse()
    {
        return addOn();
    }
}