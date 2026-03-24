Prerequisites:
1. Download Node.js thru this link https://nodejs.org/
2. Install Angular CLI thru Visual Studio Code Terminal then type the code below:
      - npm install -g @angular/cli
3. Import JobSearchDB.bacpac file to the SQL database.
4. Import JobSearchApp.postman_collection.json to Postman.    
5. Under the Angular app, change the ApiURL value in the environment.development.ts based on localhost value on the .Net Core app. 
6. Under the .Net app, change the AllowedOrigins value in the appSettings.development.json based on localhost value on the Angular app. 
7. Align the ServerName under appSettings.development.json with the current machine SQL credentials.

Important Note:
For Customer and Contractor API the update and delete field are pending for future development but exist in the UI.

Future Improvements:
Addition of RateLimiting
Implementation of Authentication for API calls
Addition of Index to database tables
Addition of caching

