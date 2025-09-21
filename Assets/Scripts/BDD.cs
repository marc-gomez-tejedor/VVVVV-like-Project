using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class BDD : MonoBehaviour
{
    private bool canDelete = false;
    public GameObject buttonPrefab;
    public Transform scrollview;

    public UnityEvent<string> changeMode;

    private void Start()
    {
        List<int> ids = DBManager.GetAllIds();

        for (int i = 0; i < ids.Count; i++)
        {
            int dbId = ids[i];
            int slotNumber = GetNextAvailableSlot();
            usedSlots.Add(slotNumber);

            GameObject button = Instantiate(buttonPrefab);
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"Save Slot {slotNumber}";
            button.GetComponentInChildren<Button>().onClick.AddListener(() => { LoadGame(dbId, slotNumber, button); });
            button.transform.SetParent(scrollview);
            button.transform.localScale = Vector2.one;
        }
    }
    private HashSet<int> usedSlots = new HashSet<int>();

    private int GetNextAvailableSlot()
    {
        int slot = 1;
        while (usedSlots.Contains(slot))
        {
            slot++;
        }
        return slot;
    }

    public void LoadGame(int dbId, int slotNumber, GameObject button)
    {
        if (canDelete)
        {
            DBManager.DeleteGame(dbId);
            Destroy(button);
            usedSlots.Remove(slotNumber);
        }
        else
        {
            DBManager.ActualGameId = dbId;
            SceneManager.LoadScene("Game");
        }
    }
    public void AddGame()
    {
        DBManager.NewGame();
        int dbId = DBManager.GetLastId();

        int slotNumber = GetNextAvailableSlot();
        usedSlots.Add(slotNumber);

        GameObject button = Instantiate(buttonPrefab);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"Save Slot {slotNumber}";
        button.GetComponentInChildren<Button>().onClick.AddListener(() => { LoadGame(dbId, slotNumber, button); });
        button.transform.SetParent(scrollview);
        button.transform.localScale = Vector2.one;
    }

    public void ChangeMode()
    {
        canDelete = !canDelete;
        if (canDelete)
        {
            changeMode.Invoke("REMOVING");
        }
        else
        {
            changeMode.Invoke("SELECTING");
        }
    }
}
