using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public TMP_Text Die;
    public void Init()
    {
        Die.text = "Well you died but with " + LevelManager.Instance.points + " Points. Better luck next time";
    }


}
