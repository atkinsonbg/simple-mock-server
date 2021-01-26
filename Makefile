publish:
	rm -R -f ./api/deploy
	dotnet publish ./api/api.csproj -r linux-musl-x64 -p:PublishSingleFile=true /p:PublishTrimmed=true -c Release -o ./api/deploy

run:
	dotnet run --project ./api/api.csproj

docker:	publish
	docker build -t atkinsonbg/mock-server:latest .

dockerrun:
	docker run -p 5000:5000 atkinsonbg/mock-server:latest

retag:
	docker tag atkinsonbg/mock-server:latest atkinsonbg/mock-server:0.1

push:
	docker push atkinsonbg/mock-server:0.1

tests:
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=../lcov ./test/test.csproj

coverage:
	coverlet ./test/bin/Debug/netcoreapp3.1/test.dll --target "dotnet" --targetargs "test ./test/test.csproj --no-build"