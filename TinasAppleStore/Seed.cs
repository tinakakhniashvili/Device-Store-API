using System;
using System.Linq;
using javax.management.loading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using sun.awt;
using TinasAppleStore.Data;
using TinasAppleStore.Models;

namespace TinasAppleStore
{
    public class Seed
    {
        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }
        public void SeedDataContext()
        {
            if (!dataContext.Products.Any())
            {
                    dataContext.Products.AddRange(new Product()
                    {
                        Name = "Product 1",
                        Price = 1456.78,
                        Description = "Description for Product 1."
                    },
                    new Product
                    {
                        Name = "Product 2",
                        Price = 789.99,
                        Description = "Description for Product 2."
                    },
                    new Product
                    {
                        Name = "Product 3",
                        Price = 999.99,
                        Description = "Description for Product 3."
                    });

                dataContext.SaveChanges();
            }
        }
    }
}
