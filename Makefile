publish:
	rm -f ./api/deploy
	dotnet publish ./api/api.csproj -r linux-musl-x64 -p:PublishSingleFile=true -c Release -o ./api/deploy

run:
	dotnet run --project ./api/api.csproj

docker:
	docker build -t mock-server .

dockerrun:
	docker run -p 80:5000 mock-server:latest