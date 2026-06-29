using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using TatooShop.Models;

namespace TatooShop.Services
{
    public class DataConnection
    {
        private const string DatabaseName = "TattooShopProject";
        private const string ConnectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=TattooShopProject;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True";
        private const string MasterConnectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True";

        private static readonly string DatabaseDirectoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TattooShop",
            "Database");
        private static readonly string DatabaseFilePath = Path.Combine(DatabaseDirectoryPath, $"{DatabaseName}.mdf");
        private static readonly string LogFilePath = Path.Combine(DatabaseDirectoryPath, $"{DatabaseName}_log.ldf");
        private static readonly string SchemaFilePath = ResolveSchemaFilePath();

        private static DataConnection? _instance;

        public List<Acc> Accounts { get; }
        public List<Sketch> Sketch { get; }
        public List<Master> Masters { get; }
        public List<Reservation> Reservations { get; }
        public List<Feedback> Feedbacks { get; }
        public List<Favourite> Favourites { get; }
        public List<SupportRequest> SupportRequests { get; }
        public List<InternalNotification> InternalNotifications { get; }

        public static DataConnection GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataConnection();
                if (_instance.Accounts.Count == 0)
                {
                    _instance.Accounts.Add(new Admin("Admin", Manager.HashPassword("123"), "admin@gmail.com", "+375291112233", "Admin", "Admin", "Admin"));
                    _instance.SaveChanges();
                }

