using System.Collections.Generic;
using UnityEngine;

public class KDManager : MonoBehaviour
{
    public static KDManager instance;
    /*public List<PlayerInstance> players = new List<PlayerInstance>();
    public List<int> playerLevel = new List<int>();
    public List<int> playerKillingCount = new List<int>();*/

    public int playerLevel;
    public int playerKillingCount;

    private void Awake()
    {
        instance = this;
    }
}
