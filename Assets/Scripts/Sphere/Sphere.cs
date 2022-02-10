//--------------------------------------------------------------------------------------
// Purpose: The main logic of an individual sphere object.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// Sphere object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Sphere : MonoBehaviour
{
    // MATERIAL SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Material Settings:")]

    // public Material array for the materials needed to set the Opaque and Fade.
    public Material[] m_amatMaterials;
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private gameobject for the opacity slider
    private GameObject m_gOpacitySlider;

    // private Renderer for the spheres main renderer
    private Renderer m_rendSphere;

    // private bool for the ready status of the sphere
    private bool m_bReady = false;

    // private bool for the Opacity Zero status
    private bool m_bOpacityZero = false;
    //--------------------------------------------------------------------------------------

    // GETTERS / SETTERS //
    //--------------------------------------------------------------------------------------
    // Setter of type bool, for setting the ready status of the sphere
    public void SetReady(bool bStatus) { m_bReady = bStatus; }

    // Setter of type bool, for setting the Opacity Zero status.
    public void SetOpacityZeroBool(bool bStatus) { m_bOpacityZero = bStatus; }

    // public Setter of type float for setting the opacity slider of this sphere
    public void SetOpacitySlider(float fValue) { m_gOpacitySlider.GetComponentInChildren<Slider>().value = fValue; }

    // private Setter for setting the Opaque Material as the current material
    private void SetOpaqueMaterial() { m_rendSphere.sharedMaterial = m_amatMaterials[0]; }

    // private Setter for setting the Fade material as the current material
    private void SetFadeMaterial() { m_rendSphere.sharedMaterial = m_amatMaterials[1]; }

    // public Getter of type GameObject for getting the slider object of this sphere
    public GameObject GetOpacitySlider() { return m_gOpacitySlider; }
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // Initalize the sphere renderer with passed in materials
        m_rendSphere = GetComponent<Renderer>();
        m_rendSphere.enabled = true;
        m_rendSphere.sharedMaterial = m_amatMaterials[0];
    }

    //--------------------------------------------------------------------------------------
    // OnEnable: Function that will call when this gameObject is enabled.
    //--------------------------------------------------------------------------------------
    private void OnEnable()
    {
        // If the sphere is ready for use
        if (m_bReady)
        {
            // Set the opacity slider to active
            m_gOpacitySlider.SetActive(true);

            // Add UpdateOpacity method as a listener for the onValueChanged event for the slider
            m_gOpacitySlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(delegate { UpdateOpacity(); });

            // Add LerpOpacity method as a listener for the onClick event of the slider objects button/ text object
            m_gOpacitySlider.GetComponentInChildren<Button>().onClick.AddListener(delegate { InitiateLerpOpacity(); });

            // Check if the Opacity Zero bool needs setting
            if (m_gOpacitySlider.GetComponentInChildren<Slider>().value == 0)
            {
                // Set the opacity zero bool
                m_bOpacityZero = true;

                // make sure this object stays disabled
                gameObject.SetActive(false);
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // OnDisable: Function that will call when this gameObject is disabled.
    //--------------------------------------------------------------------------------------
    private void OnDisable()
    {
        // Check if the Opacity is zero,
        // Only want to disable slider if it
        // is a grid reset
        if (!m_bOpacityZero)
        {
            // Set the opacity slider to inactive
            if (m_gOpacitySlider != null)
                m_gOpacitySlider.SetActive(false);
        }
    }

    //--------------------------------------------------------------------------------------
    // UpdateOpacity: Listner method for the Sliders onValueChanged event.
    //--------------------------------------------------------------------------------------
    private void UpdateOpacity()
    {
        // if the slider value is exactly one change the material to Opaque
        if (m_gOpacitySlider.GetComponentInChildren<Slider>().value == 1)
            SetOpaqueMaterial();

        // else if the slider value is set exactly to zero
        else if (m_gOpacitySlider.GetComponentInChildren<Slider>().value == 0)
        {
            // Set the opacity zero bool and disable sphere object
            // it is not needed while it cant be seen
            m_bOpacityZero = true;
            gameObject.SetActive(false);
        }

        // otherwise keep the material on the fade material.
        else
        {
            // make sure the sphere object is active and set material
            gameObject.SetActive(true);
            SetFadeMaterial();
        }

        // Make sure we are only changing the opacity of the fade material
        if (m_rendSphere.material != m_amatMaterials[0])
        {
            // Get the material color, change the alpha value by slider value, set back to the material
            Color color = m_rendSphere.material.color;
            color.a = m_gOpacitySlider.GetComponentInChildren<Slider>().value;
            m_rendSphere.material.color = color;
        }
    }

    //--------------------------------------------------------------------------------------
    // InitiateLerpOpacity: Listner method for the Sliders Text/Button onClick event.
    // Used to initate the lerp function for the opacity.
    //--------------------------------------------------------------------------------------
    public void InitiateLerpOpacity()
    {
        // Check if the slider value is less than 0.5, if it is we lerp to 1
        if (m_gOpacitySlider.GetComponentInChildren<Slider>().value < 0.5)
        {
            // Ensure that the object isn't inactive
            if (!gameObject.activeSelf)
            {
                // Set the slider slightly so gameobject doesn't flip back to inactive
                // from the onEnable method. And set the sphere to active.
                m_gOpacitySlider.GetComponentInChildren<Slider>().value = 0.01f;
                gameObject.SetActive(true);
            }

            // Start a coroutine for the lerp fucntion
            StartCoroutine(LerpOpacity(1, 1));
        }
        
        // Else if the slider value is greater than 0.5 then we lerp to 0
        else if (m_gOpacitySlider.GetComponentInChildren<Slider>().value > 0.5)
        {
            // Start a coroutine for the lerp fucntion
            StartCoroutine(LerpOpacity(0, 1));
        }
    }

    //--------------------------------------------------------------------------------------
    // LerpOpacity: Coroutine for lerping the opacity. 
    //--------------------------------------------------------------------------------------
    IEnumerator LerpOpacity(float fEnd, float fDuration)
    {
        // Reset timer and start value
        float fTime = 0;
        float fStart = m_gOpacitySlider.GetComponentInChildren<Slider>().value;

        // While the time is less than duration
        while (fTime < fDuration)
        {
            // Lerp the slider
            m_gOpacitySlider.GetComponentInChildren<Slider>().value = Mathf.Lerp(fStart, fEnd, fTime / fDuration);
            
            // update the timer by deltatime
            fTime += Time.deltaTime;
            
            // wait until next frame and contiune
            yield return null;
        }

        // Ensure slider is set to the exact end of ther lerp
        m_gOpacitySlider.GetComponentInChildren<Slider>().value = fEnd;
    }

    //--------------------------------------------------------------------------------------
    // AssignSlider: Set the slider object for this specific sphere object.
    //
    // Params:
    //      gSlider: The gameobject to set the OpacitySlider object.
    //--------------------------------------------------------------------------------------
    public void AssignSlider(GameObject gSlider)
    {
        // Set the slider for this sphere
        m_gOpacitySlider = gSlider;
    }
}