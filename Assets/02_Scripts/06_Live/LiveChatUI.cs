using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LiveChatUI : MonoBehaviour
{
    [SerializeField] private GameObject _chatBoxPrefab;
    [SerializeField] private int _maxChatCount = 5;

    private readonly Queue<GameObject> _chatBoxes = new();

    private void Awake()
    {
        CreateChatBoxes();
    }

    private void CreateChatBoxes()
    {
        for (int i = 0; i < _maxChatCount; i++)
        {
            GameObject chatBox = Instantiate(_chatBoxPrefab, transform);
            chatBox.SetActive(false);

            _chatBoxes.Enqueue(chatBox);
        }
    }

    public void AddChat(string message)
    {
        GameObject chatBox = _chatBoxes.Dequeue();

        TMP_Text chatText = chatBox.GetComponentInChildren<TMP_Text>();

        if (chatText != null)
            chatText.text = message;

        chatBox.SetActive(true);
        chatBox.transform.SetAsLastSibling();

        _chatBoxes.Enqueue(chatBox);
    }

    public void ClearChats()
    {
        foreach (GameObject chatBox in _chatBoxes)
        {
            chatBox.SetActive(false);
        }
    }
}