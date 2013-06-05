/*
 * Shamessly based on Simple TCP/IP Client DLL Code
 * http://wiki.unity3d.com/index.php/Simple_TCP/IP_Client_DLL_Code
 */
 
//using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
 
namespace SharpConnect
{
	public interface IDataReceivedCallback {
		void DataReceived(string data);
	}
	
	public class Connector
	{
		const int READ_BUFFER_SIZE = 255;
		const int PORT_NUM = 10000;
		private TcpClient client;
		private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
		private IDataReceivedCallback cb = null;
		
		public string strMessage=string.Empty;
		public string res=String.Empty;
 
		public Connector(){}
 
		public string fnConnectResult (string sNetIP, int iPORT_NUM, IDataReceivedCallback callback)
		{
			try {
				client = new TcpClient (sNetIP, iPORT_NUM);
				cb = callback;
				client.GetStream ().BeginRead (readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback (DoRead), null);
				return "Connection Succeeded";
			} catch (Exception ex) {
				return "Server is not active.  Please start server and try again." + ex.ToString ();
			}
		}
		
		private void DoRead (IAsyncResult ar)
		{ 
			int bytesRead;
			try {
				// Finish asynchronous read into readBuffer and return number of bytes read.
				bytesRead = client.GetStream ().EndRead (ar);
				//Debug.Log ("Bytes read: " + bytesRead);
				if (bytesRead < 1) {
					// if no bytes were read server has close.  
					res = "Disconnected";
					return;
				}
				// Convert the byte array the message was saved into, minus two for the
				// Chr(13) and Chr(10)
				strMessage = Encoding.ASCII.GetString (readBuffer, 0, bytesRead);
				//Debug.Log("Message: " + strMessage);
				
				if (cb != null) {
					cb.DataReceived (strMessage);
				}
				
				client.GetStream ().BeginRead (readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback (DoRead), null);
			} catch {
				res = "Disconnected";
			}
		}

 
		// Use a StreamWriter to send a message to server.
		private void SendData (string data)
		{
			StreamWriter writer = new StreamWriter (client.GetStream ());
			writer.Write (data + (char)13);
			writer.Flush ();
		}
		
		public bool isConnected ()
		{
			return client != null && client.Connected;
		}
		
		public void disconnect(){
			client.Close();
		}
		
 	}
}
