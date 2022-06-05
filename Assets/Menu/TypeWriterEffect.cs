using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static  NatoAlphabetConverter;

public class TypeWriterEffect : MonoBehaviour
{
    public float durationOfSingleCharacterAnimation=0.02f;
    public bool startDirectly = true;
    public bool forceNewLine = true;
    
    private TextMeshProUGUI _mTextMeshPro;
    
    //animation queue variables
    private IEnumerator _runningCoroutine = null;
    private Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator> ();

    void Start()
    {
        _mTextMeshPro = GetComponent<TextMeshProUGUI>() ?? gameObject.AddComponent<TextMeshProUGUI>();
        _mTextMeshPro.ForceMeshUpdate(); //since we are calling in the start, this forces to render the text mesh. Otherwise we wouldnt be able to get the attributes since it hasnt laoded yet.
        _mTextMeshPro.maxVisibleCharacters = 0; //making text invisible.
        
        if (startDirectly)
        {
             AnimateAll();
        }
    }

    public void AnimateAll()
    
    {
        var totalCharacters = _mTextMeshPro.textInfo.characterCount;
        StartCoroutine(AnimateText(0, totalCharacters));

    }

    public void AddTextLoop(int iterations)
    {
        for (int i = 0; i <= iterations; i++)
        {
            QueueTextAddition(IntToLetters(i)); 
            QueueTextAddition(LettersToName(IntToLetters(i)));
        }
        
    }


    public void QueueTextAddition(string textToAdd)

    {
        if (_runningCoroutine == null)
        {
            _runningCoroutine = AddText(textToAdd);
            StartCoroutine(_runningCoroutine);
        }
        else
            _coroutineQueue.Enqueue(AddText(textToAdd));
    }
    
    
    private IEnumerator AddText(string textToAdd)
    {
        if (forceNewLine)
            textToAdd = Environment.NewLine +textToAdd; //adding newline to the begining;
        
        var startAnimationPosition = _mTextMeshPro.text.Length; //get length
        int endAnimationPosition = startAnimationPosition + textToAdd.Length; //get new length
        _mTextMeshPro.text += textToAdd; //append
        StartCoroutine(AnimateText(startAnimationPosition, endAnimationPosition));
        
        var animationDuration = textToAdd.Length * durationOfSingleCharacterAnimation;
        yield return new WaitForSeconds(animationDuration);

        CheckAndUpdateQueue();

    }

    private void CheckAndUpdateQueue()
    {
        _runningCoroutine = null;
        if (_coroutineQueue.Count > 0)
        {
            _runningCoroutine = _coroutineQueue.Dequeue();
            StartCoroutine(_runningCoroutine);
        }
    }
    
    private IEnumerator AnimateText(int startCharPos, int endCharPos)
    {
        
        int visibleCount =startCharPos;
        var delayBetweenLetters = new WaitForSeconds(durationOfSingleCharacterAnimation);
        
        while (visibleCount<endCharPos)
        {
            visibleCount ++;
            _mTextMeshPro.maxVisibleCharacters = visibleCount;
            yield return delayBetweenLetters;
        }
    }
    
}
