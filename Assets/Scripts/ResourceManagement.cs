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
        
    }

    // Update is called once per frame
    void Update()
    {
        int amount = PlayerPrefs.GetInt("collectedDiamonds", 0);
        collectedDiamondsText.text = amount.ToString();
    }
}
