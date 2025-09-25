using System;

namespace RealWorldUnitTest.Web.Models;

public class Product : BaseEntity
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
     public string? Color{ get; set; }
}
