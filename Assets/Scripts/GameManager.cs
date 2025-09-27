using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public UIManager uiManager;

    private void Awake()
    {
        Instance = this;
    }

    public void OnCustomerServed(int reward, Vector3 pos)
    {
        uiManager.AddScore(reward);
        uiManager.SpawnFlyingCurrency(pos, reward);
    }
}
