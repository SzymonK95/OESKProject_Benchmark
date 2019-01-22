using System;
using System.Drawing;

namespace Benchmark.Helpers
{
    public static class EncryptionManager
    {
        public static double Encrypt(string message, int rSize, int gSize, int bSize)
        {
            Int64 messagePosition = 0;
            Color pixel = new Color();

            DateTime startTime = DateTime.Now;

            while (messagePosition < message.Length)
            {
                string partOfmessage = DataManager.GetPartOfMessageByPosition(
                    message,
                    (int)messagePosition,
                    rSize + gSize + bSize
                );

                pixel = NewColorUsingMessage(pixel, partOfmessage, rSize, gSize, bSize);

                messagePosition += partOfmessage.Length;
            }

            DateTime stopTime = DateTime.Now;

            return (stopTime - startTime).TotalMilliseconds;
        }

        private static Color NewColorUsingMessage(Color pixel, string message, int rSize, int gSize, int bSize)
        {
            int iiMessage = 0;

            string convertRed = DataManager.ConvertColorValueToString(pixel.R);
            string convertGreen = DataManager.ConvertColorValueToString(pixel.G);
            string convertBlue = DataManager.ConvertColorValueToString(pixel.B);

            char[] baseCRed = new char[convertRed.Length];
            char[] baseCGreen = new char[convertGreen.Length];
            char[] baseCBlue = new char[convertBlue.Length];

            for (int i = 0; i < baseCRed.Length; i++)
            {
                baseCRed[i] = convertRed[i];
                baseCGreen[i] = convertGreen[i];
                baseCBlue[i] = convertBlue[i];
            }

            for (int j = baseCRed.Length - rSize; j < baseCRed.Length; j++)
            {
                if (iiMessage < message.Length)
                {
                    baseCRed[j] = message[iiMessage];
                    iiMessage++;
                }
            }

            for (int j = baseCGreen.Length - gSize; j < baseCGreen.Length; j++)
            {
                if (iiMessage < message.Length)
                {
                    baseCGreen[j] = message[iiMessage];
                    iiMessage++;
                }
            }

            for (int j = baseCBlue.Length - bSize; j < baseCBlue.Length; j++)
            {
                if (iiMessage < message.Length)
                {
                    baseCBlue[j] = message[iiMessage];
                    iiMessage++;
                }
            }

            int newRed = Convert.ToInt32(new string(baseCRed), 2);
            int newGreen = Convert.ToInt32(new string(baseCGreen), 2);
            int newBlue = Convert.ToInt32(new string(baseCBlue), 2);

            return Color.FromArgb(pixel.A, newRed, newGreen, newBlue);
        }
    }
}
