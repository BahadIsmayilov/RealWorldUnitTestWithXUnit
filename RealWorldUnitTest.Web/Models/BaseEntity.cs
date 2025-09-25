using System;

namespace RealWorldUnitTest.Web.Models;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
