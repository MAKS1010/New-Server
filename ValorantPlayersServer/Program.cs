using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MsSql;
using System.Data.SqlClient;

internal class Program
{
    internal static void Main(string[] _)
    {
        //Console.WriteLine("Listing Player 1");
        //ListPlayers(1);
        CreatePlayer();

        Console.WriteLine("Listing All Players");
        ListPlayers();

        //DeletePlayer(1);
        //        AddFollow(3, 15);
        //        CreatePlayer();
    }

    internal static readonly string connectionStr = "Server=.;Initial Catalog=Valorant_Players;Integrated Security=false;UID=sa;PWD=P@ssw0rd;TrustServerCertificate=True;";

    internal static void CreatePlayer()
    {
        var connection = new SqlConnection(connectionStr);
        var insertQuery = new MsSqlPlayerRepository()
                                            .Insert()
                                            .WithFields(fm => fm.IncludeAll()
                                                .Exclude(f => f.Id));

        var newPlayer = insertQuery.Execute(new Player
        {
            Name = "APlayer 1",
            Email = "player1@gmail.com",
            FollowCount = 10,
            BirthDate = new DateTime(1990, 1, 1)
        }, true, connection);

        Console.WriteLine($"New player id: {newPlayer!.Id}");
    }

    internal static void AddFollow(long playerId, int followCount = 1)
    {
        var connection = new SqlConnection(connectionStr);

        MsSqlUpdateQuery<Player> updateQuery = new MsSqlPlayerRepository()
                                        .Update()
                                        .Where(m => m.Eq(f => f.Id, playerId))
                                        .UpdateFields(u => u.Exclude(f => f.BirthDate)
                                                            .Exclude(f => f.Id)
                                                            .Exclude(f => f.Name)
                                                            .Exclude(f => f.FollowCount)
                                                            .FromFieldExpression(f => f.FollowCount, $" % + {followCount}"));

        var newPlayer = updateQuery.Execute(new Player(), connection);

        Console.WriteLine($"PlayerUpdated");
    }

    internal static void DeletePlayer(long playerId)
    {
        var connection = new SqlConnection(connectionStr);

        var deleteQuery = new MsSqlPlayerRepository()
                                                .Delete()
                                                .Where(m => m.Eq(f => f.Id, playerId));

        var newPlayer = deleteQuery.Execute(connection);

        Console.WriteLine($"Player Deleted");
    }

    internal static void ListPlayers()
    {
        var connection = new SqlConnection(connectionStr);

        var selectQuery = new MsSqlPlayerRepository()
                                .Select()
                                .Returns(f => f.IncludeAll())
                                .OrderBy(om => om.FromField(f => f.Name, OrderDirections.Ascending));

        var players = selectQuery.Execute(connection);

        foreach (var player in players)
        {
            Console.WriteLine($"Player Id: {player.Id}, Name: {player.Name}, Birth date: {player.BirthDate}, Follow count: {player.FollowCount}");
        }

        Console.WriteLine($"Done");
    }
}