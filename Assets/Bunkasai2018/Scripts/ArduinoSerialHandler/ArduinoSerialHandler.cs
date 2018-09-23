using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

namespace Bunkasai2018 {
	public class ArduinoSerialHandler : MonoBehaviour {

		// Use this for initialization
		public delegate void SerialDataReceivedEventHandler (string message);
		public event SerialDataReceivedEventHandler OnDataReceived;

		//ポート名
		//例
		//Linuxでは/dev/ttyUSB0
		//windowsではCOM1
		//Macでは/dev/tty.usbmodem1421など
		public string portName = "/dev/tty.usbmodem1421";
		public int baudRate = 9600;

		private SerialPort serialPort_;
		private Thread thread_;
		private bool isRunning_ = false;

		private string message_;
		private bool isNewMessageReceived_ = false;

		void Awake () {
			if (!SettingsManager.sideABool) {
				Open ();
			}
		}

		void Update () {
			if (!SettingsManager.sideABool) {
				if (isNewMessageReceived_) {
					OnDataReceived (message_);
				}
				isNewMessageReceived_ = false;
			}
		}

		void OnDestroy () {
			Close ();
		}

		private void Open () {
			//serialPort_ = new SerialPort (portName, baudRate, Parity.None, 8, StopBits.One);
			//または
			serialPort_ = new SerialPort (portName, baudRate);
			serialPort_.Open ();

			isRunning_ = true;

			thread_ = new Thread (Read);
			thread_.Start ();
		}

		private void Close () {
			isNewMessageReceived_ = false;
			isRunning_ = false;

			if (thread_ != null && thread_.IsAlive) {
				thread_.Join ();
			}

			if (serialPort_ != null && serialPort_.IsOpen) {
				serialPort_.Close ();
				serialPort_.Dispose ();
			}
		}

		private void Read () {
			while (isRunning_ && serialPort_ != null && serialPort_.IsOpen) {
				try {
					message_ = serialPort_.ReadLine ();
					isNewMessageReceived_ = true;
				} catch (System.Exception e) {
					Debug.LogWarning (e.Message);
				}
			}
		}

		public void Write (string message) {
			try {
				serialPort_.Write (message);
			} catch (System.Exception e) {
				Debug.LogWarning (e.Message);
			}
		}
	}
}