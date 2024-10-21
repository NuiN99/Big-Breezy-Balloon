using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Slideshow : MonoBehaviour
{
    public string NextScene;
    public Texture[] imageArray;
    private int currentImage = 0;
    private GUIStyle mystyle = new GUIStyle();
    
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        Rect imageRect = new Rect(0,0,Screen.width,Screen.height);
        GUI.DrawTexture(imageRect, imageArray[currentImage]);
        mystyle.fontSize = 24;
        GUI.Label(new Rect(w-200, h-20, w+100, h-10), "Click to Advance!", mystyle);
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
