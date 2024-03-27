using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TwoOfKindManager : MonoBehaviour
{
    [SerializeField] private List<PossibleLoot> prizePrefabs;
    [SerializeField] private TwoOfKindOption optionPrefab;
    [SerializeField] private Vector2Int optionSize;
    [SerializeField] private Vector3 offsetFactor;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform optionParent;
    [SerializeField] private CinemachineVirtualCamera miniGameCamera;
    [SerializeField] private int maxPairChoice = 3;

    [Header("SFX")]
    [SerializeField] private AudioSource source;

    [SerializeField] private AudioClip optionOpenedSound;
    [SerializeField] private AudioClip pairMatchedSound;
    [SerializeField] private AudioClip nonPairMatchedSound;

    private TwoOfKindOption prevOpennedOption;
    private bool isFirstOption = true;

    private int pairCounter = 0;

    [ContextMenu("Generate options")]
    public void GenerateOptions()
    {
        for (int x = 0; x < optionSize.x; x++)
        {
            for (int y = 0; y < optionSize.y; y++)
            {
                var option = Instantiate(optionPrefab, optionParent);
                var pos = optionParent.position;
                pos.x += (offsetFactor.x + offset.x) * x;
                pos.z += (offsetFactor.z + offset.z) * y;
                option.transform.position = pos;
                option.Manager = this;
                option.PrizeName = prizePrefabs[Random.Range(0, prizePrefabs.Count)];
            }
        }

        StartGame();
    }


    public void OptionOpened(TwoOfKindOption otherOption)
    {
        if (isFirstOption)
        {
            prevOpennedOption = otherOption;
            isFirstOption = false;
            return;
        }

        if (prevOpennedOption == otherOption) return;

        isFirstOption = true;
        pairCounter++;

        if (prevOpennedOption.PrizeName == otherOption.PrizeName)
        {
            prevOpennedOption.enabled = false;
            otherOption.enabled = false;

            source.PlayOneShot(pairMatchedSound);

            if (prevOpennedOption.PrizeName.itemPrefab != null)
            {
                var newDrop = Instantiate(prevOpennedOption.PrizeName.itemPrefab);
                newDrop.count = prevOpennedOption.PrizeName.count.y;
                EventManager.OnItemAdd?.Invoke(newDrop);
            }
        }
        else
        {
            prevOpennedOption.Cover();
            otherOption.Cover();
            source.PlayOneShot(nonPairMatchedSound);
        }

        if (!CanOpen())
        {
            EndGame();
        }
    }

    public void PlayUncoveringSfx()
    {
        source.PlayOneShot(optionOpenedSound);
    }

    public bool CanOpen()
    {
        return pairCounter < maxPairChoice;
    }

    private void StartGame()
    {
        miniGameCamera.Priority = 15;
        FloorManager.Instance.ToggleFollowCamera(false);
        PlayerInputController.Instance.playerInput.DeactivateInput();
    }

    private void EndGame()
    {
        miniGameCamera.Priority = 5;
        FloorManager.Instance.ToggleFollowCamera(true);
        PlayerInputController.Instance.playerInput.ActivateInput();
    }
}