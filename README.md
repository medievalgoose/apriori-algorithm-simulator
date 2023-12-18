# Apriori Algorithm Simulator
A simulator that implements the Apriori Algorithm to produce association rules from transactions data. 

## How to use
1. Clone the repo and open the project in your IDE (I use Visual Studio).
2. Create a new database, in this case, I'm using SQL Server. You can find the schema in `database.txt` file.
3. Get the "connection string" of your newly created database. 
4. Navigate to `appsettings.json` file in the project folder and change the value for `DefaultConnection` with your database connection string.
5. Run the project.

## Additional Instructions
- Create the products first. 
- Navigate to the transactions page. Add some transactions and list the products inside the trasactions detail page.
- Open the Apriori page, specify the minimum support and minimum confidence then press Submit.

## File-based Analysis Feature
- Use the `SimulatedOrderData.csv` included in the repo as the file in File-based Analysis page.
- Specify the minimum support and confidence.
- Run the analysis.
