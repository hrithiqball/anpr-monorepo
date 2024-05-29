SET IMAGE_NAME=mlff-site-service
SET TAG_NAME=0.1
docker build -t %IMAGE_NAME%:%TAG_NAME% .
docker save %IMAGE_NAME%:%TAG_NAME% -o ./%IMAGE_NAME%-%TAG_NAME%.tar