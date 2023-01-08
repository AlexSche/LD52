using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResourceManagement : MonoBehaviour
{
    public Text collectedDiamondsText;
    public Text warning;
    private int amount;
    private int neededAmount = 10;
    // Start is called before the first frame update
    void Start()
    {
        // start to eat diamonds after 10 seconds every 10 seconds
        InvokeRepeating("eatDiamonds", 10f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        amount = PlayerPrefs.GetInt("collectedDiamonds", 0);
        collectedDiamondsText.text = amount.ToString();
    }

    void eatDiamonds() {
        //remove diamonds if avaiable else countdown
        if (amount >= neededAmount) {
            PlayerPrefs.SetInt("collectedDiamonds", amount - neededAmount);
        } else {
            startCountdown();
        }
    }

    void startCountdown() {
        ShowWarning("Your diamonds are low! Return your diamonds to the generator!");
        Invoke("EndGame", 10f);
    }

    void ShowWarning(string msg) {
        warning.text = msg;
        warning.enabled = true;
        Invoke("RemoveWarning", neededAmount);
    }

    void RemoveWarning() {
        warning.enabled = false;
    }

    void EndGame() {
        if (amount < neededAmount) {
            SceneManager.LoadScene("LostScene");
        }
    }
}
