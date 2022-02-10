//--------------------------------------------------------------------------------------
// Purpose: Generate the grid of sphere for the right side of the screen.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// GridManager object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class GridManager : MonoBehaviour
{
    // SCREEN SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Screen Settings:")]

    // public float for the percentage of the screen the left panel takes up
    public float m_fScreenPercentageChange = 30;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE VALUES //
    //--------------------------------------------------------------------------------------
    // private int for the number of sphere to be added to the grid
    private int m_nNumberOfSpheres;
    
    // private in for the number of columns in the grid based on the screensize and the amount of spheres
    private int m_nColumns = 5;
    
    // private float for the spacing between each sphere in the grid.
    private float m_fSpacing = 2;

    // private vector3 for the spawn position of the grid.
    private Vector3 m_v3SpawnPosition;
    
    // private transform array for all the sphere to use in the grid.
    private Transform[] m_atSpheres;

    // private vector3 for the current screen size, used for checking if the screen size has changed.
    private Vector3 m_v3CurrentScreenSize;

    // private int for the current sphere count of the grid, used for checking if the sphere count has changed.
    private int m_nCurrentSphereCount = 0;
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // Get all the spheres, which a children of this object
        m_atSpheres = new Transform[transform.childCount];

        // Loop through children
        for (int i = 0; i < transform.childCount; i++)
        {
            // Addd each child to the spheres array
            m_atSpheres[i] = transform.GetChild(i);
        }
    }

    //--------------------------------------------------------------------------------------
    // LateUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    private void LateUpdate()
    {
        // Calculate the spawn postion of the grid based on the camera perspective
        m_v3SpawnPosition = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width * (m_fScreenPercentageChange / 100)) + CalculateCellSizeToScreenPoint().x, Screen.height - CalculateCellSizeToScreenPoint().y, 0));

        // Check if the screen size or the amount of spheres has changed.
        if (m_v3CurrentScreenSize != new Vector3(Screen.width, Screen.height, 0.0f) || m_nCurrentSphereCount != SphereManager.m_oInstance.m_nActiveSpheres)
        {
            // Generate the grid
            GenerateGrid();

            // Reset the variables for screen size and sphere count ready for next check
            m_v3CurrentScreenSize = new Vector3(Screen.width, Screen.height, 0.0f);
            m_nCurrentSphereCount = SphereManager.m_oInstance.m_nActiveSpheres;
        } 
    }

    //--------------------------------------------------------------------------------------
    // CalculateCellSizeToScreenPoint: Calculate the size of the sphere from the cameras
    // perspective.
    //
    // Returns:
    //      Vector3: The size of a sphere in realtion to the camera
    //--------------------------------------------------------------------------------------
    private Vector3 CalculateCellSizeToScreenPoint()
    {
        // Get the min and max bounds of the sphere
        Vector3 v3Min = m_atSpheres[0].GetComponent<MeshRenderer>().bounds.min;
        Vector3 v3Max = m_atSpheres[0].GetComponent<MeshRenderer>().bounds.max;

        // Get the min and max of sphere bounds in screen
        Vector3 v3ScreenMin = Camera.main.WorldToScreenPoint(v3Min);
        Vector3 v3ScreenMax = Camera.main.WorldToScreenPoint(v3Max);

        // Calculate size
        float fScreenWidth = v3ScreenMax.x - v3ScreenMin.x;
        float fScreenHeight = v3ScreenMax.y - v3ScreenMin.y;

        // Return the size of the sphere in realtion to the camera.
        return new Vector3(fScreenWidth, fScreenHeight);
    }

    //--------------------------------------------------------------------------------------
    // CalculateScreenWidth: Calculate the width of the screen account for the screen change
    // percentage change.
    //
    // Returns:
    //      float: The screens width, accounting for percentage change.
    //--------------------------------------------------------------------------------------
    private float CalculateScreenWidth()
    {
        // Return the screen width divided by itself multiple by the percentage.
        return (Screen.width - (Screen.width * (m_fScreenPercentageChange / 100)));
    }

    //--------------------------------------------------------------------------------------
    // GenerateGrid: Generate and calculate a responsive gird based on the amount of spheres
    // and the resoultion of the screen.
    //--------------------------------------------------------------------------------------
    private void GenerateGrid()
    {
        // Reset the number of spheres before generating.
        m_nNumberOfSpheres = 0;

        // loop through all the spheres
        for (int i = 0; i < m_atSpheres.Length; ++i)
        {
            // For however many spheres are active increase the number of spheres 
            if (m_atSpheres[i].GetComponent<Sphere>().GetOpacitySlider().activeSelf)
                m_nNumberOfSpheres++;
        }

        // If there is at least 1 sphere
        if (m_nNumberOfSpheres > 0)
        {
            // Calculate the size of the screen and the possible columns and rows
            double dRatio = (CalculateScreenWidth() - (CalculateCellSizeToScreenPoint().x * 2)) / Screen.height;
            double dColsCount = System.Math.Sqrt(m_nNumberOfSpheres * dRatio);
            double dRowsCount = m_nNumberOfSpheres / dColsCount;

            // Find the best possible option of rows and columns to fit the height
            // Get the ceiling of the rows and the ceiling of sphere count divide by that value
            double dPossibleRows1 = System.Math.Ceiling(dRowsCount);
            double dPossibleCols1 = System.Math.Ceiling(m_nNumberOfSpheres / dPossibleRows1);
            
            // Increase the best rows to fit screen
            while (dPossibleRows1 * dRatio < dPossibleCols1)
            {
                dPossibleRows1++;
                dPossibleCols1 = System.Math.Ceiling(m_nNumberOfSpheres / dPossibleRows1);
            }
            
            // New double for the best option based on height and adjusted to the size of the sphere
            double dPossibleOption1 = (CalculateCellSizeToScreenPoint().x) / dPossibleRows1;

            // Find the best possible option of rows and columns to fit the width
            // Get the ceiling of the cols and the ceiling of sphere count divide by that value
            var dPossibleCols2 = System.Math.Ceiling(dColsCount);
            var dPossibleRows2 = System.Math.Ceiling(m_nNumberOfSpheres / dPossibleCols2);
            
            // Incrase the best column to fit screen
            while (dPossibleCols2 < dPossibleRows2 * dRatio)
            {
                dPossibleCols2++;
                dPossibleRows2 = System.Math.Ceiling(m_nNumberOfSpheres / dPossibleCols2);
            }

            // New double for the best option based on width and adjusted to the size of the sphere
            double dPossibleOption2 = (CalculateCellSizeToScreenPoint().x) / dPossibleCols2;

            // New double for the best column calculation
            double dBestColumn;

            // Find the best option from the two calculated options above
            if (dPossibleOption1 < dPossibleOption2)
                dBestColumn = dPossibleCols2;
            else
                dBestColumn = dPossibleCols1;

            // Calculuate the appropriate spacing and sell size
            double fCell = (dBestColumn * 0.85f);
            fCell = Mathf.Floor((float)fCell) / 100;
            fCell = fCell * (float)(SphereManager.m_oInstance.m_nPoolSize / m_nNumberOfSpheres);

            // Make sure the fCell is always at least 1
            if (fCell == 0)
                fCell = 1;

            // Apply new column and spacing values
            m_nColumns = (int)dBestColumn;
            m_fSpacing = (float)(fCell);

            // If there is at least 1 column
            if (m_nColumns > 0)
            {
                // Loop through all the spheres
                for (int i = 0; i < m_atSpheres.Length; ++i)
                {
                    // Grab only the sphere that are active in the grid
                    if (m_atSpheres[i].GetComponent<Sphere>().GetOpacitySlider().activeSelf)
                    {
                        // Set the scale of the sphere based on the newly calculated cell size
                        m_atSpheres[i].localScale = new Vector3(1, 1, 1);
                        m_atSpheres[i].localScale = new Vector3((float)fCell, (float)fCell, (float)fCell);

                        // Calculate the postion of the sphere on the grid
                        int fRow = (i / m_nColumns) - ((i / m_nColumns) * 2);
                        int nColumn = i % m_nColumns;

                        // Change the postions of the spheres to poistions on the grid
                        m_atSpheres[i].position = new Vector2(m_v3SpawnPosition.x + (nColumn * m_fSpacing), m_v3SpawnPosition.y + (fRow * m_fSpacing));
                    }
                }
            }
        }
    }
}