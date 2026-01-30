using System;
using UnityEngine;

[Serializable]
public class ShaderPropertyAnimation
{
    public Material material;
    public Material secondaryMaterial;

    //TODO: ya que siempre se usa la misma propiedad con esto, convertirlo en un id al principio y usar unicamente eso
    public string property = "value";

    public float offValue = 0;
    public float onValue = 1;

    public CoroutineAnimation animationData;

    public void TurnOn(MonoBehaviour monoBehaviour, Material material = null)
    {
        if (!material) material = this.material;

        float initialValue = material.GetFloat(property);

        void OnUpdate(float i)
        {
            material.SetFloat(property, Mathf.Lerp(initialValue, onValue, i));
            if(secondaryMaterial)secondaryMaterial.SetFloat(property, Mathf.Lerp(initialValue, onValue, i));
        }

        animationData.Play(monoBehaviour, onUpdate: OnUpdate);
    }

    public void TurnOff(MonoBehaviour monoBehaviour, Material material = null)
    {
        if (!material) material = this.material;

        float initialValue = material.GetFloat(property);

        void OnUpdate(float i)
        {
            material.SetFloat(property, Mathf.Lerp(initialValue, offValue, i));
            if (secondaryMaterial) secondaryMaterial.SetFloat(property, Mathf.Lerp(initialValue, offValue, i));
        }

        animationData.Play(monoBehaviour, onUpdate: OnUpdate);
    }


    public void TurnOnLocal(MonoBehaviour monoBehaviour, Renderer renderer)
    {
        MaterialPropertyBlock propertyBlock = new ();
        renderer.GetPropertyBlock(propertyBlock);

        float initialValue = propertyBlock.GetFloat(property);

        void OnUpdate(float i)
        {
            propertyBlock.SetFloat(property, Mathf.Lerp(initialValue, onValue, i));
            renderer.SetPropertyBlock(propertyBlock);
        }

        animationData.Play(monoBehaviour, onUpdate: OnUpdate);
    }

    public void TurnOffLocal(MonoBehaviour monoBehaviour, Renderer renderer)
    {
        MaterialPropertyBlock propertyBlock = new();
        renderer.GetPropertyBlock(propertyBlock);

        float initialValue = propertyBlock.GetFloat(property);

        void OnUpdate(float i)
        {
            material.SetFloat(property, Mathf.Lerp(initialValue, offValue, i));
            renderer.SetPropertyBlock(propertyBlock);
        }

        animationData.Play(monoBehaviour, onUpdate: OnUpdate);
    }

    public void Reset()
    {
        material.SetFloat(property, offValue);
        if (secondaryMaterial) secondaryMaterial.SetFloat(property, offValue);
    }

}
