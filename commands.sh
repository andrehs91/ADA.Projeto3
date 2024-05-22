# ------------------------------------------------------------------------------

docker build --no-cache -f ADA.Consumer/Dockerfile -t andrehs/ada.consumer .
docker push andrehs/ada.consumer

# ------------------------------------------------------------------------------

docker build --no-cache -f ADA.Producer/Dockerfile -t andrehs/ada.producer .
docker push andrehs/ada.producer

# ------------------------------------------------------------------------------