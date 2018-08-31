using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

	private Button _connectButton;
	private Button _toggleButton;
	
	private InputField _targetIp;
	private InputField _targetPort;
	private InputField _hostingPort;
	
	private GameObject _clientCanvas;
	private GameObject _serverCanvas;
	private GameObject _gameManager;
	
	// Use this for initialization
	private void Start ()
	{
		_clientCanvas = transform.Find("ClientInput").gameObject;
		_serverCanvas = transform.Find("ServerInput").gameObject;
		
		_toggleButton = transform.Find("ClientServerButton").gameObject.GetComponent<Button>();
		_connectButton = transform.Find("ConnectButton").gameObject.GetComponent<Button>();
		
		_targetIp = transform.Find("ClientInput/TargetIPInput").gameObject.GetComponent<InputField>();
		_targetPort = transform.Find("ClientInput/TargetPortInput").gameObject.GetComponent<InputField>();
		_hostingPort = transform.Find("ServerInput/HostPortInput").gameObject.GetComponent<InputField>();
		
		_gameManager = GameObject.Find("GameManager");
		
		_toggleButton.onClick.AddListener(HandleToggleCanvas);;
		_connectButton.onClick.AddListener(HandleConnect);
	}
	
	private void HandleToggleCanvas()
	{
		_clientCanvas.SetActive(!_clientCanvas.activeInHierarchy);
		_serverCanvas.SetActive(!_serverCanvas.activeInHierarchy);
		_toggleButton.GetComponentInChildren<Text>().text = _serverCanvas.activeInHierarchy ? "Act as client" : "Act as server";
	}
	
	private void HandleConnect()
	{
		if(_hostingPort.isActiveAndEnabled)
		{
			var messageContent = new object[1];
			messageContent[0] = Int32.Parse(_hostingPort.text);
			_gameManager.SendMessage("InitializeServer", messageContent);
		}
		else
		{
			var messageContent = new object[2];
			messageContent[0] = _targetIp.text;
			messageContent[1] = Int32.Parse(_targetPort.text);
			_gameManager.SendMessage("InitializeClient", messageContent);
		}

	}
}
