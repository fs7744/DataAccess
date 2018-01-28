# DataAccess

[![Build status](https://ci.appveyor.com/api/projects/status/3ixccm610jbb7hgn/branch/master?svg=true)](https://ci.appveyor.com/project/fs7744/dataaccess/branch/master)

Author: Victor.X.Qu

Email: fs7744@hotmail.com

DataAccess is a c# project for sql data mapping to object, like dapper

DataAccess is for net core , and now it base on netstandard2.0

## db supports
DataAccess base on ado.net, so you can use blow db :

* [MSSql](https://www.nuget.org/packages/VIC.DataAccess.MSSql)
* [MySql](https://www.nuget.org/packages/VIC.DataAccess.MySql)
* [PostgreSQL](https://www.nuget.org/packages/VIC.DataAccess.PostgreSQL)
* [SQLite](https://www.nuget.org/packages/VIC.DataAccess.SQLite)

### use MSSql example 

#### Use config file

##### dependencies

``` xml
    <PackageReference Include="VIC.DataAccess.Config" Version="2.0.0-beta" />
    <PackageReference Include="VIC.DataAccess.MSSql" Version="2.0.0-beta" />
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

``` xml
    <PackageReference Include="VIC.DataAccess.MSSql" Version="2.0.0-beta" />
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

### HasDB

You can see the simple code in https://github.com/fs7744/DataAccess/blob/master/example/MSSqlExample

![](https://github.com/fs7744/DataAccess/blob/master/example/MSSqlExample/test.png?raw=true)

### NoDB

You can see the simple code in https://github.com/fs7744/DataAccess/blob/master/test/performance/DapperBenchmarks.cs

``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Threshold 2 (10.0.10586)
Processor=Intel Core i7-6700 CPU 3.40GHz (Skylake), ProcessorCount=8
Frequency=3328125 Hz, Resolution=300.4695 ns, Timer=TSC
.NET Core SDK=2.0.2
  [Host]     : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT


```
 |               Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |  Op/s |   Gen 0 |   Gen 1 | Allocated |
 |--------------------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|------:|--------:|--------:|----------:|
 |               Dapper | 2.487 ms | 0.0097 ms | 0.0090 ms | 0.0023 ms | 2.472 ms | 2.480 ms | 2.487 ms | 2.495 ms | 2.505 ms | 402.1 | 23.4375 | 11.7188 |  110.7 KB |
 |              VicData | 2.477 ms | 0.0217 ms | 0.0181 ms | 0.0050 ms | 2.457 ms | 2.461 ms | 2.476 ms | 2.488 ms | 2.524 ms | 403.7 | 27.3438 | 11.7188 | 112.37 KB |
 |                Chloe | 2.522 ms | 0.0329 ms | 0.0292 ms | 0.0078 ms | 2.480 ms | 2.506 ms | 2.522 ms | 2.540 ms | 2.585 ms | 396.5 | 23.4375 | 11.7188 | 110.87 KB |
 | VicDataOnlyConverter | 2.467 ms | 0.0174 ms | 0.0155 ms | 0.0041 ms | 2.444 ms | 2.458 ms | 2.464 ms | 2.478 ms | 2.502 ms | 405.4 | 27.3438 | 11.7188 | 112.28 KB |


## All package 
* [VIC.ObjectConfig](https://www.nuget.org/packages/VIC.ObjectConfig/)
* [VIC.DataAccess](https://www.nuget.org/packages/VIC.DataAccess)
* [VIC.DataAccess.Config](https://www.nuget.org/packages/VIC.DataAccess.Config/)
* [VIC.DataAccess.MSSql](https://www.nuget.org/packages/VIC.DataAccess.MSSql)
* [VIC.DataAccess.MySql](https://www.nuget.org/packages/VIC.DataAccess.MySql)
* [VIC.DataAccess.PostgreSQL](https://www.nuget.org/packages/VIC.DataAccess.PostgreSQL)
* [VIC.DataAccess.SQLite](https://www.nuget.org/packages/VIC.DataAccess.SQLite)
