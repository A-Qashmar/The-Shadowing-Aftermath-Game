using UnityEngine;

public class Level2Manager : MonoBehaviour
{
    public int keysRequired = 2;
    public int keysCollected = 0;

    public Door bossDoor;   // uses your OLD Door.cs

    void Start()
    {
        if (bossDoor != null)
            bossDoor.Close();
    }

    public void AddKey()
    {
        keysCollected++;

        if (keysCollected >= keysRequired)
        {
            if (bossDoor != null)
                bossDoor.Open();
        }
    }
}
