using NuiN.NExtensions;
using UnityEditor;
using UnityEngine;

public class BalloonFace : MonoBehaviour
{
    [SerializeField, InjectComponent] SkinnedMeshRenderer skinnedMeshRenderer;
    private void Awake()
    {
        Texture2D face = new Texture2D(SR_RenderCamera.faceWidth, SR_RenderCamera.faceHeight, TextureFormat.ARGB32, false);
        face.LoadImage(SR_RenderCamera.curFaceBytes);

        skinnedMeshRenderer.material.mainTexture = face;
    }
}
