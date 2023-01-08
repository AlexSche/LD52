using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManagement : MonoBehaviour
{
    public Text collectedDiamondsText;
    // Start is called before the first frame update
    void Start()
    {
        // start to eat diamonds after 10 seconds every 10 seconds
        InvokeRepeating("eatDiamonds", 10.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        int amount = PlayerPrefs.GetInt("collectedDiamonds", 0);
        collectedDiamondsText.text = amount.ToString();
    }

    void eatDiamonds() {
        //remove diamonds if avaiable else countdown
        int amount = PlayerPrefs.GetInt("collectedDiamonds", 0);
        if (amount > 10) {
            PlayerPrefs.SetInt("collectedDiamonds", amount - 10);
        } else {
            startCountdown();
        }
    }

    void startCountdown() {

    }
}
