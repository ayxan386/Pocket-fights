using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldAmountChangeAction : MonoBehaviour
{
   [SerializeField] private int amount;

   public void Use()
   {
      InventoryController.Instance.AddGold(amount);
   }
}
