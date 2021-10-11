using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class DebugLogBroadcaster : MonoBehaviour 
{
	public int broadcastPort = 9999;

	IPEndPoint remoteEndPoint;
	UdpClient client;

	void OnEnable() 
	{
		remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
		client = new UdpClient();
		Application.logMessageReceived += HandlelogMessageReceived;
	}

	void OnDisable() 
	{
		Application.logMessageReceived -= HandlelogMessageReceived;
		client.Close();
		remoteEndPoint = null;
	}

	void HandlelogMessageReceived (string condition, string stackTrace, LogType type)
	{
		string s = stackTrace.Replace ("\n", "\n  ");

		string msg = string.Format ("[{0}] {1}{2}", 
		                           type.ToString ().ToUpper (), 
		                           condition,
		                           "\n    " + stackTrace.Replace ("\n", "\n    "));
		byte[] data = Encoding.UTF8.GetBytes(msg);
		client.Send(data, data.Length, remoteEndPoint);
	}
}
