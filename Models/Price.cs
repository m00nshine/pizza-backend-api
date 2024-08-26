using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class Price
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public string LongCode { get; set; } = null!;

    public int SizeId { get; set; }

    public decimal Price1 { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
