<Query Kind="Statements">
  <Reference>C:\Repos\pivotpad\PivotPad\bin\Debug\net5.0\PivotPad.dll</Reference>
  <NuGetReference>NorthwindEFCore</NuGetReference>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>NorthwindEFCore</Namespace>
  <Namespace>PivotPad</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

var dbPath = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "Northwind.sqlite");

var config = new DbContextOptionsBuilder<NorthwindDbContext>();
config.UseSqlite($"Data Source={dbPath}");

using (var db = new NorthwindDbContext(config.Options))
{	
	var data = db.Orders.Select(e => new {
		e.OrderDate,
		Customer = e.Customer.CompanyName,
		Employee = e.Employee.FirstName + " " + e.Employee.LastName,
		Region = e.Customer.Region,
		TotalAmount = e.OrderDetails.Sum(e => e.UnitPrice * e.Quantity)
	});
	data.Pivot(e => 
		e
			.Column(f => f.Employee)
			.Rows(f => f.OrderDate)
			.Measures(f => f.TotalAmount)
	);
}