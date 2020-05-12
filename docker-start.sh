
pgRunning=`docker ps --filter name=jiggswap-pgsql --format "y"`
rdRunning=`docker ps --filter name=jiggswap-redis --format "y"`

if [ "$pgRunning" == "y" ]
then
	echo "docker: jiggswap-pgsql is already running."
else
	echo "docker: starting jiggswap-pgsql"
	docker run --name jiggswap-pgsql -d --rm -e POSTGRES_PASSWORD=postgres -v $(pwd)/postgres-data:/var/lib/postgresql/data -p 5432:5432 postgres:12.2 
fi

if [ "$rdRunning" == "y" ]
then
	echo "docker: jiggswap-redis is already running."
else
	echo "docker: starting jiggswap-redis"
	docker run --name jiggswap-redis -d --rm -v $(pwd)/redis-data:/data -p 6379:6379 redis:6-buster
fi