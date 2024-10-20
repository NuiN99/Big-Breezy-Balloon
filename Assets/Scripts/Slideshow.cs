using UnityEngine;
using UnityEngine.SceneManagement;

public class Slideshow : MonoBehaviour
{
    public string NextScene;
    public Texture[] imageArray;
    private int currentImage = 0;
    
    void OnGUI()
    {
        Rect imageRect = new Rect(0,0,Screen.width,Screen.height);
        GUI.DrawTexture(imageRect, imageArray[currentImage]);
    }
    
    void Start()
    {
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            nextImage();
        }
    }

    void playSound()
    {
        //TODO
    }

    void nextImage()
    {
        currentImage++;
        if(currentImage >= imageArray.Length){
            //CHANGE SCENE
            Debug.Log("END OF SLIDESHOW");
            SceneManager.LoadScene(NextScene);
            currentImage = imageArray.Length -1;
        }
    }
}
