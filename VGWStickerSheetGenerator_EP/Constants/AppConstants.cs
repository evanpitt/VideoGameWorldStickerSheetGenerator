namespace VGWStickerSheetGenerator_EP.Constants
{
    public static class AppConstants
    {
        public const string LOG_DATE_FORMAT = "MM/dd/yyyy HH:mm:ss";
        public const string OUTPUT_FILE_DATE_FORMAT = "MM-dd-yyyy HH-mm-ss";
        public const string APP_VERSION = "v 1.0";
    }
    public static class AppFlowStates
    {
        public const string RUN_STARTED = "1: Generator starting";
        public const string LOADING_INPUT_FILE = "2: Loading input file";
        public const string INPUT_FILE_LOADED_SUCCESS = "3: Input File loaded successfully";
        public const string READING_INPUT_FILE = "4: Reading Input file";
        public const string INPUT_FILE_READ_SUCCESS = "5: Input file read successfully";
        public const string CREATE_STICKERS_STARTING = "6: Creating sticker data";
        public const string STICKERS_CALCULATED = "7: Total Stickers Calculated";
        public const string FINISHED_CREATING_STICKERS = "8: Finished creating Stickers";
        public const string EXPORTING_RESULTS = "9: Exporting Stickers to output file";
        public const string EXPORTING_RESULTS_SUCCESS = "10: Sticker Output file exported successfully";
        public const string APPLICATION_FINISHED_SUCCESSFULLY = "Application Finished Successfully";
    }
    public static class ErrorMessages
    {
        public const string NULL_INPUT_FILE = "Dude, you need to select an input file that you want me to process...";
        public const string NULL_OUTPUT_LOCATION = "Dude, you need to tell me where you want the result output file to go...";
        public const string EXCEPTION = "A Serious error occurred, inspect the log for further information";
    }
}