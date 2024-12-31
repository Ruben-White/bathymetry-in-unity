using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bathymetry : MonoBehaviour {
    // Load data from a ascii file

    [SerializeField] private String filePath = @"C:\Users\Ruben\Google Drive\Grid.asc";
    [SerializeField, HideInInspector] private MeshFilter[] meshFilters;
    [SerializeField] private Material material;
    [Range(1 / 1000, 10)]
    [SerializeField] private Vector3 scale = Vector3.one;
    public bool autoUpdate;

    // Start is called before the first frame update
    void Initialise() {
        string[] meshNames = new string[] { "Top", "Bottom", "Edges" };
        if (meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[3];
        }

        for (int i = 0; i < 3; i++) {
            if (meshFilters[i] == null) {
                GameObject meshObject = new GameObject(meshNames[i]);
                meshObject.transform.SetParent(transform);

                meshObject.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                meshFilters[i].GetComponent<MeshRenderer>().material = new Material(material);
            }
        }
    }

    // Draw the bathymetry
    public void DrawBathymetry() {
        // Initialise the mesh filters
        Initialise();

        // Load raster data
        DataLoader.RasterData rasterData = new DataLoader.RasterData();
        rasterData.readFile(filePath);
        rasterData.replaceNoDataValues(rasterData.yMin);

        Debug.Log("Data loaded");
        rasterData.printMetaData();

        // Offset the data so that the minimum values are at (0, 0, 0)
        Vector3 offset = new Vector3(-rasterData.xMin, -rasterData.yMin, -rasterData.zMin);

        // Generate the mesh
        meshFilters[0].sharedMesh = MeshGenerator.GenerateTopMesh(rasterData, scale, offset);
        meshFilters[1].sharedMesh = MeshGenerator.GenerateTopMesh(rasterData, scale, offset);
        meshFilters[2].sharedMesh = MeshGenerator.GenerateTopMesh(rasterData, scale, offset);
    }
}