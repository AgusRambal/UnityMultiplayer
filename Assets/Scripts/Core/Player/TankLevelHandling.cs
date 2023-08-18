using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankLevelHandling : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private List<GameObject> tankPartsLVL1 = new List<GameObject>();
    [SerializeField] private List<GameObject> tankPartsLVL2 = new List<GameObject>();
    [SerializeField] private List<GameObject> tankPartsLVL3 = new List<GameObject>();

    public int lvl { get; private set; } = 1;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.LevelUpEvent += SetLevel;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.LevelUpEvent -= SetLevel;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            lvl++;
            SetLevel(lvl);
        }
    }

    private void SetLevel(int lvlup)
    {
        if (lvl == 2)
        {
            tankPartsLVL1.ForEach(tankPart => tankPart.SetActive(false));
            tankPartsLVL2.ForEach(tankPart => tankPart.SetActive(true));
        }

        if (lvl == 3)
        {
            tankPartsLVL1.ForEach(tankPart => tankPart.SetActive(false));
            tankPartsLVL2.ForEach(tankPart => tankPart.SetActive(false));
            tankPartsLVL3.ForEach(tankPart => tankPart.SetActive(true));
        }
    }
}
