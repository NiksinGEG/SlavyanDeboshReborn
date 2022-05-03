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

    public Text Client_output;
    public Text Host_output;

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
        GlobalVariables.Seed = rnd.Next(3000000);
        var creating_menu = Resources.FindObjectsOfTypeAll<Menu>()[0];
        seed_field.text = GlobalVariables.Seed.ToString();

        //IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
        //IPAddress host_addr;// = entry.AddressList[0];

        Address_text.text = "Your address: ?";// + GlobalVariables.HostAddress.ToString();

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
            GlobalVariables.Seed = new System.Random().Next(3000000);
        }
        Host_output.text = $"Genered seed {GlobalVariables.Seed}...";
        //IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
        //IPAddress host_addr = IPAddress.Parse("127.0.0.1");//entry.AddressList[0];
        TcpListener listener = new TcpListener(IPAddress.Any, GlobalVariables.Port);
        listener.Start();
        TcpClient cli = listener.AcceptTcpClient();
        NetworkStream stream = cli.GetStream();
        byte[] query = System.BitConverter.GetBytes(GlobalVariables.Seed);
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
        //IPHostEntry entry = Dns.GetHostEntry(addr);
        //IPAddress host_addr = entry.AddressList[0];
        Client_output.text = "Connecting...";
        //client.Connect(host_addr, GlobalVariables.Port);
        client.Connect(addr, GlobalVariables.Port);
        Client_output.text = "Connected! Reading seed...";
        NetworkStream stream = client.GetStream();
        Client_output.text = "Got stream...";
        byte[] resp = new byte[sizeof(int)];
        stream.Read(resp, 0, sizeof(int));
        Client_output.text = $"Recieved response...";
        GlobalVariables.Seed = System.BitConverter.ToInt32(resp, 0);

        Client_output.text = $"Readed seed {GlobalVariables.Seed}!";

        SceneManager.LoadScene("SampleScene");
    }

    public void EnterDevelopMode()
    {
        GlobalVariables.Seed = new System.Random().Next(1000);
        SceneManager.LoadScene("SampleScene");
    }
<<<<<<< Updated upstream
=======

    public void SwitchTropicTreeProcentFieldValue()
    {
        tropicTreeProcentField.text = tropicTreeProcentSlider.value.ToString();
        GlobalVariables.generationSettings.tropicTreeProcent = Convert.ToInt32(tropicTreeProcentField.text);
    }

    public void SwitchTropicTreeProcentSliderValue()
    {
        tropicTreeProcentSlider.value = Convert.ToInt32(tropicTreeProcentField.text);
    }

    public void SwitchStandartTreeProcentFieldValue()
    {
        standartTreeProcentField.text = standartTreeProcentSlider.value.ToString();
        GlobalVariables.generationSettings.standartTreeProcent = Convert.ToInt32(standartTreeProcentField.text);
    }

    public void SwitchStandartTreeProcentSliderValue()
    {
        standartTreeProcentSlider.value = Convert.ToInt32(standartTreeProcentField.text);
    }

    public void SwitchWinterTreeProcentFieldValue()
    {
        winterTreeProcentField.text = winterTreeProcentSlider.value.ToString();
        GlobalVariables.generationSettings.winterTreeProcent = Convert.ToInt32(winterTreeProcentField.text);
    }

    public void SwitchWinterTreeProcentSliderValue()
    {
        winterTreeProcentSlider.value = Convert.ToInt32(winterTreeProcentField.text);
    }

    public void SwitchXField()
    {
        GlobalVariables.generationSettings.terrainChunkCountX = Convert.ToInt32(chunkCountXField.text);
    }

    public void SwitchYField()
    {
        GlobalVariables.generationSettings.terrainChunkCountY = Convert.ToInt32(chunkCountYField.text);
    }

    public void SwitchRockProcentFieldValue()
    {
        rockProcentField.text = rockProcentSlider.value.ToString();
        GlobalVariables.generationSettings.rockProcent = Convert.ToInt32(rockProcentField.text);
    }

    public void SwitchRockProcentSliderValue()
    {
        try
        {
            rockProcentSlider.value = Convert.ToInt32(rockProcentField.text);
        }
        catch
        {
            rockProcentSlider.value = Convert.ToInt32(rockProcentField.text = "20");
        }
    }

    public void SwitchMainlandsCountFieldValue()
    {
        mainlandsCountField.text = mainlandsCountSlider.value.ToString();
        GlobalVariables.generationSettings.mainlandsCount = Convert.ToInt32(mainlandsCountField.text);
    }

    public void SwitchMainlandsCountSliderValue()
    {
        try
        {
            mainlandsCountSlider.value = Convert.ToInt32(mainlandsCountField.text);
        }
        catch
        {
            mainlandsCountSlider.value = Convert.ToInt32(mainlandsCountField.text = "3");
        }
    }

    public void SwitchMixingBiomesFieldValue()
    {
        mixingBiomesField.text = mixingBiomesSlider.value.ToString();
    }

    public void SwitchMixingBiomesSliderValue()
    {
        try
        {
            mixingBiomesSlider.value = Convert.ToInt32(mixingBiomesField.text);
        }
        catch
        {
            mixingBiomesSlider.value = Convert.ToInt32(mixingBiomesField.text = "7");
        }
    }

    public void SwitchSeedFieldValue()
    {
        GlobalVariables.Seed = Convert.ToInt32(seedField.text);
        UnityEngine.Random.InitState(GlobalVariables.Seed);
    }

    public void GenerateSeedOnClick()
    {
        GlobalVariables.Seed = new System.Random().Next(3000000);
        seedField.text = GlobalVariables.Seed.ToString();
    }

    public void OpenGenerationMenu()
    {
        ShowMenu("Generation");
        GlobalVariables.generationSettings.terrainChunkCountX = 35;
        GlobalVariables.generationSettings.terrainChunkCountY = 21;

        GlobalVariables.generationSettings.tropicTreeProcent = 50;
        GlobalVariables.generationSettings.standartTreeProcent = 50;
        GlobalVariables.generationSettings.winterTreeProcent = 50;
        GlobalVariables.generationSettings.rockProcent = 20;
        GlobalVariables.generationSettings.mainlandsCount = 3;
        GlobalVariables.generationSettings.mixingBiomesCount = 7;

        GlobalVariables.Seed = new System.Random().Next(3000000);
        seedField.text = GlobalVariables.Seed.ToString();
    }
>>>>>>> Stashed changes
}
