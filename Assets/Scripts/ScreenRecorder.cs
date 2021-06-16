using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

// Screen Recorder will save individual images of active scene in any resolution and of a specific image format
// including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.

// You can compile these images into a video using ffmpeg:
// ffmpeg -i screen_3840x2160_%d.ppm -y test.avi

public class ScreenRecorder : MonoBehaviour
{
    // configure with raw, jpg, png, or ppm (simple raw format)
    public enum Format
    {
        Raw,
        Jpg,
        Png,
        Ppm
    }

    public int captureWidth = 1080;
    public int captureHeight = 1920;

    // optional game object to hide during screenshots (usually your scene canvas hud)
    public GameObject hideGameObject;

    // optimize for many screenshots will not destroy any objects so future screenshots will be fast
    public bool optimizeForManyScreenshots = true;
    public Format format = Format.Png;

    // folder to write output (defaults to data path)
    public string folder;

    // commands
    private bool _captureScreenshot;
    private bool _captureVideo;
    private int _counter; // image #

    // private vars for screenshot
    private Rect _rect;
    private RenderTexture _renderTexture;
    private Texture2D _screenShot;

    private void Update()
    {
        // check keyboard 'k' for one time screenshot capture and holding down 'v' for continious screenshots
        _captureScreenshot |= Input.GetKeyDown("k");
        _captureVideo = Input.GetKey("v");

        if (_captureScreenshot || _captureVideo)
        {
            _captureScreenshot = false;

            // hide optional game object if set
            if (hideGameObject != null) hideGameObject.SetActive(false);

            // create screenshot objects if needed
            if (_renderTexture == null)
            {
                // creates off-screen render texture that can rendered into
                _rect = new Rect(0, 0, captureWidth, captureHeight);
                _renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
                _screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
            }

            // get main camera and manually render scene into rt
            var camera = GetComponent<Camera>(); // NOTE: added because there was no reference to camera in original script; must add this script to Camera
            camera.targetTexture = _renderTexture;
            camera.Render();

            // read pixels will read from the currently active render texture so make our offscreen 
            // render texture active and then read the pixels
            RenderTexture.active = _renderTexture;
            _screenShot.ReadPixels(_rect, 0, 0);

            // reset active camera texture and render texture
            camera.targetTexture = null;
            RenderTexture.active = null;

            // get our unique filename
            var filename = UniqueFilename((int) _rect.width, (int) _rect.height);

            // pull in our file header/data bytes for the specified image format (has to be done from main thread)
            byte[] fileHeader = null;
            byte[] fileData;
            if (format == Format.Raw)
            {
                fileData = _screenShot.GetRawTextureData();
            }
            else if (format == Format.Png)
            {
                fileData = _screenShot.EncodeToPNG();
            }
            else if (format == Format.Jpg)
            {
                fileData = _screenShot.EncodeToJPG();
            }
            else // ppm
            {
                // create a file header for ppm formatted file
                var headerStr = $"P6\n{_rect.width} {_rect.height}\n255\n";
                fileHeader = Encoding.ASCII.GetBytes(headerStr);
                fileData = _screenShot.GetRawTextureData();
            }

            // create new thread to save the image to file (only operation that can be done in background)
            new Thread(() =>
            {
                // create file and write optional header with image bytes
                var f = File.Create(filename);
                if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
                f.Write(fileData, 0, fileData.Length);
                f.Close();
                Debug.Log($"Wrote screenshot {filename} of size {fileData.Length}");
            }).Start();

            // unhide optional game object if set
            if (hideGameObject != null) hideGameObject.SetActive(true);

            // cleanup if needed
            if (optimizeForManyScreenshots == false)
            {
                Destroy(_renderTexture);
                _renderTexture = null;
                _screenShot = null;
            }
        }
    }

    // create a unique filename using a one-up variable
    private string UniqueFilename(int width, int height)
    {
        // if folder not specified by now use a good default
        if (string.IsNullOrEmpty(folder))
        {
            folder = Application.dataPath;
            if (Application.isEditor)
            {
                // put screenshots in folder above asset path so unity doesn't index the files
                var stringPath = folder + "/..";
                folder = Path.GetFullPath(stringPath);
            }

            folder += "/screenshots";

            // make sure directoroy exists
            Directory.CreateDirectory(folder);

            // count number of files of specified format in folder
            var mask = $"screen_{width}x{height}*.{format.ToString().ToLower()}";
            _counter = Directory.GetFiles(folder, mask, SearchOption.TopDirectoryOnly).Length;
        }

        // use width, height, and counter for unique file name
        var filename = $"{folder}/screen_{width}x{height}_{_counter}.{format.ToString().ToLower()}";

        // up counter for next call
        ++_counter;

        // return unique filename
        return filename;
    }

    public void CaptureScreenshot()
    {
        _captureScreenshot = true;
    }
}