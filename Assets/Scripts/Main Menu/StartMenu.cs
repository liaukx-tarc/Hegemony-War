using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public static StartMenu instance;

    private void Start()
    {
        instance = this;

        width = (int)widthSlider.value;
        height = (int)heightSlider.value;

        landChance = (int)landChanceSlider.value;
        coastChance = (int)coastChanceSlider.value;
        snowChance = (int)snowChanceSlider.value;
        forestChance = (int)forestChanceSlider.value;
        desertChance = (int)desertChanceSlider.value;
        marshChance = (int)marshChanceSlider.value;

        repeatTimes = (int)repeatTimesSlider.value;
        deathLimit = (int)deathLimitSlider.value;
        birthLimit = (int)birthLimitSlider.value;
        playerNum = 0;

        colorList = colorOriginalList;
    }

    [Header("Map Size")]
    public static int width;
    public Slider widthSlider;
    public TMP_InputField widthInput;

    public void WidthSliderChange()
    {
        width = (int)widthSlider.value;
        widthInput.text = width.ToString();
    }

    public void WidthInputChange()
    {
        width = Mathf.Min(Convert.ToUInt16(widthInput.text), (int)widthSlider.maxValue);
        width = Mathf.Max(Convert.ToUInt16(widthInput.text), (int)widthSlider.minValue);
        widthInput.text = width.ToString();
        widthSlider.value = width;
    }

    [Space]
    public static int height;
    public Slider heightSlider;
    public TMP_InputField heightInput;

    public void HeightSliderChange()
    {
        height = (int)heightSlider.value;
        heightInput.text = height.ToString();
    }

    public void HeightInputChange()
    {
        height = Mathf.Min(Convert.ToUInt16(heightInput.text), (int)heightSlider.maxValue);
        height = Mathf.Max(Convert.ToUInt16(heightInput.text), (int)heightSlider.minValue);
        heightInput.text = height.ToString();
        heightSlider.value = height;
    }

    [Header("Map Type")]
    public static int landChance;
    public Slider landChanceSlider;
    public TMP_InputField landChanceInput;

    public void LandChanceSliderChange()
    {
        landChance = (int)landChanceSlider.value;
        landChanceInput.text = landChance.ToString();
    }

    public void LandChanceInputChange()
    {
        landChance = Mathf.Min(Convert.ToUInt16(landChanceInput.text), (int)landChanceSlider.maxValue);
        landChance = Mathf.Max(Convert.ToUInt16(landChanceInput.text), (int)landChanceSlider.minValue);
        landChanceInput.text = landChance.ToString();
        landChanceSlider.value = landChance;
    }

    [Space]
    public static int coastChance;
    public Slider coastChanceSlider;
    public TMP_InputField coastChanceInput;

    public void CoastChanceSliderChange()
    {
        coastChance = (int)coastChanceSlider.value;
        coastChanceInput.text = coastChance.ToString();
    }

    public void CoastChanceInputChange()
    {
        coastChance = Mathf.Min(Convert.ToUInt16(coastChanceInput.text), (int)coastChanceSlider.maxValue);
        coastChance = Mathf.Max(Convert.ToUInt16(coastChanceInput.text), (int)coastChanceSlider.minValue);
        coastChanceInput.text = coastChance.ToString();
        coastChanceSlider.value = coastChance;
    }

    [Space]
    public static int snowChance;
    public Slider snowChanceSlider;
    public TMP_InputField snowChanceInput;

    public void SnowChanceSliderChange()
    {
        snowChance = (int)snowChanceSlider.value;
        snowChanceInput.text = snowChance.ToString();
    }

    public void SnowChanceInputChange()
    {
        snowChance = Mathf.Min(Convert.ToUInt16(snowChanceInput.text), (int)snowChanceSlider.maxValue);
        snowChance = Mathf.Max(Convert.ToUInt16(snowChanceInput.text), (int)snowChanceSlider.minValue);
        snowChanceInput.text = snowChance.ToString();
        snowChanceSlider.value = snowChance;
    }

    [Space]
    public static int forestChance;
    public Slider forestChanceSlider;
    public TMP_InputField forestChanceInput;

    public void ForestChanceSliderChange()
    {
        forestChance = (int)forestChanceSlider.value;
        forestChanceInput.text = forestChance.ToString();
    }

    public void ForestChanceInputChange()
    {
        forestChance = Mathf.Min(Convert.ToUInt16(forestChanceInput.text), (int)forestChanceSlider.maxValue);
        forestChance = Mathf.Max(Convert.ToUInt16(forestChanceInput.text), (int)forestChanceSlider.minValue);
        forestChanceInput.text = forestChance.ToString();
        forestChanceSlider.value = forestChance;
    }

    [Space]
    public static int desertChance;
    public Slider desertChanceSlider;
    public TMP_InputField desertChanceInput;

    public void DesertChanceSliderChange()
    {
        desertChance = (int)desertChanceSlider.value;
        desertChanceInput.text = desertChance.ToString();
    }

    public void DesertChanceInputChange()
    {
        desertChance = Mathf.Min(Convert.ToUInt16(desertChanceInput.text), (int)desertChanceSlider.maxValue);
        desertChance = Mathf.Max(Convert.ToUInt16(desertChanceInput.text), (int)desertChanceSlider.minValue);
        desertChanceInput.text = desertChance.ToString();
        desertChanceSlider.value = desertChance;
    }

    [Space]
    public static int marshChance;
    public Slider marshChanceSlider;
    public TMP_InputField marshChanceInput;

    public void MarshChanceSliderChange()
    {
        marshChance = (int)marshChanceSlider.value;
        marshChanceInput.text = marshChance.ToString();
    }

    public void MarshChanceInputChange()
    {
        marshChance = Mathf.Min(Convert.ToUInt16(marshChanceInput.text), (int)marshChanceSlider.maxValue);
        marshChance = Mathf.Max(Convert.ToUInt16(marshChanceInput.text), (int)marshChanceSlider.minValue);
        marshChanceInput.text = marshChance.ToString();
        marshChanceSlider.value = marshChance;
    }

    [Header("Cellular Automata")]
    public static int repeatTimes;
    public Slider repeatTimesSlider;
    public TMP_InputField repeatTimesInput;

    public void RepeatTimesSliderChange()
    {
        repeatTimes = (int)repeatTimesSlider.value;
        repeatTimesInput.text = repeatTimes.ToString();
    }

    public void RepeatTimesInputChange()
    {
        repeatTimes = Mathf.Min(Convert.ToUInt16(repeatTimesInput.text), (int)repeatTimesSlider.maxValue);
        repeatTimes = Mathf.Max(Convert.ToUInt16(repeatTimesInput.text), (int)repeatTimesSlider.minValue);
        repeatTimesInput.text = repeatTimes.ToString();
        repeatTimesSlider.value = repeatTimes;
    }

    [Space]
    public static int deathLimit;
    public Slider deathLimitSlider;
    public TMP_InputField deathLimitInput;

    public void DeathLimitSliderChange()
    {
        deathLimit = (int)deathLimitSlider.value;
        deathLimitInput.text = deathLimit.ToString();
    }

    public void DeathLimitInputChange()
    {
        deathLimit = Mathf.Min(Convert.ToUInt16(deathLimitInput.text), (int)deathLimitSlider.maxValue);
        deathLimit = Mathf.Max(Convert.ToUInt16(deathLimitInput.text), (int)deathLimitSlider.minValue);
        deathLimitInput.text = deathLimit.ToString();
        deathLimitSlider.value = deathLimit;
    }

    [Space]
    public static int birthLimit;
    public Slider birthLimitSlider;
    public TMP_InputField birthLimitInput;

    public void BirthLimitSliderChange()
    {
        birthLimit = (int)birthLimitSlider.value;
        birthLimitInput.text = birthLimit.ToString();
    }

    public void BirthLimitInputChange()
    {
        birthLimit = Mathf.Min(Convert.ToUInt16(birthLimitInput.text), (int)birthLimitSlider.maxValue);
        birthLimit = Mathf.Max(Convert.ToUInt16(birthLimitInput.text), (int)birthLimitSlider.minValue);
        birthLimitInput.text = birthLimit.ToString();
        birthLimitSlider.value = birthLimit;
    }

    public void CloseStartMenu()
    {
        gameObject.SetActive(false);
    }

    [Header("Player Selection")]
    public static int playerNum = 0;
    public int maxPlayer;
    public List<PlayerSelect> playerSelectsList = new List<PlayerSelect>();
    public List<Color> colorOriginalList = new List<Color>();
    public List<Color> colorList = new List<Color>();

    public GameObject playerSelectPrefab;
    public Transform playerListObj;

    public void AddPlayer()
    {
        if(playerNum < maxPlayer)
        {
            PlayerSelect playerSelect = Instantiate(playerSelectPrefab, playerListObj).GetComponent<PlayerSelect>();
            playerSelectsList.Add(playerSelect);

            string name = "Player " + playerNum;
            playerSelect.nameInput.text = name;

            playerSelect.playerColor.color = colorList[0];
            colorList.RemoveAt(0);

            playerNum++;
        }
    }

    public static List<Color> playerColorList = new List<Color>();
    public static List<string> playerNameList = new List<string>();

    public void StartGame()
    {
        if(playerSelectsList.Count >= 2)
        {
            playerColorList.Clear();
            playerNameList.Clear();

            foreach (PlayerSelect player in playerSelectsList)
            {
                playerColorList.Add(player.playerColor.color);
                playerNameList.Add(player.nameInput.text);
            }

            SceneManager.LoadScene("In_Game");
        }
    }
}
