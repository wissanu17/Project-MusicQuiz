using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    //ref to the QuizGameUI script
    [SerializeField] private QuizGameUI quizGameUI;
    //ref to the scriptableobject file
    [SerializeField] private List<QuizDataScriptable> quizDataList;
    [SerializeField] private float timeInSeconds;
#pragma warning restore 649

    private string currentCategory = "";
    private int correctAnswerCount = 0;
    //questions data
    private List<Question> questions;
    //current question data
    private Question selectedQuetion = new Question();
    private int gameScore;
    private int lifesRemaining;
    private float currentTime;
    private QuizDataScriptable dataScriptable;

    private GameStatus gameStatus = GameStatus.NEXT;

    public GameStatus GameStatus { get { return gameStatus; } }

    public List<QuizDataScriptable> QuizData { get => quizDataList; }

    public void StartGame(int categoryIndex, string category)
    {
        currentCategory = category;
        correctAnswerCount = 0;
        gameScore = 0;
        lifesRemaining = 3;
        currentTime = timeInSeconds;
        //set the questions data
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions);
        //select the question
        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
    }

    /// <summary>
    /// Method used to randomly select the question form questions data
    /// </summary>
    private void SelectQuestion()
    {
        //get the random number
        int val = UnityEngine.Random.Range(0, questions.Count);
        //set the selectedQuetion
        selectedQuetion = questions[val];
        //send the question to quizGameUI
        quizGameUI.SetQuestion(selectedQuetion);

        questions.RemoveAt(val);
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            currentTime -= Time.deltaTime;
            SetTime(currentTime);
        }
    }

    void SetTime(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        quizGameUI.TimerText.text = time.ToString("mm':'ss");

        if (currentTime <= 0)
        {
            GameEnd();
        }
    }

    /// <summary>
    /// Method called to check the answer is correct or not
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        //set default to false
        bool correct = false;
        //if selected answer is similar to the correctAns
        if (selectedQuetion.correctAns == selectedOption)
        {
            //Yes, Ans is correct
            correctAnswerCount++;
            correct = true;
            gameScore += 1;
            quizGameUI.ScoreText.text = "Score : " + gameScore;
            currentTime+=10;
        }
        else
        {
            //No, Ans is wrong
            //Reduce Life
            lifesRemaining--;
            quizGameUI.ReduceLife(lifesRemaining);

            if (lifesRemaining == 0)
            {
                GameEnd();
            }
        }

        if (gameStatus == GameStatus.PLAYING)
        {
            if (questions.Count > 0)
            {
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
        //return the value of correct bool
        return correct;
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.GameOverPanel.SetActive(true);
        
        //save the highest score
        if (correctAnswerCount > PlayerPrefs.GetInt(currentCategory)) 
        {
            PlayerPrefs.SetInt(currentCategory, correctAnswerCount);
        }
    }
}

//Datastructure for storeing the quetions data
[System.Serializable]
public class Question
{
    public string questionInfo, questionHead;//question info and head text
    public QuestionType questionType;
    public Sprite questionImage;//image for Image Type may be use in next version
    public AudioClip audioClip;//audio for audio type 
    public UnityEngine.Video.VideoClip videoClip;   //video for video type
    public List<string> options;//options can select
    public string correctAns;
}

[System.Serializable]
public enum QuestionType //Type of quiz
{
    TEXT,
    IMAGE,
    AUDIO,
    VIDEO
}

[SerializeField]
public enum GameStatus
{
    PLAYING,
    NEXT
}