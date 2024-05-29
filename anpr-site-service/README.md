## Deployment

1. Build and export the image
   Run the [batch script](./build-image-and-export.bat)
2. Configure the environment variable file
   Edit the [file](./docker-compose.env) with any editor of your choice.

| Variables                             | Description                                                                                                                                                                 | Required      | Default Value |
| ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- | ------------- |
| `ENABLE_DEBUG_LOG`                    | Currently not in use.                                                                                                                                                       | true          | `false`       |
| `ENABLE_SIMULATION`                   | Flag to enable vehicle detection simulation. The service will continuously generate combination of different detection, by random, up to the `SIMULATION_COUNT` detections. | false         | `false`       |
| `SIMULATION_COUNT`                    | Maximum number of simulation count.                                                                                                                                         | false         | 0             |
| `SEARCH_WINDOW_IN_MILLISECONDS`       | The size of the search sliding window to match different detections into 1 matched detection.                                                                               | true          | 200           |
| `SITE_ID`                             | The site id of this service. Must exist in web api.                                                                                                                         | true          | -             |
| `MLFF_WEB_API_BASE_URL`               | The url of MLFF web api.                                                                                                                                                    | true          | -             |
| `LOG_OUTPUT_PATH`                     | The path of the analytic log within docker container. Use for analysis purpose.                                                                                             | false         | -             |
| `ENABLE_ANPR_SERVICE`                 | Flag to enable the ANPR service. Purpose of ANPR service is to connect to the MQTT server and receive ANPR detection.                                                       | true          | `false`       |
| `ANPR_ENGINE_TYPE`                    | The ANPR engine type. Currently only `RECOANPR` is available.                                                                                                               | Optional true | -             |
| `ANPR_MQTT_SERVER_IP`                 | IP address of MQTT server.                                                                                                                                                  | Optional true | -             |
| `ANPR_MQTT_SERVER_PORT`               | Port number of MQTT Server.                                                                                                                                                 | Optional true | 1883          |
| `ANPR_MQTT_TOPICS_SEPARATED_BY_COMMA` | MQTT message topic, separated by comma.                                                                                                                                     | Optional true | -             |
| `ANPR_IMAGE_NETWORK_PATH`             | The SAMBA network folder of the ANPR images. Make sure it is accessable.                                                                                                    | Optional true | -             |
| `ANPR_IMAGE_DIRECTORY_USERNAME`       | Username to access the SAMBA network folder.                                                                                                                                | Optional true | -             |
| `ANPR_IMAGE_DIRECTORY_PASSWORD`       | Password to access the SAMBA network folder.                                                                                                                                | Optional true | -             |
| `ENABLE_SPEED_RADAR_SERVICE`          | Flag to enable the speed service.                                                                                                                                           | true          | `false`       |
| `SPEED_RADAR_IP`                      | IP address of speed radar.                                                                                                                                                  | Optional true | -             |
| `SPEED_RADAR_PORT`                    | Port address of speed radar.                                                                                                                                                | Optional true | -             |
| `ENABLE_RFID_SERVICE`                 | Flag to enable RFID service                                                                                                                                                 | true          | `false`       |
| `RFID_READER_IP`                      | IP address of RFID reader                                                                                                                                                   | Optional true | -             |

3. Gather all resources in a folder

- docker-compose.env
- docker-compose.yaml

```yaml
version: "3.9"

services:
  "mlff-site-service":
    container_name: mlff-site-service
    image: mlff-site-service:1.0
    restart: unless-stopped
    privileged: true
    network_mode: host
    env_file:
      - docker-compose.env
    logging:
      options:
        max-size: "1m"
        max-file: "1000"
    volumes:
      - /etc/timezone:/etc/timezone:ro
      - /etc/localtime:/etc/localtime:ro
      - /home/rnd/mlff/site-service/data/logs:/app/logs
```

- mlff-site-service-1.0.tar
- setup.sh

```sh
#!/bin/sh

docker load -i ./mlff-site-service-1.0.tar

docker-compose -f docker-compose.yaml up --no-start
```

4. Compress as ZIP and name it as `site-service.zip`
5. Copy zip file to deployment machine [ip]

```sh
scp ./site-service.zip [user]@[ip]:~/mlff/site-service.zip
```

6. SSH to the deployment machine

```sh
ssh [user]@[ip]
```

7. Deploy using setup.sh script

```sh
chmod +x setup.sh

./setup.sh
```

8. Start the container

```
docker-compose start
```
