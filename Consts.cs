namespace GZipTestApp
{
    public static class Consts
    {
        public const int GzipHeaderWithExtraFieldLength = GzipHeaderLength + ExtraFieldLength;
        public const int FlagsHeaderPosition = 3;
        public const int ExtraFlagBitPosition = 2;
        public const int GzipHeaderLength = 10;
        public const int ExtraFieldLength = 10;
        public const int FlagsValue = 1 << ExtraFlagBitPosition;
    }
}
