using UnityEngine;
using System.Collections;


public class HUD : MonoBehaviour {
	
	public bool visible;
	public GameObject water;
	public GameObject envCtrl;
	
	private readonly string[] hudText = {
		"L: Show/hide HUD/legend",
		"1: Show/hide water",
		"2: Decrease water level",
		"3: Increase water level",
		"4: Toggle building flattening",
		"5: Use no building detection",
		"6: Use absolute height building detection",
		"7: Use neighbor height building detection",
		"8: Decrease height limit (for building detection)",
		"9: Increase height limit (for building detection)",
		"W: Move forward",
		"S: Move backward",
		"A: Move rightward",
		"D: Move leftward",
		"Q: Move downward",
		"E: Move upward",
		"Z: Rotate CCW",
		"C: Rotate CW",
		"X: Rotate forward-upward",
		"Press and hold the left mouse button",
		"and move the mouse to look around.",
		"Hold left shift to move/update faster."
	};

	private readonly string waterLevelText = "Water level: ";
	private readonly string heightLimitText = "Height limit: ";
	private readonly string buildingDetectionText = "Building detection method: ";
	private readonly string isFlattenedText = "Flatten buildings: ";
	private readonly string toggleSpikesText = "Spike removal: ";
	
	// Use this for initialization
	void Start () {

	}
	
	void OnGUI() {
		if (visible) {
			string labelText = "";
			for (int i = 0;i<hudText.Length;++i) {
				labelText += hudText[i] + '\n';
			}

			int labelW = 240;
			int labelH = 500;

			GUI.skin.label.normal.textColor = Color.black;
			GUI.Label( new Rect(6, 4, labelW, labelH), labelText);
			GUI.Label( new Rect(7, 3, labelW, labelH), labelText);
			GUI.Label( new Rect(7, 4, labelW, labelH), labelText);
			
			GUI.skin.label.normal.textColor = Color.white;
			GUI.Label( new Rect(6, 3, labelW, labelH), labelText);



			labelW = 260;
			labelH = 100;

			float waterLevel = water.transform.position.y;
			float heightLimit = envCtrl.GetComponent<EnvironmentController>().heightLimit;
			string buildingDetection = envCtrl.GetComponent<EnvironmentController>().buildingDetection.ToString();
			bool flatten = envCtrl.GetComponent<EnvironmentController>().flattenBuildings;
			bool removeSpikes = envCtrl.GetComponent<EnvironmentController>().spikeRemoval;

			labelText = waterLevelText + waterLevel + '\n' + 
						heightLimitText + heightLimit + '\n' +
						buildingDetectionText + buildingDetection + '\n' +
						isFlattenedText + (flatten ? "on" : "off") + '\n' +
						toggleSpikesText + (removeSpikes ? "on" : "off");

			GUI.skin.label.normal.textColor = Color.black;
			GUI.Label( new Rect(Screen.width-3-labelW, 4, labelW, labelH), labelText);
			GUI.Label( new Rect(Screen.width-4-labelW, 3, labelW, labelH), labelText);
			GUI.Label( new Rect(Screen.width-4-labelW, 4, labelW, labelH), labelText);
			
			GUI.skin.label.normal.textColor = Color.white;
			GUI.Label( new Rect(Screen.width-3-labelW, 3, labelW, labelH), labelText);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.L)) {
			//Toggle Legend visibility
			visible = !visible;
		}
	}
}
