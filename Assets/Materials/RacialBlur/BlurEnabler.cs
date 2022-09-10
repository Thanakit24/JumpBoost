using System.Linq;
using UnityEngine;


public class BlurEnabler : MonoBehaviour
{
    public Material blurMat;
    //public float transitionLength = 0.5f;
    //public float maxBlur = 0.15f;
    //public float velScaler = 0.015f;
    public float minVel, maxVel, maxBlur;
    public Rigidbody rb;
    

    void Update()
    {
        var clampedSpeed = Mathf.Clamp(Mathf.Abs(rb.velocity.y), minVel, maxVel);
        var blurVal = KongrooUtils.RemapRange(clampedSpeed , minVel, maxVel, 0, maxBlur);

        blurMat.SetFloat("blurWidth",  blurVal);
    }

    private void OnDestroy()
    {
        blurMat.SetFloat("blurWidth", 0f);
    }
}