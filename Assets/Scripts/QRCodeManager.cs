using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QRCodeManager : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImageReceiver, _rawImageBackground;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private RectTransform _scanZone;
    [SerializeField]
    private ServerManager _serverManager;
    
    private Texture2D _storeEncodedTexture;
    private bool _isCamAvailable;
    private WebCamTexture _cameraTexture;

    public string codeScanned;

    public void InitCamera()
    {
        SetUpCamera();
    }

    public void InitQRCode()
    {
        _storeEncodedTexture  = new Texture2D(256, 256);
        EncodeTextToQRCode(_serverManager.codeForQR);
    }

    void Update()
    {
        UpdateCameraRender();
        Scan();
    }

    private Color32[] Encode(string textForEncoding, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions()
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public void OnReceiveEncode(string textToEncode)
    {
        EncodeTextToQRCode(textToEncode);
    }

    private void EncodeTextToQRCode(string textForEncoding = "")
    {
        string textWrite = string.IsNullOrEmpty(textForEncoding) ? "You should write something" : textForEncoding;
        
        Color32[] _convertPixelToTexture = Encode(textWrite, _storeEncodedTexture.width, _storeEncodedTexture.height);
        _storeEncodedTexture.SetPixels32(_convertPixelToTexture);
        _storeEncodedTexture.Apply();
        
        _rawImageReceiver.texture = _storeEncodedTexture;
    }

    public void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            _isCamAvailable = false;
            return;
        }

        for (int i = 0; i< devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                _cameraTexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            }
        }
        
        _cameraTexture.Play();
        _rawImageBackground.texture = _cameraTexture;
        _isCamAvailable = true;
    }

    private void UpdateCameraRender()
    {
        if (_isCamAvailable == false)
        {
            return;
        }
        float ratio = (float)_cameraTexture.width / (float)_cameraTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;
        
        int orientation = -_cameraTexture.videoRotationAngle;
        _rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width, _cameraTexture.height);
            if (result != null)
            {
                codeScanned = result.Text;
                _serverManager.StartClient(true);
            }
        }
        catch
        {
            return;
        }
    }
}
