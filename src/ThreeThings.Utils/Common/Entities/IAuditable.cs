﻿namespace ThreeThings.Utils.Common.Entities;

public interface IAuditable
{
    public string CreatedBy { get; set; }

    public DateTime CreationTime { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? ModifiedTime { get; set; }
}
