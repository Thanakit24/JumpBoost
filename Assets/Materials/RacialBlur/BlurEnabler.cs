using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class BlurEnabler : MonoBehaviour
{
    public Renderer2DData rendererData;
    public string featureName = "Feature";
    public float transitionLength = 0.5f;
    public float maxDelta = 0.15f;

    private PostProcessingRenderFeature renderFeature;
    private float startTime;

    private void TryGetFeature()
    {
        renderFeature = (PostProcessingRenderFeature) rendererData.rendererFeatures
            .FirstOrDefault(f => f.name == featureName);
        if (renderFeature == null) print($"Didnt find feature with name {featureName}");
    }

    void Start()
    {
        TryGetFeature();
    }

    public void ActivateFeature()
    {
        renderFeature.SetActive(true);
        startTime = Time.time;
        renderFeature.settings.material.SetFloat("blurWidth",0f);
    }

    void Update()
    {
        if (Time.time - startTime < transitionLength)
        {
            float remapped = (Time.time - startTime) / transitionLength;
            renderFeature.settings.material.SetFloat("blurWidth", remapped * maxDelta);
        }
        else
        {
            renderFeature.SetActive(false);
        }
    }
}