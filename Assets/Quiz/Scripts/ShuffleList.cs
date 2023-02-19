using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShuffleList
{
    public static List<E> ShuffleListItems<E>(List<E> inputList)
    {
        List<E> startList = new List<E>();
        startList.AddRange(inputList);
        List<E> randomList = new List<E>();

        System.Random random = new System.Random();
        int randomIndex = 0;
        while (startList.Count > 0)
        {
            randomIndex = random.Next(0, startList.Count);
            randomList.Add(startList[randomIndex]);
            startList.RemoveAt(randomIndex);
        }

        return randomList;
    }
}
