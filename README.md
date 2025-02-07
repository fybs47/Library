To run the application, you need to run the compose file in the docker. This can be done in the console with the following command:
docker-compose up --build api.service
-The server should start at the following address:
-http://localhost:5080/index.html
-Now you can download the client part from https://github.com/fybs47/lib-front and run the application code by going to the root folder and using the command:
npm run dev
-(You need to install Node.js), after that the client application should be launched at the address:
-http://localhost:3000
