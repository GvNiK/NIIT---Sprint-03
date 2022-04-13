using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAlphaTween : Tween
{
    private float targetAlpha;
    private float initialAlpha;
    private float targetTime;
    private float stateTime;
    private MeshRenderer meshRenderer;
    private float targetDifference;
    private OccludingEnvironment occludingEnvironment;

    public MaterialAlphaTween(MeshRenderer meshRenderer, float targetAlpha, float initialAlpha, float time)
    {
        this.meshRenderer = meshRenderer;
        this.targetAlpha = targetAlpha;
        this.initialAlpha = initialAlpha;
        this.targetTime = time;
        targetDifference = targetAlpha - initialAlpha;
        occludingEnvironment = meshRenderer.gameObject.GetComponent<OccludingEnvironment>();

        Start();
    }

    public override void Start()
    {
        Color meshColor = meshRenderer.material.GetColor("_Color");

        if(targetAlpha <= 1f)
        {
            SetFadeMaterial();
        }

        if (targetTime <= 0f || IsAlphaEqual(meshColor.a))
        {
            CompleteTween();
        }
    }

    public override void Update()
    {
        if(stateTime < targetTime)
        {
            stateTime += Time.deltaTime;
            float percentComplete = stateTime / targetTime;
            Color meshColor = meshRenderer.material.GetColor("_Color");
            meshColor.a = initialAlpha + (targetDifference * percentComplete);
            meshRenderer.material.SetColor("_Color", meshColor);
        }
        else
        {
            CompleteTween();
        }
    }

    private void CompleteTween()
    {
        if(targetAlpha >= 1f)
        {
            SetOriginalMaterial();
        }

        OnComplete();
    }

    private bool IsAlphaEqual(float currentAlpha)
    {
        return currentAlpha == targetAlpha;
    }

    private void SetOriginalMaterial()
    {
        if (occludingEnvironment != null)
        {
            meshRenderer.material = occludingEnvironment.originalMaterial;
        }
    }

    private void SetFadeMaterial()
    {
        meshRenderer.material = occludingEnvironment.fadeMaterial;
    }
}
