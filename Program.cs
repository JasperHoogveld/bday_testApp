using System.Globalization;
using System.Text.Json;

namespace BirthdayCalendar;

internal partial class Program {
  private static void Main() {
    var menu = new MainMenu();
    bool showMenu = true;
    while (showMenu) {
      showMenu = menu.Show();
    }
  }
}

public class MainMenu() {
  private List<Person>? _persons;
  private readonly Person _person = new();
  private static readonly string _path = Directory.GetCurrentDirectory();
  private readonly string _jsonFile = Path.Combine(_path, "bdays.json");
  private readonly CultureInfo _culture = new("nl-NL");
  private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

  public bool Show() {
    Console.Clear();
    Console.WriteLine("Choose an option");
    Console.WriteLine("1: Set Birthday");
    Console.WriteLine("2: Get Birthdays");
    Console.WriteLine("3: Delete Birthday");
    Console.WriteLine("4: Exit");
    

    switch (Console.ReadLine()) {
      case "1":
        Console.WriteLine();
        SetBirthdays();
        return true;

      case "2":
        Console.WriteLine();
        if (!File.Exists(_jsonFile)) {
          Console.WriteLine("No birthdays set. Choose option 1 in Main Menu to set birthdays.");
          ConsoleEnd();
          return true;
        } else {
          GetBirthdays();
          ConsoleEnd();
        }
        return true;

      case "3":
        Console.WriteLine();
        DeleteBirthday();
        return true;

      case "4":
        return false;

      default:
        Console.WriteLine("Please enter a value between 1 and 4 from the menu");
        ConsoleEnd();
        return true;
    }
  }

  private void SetBirthdays() {
    if (!File.Exists(_jsonFile)) {
      File.Create(_jsonFile).Close();
      File.WriteAllText(_jsonFile, "[]");
    } else if (new FileInfo(_jsonFile).Length == 0) {
      File.WriteAllText(_jsonFile, "[]");
    }
    Console.WriteLine("Enter First Name");
    _person.FirstName = Console.ReadLine();
    Console.WriteLine("Enter Last Name");
    _person.LastName = Console.ReadLine();
    Console.WriteLine("Enter Birth Date as dd-mm-yyyy");
    try {
      DateTimeStyles styles = DateTimeStyles.AdjustToUniversal;
      string? answer = Console.ReadLine();
      if (answer != null) {
        _person.Birthday = DateTime.Parse(answer, _culture, styles);
      }
    } catch (Exception) {
      Console.WriteLine("Enter date in correct dd-mm-yyyy format");
      ConsoleEnd();
      return;
    }

    using (FileStream openStream = File.OpenRead(_jsonFile)) {
      _persons = JsonSerializer.Deserialize<List<Person>>(openStream);
    }
    _persons ??= [];
    var lastPerson = _persons.MaxBy(x => x.ID);
    if (lastPerson != null) {
      int? newID = lastPerson.ID + 1;
      int? newRequestID = lastPerson.RequestID + 1;
      _person.ID = newID;
      _person.RequestID = newRequestID;
    } else {
      _person.ID = 1;
      _person.RequestID = 1;
    }
    _persons.Add(_person);
    WriteToJsonFile();
    Console.WriteLine(_person.FirstName + " " + _person.LastName + "'s birthday is set to " + _person.Birthday.ToString("dd-MM-yyyy", _culture));
    ConsoleEnd();
  }

  private void GetBirthdays() {
    using (FileStream openStream = File.OpenRead(_jsonFile)) {
      _persons = JsonSerializer.Deserialize<List<Person>>(openStream);
    }
    if (_persons != null) {
      int _requestID = 1;
      foreach (Person personOut in _persons) {
        personOut.RequestID = _requestID;
        Console.WriteLine(personOut.RequestID + " | " + (personOut?.FirstName ?? "Empty") + " " + (personOut?.LastName ?? "Empty") + "'s birthday is on " + personOut?.Birthday.ToString("dd-MM-yyyy", _culture));
        _requestID++;
      }
    }
    Console.WriteLine();
  }

  private void DeleteBirthday() {
    Console.WriteLine("Enter ID of persons birthday to remove or 'c' to cancel");
    GetBirthdays();
    try {
      string? input = Console.ReadLine();
      if (input != null && input.Equals("c", StringComparison.OrdinalIgnoreCase)) {
        return;
      }
      if (input != null) {
        int? personID = int.Parse(input, _culture);
        if (personID != null) {
          using (FileStream openStream = File.OpenRead(_jsonFile)) {
            _persons = JsonSerializer.Deserialize<List<Person>>(openStream);
          }
          Person? personToRemove = null;
          if (_persons != null) {
            int _requestID = 1;
            foreach (Person person in _persons) {
              person.RequestID = _requestID;
              if (person.RequestID == personID) {
                Console.WriteLine("Are you sure? Y or N");
                string? answer = Console.ReadLine();
                if (answer?.Equals("y", StringComparison.CurrentCultureIgnoreCase) ?? false) {
                  personToRemove = person;
                  break;
                } else if (answer?.Equals("n", StringComparison.CurrentCultureIgnoreCase) ?? false) {
                  break;
                }
              }
              _requestID++;
            }
          }
          if (personToRemove != null) {
            _persons?.Remove(personToRemove);
            WriteToJsonFile();
            Console.WriteLine(personToRemove.FirstName + " " + personToRemove.LastName + "'s birthday has been removed from the list");
          }
        }
      }
    } catch (Exception) {
      Console.WriteLine("Please enter an ID from the list of Birthdays");
      ConsoleEnd();
      return;
    }
    ConsoleEnd();
  }

  private void WriteToJsonFile() {
    string json = JsonSerializer.Serialize(_persons, typeof(IEnumerable<IPerson>), _options);
    File.WriteAllText(_jsonFile, json);
  }

  private static void ConsoleEnd() {
    Console.WriteLine("Press Any key to return to the Main Menu");
    Console.ReadKey();
  }
}

