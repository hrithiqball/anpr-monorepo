namespace MlffSiteService.Models;

public class Constants
{
    public static class EnvironmentVariables
    {
        public const string ENABLE_DEBUG_LOG = "ENABLE_DEBUG_LOG";
        public const string ENABLE_SIMULATION = "ENABLE_SIMULATION";
        public const string SITE_ID = "SITE_ID";
        public const string MLFF_WEB_API_BASE_URL = "MLFF_WEB_API_BASE_URL";
        public const string SEARCH_WINDOW_IN_MILLISECONDS = "SEARCH_WINDOW_IN_MILLISECONDS";
        public const string LOG_OUTPUT_PATH = "LOG_OUTPUT_PATH";

        public const string ENABLE_ANPR_SERVICE = "ENABLE_ANPR_SERVICE";
        public const string ENABLE_POST_ANPR = "ENABLE_POST_ANPR";
        public const string ANPR_ENGINE_TYPE = "ANPR_ENGINE_TYPE";
        public const string ANPR_MQTT_SERVER_IP = "ANPR_MQTT_SERVER_IP";
        public const string ANPR_MQTT_SERVER_PORT = "ANPR_MQTT_SERVER_PORT";
        public const string ANPR_MQTT_TOPICS_SEPARATED_BY_COMMA = "ANPR_MQTT_TOPICS_SEPARATED_BY_COMMA";
        public const string ANPR_IMAGE_NETWORK_PATH = "ANPR_IMAGE_NETWORK_PATH";
        public const string ANPR_IMAGE_MOUNTED_NETWORK_PATH = "ANPR_IMAGE_MOUNTED_NETWORK_PATH";
        public const string ANPR_IMAGE_DIRECTORY_USERNAME = "ANPR_IMAGE_DIRECTORY_USERNAME";
        public const string ANPR_IMAGE_DIRECTORY_PASSWORD = "ANPR_IMAGE_DIRECTORY_PASSWORD";

        public const string ENABLE_SPEED_RADAR_SERVICE = "ENABLE_SPEED_RADAR_SERVICE";
        public const string ENABLE_POST_SPEED = "ENABLE_POST_SPEED";
        public const string SPEED_RADAR_IP = "SPEED_RADAR_IP";
        public const string SPEED_RADAR_PORT = "SPEED_RADAR_PORT";

        public const string ENABLE_RFID_SERVICE = "ENABLE_RFID_SERVICE";
        public const string ENABLE_POST_RFID = "ENABLE_POST_RFID";
        public const string RFID_READER_IP = "RFID_READER_IP";
        public const string READER_MODE = "READER_MODE";

        public const string ENABLE_LANE_1 = "ENABLE_LANE_1";
        public const string ENABLE_LANE_2 = "ENABLE_LANE_2";
        public const string PRESENCE_RADAR_LANE_1 = "PRESENCE_RADAR_LANE_1";
        public const string PRESENCE_RADAR_LANE_2 = "PRESENCE_RADAR_LANE_2";
    }

    public static class AnprEngineTypes
    {
        public const string RECOANPR = "RECOANPR";
    }
}