Dapper.DataRepositories
=======================
There are some pretty cool stuff happening out there: Micro ORMs like [Dapper](https://code.google.com/p/dapper-dot-net/), [Repository Pattern](http://msdn.microsoft.com/en-us/library/ff649690.aspx) as nice and usefull option and [async programming](http://msdn.microsoft.com/en-us/library/hh191443.aspx). This package combines them and lets you to implement the repository pattern easily and fast.  

This package contains 3 elements:

* DataConnection: It's just a database connection wrapper. It could be useful for your no conventional data repositories, especially for those that does not needs to implement all CRUD operations. It will be the base class for all the data repositories directly or indirectly.
* IDataRepository: the contract base for the data repositories that needs to implement CRUD operations (and more). 
* DataRepository: the base class for your data repositories that needs to implement CRUD operations (and more).  


How do I use those things?
--------------------------
Lets say you have this POCO entity:

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

Using the elements, you only need to do a couple of things in order to create a data repository for this "User" POCO.  

First, create the repository contract inheriting from IDataRepository:

    public interface IUsersRepository : IDataRepository<User>
    {
        //IUsersRepository is inheriting CRUD operations 
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

No SQL. No repeated code. Three pretty basic steps: decorate your POCOs, create your repository contracts and create your repositories.  
All those functions are virtual at DataRepository level so you can easily override their behavior. Lets say you have your heavy custom stored procedure to insert users called "sp_AddUser", so you can easily use it by overriden the "Insert" function at repository level, like this:

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

And what if I want to handle more function rather CRUD operations? Easy, define them at repository contract level and implement them at repository level, like this:

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