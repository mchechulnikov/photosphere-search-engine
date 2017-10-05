namespace Jbta.SearchEngine.IntegrationTests.Resources
{
    internal static class TestTextFiles
    {
        private const string RootDirectory = ".\\test-texts\\";
        private static readonly string TextsDirectoryPath = $"{RootDirectory}EncodingTests\\";

        // Common
        public static readonly string OneLineFile = $"{RootDirectory}one-line-file.txt";
        public static readonly string TwoLinesFile = $"{RootDirectory}two-lines-file.txt";
        public static readonly string WarAndPeace1 = $"{RootDirectory}Толстой Л. - Война и Мир ч.1.txt";
        public static readonly string WarAndPeace2 = $"{RootDirectory}Толстой Л. - Война и Мир ч.2.txt";

        // Encoding
        public static readonly string Utf8 = $"{TextsDirectoryPath}UTF-8.txt";
        public static readonly string Utf16 = $"{TextsDirectoryPath}UTF-16.txt";
        public static readonly string Utf16Be = $"{TextsDirectoryPath}UTF-16BE.txt";
        public static readonly string Windows1251 = $"{TextsDirectoryPath}Windows-1251.txt";
        public static readonly string Windows1252 = $"{TextsDirectoryPath}Windows-1252.txt";

        public const int OneLineFileWordsCount = 6;
        public const int TwoLinesFileWordsCount = 13;
    }
}