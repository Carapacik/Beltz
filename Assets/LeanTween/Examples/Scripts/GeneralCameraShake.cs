#if !UNITY_FLASH
using UnityEngine;

public class GeneralCameraShake : MonoBehaviour
{
    private GameObject avatarBig;
    private AudioClip boomAudioClip;
    private float jumpIter = 9.5f;

    // Use this for initialization
    private void Start()
    {
        avatarBig = GameObject.Find("AvatarBig");

        var volumeCurve = new AnimationCurve(new Keyframe(8.130963E-06f, 0.06526042f, 0f, -1f),
            new Keyframe(0.0007692695f, 2.449077f, 9.078861f, 9.078861f),
            new Keyframe(0.01541314f, 0.9343268f, -40f, -40f),
            new Keyframe(0.05169491f, 0.03835937f, -0.08621139f, -0.08621139f));
        var frequencyCurve = new AnimationCurve(new Keyframe(0f, 0.003005181f, 0f, 0f),
            new Keyframe(0.01507768f, 0.002227979f, 0f, 0f));
        boomAudioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve,
            LeanAudio.options().setVibrato(new[] {new Vector3(0.1f, 0f, 0f)}));


        bigGuyJump();
    }

    private void bigGuyJump()
    {
        var height = Mathf.PerlinNoise(jumpIter, 0f) * 10f;
        height = height * height * 0.3f;
        // Debug.Log("height:"+height+" jumpIter:"+jumpIter);

        LeanTween.moveY(avatarBig, height, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            LeanTween.moveY(avatarBig, 0f, 0.27f).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
            {
                LeanTween.cancel(gameObject);

                /**************
                * Camera Shake
                **************/

                var shakeAmt = height * 0.2f; // the degrees to shake the camera
                var shakePeriodTime = 0.42f; // The period of each shake
                var dropOffTime = 1.6f; // How long it takes the shaking to settle down to nothing
                var shakeTween = LeanTween.rotateAroundLocal(gameObject, Vector3.right, shakeAmt, shakePeriodTime)
                    .setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
                    .setLoopClamp()
                    .setRepeat(-1);

                // Slow the camera shake down to zero
                LeanTween.value(gameObject, shakeAmt, 0f, dropOffTime).setOnUpdate(
                    val => { shakeTween.setTo(Vector3.right * val); }
                ).setEase(LeanTweenType.easeOutQuad);


                /********************
                * Shake scene objects
                ********************/

                // Make the boxes jump from the big stomping
                var boxes = GameObject
                    .FindGameObjectsWithTag(
                        "Respawn"); // I just arbitrarily tagged the boxes with this since it was available in the scene
                foreach (var box in boxes) box.GetComponent<Rigidbody>().AddForce(Vector3.up * 100 * height);

                // Make the lamps spin from the big stomping
                var lamps = GameObject
                    .FindGameObjectsWithTag(
                        "GameController"); // I just arbitrarily tagged the lamps with this since it was available in the scene
                foreach (var lamp in lamps)
                {
                    var z = lamp.transform.eulerAngles.z;
                    z = z > 0.0f && z < 180f
                        ? 1
                        : -1; // push the lamps in whatever direction they are currently swinging
                    lamp.GetComponent<Rigidbody>().AddForce(new Vector3(z, 0f, 0f) * 15 * height);
                }

                // Play BOOM!
                LeanAudio.play(boomAudioClip, transform.position,
                    height * 0.2f); // Like this sound? : http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,0,0.003005181,0,0,0.01507768,0.002227979,0,0,8~8,8.130963E-06,0.06526042,0,-1,0.0007692695,2.449077,9.078861,9.078861,0.01541314,0.9343268,-40,-40,0.05169491,0.03835937,-0.08621139,-0.08621139,8~0.1,0,0,~44100

                // Have the jump happen again 2 seconds from now
                LeanTween.delayedCall(2f, bigGuyJump);
            });
        });
        jumpIter += 5.2f;
    }
}
#endif