=======================
Dapper.DataRepositories
=======================
There is some pretty cool stuff happening out there: Micro ORMs like [Dapper](https://code.google.com/p/dapper-dot-net/), [Repository Pattern](http://msdn.microsoft.com/en-us/library/ff649690.aspx) as nice and usefull option to implement data layers and [async programming](http://msdn.microsoft.com/en-us/library/hh191443.aspx) as a way to improve the performance of our code. This package combines them and lets you to implement the repository pattern easily and fast.  

This package contains 3 components:

* DataConnection: It's just a database connection wrapper. It could be useful for your no conventional data repositories, especially for those that does not needs to implement all CRUD operations. It will be the base class for all the data repositories directly or indirectly.
* IDataRepository: the contract base for the data repositories that needs to implement CRUD operations (and more). 
* DataRepository: the base class for your data repositories that needs to implement CRUD operations (and more).  


How do I use those things?
--------------------------
Lets say we have this POCO definition:

    [StoredAs("Users")]
    public class User
	{
		[KeyProperty(Identity = true)]
		public int Id { get; set; }
		
		public string Login { get; set;}
		
		[StoredAs("FName")]
		public string FirstName { get; set; }
		
		[StoredAs("LName")]
		public string LastName { get; set; }
		
		public string Email { get; set; }
		
		public DateTime DateOfBirth { get; set; }
		
		[StatusProperty]
		public UserStatus Status { get; set; }
		
		[NonStored]
		public string FullName
		{
			get
			{
				return string.Format("{0} {1}", FirstName, LastName);
			}
		}
	}
	
	public enum UserStatus : byte
	{
		Registered = 1,
		
		Active = 2,
		
		[Deleted]
		Inactive = 3
	}

NOTE: for more information about how to define the metadata of your POCO's check the [MicroOrm.Pocos.SqlGenerator](https://github.com/ElNinjaGaiden/MicroOrm.Pocos.SqlGenerator) package.  

Using the elements of this package, you only need to do a couple of things in order to create a data repository for this "User" POCO.  

First, create the repository contract inheriting from IDataRepository:

    public interface IUsersRepository : IDataRepository<User>
    {
        //IUsersRepository is inheriting all CRUD operations 
    }
    
Then, implements the repository:

    public class UsersRepository : DataRepository<User>, IUsersRepository
    {
        //NOTE: Because this is a "Dependency Injection Oriented Package"
        //we need to pass the database connection and the SQL Generator as parameters
        public UsersRepository(IDbConnection connection, ISqlGenerator<User> sqlGenerator)
            : base(connection, sqlGenerator)
        {
        }
    }
    
Simple as that, we have defined a fully functional data repository for the "User" POCO. Because the inheritance pattern we are doing here, both repository contract and repository implementation contains this functions: 

    IEnumerable<User> GetAll();
    
    Task<IEnumerable<User>> GetAllAsync();
    
    IEnumerable<User> GetWhere(object filters);
    
    Task<IEnumerable<User>> GetWhereAsync(object filters);
    
    User GetFirst(object filters);
    
    Task<User> GetFirstAsync(object filters);
    
    bool Insert(User instance);
    
    Task<bool> InsertAsync(User instance);
    
    bool Update(User instance);
    
    Task<bool> UpdateAsync(User instance);
    
    bool Delete(object key);
    
    Task<bool> DeleteAsync(object key);

No SQL. No repeated code. Three pretty basic steps:
* Decorate your POCO's
* Define your repository contracts
* Implement your repositories.  

All those functions are virtual at DataRepository level so you can easily override them. Lets say we have a heavy custom stored procedure to insert users called "sp_AddUser". If we want to use that stored procedure, we only need to overwrite the "Insert" function at repository level, like this:

    public class UsersRepository : DataRepository<User>, IUsersRepository
    {
        public UsersRepository(IDbConnection connection, ISqlGenerator<User> sqlGenerator)
            : base(connection, sqlGenerator)
        {
        }
        
        //Custom insert implementation
        public bool override Insert(User instance)
        {
            var newId = Connection.Query<int>("sp_AddUser", instance, commandType: CommandType.StoredProcedure).Single();
            instance.Id = newId;
            return newId > 0;
        }
    }

And what if I want to handle more functions rather than CRUD operations? Easy, define them at repository contract level and implement them at repository level, like this:

    //Repository contract
    public interface IUsersRepository : IDataRepository<User>
    {
        //Totally custom function
        User GetOlderUser();
    }
    
    //Repository implementation
    public class UsersRepository : DataRepository<User>, IUsersRepository
    {
        public UsersRepository(IDbConnection connection, ISqlGenerator<User> sqlGenerator)
            : base(connection, sqlGenerator)
        {
        }
        
        //Totally custom function
        public User GetOlderUser()
        {
            return Connection.Query<User>("sp_GetOlderUser", commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
	
Don't want/need all CRUD operations?
------------------------------------

There are some situations when we don't need all CRUD operations. If that' the case, we only need to do a couple of variants:

* Don't inherit from IDataRepository at repository contract level and include in it only the operations that you need
* Inherit from DataConnection instead of DataRepository at repository implementation level

Example:

	//POCO definition
	[StoredAs("EventLogs")]
    public class EventLog
	{
		[KeyProperty(Identity = true)]
		public long Id { get; set; }
		
		public string Description { get; set;}
		
		public DateTime AddedOn { get; set; }
	}
	
	//Repository contract definition
	public interface IEventLogsRepository
	{
		void Insert(EventLog instance);
	}
	
	//Repository implementation
	public class EventLogsRepository : DataConnection
	{
		public EventLogsRepository(IDbConnection connection)
			: base(connection)
		{
		}
		
		public void Insert(EventLog instance)
		{
			...
		}
	}
	
License
-------
The MIT License (MIT)

Copyright (c) 2014 Diego García

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
