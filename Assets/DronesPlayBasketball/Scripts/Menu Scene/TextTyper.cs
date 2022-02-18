using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TextTyper : MonoBehaviour
{
    public float letterPause = 0.2f;
    public string message;

    TMP_Text textComp;

    void Awake()
    {
        textComp = GetComponent<TMP_Text>();
        //message = textComp.text;
    }

    void OnEnable()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textComp.text = "";
        yield return new WaitForSeconds(letterPause);

        foreach (char letter in message)
        {
            textComp.text += letter;
            yield return new WaitForSeconds(letterPause);
        }

        yield return new WaitForSeconds(letterPause);
        textComp.enabled = false;
        yield return new WaitForSeconds(letterPause);
        textComp.enabled = true;
        yield return new WaitForSeconds(letterPause);
        textComp.enabled = false;
        yield return new WaitForSeconds(letterPause);
        textComp.enabled = true;
        yield return new WaitForSeconds(letterPause);

        StartCoroutine(TypeText());
    }
}