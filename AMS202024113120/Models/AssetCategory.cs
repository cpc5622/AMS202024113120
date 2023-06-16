using System;
using System.Collections.Generic;

namespace AMS202024113120.Models;

public partial class AssetCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
