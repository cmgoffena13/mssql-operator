.PHONY: help build run test clean setup docker-up docker-down

help:
	@echo "Available commands:"
	@echo "  build      - Build the project"
	@echo "  run        - Run the project"
	@echo "  clean      - Clean build artifacts"
	@echo "  setup      - Setup project (restore packages)"
	@echo "  docker-up  - Start SQL Server in Docker"
	@echo "  docker-down- Stop Docker containers"
	@echo "  dev        - Start SQL Server and run project"

build:
	dotnet build src/MssqlOperator/

run:
	dotnet run --project src/MssqlOperator/

clean:
	dotnet clean src/MssqlOperator/

setup:
	dotnet restore src/MssqlOperator/

docker-up:
	docker-compose up sqlserver -d

docker-down:
	docker-compose down

dev: docker-up
	@echo "Waiting for SQL Server to start..."
	@sleep 5
	dotnet run --project src/MssqlOperator/

package:
	dotnet pack src/MssqlOperator/ -c Release -o ./packages

install-tool: package
	dotnet tool install --global --add-source ./packages MssqlOperator

update-tool: package
	dotnet tool update --global --add-source ./packages MssqlOperator

uninstall-tool:
	dotnet tool uninstall --global MssqlOperator

docker-all:
	docker-compose up --build
