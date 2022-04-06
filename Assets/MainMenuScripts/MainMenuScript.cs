using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Web;
using System.Net;
using System.Net.Sockets;

public class MainMenuScript : MonoBehaviour
{
    public InputField seed_field;
    public Text Address_text;
    public InputField host_addr_field;

    private void Awake()
    {
        ShowMenu("Main");
    }

    private void ShowMenu(string name)
    {
        var menus = Resources.FindObjectsOfTypeAll<Menu>();
        for(int i = 0; i < menus.Length; i++)
        {
            if (menus[i].Name.Equals(name))
                menus[i].gameObject.SetActive(true);
            else
                menus[i].gameObject.SetActive(false);
        }
    }

    public void StartBtnPressed()
    {
        System.Random rnd = new System.Random();
        GlobalVariables.Seed = rnd.Next(1000);
        var creating_menu = Resources.FindObjectsOfTypeAll<Menu>()[0];
        seed_field.text = GlobalVariables.Seed.ToString();

        IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress host_addr = entry.AddressList[0];
        Address_text.text = "Your address: " + host_addr.ToString();

        ShowMenu("Creation");
    }

    public void BackToMain()
    {
        ShowMenu("Main");
    }

    public void ExitBtnPressed()
    {
        Application.Quit();
    }

    public void CreateGame()
    {
        try
        {
            GlobalVariables.Seed = System.Convert.ToInt32(seed_field.text);
        }
        catch
        {
            GlobalVariables.Seed = new System.Random().Next(1000);
        }

        IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress host_addr = entry.AddressList[0];
        TcpListener listener = new TcpListener(host_addr, GlobalVariables.Port);
        listener.Start();
        print("Waiting connection");
        TcpClient cli = listener.AcceptTcpClient();
        NetworkStream stream = cli.GetStream();
        byte[] query = System.BitConverter.GetBytes(GlobalVariables.Seed);
        if (System.BitConverter.IsLittleEndian)
            System.Array.Reverse(query);
        //byte[] query = new byte[] { (byte)GlobalVariables.Seed };
        stream.Write(query, 0, query.Length);
        listener.Stop();
        SceneManager.LoadScene("SampleScene");
    }

    public void ConnectBtnPressed()
    {
        ShowMenu("Connecting");
    }

    public void ConnectToGame()
    {
        string addr = host_addr_field.text;

        TcpClient client = new TcpClient();
        IPHostEntry entry = Dns.GetHostEntry(addr);
        IPAddress host_addr = entry.AddressList[0];
        print("Connecting...");
        client.Connect(host_addr, GlobalVariables.Port);
        NetworkStream stream = client.GetStream();
        byte[] resp = new byte[4];
        stream.Read(resp, 0, 4);
        GlobalVariables.Seed = System.Convert.ToInt32(resp);
        print($"Readed seed {GlobalVariables.Seed}!");

        SceneManager.LoadScene("SampleScene");
    }

    public void EnterDevelopMode()
    {
        GlobalVariables.Seed = new System.Random().Next(1000);
        SceneManager.LoadScene("SampleScene");
    }
}
