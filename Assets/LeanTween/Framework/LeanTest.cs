using System.Collections;
using UnityEngine;

public class LeanTester : MonoBehaviour
{
    public float timeout = 15f;

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
    public void Start()
    {
        StartCoroutine(timeoutCheck());
    }

    private IEnumerator timeoutCheck()
    {
        var pauseEndTime = Time.realtimeSinceStartup + timeout;
        while (Time.realtimeSinceStartup < pauseEndTime) yield return 0;
        if (LeanTest.testsFinished == false)
        {
            Debug.Log(LeanTest.formatB("Tests timed out!"));
            LeanTest.overview();
        }
    }
#endif
}

public class LeanTest : object
{
    public static int expected = 0;
    private static int tests;
    private static int passes;

    public static float timeout = 15f;
    public static bool timeoutStarted;
    public static bool testsFinished;

    public static void debug(string name, bool didPass, string failExplaination = null)
    {
        expect(didPass, name, failExplaination);
    }

    public static void expect(bool didPass, string definition, string failExplaination = null)
    {
        var len = printOutLength(definition);
        var paddingLen = 40 - (int) (len * 1.05f);
#if UNITY_FLASH
		string padding = padRight(paddingLen);
#else
        var padding = "".PadRight(paddingLen, "_"[0]);
#endif
        var logName = formatB(definition) + " " + padding + " [ " +
                      (didPass ? formatC("pass", "green") : formatC("fail", "red")) + " ]";
        if (didPass == false && failExplaination != null)
            logName += " - " + failExplaination;
        Debug.Log(logName);
        if (didPass)
            passes++;
        tests++;

        // Debug.Log("tests:"+tests+" expected:"+expected);
        if (tests == expected && testsFinished == false)
            overview();
        else if (tests > expected)
            Debug.Log(formatB("Too many tests for a final report!") + " set LeanTest.expected = " + tests);

        if (timeoutStarted == false)
        {
            timeoutStarted = true;
            var tester = new GameObject();
            tester.name = "~LeanTest";
            var test = tester.AddComponent(typeof(LeanTester)) as LeanTester;
            test.timeout = timeout;
#if !UNITY_EDITOR
			tester.hideFlags = HideFlags.HideAndDontSave;
#endif
        }
    }

    public static string padRight(int len)
    {
        var str = "";
        for (var i = 0; i < len; i++) str += "_";
        return str;
    }

    public static float printOutLength(string str)
    {
        var len = 0.0f;
        for (var i = 0; i < str.Length; i++)
            if (str[i] == "I"[0])
                len += 0.5f;
            else if (str[i] == "J"[0])
                len += 0.85f;
            else
                len += 1.0f;
        return len;
    }

    public static string formatBC(string str, string color)
    {
        return formatC(formatB(str), color);
    }

    public static string formatB(string str)
    {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		return str;
#else
        return "<b>" + str + "</b>";
#endif
    }

    public static string formatC(string str, string color)
    {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		return str;
#else
        return "<color=" + color + ">" + str + "</color>";
#endif
    }

    public static void overview()
    {
        testsFinished = true;
        var failedCnt = expected - passes;
        var failedStr = failedCnt > 0 ? formatBC("" + failedCnt, "red") : "" + failedCnt;
        Debug.Log(formatB("Final Report:") + " _____________________ PASSED: " + formatBC("" + passes, "green") +
                  " FAILED: " + failedStr + " ");
    }
}