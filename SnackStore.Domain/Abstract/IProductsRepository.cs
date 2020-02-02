using System.Linq;
using SnackStore.Domain.Entities;

namespace SnackStore.Domain.Abstract
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }

        void SaveProduct(Product product);

        Product DeleteProduct(int productID);
    }
}
