using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alteruna.Trinity;

public class PlayroomMenu : MonoBehaviour
{
    [SerializeField]
    private Text TitleText;
    [SerializeField]
    private ScrollRect ScrollRect;
    [SerializeField]
    private GameObject PlayroomEntry;
    [SerializeField]
    private GameObject ContentContainer;
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private Button LeaveButton;

    private AlterunaTrinity mTrinity;
    private List<Playroom> mAvailablePlayrooms = new List<Playroom>();
    private List<GameObject> mListObjects = new List<GameObject>();

    private void OnNewAvailableDevice(AlterunaTrinity origin, IDevice device)
    {
        UpdateList();
    }

    private void OnLostAvailableDevice(AlterunaTrinity origin, IDevice device)
    {
        UpdateList();
    }

    private void UpdateList()
    {
        mTrinity?.GetAvailablePlayrooms(mAvailablePlayrooms);

        for (int i = 0; i < mListObjects.Count; i++)
        {
            Destroy(mListObjects[i]);
        }
        mListObjects.Clear();

        if (ContentContainer != null)
        {
            for (int i = 0; i < mAvailablePlayrooms.Count; i++)
            {
                GameObject entry = Instantiate(PlayroomEntry, ContentContainer.transform);
                entry.SetActive(true);
                mListObjects.Add(entry);
                Playroom room = mAvailablePlayrooms[i];
                entry.GetComponentInChildren<Text>().text = room.Name;
                entry.GetComponentInChildren<Button>().onClick.AddListener(() => { JoinRoom(room); });
            }
        }
    }

    private void JoinRoom(Playroom room)
    {
        mTrinity?.LeavePlayroom();
        mTrinity?.JoinRemotePlayroom(room?.Host);

        if (TitleText != null)
        {
            TitleText.text = "In Playroom " + room?.Name;
        }
    }

    private void JoinOwnRoom()
    {
        mTrinity?.LeavePlayroom();
        mTrinity?.JoinOwnPlayroom();

        if (TitleText != null)
        {
            TitleText.text = "In my own playroom";
        }
    }

    private void LeaveProom()
    {
        mTrinity?.LeavePlayroom();

        if (TitleText != null)
        {
            TitleText.text = "Trinity Playrooms";
        }
    }

    private void Start()
    {
        if (mTrinity == null)
        {
            mTrinity = FindObjectOfType<AlterunaTrinity>();
        }

        if (mTrinity != null)
        {
            mTrinity.NewAvailableDevice.AddListener(OnNewAvailableDevice);
            mTrinity.LostAvailableDevice.AddListener(OnLostAvailableDevice);
            StartButton.onClick.AddListener(JoinOwnRoom);
            LeaveButton.onClick.AddListener(LeaveProom);
        }
    }
}
