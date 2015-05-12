using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class readmesh : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Debug.Log("Running script!!!\n");
		string dir_path;
		string inp_ln;
		dir_path = @"C:\Users\Kricka\Desktop\mesh\A\";	//Change as appropriate
		StreamReader inp_stm = new StreamReader(dir_path + "a.1.poly");
		int lc = 0;
		bool sepNodeFile = false;
		string[] words;
		//int iinp;	//Integer input holder
		//double dinp;	//Floating point input holder
		//int dim;	//Must be 2. Not currently used, though.
		int nrVert, nrAttr, nrBM;
		Vector3[] vertices;
		Vector2[] UV;
		int[] triangles;

		//Read poly file
		Debug.Log("Reading POLY file!");
		inp_ln = inp_stm.ReadLine( );	//Read first line
		words = inp_ln.Split('\t');
		int.TryParse(words[0], out nrVert);
		//int.TryParse(words[1], out dim);
		int.TryParse(words[2], out nrAttr);
		int.TryParse(words[3], out nrBM);

		vertices = new Vector3[nrVert];

		for (int i = 0;i<nrVert;++i) {
			inp_ln = inp_stm.ReadLine( );	//Read vertex line
			words = inp_ln.Split('\t');
			float.TryParse(words[0], out vertices[i].x);
			float.TryParse(words[1], out vertices[i].y);
			float.TryParse(words[2], out vertices[i].z);
			float attr, bm;	//Dummy placeholders for now
			for (int k = 3;k<3+nrAttr;++k) {
				float.TryParse(words[k], out attr);
			}
			for (int k = 3+nrAttr;k<3+nrAttr+nrBM;++k) {
				float.TryParse(words[k], out bm);
			}
		}
		if (nrVert == 0) {
			sepNodeFile = true;	//Separate node file exists
		}






		//TODO below

		while(!inp_stm.EndOfStream) {
			++lc;
			inp_ln = inp_stm.ReadLine( );	//Read line

			//Split line at tabs
			words = inp_ln.Split('\t');

			if (inp_ln[0] == '#') {
				Debug.Log("Read a comment!");
			}
			Debug.Log("Line #" + lc);
			Debug.Log(inp_ln + "\n");
		}
		inp_stm.Close( );

		//Read node file (if necessary)
		if (sepNodeFile) {
			inp_stm = new StreamReader(dir_path + "a.1.node");
			Debug.Log("Reading NODE file!");
			while (!inp_stm.EndOfStream) {
				inp_ln = inp_stm.ReadLine( );	//Read line

				//Split line at tabs
				words = inp_ln.Split('\t');
				
				if (inp_ln[0] == '#') {
					Debug.Log("Read a comment!");
				}
				Debug.Log("Line #" + lc);
				Debug.Log(inp_ln + "\n");
			}
			inp_stm.Close( );
		}

		//Read ele file (triangles)
		inp_stm = new StreamReader(dir_path + "a.1.ele");
		Debug.Log("Reading ELE file!");
		first_line = inp_stm.ReadLine ();
		string[] meta = lineOfText.Split(first_line, StringSplitOptions.RemoveEmptyEntries);
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
			line = inp_stm.ReadLine( );
			string[] tokens = lineOfText.Split(line, StringSplitOptions.RemoveEmptyEntries);
			triArray.Add(Convert.ToInt32(tokens[1]));
			triArray.Add(Convert.ToInt32(tokens[2]));
			triArray.Add(Convert.ToInt32(tokens[3]));
		}

		triangles = (int[])triArray.ToArray (typeof(int));

		/*
		while (!inp_stm.EndOfStream) {
			inp_ln = inp_stm.ReadLine( );	//Read line


			//Split line at tabs
			words = inp_ln.Split('\t');
			
			if (inp_ln[0] == '#') {
				Debug.Log("Read a comment!");
			}
			Debug.Log("Line #" + lc);
			Debug.Log(inp_ln + "\n");
		}*/
		inp_stm.Close( );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
