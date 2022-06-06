using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class AuraControl : MonoBehaviour
{

    public Material[] _auraMat;
    public Color _AuraColor = Color.green;
    public Color _RimColor = Color.green;
    public Texture2D _noiseTexture;
    [Range(0.002f, 0.8f)]
    public float _auraWidth = 0.3f;

    [Range(-0.06f, 0f)]
    public float _AuraZ = -0.05f;

    [Range(0.0f, 0.2f)]
    public float _noiseScale = 0.01f;

    [Range(-10, 10)]
    public float _speedX = 3f;

    [Range(-10, 10)]
    public float _speedY = 3f;

    [Range(0.01f, 10.0f)]
    public float _noiseOpacity = 10f;

    [Range(0.5f, 3.0f)]
    public float _brightness = 2f;

    [Range(0f, 1f)]
    public float _rimEdge = 0.1f;

    [Range(0.01f, 10f)]
    public float _rimPower = 1f;

    public bool _editMode;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_editMode)
        {
            UpdateAura();
        }
    }

	public void UpdateAura(){

		foreach (Material auras in _auraMat)
        {
            auras.SetColor("_Color2", _AuraColor);
            auras.SetColor("_ColorR", _RimColor);
			auras.SetFloat("_Outline", _auraWidth);
            auras.SetFloat("_OutlineZ", _AuraZ);
			auras.SetFloat("_Scale", _noiseScale);
			auras.SetFloat("_SpeedX", _speedX);
			auras.SetFloat("_SpeedY", _speedY);
			auras.SetTexture("_NoiseTex", _noiseTexture);
			auras.SetFloat("_Opacity", _noiseOpacity);
			auras.SetFloat("_Brightness", _brightness);
			auras.SetFloat("_Edge", _rimEdge);
			auras.SetFloat("_RimPower", _rimPower);
        }

	}

    public void RevertAura(){
        foreach (Material auras in _auraMat)
        {
            auras.SetFloat("_Outline", 0f);
        }
    }
}
