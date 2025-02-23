using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    [SerializeField]
    public QuestionData question;

    [SerializeField]
    private TMP_InputField[] inputFields; // Array of input fields

    [SerializeField]
    private GameObject letterContainer;

    [SerializeField]
    private Image questionImageUI;

    [SerializeField]
    private Text questionTextUI;

    [SerializeField]
    private TMP_Text messageText;

    [SerializeField]
    private AudioClip correctSound;
    
    [SerializeField]
    private AudioClip wrongSound;

    private AudioSource audioSource;
    private int hintIndex = 0;
    private int mistakes = 0;
    private int firstWordLength = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DisplayQuestion();
    }

    private void DisplayQuestion()
    {
        if (question != null)
        {
            if (!string.IsNullOrEmpty(question.questionText))
            {
                questionTextUI.text = question.questionText;
            }

            if (question.questionImage != null)
            {
                questionImageUI.sprite = question.questionImage;
                questionImageUI.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("question.questionImage is null");
                questionImageUI.gameObject.SetActive(false);
            }
        }
    }

    public void CheckAnswer()
{
    Debug.Log("CheckAnswer function called!");

    // Collect all input field values and combine them into one string
    string userInput = "";
    foreach (TMP_InputField inputField in inputFields)
    {
        userInput += inputField.text; // Append each input field's text
    }
    
    hintIndex = 0;
    userInput = userInput.ToUpper().Trim(); // Convert to uppercase and trim spaces
    Debug.Log("User input (processed): " + userInput);

    // Process the correct answer: Remove spaces and convert to uppercase
    string correctAnswer = question.answer.Replace(" ", "").ToUpper();
    Debug.Log("Correct answer (processed): " + correctAnswer);

    // Compare processed user input with processed correct answer
    if (userInput == correctAnswer)
    {
         Debug.Log("Correct");

        PlaySound(correctSound);
        DisplayMessage("CORRECT!", Color.green);
        ProceedToNextQuestion();

       
    }
    else
    {
        Debug.Log("Wrong");

        HighlightLetters(userInput, correctAnswer);
         DisplayMessage("Wrong! 🎉", Color.red);
        PlaySound(wrongSound);

        mistakes ++;

        

        if(mistakes > 3)
        {
            Debug.LogWarning("Too many mistakes");
        }
    }

    // Clear all input fields after checking
    foreach (TMP_InputField inputField in inputFields)
    {
        inputField.text = "";
    }

    
}


    private void HighlightLetters(string userInput, string correctAnswer)
    {
        for (int i = 0; i < letterContainer.transform.childCount; i++)
        {
            GameObject letter = letterContainer.transform.GetChild(i).gameObject;
            TMP_Text letterText = letter.GetComponentInChildren<TMP_Text>();
            Image letterBackground = letter.GetComponent<Image>();

            if (i < userInput.Length && i < correctAnswer.Length && userInput[i] == correctAnswer[i])
            {
                letterBackground.color = Color.green;
            }
            else
            {
                letterBackground.color = Color.red;
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void DisplayMessage(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            StartCoroutine(FadeMessage());
        }
    }

    private IEnumerator FadeMessage()
    {
        Debug.Log("Fade Message called");
        float duration = 2f;
        float elapsedTime = 0f;

        Color startColor = messageText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < duration)
        {
            messageText.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        messageText.color = endColor;

    }

    private void ProceedToNextQuestion()
    {
        hintIndex = 0;
       StartCoroutine(LoadNextQuestion());

    }



    private IEnumerator LoadNextQuestion()
    {
        Debug.Log("Loading the next question...");

        int next = SceneManager.GetActiveScene().buildIndex + 1; 

        yield return new WaitForSeconds(1.75f);
       
       if (next < SceneManager.sceneCountInBuildSettings)
       {
         SceneManager.LoadSceneAsync(next);
       }
       else
       {
         Debug.LogWarning("No more scenes to load!"); 
       }
}


    public void HintSystem()
    {
        hintIndex ++;

        switch(hintIndex)
        {
            case 1:
              firstHint();
              break;
            case 2:
              secondHint();
              break;
            case 3:
              thirdHint();
              break;
            default:
              Debug.LogWarning("no more hints");
              break;
        }
        
    }





    private void firstHint()
    {
      Debug.Log("first Hint");
      firstWordLength = question.answer.IndexOf(' ');

      if (firstWordLength == -1)
      {
        // If there's no space, the entire answer is a single word
        firstWordLength = question.answer.Length;
      }

      Debug.Log("First word length: " + firstWordLength);

      string firstWord = question.answer.Substring(0, firstWordLength);

      Debug.Log("Filling input fields with: " + firstWord);

    // Fill the input fields with letters from the first word
    for (int i = 0; i < inputFields.Length; i++)
    {
        if (i < firstWord.Length)
        {
            inputFields[i].text = firstWord[i].ToString(); // Assign each letter to input field
        }
        else
        {
            inputFields[i].text = ""; // Clear any remaining input fields
        }
    }


     }




    private void secondHint()
    {
        Debug.Log("second Hint");

        string myword = question.answer.Replace(" ", "");
        int answerLength = myword.Length;

        Debug.Log("Answer length without spaces: " + answerLength);

        int randomLatter = UnityEngine.Random.Range(firstWordLength, answerLength);

        
        string myLatter = myword[randomLatter].ToString();

        TMP_InputField myField = inputFields[randomLatter];

        myField.text = myLatter;

    }


    private void thirdHint()
    {
        Debug.Log(" third Hint");

         string myword = question.answer.Replace(" ", "");
         int answerLength = myword.Length;

         for(int i = 0; i < answerLength; i++)
         {
            inputFields[i].text = myword[i].ToString();
         }
    }


    public void ResetScene()
    {
        hintIndex = 0;
        for(int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].text = "";
        }

        /*
        optional :
        SceneManager.LoadSceneAsync(0);
        */
    }





}

