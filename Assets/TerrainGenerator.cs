using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
	public Color snow_color, tree_color, grass_color, mountain_color, sea_color,sand_color;

	// number of vertices on one row of the map
	public int size = 17; // n^2+1 
	public float unitSize; // length between 2 vertices
	public float heightRange; // height should be between -heightRange~heightRange
	public float smoothness = 1.7f;//how much height is going to decrease;

	public Shader shader;
	public Sun sun;

	//a plane at a certain height across landscape heightmap
	public GameObject seaObject;

	private float heightMin, heightMax, heightAvg, seaLevel;

	private float[,] heightMap;


	// Use this for initialization
	void Start () {

		GenerateHeightMap ();

        // terrain
		MeshFilter terrainMesh = this.gameObject.AddComponent<MeshFilter> ();
		terrainMesh.mesh = GenerateMesh ();

		MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer> ();
		renderer.material.shader = shader;

        // sea
		MeshFilter seaMesh = seaObject.AddComponent<MeshFilter> ();
		seaMesh.mesh = GenerateSea ();

		MeshRenderer seaRenderer = seaObject.AddComponent<MeshRenderer> ();
		seaRenderer.material.shader = shader;



		// move to center (sun rotates around 0,0,0)
		this.gameObject.transform.localPosition = new Vector3(-unitSize*(size-1) / 2 , 0, -unitSize*(size-1) / 2);
		seaObject.transform.localPosition = new Vector3(-unitSize*(size-1) / 2 , 0, -unitSize*(size-1) / 2);
		sun.gameObject.transform.position = new Vector3(0 , unitSize*(size-1), 0);

		GetComponent<MeshCollider> ().sharedMesh = terrainMesh.mesh;
        seaObject.GetComponent<MeshCollider>().sharedMesh = seaMesh.mesh;

    }


	Mesh GenerateSea() {

        // generate a flat sea square at sealevel
		Mesh mesh = new Mesh();

		Vector3[] mVertices = new Vector3[4];
		mVertices [0] = new Vector3 (0, seaLevel, 0);
		mVertices [1] = new Vector3 (0, seaLevel, unitSize*(size-1));
		mVertices [2] = new Vector3 (unitSize*(size-1), seaLevel, 0);
		mVertices [3] = new Vector3 (unitSize*(size-1), seaLevel, unitSize*(size-1));

		int[] mTriangles = new int[6];

		mTriangles [0] = 0;
		mTriangles [1] = 3;
		mTriangles [2] = 2;

		mTriangles [3] = 0;
		mTriangles [4] = 1;
		mTriangles [5] = 3;

		Color[] colors = new Color[mVertices.Length];
		for (int i = 0; i < mVertices.Length; i++) {
			colors [i] = sea_color;
		}

		mesh.vertices = mVertices;
		mesh.triangles = mTriangles;
		mesh.colors = colors;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		return mesh;
	}

    // return if a coordinate is in the range of height map
    bool inRange(int x, int z) {
		return (x >= 0) && (z >= 0) && (x < size) && (z < size);
	}

	// Use Diamond Square Algorithm to generate height map
	void GenerateHeightMap() {
		heightMap = new float[size, size];
		//isSet stores whether a vertex has already been given a height
		bool[,] isSet = new bool[size, size];

		for (int x = 0; x < size; x++) {
			for (int z = 0; z < size; z++) {
				isSet [x, z] = false;
			}
		}

		float range = heightRange;

		// generate height on 4 cornors of the map
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



			// Diamond Step:
			//For each square in the array, set the midpoint of that square to be the average of the four corner points plus a random value.
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


			// Square Step:
			// For each diamond in the array, set the midpoint of that diamond to be the average of the four corner points plus a random value.
			// On the boundary, set the midpoint of that diamond to be the average of the three corner points plus a random value.
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

    // generate terrain mesh based on height map
	Mesh GenerateMesh() {
		Mesh mesh = new Mesh();

		Vector3[] mVertices = new Vector3[size * size];
		Vector2[] mUv = new Vector2[size * size];


		int counter = 0;
		float min = 0, max=0, sum=0;
		for (int x = 0; x < size; x++) {
			for (int z = 0; z < size; z++) {
				if (heightMap [x, z] < min) {
					min = heightMap [x, z];
				}

				if(heightMap [x, z] > max){
					max = heightMap [x, z];
				}

				sum += heightMap [x, z];

				mVertices[counter] = new Vector3(x * unitSize, heightMap[x, z], z * unitSize);

				counter++;
			}
		}

		heightAvg = (float)sum / (size * size);
		heightMin = min;
		heightMax = max;

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
		mesh.triangles = mTriangles;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		Coloring (mesh);
		return mesh;

	}


	// Coloring heightmap based on the height value of each vertex
	// From low to high, coloured as sand, tree, grass, tree, mountain, and snow
	void Coloring(Mesh mesh){


        
        seaLevel = heightAvg - (heightMax - heightMin) * 0.2f;

        // level for different colors
        double[] lvl = new double[5];
        lvl[0] = seaLevel;
        lvl[1] = seaLevel + (heightAvg - seaLevel) * 0.2;
        lvl[2] = 0.1 * (heightMax - heightAvg) + heightAvg;
        lvl[3] = 0.3 * (heightMax - heightAvg) + heightAvg;
        lvl[4] = 0.6 * (heightMax - heightAvg) + heightAvg;

        Color[] colors = new Color[mesh.vertices.Length];

		for (int i = 0; i < colors.Length; i++) {

            float currentY = mesh.vertices[i].y;
            // add some noises
            currentY += Random.Range(0.5f, -0.5f);

            if (currentY <= lvl[0]) {
                colors[i] = sand_color;
            } else if (currentY > lvl[0] && currentY <= lvl[1]) {
                colors[i] = sand_color;
            } else if (currentY > lvl[1] && currentY <= lvl[2]) {
                colors[i] = tree_color;
            } else if (currentY > lvl[2] && currentY <= lvl[3]) {
                colors[i] = grass_color;
            } else if (currentY > lvl[3] && currentY <= lvl[4])
            {
                colors[i] = mountain_color;
            } else {
                colors[i] = snow_color;
            }
		}

		mesh.colors = colors;
	}



	// Update is called once per frame
	void Update () {
		// Update point loght color and point light position by position and color change of sun
		MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
		renderer.material.SetColor ("_PointLightColor", this.sun.color);
		renderer.material.SetVector ("_PointLightPosition", this.sun.GetPointLightPosition());

		renderer = seaObject.GetComponent<MeshRenderer>();
		renderer.material.SetColor ("_PointLightColor", this.sun.color);
		renderer.material.SetVector ("_PointLightPosition", this.sun.GetPointLightPosition());


	}
}
