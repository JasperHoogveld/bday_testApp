namespace BirthdayCalendar;

public class Person : IPerson {

  public int? ID { get; set; }
  public int? RequestID { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public DateTime Birthday { get; set; }
}

public interface IPerson {
  public int? ID { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public DateTime Birthday { get; set; }
}

