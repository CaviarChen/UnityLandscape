using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
	public int size = 17; // 2^4+1
	public float unitSize;
	public float heightRange;
	public float smoothness = 1.7f;

	private float[,] heightMap;

	// Use this for initialization
	void Start () {
		GenerateHeightMap ();
		GetComponent<MeshFilter> ().mesh = GenerateMesh ();
		
	}

	bool inRange(int x, int z) {
		return (x >= 0) && (z >= 0) && (x < size) && (z < size);
	}

	// Use Diamond Square Algorithm to generate height map
	void GenerateHeightMap() {
		heightMap = new float[size, size];
		bool[,] isSet = new bool[size, size];

		for (int x = 0; x < size; x++) {
			for (int z = 0; z < size; z++) {
				isSet [x, z] = false;
			}
		}

		float range = heightRange;

		// Corner
		heightMap[0,0] = Random.Range(-range, range);
		heightMap[size-1,0] = Random.Range(-range, range);
		heightMap[0,size-1] = Random.Range(-range, range);
		heightMap[size-1,size-1] = Random.Range(-range, range);

		isSet [0, 0] = true;
		isSet [size-1, 0] = true;
		isSet [0, size-1] = true;
		isSet [size-1, size-1] = true;

		// Diamond Square
		for (int length = size - 1; length >= 2; length /= 2) {
			int half = length / 2;


			range /= smoothness;



			// Diamond
			for (int x = half; x < size-half; x += length) {
				for (int z = half; z < size-half; z += length) {
					float tmp = 0;



					tmp += heightMap [x + half, z + half];
					tmp += heightMap [x + half, z - half];
					tmp += heightMap [x - half, z + half];
					tmp += heightMap [x - half, z - half];

					heightMap [x, z] = tmp / 4.0f + Random.Range(-range, range);
					isSet [x, z] = true;

				}
			}
				

			// Square
			for (int x = 0; x < size; x += half) {
				for (int z = 0; z < size; z += half) {
					if (!isSet[x,z]) { 

						// haven't benn computed in diamond step
						float tmp = 0;
						int counter = 0;

						if (inRange (x + half, z)) {
							tmp += heightMap [x + half, z];
							counter++;
						}
						if (inRange (x - half, z)) {
							tmp += heightMap [x - half, z];
							counter++;
						}
						if (inRange (x, z + half)) {
							tmp += heightMap [x, z + half];
							counter++;
						}
						if (inRange (x, z - half)) {
							tmp += heightMap [x, z - half];
							counter++;
						}

						heightMap [x, z] = tmp / counter + Random.Range(-range, range);

						isSet [x, z] = true;

					}
					

				}
			}




		}



	}


	Mesh GenerateMesh() {
		Mesh mesh = new Mesh();

		Vector3[] mVertices = new Vector3[size * size];
		Vector2[] mUv = new Vector2[size * size];


		int counter = 0;
		for (int x = 0; x < size; x++) {
			for (int z = 0; z < size; z++) {
				
				mVertices[counter] = new Vector3(x * unitSize, heightMap[x, z], z * unitSize);

				counter++;
			}
		}
			
		int[] mTriangles = new int[(size - 1) * (size - 1) * 2 * 3];

		counter = 0;

		for (int i = 0; i < size-1; i++) {
			for (int j = 0; j < size-1; j++) {

				//  a __ b
				//   |  |
				//  c -- d

				int a = i * size + j;
				int b = i * size + j + 1;
				int c = (i+1) * size + j;
				int d = (i+1) * size + j + 1;

				mTriangles [counter++] = a;
				mTriangles [counter++] = d;
				mTriangles [counter++] = c;

				mTriangles [counter++] = a;
				mTriangles [counter++] = b;
				mTriangles [counter++] = d;

			}
		}




		mesh.vertices = mVertices;
		//mesh.uv = mUVs;
		mesh.triangles = mTriangles;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		return mesh;
		
	}





	
	// Update is called once per frame
	void Update () {
		
	}
}
