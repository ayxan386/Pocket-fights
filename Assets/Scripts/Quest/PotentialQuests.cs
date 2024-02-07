using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Quest
{
    [CreateAssetMenu(fileName = "PotentialQuests", menuName = "Quests", order = 0)]
    public class PotentialQuests : ScriptableObject
    {
        public List<PotentialQuest> potentialQuests;

        public void AddQuests(List<QuestData> currentQuests, int desiredCount)
        {
            Debug.Log($"Trying to add quest to {currentQuests.Count} upto {desiredCount}");
            var flag = 0;
            while (currentQuests.Count < desiredCount && flag < 100)
            {
                flag++;
                var potentialQuest = potentialQuests[Random.Range(0, potentialQuests.Count)];

                var questData = new QuestData(potentialQuest);
                Debug.Log($"Adding new quest {questData}");
                currentQuests.Add(questData);
            }
        }
    }

    [Serializable]
    public class PotentialQuest
    {
        public string title;
        public Vector2Int goalRange;
        public string targetName;
        public int goldAmount;
        public int expAmount;
    }
}