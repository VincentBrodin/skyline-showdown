using UnityEngine;

namespace Scenes.Icon{
    public class ScreenShots : MonoBehaviour{
        public Vector2Int size;
        public new Camera camera;

        private void Start(){
            RenderTexture rt = new(size.x, size.y, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new(size.x, size.y, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(size.x, size.y);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log($"Took screenshot to: {filename}");
        }

        private static string ScreenShotName(int width, int height){
            return
                $"{Application.persistentDataPath}/screen_{width}x{height}_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        }
    }
}