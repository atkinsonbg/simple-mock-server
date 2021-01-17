FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1.0-alpine3.10
WORKDIR /api
COPY ./api/deploy/api api
ENV ASPNETCORE_URLS=http://+:5000
CMD ["./api"]