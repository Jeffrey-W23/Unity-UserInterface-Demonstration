//--------------------------------------------------------------------------------------
// Purpose: Manager the Sphere and Slider Object Pools.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// SphereManager object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class SphereManager : MonoBehaviour
{
    // POOL SETUP //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Sphere Pool Setup:")]

    // public int value for the size of the sphere and slider object pool.
    public int m_nPoolSize = 1000;

    // public sphere object for the pool blueprint
    public Sphere m_oSpherePrefab;

    // public Gameobject for the scroll views content section, used for setting parent of slider.
    public GameObject m_gScrollContent;

    // public Gameobject for the slider pool blueprint
    public GameObject m_gSliderPrefab;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // SPHERE SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Sphere Settings:")]

    // public gameobject for the slider for all sphere
    public GameObject m_gAllSpheresSlider;

    // public int for the spheres that are to be activated.
    [Range(0, 1000)]
    public int m_nActiveSpheres = 50;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // GRID SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Grid Settings:")]

    // public gameobject for the spheres parent used to generate grid
    public GameObject m_gSphereGirdParent;
    //--------------------------------------------------------------------------------------

    // PUBLIC HIDDEN //
    //--------------------------------------------------------------------------------------
    // new singleton for getting the sphere manager
    [HideInInspector]
    public static SphereManager m_oInstance;
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private sphere array for the sphere object pool
    private Sphere[] m_aoSphereObjectPool;

    // private gameobject array for the slider object pool
    private GameObject[] m_agSliderObjectPool;

    // private int for checking the current spheres that need to be active
    private int m_nCurrentActiveSpheres;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Awake()
    {
        // set instance
        m_oInstance = this;

        // Initialize sphere and slider object pools
        m_aoSphereObjectPool = new Sphere[m_nPoolSize];
        m_agSliderObjectPool = new GameObject[m_nPoolSize];

        // loop through the object pools
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // Instantiate Sphere and Slider object
            Sphere oSphereObject = Instantiate(m_oSpherePrefab);
            GameObject gSliderObject = Instantiate(m_gSliderPrefab);

            // Set the text object and assign a number,
            // set the parent of the slider object so it can be added to the UI panels.
            gSliderObject.GetComponent<RectTransform>().SetParent(m_gScrollContent.transform);
            gSliderObject.GetComponentInChildren<Text>().text = "Sphere " + (i + 1);

            // Set spheres parent to the gameobject that will generate grid
            oSphereObject.GetComponent<Transform>().SetParent(m_gSphereGirdParent.transform);

            // Assign this slider to this sphere object for later use
            oSphereObject.AssignSlider(gSliderObject);

            // Set the sphere and slider objects to inactive
            oSphereObject.gameObject.SetActive(false);
            gSliderObject.SetActive(false);

            // Set the sphere to ready
            oSphereObject.SetReady(true);

            // Add inactive objects to the pools ready for usage
            m_aoSphereObjectPool[i] = oSphereObject;
            m_agSliderObjectPool[i] = gSliderObject;
        }

        // Add AdjustAllSpheres method as a listener for the onValueChanged event for the all sphere slider
        m_gAllSpheresSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(delegate { AdjustAllSpheres(); });

        // Add InitiateLerpAllOpacity method as a listener for the onClick event of the all sphere slider objects button/ text object
        m_gAllSpheresSlider.GetComponentInChildren<Button>().onClick.AddListener(delegate { InitiateLerpAllOpacity(); });
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void Update()
    {
        // if the current active spheres doesn't match the actual active spheres
        // and ensure it is smaller than the pool size.
        if (m_nCurrentActiveSpheres != m_nActiveSpheres && m_nActiveSpheres <= m_nPoolSize)
        {
            // Loop through and reset the pool.
            for (int i = 0; i < m_nPoolSize; ++i)
            {
                // temporarily set the opacity zero bool to false,
                // this is used to disable sphere but not the slider,
                // but when reloading the grid it needs to be set false
                // so everything can reset correctly. 
                m_aoSphereObjectPool[i].SetOpacityZeroBool(false);
                m_agSliderObjectPool[i].SetActive(false);

                // If any sphere is active change it back to inactve
                if (m_aoSphereObjectPool[i].gameObject.activeSelf)
                    m_aoSphereObjectPool[i].gameObject.SetActive(false);
            }

            // Loop through the pool and active more spheres
            for (int i = 0; i < m_nActiveSpheres; ++i)
            {
                // Active spehere
                m_aoSphereObjectPool[i].gameObject.SetActive(true);
            }

            // Update the current active spheres
            m_nCurrentActiveSpheres = m_nActiveSpheres;
        }
    }

    //--------------------------------------------------------------------------------------
    // AdjustAllSpheres: Loop through all active spheres and adjust opacity alltogether.
    //--------------------------------------------------------------------------------------
    private void AdjustAllSpheres()
    {
        // Loop through the pool.
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // If any sphere is active change it back to inactve
            if (m_aoSphereObjectPool[i].GetOpacitySlider().activeSelf)
                m_aoSphereObjectPool[i].SetOpacitySlider(m_gAllSpheresSlider.GetComponentInChildren<Slider>().value);
        }
    }

    //--------------------------------------------------------------------------------------
    // InitiateLerpAllOpacity: Loop through all active spheres and lerp the opacity alltogether.
    //--------------------------------------------------------------------------------------
    private void InitiateLerpAllOpacity()
    {
        // Loop through the pool.
        for (int i = 0; i < m_nPoolSize; ++i)
        {
            // If any sphere is active change it back to inactve
            if (m_aoSphereObjectPool[i].GetOpacitySlider().activeSelf)
                m_aoSphereObjectPool[i].InitiateLerpOpacity();
        }
    }
}
