using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Slider loadingBar;
    public Image fill;
    private float progressValue = 0f;
    private float minLoadingTime = 3f;

    public TMP_Text gameTipPlaceholder;

    private List<string> gameTips = new List<string>()
    {
        "Dehydration can still occur in cold weather. Always carry a sufficient supply of water.",
        "In freezing conditions, maintaining body warmth is crucial. Always have enough resources with you to start a fire."
    };

    public void LoadLevel()
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        SetRandomTip();

        // Run async loading
        fill.fillAmount = progressValue;
        StartCoroutine(LoadLevelAsync());
    }

    void SetRandomTip()
    {
        int randomIndex = Random.Range(0, gameTips.Count);
        string randomTip = gameTips[randomIndex];
        gameTipPlaceholder.text = randomTip;
    }

    private IEnumerator LoadLevelAsync()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Glacial Frontier");
        loadOperation.allowSceneActivation = false;

        var elapsedTime = 0f;

        while (elapsedTime < minLoadingTime & !loadOperation.isDone)
        {
            progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            fill.fillAmount = elapsedTime / minLoadingTime; // chtěl bych nějak spojit čas a progressValue
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        loadOperation.allowSceneActivation = true;
    }
}