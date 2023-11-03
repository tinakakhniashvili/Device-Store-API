using TinasAppleStore.Data;

namespace TinasAppleStore
{
    public class Seed
    {

        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }
        public void SeedDataContext() { 
        }
    }
}
