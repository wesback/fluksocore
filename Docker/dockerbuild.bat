docker build --rm --tag fluksocore --file Dockerfile.arm .
docker tag fluksocore wesback/fluksocore
docker push wesback/fluksocore
