using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    [CreateAssetMenu(fileName = "PotentialQuests", menuName = "Quests", order = 0)]
    public class PotentialQuests : ScriptableObject
    {
        public List<PotentialQuest> potentialQuests;

        public void AddQuests(List<QuestData> currentQuests, int desiredCount)
        {
            var flag = 0;

            while (currentQuests.Count < desiredCount && flag < potentialQuests.Count)
            {
                var potentialQuest = potentialQuests[flag];
                flag++;
                if (currentQuests.Exists(quest => quest.targetName == potentialQuest.targetName)) continue;

                var questData = new QuestData(potentialQuest);
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