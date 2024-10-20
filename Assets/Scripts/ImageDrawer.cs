using UnityEngine;

public class ImageDrawer : MonoBehaviour
{
    private Texture2D Image;
    public int res = 1024;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Image = new Texture2D(res,res,TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.SetTexture("_BaseMap",this.Image);
        clearImage();
    }

    // Update is called once per frame
    void Update()
    {
        // do drawing to image here
    }


    public void clearImage()
    {
        for(int i = 0; i < res; i++)
        {
            for(int j = 0; j < res; j++) {
                this.Image.SetPixel(i,j, Color.white);
            }
        }
        this.Image.Apply();
    }
}
