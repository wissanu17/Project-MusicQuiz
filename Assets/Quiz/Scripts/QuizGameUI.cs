using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class QuizGameUI : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private QuizManager quizManager;
    [SerializeField] private CategoryBtnScript categoryBtnPrefab;
    [SerializeField] private GameObject scrollHolder;
    [SerializeField] private Text scoreText, timerText;
    [SerializeField] private List<Image> lifeImageList;
    [SerializeField] private GameObject gameOverPanel, mainMenu, gamePanel;
    [SerializeField] private Color correctCol, wrongCol, normalCol; //color of buttons
    [SerializeField] private Image questionImg; //image of each question to show image
    [SerializeField] private UnityEngine.Video.VideoPlayer questionVideo;   //to show video
    [SerializeField] private AudioSource questionAudio;             //audio source for audio clip
    [SerializeField] private Text questionInfoText, questionHeadText; //text to show question
    [SerializeField] private List<Button> options; //options button follow the question list
#pragma warning restore 649

    private float audioLength;          //store audio length
    private Question question;          //store current question data
    private bool answered = false;      //bool to keep track if answered or not

    public Text TimerText { get => timerText; }
    public Text ScoreText { get => scoreText; }
    public GameObject GameOverPanel { get => gameOverPanel; }

    private void Start()
    {
        //add the listner to all the buttons
        for (int i = 0; i < options.Count; i++)
        {
            Button localBtn = options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

        CreateCategoryButtons();

    }
    /// <summary>
    /// Method which populate the question on the screen
    /// </summary>
    /// <param name="question"></param>
    public void SetQuestion(Question question)
    {
        //set the question
        this.question = question;
        //check for questionType
        switch (question.questionType)
        {
            case QuestionType.TEXT:
                questionImg.transform.parent.gameObject.SetActive(false);
                questionHeadText.transform.gameObject.SetActive(true);
                questionInfoText.transform.gameObject.SetActive(true);
                break;
            case QuestionType.IMAGE:
                questionImg.transform.parent.gameObject.SetActive(true);
                questionVideo.transform.gameObject.SetActive(false);
                questionImg.transform.gameObject.SetActive(true);
                questionAudio.transform.gameObject.SetActive(false);
                questionImg.sprite = question.questionImage;
                break;
            case QuestionType.AUDIO:
                questionVideo.transform.parent.gameObject.SetActive(true); 
                questionVideo.transform.gameObject.SetActive(false);
                questionImg.transform.gameObject.SetActive(false);
                questionAudio.transform.gameObject.SetActive(true);
                questionHeadText.transform.gameObject.SetActive(true);
                questionInfoText.transform.gameObject.SetActive(false);
                audioLength = question.audioClip.length;
                StartCoroutine(PlayAudio());
                break;
            case QuestionType.VIDEO: //Craete for use in future
                questionVideo.transform.parent.gameObject.SetActive(true);
                questionVideo.transform.gameObject.SetActive(true);
                questionImg.transform.gameObject.SetActive(false);
                questionAudio.transform.gameObject.SetActive(false);

                questionVideo.clip = question.videoClip;
                questionVideo.Play();
                break;
        }

        questionInfoText.text = question.questionInfo;
        questionHeadText.text = question.questionHead;

        //suffle the list of options
        List<string> ansOptions = ShuffleList.ShuffleListItems<string>(question.options);

        //assign options to respective option buttons
        for (int i = 0; i < options.Count; i++)
        {
            //set the option text
            options[i].GetComponentInChildren<Text>().text = ansOptions[i];
            options[i].name = ansOptions[i];
            options[i].image.color = normalCol; //set color of button to normal color every question
        }

        answered = false;                       

    }

    public void ReduceLife(int remainingLife)
    {
        lifeImageList[remainingLife].color = Color.gray;
    }

    /// <summary>
    /// IEnumerator to repeate the audio
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayAudio()
    {
        //if questionType is audio
        if (question.questionType == QuestionType.AUDIO)
        {
            questionAudio.PlayOneShot(question.audioClip);
            yield return new WaitForSeconds(audioLength + 0.5f);
            StartCoroutine(PlayAudio());
        }
        else //if questionType is not audio
        {
            //stop the Coroutine
            StopCoroutine(PlayAudio());
            yield return null;
        }
    }

    /// <summary>
    /// Method assigned to the buttons
    /// </summary>
    /// <param name="btn">ref to the button object</param>
    void OnClick(Button btn)
    {
        if (quizManager.GameStatus == GameStatus.PLAYING)
        {
            //if answered is false
            if (!answered)
            {
                //set answered true
                answered = true;
                //get the bool value
                bool val = quizManager.Answer(btn.name);

                //if its true
                if (val)
                {
                    //set color to correct
                    //btn.image.color = correctCol;
                    StartCoroutine(BlinkImg(btn.image));
                }
                else
                {
                    //set it to wrong color
                    btn.image.color = wrongCol;
                }
            }
        }
    }

    /// <summary>
    /// Method to create Category(option) Buttons
    /// </summary>
    void CreateCategoryButtons()
    {
        //loop catgories in QuizManager
        for (int i = 0; i < quizManager.QuizData.Count; i++)
        {
            //Create new CategoryBtn
            CategoryBtnScript categoryBtn = Instantiate(categoryBtnPrefab, scrollHolder.transform);
            //Set the button by index of question data
            categoryBtn.SetButton(quizManager.QuizData[i].categoryName, quizManager.QuizData[i].questions.Count);
            int index = i;
            //Add listner to button which calls CategoryBtn method
            categoryBtn.Btn.onClick.AddListener(() => CategoryBtn(index, quizManager.QuizData[index].categoryName));
        }
    }

    //Method called by Category Button
    private void CategoryBtn(int index, string category)
    {
        quizManager.StartGame(index, category); //start the game 
        mainMenu.SetActive(false);              //deactivate mainMenu
        gamePanel.SetActive(true);              //activate game panel
    }

    //this give blink effect [if needed use or dont use]
    IEnumerator BlinkImg(Image img)
    {
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            img.color = correctCol;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RestryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
