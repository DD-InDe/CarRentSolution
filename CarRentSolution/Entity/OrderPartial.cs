using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CarRentSolution.Entity;

public partial class Order
{
    [NotMapped] public string? ClientFullName { get; set; } = "";

    public string GetColor()
    {
        string color = "#1b6ec2";
        if (OrderHistories.Count > 0)
        {
            color = OrderHistories.Last()
                    .StatusId switch
                {
                    1 => "#1b6ec2",
                    2 => "#decc54",
                    3 => "#de5454",
                    4 => "gray",
                    5 => "#79de54",
                };
        }

        return color;
    }

    public string GetStatus()
    {
        return OrderHistories.Last()
            .Status.Name;
    }
}