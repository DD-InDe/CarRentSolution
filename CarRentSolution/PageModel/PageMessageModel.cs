using System.Drawing;

namespace CarRentSolution.PageModel;

public class PageMessageModel
{
    public string Message { get; private set; } = "";
    public String Color { get; private set; } = "white";

    public void Change(string message, MessageType messageType)
    {
        Message = message;
        Color = messageType switch
        {
            MessageType.Error => "red",
            MessageType.Info => "yellow",
            MessageType.Ok => "green"
        };
    }

    public void Clear()
    {
        Message = "";
        Color = "white";
    }
}