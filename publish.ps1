# build & compress
mkdir publish
echo 'building Jiggswap.Api...'
dotnet publish ./Jiggswap.Api -c Release -o ./publish/Api -v q
echo 'building Jiggswap.Migrations...'
dotnet publish ./Jiggswap.Migrations -c Release -o ./publish/Migrations -v q

# copy appsettings
echo 'copying appsettings...'
xcopy .\config\appsettings.api.json .\publish\Api\appsettings.json /Y /q
xcopy .\config\appsettings.migrations.json .\publish\Migrations\appsettings.json /Y /q

Compress-Archive ./publish/* build.zip -F

# copy build to server
echo 'copying build to remote...'
scp -q -o LogLevel=QUIET build.zip root@api.jiggswap.com:~/
echo 'unzipping archive...'
ssh root@api.jiggswap.com "rm -rf /root/api.jiggswap.com; unzip -q -o build.zip -d /root/api.jiggswap.com; rm build.zip"

#REM run migrations
echo 'running migrations...'
ssh root@api.jiggswap.com "dotnet /root/api.jiggswap.com/Migrations/Jiggswap.Migrations.dll"

# copy nginx config
echo 'copying nginx config...'
scp -q -o LogLevel=QUIET ./config/nginx.conf root@api.jiggswap.com:/etc/nginx/sites-available/api.jiggswap.com
ssh root@api.jiggswap.com ln -s -f /etc/nginx/sites-available/api.jiggswap.com /etc/nginx/sites-enabled/

# restart api
echo 'restarting api...'
scp -q -o LogLevel=QUIET ./config/api.jiggswap.com.service root@api.jiggswap.com:/etc/systemd/system/api.jiggswap.com.service
ssh root@api.jiggswap.com "systemctl daemon-reload"
ssh root@api.jiggswap.com "systemctl enable api.jiggswap.com"
ssh root@api.jiggswap.com "systemctl stop api.jiggswap.com"
ssh root@api.jiggswap.com "systemctl start api.jiggswap.com"

#cleanup
echo 'cleaning up...'
rm build.zip
rm -r -fo ./publish

echo 'publish finished.'