using QRCoder;

namespace SmartCity.Domain.Utils
{
    public static class QRCodeUtils
    {
        public static byte[] GenerateQR(string data)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                var pngByteQRCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = pngByteQRCode.GetGraphic(12);
                return qrCodeBytes;
            }
        }

    }
}
