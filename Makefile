publish:
	rm -R -f ./api/deploy
	dotnet publish ./api/api.csproj -r linux-musl-x64 -p:PublishSingleFile=true -c Release -o ./api/deploy

run:
	dotnet run --project ./api/api.csproj

docker:	publish
	docker build -t atkinsonbg/mock-server .

dockerrun:
	docker run -p 5000:5000 atkinsonbg/mock-server:latest