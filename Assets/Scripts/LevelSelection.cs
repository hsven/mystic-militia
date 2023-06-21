using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
   [SerializeField] private bool unlocked;
   public Image unlockImage;
   public Image oldButtonImage;
   public Sprite buttonImage1;
   public Sprite buttonImage2;

    private void Update(){
        updateLevelImage();
    }

   private void updateLevelImage(){
    if(!unlocked){
        unlockImage.gameObject.SetActive(true);
        oldButtonImage.sprite = buttonImage2;
    }
    else{
        unlockImage.gameObject.SetActive(false);
        oldButtonImage.sprite = buttonImage1;
    }
   }

   public void PressSeleciton(string _levelName){
    if(unlocked){
        SceneManager.LoadScene(_levelName);
    }
   }
}
