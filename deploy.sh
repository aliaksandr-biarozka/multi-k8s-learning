docker build -t aliaksandrbiarozka/multi-react-client:latest - t aliaksandrbiarozka/multi-react-client:$SHA -f ./react-client/Dockerfile ./react-client
docker build -t aliaksandrbiarozka/multi-server:latest -t aliaksandrbiarozka/multi-server:$SHA -f ./server/Dockerfile ./server
docker build -t aliaksandrbiarozka/multi-worker:latest -t aliaksandrbiarozka/multi-worker:$SHA -f ./worker/Dockerfile ./worker

docker push aliaksandrbiarozka/multi-react-client:latest
docker push aliaksandrbiarozka/multi-react-client:$SHA

docker push aliaksandrbiarozka/multi-server:latest
docker push aliaksandrbiarozka/multi-server:$SHA

docker push aliaksandrbiarozka/multi-worker:latest
docker push aliaksandrbiarozka/multi-worker:$SHA

kubectl apply -f simplek8s
kubectl set image deployments/server-deployment server=aliaksandrbiarozka/multi-server:$SHA
kubectl set image deployments/client-deployment client=aliaksandrbiarozka/multi-react-client:$SHA
kubectl set image deployments/worker-deployment worker=aliaksandrbiarozka/multi-worker:$SHA