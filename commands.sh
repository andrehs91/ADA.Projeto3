# ------------------------------------------------------------------------------

docker build -f ADA.Consumer/Dockerfile -t andrehs/ada.consumer .
docker push andrehs/ada.consumer

# ------------------------------------------------------------------------------

docker build -f ADA.Producer/Dockerfile -t andrehs/ada.producer .
docker push andrehs/ada.producer

# ------------------------------------------------------------------------------