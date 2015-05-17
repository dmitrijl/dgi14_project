using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class readmesh : MonoBehaviour {

	public GameObject plane;
	public string filesetName;
	public float x_base;
	public float z_base;

	// Use this for initialization
	void Start () {
		//Debug.Log("Running script!!!\n");
		string dir_path;
		string inp_ln;
		//dir_path = @"C:\Users\Dmitrij\Documents\KTH\dgi\TriangleUnity\Assets\polyfiles\";	//Change as appropriate
		dir_path = @"Assets\polyfiles\abcd\";	//Change as appropriate
		int lc = 0;
		bool sepNodeFile = false;
		string[] words;
		//int iinp;	//Integer input holder
		//double dinp;	//Floating point input holder
		//int dim;	//Must be 2. Not currently used, though.
		int nrVert, nrAttr, nrBM;
		Vector3[] vertices;
		Vector2[] UV = new Vector2[0];
		int[] triangles;
		Char[] delim = {' '};


		string polyname = filesetName;
		//Read poly file
		StreamReader inp_stm = new StreamReader(dir_path + polyname + ".1.poly");
		Debug.Log("Reading POLY file!");
		inp_ln = inp_stm.ReadLine( );	//Read line about vertices
		words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
		int.TryParse(words[0], out nrVert);
		//int.TryParse(words[1], out dim);
		int.TryParse(words[2], out nrAttr);
		int.TryParse(words[3], out nrBM);

		vertices = new Vector3[nrVert];
		if (nrVert == 0) {
			sepNodeFile = true;	//Separate node file exists
		} else {
			for (int i = 0; i<nrVert; ++i) {
				inp_ln = inp_stm.ReadLine ();	//Read vertex line
				words = inp_ln.Split (delim, StringSplitOptions.RemoveEmptyEntries);
				float.TryParse (words [1], out vertices [i].x);
				float.TryParse (words [2], out vertices [i].y);
				float.TryParse (words [3], out vertices [i].z);
				float attr, bm;	//Dummy placeholders for now
				for (int k = 3; k<3+nrAttr; ++k) {
					float.TryParse (words [k], out attr);
				}
				for (int k = 3+nrAttr; k<3+nrAttr+nrBM; ++k) {
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





		//Read node file (if necessary)
		if (sepNodeFile) {
			inp_stm = new StreamReader(dir_path + polyname + ".1.node");
			Debug.Log("Reading NODE file!");
			inp_ln = inp_stm.ReadLine( );	//Read line about vertices
			words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
			int.TryParse(words[0], out nrVert);
			//int.TryParse(words[1], out dim);
			int.TryParse(words[2], out nrAttr);
			int.TryParse(words[3], out nrBM);
			
			vertices = new Vector3[nrVert];
			
			for (int i = 0;i<nrVert;++i) {
				inp_ln = inp_stm.ReadLine( );	//Read vertex line
				words = inp_ln.Split(delim, StringSplitOptions.RemoveEmptyEntries);
				float.TryParse(words[1], out vertices[i].x);
				float.TryParse(words[2], out vertices[i].z);
				float.TryParse(words[3], out vertices[i].y);
				float attr, bm;	//Dummy placeholders for now
				for (int k = 3;k<3+nrAttr;++k) {
					float.TryParse(words[k], out attr);
				}
				for (int k = 3+nrAttr;k<3+nrAttr+nrBM;++k) {
					float.TryParse(words[k], out bm);
				}
			}
			inp_stm.Close( );
		}







		//Read ele file (triangles)
		inp_stm = new StreamReader(dir_path + polyname + ".1.ele");
		Debug.Log("Reading ELE file!");
		inp_ln = inp_stm.ReadLine ();
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




		//Debug everything
		/*Debug.Log("This is the read result:\n");
		Debug.Log(vertices);
		foreach (Vector3 v3 in vertices) {
			Debug.Log(v3.x);
			Debug.Log(v3.y);
			Debug.Log(v3.z);
		}
		Debug.Log(triangles);
		for (int i = 0; i<triangles.Length; ++i) {
			Debug.Log(triangles[i]);
		}*/

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

		Mesh mesh = new Mesh ();
		plane.GetComponent<MeshFilter>().mesh = mesh;
		//plane.GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = vertices;
		//mesh.uv = UV;
		Array.Reverse (triangles);
		mesh.triangles = triangles;

		Vector2[] uvs = new Vector2[vertices.Length];
		
		for (int i=0; i < uvs.Length; i++) {
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		}
		mesh.uv = uvs;


		
		
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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
