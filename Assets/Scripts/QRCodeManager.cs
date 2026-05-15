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
    private TMP_InputField _textInputField;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private TextMeshProUGUI _textOut;
    [SerializeField]
    private RectTransform _scanZone;
    
    private Texture2D _storeEncodedTexture;
    private bool _isCamAvailable;
    private WebCamTexture _cameraTexture;
    
    void Start()
    {
        _storeEncodedTexture  = new Texture2D(256, 256);
        SetUpCamera();
    }

    void Update()
    {
        UpdateCameraRender();
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

    public void OnClickEncode()
    {
        EncodeTextToQRCode();
    }

    private void EncodeTextToQRCode()
    {
        string textWrite = string.IsNullOrEmpty(_textInputField.text) ? "You should write something" : _textInputField.text;
        
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
        _rawImageReceiver.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    public void OnClickScan()
    {
        Scan();
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width, _cameraTexture.height);
            if (result != null)
            {
                _textOut.text = result.Text;
            }
            else
            {
                _textOut.text = "Failed to read QR code";
            }
        }
        catch
        {
            _textOut.text = "Failed to try";
        }
    }
}
