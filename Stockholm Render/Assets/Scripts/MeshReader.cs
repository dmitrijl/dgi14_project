using UnityEngine;
//using UnityEditor;	//Must be commented when building
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class MeshReader : MonoBehaviour {

	public GameObject plane;
	public string filesetName;
	public string fileName;
	public float x_base;
	public float z_base;
	public bool save_as_asset;
	public bool read_mesh;


	public static class Classification {
		public const int Unclassified=1;
		public const int Ground=2;
		public const int Water=9;
		public const int Bridge=11;
		public const int Building=21;
		public const int Undefined=-1;

		public static readonly Color32 cUnclassified=(Color32)Color.red;
		public static readonly Color32 cGround=(Color32)Color.grey;
		public static readonly Color32 cWater=(Color32)Color.Lerp(Color.blue, Color.cyan, 0.2f);
		public static readonly Color32 cBridge=(Color32)Color.Lerp(Color.grey, Color.white, 0.5f);
		public static readonly Color32 cBuilding=(Color32)Color.magenta;
		public static readonly Color32 cUndefined=(Color32)Color.yellow;

		public static Color32 intToColor32(int ic) {
			switch (ic) {
			case Unclassified: return cUnclassified;
			case Ground: return cGround;
			case Water: return cWater;
			case Bridge: return cBridge;
			case Building: return cBuilding;
			default: return cUndefined;
			}
		}

		public static int color32ToInt(Color32 c32) {
			if (c32.Equals(cUnclassified)) return Unclassified;
			if (c32.Equals(cGround)) return Ground;
			if (c32.Equals(cWater)) return Water;
			if (c32.Equals(cBridge)) return Bridge;
			if (c32.Equals(cBuilding)) return Building;
			return Undefined;
		}

	}

	private Mesh mesh;
	private string dir_path;
	private bool sepNodeFile;
	private string[] words;
	private Char[] delim = {' '};
	private int nrVert, nrAttr, nrBM;
	//private int dim;	//Must be 2. Not currently used, though.

	private Vector3[] vertices;
	private Vector2[] UV;
	private int[] triangles;
	private int[] classifications;
	

	//Prints the vertices
	void debugVertices() {
		Debug.Log("The vertices are:\n");
		Debug.Log(vertices);
		foreach (Vector3 v3 in vertices) {
			Debug.Log(v3.x);
			Debug.Log(v3.y);
			Debug.Log(v3.z);
		}
	}

	//Prints the triangles
	void debugTriangles() {
		Debug.Log("The triangles are:\n");
		Debug.Log(triangles);
		for (int i = 0; i<triangles.Length; ++i) {
			Debug.Log(triangles[i]);
		}
	}


	//Reads and parses a .poly file
	void readPolyFile(string fPath) {
		Debug.Log("Reading POLY file!");
		StreamReader inp_stm = new StreamReader(fPath);
		string inp_ln = inp_stm.ReadLine( );	//Read line about vertices
		words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		int.TryParse(words[0], out nrVert);
		//int.TryParse(words[1], out dim);
		int.TryParse(words[2], out nrAttr);
		--nrAttr;	//To compensate for the fact that height is an attribute
		int.TryParse(words[3], out nrBM);
		
		vertices = new Vector3[nrVert];
		if (nrVert == 0) {
			sepNodeFile = true;	//Separate node file exists
		} else {
			classifications = new int[nrVert];
			for (int i = 0; i<nrVert; ++i) {
				inp_ln = inp_stm.ReadLine ();	//Read vertex line
				words = inp_ln.Split (delim, StringSplitOptions.RemoveEmptyEntries);
				float.TryParse (words [1], out vertices [i].x);
				float.TryParse (words [2], out vertices [i].y);
				float.TryParse (words [3], out vertices [i].z);
				float attr, bm;	//Dummy placeholders for now
				for (int k = 4; k<4+nrAttr; ++k) {
					float.TryParse (words [k], out attr);
					classifications[i] = (int)attr;
				}
				for (int k = 4+nrAttr; k<4+nrAttr+nrBM; ++k) {
					float.TryParse (words [k], out bm);
				}
			}
		}
		
		//Read line about segments
		inp_ln = inp_stm.ReadLine( );
		words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		int nrSegments;
		int.TryParse (words [0], out nrSegments);
		
		for (int i = 0;i < nrSegments; ++i) {
			inp_ln = inp_stm.ReadLine( );
			//TODO parse segments
		}
		
		//Read line about holes
		inp_ln = inp_stm.ReadLine( );
		words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		int nrHoles;
		int.TryParse (words [0], out nrHoles);
		
		for (int i = 0;i < nrHoles; ++i) {
			inp_ln = inp_stm.ReadLine( );
			//TODO parse holes
		}
	}

	
	//Reads and parses a .node file
	void readNodeFile(string fPath) {
		Debug.Log("Reading NODE file!");
		StreamReader inp_stm = new StreamReader(fPath);
		string inp_ln = inp_stm.ReadLine( );	//Read line about vertices
		words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		int.TryParse(words[0], out nrVert);
		//int.TryParse(words[1], out dim);
		int.TryParse(words[2], out nrAttr);
		--nrAttr;	//To compensate for the fact that height is an attribute
		int.TryParse(words[3], out nrBM);
		
		vertices = new Vector3[nrVert];
		classifications = new int[nrVert];
		
		for (int i = 0;i<nrVert;++i) {
			inp_ln = inp_stm.ReadLine( );	//Read vertex line
			words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
			float.TryParse(words[1], out vertices[i].x);
			float.TryParse(words[2], out vertices[i].z);
			float.TryParse(words[3], out vertices[i].y);
			float attr, bm;	//Dummy placeholders for now
			for (int k = 4;k<4+nrAttr;++k) {
				float.TryParse(words[k], out attr);
				classifications[i] = (int)attr;
			}
			for (int k = 4+nrAttr;k<4+nrAttr+nrBM;++k) {
				float.TryParse(words[k], out bm);
			}
		}
		inp_stm.Close( );
	}


	//Reads and parses a .ele file
	void readEleFile(string fPath) {
		Debug.Log("Reading ELE file!");
		StreamReader inp_stm = new StreamReader(fPath);
		string inp_ln = inp_stm.ReadLine ();
		string[] meta = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		if (meta.Length != 3) {
			Debug.Log("First line has more than 3 entries...");
		}
		int numTriangles =  Convert.ToInt32(meta [0]);
		int triangleNumVertices = Convert.ToInt32 (meta [1]);
		if (triangleNumVertices != 3) {
			Debug.Log("Not 3 vertices in a triangle!");
		}
		
		ArrayList triArray = new ArrayList (numTriangles*3);
		for(int i = 0;i < numTriangles; i++) {
			inp_ln = inp_stm.ReadLine( );
			string[] tokens = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
			triArray.Add(Convert.ToInt32(tokens[1])-1);
			triArray.Add(Convert.ToInt32(tokens[2])-1);
			triArray.Add(Convert.ToInt32(tokens[3])-1);
		}
		
		triangles = (int[])triArray.ToArray (typeof(int));
		inp_stm.Close( );
	}


	// Use this for initialization
	void Start () {
		//Debug.Log("Running script!!!\n");
		if (!read_mesh) {
			Debug.Log("Skipped reading mesh from file.\n");
			Debug.Log("Loading from asset database...\n");
			//mesh = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Stadsholmen/res.asset", typeof(Mesh)) as Mesh;
			//plane.GetComponent<MeshFilter>().mesh = mesh;
			return;
		}

		dir_path = "Assets/polyfiles/";	//Change as appropriate
		UV = new Vector2[0];
		classifications = new int[0];
		sepNodeFile = false;
		string polyname = filesetName + "/" + fileName;
		StreamReader inp_stm;

		//Read poly file
		readPolyFile(dir_path + polyname + ".1.poly");

		//Read node file (if necessary)
		if (sepNodeFile) {
			readNodeFile(dir_path + polyname + ".1.node");
		}

		//Read ele file (triangles)
		readEleFile(dir_path + polyname + ".1.ele");


		//Debug vertices and triangles
		//debugVertices();
		//debugTriangles();


		float verticesMinX = 9999999999.0f;
		float verticesMaxX = -99999999999.0f;
		float verticesMinZ = 9999999999.0f;
		float verticesMaxZ = -99999999999.0f;
		float newmin = 0f;
		float newmax = 100f;
		for (int i = 0; i < vertices.Length; i++) {
			if(vertices[i].x < verticesMinX) {
				verticesMinX = vertices[i].x;
			} else if (vertices[i].x > verticesMaxX) {
				verticesMaxX = vertices[i].x;
			}

			if(vertices[i].z < verticesMinZ) {
				verticesMinZ = vertices[i].z;
			} else if (vertices[i].z > verticesMaxZ) {
				verticesMaxZ = vertices[i].z;
			}
		}

		for (int i = 0; i < vertices.Length; i++) {
			//vertices[i].x = ((vertices[i].x - verticesMinX)*newmax)/(verticesMaxX-verticesMinX);
			//vertices[i].z = ((vertices[i].z - verticesMinZ)*newmax)/(verticesMaxZ-verticesMinZ);
			vertices[i].x = (vertices[i].x - verticesMinX) + x_base;
			vertices[i].z = (vertices[i].z - verticesMinZ) + z_base;
			//vertices[i].y = -vertices[i].y;
		}

		//for (int i = 0; i < vertices.Length; i++) {
		//	vertices[i].x = - vertices[i].x;
		//}

		Debug.Log ("Max X: " + (verticesMaxX - verticesMinX));
		Debug.Log ("Max Z: " + (verticesMaxZ - verticesMinZ));
		/*
		Array.Reverse (vertices);
		int l = vertices.Length-1;
		for (int i = 0; i < triangles.Length; i++) {
			triangles[i] = l-triangles[i];
		}
		*/





		mesh = new Mesh();
		plane.GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = vertices;
		//mesh.uv = UV;
		Array.Reverse (triangles);
		mesh.triangles = triangles;

		Vector2[] uvs = new Vector2[vertices.Length];
		
		for (int i=0; i < uvs.Length; i++) {
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		}
		mesh.uv = uvs;


		//Color the vertices
		Color32[] colors = new Color32[vertices.Length];
		for (int j = 0; j < vertices.Length; ++j) {
			colors[j] = Classification.intToColor32(classifications[j]);
		}
		mesh.colors32 = colors;
		
		
		Debug.Log ("numer of vertices: " + mesh.vertices.Length);
		Debug.Log ("number of triangles: " + (mesh.triangles.Length/3.0f));
		Debug.Log ("number of normals: " + mesh.normals.Length);
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		Debug.Log ("numer of vertices after recalculation: " + mesh.vertices.Length);
		Debug.Log ("number of triangles after recalculation: " + (mesh.triangles.Length/3.0f));
		Debug.Log ("number of normals after recalculation: " + mesh.normals.Length);
		/*for (int i = 0; i < vertices.Length; i++) {
			vertices[i].x = - vertices[i].x;
		}*/
		/*
		Vector3[] normals = mesh.normals;
		for (int i = 0; i < normals.Length; i++) {
			normals[i] = -normals[i];
		}
		mesh.normals = normals;

		*/


		/*
		//Must be commented when building
		if (save_as_asset) {
			AssetDatabase.CreateFolder("Assets/Prefabs", filesetName);
			AssetDatabase.CreateAsset(mesh, "Assets/Prefabs/" + polyname + ".asset");
			AssetDatabase.SaveAssets();

			//PrefabUtility.CreatePrefab (dir_path + filesetName + ".prefab",plane);
			//plane.GetComponent<MeshFilter>().mesh = mesh;

			UnityEngine.Object prefab = EditorUtility.CreateEmptyPrefab("Assets/Prefabs/" + polyname + ".prefab");
			EditorUtility.ReplacePrefab(plane, prefab, ReplacePrefabOptions.ReplaceNameBased);
		}
		*/

		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
