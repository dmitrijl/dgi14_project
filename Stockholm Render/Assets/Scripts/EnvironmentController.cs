﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class EnvironmentController : MonoBehaviour {

	public GameObject water, plane;
	public bool colorize;
	public bool flattenBuildings;
	public bool flattenAll;

	public enum BuildingDetection{None, AbsoluteHeight, NeighborHeight};
	public BuildingDetection buildingDetection;
	public float heightLimit;
	public bool spikeRemoval;
	public bool heightMapMode;

	public readonly float updateSpeed = 2f;
	public readonly float shiftSpeedIncreaseFactor = 4;

	private List<int>[] nbList;
	private Mesh mesh;
	private Vector3[] vertices;
	private Vector2[] UV;
	private int[] triangles;
	private int[] originalClassifications;
	private int[] classifications;
	private float[] orignialHeights;
	private Color32[] colors;


	//Converts the triangles into edge lists (each triangle consisting of three edges)
	void trianglesToEdges(int nrVert, ref int[] t, out List<int>[] nbl) {
		//Convert triangles to edges. Each triangle is three edges
		nbl = new List<int>[nrVert];
		for (int i = 0;i<nrVert;++i) {
			nbl[i] = new List<int>();
		}
		int n1, n2;
		for (int i = 0;i<t.Length;i+=3) {
			for (int j = 0;j<3;++j) {
				for (int k = 0;k<3;++k) {
					if (j == k) continue;
					n1 = t[i+j];
					n2 = t[i+k];
					nbl[n1].Add(n2);
				}
			}
		}
	}
	
	//Lower the y value of all water classified nodes
	void lowerWater(ref Vector3[] v, ref int[] c) {
		for (int i = 0;i<v.Length;++i) {
			if (c[i] == MeshReader.Classification.Water) {
				v[i].y = -1.0f;
			}
		}
	}

	//Set the hgieht of all vertices to zero
	void flattenEverything(ref Vector3[] v) {
		for (int i = 0;i<v.Length;++i) {
			v[i].y = 0.0f;
		}
	}
	
	//Remove big spikes
	void removeSpikes(ref Vector3[] v, ref List<int>[] nbLists, float hl, ref int[] c) {
		for (int i = 0;i<v.Length;++i) {
			//if (v[i].y > 100.0f) Debug.Log("Vertex " + i + " has height " + v[i].y);
		}

		int count = 0;
		float greatestChange = -1f;
		for (int n1 = 0;n1<nbLists.Length;++n1) {
			for (int j = 0;j<nbLists[n1].Count;++j) {
				int n2 = nbLists[n1][j];
				if (v[n1].y > hl) {
					greatestChange = Math.Max (greatestChange, Math.Abs(v[n1].y - v[n2].y));
					v[n1].y = v[n2].y;
					++count;
				}
			}
		}
		//Debug.Log("Number of changes: " + count + ", greatest change: " + greatestChange);
	}
	
	//Detects building using the neighbor height method
	void detectBuildingsNH(ref Vector3[] v, ref List<int>[] nbLists, float hl, ref int[] c) {
		//Look for great height differences between neighboring vertices
		int n1, n2;
		float avgH;
		int nrDiffs;
		int changes;
		for (int i = 0;i<1;++i) {
			changes = 0;
			for (n1 = 0; n1<nbLists.Length; ++n1) {
				avgH = 0.0f;
				nrDiffs = 0;
				for (int j = 0; j<nbLists[n1].Count; ++j) {
					n2 = nbLists[n1][j];
					
					if (v[n1].y - v[n2].y > hl && (c[n2] != MeshReader.Classification.Water || 
					                                       c[n2] != MeshReader.Classification.Unclassified) && 
					                                      (c[n1] == MeshReader.Classification.Building || 
					                                       c[n1] == MeshReader.Classification.Ground) ) {
						avgH += v[n2].y;
						++nrDiffs;
					}
				}
				if (avgH > 0.0f) {
					avgH /= nrDiffs;
					v[n1].y = avgH;
					c[n1] = MeshReader.Classification.Building;
					++changes;
				}
			}
			//Debug.Log("Iteration " + (i+1) + ": Number of changes: " + changes);
		}
				
	}
	
	//Detects building using the absolute height method
	void detectBuildingsAH(ref Vector3[] v, float hl, ref int[] c) {
		for (int j = 0; j < v.Length; ++j) {
			if (v[j].y > hl) {
				c[j] = MeshReader.Classification.Building;	//High point
				v[j].y = hl;
			}
		}
	}


	//Update the mesh
	void updateMesh() {
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int i=0; i < uvs.Length; i++) {
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		}
		mesh.uv = uvs;

		//Color the vertices
		if (colorize) {
			if (heightMapMode == true) {

				for (int j = 0; j < vertices.Length; ++j) {
					if (classifications[j] == MeshReader.Classification.Building) {
						byte y = (byte)Math.Min((int)mesh.vertices[j].y, 255);
						//byte y = (byte)mesh.vertices[j].y;

						if (y == 255) {
							//Debug.Log("byte y is 255, float value was " + mesh.vertices[j].y);
						}

						//colors[j] = (Color32)Color.cyan;
						colors[j] = new Color32(y, y, y, 255);
					} else {
						colors[j] = (Color32)Color.black;
					}
				}
			} else {
				for (int j = 0; j < vertices.Length; ++j) {
					colors [j] = MeshReader.Classification.intToColor32 (classifications [j]);
				}
			}
		} else {
			for (int j = 0; j < vertices.Length; ++j) {
				colors [j] = (Color32)Color.white;
			}
		}
		mesh.colors32 = colors;

		if (flattenAll) {
			flattenEverything(ref vertices);
			mesh.vertices = vertices;
		}
	}


	//Process the model, apply building detection, etc...
	void processModel(BuildingDetection bd) {
		//Clone orignialHeights and classifications first
		for (int i = 0;i<vertices.Length;++i) {
			vertices[i].y = orignialHeights[i];
			classifications[i] = originalClassifications[i];
		}

		//Select algorithm
		if (buildingDetection == BuildingDetection.NeighborHeight) {
			detectBuildingsNH(ref vertices, ref nbList, heightLimit, ref classifications);
		} else if (buildingDetection == BuildingDetection.AbsoluteHeight) {
			detectBuildingsAH(ref vertices, heightLimit, ref classifications);
		}

		//Restore originalHeights or use the new ones
		float diff = 0.0f;
		int nrDiffs = 0;
		if (!flattenBuildings) {
			for (int i = 0;i<orignialHeights.Length;++i) {
				if (vertices[i].y != orignialHeights[i]) {
					diff += Math.Abs(vertices[i].y - orignialHeights[i]);
					++nrDiffs;
				}
				vertices[i].y = orignialHeights[i];
			}
		}
		//Debug.Log("Average diff: " + diff/nrDiffs);

		if (spikeRemoval) {
			removeSpikes (ref vertices, ref nbList, 50.0f, ref classifications);
		}
	}


	// Use this for initialization
	void Start () {

		mesh = plane.GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		UV = mesh.uv;
		triangles = mesh.triangles;
		colors = mesh.colors32;
		classifications = new int[vertices.Length];
		originalClassifications = new int[classifications.Length];
		orignialHeights = new float[vertices.Length];

		//Extract classifications information from colors
		for (int i = 0; i < vertices.Length; ++i) {
			originalClassifications[i] = MeshReader.Classification.color32ToInt((Color32)colors[i]);
			orignialHeights[i] = vertices[i].y;
		}

		//Create an edge list for building and spike detection and maybe other things
		trianglesToEdges(vertices.Length, ref triangles, out nbList);
		processModel(buildingDetection);


		//Apply changes
		//mesh = new Mesh();
		//plane.GetComponent<MeshFilter>().mesh = mesh;
		updateMesh();

		Application.CaptureScreenshot("building_heightmap.png");
	}
	

	// Update is called once per frame
	void Update () {
		return;

		float us = updateSpeed;
		if (Input.GetKey (KeyCode.LeftShift) == true) {
			us *= shiftSpeedIncreaseFactor;
		}
		float uc = us * Time.deltaTime;

		bool needProcessing = false;

		if(Input.GetKeyDown (KeyCode.Alpha1)) {
			//Toggle static water
			water.SetActive(!water.activeSelf);
		}

		if(Input.GetKey (KeyCode.Alpha2)) {
			//Decrease the water level
			water.transform.Translate(Vector3.down*uc);
		}

		if(Input.GetKey (KeyCode.Alpha3)) {
			//Increase the water level
			water.transform.Translate(Vector3.up*uc);
		}

		if(Input.GetKeyDown (KeyCode.Alpha4)) {
			//Toggle building flattening
			flattenBuildings = !flattenBuildings;
			needProcessing = true;
		}
		
		if(Input.GetKeyDown (KeyCode.Alpha5)) {
			//Use no building detection
			buildingDetection = BuildingDetection.None;
			needProcessing = true;
		}
		
		if(Input.GetKeyDown (KeyCode.Alpha6)) {
			//Use absolute height building detection
			buildingDetection = BuildingDetection.AbsoluteHeight;
			needProcessing = true;
		}

		if(Input.GetKeyDown (KeyCode.Alpha7)) {
			//Use neighbor height building detection
			buildingDetection = BuildingDetection.NeighborHeight;
			needProcessing = true;
		}
		
		if(Input.GetKey (KeyCode.Alpha8)) {
			//Decrease height limit for building detection
			heightLimit -= uc;
			needProcessing = true;
		}
		
		if(Input.GetKey (KeyCode.Alpha9)) {
			//Increase height limit for building detection
			heightLimit += uc;
			needProcessing = true;
		}

		if(Input.GetKeyDown (KeyCode.Alpha0)) {
			//Toggle spike removal
			spikeRemoval = !spikeRemoval;
			needProcessing = true;
		}

		if(Input.GetKeyDown (KeyCode.H)) {
			//Toggle heightmap mode
			heightMapMode = !heightMapMode;
			needProcessing = true;
		}

		if (needProcessing) {
			processModel (buildingDetection);
		}

		if(Input.GetKey (KeyCode.Escape)) {
			//Exit program
			Application.Quit();
		}

		updateMesh();
	}
}
