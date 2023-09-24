namespace AspNetCoreDemo.Controllers
{
	public static class Constants
	{
		public const string FileSelectionError = "Please select a .txt file for upload.";
		public const string FileUploadError = "Error processing the file: ";
		
		public const string FileUploadSuccessful = "File uploaded successfully.";
		public const string GetAllSuccessful = "Got all Swift records successfully.";
		
		public const string SelectFileLable = "Select a .txt file:";
		
		public const string Index = "Index";
		public const string Error = "Error";
		
		public const string ErrorMessage = "ErrorMessage";
		
		public const string FormatFile = ".txt";
		public const string FormatDate = "yyyyMMddHHmmssfff";
		
		public const string VisitedHomeIndexViewLog = "Visited Home/Index";
		public const string VisitedSwiftIndexViewLog = "Visited Swift/Index";
        public const string FileSelectionErrorLog = "Selected a wrong type file.";
        public const string NotFileSelectedErrorLog = "Tried to upload without selecting a file.";
        public const string NoRecordsErrorLog = "No database entries are shown.";
    }
}
