using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager instance;
    private MotionBlur motBlur;
    public Volume volume; 
    

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnMotionBlur()
    {
        if (volume.profile.TryGet<MotionBlur>(out motBlur))
        {
            motBlur.active = true;
        }
    }

    public void OffMotionBlur()
    {
        if (volume.profile.TryGet<MotionBlur>(out motBlur))
        {
            motBlur.active = false;
        }
    }
// Update is called once per frame
void Update()
    {
        
    }
}
