using System.Collections;
using TMPro;
using UnityEngine;

public class UI_TextBlinkEffect : MonoBehaviour
{
    [SerializeField] private float cycleDuration;
    [SerializeField] private TextMeshProUGUI text;

    private Color transparentColor = new Color(1, 1, 1, 0.2f);

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        float halfCycle = cycleDuration / 2;

        while (true)
        {
            ToggleColor(Color.white);
            yield return new WaitForSeconds(halfCycle);

            ToggleColor(transparentColor);
            yield return new WaitForSeconds(halfCycle);
        }
        ;
    }

    private void ToggleColor(Color color)
    {
        text.color = color;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
