using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Logger;

public class MainMenuScript : MonoBehaviour
{
    public InputField chunkCountXField;
    public InputField chunkCountYField;

    public Slider tropicTreeProcentSlider;
    public Slider standartTreeProcentSlider;
    public Slider winterTreeProcentSlider;

    public InputField tropicTreeProcentField;
    public InputField standartTreeProcentField;
    public InputField winterTreeProcentField;

    public Slider rockProcentSlider;
    public InputField rockProcentField;

    public Slider mainlandsCountSlider;
    public InputField mainlandsCountField;

    public Slider mixingBiomesSlider;
    public InputField mixingBiomesField;

    public InputField seedField;
    public Button seedGenerateBtn;

    public InputField seed_field;
    public Text Address_text;

    public Text Client_output;
    public Text Host_output;

    public InputField hostNameField;

    public HostList Host_List;

    List<Tuple<string, string>> Hosts = new List<Tuple<string, string>>();

    bool connected = false;

    public MenuManager menuManager;

    private void Awake()
    {
        menuManager.ShowMenu("Main");
    }

    public void Update()
    {
        try
        {
            foreach (var tupl in Hosts)
            {
                if (!Host_List.HasHost(tupl.Item2))
                    Host_List.Add(tupl.Item1, tupl.Item2);
            }
        }
        catch { }
    }

    public void StartBtnPressed()
    {
        System.Random rnd = new System.Random();
        GlobalVariables.generationSettings.Seed = rnd.Next(3000000);

        menuManager.ShowMenu("Creation");
    }

    public void BackToMain()
    {
        menuManager.ShowMenu("Main");
    }

    public void ExitBtnPressed()
    {
        Application.Quit();
    }

    public void CreateGame()
    {
        GlobalVariables.SelfName = hostNameField.text;

        Host_output.text = $"Genered seed {GlobalVariables.generationSettings.Seed}...";
        Debug.Log($"Genered seed {GlobalVariables.generationSettings.Seed}...");
        Broadcasting();
        TcpListener listener = new TcpListener(IPAddress.Any, GlobalVariables.Port);
        listener.Start();
        Debug.Log("Waiting client...");
        TcpClient cli = listener.AcceptTcpClient();
        connected = true;
        Debug.Log("Connected...");
        GlobalVariables.NetStream = cli.GetStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(GlobalVariables.NetStream, GlobalVariables.generationSettings);
        Debug.Log("Sended...");
        listener.Stop();
        SceneManager.LoadScene("SampleScene");
    }

    public void ConnectBtnPressed()
    {
        menuManager.ShowMenu("Connecting");
        ListenHostsAsync();
    }

    public void ConnectToGame(string addr)
    {
        TcpClient client = new TcpClient();
        Client_output.text = "Connecting...";
        Debug.Log("Connecting...");
        client.Connect(addr, GlobalVariables.Port);
        Debug.Log("Connected...");
        connected = true;
        Client_output.text = "Connected! Reading seed...";
        GlobalVariables.NetStream = client.GetStream();
        Client_output.text = "Got stream...";
        Debug.Log("Got stream...");
        var formatter = new BinaryFormatter();
        GlobalVariables.generationSettings = (GenerationSettings)formatter.Deserialize(GlobalVariables.NetStream);
        Client_output.text = $"Recieved response...";
        Debug.Log($"Got seed: {GlobalVariables.generationSettings.Seed}");

        Client_output.text = $"Readed config!";

        EnterGame();
    }

    public void EnterGame()
    {
        UnityEngine.Random.InitState(GlobalVariables.generationSettings.Seed);
        Loger.Log("Logs\\MainMenu.log", "Entering game...");
        SceneManager.LoadScene("SampleScene");
    }

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
            rockProcentSlider.value = Convert.ToInt32(rockProcentField.text = "5");
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
            mixingBiomesSlider.value = Convert.ToInt32(mixingBiomesField.text = "3");
        }
    }

    public void SwitchSeedFieldValue()
    {
        GlobalVariables.generationSettings.Seed = Convert.ToInt32(seedField.text);
        UnityEngine.Random.InitState(GlobalVariables.generationSettings.Seed);
    }

    public void GenerateSeedOnClick()
    {
        GlobalVariables.generationSettings.Seed = new System.Random().Next(3000000);
        seedField.text = GlobalVariables.generationSettings.Seed.ToString();
    }

    public void OpenGenerationMenu()
    {
        menuManager.ShowMenu("Generation");
        GlobalVariables.generationSettings.terrainChunkCountX = 35;
        GlobalVariables.generationSettings.terrainChunkCountY = 21;

        GlobalVariables.generationSettings.tropicTreeProcent = 50;
        GlobalVariables.generationSettings.standartTreeProcent = 50;
        GlobalVariables.generationSettings.winterTreeProcent = 50;
        GlobalVariables.generationSettings.rockProcent = 5;
        GlobalVariables.generationSettings.mainlandsCount = 3;
        GlobalVariables.generationSettings.mixingBiomesCount = 3;

        GlobalVariables.generationSettings.Seed = new System.Random().Next(3000000);
        seedField.text = GlobalVariables.generationSettings.Seed.ToString();
    }

    public async void ListenHostsAsync()
    {
        UdpClient uc = new UdpClient();
        uc.Client.Bind(new IPEndPoint(IPAddress.Any, GlobalVariables.Port));
        await Task.Run(() =>
        {
            while(!connected)
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, GlobalVariables.Port);
                byte[] responce = uc.Receive(ref remote);
                string r = Encoding.UTF8.GetString(responce);
                //Debug.Log("Recieved: " + r);
                if (r.Contains("SDHost"))
                {
                    string name = r.Substring(6);
                    string addr = remote.Address.MapToIPv4().ToString();
                    if (!Host_List.HasHost(name))
                        Hosts.Add(new Tuple<string, string>(addr, name));
                }
            }
        });
        
    }

    public async void Broadcasting()
    {
        await Task.Run(() =>
        {
            while (!connected)
            {
                IPHostEntry entry = Dns.GetHostEntry(IPAddress.Parse("127.0.0.1"));
                foreach (var a in entry.AddressList)
                {
                    IPAddress addr = a.MapToIPv4();
                    IPEndPoint ep = new IPEndPoint(addr, GlobalVariables.Port);
                    UdpClient uc = new UdpClient();
                    try
                    {
                        uc.Client.Bind(ep);
                        string message = $"SDHost{GlobalVariables.SelfName}";
                        byte[] tosend = Encoding.UTF8.GetBytes(message);
                        uc.Send(tosend, tosend.Length, "255.255.255.255", GlobalVariables.Port);
                        //Debug.Log("Sended on " + addr.ToString());
                    }
                    catch (SocketException ex)
                    {
                        Debug.Log(ex.Message + "\n" + "Error on " + addr.ToString());
                    }
                }
            }
        });
    }
}
