using UnityEngine;
using UnityEngine.UI;

public class DeathBoxController : MonoBehaviour
{
    public Text FloatedAwayText;

    private string[] floatedAwayTexts =
    {
        "Oh no! You floated away and now you have no way to return. Please, do not float again into emptiness.",
        "Do you have some affection for the vacuum? Please, do not float again into the void.",
        "Come on! You can not possibly like the emptiness and darkness and coldness and other nesses of the space, do you?"
    };

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Invoke("ShowFloatedAwayText", 2.0f);
        }
    }

    void ShowFloatedAwayText()
    {
        FloatedAwayText.text = floatedAwayTexts[Random.Range(0, floatedAwayTexts.Length)];
        FloatedAwayText.gameObject.SetActive(true);
        Invoke("RespawnPlayer", 4.0f);
    }

    void RespawnPlayer()
    {
        FloatedAwayText.gameObject.SetActive(false);
        var player = GameObject.FindObjectOfType<PlayerController2D>();
        player.Respawn();
    }
}
