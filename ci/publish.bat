echo off

IF [%1]==[] goto noparam

echo "Build image '%1' and 'latest'..."
docker build --progress plain -f ./Dockerfile -t ghcr.io/mylab-task/task-kicker:%1 -t ghcr.io/mylab-task/task-kicker:latest ../src

echo "Publish image '%1' ..."
docker push ghcr.io/mylab-task/task-kicker:%1

echo "Publish image 'latest' ..."
docker push ghcr.io/mylab-task/task-kicker:latest

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"