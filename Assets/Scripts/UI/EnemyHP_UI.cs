using UnityEngine;
using UnityEngine.UI;

public class EnemyHP_UI : MonoBehaviour
{
    [SerializeField] private EnemyScript enemy;
    [SerializeField] private Transform hPBar;

    private Image barImage;

    private void Start()
    {
        barImage = hPBar.GetComponent<Image>();
        enemy.OnDamageTaken += Enemy_OnDamageTaken;
    }

    private void Enemy_OnDamageTaken(object sender, System.EventArgs e)
    {
        float hpPercantage = enemy.CurrentEnemyHP() / enemy.MaxEnemyHP();
        barImage.fillAmount = hpPercantage;
        if (hpPercantage > 0.6f)
        {
            barImage.color = Color.green;
        }
        else if (hpPercantage < 0.2f)
        {
            barImage.color = Color.red;
        }
        else
        {
            float hpNormalized = (hpPercantage - 0.2f) / 0.4f;
            float redPart = Mathf.Lerp(1, 0, hpNormalized)*2;
            float greenPart = Mathf.Lerp(0, 1, hpNormalized)*2;
            barImage.color = new Color(redPart, greenPart, 0, 1);
        }

    }
}
