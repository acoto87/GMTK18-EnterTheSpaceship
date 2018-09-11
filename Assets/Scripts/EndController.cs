using UnityEngine;

public class EndController : MonoBehaviour
{
    public GameObject endText;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            endText.SetActive(true);
        }
    }
}