                _instance.MarkPastReservationsAsVisited();
            }

            return _instance;
        }

        public static DataConnection GetInstance(string connectionString) => GetInstance();

        private DataConnection()
        {
            EnsureDatabase();

            Accounts = LoadAccounts();
            Masters = LoadMasters();
            Sketch = LoadSketches();
            Reservations = LoadReservations();
            Feedbacks = LoadFeedbacks();
            Favourites = LoadFavourites();
            SupportRequests = LoadSupportRequests();
            InternalNotifications = LoadInternalNotifications();
        }

        public int SaveChanges()
        {
            AssignIds(Accounts);
            AssignIds(Masters);
            AssignIds(Sketch);
            AssignIds(Reservations);
            AssignIds(Feedbacks);
            AssignIds(Favourites);
            AssignIds(SupportRequests);
            AssignIds(InternalNotifications);

            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.Favourites;");
            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.Feedbacks;");
            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.Reservations;");
            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.SupportRequests;");
            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.InternalNotifications;");
            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.Sketches;");
            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.Masters;");
            ExecuteNonQuery(connection, transaction, "DELETE FROM dbo.Accounts;");

            foreach (var account in Accounts)
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.Accounts (Id, AccountType, Login, Password, EMail, Phone, Surname, Name, MiddleName, Address)
                      VALUES (@Id, @AccountType, @Login, @Password, @EMail, @Phone, @Surname, @Name, @MiddleName, @Address);";
                command.Parameters.AddWithValue("@Id", account.Id);
                command.Parameters.AddWithValue("@AccountType", (int)account.AccountType);
                command.Parameters.AddWithValue("@Login", account.Login);
                command.Parameters.AddWithValue("@Password", account.Password);
                command.Parameters.AddWithValue("@EMail", account.EMail);
                command.Parameters.AddWithValue("@Phone", account.Phone);
                command.Parameters.AddWithValue("@Surname", account.Surname);
                command.Parameters.AddWithValue("@Name", account.Name);
                command.Parameters.AddWithValue("@MiddleName", account.MiddleName);
                command.Parameters.AddWithValue("@Address", account is User user ? user.Address : (object)DBNull.Value);
                command.ExecuteNonQuery();
            }

            foreach (var master in Masters)
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.Masters (Id, Image, Surname, Name, MiddleName, Type, Experience)
                      VALUES (@Id, @Image, @Surname, @Name, @MiddleName, @Type, @Experience);";
                command.Parameters.AddWithValue("@Id", master.Id);
                command.Parameters.AddWithValue("@Image", (object?)master.Image ?? DBNull.Value);
                command.Parameters.AddWithValue("@Surname", master.Surname);
                command.Parameters.AddWithValue("@Name", master.Name);
                command.Parameters.AddWithValue("@MiddleName", master.MiddleName);
                command.Parameters.AddWithValue("@Type", (int)master.Type);
                command.Parameters.AddWithValue("@Experience", master.Experience);
                command.ExecuteNonQuery();
            }

            foreach (var sketch in Sketch)
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.Sketches (Id, Type, MasterId, Image, Placement, [Size], EstimatedHours, Complexity)
                      VALUES (@Id, @Type, @MasterId, @Image, @Placement, @Size, @EstimatedHours, @Complexity);";
                command.Parameters.AddWithValue("@Id", sketch.Id);
                command.Parameters.AddWithValue("@Type", (int)sketch.Type);
                command.Parameters.AddWithValue("@MasterId", (object?)sketch.Master?.Id ?? DBNull.Value);
                command.Parameters.AddWithValue("@Image", (object?)sketch.Image ?? DBNull.Value);
                command.Parameters.AddWithValue("@Placement", (int)sketch.Placement);
                command.Parameters.AddWithValue("@Size", (int)sketch.Size);
                command.Parameters.AddWithValue("@EstimatedHours", sketch.EstimatedHours);
                command.Parameters.AddWithValue("@Complexity", (int)sketch.Complexity);
                command.ExecuteNonQuery();
            }

            foreach (var reservation in Reservations.Where(item => item.User != null && item.Master != null))
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.Reservations (Id, UserId, MasterId, [Date], [Time], [Status])
                      VALUES (@Id, @UserId, @MasterId, @Date, @Time, @Status);";
                command.Parameters.AddWithValue("@Id", reservation.Id);
                command.Parameters.AddWithValue("@UserId", reservation.User.Id);
                command.Parameters.AddWithValue("@MasterId", reservation.Master.Id);
                command.Parameters.AddWithValue("@Date", reservation.Date);
                command.Parameters.AddWithValue("@Time", (int)reservation.Time);
                command.Parameters.AddWithValue("@Status", (int)reservation.Status);
                command.ExecuteNonQuery();
            }

            foreach (var feedback in Feedbacks.Where(item => item.User != null && item.Master != null))
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.Feedbacks (Id, UserId, MasterId, Rating, Comment, CreatedAt)
                      VALUES (@Id, @UserId, @MasterId, @Rating, @Comment, @CreatedAt);";
                command.Parameters.AddWithValue("@Id", feedback.Id);
                command.Parameters.AddWithValue("@UserId", feedback.User.Id);
                command.Parameters.AddWithValue("@MasterId", feedback.Master.Id);
                command.Parameters.AddWithValue("@Rating", feedback.Rating);
                command.Parameters.AddWithValue("@Comment", (object?)feedback.Comment ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", feedback.CreatedAt);
                command.ExecuteNonQuery();
            }

            foreach (var favourite in Favourites.Where(item => item.User != null && item.Sketch != null))
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.Favourites (Id, UserId, SketchId)
                      VALUES (@Id, @UserId, @SketchId);";
                command.Parameters.AddWithValue("@Id", favourite.Id);
                command.Parameters.AddWithValue("@UserId", favourite.User.Id);
                command.Parameters.AddWithValue("@SketchId", favourite.Sketch.Id);
                command.ExecuteNonQuery();
            }

            foreach (var request in SupportRequests.Where(item => item.User != null))
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.SupportRequests (Id, UserId, Subject, Message, AdminReply, CreatedAt, ProcessedAt, IsProcessed)
                      VALUES (@Id, @UserId, @Subject, @Message, @AdminReply, @CreatedAt, @ProcessedAt, @IsProcessed);";
                command.Parameters.AddWithValue("@Id", request.Id);
                command.Parameters.AddWithValue("@UserId", request.User.Id);
                command.Parameters.AddWithValue("@Subject", request.Subject);
                command.Parameters.AddWithValue("@Message", request.Message);
                command.Parameters.AddWithValue("@AdminReply", (object?)request.AdminReply ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
                command.Parameters.AddWithValue("@ProcessedAt", (object?)request.ProcessedAt ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsProcessed", request.IsProcessed);
                command.ExecuteNonQuery();
            }

            foreach (var notification in InternalNotifications.Where(item => item.User != null))
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    @"INSERT INTO dbo.InternalNotifications (Id, UserId, Title, Message, CreatedAt, IsRead)
                      VALUES (@Id, @UserId, @Title, @Message, @CreatedAt, @IsRead);";
                command.Parameters.AddWithValue("@Id", notification.Id);
                command.Parameters.AddWithValue("@UserId", notification.User.Id);
                command.Parameters.AddWithValue("@Title", notification.Title);
                command.Parameters.AddWithValue("@Message", notification.Message);
                command.Parameters.AddWithValue("@CreatedAt", notification.CreatedAt);
                command.Parameters.AddWithValue("@IsRead", notification.IsRead);
                command.ExecuteNonQuery();
            }

            transaction.Commit();
            return 1;
        }

        public static List<User> GetUsers() => GetInstance().Accounts
            .Where(account => account.AccountType == AccountType.User)
            .Select(account => account as User)
            .Where(user => user != null)
            .Cast<User>()
            .ToList();

        public static List<Sketch> GetSketches() => GetInstance().Sketch.ToList();
        public static List<Master> GetMasters() => GetInstance().Masters.ToList();
        public static List<Reservation> GetReservations()
        {
            var instance = GetInstance();
            instance.MarkPastReservationsAsVisited();
            return instance.Reservations.ToList();
        }
        public static List<Feedback> GetFeedbacks() => GetInstance().Feedbacks.ToList();
        public static List<Favourite> GetFavourites() => GetInstance().Favourites.ToList();
        public static List<SupportRequest> GetSupportRequests() => GetInstance().SupportRequests.ToList();
        public static List<InternalNotification> GetInternalNotifications() => GetInstance().InternalNotifications.ToList();

        public static List<User> SearchUsers(string query)
        {
            query = query?.ToLower();

            if (!string.IsNullOrEmpty(query))
            {
                var tags = query.Split(' ');
                return GetUsers().Where(user => tags.All(tag => user.ForSearch().Contains(tag))).ToList();
            }

            return GetUsers();
        }

        public static List<Admin> SearchAdmins(string query)
        {
            query = query?.ToLower();

            if (!string.IsNullOrEmpty(query))
            {
                var tags = query.Split(' ');
                return GetInstance().Accounts
                    .Where(account => account.AccountType == AccountType.Admin && tags.All(tag => account.ForSearch().Contains(tag)))
                    .Cast<Admin>()
                    .ToList();
            }

            return GetInstance().Accounts.Where(account => account.AccountType == AccountType.Admin).Cast<Admin>().ToList();
        }

        public static List<Master> SearchMasters(string query)
        {
            query = query?.ToLower();

            if (!string.IsNullOrEmpty(query))
            {
                var tags = query.Split(' ');
                return GetMasters().Where(master => tags.All(tag => master.ForSearch().Contains(tag))).ToList();
            }

            return GetMasters();
        }

        private static void EnsureDatabase()
        {
            Directory.CreateDirectory(DatabaseDirectoryPath);

            using (var connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText =
                    $@"
IF DB_ID(N'{DatabaseName}') IS NULL
BEGIN
    CREATE DATABASE [{DatabaseName}]
    ON PRIMARY (
        NAME = N'{DatabaseName}',
        FILENAME = N'{DatabaseFilePath.Replace("'", "''")}'
    )
    LOG ON (
        NAME = N'{DatabaseName}_log',
        FILENAME = N'{LogFilePath.Replace("'", "''")}'
    );
END;";
                command.ExecuteNonQuery();
            }

            using var dbConnection = CreateConnection();
            dbConnection.Open();
            using var schemaCommand = dbConnection.CreateCommand();
            schemaCommand.CommandText = File.ReadAllText(SchemaFilePath);
            schemaCommand.ExecuteNonQuery();
        }

        private static List<Acc> LoadAccounts()
        {
            var result = new List<Acc>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, AccountType, Login, Password, EMail, Phone, Surname, Name, MiddleName, Address FROM dbo.Accounts ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var type = (AccountType)reader.GetInt32(1);
                Acc account = type == AccountType.Admin
                    ? new Admin(reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8))
                    : new User(reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.IsDBNull(9) ? string.Empty : reader.GetString(9));

                account.Id = reader.GetInt32(0);
                account.AccountType = type;
                result.Add(account);
            }

            return result;
        }

        private List<Sketch> LoadSketches()
        {
            var masters = Masters.ToList();
            var result = new List<Sketch>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Type, MasterId, Image, Placement, [Size], EstimatedHours, Complexity FROM dbo.Sketches ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var type = (TatooTypes)reader.GetInt32(1);
                var masterId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                var master = ResolveSketchMaster(masters, type, masterId);

                result.Add(new Sketch(
                    reader.IsDBNull(3) ? Array.Empty<byte>() : (byte[])reader["Image"],
                    type,
                    reader.IsDBNull(4) ? TattooPlacement.None : (TattooPlacement)reader.GetInt32(4),
                    reader.IsDBNull(5) ? TattooSize.None : (TattooSize)reader.GetInt32(5),
                    reader.IsDBNull(6) ? 1 : reader.GetInt32(6),
                    reader.IsDBNull(7) ? TattooComplexity.None : (TattooComplexity)reader.GetInt32(7),
                    master)
                {
                    Id = reader.GetInt32(0)
                });
            }

            return result;
        }

        private static List<Master> LoadMasters()
        {
            var result = new List<Master>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Image, Surname, Name, MiddleName, Type, Experience FROM dbo.Masters ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Master(
                    reader.GetInt32(0),
                    reader.IsDBNull(1) ? Array.Empty<byte>() : (byte[])reader["Image"],
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    (TatooTypes)reader.GetInt32(5),
                    reader.GetInt32(6)));
            }

            return result;
        }

        private List<Reservation> LoadReservations()
        {
            var users = Accounts.OfType<User>().ToDictionary(user => user.Id);
            var masters = Masters.ToDictionary(master => master.Id);
            var result = new List<Reservation>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, MasterId, [Date], [Time], [Status] FROM dbo.Reservations ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!users.TryGetValue(reader.GetInt32(1), out var user) || !masters.TryGetValue(reader.GetInt32(2), out var master))
                    continue;

                result.Add(new Reservation(user, master, reader.GetDateTime(3), (UserTime)reader.GetInt32(4))
                {
                    Id = reader.GetInt32(0),
                    Status = reader.IsDBNull(5) ? ReservationStatus.Pending : (ReservationStatus)reader.GetInt32(5)
                });
            }

            return result;
        }

        private List<Feedback> LoadFeedbacks()
        {
            var users = Accounts.OfType<User>().ToDictionary(user => user.Id);
            var masters = Masters.ToDictionary(master => master.Id);
            var result = new List<Feedback>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, MasterId, Rating, Comment, CreatedAt FROM dbo.Feedbacks ORDER BY CreatedAt DESC, Id DESC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!users.TryGetValue(reader.GetInt32(1), out var user) || !masters.TryGetValue(reader.GetInt32(2), out var master))
                    continue;

                result.Add(new Feedback(user, master, reader.IsDBNull(4) ? null : reader.GetString(4), reader.IsDBNull(3) ? 5 : reader.GetInt32(3))
                {
                    Id = reader.GetInt32(0),
                    CreatedAt = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5)
                });
            }

            return result;
        }

        private List<Favourite> LoadFavourites()
        {
            var users = Accounts.OfType<User>().ToDictionary(user => user.Id);
            var sketches = Sketch.ToDictionary(item => item.Id);
            var result = new List<Favourite>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, SketchId FROM dbo.Favourites ORDER BY Id;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!users.TryGetValue(reader.GetInt32(1), out var user) || !sketches.TryGetValue(reader.GetInt32(2), out var sketch))
                    continue;

                result.Add(new Favourite(user, sketch)
                {
                    Id = reader.GetInt32(0)
                });
            }

            return result;
        }

        private List<SupportRequest> LoadSupportRequests()
        {
            var users = Accounts.OfType<User>().ToDictionary(user => user.Id);
            var result = new List<SupportRequest>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, Subject, Message, AdminReply, CreatedAt, ProcessedAt, IsProcessed FROM dbo.SupportRequests ORDER BY IsProcessed, CreatedAt DESC, Id DESC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!users.TryGetValue(reader.GetInt32(1), out var user))
                    continue;

                result.Add(new SupportRequest(user, reader.GetString(2), reader.GetString(3))
                {
                    Id = reader.GetInt32(0),
                    AdminReply = reader.IsDBNull(4) ? null : reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5),
                    ProcessedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    IsProcessed = reader.GetBoolean(7)
                });
            }

            return result;
        }

        private List<InternalNotification> LoadInternalNotifications()
        {
            var users = Accounts.OfType<User>().ToDictionary(user => user.Id);
            var result = new List<InternalNotification>();

            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, Title, Message, CreatedAt, IsRead FROM dbo.InternalNotifications ORDER BY CreatedAt DESC, Id DESC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!users.TryGetValue(reader.GetInt32(1), out var user))
                    continue;

                result.Add(new InternalNotification(user, reader.GetString(2), reader.GetString(3))
                {
                    Id = reader.GetInt32(0),
                    CreatedAt = reader.GetDateTime(4),
                    IsRead = reader.GetBoolean(5)
                });
            }

            return result;
        }

        private static SqlConnection CreateConnection() => new(ConnectionString);

        private static Master? ResolveSketchMaster(List<Master> masters, TatooTypes type, int? masterId)
        {
            if (masterId.HasValue)
            {
                var explicitMaster = masters.FirstOrDefault(master => master.Id == masterId.Value);
                if (explicitMaster != null)
                    return explicitMaster;
            }

            return masters.FirstOrDefault(master => master.Type == type)
                ?? masters.FirstOrDefault();
        }

        private static void ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, string sql)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        private void MarkPastReservationsAsVisited()
        {
            var now = DateTime.Now;
            var hasChanges = false;

            foreach (var reservation in Reservations.Where(item =>
                         item.GetReservationDateTime() <= now &&
                         item.Status != ReservationStatus.Cancelled &&
                         item.Status != ReservationStatus.Visited))
            {
                reservation.Status = ReservationStatus.Visited;
                hasChanges = true;
            }

            if (hasChanges)
                SaveChanges();
        }

        private static void AssignIds<T>(List<T> items) where T : class
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                return;

            var nextId = items
                .Select(item => (int?)idProperty.GetValue(item))
                .Where(id => id.HasValue && id.Value > 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            foreach (var item in items)
            {
                var currentId = (int?)idProperty.GetValue(item) ?? 0;
                if (currentId <= 0)
                {
                    idProperty.SetValue(item, nextId);
                    nextId++;
                }
            }
        }

        private static string ResolveSchemaFilePath()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var publishSchemaFilePath = Path.Combine(baseDirectory, "Database", "schema.sql");
            if (File.Exists(publishSchemaFilePath))
                return publishSchemaFilePath;

            var projectSchemaFilePath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Database", "schema.sql"));
            if (File.Exists(projectSchemaFilePath))
                return projectSchemaFilePath;

            return publishSchemaFilePath;
        }
    }
}
