namespace Assignment3_API.Models
{
    public class ProductType : BaseEntity
    {
        public int ProductTypeId { get; set; }

        public virtual ICollection<Product> Products { get; set; }

    }
}
