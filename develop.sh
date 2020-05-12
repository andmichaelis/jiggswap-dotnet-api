RunMigrations=0
StopDocker=0

for arg in "$@"
do
	case "$arg" in
	-h) echo "develop.sh usage:"
		echo "-s: Stop Docker Containers after Jiggswap.Api"
		echo "-h: Show this help text"
		exit 0
		;;
	-m)	RunMigrations=1
		echo "-m: migrations will run"
		;;
	-s) StopDocker=1
		echo "-s: docker will stop containers when dotnet ends"
		;;
	esac
done

bash ./docker-start.sh

if [ $RunMigrations -eq 1 ] 
then
	echo "running migrations"

	cd ./Jiggswap.Migrations
	migrationstatus=$(dotnet run)
	if [ $? -eq 127 ]
	then
		echo "exiting ./develop.sh"
		exit 127
	fi
	cd ..
fi

cd ./Jiggswap.Api
dotnet watch run

if [ $StopDocker -eq 1 ] 
then
	docker stop jiggswap-pgsql
	docker stop jiggswap-redis
fi
