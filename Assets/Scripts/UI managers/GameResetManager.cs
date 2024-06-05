using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResetManager : MonoBehaviour
{
    public void ResetData()
    {
        DataManager.Instance.DeleteAllData();
    }
}
