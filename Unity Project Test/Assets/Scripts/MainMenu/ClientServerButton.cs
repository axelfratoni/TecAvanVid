using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientServerButton : MonoBehaviour {

    private Button toggleButton;

    public GameObject clientCanvas;
    public GameObject serverCanvas;

    private void Start()
    {
        toggleButton = GetComponent<Button>();
        toggleButton.onClick.AddListener(HandleToggleCanvas);
    }

    private void HandleToggleCanvas()
    {
        clientCanvas.SetActive(!clientCanvas.activeInHierarchy);
        serverCanvas.SetActive(!serverCanvas.activeInHierarchy);
        toggleButton.GetComponentInChildren<Text>().text = serverCanvas.activeInHierarchy ? "Act as client" : "Act as server";
    }
}
