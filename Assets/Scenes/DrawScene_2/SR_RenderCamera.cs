using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SR_RenderCamera : MonoBehaviour {

    private int FileCounter = 0;

    Camera Cam;
    RenderTexture currentRT;
    public int res = 1024;
    private static String facesPath = Application.dataPath + "/Drawings/Faces/";
    static DirectoryInfo dir = new DirectoryInfo(facesPath);

    void Start()
    {
        Cam = GetComponent<Camera>();
        currentRT = RenderTexture.active;
        FileCounter = dir.GetFiles("*.png").Length; // find how many files are in the folder first.
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CamCapture();   
            // TODO:
            // WAIT FOR IMAGE TO SAVE,
            // GO TO NEXT SCENE (level1?)
        }
    }

    //UNUSED
    void clearImage(Texture2D Image)
    {
        Color transparent = new(0f,0f,0f,0f);
        for(int i = 0; i < res; i++)
        {
            for(int j = 0; j < res; j++) {
                Image.SetPixel(i,j,transparent);
            }
        }
        Image.Apply();
    }

    // deletes any pixel that isnt black from the screen shot
    void scrubImage(Texture2D Image)
    {
        Color currPixel;
        Color transparent = new(0f,0f,0f,0f);
        for(int i = 0; i < res; i++) //because nothing was simpler than a nested for loop
        {
            for(int j = 0; j < res; j++) {
                currPixel = Image.GetPixel(i,j);
                if(!currPixel.Equals(Color.black)) {
                    Image.SetPixel(i,j,transparent);
                }
            }
        }
        Image.Apply();
    }

    void CamCapture()
    {
        Cam.Render();
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        RenderTexture.active = Cam.targetTexture;
        
        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
        sw.Start();
        // read camera capture
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        UnityEngine.Debug.Log($"<got camera pixels> elapsed: {sw.Elapsed}");
        
        // delete any non-black pixels
        scrubImage(Image);
        UnityEngine.Debug.Log($"<scrubed image> elapsed: {sw.Elapsed}");
        RenderTexture.active = currentRT;
        
        // encode to save
        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        // save image
        File.WriteAllBytes(facesPath + FileCounter + ".png", Bytes);
        UnityEngine.Debug.Log($"<saved image in faces> elapsed: {sw.Elapsed}");
        File.WriteAllBytes(Application.dataPath + "/Drawings/currFace.png", Bytes);
        UnityEngine.Debug.Log($"<saved image as currFace> elapsed: {sw.Elapsed}");
        FileCounter++;
    }
   
}