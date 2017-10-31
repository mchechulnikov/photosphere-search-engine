namespace Photosphere.SearchEngine.IntegrationTests.Resources
{
    internal static class TestTextFiles
    {
        private static readonly string TextsDirectoryPath = $"{RootDirectory}EncodingTests\\";
        public const string RootDirectory = ".\\test-texts\\";

        public static readonly string OneLineFile = $"{RootDirectory}one-line-file.txt";
        public static readonly string TwoLinesFile = $"{RootDirectory}two-lines-file.txt";
        public static readonly string WarAndPeace1 = $"{RootDirectory}war-and-peace-1.txt";
        public static readonly string WarAndPeace2 = $"{RootDirectory}war-and-peace-2.txt";

        public static readonly string Utf8 = $"{TextsDirectoryPath}UTF-8.txt";
        public static readonly string Utf16 = $"{TextsDirectoryPath}UTF-16.txt";
        public static readonly string Utf16Be = $"{TextsDirectoryPath}UTF-16BE.txt";
        public static readonly string Windows1251 = $"{TextsDirectoryPath}Windows-1251.txt";
        public static readonly string Windows1252 = $"{TextsDirectoryPath}Windows-1252.txt";

        public const int OneLineFileWordsCount = 6;
        public const int TwoLinesFileWordsCount = 13;
    }
}