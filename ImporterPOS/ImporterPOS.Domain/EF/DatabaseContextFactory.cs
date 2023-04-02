using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.EF
{
    public class DatabaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _optionsBuilder;

        public DatabaseContextFactory(Action<DbContextOptionsBuilder> optionsBuilder)
        {
            _optionsBuilder = optionsBuilder;
        }

        public DatabaseContext CreateDbContext(string[]? args = null)
        {
            DbContextOptionsBuilder<DatabaseContext> options = new DbContextOptionsBuilder<DatabaseContext>();

            _optionsBuilder(options);

            return new DatabaseContext(options.Options);
        }
    }
}
