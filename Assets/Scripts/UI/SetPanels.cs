//--------------------------------------------------------------------------------------
// Purpose: Setup and ensure the Slider and Sphere Panel sizes.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// SetPanels object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class SetPanels : MonoBehaviour
{
    // PUBLIC VALUES //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Panel Setting:")]

    // Public gameobject for the slider panel
    public GameObject m_gSliderPanel;

    // public gameobject for the sphere panel
    public GameObject m_gSpherePanel;

    // public Canvas for the main scenes UI canvas
    public Canvas m_cMainCanvas;

    // float for the percentage to set the sliders right rect value 
    public float m_fSliderWidthPercentage;

    // float for the percentage to set the sphere right ret value
    public float m_fSphereWidthPercentage;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // Set the inital size of both panels right value
        SetSliderPanel(m_cMainCanvas.GetComponent<RectTransform>().rect.width, m_fSliderWidthPercentage);
        SetSpherePanel(m_cMainCanvas.GetComponent<RectTransform>().rect.width, m_fSphereWidthPercentage);
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void Update()
    {
        // Check if the canvas size has changed
        if (m_cMainCanvas.GetComponent<RectTransform>().hasChanged)
        {
            // Update the width of the panels
            SetSliderPanel(m_cMainCanvas.GetComponent<RectTransform>().rect.width, m_fSliderWidthPercentage);
            SetSpherePanel(m_cMainCanvas.GetComponent<RectTransform>().rect.width, m_fSphereWidthPercentage);

            // Ensure the value changed bool is reset
            m_cMainCanvas.GetComponent<RectTransform>().hasChanged = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // SetSliderPanel: Set the width of the Slider Panel by a set percentage.
    //
    // Params:
    //      fCanvasWidth: The width of the sliders parent canvas.
    //      fPercentage: The percentage of the screen to alter the right rect value.
    //--------------------------------------------------------------------------------------
    private void SetSliderPanel(float fCanvasWidth, float fPercentage)
    {
        // Prepare percentage variable for usage.
        fPercentage = fPercentage / 100;

        // Set the panels right value to a percentage of the canvas width
        m_gSliderPanel.GetComponent<RectTransform>().offsetMax =
            new Vector2(-(fCanvasWidth * fPercentage), m_gSliderPanel.GetComponent<RectTransform>().offsetMax.y);
    }

    //--------------------------------------------------------------------------------------
    // SetSpherePanel: Set the width of the Sphere Panel by a set percentage.
    //
    // Params:
    //      fCanvasWidth: The width of the sphere parent canvas.
    //      fPercentage: The percentage of the screen to alter the right rect value.
    //--------------------------------------------------------------------------------------
    private void SetSpherePanel(float fCanvasWidth, float fPercentage)
    {
        // Prepare percentage variable for usage.
        fPercentage = fPercentage / 100;

        // Set the panels right value to a percentage of the canvas width
        m_gSpherePanel.GetComponent<RectTransform>().offsetMin =
            new Vector2((fCanvasWidth * fPercentage), m_gSpherePanel.GetComponent<RectTransform>().offsetMin.y);
    }
}