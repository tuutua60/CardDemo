using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundView : MonoBehaviour
{
    public Image imgRoundStart;

    public Text txtRoundCount;
    public Text txtLogicFrame;

    public Button btnSkip;
    public Button btnPause;
    public Button btnChangeSpeed;
    public Text txtSpeed;

    public void Initialize()
    {
        txtSpeed.text = "X1";
        txtRoundCount.text = "1";
    }
}
