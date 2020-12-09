using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageNumberDisplay : GridCombatEffects
{
    public TMPro.TextMeshPro damageText;
    public AnimationCurve curve;
    public Vector3 startPos;
    public Vector3 endPos;
    public float displayTime = 1;

    protected override void OnDamageTaken(int damage)
    {
        StartCoroutine(DisplayDamage(damage));
    }

    IEnumerator DisplayDamage(int damage)
    {
        damageText.text = damage.ToString();
        damageText.enabled = true;
        float timer = 0;
        while(timer < displayTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, curve.Evaluate(timer));
            timer += Time.deltaTime;
            yield return null;
        }
        damageText.enabled = false;
    }
}
