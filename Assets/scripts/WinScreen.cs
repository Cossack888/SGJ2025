using TMPro;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public TMP_Text Win;
    public void Init()
    {
        Win.text = "Congratulations you finished the level with " + LevelManager.Instance.points + " Points";
    }


}
