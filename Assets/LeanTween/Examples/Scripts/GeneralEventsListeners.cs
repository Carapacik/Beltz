#if !UNITY_FLASH
using UnityEngine;

public class GeneralEventsListeners : MonoBehaviour
{
    // It's best to make this a public enum that you use throughout your project, so every class can have access to it
    public enum MyEvents
    {
        CHANGE_COLOR,
        JUMP,
        LENGTH
    }

    private Color fromColor;

    private Vector3 towardsRotation;
    private float turnForIter;
    private float turnForLength = 0.5f;

    private void Awake()
    {
        LeanTween.LISTENERS_MAX = 100; // This is the maximum of event listeners you will have added as listeners
        LeanTween.EVENTS_MAX = (int) MyEvents.LENGTH; // The maximum amount of events you will be dispatching

        fromColor = GetComponent<Renderer>().material.color;
    }

    private void Start()
    {
        // Adding Listeners, it's best to use an enum so your listeners are more descriptive but you could use an int like 0,1,2,...
        LeanTween.addListener(gameObject, (int) MyEvents.CHANGE_COLOR, changeColor);
        LeanTween.addListener(gameObject, (int) MyEvents.JUMP, jumpUp);
    }

    private void FixedUpdate()
    {
        if (turnForIter < turnForLength)
        {
            GetComponent<Rigidbody>().MoveRotation(GetComponent<Rigidbody>().rotation *
                                                   Quaternion.Euler(towardsRotation * Time.deltaTime));
            turnForIter += Time.deltaTime;
        }

        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 4.5f);
    }

    // ****** Physics / AI Stuff

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 2)
            towardsRotation = new Vector3(0f, Random.Range(-180, 180), 0f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != 2)
        {
            turnForIter = 0f;
            turnForLength = Random.Range(0.5f, 1.5f);
        }
    }

    // ****** Key and clicking detection

    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.J)) // Are you also pressing the "j" key while clicking
            LeanTween.dispatchEvent((int) MyEvents.JUMP);
        else
            LeanTween.dispatchEvent((int) MyEvents.CHANGE_COLOR,
                transform); // with every dispatched event, you can include an object (retrieve this object with the *.data var in LTEvent)
    }

    // ****** Event Listening Methods

    private void jumpUp(LTEvent e)
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 300f);
    }

    private void changeColor(LTEvent e)
    {
        var tran = (Transform) e.data;
        var distance = Vector3.Distance(tran.position, transform.position);
        var to = new Color(Random.Range(0f, 1f), 0f, Random.Range(0f, 1f));
        LeanTween.value(gameObject, fromColor, to, 0.8f).setLoopPingPong(1).setDelay(distance * 0.05f).setOnUpdate(
            col => { GetComponent<Renderer>().material.color = col; }
        );
    }
}
#endif