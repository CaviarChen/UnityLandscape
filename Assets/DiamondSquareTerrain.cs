using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquareTerrain : MonoBehaviour {
	public int numEdges;
	public float mapLength;
	public float height;

	Vector3[] mVertices;
	Vector2[] mUVs;
	int[] mTriangles;
	int numVertices;

	// Use this for initialization
	void Start () {
		SetVerticesPositions ();
		Generate4CornorHeight ();
		GenerateOtherPointHeight ();
		GenerateMyMesh ();
	}

	//meshGenerator
	void SetVerticesPositions(){
		numVertices = (numEdges + 1) * (numEdges + 1);
		mVertices = new Vector3[numVertices];
		mUVs = new Vector2[numVertices];
		//number of triangles = twice the number of squares and each triangle have 3 vertices
		mTriangles = new int[numEdges * numEdges * 2 * 3]; 

		float halfMapLength = 0.5f * mapLength;
		float edgeLength = mapLength / numEdges;

		//iteration sets up positions of mVertices, mUVs, mTriangles;
		int triangleIndx = 0;
		for (int i = 0; i <= numEdges; i++) {
			for (int j = 0; j <= numEdges; j++) {
				mVertices [i * (numEdges + 1) + j] = new Vector3 (-halfMapLength + j * edgeLength, 0.0f, halfMapLength - i * edgeLength);
				mUVs [i * (numEdges + 1) + j] = new Vector2 (i * (1.0f / numEdges), j * (1.0f / numEdges));
				//no need to create triangles when we reach the right most and bottom most line in the map.
				if (i < numEdges && j < numEdges) {
					int topleft = i * (numEdges + 1) + j;
					int topright = i * (numEdges + 1) + j + 1;
					int bottonleft = (i + 1) * (numEdges + 1) + j;
					int bottonright = (i + 1) * (numEdges + 1) + j + 1;

					mTriangles [triangleIndx] = bottonleft;
					mTriangles [triangleIndx + 1] = topleft;
					mTriangles [triangleIndx + 2] = bottonright;

					mTriangles [triangleIndx + 3] = topright;
					mTriangles [triangleIndx + 4] = bottonright;
					mTriangles [triangleIndx + 5] = topleft;

					triangleIndx += 6;
				}
			}
		}
	}

	void Generate4CornorHeight(){
		mVertices[0].y = Random.Range(-height,height);
		mVertices [numEdges].y = Random.Range (-height, height);
		mVertices [numVertices - numEdges - 1].y = Random.Range (-height, height);
		mVertices [numVertices - 1].y = Random.Range (-height, height);
	}
		
	void GenerateOtherPointHeight(){
		//number of iterations need to apply DiamondSquare Algorithm
		int numIteration = (int)Mathf.Log(numEdges,2);

		//number of squares on each side
		//at initial point, it is 1 entile big square
		//the big square has length equal the number of edges
		//after each iteration, numSquareEachSide will be double
		//lengthOfSquare will be halved.
		int numSquareEachSide = 1;
		int lengthOfSquare = numEdges;

		//iteration to give height to each vertex
		for (int i = 0; i < numIteration; i++) {

			//go through the row
			int row = 0;
			for (int j = 0; j < numSquareEachSide; j++) {
				
				//go through the colomn
				int colomn = 0;
				for (int k = 0; k < numSquareEachSide; k++) {

					//apply diamond-square algorithm to vertex at point (row,colomn);
					ApplyDiamondSquareAlgorithm(row,colomn,lengthOfSquare);

					colomn += lengthOfSquare;
				}
			row += lengthOfSquare;
			}

			numSquareEachSide *= 2;
			lengthOfSquare /= 2;
			height *= 0.6f;
		}

	}
	private void ApplyDiamondSquareAlgorithm(int row,int col,int lengthOfSquare){
		int halfLenghtOfSqaure = (int)(lengthOfSquare * 0.5f);

		int topleftIndex = row * (numEdges + 1) + col;
		int toprightIndex = topleftIndex + lengthOfSquare;
		int bottomleftIndex = (row + lengthOfSquare) * (numEdges + 1) + col;
		int bottomrightIndex = bottomleftIndex + lengthOfSquare;

		int midIndex = (row + halfLenghtOfSqaure) * (numEdges + 1) + col + halfLenghtOfSqaure;

		mVertices [midIndex].y = Random.Range (-height, height) + 0.25f * (mVertices [topleftIndex].y + 
			mVertices [toprightIndex].y + mVertices [bottomleftIndex].y + mVertices [bottomrightIndex].y);


		int topIndex = topleftIndex + halfLenghtOfSqaure;
		int leftIndex = midIndex - halfLenghtOfSqaure;
		int rightIndex = midIndex + halfLenghtOfSqaure;
		int bottomIndex = bottomleftIndex + halfLenghtOfSqaure;

		mVertices [topIndex].y = Random.Range (-height, height) + (mVertices [topleftIndex].y + mVertices [toprightIndex].y + mVertices [midIndex].y) / 3;
		mVertices [leftIndex].y = Random.Range (-height, height) + (mVertices [topleftIndex].y + mVertices [bottomleftIndex].y + mVertices [midIndex].y) / 3;
		mVertices [bottomIndex].y = Random.Range (-height, height) + (mVertices [bottomleftIndex].y + mVertices [bottomrightIndex].y + mVertices [midIndex].y) / 3;
		mVertices [rightIndex].y = Random.Range (-height, height) + (mVertices [toprightIndex].y + mVertices [bottomrightIndex].y + mVertices [midIndex].y) / 3;

	}

	void GenerateMyMesh(){
		//assign values to mash
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = mVertices;
		mesh.uv = mUVs;
		mesh.triangles = mTriangles;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
	}
}
