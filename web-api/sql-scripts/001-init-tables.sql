CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- #region site 
CREATE TABLE IF NOT EXISTS "site"
(
    "id"               text      NOT NULL PRIMARY KEY,
    "location_name"    text,
    "latitude"         numeric,
    "longitude"        numeric,
    "kilometer_marker" numeric,
    "created_by"       text,
    "date_created"     timestamp NOT NULL,
    "modified_by"      text,
    "date_modified"    timestamp NOT NULL
);
-- #endregion 

--#region detection
CREATE TABLE IF NOT EXISTS speed_detection
(
    "uid"            uuid      NOT NULL PRIMARY KEY,
    "site_id"        text      NOT NULL,
    "speed_kmh"      numeric   NOT NULL,
    "date_detection" timestamp NOT NULL,
    "date_created"   timestamp NOT NULL
);

CREATE INDEX IF NOT EXISTS speed_detection_detection_date
    on speed_detection (date_detection desc);
CREATE INDEX IF NOT EXISTS speed_detection_speed_kmh
    on speed_detection (speed_kmh);

CREATE TABLE IF NOT EXISTS "license_plate_recognition"
(
    "uid"                uuid      NOT NULL PRIMARY KEY,
    "site_id"            text      NOT NULL,
    "vehicle_image_path" text,
    "plate_image_path"   text,
    "plate_number"       text      NOT NULL,
    "bbox_top"           integer,
    "bbox_left"          integer,
    "bbox_height"        integer,
    "bbox_width"         integer,
    "confidence_lpd"     numeric,
    "confidence_ocr"     numeric,
    "date_detection"     timestamp NOT NULL,
    "date_created"       timestamp NOT NULL
);

CREATE INDEX IF NOT EXISTS license_plate_recognition_detection_date
    on license_plate_recognition (date_detection desc);
CREATE INDEX IF NOT EXISTS license_plate_recognition_plate_number
    on license_plate_recognition (plate_number);

CREATE TABLE IF NOT EXISTS rfid_detection
(
    "uid"            uuid      NOT NULL PRIMARY KEY,
    "tag_id"         text      NOT NULL,
    "site_id"        text      NOT NULL,
    "date_detection" timestamp NOT NULL,
    "date_created"   timestamp NOT NULL
);

CREATE INDEX IF NOT EXISTS rfid_detection_detection_date
    on rfid_detection (date_detection desc);
CREATE INDEX IF NOT EXISTS rfid_detection_tag_id
    on rfid_detection (tag_id);
--#endregion

--#region match
CREATE TABLE IF NOT EXISTS detection_match
(
    "uid"            uuid      NOT NULL PRIMARY KEY,
    "site_id"        text      NOT NULL,
    "tag_id"         text,
    "plate_number"   text,
    "speed"          integer,
    "verified"       boolean   NOT NULL DEFAULT false,
    "correctness"    boolean,
    "date_matched" timestamp NOT NULL,
    "date_created"   timestamp NOT NULL
);
CREATE INDEX IF NOT EXISTS detection_match_site_id
    on detection_match (site_id);
CREATE INDEX IF NOT EXISTS detection_match_tag_id
    on detection_match (tag_id);
CREATE INDEX IF NOT EXISTS detection_match_speed_id
    on detection_match (speed);
CREATE INDEX IF NOT EXISTS detection_match_verified
    on detection_match (verified);
CREATE INDEX IF NOT EXISTS detection_match_correctness
    on detection_match (correctness);
--#endregion match

--#region watchlist
CREATE TABLE IF NOT EXISTS "watchlist"
(
    "uid"            uuid      NOT NULL PRIMARY KEY,
    "monitor_option" integer   NOT NULL,
    "value"          text      NOT NULL,
    "created_by"     text,
    "date_created"   timestamp NOT NULL,
    "modified_by"    text,
    "date_modified"  timestamp NOT NULL,
    "remarks"        text,
    "tag_color"      text,
);
--#endregion 


INSERT INTO site(id, location_name, created_by, date_created, modified_by, date_modified)
VALUES ('WCE-POC-00001', 'POC-LOCATION', 'EDMUND', CURRENT_TIMESTAMP, 'EDMUND', CURRENT_TIMESTAMP);