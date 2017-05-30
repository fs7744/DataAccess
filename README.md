# DataAccess

Author: Victor.X.Qu

Email: fs7744@hotmail.com

DataAccess is a c# project for sql data mapping to object, like dapper

DataAccess is for net core , so it base on netstandard1.3

## db supports
DataAccess base on ado.net, so you can use blow db :

* [MSSql](https://www.nuget.org/packages/VIC.DataAccess.MSSql)
* [MySql](https://www.nuget.org/packages/VIC.DataAccess.MySql)
* [PostgreSQL](https://www.nuget.org/packages/VIC.DataAccess.PostgreSQL)
* [SQLite](https://www.nuget.org/packages/VIC.DataAccess.SQLite)

### use MSSql example 

#### Use config file

##### dependencies

``` json
  "dependencies": {
    "VIC.DataAccess.MSSql": "1.0.2",
    "VIC.DataAccess.Config": "1.0.2"
  },
```

You can config sql in xml file for DataAcces, like:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<DbConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ConnectionStrings>
    <DataConnection Name="Test" ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestDataAccess;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" />
  </ConnectionStrings>
  <SqlConfigs>
    <DbSql CommandName="SelectByName" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
SELECT top 1
    Id
    ,Age
    ,Name
    ,JoinDate
    ,[Money]
FROM [dbo].[Students] WITH(NOLOCK)
WHERE @Name = Name
      ]]>
      </Text>
      <PreParameters>
        <Parameter Name="@Name" Direction="Input" Type="AnsiString" />
      </PreParameters>
    </DbSql>
    <DbSql CommandName="SelectAll" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
SELECT
    Id
    ,Age
    ,Name
    ,JoinDate
    ,[Money]
FROM [dbo].[Students] WITH(NOLOCK)
      ]]>
      </Text>
    </DbSql>
    <DbSql CommandName="SelectAllAge" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
SELECT
    sum(Age) as Age
FROM [dbo].[Students] WITH(NOLOCK)
      ]]>
      </Text>
    </DbSql>
    <DbSql CommandName="Clear" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
delete from [dbo].[Students]
      ]]>
      </Text>
    </DbSql>
    <DbSql CommandName="BulkCopy" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
[dbo].[Students]
      ]]>
      </Text>
    </DbSql>
  </SqlConfigs>
</DbConfig>
```

Code for use :

``` csharp
var provider = new ServiceCollection()
                     .UseDataAccess()
                     .UseDataAccessConfig(Directory.GetCurrentDirectory(), false, "db.xml")
                     .BuildServiceProvider();

List<Student> students = GenerateStudents(count);

var db = provider.GetService<IDbManager>();        

var command = db.GetCommand("BulkCopy");
await command.ExecuteBulkCopyAsync(students);

var command = db.GetCommand("SelectByName");
Student s = await command.ExecuteEntityAsync<Student>(new { Name = "3" });

var command = db.GetCommand("SelectAll");
List<Student> students = await command.ExecuteEntityListAsync<Student>();

var command = db.GetCommand("SelectAllAge");
int? age = await command.ExecuteScalarAsync<int?>();

```

#### No config file

##### dependencies

``` json
  "dependencies": {
    "VIC.DataAccess.MSSql": "1.0.2"
  },
```

``` csharp
var provider = new ServiceCollection()
                     .UseDataAccess()
                     .BuildServiceProvider();

var command = provider.GetService<IDataCommand>(); 
command.ConnectionString = "sqlConnectionString";
command.Text = "sql";
command.Type = CommandType.Text;
Student s = await command.ExecuteEntityAsync<Student>(new { Name = "3" });

```

## Test performance

You can see the simple code in https://github.com/fs7744/DataAccess/blob/master/example/MSSqlExample

![](https://github.com/fs7744/DataAccess/blob/master/example/MSSqlExample/test.png?raw=true)

## All package 
* [VIC.ObjectConfig](https://www.nuget.org/packages/VIC.ObjectConfig/)
* [VIC.DataAccess](https://www.nuget.org/packages/VIC.DataAccess)
* [VIC.DataAccess.Config](https://www.nuget.org/packages/VIC.DataAccess.Config/)
* [VIC.DataAccess.MSSql](https://www.nuget.org/packages/VIC.DataAccess.MSSql)
* [VIC.DataAccess.MySql](https://www.nuget.org/packages/VIC.DataAccess.MySql)
* [VIC.DataAccess.PostgreSQL](https://www.nuget.org/packages/VIC.DataAccess.PostgreSQL)
* [VIC.DataAccess.SQLite](https://www.nuget.org/packages/VIC.DataAccess.SQLite)