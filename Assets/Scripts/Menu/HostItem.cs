using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostItem : MonoBehaviour
{
    [SerializeField] public Text HostAddressText;
    [SerializeField] public Text HostNameText;
    //[SerializeField] MainMenuScript MainMenu;
    
    public void OnConnectBtnClick()
    {
        FindObjectOfType<MainMenuScript>().ConnectToGame(HostAddressText.text);
    }
}
