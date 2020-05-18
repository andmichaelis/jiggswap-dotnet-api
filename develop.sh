RunMigrations=0
StopDocker=0
SeedDatabase=0

for arg in "$@"
do
	case "$arg" in
	-h) echo "develop.sh usage:"
		echo "-h: Show this help text"
		echo "-m: Run Database Migrations"
		echo "-d: Seed Database"
		echo "-s: Stop Docker Containers after Jiggswap.Api"
		exit 0
		;;
	-m)	RunMigrations=1
		echo "-m: migrations will run"
		;;
	-d) SeedDatabase=1
		echo "-d: Database will be seeded"
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

if [ $SeedDatabase -eq 1 ]
then
	echo "Seeding database with dummy data"
	cd ./Jiggswap.DatabaseSeeder
	dotnet run
	cd ..
fi

cd ./Jiggswap.Api
dotnet watch run

if [ $StopDocker -eq 1 ] 
then
	docker stop jiggswap-pgsql
	docker stop jiggswap-redis
fi
