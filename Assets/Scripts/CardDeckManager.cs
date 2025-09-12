using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeckManager : MonoBehaviour
{
    #region Singleton
    private static CardDeckManager instance;

    public static CardDeckManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<CardDeckManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new(typeof(CardDeckManager).Name);
                    instance = singletonObject.AddComponent<CardDeckManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }
    #endregion

    
}
