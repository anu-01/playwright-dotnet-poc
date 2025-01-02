
namespace Helpers;
public class BookingDates
{
    public string Checkin { get; set; }
    public string Checkout { get; set; }
}

public class Booking
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public int Totalprice { get; set; }
    public bool Depositpaid { get; set; }
    public BookingDates Bookingdates { get; set; }
    public string Additionalneeds { get; set; }
}

public class BookingResponse
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
}
