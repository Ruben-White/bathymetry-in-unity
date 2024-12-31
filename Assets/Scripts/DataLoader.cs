using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSGeo.GDAL;
using Unity.VisualScripting;

public class DataLoader {
    public class RasterData {
        // Data
        public float[,] yData;

        // MetaData
        public int xSize, zSize;
        public float xRes, zRes;
        public float xMin, zMin, yMin;
        public float xMax, zMax, yMax;
        public float noDataValue;

        public void readFile(string filePath) {
            // Register the GDAL drivers
            Gdal.AllRegister();

            // Open the file
            Dataset Dataset = Gdal.Open(filePath, Access.GA_ReadOnly);

            // Get the geotransform
            double[] transform = new double[6];
            Dataset.GetGeoTransform(transform);

            // Get the size and bounds of the raster
            xSize = Dataset.RasterXSize;
            zSize = Dataset.RasterYSize;
            xRes = (float)transform[1];
            zRes = (float)transform[5];
            xMin = (float)transform[0];
            xMax = xMin + xSize * xRes;
            yMax = (float)transform[3];
            yMin = yMax + zSize * zRes;

            // Get the yData as a 1D array
            float[] yData1D = new float[xSize * zSize];
            Dataset.GetRasterBand(1).ReadRaster(0, 0, xSize, zSize, yData1D, xSize, zSize, 0, 0);

            // Convert the yData to 2D array
            yData = new float[zSize, xSize];
            for (int i = 0; i < zSize; i++) {
                for (int j = 0; j < xSize; j++) {
                    yData[i, j] = yData1D[i * xSize + j];
                }
            }

            // Get the nodata value
            int hasNoData;
            double noDataValueOut;
            Dataset.GetRasterBand(1).GetNoDataValue(out noDataValueOut, out hasNoData);

            // Check if the no yData value is valid
            if (hasNoData == 0) {
                Debug.Log("No yData value not found. Using NaN.");
                noDataValue = float.NaN;
            } else {
                Debug.Log("No yData value found: " + noDataValueOut);
                noDataValue = (float)noDataValueOut;
            }

            // Get the min and max values of the yData
            updateMinMax();

            // Close the yDataset
            Dataset.Dispose();
        }

        public void updateMinMax() {
            // Update the min and max values of the yData
            yMin = float.MaxValue;
            yMax = float.MinValue;
            for (int i = 0; i < zSize; i++) {
                for (int j = 0; j < xSize; j++) {
                    if (yData[i, j] < yMin && yData[i, j] != noDataValue) {
                        yMin = yData[i, j];
                    }
                    if (yData[i, j] > zMax && yData[i, j] != noDataValue) {
                        yMax = yData[i, j];
                    }
                }
            }
        }

        public void replaceNoDataValues(float newValue) {
            // Replace the no data values with the new value
            for (int i = 0; i < zSize; i++) {
                for (int j = 0; j < xSize; j++) {
                    if (yData[i, j] == noDataValue) {
                        yData[i, j] = newValue;
                    }
                }
            }

            // Update the min and max values of the Data
            updateMinMax();
        }

        public void printMetaData() {
            // Print the metaData
            Debug.Log("xSize: " + xSize + "; zSize: " + zSize);
            Debug.Log("xMin: " + xMin + "; zMin: " + zMin + "; yMin: " + yMin);
            Debug.Log("xMax: " + xMax + "; zMax: " + zMax + "; yMax: " + yMax);
            Debug.Log("xRes: " + xRes + "; zRes: " + zRes);
        }
    }
}

