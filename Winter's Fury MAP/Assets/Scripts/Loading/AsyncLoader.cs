using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    public Image fill;
    private float progressValue = 0f;
    // private readonly float minLoadingTime = 3f;

    public TMP_Text gameTipPlaceholder;

    private readonly List<string> gameTips = new List<string>()
    {
        "Dehydration can still occur in cold weather. Always carry a sufficient supply of water.",
        "In freezing conditions, maintaining body warmth is crucial. Always have enough resources with you to start a fire.",
        "The frozen landscape is both beautiful and deadly. Respect its challenges by arming yourself with warmth, sustenance, and a well-fortified refuge.",
        "In the icy wilderness, fire is your ally. Carry the means to ignite warmth wherever you go, for in winter, it's not just a luxury – it's a necessity.",
        "Blizzards can strike unexpectedly. Seek shelter immediately to wait out the storm, and ensure you have enough supplies to weather the whiteout.",
        "Beware of hastily indulging in canned goods. Consuming uncooked or spoiled provisions from crates may lead to food poisoning. Ensure proper preparation before dining."
    };

    private bool isClicked = false;
    private bool isReadyToPlay;
    public GameObject pressKeyInfo;
    public CanvasGroup pkcg;

    private void Start()
    {
        pkcg = pressKeyInfo.GetComponent<CanvasGroup>();
        pkcg.alpha = 0;
        loadingScreen.SetActive(false);
        // pkcg.LeanAlpha(1, 2);
        // LeanTween.alpha(pressKeyInfo, 1f, 0.5f).setEase(LeanTweenType.easeInCirc);
    }

    private void Update()
    {
        if (Input.anyKeyDown && isReadyToPlay)
        {
            isClicked = true;
        }

        if (isReadyToPlay)
        {
            // if (pkcg.alpha < 1)
            // {
            //     pkcg.alpha += Time.deltaTime;
            // }
            pkcg.LeanAlpha(1, 1);
            isReadyToPlay = false;
        }
    }

    public void LoadLevel()
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        SetRandomTip();

        fill.fillAmount = progressValue;
        StartCoroutine(LoadLevelAsync());
        // SceneManager.LoadScene(1);
    }

    private void SetRandomTip()
    {
        int randomIndex = Random.Range(0, gameTips.Count);
        string randomTip = gameTips[randomIndex];
        gameTipPlaceholder.text = randomTip;
    }

    private IEnumerator LoadLevelAsync() // NEFUNGUJE
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Glacial Frontier");
        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            fill.fillAmount = progressValue;
            Debug.Log("returning");
            yield return null;
        }

        Debug.Log("isReadyToPlay");
        isReadyToPlay = true;
        while (!isClicked)
        {
            Debug.Log("isClicked");
            yield return null;
        }

        loadOperation.allowSceneActivation = true;
    }
}