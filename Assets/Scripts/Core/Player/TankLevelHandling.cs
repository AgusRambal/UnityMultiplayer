using System.Collections.Generic;
using UnityEngine;

public class TankLevelHandling : MonoBehaviour
{
    [SerializeField] private List<GameObject> tankPartsLVL1 = new List<GameObject>();
    [SerializeField] private List<GameObject> tankPartsLVL2 = new List<GameObject>();
    [SerializeField] private List<GameObject> tankPartsLVL3 = new List<GameObject>();

    public int lvl { get; private set; } = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            lvl++;
            SetLevel(lvl);
        }
    }

    private void SetLevel(int id)
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
