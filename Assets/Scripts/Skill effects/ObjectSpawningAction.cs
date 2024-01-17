using UnityEngine;

public class ObjectSpawningAction : MonoBehaviour
{
    [SerializeField] private GameObject objPrefab;

    public void ApplyEffectToCaster(Skill usedSkill, StatController caster, StatController target)
    {
        Instantiate(objPrefab, Vector3.zero, Quaternion.identity, caster.transform);
    }
}