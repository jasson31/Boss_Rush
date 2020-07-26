using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public Boss boss;
    private TextMesh text;
    private const float moveSpeed = 2f;
    private Color color;

    private void Awake()
    {
        text = GetComponent<TextMesh>();
        color = text.color;
    }
    private void OnEnable()
    {
        text.color = color;
        StartCoroutine(Fading());
    }

    private void OnDisable()
    {
        boss.RetrieveDamageText(this);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    private IEnumerator Fading()
    {
        Color color = text.color;
        yield return null;
        Vector3 startPosition = transform.position;

        for (float f = 0; f <= 1; f += Time.deltaTime)
        {
            transform.position = startPosition + Vector3.Lerp(Vector3.zero, Vector3.up * 2f, AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(f));
            color.a = 1 - f;
            text.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
