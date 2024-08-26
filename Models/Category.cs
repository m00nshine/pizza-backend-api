using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
