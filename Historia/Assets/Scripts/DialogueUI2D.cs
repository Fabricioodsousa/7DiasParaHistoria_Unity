using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI2D : MonoBehaviour
{
    [Header("Referências")]
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Configuração do Efeito")]
    public float typingSpeed = 0.03f; // tempo entre cada letra

    private NPCDialogue2D currentNPC;
    private bool isActive = false;
    private bool isTyping = false;
    private string currentSentence;
    private Coroutine typingCoroutine;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        // Se apertar espaço, pula ou vai para próxima frase
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Termina de digitar instantaneamente
                StopTypingInstantly();
            }
            else if (currentNPC != null)
            {
                // Avança para a próxima frase
                currentNPC.NextSentence();
            }
        }
    }

    public void ShowDialogue(string npcName, string firstSentence, NPCDialogue2D npc)
    {
        currentNPC = npc;
        nameText.text = npcName;
        panel.SetActive(true);
        isActive = true;

        DisplaySentence(firstSentence);
    }



    public void UpdateText(string newSentence)
    {
        DisplaySentence(newSentence);
    }

    public void HideDialogue()
    {
        panel.SetActive(false);
        isActive = false;
        currentNPC = null;
    }

    private void DisplaySentence(string sentence)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentSentence = sentence;
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void StopTypingInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentSentence;
        isTyping = false;
    }
}
