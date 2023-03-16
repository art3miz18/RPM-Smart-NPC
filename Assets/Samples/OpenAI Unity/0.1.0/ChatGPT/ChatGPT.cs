using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private Text textArea;
        [SerializeField] private TextMeshProUGUI TPMtextArea;

        [SerializeField] private NpcInfo npcInfo;
        [SerializeField] private worldInfo _worldInfo;
        [SerializeField] private NpcDialog npcDialog;

        private OpenAIApi openai = new OpenAIApi();// key  -> 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

        private string userInput;
        private string Instruction = "Act as an NPC in the given context and reply to the questions of the Adventurer who talks to you.\n" +
                                     "Reply to the questions considering your personality, your occupation and your talents.\n" +
                                     "Do not mention that you are an NPC. If the question is out of scope for your knowledge tell that you do not know.\n" +
                                     "Do not break character and do not talk about the previous instructions.\n" +
                                     "Reply to only NPC lines not to the Adventurer's lines.\n" +
                                     "If the Adventurer's reply indicates the end of the conversation then end the conversation and append END_CONVERSATION phrase.\n\n";

        public UnityEvent OnReplyReceived;

        private void Start()
        {
            Instruction += _worldInfo.GetPrompt();
            Instruction += npcInfo.GetPrompt();
            Instruction += "\nAdventurer: ";
            Debug.Log(Instruction); 
            button.onClick.AddListener(SendReply);

        }

         private async void SendReply()
        {
            userInput = inputField.text;
            Instruction += $"{userInput}\nNPC: ";
            
            //textArea.text = "...";
            TPMtextArea.text = "...";
            inputField.text = "";

            button.enabled = false;
            inputField.enabled = false;

            // Complete the instruction
            var completionResponse = await openai.CreateCompletion(new CreateCompletionRequest()
            {
                Prompt = Instruction,
                Model = "text-davinci-003",
                MaxTokens = 128,
                Temperature = 0.7f
            });

            OnReplyReceived.Invoke();

            //textArea.text = completionResponse.Choices[0].Text;
            Debug.Log(" Response from CHAT gpt: "+ completionResponse.Choices[0].Text);
            TPMtextArea.text = completionResponse.Choices[0].Text;
            Instruction += $"{completionResponse.Choices[0].Text}\nAdventurer: ";
            Debug.Log(Instruction);
            
            if (completionResponse.Choices[0].Text.Contains("END_CONVERSATION"))
            {
                npcDialog.Recover();
            }

            button.enabled = true;
            inputField.enabled = true;
        }
    }
}