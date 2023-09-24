namespace AspNetCoreDemo.Services
{
    public static class ServicesConstants
    {
        public const string ReadingFileErrorMessage = "Error reading the file.";
        public const string MatchErrorMessage = "Match is not successful.";
        public const string CreateErrorMessage = "Data is not saved successful.";
        public const string NoRecordsFoundMessage = "No records found for now.";

        public const string BlockNumber = "blockNumber";
        public const string BlockContent = "blockContent";

        public const string BlockNumber1 = "1";
        public const string BlockNumber2 = "2";
        public const string BlockNumber3 = "3";

        public const string FieldCode20 = "20";
        public const string fieldCode21 = "21";

        public const string TextBlockStartsWith = ":79:";
        public const string TextBlockFinishWith = "-}{5:";
        
        public const string BlockPattern = @"\{(?<blockNumber>\d+):(?<blockContent>.*?)\}";
        public const string TextBlockPattern = @":\d+:[^:]+";
        public const string TrailerBlockPattern = @"{MAC:(.*?)\}\{CHK:(.*?)\}}";


    }
}
