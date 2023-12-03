using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMP_Text))]
public class TypeWriter : MonoBehaviour
{
    private TMP_Text _textBox;

    // CONTROLLERS
    private int _currentVisibleIndex;
    private Coroutine _typewriterCoroutine;
    private bool _readyForNewText = true;

    // DELAY
    private WaitForSeconds _simpleDelay;
    // private WaitForSeconds _interpunctuationDelay;

    // EVENT
    private WaitForSeconds _textboxFullEventDelay;
    [SerializeField] [Range(0.1f, 0.5f)] private float sendDoneDelay = 0.25f;
    public static event Action CompleteTextRevealed;
    public static event Action<char> CharactersRevealed;

    // IN UNITY
    [Header("Typewriter settings")]
    [SerializeField] private float charactersPerSecond = 30;
    // [SerializeField] private float interpunctuationDelay = 0.2f;

    // private void Start()
    // {
    //     StartCoroutine(Wait());
    // }

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>(); // získání componenty
        // nastavení delayů dle hodnot
        _simpleDelay = new WaitForSeconds(1 / charactersPerSecond);
        // _interpunctuationDelay = new WaitForSeconds(1 / interpunctuationDelay);
        _textboxFullEventDelay = new WaitForSeconds(sendDoneDelay);
    }

    // zavolá se pokaždé pokud je změna v textboxu - i odkrývání charů
    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(SetText);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(SetText);
    }

    private void SetText(Object obj) // kvůli eventu - object (ne string)
    {
        if (!_readyForNewText) return; // Enable a Disable by se zavolaly při každé změně -> TW by se zastavoval a začínal znova

        _readyForNewText = false; // kontrola že se text zobrazuje jak má

        if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine); // kontrola, že pouze jeden TW coroutine běží

        _textBox.maxVisibleCharacters = 0; // místo appendování charů - bylo by upravování stringů - not good for performance
        _currentVisibleIndex = 0; // kdyby víc textu, zůstalo by to na maxu

        _typewriterCoroutine = StartCoroutine(Typewriter()); // pro ovládání typewriteru - ienumerator, třeba pro zastavení
    }

    private IEnumerator Typewriter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo; // reference na text box info

        while (_currentVisibleIndex < textInfo.characterCount + 1) // dokud viditelný text méně než velikost textboxu
        {
            var lastCharIndex = textInfo.characterCount - 1; // latest char count

            if (_currentVisibleIndex == lastCharIndex)
            {
                _textBox.maxVisibleCharacters++;
                yield return _textboxFullEventDelay; // počká na duration fulleventdelay
                CompleteTextRevealed?.Invoke(); // invoke eventu
                _readyForNewText = true;
                yield break; // yield break out of coroutine
            }
            
            char character = textInfo.characterInfo[_currentVisibleIndex].character; // currently to be displayed char, místo _textbox.text (kvůli počítí tagů <b>)
            _textBox.maxVisibleCharacters++; // reveal next char to be displayed

            // if (character == '\'') yield return _interpunctuationDelay;
            // else yield return _simpleDelay;
            yield return _simpleDelay;

            CharactersRevealed?.Invoke(character); // passováním charu můžou ostatní reagovat na event, př. zahrát zvuk jen při "A"
            _currentVisibleIndex++;
        }
    }

    // private IEnumerator Wait()
    // {
    //     yield return new WaitForSeconds(2f);
    // }

    // void Skip()
    // {
    //     CompleteTextRevealed?.Invoke(); // invokování eventu, aby ostatní mohli subscribnout k textboxu a něco udělat třeba když je vidět celý
    // }
}