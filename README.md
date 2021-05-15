
# Shopping Cart API with Stock Control using Clean Archtirecture & Domain Driven Design Concepts
Shopping Management

This is an ASP.NET Core Web API Project including principles of Clean Architecture and how Domain Driven Design concepts work with Clean Architecture


#### Technologies 
- ASP.NET Core 5
- MongoDB
- Redis
- gRPC
- Docker

To run the project, you will need the following tools;
- Visual Studio 2019
- .Net 5.0 or later
- Docker Desktop

To Install the project;
- Clone the repository
- Open Docker for Windows. Go to the Settings > Advanced option, from the Docker icon in the system tray, to configure the minimum amount of memory and CPU like so:
Memory: 4 GB
CPU: 2
- At the root directory of the project which include docker-compose.yml files, run below command:

**docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d**

After the microservice, database and portainer are composed you can launch the following services;

- Basket.API => http://localhost:8002/swagger/index.html

- Stock.API => http://localhost:8000/swagger/index.html

- gRPC => http://localhost:8001

- Portainer (Container Management) = http://localhost:9000/
		 username = "admin"
		 password = "admin1234"

** If you want to run the project without docker-compose you should set "Basket.API, Stock.API and Stock.Grpc" as a multiple startup projects" under the property pages of the solution. You can find below the screenshot of property page

![enter image description here](https://drive.google.com/uc?export=download&id=1KTsDCHem2On96q7QGFIEJ2xDzK4j2qNQ)


#### Basket.API
Shopping Cart data is stored in Redis using JSON Serialization. 
Before adding any product into cart, quantity information of that product is being checked using gRPC Client. We are able to make call to another API (Stock.API) using gRPC.

#### Stock.API
This API is used to store stock information of the products
Stock information will be automatically inserted into MongoDB with any endpoint call.

#### Stock.gRPC
This API is used to communicate with Basket.API and Stock data. You can find detail of ProtoBuf and gRPC Service implementations.

Project consists of Core and Periphery Layers. 
- Core Layers: Basket.Application - Basket.Domain & Stock.Application - Stock.Domain
- Periphery Layers: Basket.API - Basket.Infrastructure & Stock.API - Stock.Infrastructure

[![](https://blob.jacobsdata.com/software-alchemy/entry12/clean-domain-driven-design-jacobs1.png)](https://blob.jacobsdata.com/software-alchemy/entry12/clean-domain-driven-design-jacobs1.png)

