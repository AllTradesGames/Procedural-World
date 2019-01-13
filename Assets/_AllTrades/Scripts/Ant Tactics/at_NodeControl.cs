using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class at_NodeControl : MonoBehaviour
{

    public Text amountText;
    public int team;
    public int amount;

    public int growthPerSec = 1;

    private float growthDelay;
    private float growthTimer;

    // Start is called before the first frame update
    void Start()
    {
        amountText.text = amount.ToString();
        growthDelay = 1f / (float)growthPerSec;
    }

    void FixedUpdate()
    {
        if (team != 0)
        {
            growthTimer += Time.deltaTime;
            if (growthTimer > growthDelay)
            {
                AddAmount(1);
                growthTimer = 0f;
            }
        }
    }

    void AddAmount(int inAmount)
    {
        amount += inAmount;
        amountText.text = amount.ToString();
    }
}
