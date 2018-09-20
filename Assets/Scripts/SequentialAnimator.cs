using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialAnimator : MonoBehaviour
{

    public CanvasGroup[] uiElements;

    private void Awake()
    {
        foreach (CanvasGroup cg in uiElements)
        {
            cg.alpha = 0;
        }
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        foreach (CanvasGroup cg in uiElements)
        {
            yield return StartCoroutine(Fader.Instance.FadeCanvasGroup(cg, 0, 1, 1));
        }
    }

    private IEnumerator FadeOut()
    {
        foreach (CanvasGroup cg in uiElements)
        {
            yield return StartCoroutine(Fader.Instance.FadeCanvasGroup(cg, 1, 0, 1));
        }
        CanvasGroup background = GetComponent<CanvasGroup>();
        if (background != null)
            yield return StartCoroutine(Fader.Instance.FadeCanvasGroup(background, 1, 0, 1));
    }

    // Update is called once per frame
    void Update()
    {

    }


}
