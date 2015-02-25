namespace Trellheim.Server.Core.Database
{
    using System.Text;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Data.Shared;

    public sealed class DatabaseActor : ReceiveActor
    {
        #region Get

        public sealed class GetCharacters
        {
            public int AccountId { get; set; }

            public GetCharacters(int accountId)
            {
                AccountId = accountId;
            }
        }

        #endregion

        #region Upserts
        public sealed class RegisterNewAccount
        {
            public Account Account { get; private set; }

            public RegisterNewAccount(Account account)
            {
                Account = account;
            }
        }
        #endregion

        private EntityContext database;

        public DatabaseActor(EntityContext database)
        {
            this.database = database;
            Receive<RegisterNewAccount>(msg =>
            {
                database.EntitySet<Account>().Add(msg.Account);
                database.SaveChangesAsync();
            });
        }
    }
}
