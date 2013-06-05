using UnityEngine;
using System;
using SharpConnect;

public class TestWiiMote : MonoBehaviour, IDataReceivedCallback
{
 
	private SharpConnect.Connector conn = null;
	
 	public string connectionIP = "192.168.2.5";
	public int connectionPort = 25001;
	
	private float pitch = 0.0f;
	private float roll = 0.0f;
	
	void Start ()
	{
		conn = new Connector ();		
	}
	
	void Update ()
	{
		Vector3 newRotation = transform.localRotation.eulerAngles;
		newRotation.z = roll * 30.0f;
		newRotation.x = -pitch * 30.0f;
		transform.localRotation = Quaternion.Euler(newRotation);
	}
	
	void OnGUI ()
	{
		GUI.Label (new Rect (10, 50, 100, 20), "Pitch: " + pitch);
		GUI.Label (new Rect (10, 70, 100, 20), "Roll: " + roll);
		if (!conn.isConnected ()) {
			GUI.Label (new Rect (10, 10, 200, 20), "Status: Disconnected");
			if (GUI.Button (new Rect (10, 30, 120, 20), "Client Connect")) {
				conn.fnConnectResult (connectionIP, connectionPort, this);
			}
		} else {
			GUI.Label (new Rect (10, 10, 300, 20), "Status: Connected");
			if (GUI.Button (new Rect (10, 30, 120, 20), "Disconnect")) {
				conn.disconnect ();
			}
		}
	}
	
	public void DataReceived (string data)
	{
		if (data.StartsWith ("WIIMOTE")) {
			string[] coords = data.Split (':');
			pitch = float.Parse (coords [1]);
			pitch = Math.Max (pitch, -1.4f);
			pitch = Math.Min (pitch, 1.4f);
			
			roll = float.Parse (coords [2]);
		} else {
			Debug.Log ("Unknown command: " + data);
		}
		
		
	}
}
