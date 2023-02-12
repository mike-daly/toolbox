using System.Data;

/// <summary>
/// read standard input and upsert sha1 checksum and file path (either filename or full path, both are good)
/// </summary>
database db = new database();

string[] arguments = Environment.GetCommandLineArgs();
if (arguments.Length < 2)
{
    DoHelpAndExit("missing argument");
}

dbMode insertNotUpdate = dbMode.unknown;

switch (arguments[1])
{
    case "-a":
        insertNotUpdate = dbMode.update;
        break;
    case "-i":
        insertNotUpdate = dbMode.insert;
        break;
    case "-h":
        DoHelpAndExit("");
        break;
    default:
        DoHelpAndExit($"unexpected commandline argument {args[1]}");
        break;
}

int lineNumber = 0;
string? inputLine = Console.ReadLine();

while (inputLine != null && inputLine.Length > 0)
{
    lineNumber++;
    const int checksumLength = 40;

    if (inputLine.Length <checksumLength+1)
    {
        DoHelpAndExit($"input line too short.  {checksumLength + 1} minimum, got {inputLine.Length} for line:  {inputLine}");
    }

    string[] inputTokens = new string[2];

    switch (insertNotUpdate)
    {
        case dbMode.insert:
            inputTokens[0] = inputLine.Substring(0, checksumLength);
            inputTokens[1] = inputLine.Substring(checksumLength+1);

            db.insertSha1Path(inputTokens[0], inputTokens[1]);

            break;

            /*
        case dbMode.update:
            inputTokens[0] = inputLine.Substring(0, 39);
            inputTokens[1] = inputLine.Substring(41);

            db.insertSha1Path(inputTokens[0], inputTokens[1]);

            break;
            */

        default:
            DoHelpAndExit($"internal error, unknown database mode:  {insertNotUpdate.ToString()}");
            break;

    }

    inputLine = Console.ReadLine();
}

db.Close();
Environment.Exit(0);


/// <summary>
/// Print help with optional message, and exit.
/// </summary>
void DoHelpAndExit(string message)
{
    Console.WriteLine(message);
    Console.WriteLine();
    Console.WriteLine(
@"
-i  insert rows containing SHA1 and full path
-a  append to existing rows (sha1 and filename)

-h  help (this)
");
    Environment.Exit(1);
}

/// <summary>
/// database operation mode
/// </summary>
enum dbMode { unknown, insert, update };
