using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator {
    public static Mesh GenerateTopMesh(DataLoader.RasterData rasterData, Vector3 scale, Vector3 offset) {
        // Get the dimensions of the data
        int xSize = rasterData.xSize;
        int zSize = rasterData.zSize;

        // Get the cell size of the data
        scale.x = 1;
        scale.y = 1;
        scale.z = 1;

        // Get the offset of the data
        offset.x = 0;
        offset.y = 0;
        offset.z = 0;

        // Create the vertices
        Vector3[] vertices = new Vector3[xSize * zSize];
        for (int i = 0; i < xSize; i++) {
            for (int j = 0; j < zSize; j++) {
                vertices[i * zSize + j] = new Vector3((j + offset.x) * scale.x, (rasterData.yData[i, j] + offset.y) * scale.y, (i + offset.z) * scale.z);
            }
        }

        // Create the triangles
        int[] triangles = new int[(xSize - 1) * (zSize - 1) * 6];
        int index = 0;
        for (int i = 0; i < xSize - 1; i++) {
            for (int j = 0; j < zSize - 1; j++) {
                // Assign the vertices of the triangles in counter-clockwise order
                triangles[index] = i * zSize + j;
                triangles[index + 1] = (i + 1) * zSize + j;
                triangles[index + 2] = i * zSize + j + 1;
                triangles[index + 3] = i * zSize + j + 1;
                triangles[index + 4] = (i + 1) * zSize + j;
                triangles[index + 5] = (i + 1) * zSize + j + 1;
                index += 6;
            }
        }

        // Create the uvs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < xSize; i++) {
            for (int j = 0; j < zSize; j++) {
                uvs[i * zSize + j] = new Vector2((float)j / (zSize - 1), (float)i / (xSize - 1));
            }
        }

        // Create the mesh and assign the vertices, triangles and uvs
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Use 32-bit indices to avoid the 65535 vertices limit
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Recalculate the normals
        mesh.RecalculateNormals();

        // Return the mesh
        return mesh;
    }
}