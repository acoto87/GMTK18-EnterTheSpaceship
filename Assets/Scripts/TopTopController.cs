using UnityEngine;
using UnityEngine.UI;

public class TopTopController : MonoBehaviour
{
    public Text text;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);
        }
    }
}
